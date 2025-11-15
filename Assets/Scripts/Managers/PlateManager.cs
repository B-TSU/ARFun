using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages plate selection, spawning, and placement in AR space
/// </summary>
public class PlateManager : MonoBehaviour
{
    public static PlateManager Instance { get; private set; }

    [Header("Plate Prefabs")]
    [SerializeField] private List<GameObject> platePrefabs = new List<GameObject>();

    [Header("AR Placement")]
    [SerializeField] private Transform plateAnchor;
    [SerializeField] private GameObject placementIndicatorPrefab;
    [SerializeField] private LayerMask placementLayerMask = 1 << 0; // Default layer

    private GameObject currentPlate;
    private GameObject placementIndicator;
    private int selectedPlateIndex = -1;

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
    /// Selects a plate by index
    /// </summary>
    public void SelectPlate(int plateIndex)
    {
        if (plateIndex >= 0 && plateIndex < platePrefabs.Count)
        {
            selectedPlateIndex = plateIndex;
            Debug.Log($"Plate {plateIndex} selected");
        }
    }

    /// <summary>
    /// Spawns the selected plate at the given position and rotation
    /// </summary>
    public GameObject SpawnPlate(Vector3 position, Quaternion rotation)
    {
        if (selectedPlateIndex < 0 || selectedPlateIndex >= platePrefabs.Count)
        {
            Debug.LogError("No plate selected!");
            return null;
        }

        // Destroy existing plate if any
        if (currentPlate != null)
        {
            Destroy(currentPlate);
        }

        // Spawn new plate
        currentPlate = Instantiate(platePrefabs[selectedPlateIndex], position, rotation, plateAnchor);
        
        return currentPlate;
    }

    /// <summary>
    /// Gets the current plate GameObject
    /// </summary>
    public GameObject GetCurrentPlate()
    {
        return currentPlate;
    }

    /// <summary>
    /// Shows placement indicator at the given position
    /// </summary>
    public void ShowPlacementIndicator(Vector3 position, Quaternion rotation)
    {
        if (placementIndicatorPrefab == null) return;

        if (placementIndicator == null)
        {
            placementIndicator = Instantiate(placementIndicatorPrefab, position, rotation);
        }
        else
        {
            placementIndicator.transform.position = position;
            placementIndicator.transform.rotation = rotation;
            placementIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the placement indicator
    /// </summary>
    public void HidePlacementIndicator()
    {
        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Confirms plate placement and anchors it
    /// </summary>
    public void ConfirmPlatePlacement()
    {
        if (currentPlate != null)
        {
            HidePlacementIndicator();
            Debug.Log("Plate placement confirmed");
        }
    }

    /// <summary>
    /// Resets plate selection and removes current plate
    /// </summary>
    public void ResetPlate()
    {
        if (currentPlate != null)
        {
            Destroy(currentPlate);
            currentPlate = null;
        }
        selectedPlateIndex = -1;
        HidePlacementIndicator();
    }
}

