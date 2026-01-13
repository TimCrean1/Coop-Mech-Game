using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoroutineManager : MonoBehaviour
{
    [Header("Input Window Duration")]
    [SerializeField][Range(0.001f, 1)] private float movementSyncWindow = 0.2f;

    [Header("Time Counters")]
    private float p1MoveTime;
    private float p2MoveTime;
    private float p1LookTime;
    private float p2LookTime;

    [Header("Input Storage")]
    private Vector2 p1MoveInput;
    private Vector2 p2MoveInput;
    private float p1LookInput;
    private float p2LookInput;

    private void Start()
    {

    }

    #region Variable setters

    public void SetP1Input(Vector2 MoveInput)
    {
        p1MoveInput = MoveInput;
        p1MoveTime = Time.time;
    }
    public void SetP2Input(Vector2 MoveInput)
    {
        p2MoveInput = MoveInput;
        p2MoveTime = Time.time;
    }

    public void SetP1LookInput(float LookInput)
    {
        p1LookInput = LookInput;
        p1LookTime = Time.time;
    }
    public void SetP2LookInput(float LookInput)
    {
        p2LookInput = LookInput;
        p2LookTime = Time.time;
    }

    #endregion

    #region Input Retrieval

    public bool TryGetSyncedMove(out Vector2 syncedInput)
    {
        syncedInput = Vector2.zero;

        // Check if inputs are within the sync window and that inputs are identical
        if (Mathf.Abs(p1MoveTime - p2MoveTime) <= movementSyncWindow && p1MoveInput == p2MoveInput)
        {
            // Inputs are synced
            syncedInput = (p1MoveInput + p2MoveInput) * 0.5f;

            // Reset times so it only triggers once
            p1MoveTime = -1;
            p2MoveTime = -1;
            return true;
        }

        return false;
    }

    public bool TryGetSyncedLook(out float syncedInput)
    {
        syncedInput = 0f;

        // Check if inputs are within the sync window and that inputs are identical
        if (Mathf.Abs(p1LookTime - p2LookTime) <= movementSyncWindow && p1LookInput == p2LookInput)
        {
            // Inputs are synced
            syncedInput = (p1LookInput + p2LookInput) * 0.5f;

            // Reset times so it only triggers once
            p1LookTime = -1;
            p2LookTime = -1;
            return true;
        }

        return false;
    }
    #endregion
}