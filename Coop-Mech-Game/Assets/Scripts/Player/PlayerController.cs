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
    [SerializeField] private Vector2 combinedShootInput;
    [SerializeField] private Vector2 combinedMeleeInput;
    [SerializeField] private float P1LookInput;
    [SerializeField] private float P2LookInput;

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;

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

            if (playerCoroutineManager.TryGetSyncedLook(out float syncedLookInput))
            {
                baseMovement.SetLookInput(syncedLookInput);
            }

            if (playerCoroutineManager.TryGetSyncedShoot(out Vector2 syncedShootInput))
            {
                print("Both Players Shooting!");
                // Handle shooting logic here
            }

            if (playerCoroutineManager.TryGetSyncedMelee(out Vector2 syncedMeleeInput))
            {
                print("Both Players Melee Attacking!");
                // Handle melee logic here
            }
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

        playerInputActions.Player.P1Shoot.started += P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled += P1ShootAction;

        playerInputActions.Player.P2Shoot.started += P2ShootAction;
        playerInputActions.Player.P2Shoot.canceled += P2ShootAction;

        playerInputActions.Player.P1Melee.started += P1MeleeAction;
        playerInputActions.Player.P1Melee.canceled += P1MeleeAction;

        playerInputActions.Player.P2Melee.started += P2MeleeAction;
        playerInputActions.Player.P2Melee.canceled += P2MeleeAction;

        playerInputActions.Player.Jump.performed += JumpActionPerformed;
        playerInputActions.Player.Jump.canceled += JumpActionCanceled;
    }

    private void UnsubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move.started -= P2MoveAction;
        playerInputActions.Player.P2Move.canceled -= P2MoveAction;

        playerInputActions.Player.Jump.performed -= JumpActionPerformed;
        playerInputActions.Player.Jump.canceled -= JumpActionCanceled;
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

    private void P1ShootAction(InputAction.CallbackContext context)
    {
        float p1shootInput = context.ReadValue<float>();
        combinedShootInput.x = p1shootInput;
        playerCoroutineManager.setP1ShootInput(p1shootInput);
    }
    private void P2ShootAction(InputAction.CallbackContext context)
    {
        float p2shootInput = context.ReadValue<float>();
        combinedShootInput.y = p2shootInput;
        playerCoroutineManager.setP2ShootInput(p2shootInput);
    }

    private void P1MeleeAction(InputAction.CallbackContext context)
    {
        float p1meleeInput = context.ReadValue<float>();
        combinedMeleeInput.x = p1meleeInput;
        playerCoroutineManager.setP1MeleeInput(p1meleeInput);
    }

    private void P2MeleeAction(InputAction.CallbackContext context)
    {
        float p2meleeInput = context.ReadValue<float>();
        combinedMeleeInput.y = p2meleeInput;
        playerCoroutineManager.setP2MeleeInput(p2meleeInput);
    }

    private void JumpActionPerformed(InputAction.CallbackContext context)
    {
        baseMovement.Jump();
    }

    private void JumpActionCanceled(InputAction.CallbackContext context)
    {
        baseMovement.CancelJump();
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