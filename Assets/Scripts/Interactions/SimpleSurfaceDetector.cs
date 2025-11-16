using UnityEngine;
using UnityEngine.UI;
#if META_XR_SDK_AVAILABLE
using Meta.XR;
#endif

/// <summary>
/// Simple surface detection script - displays text when a surface is detected
/// </summary>
public class SimpleSurfaceDetector : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera arCamera;
    
    public enum EyeAnchor
    {
        Center,  // Center eye (recommended for gaze detection)
        Left,    // Left eye
        Right    // Right eye
    }
    [SerializeField] private EyeAnchor eyeAnchorToUse = EyeAnchor.Center;

    [Header("Detection Settings")]
    [SerializeField] private float maxDistance = 5f; // Maximum distance to detect surfaces
    [SerializeField] private LayerMask surfaceLayerMask = -1; // Layers to detect

    [Header("UI Display")]
    [SerializeField] private Text displayText; // Text UI to show detection status
    [SerializeField] private string surfaceDetectedText = "Surface Detected!";
    [SerializeField] private string noSurfaceText = "No Surface";

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true; // Show ray in Scene view
    [SerializeField] private Color rayColor = Color.green;

    // Meta Building Blocks references
#if META_XR_SDK_AVAILABLE
    private EnvironmentRaycastManager environmentRaycastManager;
    private OVRSceneManager sceneManager;
#endif

    private void Start()
    {
        // Find camera if not assigned
        if (arCamera == null)
        {
            // Try to find camera from Meta Building Block Camera Rig
            GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
            if (cameraRig != null)
            {
                Transform trackingSpace = cameraRig.transform.Find("TrackingSpace");
                if (trackingSpace != null)
                {
                    Transform eyeAnchor = null;
                    
                    // Find the selected eye anchor
                    switch (eyeAnchorToUse)
                    {
                        case EyeAnchor.Center:
                            eyeAnchor = trackingSpace.Find("CenterEyeAnchor");
                            break;
                        case EyeAnchor.Left:
                            eyeAnchor = trackingSpace.Find("LeftEyeAnchor");
                            break;
                        case EyeAnchor.Right:
                            eyeAnchor = trackingSpace.Find("RightEyeAnchor");
                            break;
                    }
                    
                    if (eyeAnchor != null)
                    {
                        arCamera = eyeAnchor.GetComponent<Camera>();
                        Debug.Log($"Using {eyeAnchorToUse} Eye Anchor for surface detection");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find {eyeAnchorToUse} Eye Anchor, trying CenterEyeAnchor");
                        // Fallback to center eye
                        eyeAnchor = trackingSpace.Find("CenterEyeAnchor");
                        if (eyeAnchor != null)
                        {
                            arCamera = eyeAnchor.GetComponent<Camera>();
                        }
                    }
                }
            }

            // Fallback to main camera
            if (arCamera == null)
            {
                arCamera = Camera.main;
                Debug.LogWarning("Using Main Camera as fallback");
            }
        }

        // Find Meta Building Blocks components
#if META_XR_SDK_AVAILABLE
        environmentRaycastManager = FindObjectOfType<EnvironmentRaycastManager>();
        sceneManager = FindObjectOfType<OVRSceneManager>();
        
        if (environmentRaycastManager != null)
        {
            Debug.Log("Meta Environment Raycast Manager found!");
        }
        if (sceneManager != null)
        {
            Debug.Log("OVR Scene Manager found!");
        }
#endif

        // Create text UI if not assigned
        if (displayText == null)
        {
            CreateTextUI();
        }
    }

    private void Update()
    {
        if (arCamera == null)
        {
            UpdateText("Camera Not Found!");
            return;
        }

        bool hitSurface = false;
        Vector3 hitPosition = Vector3.zero;
        float hitDistance = 0f;
        string hitObjectName = "";

        // Try Meta Building Blocks surface detection first
#if META_XR_SDK_AVAILABLE
        // Method 1: Use Meta Environment Raycast (detects real-world surfaces)
        if (environmentRaycastManager != null)
        {
            Vector3 rayOrigin = arCamera.transform.position;
            Vector3 rayDirection = arCamera.transform.forward;
            
            // Meta's Environment Raycast detects real-world surfaces
            // It uses depth sensing to detect tables, floors, walls, etc.
            RaycastHit envHit;
            if (Physics.Raycast(rayOrigin, rayDirection, out envHit, maxDistance, surfaceLayerMask))
            {
                hitSurface = true;
                hitPosition = envHit.point;
                hitDistance = envHit.distance;
                hitObjectName = envHit.collider.name;
            }
        }

        // Method 2: Use OVRSceneManager (scene understanding - detects planes)
        if (!hitSurface && sceneManager != null)
        {
            // OVRSceneManager creates GameObjects for detected real-world planes
            // These have colliders that represent tables, floors, etc.
            Vector3 rayOrigin = arCamera.transform.position;
            Vector3 rayDirection = arCamera.transform.forward;
            RaycastHit sceneHit;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out sceneHit, maxDistance, surfaceLayerMask))
            {
                hitSurface = true;
                hitPosition = sceneHit.point;
                hitDistance = sceneHit.distance;
                hitObjectName = sceneHit.collider.name;
            }
        }
#endif

        // Fallback: Standard Physics raycast (for virtual objects with colliders)
        if (!hitSurface)
        {
            Vector3 rayOrigin = arCamera.transform.position;
            Vector3 rayDirection = arCamera.transform.forward;
            RaycastHit hit;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance, surfaceLayerMask))
            {
                hitSurface = true;
                hitPosition = hit.point;
                hitDistance = hit.distance;
                hitObjectName = hit.collider.name;
            }
        }

        // Update display
        if (hitSurface)
        {
            string info = $"{surfaceDetectedText}\nDistance: {hitDistance:F2}m\nObject: {hitObjectName}";
            UpdateText(info);
        }
        else
        {
            UpdateText(noSurfaceText);
        }
    }

    /// <summary>
    /// Updates the display text
    /// </summary>
    private void UpdateText(string text)
    {
        if (displayText != null)
        {
            displayText.text = text;
        }
        else
        {
            Debug.Log($"Surface Detection: {text}");
        }
    }

    /// <summary>
    /// Creates a simple text UI if none is assigned
    /// </summary>
    private void CreateTextUI()
    {
        // Create Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Position canvas in front of camera
            canvasObj.transform.SetParent(arCamera.transform);
            canvasObj.transform.localPosition = new Vector3(0, 0, 2f);
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }

        // Create Text GameObject
        GameObject textObj = new GameObject("SurfaceDetectionText");
        textObj.transform.SetParent(canvas.transform, false);

        displayText = textObj.AddComponent<Text>();
        displayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        displayText.fontSize = 24;
        displayText.color = Color.white;
        displayText.alignment = TextAnchor.MiddleCenter;
        displayText.text = noSurfaceText;

        // Position text in center of canvas
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(400, 100);
    }

    /// <summary>
    /// Draw debug ray in Scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (showDebugRay && arCamera != null)
        {
            Gizmos.color = rayColor;
            Vector3 rayOrigin = arCamera.transform.position;
            Vector3 rayEnd = rayOrigin + arCamera.transform.forward * maxDistance;
            Gizmos.DrawLine(rayOrigin, rayEnd);
        }
    }
}

