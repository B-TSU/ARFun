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
}

