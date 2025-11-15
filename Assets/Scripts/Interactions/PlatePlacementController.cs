using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Handles plate placement interaction in AR space
/// </summary>
public class PlatePlacementController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float placementUpdateRate = 0.1f; // Update placement indicator every 0.1 seconds
    [SerializeField] private float minPlacementDistance = 0.3f; // Minimum distance from camera
    [SerializeField] private float maxPlacementDistance = 3f; // Maximum distance from camera

    private float lastUpdateTime;
    private bool isPlacingPlate = false;

    private void Update()
    {
        // Only update if in plate placement state
        if (GameStateManager.Instance == null || 
            !GameStateManager.Instance.IsState(GameState.PlatePlacement))
        {
            return;
        }

        // Update placement indicator at specified rate
        if (Time.time - lastUpdateTime >= placementUpdateRate)
        {
            UpdatePlatePlacement();
            lastUpdateTime = Time.time;
        }

        // Handle input for confirming placement
        HandlePlacementInput();
    }

    /// <summary>
    /// Updates the plate placement indicator based on AR raycast
    /// </summary>
    private void UpdatePlatePlacement()
    {
        if (ARManager.Instance == null || PlateManager.Instance == null)
        {
            return;
        }

        // Raycast from screen center
        if (ARManager.Instance.RaycastFromScreenCenter(out Vector3 hitPosition, out Quaternion hitRotation))
        {
            // Check distance constraints
            Camera arCamera = ARManager.Instance.GetARCamera();
            if (arCamera != null)
            {
                float distance = Vector3.Distance(arCamera.transform.position, hitPosition);
                
                if (distance >= minPlacementDistance && distance <= maxPlacementDistance)
                {
                    // Show placement indicator
                    PlateManager.Instance.ShowPlacementIndicator(hitPosition, hitRotation);
                }
                else
                {
                    PlateManager.Instance.HidePlacementIndicator();
                }
            }
        }
        else
        {
            PlateManager.Instance.HidePlacementIndicator();
        }
    }

    /// <summary>
    /// Handles input for confirming plate placement
    /// </summary>
    private void HandlePlacementInput()
    {
        // Example: Confirm on tap/click
        // You can modify this to use your input system
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ConfirmPlacement();
        }
    }

    /// <summary>
    /// Confirms the plate placement and spawns the plate
    /// </summary>
    public void ConfirmPlacement()
    {
        if (ARManager.Instance == null || PlateManager.Instance == null)
        {
            return;
        }

        // Get placement position
        if (ARManager.Instance.RaycastFromScreenCenter(out Vector3 hitPosition, out Quaternion hitRotation))
        {
            // Spawn plate
            GameObject plate = PlateManager.Instance.SpawnPlate(hitPosition, hitRotation);
            
            if (plate != null)
            {
                // Create AR anchor for persistent placement
                ARAnchor anchor = ARManager.Instance.CreateAnchor(hitPosition, hitRotation);
                if (anchor != null)
                {
                    plate.transform.SetParent(anchor.transform);
                }

                // Confirm placement
                PlateManager.Instance.ConfirmPlatePlacement();

                // Move to confirmation state
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ChangeState(GameState.PlateConfirmation);
                }
            }
        }
    }

    /// <summary>
    /// Called when entering plate placement state
    /// </summary>
    public void OnEnterPlatePlacement()
    {
        isPlacingPlate = true;
        lastUpdateTime = Time.time;
    }

    /// <summary>
    /// Called when exiting plate placement state
    /// </summary>
    public void OnExitPlatePlacement()
    {
        isPlacingPlate = false;
        if (PlateManager.Instance != null)
        {
            PlateManager.Instance.HidePlacementIndicator();
        }
    }
}

