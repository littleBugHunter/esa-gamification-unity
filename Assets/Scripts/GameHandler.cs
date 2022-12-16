// Created by Krista Plagemann //
// Handles GameStates //

using UnityEngine;

/// <summary>
/// State of the game.
/// </summary>
public enum GameState { CraterRadarMark, CraterPinchMark, CraterTapMark }

[DefaultExecutionOrder(-10)]    // executed this script before any other custom script
public class GameHandler : MonoBehaviour
{
    private GameState state;

    public GameState State
    {
        get { return state; }
        set
        {
            state = value;
            OnStateChangedEvent(state);
        }
    }

    /// <summary>
    /// Delegate for function that change on game state change.
    /// </summary>
    /// <param name="state">The next state of the game</param>
    public delegate void OnStateChanged(GameState state);
    /// <summary>
    /// When the state of the game changes the functions in this will be called.
    /// </summary>
    public event OnStateChanged OnStateChangedEvent;

    /// <summary>
    /// Singleton that handles the state of the game.
    /// </summary>
    public static GameHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);    // Is not destroyed on scene changes
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        State = GameState.CraterRadarMark;  // Assigning the first state.
    }

    /// <summary>
    /// Set the state using the enum index.
    /// </summary>
    /// <param name="index">Index of the next enum GameState.</param>
    public void SetState(int index)
    {
        State = (GameState)index;
    }

    /// <summary>
    /// Call this to set the state using the enum GameState.
    /// </summary>
    /// <param name="state">Next GameState.</param>
    public void SetGameState(GameState state)
    {
        State = state;
    }

    /// <summary>
    /// Get the index of the current state in the GameState enum.
    /// </summary>
    /// <returns>GameState index of the current state</returns>
    public int GetCurrentStateEnum() { return (int)state; }

}
