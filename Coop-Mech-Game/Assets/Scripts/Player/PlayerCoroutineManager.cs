using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoroutineManager : MonoBehaviour
{
    [Header("Input Window Duration")]
    [SerializeField][Range(0.001f, 1)] private float movementSyncWindow = 0.2f;
    [SerializeField][Range(0.001f, 1)] private float shootSyncWindow = 0.2f;

    [Header("Time Counters")]
    private float p1MoveTime;
    private float p2MoveTime;
    private float p1ShootTime;
    private float p2ShootTime;

    [Header("Input Storage")]
    private Vector2 p1MoveInput;
    private Vector2 p2MoveInput;
    private float p1ShootInput;
    private float p2ShootInput;

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

    public void SetP1Shoot(float ShootInput)
    {
        p1ShootInput = ShootInput;
        p1ShootTime = Time.time;
    }
    public void SetP2Shoot(float ShootInput)
    {
        p2ShootInput = ShootInput;
        p2ShootTime = Time.time;
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

    public bool TryGetSyncedShoot(out float syncedInput)
    {
        syncedInput = 0;

        // Check if inputs are within the sync window and that inputs are identical
        if (Mathf.Abs(p1ShootTime - p2ShootTime) <= shootSyncWindow && Mathf.Approximately(p1ShootInput, p2ShootInput))
        {
            // Inputs are synced
            syncedInput = (p1ShootInput + p2ShootInput) * 0.5f;

            // Reset times so it only triggers once
            p1ShootTime = -1;
            p2ShootTime = -1;
            return true;
        }

        return false;
    }

    #endregion
}