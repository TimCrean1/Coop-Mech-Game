using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EPlayerState
{
    Moving,
    Paused,
}

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Player State")]
    public EPlayerState currentState = EPlayerState.Moving;

    [Header("Player Input")]
    private PlayerInputActions playerInputActions; 
    [SerializeField] private Vector2 P1MovementInput;
    [SerializeField] private Vector2 P2MovementInput;
    [SerializeField] private float P1LookInput;
    [SerializeField] private float P2LookInput;

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;

    [Header("Mouse Positions")]
    [SerializeField] private Vector2 mouse1Pos; //Screen space pos
    [SerializeField] private Vector2 mouse2Pos;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
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

        playerInputActions.Player.P1Look.started += P1LookAction;
        playerInputActions.Player.P1Look.canceled += P1LookAction;

        playerInputActions.Player.P2Look.started += P2LookAction;
        playerInputActions.Player.P2Look.canceled += P2LookAction;
    }

    private void UnsubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move.started -= P2MoveAction;
        playerInputActions.Player.P2Move.canceled -= P2MoveAction;
    }

    private void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
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

    private void P1LookAction(InputAction.CallbackContext context)
    {
        P1LookInput = context.ReadValue<float>();
        playerCoroutineManager.SetP1LookInput(P1LookInput);
    }

    private void P2LookAction(InputAction.CallbackContext context)
    {
        P2LookInput = context.ReadValue<float>();
        playerCoroutineManager.SetP2LookInput(P2LookInput);
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