using UnityEngine;

/// <summary>
/// Handles flower placement on plate surface with constraints
/// </summary>
public class FlowerPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private float plateSurfaceOffset = 0.01f; // Offset above plate surface
    [SerializeField] private LayerMask plateLayer = 1 << 8; // Plate layer (adjust as needed)
    [SerializeField] private bool snapToPlate = true; // Automatically snap to plate when placed
    
    [Header("Constraints")]
    [SerializeField] private bool constrainToPlate = true; // Keep flower on plate
    [SerializeField] private float maxDistanceFromPlate = 0.5f; // Max distance before resetting
    
    private bool isPlaced = false;
    private Transform plateTransform;
    private Vector3 localPlacementPosition;
    
    /// <summary>
    /// Places the flower on the plate surface
    /// </summary>
    /// <param name="plate">The plate Transform to place the flower on</param>
    /// <param name="localPosition">Local position relative to plate (use Vector3.zero for center)</param>
    public void PlaceOnPlate(Transform plate, Vector3 localPosition)
    {
        if (plate == null)
        {
            Debug.LogWarning("Cannot place flower: Plate is null");
            return;
        }
        
        plateTransform = plate;
        localPlacementPosition = localPosition;
        
        // Set parent to plate
        transform.SetParent(plateTransform);
        
        // Set local position (relative to plate)
        Vector3 finalPosition = localPosition + Vector3.up * plateSurfaceOffset;
        transform.localPosition = finalPosition;
        
        // Keep upright rotation (or match plate rotation)
        transform.localRotation = Quaternion.identity;
        
        isPlaced = true;
        
        // Disable physics movement if kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.isKinematic)
        {
            // Already kinematic, good for placement
        }
        
        Debug.Log($"Flower placed on plate at local position: {localPosition}");
    }
    
    /// <summary>
    /// Places the flower at a world position, snapping to plate if nearby
    /// </summary>
    public void PlaceAtPosition(Vector3 worldPosition, Quaternion rotation)
    {
        // Check if position is near a plate
        if (snapToPlate)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(worldPosition, 0.2f, plateLayer);
            
            if (nearbyColliders.Length > 0)
            {
                // Find the closest plate
                Transform closestPlate = null;
                float closestDistance = float.MaxValue;
                
                foreach (Collider col in nearbyColliders)
                {
                    float distance = Vector3.Distance(worldPosition, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlate = col.transform;
                    }
                }
                
                if (closestPlate != null)
                {
                    // Convert world position to local position relative to plate
                    Vector3 localPos = closestPlate.InverseTransformPoint(worldPosition);
                    localPos.y = plateSurfaceOffset; // Snap to plate surface
                    
                    PlaceOnPlate(closestPlate, localPos);
                    return;
                }
            }
        }
        
        // No plate found, place at world position
        transform.position = worldPosition;
        transform.rotation = rotation;
        isPlaced = false;
    }
    
    /// <summary>
    /// Checks if flower is placed on a plate
    /// </summary>
    public bool IsPlaced()
    {
        return isPlaced && plateTransform != null;
    }
    
    /// <summary>
    /// Gets the plate this flower is placed on
    /// </summary>
    public Transform GetPlate()
    {
        return plateTransform;
    }
    
    /// <summary>
    /// Removes flower from plate
    /// </summary>
    public void RemoveFromPlate()
    {
        if (isPlaced)
        {
            transform.SetParent(null);
            plateTransform = null;
            isPlaced = false;
        }
    }
    
    private void Update()
    {
        // Constrain flower to plate if enabled
        if (constrainToPlate && isPlaced && plateTransform != null)
        {
            // Check if flower has moved too far from plate
            float distanceFromPlate = Vector3.Distance(
                transform.position, 
                plateTransform.TransformPoint(localPlacementPosition)
            );
            
            if (distanceFromPlate > maxDistanceFromPlate)
            {
                // Reset position to plate
                transform.localPosition = localPlacementPosition + Vector3.up * plateSurfaceOffset;
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize placement offset
        if (isPlaced && plateTransform != null)
        {
            Vector3 plateSurfacePos = plateTransform.TransformPoint(localPlacementPosition);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(plateSurfacePos, 0.05f);
            
            Vector3 flowerPos = plateTransform.TransformPoint(localPlacementPosition + Vector3.up * plateSurfaceOffset);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(plateSurfacePos, flowerPos);
            Gizmos.DrawWireSphere(flowerPos, 0.03f);
        }
    }
}

