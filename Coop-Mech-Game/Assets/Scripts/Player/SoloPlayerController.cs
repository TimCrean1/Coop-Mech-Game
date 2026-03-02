using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoloPlayerController : MonoBehaviour
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
    [SerializeField] private float P1DashInput;
    [SerializeField] private float P2DashInput;

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;
    [SerializeField] private GameObject mainCamera;

    [Header("Mouse Positions")]
    [SerializeField] public Vector2 mouse1Pos;
    [SerializeField] public Vector2 mouse2Pos;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        SubscribeInputActions();
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        UnsubscribeInputActions();
        playerInputActions.Disable();
    }

    private void FixedUpdate()
    {
        ProcessMouseInput();

        if (currentState != EPlayerState.Moving)
            return;

        // Movement
        if (playerCoroutineManager.TryGetSyncedMove(out Vector2 syncedMoveInput))
        {
            baseMovement.SetMovementInput(syncedMoveInput);
        }
        else
        {
            baseMovement.SetMovementInput(Vector2.zero);
        }

        // Shooting
        if (playerCoroutineManager.TryGetSyncedShoot(out float syncedShootInput))
        {
            baseMovement.Shoot(syncedShootInput);
        }

        // Dash
        if (playerCoroutineManager.TryGetSyncedDash(out Vector2 syncedDashOutput))
        {
            baseMovement.Dash(syncedDashOutput);
        }

        // Look
        baseMovement.SetLookInput(mouse1Pos, mouse2Pos);
    }

    #endregion

    #region Input Handling

    private void SubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started += P1MoveAction;
        playerInputActions.Player.P1Move.performed += P1MoveAction;
        playerInputActions.Player.P1Move.canceled += P1MoveAction;

        playerInputActions.Player.P2Move2.started += P2MoveAction;
        playerInputActions.Player.P2Move2.performed += P2MoveAction;
        playerInputActions.Player.P2Move2.canceled += P2MoveAction;

        playerInputActions.Player.P1Shoot.started += P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled += P1ShootAction;

        playerInputActions.Player.P2Shoot.started += P2ShootAction;

        // Dash
        playerInputActions.Player.P1Dash.started += P1DashAction;
        playerInputActions.Player.P1Dash.performed += P1DashAction;
        playerInputActions.Player.P1Dash.canceled += P1DashAction;

        playerInputActions.Player.P2Dash2.started += P2DashAction;
        playerInputActions.Player.P2Dash2.performed += P2DashAction;
        playerInputActions.Player.P2Dash2.canceled += P2DashAction;
    }

    private void UnsubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.performed -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move2.started -= P2MoveAction;
        playerInputActions.Player.P2Move2.performed -= P2MoveAction;
        playerInputActions.Player.P2Move2.canceled -= P2MoveAction;

        playerInputActions.Player.P1Shoot.started -= P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled -= P1ShootAction;

        playerInputActions.Player.P2Shoot.started -= P2ShootAction;

        // Dash
        playerInputActions.Player.P1Dash.started -= P1DashAction;
        playerInputActions.Player.P1Dash.performed -= P1DashAction;
        playerInputActions.Player.P1Dash.canceled -= P1DashAction;

        playerInputActions.Player.P2Dash2.started -= P2DashAction;
        playerInputActions.Player.P2Dash2.performed -= P2DashAction;
        playerInputActions.Player.P2Dash2.canceled -= P2DashAction;
    }

    #endregion

    #region Input Actions

    public void P1MoveAction(InputAction.CallbackContext context)
    {
        P1MovementInput = context.ReadValue<Vector2>();
        playerCoroutineManager.SetP1Input(P1MovementInput);
    }

    public void P2MoveAction(InputAction.CallbackContext context)
    {
        P2MovementInput = context.ReadValue<Vector2>();
        playerCoroutineManager.SetP2Input(P2MovementInput);
    }

    public void P1ShootAction(InputAction.CallbackContext context)
    {
        P1ShootInput = context.ReadValue<float>();
        playerCoroutineManager.SetP1Shoot(P1ShootInput);
    }

    public void P2ShootAction(InputAction.CallbackContext context)
    {
        P2ShootInput = context.ReadValue<float>();
        playerCoroutineManager.SetP2Shoot(P2ShootInput);
    }

    public void P1DashAction(InputAction.CallbackContext context)
    {
        P1DashInput = context.ReadValue<float>();
        playerCoroutineManager.SetP1Dash(P1DashInput);
    }

    public void P2DashAction(InputAction.CallbackContext context)
    {
        P2DashInput = context.ReadValue<float>();
        playerCoroutineManager.SetP2Dash(P2DashInput);
    }

    public void ProcessMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;

        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        mouse1Pos = mousePos;
        mouse2Pos = mousePos;
    }

    #endregion
}