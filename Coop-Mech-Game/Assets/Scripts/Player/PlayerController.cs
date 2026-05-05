using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public enum EPlayerState
{
    Moving,
    Paused
}

public class PlayerController : NetworkBehaviour
{
    #region Variables

    [Header("Player State")]
    public EPlayerState currentState = EPlayerState.Moving;
    [SerializeField] private bool isScrambled = false;

    [Header("Player Input")]
    private PlayerInputActions playerInputActions; 

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private MovementIndicator leftIndicator;
    [SerializeField] private MovementIndicator rightIndicator;
    [SerializeField] public Camera baseCamera;
    [SerializeField] public Camera overlayCamera;
    [SerializeField] public GameObject uiCanvas;
    [SerializeField] public GameObject indicatorCanvas;
    [SerializeField] private TeamWeaponManager teamWeaponManager;
    [SerializeField] private UtilityManagerScript utilityManager;
    [SerializeField] private UI_Manager uiManager;
    [SerializeField] private PlayerAudioManager audioManager;

    public NetworkVariable<Vector2> mouse1Pos = new NetworkVariable<Vector2>();
    public NetworkVariable<Vector2> mouse2Pos = new NetworkVariable<Vector2>();

    [Header("Players")]
    [SerializeField]public TestPlayerObjectScript player1;
    [SerializeField]public TestPlayerObjectScript player2;

    [Header("Inventory")]
    public Tuple<ShopItemSO,ShopItemSO> playerWeapons;
    public Tuple<ShopItemSO,ShopItemSO> playerUtilities;
    
    #endregion

    #region Unity Functions

     
    public override void OnNetworkSpawn()
    {
        //if(!IsOwner) { return; }
        //mainCamera.GetComponent<Camera>().enabled = true;
        //GameManager.Instance.AddController(this);
    }
   
    private void Awake()
    {
        //GameManager.Instance.AddController(this);
        //playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // GameManager.Instance.AddController(this);
        //mainCamera.GetComponent<Camera>().enabled = true;
    }

    private void OnEnable()
    {
        if (!IsOwner) { return; }
        // SubscribeInputActions();
        if (player1 != null){player1.SwitchActionMap(EPlayerState.Moving);}
        if (player2 != null){player2.SwitchActionMap(EPlayerState.Moving);}
    }

    private void OnDisable()
    {
        // UnsubscribeInputActions();
        if (player1 != null){player1.SwitchActionMap(currentState);}
        if (player2 != null){player2.SwitchActionMap(currentState);}
    }

    private void FixedUpdate()
    {
        if (currentState == EPlayerState.Moving)
        {    
            if (playerCoroutineManager.TryGetSyncedMove(out Vector2 syncedMoveInput))
            {
                baseMovement.SetMovementInput(syncedMoveInput);
            }
            else
            {
                baseMovement.SetMovementInput(Vector2.zero);
            }
            if (playerCoroutineManager.TryGetSyncedShoot(out float syncedShootInput))
            {
                baseMovement.Shoot(syncedShootInput);
            }

            baseMovement.SetLookInput(mouse1Pos.Value, mouse2Pos.Value);

            teamWeaponManager.SetMouseDistance(Vector2.Distance(mouse1Pos.Value, mouse2Pos.Value));

            if (playerCoroutineManager.TryGetSyncedJump(out float syncedJumpInput))
            {
                baseMovement.Jump(syncedJumpInput);
            }

            if (playerCoroutineManager.TryGetSyncedDash(out Vector2 syncedDashOutput))
            {
                baseMovement.Dash(syncedDashOutput);
            }

            if (playerCoroutineManager.TryGetSyncedUtility(out float syncedUtilityInput))
            {
                utilityManager.SetUtilActivationSynced(true);
            }
            else
            {
                utilityManager.SetUtilActivationSynced(false);
            }
        }
    }

    #endregion

    #region Shop

    /// <summary>
    /// Grabs the player number and team of the local player to determine which PlayerController to reference for shop interactions.
    /// </summary>
    /// <param name="item">Reference to the bought ShopItemSO</param>
    /// <param name="playerNum"></param>

    public void ChangeWeapon(ShopItemSO item, int playerNum)
    {
        teamWeaponManager.PurchaseWeapon(playerNum, item);
        teamWeaponManager.PurchaseWeaponRpc(playerNum, item.itemIndex);
        Debug.Log("Changed player " + playerNum + " weapon to " + item.itemName);
    }

    public void ChangeUtility(ShopItemSO item, int playerNum)
    {
        teamWeaponManager.PurchaseUtility(playerNum, item);
        teamWeaponManager.PurchaseUtilityRpc(playerNum, item.itemIndex);
        Debug.Log("Changed player " + playerNum + " utility to " + item.itemName);

    }

    #endregion

