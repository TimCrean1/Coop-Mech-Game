using Unity.Netcode;
//using Unity.Services.Matchmaker.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Services.Authentication;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public PlayerController playerController;
    private Vector2 mousePos;
    [SerializeField] private string playerIndex;
    [SerializeField] private string playerTeam;
    [SerializeField] private string playerNumber;
    [SerializeField] private string idCheck;
    private PlayerInputActions playerInputActions;

    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

        Player localPlayer = lobby.Players.Find(p =>
        p.Id == AuthenticationService.Instance.PlayerId);

        playerIndex = AuthenticationService.Instance.PlayerId;

        playerTeam = localPlayer.Data[LobbyManager.KEY_PLAYER_TEAM].Value;

        playerNumber = localPlayer.Data[LobbyManager.KEY_PLAYER_NUMBER].Value;
        

        

        
        // for running code on tick rather than update
        NetworkManager.NetworkTickSystem.Tick += Tick;

        if (GameManager.Instance._playerControllers.Count < 0)
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

        } else if (playerTeam == "Blue")
        {
            playerController = GameManager.Instance._playerControllers[1];
            GameManager.Instance._playerControllers[1].baseCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[1].overlayCamera.gameObject.SetActive(true);
            GameManager.Instance._playerControllers[1].uiCanvas.gameObject.SetActive(true);
        }

        

        //if (GameManager.Instance._playerControllers[0].player1 == null)
        //{
        //    GameManager.Instance._playerControllers[0].player1 = this;
        //    isPlayerOne = true;
        //}
        //else
        //{
        //    GameManager.Instance._playerControllers[0].player2 = this;
        //    isPlayerOne = false;
        //}
        playerInputActions = new PlayerInputActions();
        SubscribeInputActions();
        playerInputActions.Player.Enable();
    }
    //[ServerRpc]
    //private void AskServerForIdServerRpc()
    //{

    //}
    
    void OnDisable()
    {
        UnsubscribeInputActions();
        
    }

    public void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                Cursor.visible = false;
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

    private void SubscribeInputActions()
    {
        if (playerNumber == "One") 
        {
            // OwnerClientId 0 is player 1 of mech 1
            // OwnerClientId 2 is player 1 of mech 2

            // playerInputActions.Player.P1Move.started += playerController.P1MoveAction;
            // playerInputActions.Player.P1Move.canceled += playerController.P1MoveAction;

            // playerInputActions.Player.P1Shoot.performed += playerController.P1ShootAction;
            // playerInputActions.Player.P1Shoot.canceled += playerController.P1ShootAction;

            playerInputActions.Player.P1Move.started += P1MoveAction;
            playerInputActions.Player.P1Move.performed += P1MoveAction;
            playerInputActions.Player.P1Move.canceled += P1MoveAction;

            playerInputActions.Player.P1Shoot.started += P1ShootAction;
            playerInputActions.Player.P1Shoot.performed += P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled += P1ShootAction;
        }
        else if (playerNumber == "Two") 
        {
            // playerInputActions.Player.P2Move.started += playerController.P2MoveAction;
            // playerInputActions.Player.P2Move.canceled += playerController.P2MoveAction;

            // playerInputActions.Player.P2Shoot.started += playerController.P2ShootAction;
            // playerInputActions.Player.P2Shoot.canceled += playerController.P2ShootAction;

            playerInputActions.Player.P2Move.started += P2MoveAction;
            playerInputActions.Player.P2Move.performed += P2MoveAction;
            playerInputActions.Player.P2Move.canceled += P2MoveAction;

            playerInputActions.Player.P2Shoot.started += P2ShootAction;
            playerInputActions.Player.P2Shoot.performed += P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled += P2ShootAction;
        }
    }

    private void UnsubscribeInputActions()
    {
        if (playerNumber == "One")
        {
            // playerInputActions.Player.P1Move.started -= playerController.P1MoveAction;
            // playerInputActions.Player.P1Move.canceled -= playerController.P1MoveAction;
            playerInputActions.Player.P1Move.started -= P1MoveAction;
            playerInputActions.Player.P1Move.performed -= P1MoveAction;
            playerInputActions.Player.P1Move.canceled -= P1MoveAction;

            // playerInputActions.Player.P1Shoot.started -= playerController.P1ShootAction;
            // playerInputActions.Player.P1Shoot.canceled -= playerController.P1ShootAction;

            playerInputActions.Player.P1Shoot.started -= P1ShootAction;
            playerInputActions.Player.P1Shoot.performed -= P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled -= P1ShootAction;
        }
        else if (playerNumber == "Two")
        {
            // playerInputActions.Player.P2Move.started -= playerController.P2MoveAction;
            // playerInputActions.Player.P2Move.canceled -= playerController.P2MoveAction;
            playerInputActions.Player.P2Move.started -= P2MoveAction;
            playerInputActions.Player.P2Move.performed -= P2MoveAction;
            playerInputActions.Player.P2Move.canceled -= P2MoveAction;

            // playerInputActions.Player.P2Shoot.started -= playerController.P2ShootAction;
            // playerInputActions.Player.P2Shoot.canceled -= playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.started -= P2ShootAction;
            playerInputActions.Player.P2Shoot.performed -= P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled -= P2ShootAction;
        }
    }
    //[ClientRpc]
    //private void SendMousePosToServerRpc(Vector2 mousePos)
    //{

    //}
    void Tick()
    {
        if (!IsOwner) { return; }
        // Get mouse position in screen space and normalize
        mousePos = Input.mousePosition;
        mousePos.x = mousePos.x / Screen.width;
        mousePos.y = mousePos.y / Screen.height;
        //mouseNetPos.Value = mousePos;

        // Send mouse position to PlayerController
        if (playerNumber == "One")
        {
            playerController.ProcessMouse1InputServerRpc(mousePos);
            //Debug.Log("player one" + mousePos);

        }
        else if (playerNumber == "Two")
        {
            playerController.ProcessMouse2InputServerRpc(mousePos);
            //Debug.Log("player two" + mousePos);
        }
        if (playerInputActions == null)
        {
            Debug.Log("playerInputActions is null");
        }
        //Debug.Log($"Tick: {NetworkManager.LocalTime.Tick}");
    }
    void Update()
    {
        //if (!IsOwner) { return; }
        //// Get mouse position in screen space and normalize
        //mousePos = Input.mousePosition;
        //mousePos.x = mousePos.x / Screen.width;
        //mousePos.y = mousePos.y / Screen.height;
        ////mouseNetPos.Value = mousePos;

        //// Send mouse position to PlayerController
        //if (OwnerClientId == 0 || OwnerClientId == 1)
        //{
        //    playerController.ProcessMouse1InputServerRpc(mousePos);
        //    //Debug.Log("player one" + mousePos);

        //}
        //else if (OwnerClientId == 2 || OwnerClientId == 3)
        //{
        //    playerController.ProcessMouse2InputServerRpc(mousePos);
        //    //Debug.Log("player two" + mousePos);
        //}
        //if (playerInputActions == null)
        //{
        //    Debug.Log("playerInputActions is null");
        //}
    }

    #region Input Actions
    private void P1MoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("P1 Move Action triggered");
        Vector2 moveInput = context.ReadValue<Vector2>();
        playerController.P1MoveActionServerRpc(moveInput);
    }

    private void P2MoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("P2 Move Action triggered");
        Vector2 moveInput = context.ReadValue<Vector2>();
        playerController.P2MoveActionServerRpc(moveInput);
    }

    private void P1ShootAction(InputAction.CallbackContext context)
    {
        float isShooting = context.ReadValue<float>();
        playerController.P1ShootActionServerRpc(isShooting);
    }

    private void P2ShootAction(InputAction.CallbackContext context)
    {
        float isShooting = context.ReadValue<float>();
        playerController.P2ShootActionServerRpc(isShooting);
    }
    #endregion
}