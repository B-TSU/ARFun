using UnityEngine;

/// <summary>
/// Activates the current menu when the user looks at the left controller
/// </summary>
public class ControllerMenuActivator : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] private Transform leftController;

    [Header("Look Detection Settings")]
    [SerializeField] private float lookThreshold = 0.8f; // Dot product threshold (0.8 = ~37 degrees)
    [SerializeField] private float maxDistance = 2f; // Maximum distance to detect looking

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float lookSustainTime = 0.2f;
    private float lookTimer = 0f;


    private bool isLookingAtController = false;
    private bool menuWasVisible = false;

    private void Start()
    {
        // Auto-find camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Try to find left controller if not assigned
        if (leftController == null)
        {
            FindLeftController();
        }
    }

    private void Update()
    {
        if (leftController == null || mainCamera == null)
        {
            return;
        }

        // Check if user is looking at the left controller
        bool currentlyLooking = IsLookingAtController();

        if (currentlyLooking)
        {
            lookTimer += Time.deltaTime;
            if (!isLookingAtController && lookTimer >= lookSustainTime)
                OnStartLookingAtController();
        }
        else
        {
            lookTimer -= Time.deltaTime;
            if (isLookingAtController && lookTimer <= 0f)
                OnStopLookingAtController();
        }

        lookTimer = Mathf.Clamp(lookTimer, 0f, lookSustainTime);
        isLookingAtController = lookTimer >= lookSustainTime;

    }

    /// <summary>
    /// Checks if the user is looking at the left controller
    /// </summary>
    private bool IsLookingAtController()
    {
        Vector3 headPosition = mainCamera.transform.position;
        Vector3 headForward = mainCamera.transform.forward;
        Vector3 controllerPosition = leftController.position;

        // Calculate direction from head to controller
        Vector3 headToController = (controllerPosition - headPosition).normalized;

        // Check distance
        float distance = Vector3.Distance(headPosition, controllerPosition);
        if (distance > maxDistance)
        {
            return false;
        }

        // Calculate dot product (1.0 = looking directly at, 0.0 = perpendicular)
        float dot = Vector3.Dot(headForward, headToController);

        return dot > lookThreshold;
    }

    /// <summary>
    /// Called when the user starts looking at the controller
    /// </summary>
    private void OnStartLookingAtController()
    {
        // Remember if menu was already visible
        menuWasVisible = IsAnyMenuVisible();

        // Show the current menu
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCurrentMenu();
        }
    }

    /// <summary>
    /// Called when the user stops looking at the controller
    /// </summary>
    private void OnStopLookingAtController()
    {
        // Only hide menu if it wasn't visible before we started looking
        if (!menuWasVisible)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
            }
        }
    }

    /// <summary>
    /// Checks if any menu panel is currently visible
    /// </summary>
    private bool IsAnyMenuVisible()
    {
        if (UIManager.Instance == null)
        {
            return false;
        }

        // We can't directly access private panels, so we'll check via GameStateManager
        // For now, we'll assume menu is visible if we're in a menu state
        if (GameStateManager.Instance != null)
        {
            GameState currentState = GameStateManager.Instance.GetCurrentState();
            return currentState == GameState.MainMenu ||
                   currentState == GameState.PlateSelection ||
                   currentState == GameState.FlowerSelection;
        }

        return false;
    }

    /// <summary>
    /// Attempts to find the left controller in the scene
    /// </summary>
    private void FindLeftController()
    {
        // Try common names for left controller anchor
        string[] possibleNames = {
            "LeftControllerAnchor",
            "LeftControllerInHandAnchor",
            "LeftHandAnchor",
            "left_oculus_controller_world"
        };

        foreach (string name in possibleNames)
        {
            GameObject found = GameObject.Find(name);
            if (found != null)
            {
                leftController = found.transform;
                Debug.Log($"ControllerMenuActivator: Found left controller at {name}");
                return;
            }
        }

        Debug.LogWarning("ControllerMenuActivator: Could not find left controller. Please assign it in the Inspector.");
    }
}
