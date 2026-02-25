using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCoroutineManager : NetworkBehaviour
{
    [Header("Input Window Duration")]
    [SerializeField][Range(0.001f, 1)] private float movementSyncWindow = 0.5f;
    [SerializeField][Range(0.001f, 1)] private float shootSyncWindow = 0.2f;
    [SerializeField][Range(0.001f, 5)] private float unsyncedMoveMultiplier = 0.15f;
    [SerializeField][Range(0.001f, 2)] private float syncedMoveMultiplier = 1;

    [Header("Time Counters")]
    private float p1MoveTime;
    private float p2MoveTime;
    private float p1ShootTime;
    private float p2ShootTime;

    [Header("Input Storage")]
    private Vector2 p1MoveInput;
    private Vector2 p2MoveInput;
    //private float p1ShootInput;
    //private float p2ShootInput;
    private NetworkVariable<float> p1ShootInput = new NetworkVariable<float>();
    private NetworkVariable<float> p2ShootInput = new NetworkVariable<float>();

    [Header("Combo Variables")]
    [SerializeField] private SingleComboScript comboManager;
    [SerializeField][Range(0,100)] private float syncedMoveScore = 0.1f;
    [SerializeField][Range(0,100)] private float syncedShootScore = 5;
    [SerializeField][Range(0,100)] private float syncedUtilityScore = 5;

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
        p1ShootInput.Value = ShootInput;
        p1ShootTime = Time.time;
    }
    public void SetP2Shoot(float ShootInput)
    {
        Debug.Log("SetP2Shoot " + ShootInput);
        p2ShootInput.Value = ShootInput;
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
            //Debug.Log("move1");
            // Inputs are synced
            syncedInput = (p1MoveInput + p2MoveInput) * (syncedMoveMultiplier - 0.5f);

            if (syncedInput!= Vector2.zero) comboManager.AddPointsRpc(syncedMoveScore);

            // Reset times so it only triggers once
            p1MoveTime = -1;
            p2MoveTime = -1;

            return true;
        }

        // If the inputs are not identical and the sync window has passed, average the inputs anyway
        else if(p1MoveInput != p2MoveInput)
        {
            //Debug.Log("move2");
            // Average the two inputs even though they are not identical
            syncedInput = (p1MoveInput + p2MoveInput) * unsyncedMoveMultiplier;

            // Reset times so it only triggers once
            p1MoveTime = -1;
            p2MoveTime = -1;
            return true;
        }
        else if (p1MoveInput == p2MoveInput && Mathf.Abs(p1MoveTime - p2MoveTime) >= movementSyncWindow)
        {

            //Debug.Log("move3");
            syncedInput = (p1MoveInput + p2MoveInput) * 0.5f; //1.0 speed

            p1MoveTime = -1;
            p2MoveTime = -1;
            return true;
        }

        else if (p1MoveInput != p2MoveInput && Mathf.Abs(p1MoveTime - p2MoveTime) >= movementSyncWindow)
        {

            //Debug.Log("move4");
            // Average the two inputs even though they are not identical
            syncedInput = (p1MoveInput + p2MoveInput) * unsyncedMoveMultiplier;

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
        if (Mathf.Abs(p1ShootTime - p2ShootTime) <= shootSyncWindow 
            && Mathf.Approximately(p1ShootInput.Value, p2ShootInput.Value))
        {
            //Debug.Log("This is where we might shoot1");
            // Inputs are synced
            syncedInput = (p1ShootInput.Value + p2ShootInput.Value) * 0.5f; //this will be 2 * .5 = 1

            if (syncedInput != 0) comboManager.AddPointsRpc(syncedShootScore);

            // Reset times so it only triggers once
            p1ShootTime = -1;
            p2ShootTime = -1;

            return true;
        }
        else if (p1ShootInput.Value > 0 && p2ShootInput.Value <= 0)
        {
            //Debug.Log("This is where we might shoot2");
            syncedInput = 0.25f;

            p1ShootTime = -1;
            p2ShootTime = -1;
            return true;
        }
        else if (p1ShootInput.Value <= 0 && p2ShootInput.Value > 0)
        {
            //Debug.Log("This is where we might shoot3");
            syncedInput = 0.75f;

            p1ShootTime = -1;
            p2ShootTime = -1;
            return true;
        }
        else if (Mathf.Abs(p1ShootTime - p2ShootTime) >= shootSyncWindow && p1ShootInput.Value == 1 && p2ShootInput.Value == 1)
        {
            // Inputs are synced
            syncedInput = (p1ShootInput.Value + p2ShootInput.Value) * 0.5f;

            // Reset times so it only triggers once
            p1ShootTime = -1;
            p2ShootTime = -1;
            return true;
        }
        return false;
    }
    #endregion
}