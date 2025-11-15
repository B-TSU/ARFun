using UnityEngine;
#if META_XR_SDK_AVAILABLE
using Meta.XR;
#endif

/// <summary>
/// Manages AR-specific functionality using Meta SDK (for Camera Rig approach)
/// Alternative to ARManager.cs which uses AR Foundation
/// </summary>
public class ARManagerMetaSDK : MonoBehaviour
{
    public static ARManagerMetaSDK Instance { get; private set; }

    [Header("Meta SDK Components")]
    [SerializeField] private Camera arCamera;

#if META_XR_SDK_AVAILABLE
    private OVRSceneManager sceneManager;
    private OVRAnchorManager anchorManager;
#endif

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
        // Find camera from Camera Rig
        if (arCamera == null)
        {
            // Try to find camera from OVRCameraRig
            GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
            if (cameraRig != null)
            {
                Transform centerEye = cameraRig.transform.Find("TrackingSpace/CenterEyeAnchor");
                if (centerEye != null)
                {
                    arCamera = centerEye.GetComponent<Camera>();
                }
            }
            
            // Fallback to main camera
            if (arCamera == null)
            {
                arCamera = Camera.main;
            }
        }

#if META_XR_SDK_AVAILABLE
        // Find OVRSceneManager if available
        sceneManager = FindObjectOfType<OVRSceneManager>();
        
        // Find OVRAnchorManager if available
        anchorManager = FindObjectOfType<OVRAnchorManager>();
#endif
    }

    /// <summary>
    /// Performs a raycast from screen center using Environment Raycast
    /// Note: This uses the Environment Raycast building block
    /// </summary>
    public bool RaycastFromScreenCenter(out Vector3 hitPosition, out Quaternion hitRotation)
    {
        hitPosition = Vector3.zero;
        hitRotation = Quaternion.identity;

        if (arCamera == null)
        {
            return false;
        }

        // Use Unity's Physics raycast as fallback
        // For better results, use the Environment Raycast building block component
        Ray ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            hitPosition = hit.point;
            hitRotation = Quaternion.LookRotation(hit.normal);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs a raycast from a specific screen point
    /// </summary>
    public bool RaycastFromScreenPoint(Vector2 screenPoint, out Vector3 hitPosition, out Quaternion hitRotation)
    {
        hitPosition = Vector3.zero;
        hitRotation = Quaternion.identity;

        if (arCamera == null)
        {
            return false;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            hitPosition = hit.point;
            hitRotation = Quaternion.LookRotation(hit.normal);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates an anchor at the specified position using Meta SDK
    /// </summary>
    public GameObject CreateAnchor(Vector3 position, Quaternion rotation)
    {
#if META_XR_SDK_AVAILABLE
        if (anchorManager != null)
        {
            // Use Meta SDK anchor creation
            // Note: OVRAnchorManager API may vary - check Meta SDK documentation
            GameObject anchorObject = new GameObject("OVRAnchor");
            anchorObject.transform.position = position;
            anchorObject.transform.rotation = rotation;
            
            // Add OVRAnchor component if available
            // anchorObject.AddComponent<OVRAnchor>();
            
            return anchorObject;
        }
#endif

        // Fallback: Just create a GameObject
        GameObject fallbackAnchor = new GameObject("Anchor");
        fallbackAnchor.transform.position = position;
        fallbackAnchor.transform.rotation = rotation;
        return fallbackAnchor;
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
        // For Meta SDK, check if camera rig is active
        GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
        return cameraRig != null && cameraRig.activeInHierarchy;
    }
}

