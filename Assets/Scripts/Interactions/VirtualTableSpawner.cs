using UnityEngine;
#if META_XR_SDK_AVAILABLE
using Meta.XR;
#endif
using UnityEngine.XR;
using UnityEngine.InputSystem;

/// <summary>
/// Spawns a virtual table for testing surface detection
/// </summary>
public class VirtualTableSpawner : MonoBehaviour
{
    [Header("Table Settings")]
    [SerializeField] private GameObject tablePrefab; // Optional: Use a prefab instead of creating a cube
    [SerializeField] private Vector3 tablePosition = new Vector3(0, 0.75f, 1.5f); // Position in front of camera (only used if spawnFromController is false)
    [SerializeField] private Vector3 tableSize = new Vector3(1f, 0.05f, 1f); // Width, Height, Depth (only used if no prefab)
    [SerializeField] private Material tableMaterial; // Optional material (only used if no prefab)
    [SerializeField] private bool spawnFromController = true; // Spawn from controller tip instead of fixed position
    [SerializeField] private float controllerTipOffset = 0.1f; // Distance from controller center to tip

    [Header("Spawn Settings")]
    [SerializeField] private KeyCode spawnKey = KeyCode.T; // Press T to spawn table (fallback for testing)
    [SerializeField] private bool spawnOnStart = false; // Spawn automatically on start
    [SerializeField] private bool useXRController = true; // Use XR controller trigger
    
    [Header("Confirmation Settings")]
    [SerializeField] private bool requireConfirmation = true; // Require A button press to confirm position
    [SerializeField] private KeyCode confirmKey = KeyCode.Space; // Keyboard key for confirmation (fallback)

    private GameObject virtualTable;
    private Transform rightControllerTransform;
    private bool isTableConfirmed = false; // Whether the table position has been confirmed

    private void Start()
    {
        // Find right controller transform
        FindRightController();

        if (spawnOnStart)
        {
            SpawnTable();
        }
    }

    /// <summary>
    /// Finds the right-hand controller transform
    /// </summary>
    private void FindRightController()
    {
        // Try to find controller from Meta Building Blocks
        GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (cameraRig != null)
        {
            // Look for right controller anchor
            Transform trackingSpace = cameraRig.transform.Find("TrackingSpace");
            if (trackingSpace != null)
            {
                // Try different possible names for controller anchors
                rightControllerTransform = trackingSpace.Find("RightHandAnchor");
                if (rightControllerTransform == null)
                {
                    rightControllerTransform = trackingSpace.Find("RightControllerAnchor");
                }
                if (rightControllerTransform == null)
                {
                    rightControllerTransform = trackingSpace.Find("RightHand");
                }
            }
        }

        // Fallback: Try to find via XR Input
        if (rightControllerTransform == null)
        {
            // This will be updated in Update() if found
        }
    }

