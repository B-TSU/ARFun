using UnityEngine;
using System;

/// <summary>
/// Manages the overall game state and flow of the AR Ikebana application
/// </summary>
public enum GameState
{
    MainMenu,
    PlateSelection,
    PlatePlacement,
    PlateConfirmation,
    FlowerSelection,
    FlowerArrangement,
    Trimming,
    Screenshot
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("State Management")]
    [SerializeField] private GameState currentState = GameState.MainMenu;
    
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    /// <summary>
    /// Changes the current game state and notifies listeners
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        GameState previousState = currentState;
        currentState = newState;

        Debug.Log($"State changed from {previousState} to {newState}");
        OnStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Gets the current game state
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Checks if the current state matches the given state
    /// </summary>
    public bool IsState(GameState state)
    {
        return currentState == state;
    }
}

