using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages flower selection, spawning, and arrangement in AR space
/// </summary>
public class FlowerManager : MonoBehaviour
{
    public static FlowerManager Instance { get; private set; }

    [Header("Flower Prefabs")]
    [SerializeField] private List<GameObject> flowerPrefabs = new List<GameObject>();

    [Header("Flower Container")]
    [SerializeField] private Transform flowerContainer;

    private List<GameObject> spawnedFlowers = new List<GameObject>();
    private int selectedFlowerIndex = -1;

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

    private void Start()
    {
        // Create flower container if it doesn't exist
        if (flowerContainer == null)
        {
            GameObject container = new GameObject("FlowerContainer");
            container.transform.SetParent(transform);
            flowerContainer = container.transform;
        }
    }

    /// <summary>
    /// Selects a flower by index
    /// </summary>
    public void SelectFlower(int flowerIndex)
    {
        if (flowerIndex >= 0 && flowerIndex < flowerPrefabs.Count)
        {
            selectedFlowerIndex = flowerIndex;
            Debug.Log($"Flower {flowerIndex} selected");
        }
    }

    /// <summary>
    /// Spawns the selected flower at the given position
    /// </summary>
    public GameObject SpawnFlower(Vector3 position, Quaternion rotation)
    {
        if (selectedFlowerIndex < 0 || selectedFlowerIndex >= flowerPrefabs.Count)
        {
            Debug.LogError("No flower selected!");
            return null;
        }

        GameObject flower = Instantiate(
            flowerPrefabs[selectedFlowerIndex], 
            position, 
            rotation, 
            flowerContainer
        );

        // Set flower ID and tag for identification
        FlowerData flowerData = flower.GetComponent<FlowerData>();
        if (flowerData != null)
        {
            flowerData.SetFlowerID(selectedFlowerIndex);
        }
        
        // Set tag if not already set
        if (string.IsNullOrEmpty(flower.tag) || flower.tag == "Untagged")
        {
            flower.tag = "Flower";
        }

        spawnedFlowers.Add(flower);
        return flower;
    }

    /// <summary>
    /// Gets all spawned flowers
    /// </summary>
    public List<GameObject> GetSpawnedFlowers()
    {
        return spawnedFlowers;
    }

    /// <summary>
    /// Removes a specific flower
    /// </summary>
    public void RemoveFlower(GameObject flower)
    {
        if (spawnedFlowers.Contains(flower))
        {
            spawnedFlowers.Remove(flower);
            Destroy(flower);
        }
    }

    /// <summary>
    /// Clears all spawned flowers
    /// </summary>
    public void ClearAllFlowers()
    {
        foreach (GameObject flower in spawnedFlowers)
        {
            if (flower != null)
            {
                Destroy(flower);
            }
        }
        spawnedFlowers.Clear();
    }

    /// <summary>
    /// Gets the number of available flower types
    /// </summary>
    public int GetFlowerCount()
    {
        return flowerPrefabs.Count;
    }

    /// <summary>
    /// Spawns a flower on the plate at the specified local position
    /// </summary>
    /// <param name="plate">The plate Transform to place the flower on</param>
    /// <param name="localPosition">Local position relative to plate (use Vector3.zero for center)</param>
    public GameObject SpawnFlowerOnPlate(Transform plate, Vector3 localPosition)
    {
        if (plate == null)
        {
            Debug.LogError("Cannot spawn flower: Plate is null!");
            return null;
        }

        if (selectedFlowerIndex < 0 || selectedFlowerIndex >= flowerPrefabs.Count)
        {
            Debug.LogError("No flower selected!");
            return null;
        }

        // Convert local position to world position for initial spawn
        Vector3 worldPosition = plate.TransformPoint(localPosition);
        Quaternion worldRotation = plate.rotation;

        // Spawn flower
        GameObject flower = Instantiate(
            flowerPrefabs[selectedFlowerIndex],
            worldPosition,
            worldRotation,
            flowerContainer
        );

        spawnedFlowers.Add(flower);

        // Set flower ID and tag for identification
        FlowerData flowerData = flower.GetComponent<FlowerData>();
        if (flowerData != null)
        {
            flowerData.SetFlowerID(selectedFlowerIndex);
        }
        
        // Set tag if not already set
        if (string.IsNullOrEmpty(flower.tag) || flower.tag == "Untagged")
        {
            flower.tag = "Flower";
        }

        // Try to place on plate using FlowerPlacement component
        FlowerPlacement placement = flower.GetComponent<FlowerPlacement>();
        if (placement != null)
        {
            placement.PlaceOnPlate(plate, localPosition);
        }
        else
        {
            // If no FlowerPlacement component, manually parent to plate
            flower.transform.SetParent(plate);
            flower.transform.localPosition = localPosition;
            flower.transform.localRotation = Quaternion.identity;
        }

        return flower;
    }

    /// <summary>
    /// Gets all flowers of a specific type
    /// </summary>
    public List<GameObject> GetFlowersByType(string flowerType)
    {
        List<GameObject> flowersOfType = new List<GameObject>();
        
        foreach (GameObject flower in spawnedFlowers)
        {
            if (flower == null) continue;
            
            FlowerData data = flower.GetComponent<FlowerData>();
            if (data != null && data.GetFlowerType() == flowerType)
            {
                flowersOfType.Add(flower);
            }
        }
        
        return flowersOfType;
    }

    /// <summary>
    /// Gets all flowers with a specific tag
    /// </summary>
    public List<GameObject> GetFlowersByTag(string tag)
    {
        List<GameObject> flowersWithTag = new List<GameObject>();
        
        foreach (GameObject flower in spawnedFlowers)
        {
            if (flower != null && flower.CompareTag(tag))
            {
                flowersWithTag.Add(flower);
            }
        }
        
        return flowersWithTag;
    }

    /// <summary>
    /// Gets flower data for a specific flower
    /// </summary>
    public FlowerData GetFlowerData(GameObject flower)
    {
        if (flower == null) return null;
        return flower.GetComponent<FlowerData>();
    }
}

