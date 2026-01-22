using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

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

    [Header("Player Input")]
    private PlayerInputActions playerInputActions; 
    [SerializeField] private Vector2 P1MovementInput;
    [SerializeField] private Vector2 P2MovementInput;
    [SerializeField] private float P1ShootInput;
    [SerializeField] private float P2ShootInput;

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;
    [SerializeField] private GameObject mainCamera;

    [Header("Mouse Positions")]
    [SerializeField] private Vector2 mouse1Pos; //Screen space pos
    [SerializeField] private Vector2 mouse2Pos;


    [Header("Players")]
    [SerializeField]public TestPlayerObjectScript player1;
    [SerializeField]public TestPlayerObjectScript player2;
    #endregion

    #region Unity Functions
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }
        mainCamera.GetComponent<Camera>().enabled = true;
        //GameManager.Instance.AddController(this);
    }
    private void Awake()
    {
        
        //playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        GameManager.Instance.AddController(this);
    }

    private void OnEnable()
    {
        if (!IsOwner) { return; }
        SubscribeInputActions();
        SwitchActionMap(EPlayerState.Moving);
    }

    private void OnDisable()
    {
        UnsubscribeInputActions();
        SwitchActionMap(currentState);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
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
                baseMovement.Shoot();
            }
            mouse1Pos = Mouse.current.position.ReadValue();
            mouse1Pos.x = mouse1Pos.x/Screen.width;
            mouse1Pos.y = mouse1Pos.y/Screen.height;
            // mouse1Pos = mouse1Pos * 2f - Vector2.one;
            
            mouse2Pos = mouse1Pos;

            baseMovement.SetLookInput(mouse1Pos, mouse2Pos);
        }
    }

    #endregion

    #region Input Handling

    private void SubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started += P1MoveAction;
        playerInputActions.Player.P1Move.canceled += P1MoveAction;

        playerInputActions.Player.P2Move.started += P2MoveAction;
        playerInputActions.Player.P2Move.canceled += P2MoveAction;

        playerInputActions.Player.P1Shoot.started += P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled += P1ShootAction;

        playerInputActions.Player.P2Shoot.started += P2ShootAction;
        playerInputActions.Player.P2Shoot.canceled += P2ShootAction;
    }

    private void UnsubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move.started -= P2MoveAction;
        playerInputActions.Player.P2Move.canceled -= P2MoveAction;

        playerInputActions.Player.P1Shoot.started -= P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled -= P1ShootAction;

        playerInputActions.Player.P2Shoot.started -= P2ShootAction;
        playerInputActions.Player.P2Shoot.canceled -= P2ShootAction;
    }

    private void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                // Cursor.visible = false;
                // Cursor.lockState = CursorLockMode.Locked;
                break;

            case EPlayerState.Paused:
                playerInputActions.UI.Enable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            default:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    #endregion

    #region Input Actions

    private void P1MoveAction(InputAction.CallbackContext context)
    {
        P1MovementInput = context.ReadValue<Vector2>();
        playerCoroutineManager.SetP1Input(P1MovementInput);
    }

    private void P2MoveAction(InputAction.CallbackContext context)
    {
        P2MovementInput = context.ReadValue<Vector2>();
        playerCoroutineManager.SetP2Input(P2MovementInput);
    }

    private void P1ShootAction(InputAction.CallbackContext context)
    {
        P1ShootInput = context.ReadValue<float>();
        playerCoroutineManager.SetP1Shoot(P1ShootInput);
    }

    private void P2ShootAction(InputAction.CallbackContext context)
    {
        P2ShootInput = context.ReadValue<float>();
        playerCoroutineManager.SetP2Shoot(P2ShootInput);
    }

    #endregion

    #region Callbacks

    public void OnGamePausedReceived()
    {
        SwitchActionMap(EPlayerState.Paused);
    }

    public void OnGameResumedReceived()
    {
        SwitchActionMap(EPlayerState.Moving);
    }

    #endregion
}