using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ESoloPlayerState
{
    Moving,
    Paused
}

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

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    [SerializeField] private PlayerCoroutineManager playerCoroutineManager;
    [SerializeField] private GameObject mainCamera;

    [Header("Mouse Positions")]
    [SerializeField] public Vector2 mouse1Pos; //Screen space pos
    [SerializeField] public Vector2 mouse2Pos;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // GameManager.Instance.AddController(this);
        //mainCamera.GetComponent<Camera>().enabled = true;
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
        // if (player1 != null){player1.SwitchActionMap(currentState);}
        // if (player2 != null){player2.SwitchActionMap(currentState);}
    }

    private void FixedUpdate()
    {
        ProcessMouseInput();
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

            baseMovement.SetLookInput(mouse1Pos, mouse2Pos);
        }
    }

    #endregion

    #region Input Handling

    private void SubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started += P1MoveAction;
        playerInputActions.Player.P1Move.canceled += P1MoveAction;

        playerInputActions.Player.P2Move2.started += P2MoveAction;
        playerInputActions.Player.P2Move2.canceled += P2MoveAction;

        playerInputActions.Player.P1Shoot.started += P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled += P1ShootAction;

        playerInputActions.Player.P2Shoot.started += P2ShootAction;
    }

    private void UnsubscribeInputActions()
    {
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move2.started -= P2MoveAction;
        playerInputActions.Player.P2Move2.canceled -= P2MoveAction;

        playerInputActions.Player.P1Shoot.started -= P1ShootAction;
        playerInputActions.Player.P1Shoot.canceled -= P1ShootAction;

        playerInputActions.Player.P2Shoot.started -= P2ShootAction;
    }

    private void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                break;

            case EPlayerState.Paused:
                playerInputActions.UI.Enable();
                // Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;
                break;

            default:
                // Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    #endregion

    #region Input Actions

    public void P1MoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("yes");
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

    public void ProcessMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos.x = mousePos.x/Screen.width;
        mousePos.y = mousePos.y/Screen.height;
        mouse1Pos = mousePos;
        mouse2Pos = mousePos;
    }

    #endregion

    #region Callbacks

    // public void OnGamePausedReceived()
    // {
        
    // }

    // public void OnGameResumedReceived()
    // {
        
    // }

    #endregion
}