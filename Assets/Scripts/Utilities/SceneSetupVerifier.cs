using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Utility script to verify that all required scene building blocks are set up correctly.
/// Attach this to any GameObject and click "Verify Setup" in the inspector.
/// </summary>
public class SceneSetupVerifier : MonoBehaviour
{
    [Header("Verification")]
    [SerializeField] private bool verifyOnStart = true;

    private void Start()
    {
        if (verifyOnStart)
        {
            VerifySetup();
        }
    }

    [ContextMenu("Verify Setup")]
    public void VerifySetup()
    {
        Debug.Log("=== AR Ikebana Scene Setup Verification ===\n");
        
        bool allGood = true;

        // Check XR Origin
        allGood &= VerifyXROrigin();
        
        // Check Managers
        allGood &= VerifyManagers();
        
        // Check AR Content
        allGood &= VerifyARContent();
        
        // Check UI Canvas
        allGood &= VerifyUICanvas();
        
        // Check World Space Canvas
        allGood &= VerifyWorldSpaceCanvas();
        
        // Check Lighting
        allGood &= VerifyLighting();

        if (allGood)
        {
            Debug.Log("✓ All scene building blocks are properly set up!");
        }
        else
        {
            Debug.LogWarning("⚠ Some components are missing or misconfigured. Check the logs above.");
        }
    }

    private bool VerifyXROrigin()
    {
        Debug.Log("Checking XR Origin...");
        bool isValid = true;

        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            Debug.LogError("✗ XR Origin not found! Create it via: XR → XR Origin (AR Foundation)");
            return false;
        }

        // Check AR Components
        ARPlaneManager planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("✗ ARPlaneManager not found on XR Origin!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ ARPlaneManager found");
        }

        ARRaycastManager raycastManager = xrOrigin.GetComponent<ARRaycastManager>();
        if (raycastManager == null)
        {
            Debug.LogError("✗ ARRaycastManager not found on XR Origin!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ ARRaycastManager found");
        }

        ARAnchorManager anchorManager = xrOrigin.GetComponent<ARAnchorManager>();
        if (anchorManager == null)
        {
            Debug.LogError("✗ ARAnchorManager not found on XR Origin!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ ARAnchorManager found");
        }

        // Check Main Camera
        Camera mainCamera = xrOrigin.GetComponentInChildren<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("✗ Main Camera not found as child of XR Origin!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ Main Camera found");
        }

        return isValid;
    }

    private bool VerifyManagers()
    {
        Debug.Log("\nChecking Managers...");
        bool isValid = true;

        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            Debug.LogError("✗ Managers GameObject not found!");
            return false;
        }

        // Check each manager script
        string[] managerNames = {
            "GameStateManager",
            "UIManager",
            "ARManager",
            "PlateManager",
            "FlowerManager",
            "TrimmingManager"
        };

        foreach (string managerName in managerNames)
        {
            Component manager = managers.GetComponent(managerName);
            if (manager == null)
            {
                Debug.LogError($"✗ {managerName} not found on Managers GameObject!");
                isValid = false;
            }
            else
            {
                Debug.Log($"✓ {managerName} found");
            }
        }

        return isValid;
    }

    private bool VerifyARContent()
    {
        Debug.Log("\nChecking AR Content...");
        bool isValid = true;

        GameObject arContent = GameObject.Find("AR Content");
        if (arContent == null)
        {
            Debug.LogError("✗ AR Content GameObject not found!");
            return false;
        }

        Transform plateAnchor = arContent.transform.Find("PlateAnchor");
        if (plateAnchor == null)
        {
            Debug.LogError("✗ PlateAnchor not found under AR Content!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ PlateAnchor found");
        }

        Transform flowerContainer = arContent.transform.Find("FlowerContainer");
        if (flowerContainer == null)
        {
            Debug.LogError("✗ FlowerContainer not found under AR Content!");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ FlowerContainer found");
        }

        return isValid;
    }

    private bool VerifyUICanvas()
    {
        Debug.Log("\nChecking UI Canvas...");
        bool isValid = true;

        Canvas uiCanvas = GameObject.FindObjectOfType<Canvas>();
        if (uiCanvas == null || uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("✗ UI Canvas (Screen Space Overlay) not found!");
            return false;
        }

        string[] requiredPanels = {
            "MainMenuPanel",
            "PlateSelectionPanel",
            "PlatePlacementPanel",
            "FlowerSelectionPanel",
            "TrimmingPanel",
            "AIAssistantPanel",
            "ScreenshotPanel"
        };

        foreach (string panelName in requiredPanels)
        {
            Transform panel = uiCanvas.transform.Find(panelName);
            if (panel == null)
            {
                Debug.LogError($"✗ {panelName} not found in UI Canvas!");
                isValid = false;
            }
            else
            {
                Debug.Log($"✓ {panelName} found");
            }
        }

        return isValid;
    }

    private bool VerifyWorldSpaceCanvas()
    {
        Debug.Log("\nChecking World Space Canvas...");
        
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        bool foundWorldSpace = false;

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                foundWorldSpace = true;
                Debug.Log("✓ World Space Canvas found");
                break;
            }
        }

        if (!foundWorldSpace)
        {
            Debug.LogWarning("⚠ World Space Canvas not found (optional but recommended)");
        }

        return true; // Not critical, so return true
    }

    private bool VerifyLighting()
    {
        Debug.Log("\nChecking Lighting...");
        
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        bool foundDirectional = false;

        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                foundDirectional = true;
                Debug.Log("✓ Directional Light found");
                break;
            }
        }

        if (!foundDirectional)
        {
            Debug.LogWarning("⚠ Directional Light not found (recommended)");
        }

        return true; // Not critical, so return true
    }
}

