using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages UI panel visibility and transitions
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject plateSelectionPanel;
    [SerializeField] private GameObject platePlacementPanel;
    [SerializeField] private GameObject flowerSelectionPanel;
    [SerializeField] private GameObject trimmingPanel;

    private Dictionary<GameState, GameObject> stateToPanelMap;

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
        InitializePanelMap();
        
        // Subscribe to state changes
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    private void InitializePanelMap()
    {
        stateToPanelMap = new Dictionary<GameState, GameObject>
        {
            { GameState.MainMenu, mainMenuPanel },
            { GameState.PlateSelection, plateSelectionPanel },
            { GameState.PlatePlacement, platePlacementPanel },
            { GameState.PlateConfirmation, platePlacementPanel }, // Can reuse placement panel
            { GameState.FlowerSelection, flowerSelectionPanel },
            { GameState.FlowerArrangement, null }, // No UI panel needed
            { GameState.Trimming, trimmingPanel }
        };
    }

    private void HandleStateChanged(GameState newState)
    {
        ShowPanelForState(newState);
    }

    /// <summary>
    /// Shows the appropriate panel for the given state
    /// </summary>
    public void ShowPanelForState(GameState state)
    {
        // Hide all panels first
        HideAllPanels();

        // Show the panel for the current state
        if (stateToPanelMap.ContainsKey(state) && stateToPanelMap[state] != null)
        {
            stateToPanelMap[state].SetActive(true);
        }
    }

    /// <summary>
    /// Hides all UI panels
    /// </summary>
    public void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (plateSelectionPanel != null) plateSelectionPanel.SetActive(false);
        if (platePlacementPanel != null) platePlacementPanel.SetActive(false);
        if (flowerSelectionPanel != null) flowerSelectionPanel.SetActive(false);
        if (trimmingPanel != null) trimmingPanel.SetActive(false);
    }
}

