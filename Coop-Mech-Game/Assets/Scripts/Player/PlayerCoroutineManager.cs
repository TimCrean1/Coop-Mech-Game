using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoroutineManager : MonoBehaviour
{
    [Header("Input Window Duration")]
    [SerializeField][Range(0.001f, 1)] private float movementSyncWindow = 0.2f;
    //[SerializeField][Range(0.001f, 1)] private float rotationSyncWindow = 0.2f; 
    //TODO: Split movement and rotation
    [SerializeField][Range(0.001f, 1)] private float punchSyncWindow = 0.2f;
    [SerializeField][Range(0.001f, 1)] private float shootSyncWindow = 0.2f;

    [Header("Time Counters")]
    private float p1MoveTime;
    private float p2MoveTime;
    private float p1ShootTime;
    private float p2ShootTime;
    private float p1MeleeTime;
    private float p2MeleeTime;

    [Header("Input Storage")]
    private Vector2 p1MoveInput;
    private Vector2 p2MoveInput;
    private Vector2 combinedShootInput;
    private Vector2 combinedMeleeInput;

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
    public void setP1ShootInput(float ShootInput)
    {
        combinedShootInput.x = ShootInput;
        p1ShootTime = Time.time;
    }
    public void setP2ShootInput(float ShootInput)
    {
        combinedShootInput.y = ShootInput;
        p2ShootTime = Time.time;
    }
    public void setP1MeleeInput(float MeleeInput)
    {
        combinedMeleeInput.x = MeleeInput;
        p1MeleeTime = Time.time;
    }
    public void setP2MeleeInput(float MeleeInput)
    {
        combinedMeleeInput.y = MeleeInput;
        p2MeleeTime = Time.time;
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

    public bool TryGetSyncedShoot(out Vector2 syncedInput)
    {
        syncedInput = Vector2.zero;

        // Check if inputs are within the sync window and that inputs are identical
        if (Mathf.Abs(p1ShootTime - p2ShootTime) <= shootSyncWindow && combinedShootInput.x == 1 && combinedShootInput.y == 1)
        {
            // Inputs are synced
            syncedInput = new Vector2(combinedShootInput.x, combinedShootInput.y);

            // Reset times so it only triggers once
            p1ShootTime = -1;
            p2ShootTime = -1;
            return true;
        }

        return false;
    }

    public bool TryGetSyncedMelee(out Vector2 syncedInput)
    {
        syncedInput = Vector2.zero;

        // Check if inputs are within the sync window and that inputs are identical
        if (Mathf.Abs(p1MeleeTime - p2MeleeTime) <= punchSyncWindow && combinedMeleeInput.x == 1 && combinedMeleeInput.y == 1)
        {
            // Inputs are synced
            syncedInput = new Vector2(combinedMeleeInput.x, combinedMeleeInput.y);

            // Reset times so it only triggers once
            p1MeleeTime = -1;
            p2MeleeTime = -1;
            return true;
        }

        return false;
    }

    #endregion
}