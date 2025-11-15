using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages flower trimming functionality including mesh cutting
/// </summary>
public class TrimmingManager : MonoBehaviour
{
    public static TrimmingManager Instance { get; private set; }

    [Header("Trimming Settings")]
    [SerializeField] private LayerMask flowerLayerMask;
    [SerializeField] private float cutPlaneThickness = 0.1f;
    [SerializeField] private Material cutMaterial;

    [Header("Scissor Tool")]
    [SerializeField] private GameObject scissorToolPrefab;
    [SerializeField] private Transform scissorToolParent;

    private GameObject currentScissorTool;
    private GameObject selectedFlower;
    private bool isTrimmingMode = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Enters trimming mode for a specific flower
    /// </summary>
    public void EnterTrimmingMode(GameObject flower)
    {
        selectedFlower = flower;
        isTrimmingMode = true;

        // Spawn scissor tool if needed
        if (currentScissorTool == null && scissorToolPrefab != null)
        {
            currentScissorTool = Instantiate(scissorToolPrefab, scissorToolParent);
        }

        if (currentScissorTool != null)
        {
            currentScissorTool.SetActive(true);
        }

        Debug.Log($"Entered trimming mode for flower: {flower.name}");
    }

    /// <summary>
    /// Exits trimming mode
    /// </summary>
    public void ExitTrimmingMode()
    {
        isTrimmingMode = false;
        selectedFlower = null;

        if (currentScissorTool != null)
        {
            currentScissorTool.SetActive(false);
        }
    }

    /// <summary>
    /// Performs a cut on the selected flower at the given position and direction
    /// </summary>
    public void PerformCut(Vector3 cutPosition, Vector3 cutNormal)
    {
        if (!isTrimmingMode || selectedFlower == null)
        {
            Debug.LogWarning("Cannot perform cut: Not in trimming mode or no flower selected");
            return;
        }

        MeshFilter meshFilter = selectedFlower.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogError("Selected flower does not have a valid mesh!");
            return;
        }

        // Perform mesh cutting
        CutMesh(meshFilter, cutPosition, cutNormal);
    }

    /// <summary>
    /// Cuts a mesh using a plane defined by position and normal
    /// </summary>
    private void CutMesh(MeshFilter meshFilter, Vector3 planePoint, Vector3 planeNormal)
    {
        Mesh originalMesh = meshFilter.mesh;
        Plane cutPlane = new Plane(planeNormal, planePoint);

        // This is a simplified version - you may want to use a more robust mesh cutting library
        // For now, this demonstrates the concept
        Debug.Log($"Cutting mesh at position: {planePoint} with normal: {planeNormal}");

        // TODO: Implement actual mesh cutting algorithm
        // This would involve:
        // 1. Finding all edges that intersect the cutting plane
        // 2. Creating new vertices at intersection points
        // 3. Splitting the mesh into two parts
        // 4. Capping the cut surfaces
        // 5. Updating the mesh

        // For a production implementation, consider using:
        // - MeshCut library
        // - EzySlice library
        // - Or implement a custom solution based on mesh triangulation
    }

    /// <summary>
    /// Checks if trimming mode is active
    /// </summary>
    public bool IsTrimmingMode()
    {
        return isTrimmingMode;
    }

    /// <summary>
    /// Gets the currently selected flower for trimming
    /// </summary>
    public GameObject GetSelectedFlower()
    {
        return selectedFlower;
    }
}