    private void Update()
    {
        // Try to find controller transform if not found yet
        if (rightControllerTransform == null)
        {
            FindRightController();
        }

        bool triggerPressed = false;

        // Check XR controller trigger (right hand)
        if (useXRController)
        {
#if META_XR_SDK_AVAILABLE
            // Try Meta's OVRInput first (if using Meta SDK)
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                triggerPressed = true;
            }
#endif

            // Fallback: Try Unity XR Input System
            if (!triggerPressed)
            {
                UnityEngine.XR.InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                if (rightController.isValid)
                {
                    rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool triggerValue);
                    if (triggerValue && !wasTriggerPressed)
                    {
                        triggerPressed = true;
                    }
                    wasTriggerPressed = triggerValue;
                }
            }
        }

        // Fallback: Keyboard input (for testing in editor) - using new Input System
        if (!triggerPressed)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                // Convert KeyCode to Key for new Input System
                Key key = Key.None;
                switch (spawnKey)
                {
                    case KeyCode.T: key = Key.T; break;
                    case KeyCode.Space: key = Key.Space; break;
                    case KeyCode.Return: key = Key.Enter; break;
                    case KeyCode.E: key = Key.E; break;
                    case KeyCode.F: key = Key.F; break;
                    default: key = Key.T; break; // Default to T
                }
                
                if (keyboard[key].wasPressedThisFrame)
                {
                    triggerPressed = true;
                }
            }
        }

        // Spawn/reposition table on trigger press (only if not confirmed)
        if (triggerPressed && !isTableConfirmed)
        {
            if (virtualTable == null)
            {
                // No table exists, spawn a new one
                SpawnTable();
                isTableConfirmed = false; // New table needs confirmation
            }
            else
            {
                // Table exists but not confirmed, reposition it
                RepositionTable();
            }
        }
        else if (triggerPressed && isTableConfirmed)
        {
            // Table is confirmed, trigger does nothing
            Debug.Log("Table position is confirmed. Cannot spawn/reposition.");
        }

        // Check for confirmation (A button on right controller)
        if (virtualTable != null && !isTableConfirmed && requireConfirmation)
        {
            bool confirmPressed = false;

            // Check XR controller A button (right hand)
            if (useXRController)
            {
#if META_XR_SDK_AVAILABLE
                // Try Meta's OVRInput first
                if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
                {
                    confirmPressed = true;
                }
#endif

                // Fallback: Try Unity XR Input System
                if (!confirmPressed)
                {
                    UnityEngine.XR.InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                    if (rightController.isValid)
                    {
                        // A button is typically the primary button
                        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool aButtonValue);
                        if (aButtonValue && !wasAButtonPressed)
                        {
                            confirmPressed = true;
                        }
                        wasAButtonPressed = aButtonValue;
                    }
                }
            }

            // Fallback: Keyboard input (for testing in editor)
            if (!confirmPressed)
            {
                Keyboard keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    Key key = Key.None;
                    switch (confirmKey)
                    {
                        case KeyCode.Space: key = Key.Space; break;
                        case KeyCode.Return: key = Key.Enter; break;
                        default: key = Key.Space; break;
                    }
                    
                    if (keyboard[key].wasPressedThisFrame)
                    {
                        confirmPressed = true;
                    }
                }
            }

            if (confirmPressed)
            {
                ConfirmTablePosition();
            }
        }
    }

    private bool wasTriggerPressed = false;
    private bool wasAButtonPressed = false;

    /// <summary>
    /// Gets the spawn position (either from controller tip or fixed position)
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        if (spawnFromController && rightControllerTransform != null)
        {
            // Spawn from controller tip (position + forward direction * offset)
            Vector3 tipPosition = rightControllerTransform.position + rightControllerTransform.forward * controllerTipOffset;
            return tipPosition;
        }
        else
        {
            // Use fixed position
            return tablePosition;
        }
    }

    /// <summary>
    /// Gets the spawn rotation (either from controller or default)
    /// </summary>
    private Quaternion GetSpawnRotation()
    {
        if (spawnFromController && rightControllerTransform != null)
        {
            // Use controller's rotation, but keep table horizontal
            Vector3 forward = rightControllerTransform.forward;
            forward.y = 0; // Keep horizontal
            if (forward.magnitude > 0.1f)
            {
                return Quaternion.LookRotation(forward, Vector3.up);
            }
        }
        return Quaternion.identity;
    }

    /// <summary>
    /// Spawns a virtual table with a collider
    /// </summary>
    public void SpawnTable()
    {
        // Destroy existing table if any
        if (virtualTable != null)
        {
            Destroy(virtualTable);
        }

        // Get spawn position and rotation
        Vector3 spawnPos = GetSpawnPosition();
        Quaternion spawnRot = GetSpawnRotation();

        // Use prefab if provided, otherwise create a simple cube
        if (tablePrefab != null)
        {
            // Spawn from prefab
            virtualTable = Instantiate(tablePrefab, spawnPos, spawnRot);
            virtualTable.name = "VirtualTable";
            
            // Make sure it has a collider
            Collider col = virtualTable.GetComponent<Collider>();
            if (col == null)
            {
                // Try to find collider in children
                col = virtualTable.GetComponentInChildren<Collider>();
                if (col == null)
                {
                    // Add a box collider if none exists
                    virtualTable.AddComponent<BoxCollider>();
                    Debug.LogWarning("VirtualTableSpawner: No collider found on table prefab. Added BoxCollider.");
                }
            }
        }
        else
        {
            // Create table GameObject from primitive
            virtualTable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            virtualTable.name = "VirtualTable";
            virtualTable.transform.position = spawnPos;
            virtualTable.transform.rotation = spawnRot;
            virtualTable.transform.localScale = tableSize;

            // Set material if provided
            if (tableMaterial != null)
            {
                Renderer renderer = virtualTable.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = tableMaterial;
                }
            }
            else
            {
                // Use a semi-transparent material so you can see through it
                Renderer renderer = virtualTable.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Try to find a compatible shader (Meta/Lit or Standard)
                    Shader shader = null;
                    
                    // Try Meta/Lit first (for Meta render pipeline)
                    shader = Shader.Find("Meta/Lit");
                    
                    // Fallback to Standard (built-in, works with most pipelines)
                    if (shader == null)
                    {
                        shader = Shader.Find("Standard");
                    }
                    
                    // Last resort: Unlit/Transparent
                    if (shader == null)
                    {
                        shader = Shader.Find("Unlit/Transparent");
                    }
                    
                    if (shader != null)
                    {
                        Material mat = new Material(shader);
                        mat.color = new Color(0.5f, 0.3f, 0.2f, 0.5f); // Brown, semi-transparent
                        
                        // Only set transparent properties if using Standard shader
                        if (shader.name.Contains("Standard"))
                        {
                            mat.SetFloat("_Mode", 3); // Transparent mode
                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat.SetInt("_ZWrite", 0);
                            mat.DisableKeyword("_ALPHATEST_ON");
                            mat.EnableKeyword("_ALPHABLEND_ON");
                            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        }
                        
                        mat.renderQueue = 2500; // Lower render queue so hands render on top
                        renderer.material = mat;
                    }
                    else
                    {
                        Debug.LogWarning("Could not find compatible shader for table material!");
                    }
                }
            }

            // Make sure it has a collider (CreatePrimitive already adds one, but ensure it's enabled)
            Collider col = virtualTable.GetComponent<Collider>();
            if (col == null)
            {
                virtualTable.AddComponent<BoxCollider>();
            }
            else
            {
                col.enabled = true;
            }
        }

        Debug.Log($"Virtual table spawned at {spawnPos}");
        
        if (requireConfirmation)
        {
            Debug.Log("Press A button (right controller) to confirm table position");
        }
    }

    /// <summary>
    /// Repositions the existing table to the controller tip
    /// </summary>
    private void RepositionTable()
    {
        if (virtualTable == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        Quaternion spawnRot = GetSpawnRotation();

        virtualTable.transform.position = spawnPos;
        virtualTable.transform.rotation = spawnRot;

        Debug.Log($"Table repositioned to {spawnPos}. Press A button to confirm.");
    }

    /// <summary>
    /// Confirms the table position
    /// </summary>
    public void ConfirmTablePosition()
    {
        if (virtualTable == null) return;

        isTableConfirmed = true;
        Debug.Log("Table position confirmed!");
        
        // You can add additional logic here, like:
        // - Lock the table in place
        // - Enable interactions
        // - Trigger events
        // - etc.
    }

    /// <summary>
    /// Gets whether the table position has been confirmed
    /// </summary>
    public bool IsTableConfirmed()
    {
        return isTableConfirmed;
    }

    /// <summary>
    /// Destroys the virtual table
    /// </summary>
    public void DestroyTable()
    {
        if (virtualTable != null)
        {
            Destroy(virtualTable);
            virtualTable = null;
            isTableConfirmed = false;
            Debug.Log("Virtual table destroyed");
        }
    }

    /// <summary>
    /// Gets the virtual table GameObject
    /// </summary>
    public GameObject GetTable()
    {
        return virtualTable;
    }
}

