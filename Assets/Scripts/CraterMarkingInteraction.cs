// Created by Krista Plagemann //
// Handles the different crater marking states. Not really that optimal yet lol. //

using UnityEngine;

/// <summary>
/// Unused so far, but could be used to have set crater types.
/// </summary>
public enum craterTyper { SMALL, MEDIUM, BIG };

public class CraterMarkingInteraction : MonoBehaviour
{
    [Tooltip("Prefab of the dot that marks craters in radar marking.")]
    [SerializeField]
    private GameObject markerPrefab;

    [Tooltip("Objects that belong to radar marking only to disable when in other mode.")]
    [SerializeField]
    private GameObject[] craterRadarMarkObjects;

    #region state changes

    private void Awake()
    {
        GameHandler.Instance.OnStateChangedEvent += changeCraterMarkingVisuals;
    }

    /// <summary>
    /// Set the next state according to the enum index (not very optimal cuz who knows the index but ohwell)
    /// </summary>
    /// <param name="enumIndex">index in the GameState enum</param>
    public void SetState(int enumIndex)
    {
        if(GameHandler.Instance.GetCurrentStateEnum() != enumIndex) // first check if we're already in the state we want
        GameHandler.Instance.SetState(enumIndex);
    }

    /// <summary>
    /// Changes the visibility of objects belonging to the radar marking type (this is a stupid way to do this :D)
    /// </summary>
    /// <param name="state">Next State</param>
    private void changeCraterMarkingVisuals(GameState state)
    {
        if(state == GameState.CraterRadarMark)
        {
            foreach (GameObject go in craterRadarMarkObjects)
                go.SetActive(true);
        }
        else
            foreach (GameObject go in craterRadarMarkObjects)
                go.SetActive(false);
    }

    #endregion

    /// <summary>
    /// Makes a new crater marking point on the center of the map(which is the center of the radar).
    /// </summary>
    public void markCrater()
    {
        GameObject newMarker = Instantiate(markerPrefab,transform);
        newMarker.transform.parent = transform.GetChild(0).transform;
    }



}
