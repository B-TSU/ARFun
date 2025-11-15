using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages AR-specific functionality including plane detection and raycasting
/// </summary>
public class ARManager : MonoBehaviour
{
    public static ARManager Instance { get; private set; }

    [Header("AR Components")]
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private ARAnchorManager arAnchorManager;
    [SerializeField] private Camera arCamera;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask raycastLayerMask = 1 << 0; // Default layer

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

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
        // Find AR components if not assigned
        if (arRaycastManager == null)
        {
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
        }

        if (arPlaneManager == null)
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
        }

        if (arAnchorManager == null)
        {
            arAnchorManager = FindObjectOfType<ARAnchorManager>();
        }

        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    /// <summary>
    /// Performs a raycast from screen center to detect AR planes
    /// </summary>
    public bool RaycastFromScreenCenter(out Vector3 hitPosition, out Quaternion hitRotation)
    {
        hitPosition = Vector3.zero;
        hitRotation = Quaternion.identity;

        if (arRaycastManager == null || arCamera == null)
        {
            return false;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return RaycastFromScreenPoint(screenCenter, out hitPosition, out hitRotation);
    }

    /// <summary>
    /// Performs a raycast from a specific screen point
    /// </summary>
    public bool RaycastFromScreenPoint(Vector2 screenPoint, out Vector3 hitPosition, out Quaternion hitRotation)
    {
        hitPosition = Vector3.zero;
        hitRotation = Quaternion.identity;

        if (arRaycastManager == null)
        {
            return false;
        }

        hits.Clear();
        if (arRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            hitPosition = hit.pose.position;
            hitRotation = hit.pose.rotation;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates an AR anchor at the specified position
    /// </summary>
    public ARAnchor CreateAnchor(Vector3 position, Quaternion rotation)
    {
        if (arAnchorManager == null)
        {
            Debug.LogError("ARAnchorManager not found!");
            return null;
        }

        // Create a new GameObject for the anchor
        GameObject anchorObject = new GameObject("ARAnchor");
        anchorObject.transform.position = position;
        anchorObject.transform.rotation = rotation;
        
        // Add ARAnchor component (this will automatically be tracked by ARAnchorManager)
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();
        
        return anchor;
    }

    /// <summary>
    /// Enables or disables plane detection
    /// </summary>
    public void SetPlaneDetectionEnabled(bool enabled)
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = enabled;
            
            // Enable/disable all existing planes
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(enabled);
            }
        }
    }

    /// <summary>
    /// Gets the AR camera
    /// </summary>
    public Camera GetARCamera()
    {
        return arCamera;
    }

    /// <summary>
    /// Checks if AR is currently tracking
    /// </summary>
    public bool IsARTracking()
    {
        return arPlaneManager != null && arPlaneManager.subsystem != null && 
               arPlaneManager.subsystem.running;
    }
}

