using Unity.Netcode;
//using Unity.Services.Matchmaker.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Services.Authentication;
using System.Collections;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public PlayerController playerController;
    private Vector2 mousePos;
    [SerializeField] private string playerIndex;
    [SerializeField] private string playerTeam;
    [SerializeField] private string playerNumber;
    [SerializeField] private string playerName;
    [SerializeField] private string idCheck;
    private PlayerInputActions playerInputActions;
    private bool isInitialized = false;

    private Vector2 lastSentMousePos;
    private float nextMouseSendTime = 0f;

    // 20 updates/sec
    private const float MOUSE_SEND_INTERVAL = 0.05f;

    // Ignore tiny movement changes
    private const float MOUSE_SEND_THRESHOLD = 0.01f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        // Wait for network objects to fully exist
        while (
            GameManager.Instance == null ||
            !GameManager.Instance.IsSpawned ||
            NetworkManager.Singleton == null ||
            !NetworkManager.Singleton.IsConnectedClient
        )
        {
            yield return null;
        }

        while (GameManager.Instance._playerControllers.Count < 2)
        {
            yield return null;
        }

        playerIndex = BootstrapScript.Instance.playerIndex;
        playerTeam = BootstrapScript.Instance.playerTeam;
        playerNumber = BootstrapScript.Instance.playerNumber;

        NetworkManager.NetworkTickSystem.Tick += Tick;

        Initialize();

        // Tell the server THIS CLIENT is fully initialized
        GameManager.Instance.ClientReadyRpc();
    }

    private void Initialize()
    {
        if (GameManager.Instance._playerControllers.Count <= 0)
        {
            Debug.Log("Playercontroller list is empty");
        }
        
        // ask the server for an id based on connected clients
        // host currently struggles to join the game properly (not getting player controller)
        if (playerTeam == "Red")
        {
            playerController = GameManager.Instance._playerControllers[0];
            GameManager.Instance._playerControllers[0].baseCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[0].overlayCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[0].uiCanvas.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[0].indicatorCanvas.gameObject.SetActive(true);

        } else if (playerTeam == "Blue")
        {
            playerController = GameManager.Instance._playerControllers[1];
            GameManager.Instance._playerControllers[1].baseCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[1].overlayCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[1].uiCanvas.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[1].indicatorCanvas.gameObject.SetActive(true);
        }

        // playerInputActions = new PlayerInputActions();
        playerInputActions = InputManager.Instance.InputActions;
        SubscribeInputActions();
        playerInputActions.Player.Enable();

        isInitialized = true;
    }

    void FixedUpdate()
    {
        if (isInitialized)
        {
            if (playerController.currentState == EPlayerState.Moving)
            {
                if (Application.isFocused == false)
                {
                    SwitchActionMap(EPlayerState.Paused);
                }
            }
        }
    }

    #region Getters

    public string GetPlayerTeam()
    {
        return playerTeam;
    }

    public string GetPlayerNum()
    {
        return playerNumber;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    #endregion



    
    void OnDisable()
    {
        UnsubscribeInputActions();
        
    }

    public void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.Pause.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                if (PauseMenu.Instance.isShowing == true)
                {
                    PauseMenu.Instance.Hide();
                }
                Cursor.visible = false;
                break;

            case EPlayerState.Paused:
                playerInputActions.Pause.Enable();
                if (PauseMenu.Instance.isShowing == false)
                {
                    PauseMenu.Instance.Show();
                }
                Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;
                break;

            default:
                // Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

#region Input Assignments

    private void SubscribeInputActions()
    {
        if (playerNumber == "One")
        {
            // MOVEMENT
            playerInputActions.Player.P1Move.performed += P1MoveAction;
            playerInputActions.Player.P1Move.canceled += P1MoveAction;

            // SHOOT
            playerInputActions.Player.P1Shoot.performed += P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled += P1ShootAction;

            // JUMP
            playerInputActions.Player.P1Jump.performed += P1JumpAction;
            playerInputActions.Player.P1Jump.canceled += P1JumpAction;

            // DASH
            playerInputActions.Player.P1Dash.performed += P1DashAction;
            playerInputActions.Player.P1Dash.canceled += P1DashAction;

            // UTILITY
            playerInputActions.Player.P1Utility.performed += P1UtilityAction;
            playerInputActions.Player.P1Utility.canceled += P1UtilityAction;

            // RELOAD
            playerInputActions.Player.P1Reload.performed += P1ReloadAction;

            // COUNTDOWN
            playerInputActions.Player.P1Countdown.performed += P1CountdownAction;

            // DEBUG ROUND END
            playerInputActions.Player.RoundEnd.performed += EndRound;
        }
        else if (playerNumber == "Two")
        {
            // MOVEMENT
            playerInputActions.Player.P2Move.performed += P2MoveAction;
            playerInputActions.Player.P2Move.canceled += P2MoveAction;

            // SHOOT
            playerInputActions.Player.P2Shoot.performed += P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled += P2ShootAction;

            // JUMP
            playerInputActions.Player.P2Jump.performed += P2JumpAction;
            playerInputActions.Player.P2Jump.canceled += P2JumpAction;

            // DASH
            playerInputActions.Player.P2Dash.performed += P2DashAction;
            playerInputActions.Player.P2Dash.canceled += P2DashAction;

            // UTILITY
            playerInputActions.Player.P2Utility.performed += P2UtilityAction;
            playerInputActions.Player.P2Utility.canceled += P2UtilityAction;

            // RELOAD
            playerInputActions.Player.P2Reload.performed += P2ReloadAction;

            // COUNTDOWN
            playerInputActions.Player.P2Countdown.performed += P2CountdownAction;
        }

        // PAUSE
        playerInputActions.Player.Pause.performed += PauseAction;
        playerInputActions.Pause.Resume.performed += ResumeAction;
    }

    private void UnsubscribeInputActions()
    {
        if (playerInputActions == null)
            return;

        if (playerNumber == "One")
        {
            playerInputActions.Player.P1Move.performed -= P1MoveAction;
            playerInputActions.Player.P1Move.canceled -= P1MoveAction;

            playerInputActions.Player.P1Shoot.performed -= P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled -= P1ShootAction;

            playerInputActions.Player.P1Jump.performed -= P1JumpAction;
            playerInputActions.Player.P1Jump.canceled -= P1JumpAction;

            playerInputActions.Player.P1Dash.performed -= P1DashAction;
            playerInputActions.Player.P1Dash.canceled -= P1DashAction;

            playerInputActions.Player.P1Utility.performed -= P1UtilityAction;
            playerInputActions.Player.P1Utility.canceled -= P1UtilityAction;

            playerInputActions.Player.P1Reload.performed -= P1ReloadAction;

            playerInputActions.Player.P1Countdown.performed -= P1CountdownAction;

            playerInputActions.Player.RoundEnd.performed -= EndRound;
        }
        else if (playerNumber == "Two")
        {
            playerInputActions.Player.P2Move.performed -= P2MoveAction;
            playerInputActions.Player.P2Move.canceled -= P2MoveAction;

            playerInputActions.Player.P2Shoot.performed -= P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled -= P2ShootAction;

            playerInputActions.Player.P2Jump.performed -= P2JumpAction;
            playerInputActions.Player.P2Jump.canceled -= P2JumpAction;

            playerInputActions.Player.P2Dash.performed -= P2DashAction;
            playerInputActions.Player.P2Dash.canceled -= P2DashAction;

            playerInputActions.Player.P2Utility.performed -= P2UtilityAction;
            playerInputActions.Player.P2Utility.canceled -= P2UtilityAction;

            playerInputActions.Player.P2Reload.performed -= P2ReloadAction;

            playerInputActions.Player.P2Countdown.performed -= P2CountdownAction;
        }

        playerInputActions.Player.Pause.performed -= PauseAction;
        playerInputActions.Pause.Resume.performed -= ResumeAction;
    }

    #endregion

    #region Input Actions

    private void P1MoveAction(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        playerController.P1MoveActionServerRpc(moveInput);
    }

    private void P2MoveAction(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        playerController.P2MoveActionServerRpc(moveInput);
    }

    private void P1ShootAction(InputAction.CallbackContext context)
    {
        bool isShooting = context.phase != InputActionPhase.Canceled;
        playerController.P1ShootActionServerRpc(isShooting ? 1f : 0f);
    }

    private void P2ShootAction(InputAction.CallbackContext context)
    {
        bool isShooting = context.phase != InputActionPhase.Canceled;
        playerController.P2ShootActionServerRpc(isShooting ? 1f : 0f);
    }

    private void P1JumpAction(InputAction.CallbackContext context)
    {
        bool isJumping = context.phase != InputActionPhase.Canceled;
        playerController.P1JumpInputServerRpc(isJumping ? 1f : 0f);
    }

    private void P2JumpAction(InputAction.CallbackContext context)
    {
        bool isJumping = context.phase != InputActionPhase.Canceled;
        playerController.P2JumpInputServerRpc(isJumping ? 1f : 0f);
    }

    private void P1DashAction(InputAction.CallbackContext context)
    {
        bool isDashing = context.phase != InputActionPhase.Canceled;
        playerController.P1DashInputServerRpc(isDashing ? 1f : 0f);
    }

    private void P2DashAction(InputAction.CallbackContext context)
    {
        bool isDashing = context.phase != InputActionPhase.Canceled;
        playerController.P2DashInputServerRpc(isDashing ? 1f : 0f);
    }

    private void P1UtilityAction(InputAction.CallbackContext context)
    {
        bool isUsingUtility = context.phase != InputActionPhase.Canceled;
        playerController.P1UtilityInputServerRpc(isUsingUtility ? 1f : 0f);
    }

    private void P2UtilityAction(InputAction.CallbackContext context)
    {
        bool isUsingUtility = context.phase != InputActionPhase.Canceled;
        playerController.P2UtilityInputServerRpc(isUsingUtility ? 1f : 0f);
    }

    private void P1ReloadAction(InputAction.CallbackContext context)
    {
        playerController.P1ReloadInputServerRpc(1f);
    }

    private void P2ReloadAction(InputAction.CallbackContext context)
    {
        playerController.P2ReloadInputServerRpc(1f);
    }

    private void P1CountdownAction(InputAction.CallbackContext context)
    {
        playerController.P1CountdownInputServerRpc();
    }

    private void P2CountdownAction(InputAction.CallbackContext context)
    {
        playerController.P2CountdownInputServerRpc();
    }

    private void EndRound(InputAction.CallbackContext context)
    {
        GameManager.Instance.OnRoundEnd.Invoke();
    }

    private void PauseAction(InputAction.CallbackContext context)
    {
        SwitchActionMap(EPlayerState.Paused);
    }

    private void ResumeAction(InputAction.CallbackContext context)
    {
        SwitchActionMap(EPlayerState.Moving);
    }

#endregion



    public override void OnNetworkDespawn()
    {
        if (NetworkManager != null &&
            NetworkManager.NetworkTickSystem != null)
        {
            NetworkManager.NetworkTickSystem.Tick -= Tick;
        }
    }
}