    #region Input Actions
    [Rpc(SendTo.Server)]
    public void P1MoveActionServerRpc(Vector2 P1MovementInput)
    {
        if (isScrambled)
        {
            P1MovementInput = new Vector2(-P1MovementInput.x, -P1MovementInput.y); // If scrambled, invert the movement input for player 1
        }
        playerCoroutineManager.SetP1Input(P1MovementInput);
        leftIndicator.SetMoveInput(P1MovementInput);
    }
    [Rpc(SendTo.Server)]
    public void P2MoveActionServerRpc(Vector2 P2MovementInput)
    {
        if (isScrambled)
        {
            P2MovementInput = new Vector2(-P2MovementInput.x, -P2MovementInput.y); // If scrambled, invert the movement input for player 2
        }
        playerCoroutineManager.SetP2Input(P2MovementInput);
        rightIndicator.SetMoveInput(P2MovementInput);
    }

    [Rpc(SendTo.Server)]
    public void P1ShootActionServerRpc(float P1ShootInput)
    {
        if (!isScrambled)
        {
            playerCoroutineManager.SetP1Shoot(P1ShootInput);
        }
        else
        {
            playerCoroutineManager.SetP2Shoot(P1ShootInput);
        }
    }

    [Rpc(SendTo.Server)]
    public void P2ShootActionServerRpc(float P2ShootInput)
    {
        if (!isScrambled)
        {
            playerCoroutineManager.SetP2Shoot(P2ShootInput);
        }
        else
        {
            playerCoroutineManager.SetP1Shoot(P2ShootInput);
        }
    }
    [Rpc(SendTo.Server)]
    public void ProcessMouse1InputServerRpc(Vector2 mousePos)
    {
        if (!isScrambled) mouse1Pos.Value = mousePos;
        else {mouse1Pos.Value = new Vector2(1 - mousePos.x, 1 - mousePos.y);} // If scrambled, invert the mouse position for player 1
    }
    [Rpc(SendTo.Server)]
    public void ProcessMouse2InputServerRpc(Vector2 mousePos)
    {
        if (!isScrambled) mouse2Pos.Value = mousePos;
        else {mouse2Pos.Value = new Vector2(1 - mousePos.x, 1 - mousePos.y);} // If scrambled, invert the mouse position for player 2
    }
    [Rpc(SendTo.Server)]
    public void P1JumpInputServerRpc(float P1JumpInput)
    {
        playerCoroutineManager.SetP1Jump(P1JumpInput);
    }
    [Rpc(SendTo.Server)]
    public void P2JumpInputServerRpc(float P2JumpInput)
    {
        playerCoroutineManager.SetP2Jump(P2JumpInput);
    }
    [Rpc(SendTo.Server)]
    public void P1DashInputServerRpc(float P1DashInput)
    {
        playerCoroutineManager.SetP1Dash(P1DashInput);
    }
    [Rpc(SendTo.Server)]
    public void P2DashInputServerRpc(float P2DashInput)
    {
        playerCoroutineManager.SetP2Dash(P2DashInput);
    }
    [Rpc(SendTo.Server)]
    public void P1UtilityInputServerRpc(float P1UtilityInput)
    {
        utilityManager.P1Utility();
        playerCoroutineManager.SetP1Utility(P1UtilityInput);
    }
    [Rpc(SendTo.Server)]
    public void P2UtilityInputServerRpc(float P2UtilityInput)
    {
        utilityManager.P2Utility();
        playerCoroutineManager.SetP2Utility(P2UtilityInput);
    }

    [Rpc(SendTo.Server)]
    public void P1ReloadInputServerRpc(float P1ReloadInput)
    {
        teamWeaponManager.P1Reload();
    }
    [Rpc(SendTo.Server)]
    public void P2ReloadInputServerRpc(float P2ReloadInput)
    {
        teamWeaponManager.P2Reload();
    }

    [Rpc(SendTo.Server)]
    public void P1CountdownInputServerRpc()
    {
        uiManager.SetCountdownRpc();
    }

    [Rpc(SendTo.Server)]
    public void P2CountdownInputServerRpc()
    {
        uiManager.SetCountdownRpc();
    }

    #endregion

    #region Callbacks

    public void OnGamePausedReceived()
    {
        if (player1 != null){player1.SwitchActionMap(EPlayerState.Paused);}
        if (player2 != null){player2.SwitchActionMap(EPlayerState.Paused);}
    }

    public void OnGameResumedReceived()
    {
        if (player1 != null){player1.SwitchActionMap(EPlayerState.Moving);}
        if (player2 != null){player2.SwitchActionMap(EPlayerState.Moving);}
    }

    #endregion

    #region Getters and Setters

    public CharacterMovement GetCharacterMovement()
    {
        return gameObject.GetComponent<CharacterMovement>();
    }

    public void SetScramble(bool value)
    {
        isScrambled = value;
    }
    public bool GetIsScrambled()
    {
        return isScrambled;
    }

    public PlayerAudioManager GetAudioManager()
    {
        return audioManager;
    }

    #endregion
}