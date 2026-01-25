using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public PlayerController playerController;
    [SerializeField]private bool isPlayerOne;
    [SerializeField]private bool isPlayerTwo;
    private Vector2 mousePos;
    //private float mouseX;
    //private float mouseY;

    private bool controlsInitialized = false;

    private NetworkVariable<int> playerIndex = new NetworkVariable<int>();
    private NetworkVariable<Vector2> mouseNetPos = new NetworkVariable<Vector2>();
    private PlayerInputActions playerInputActions;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        //playerController = GameManager.Instance._playerControllers[0];


        if (IsServer)
        {
            playerIndex.Value = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
        }

        // Listen for index being set on all clients
        playerIndex.OnValueChanged += OnPlayerIndexSet;

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
        playerController = GameManager.Instance._playerControllers[0];
        playerInputActions = new PlayerInputActions();
        //SubscribeInputActions();
        //playerInputActions.Player.Enable();
    }

    private void OnPlayerIndexSet(int oldValue, int newValue)
    {
        if (!IsOwner || controlsInitialized) return;
        SubscribeInputActions(newValue);
        playerInputActions.Player.Enable();
        controlsInitialized = true;
    }

    void OnDisable()
    {
        UnsubscribeInputActions();
    }

    public void SwitchActionMap(EPlayerState state)
    {

        if(!IsOwner) { return; }
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

    private void SubscribeInputActions(int index)
    {
        if (index == 0)
        {
            playerInputActions.Player.P1Move.started += playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled += playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.performed += playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled += playerController.P1ShootAction;
        }
        else if (index == 1)
        {
            playerInputActions.Player.P2Move.started += playerController.P2MoveAction;
            playerInputActions.Player.P2Move.canceled += playerController.P2MoveAction;

            playerInputActions.Player.P2Shoot.started += playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled += playerController.P2ShootAction;
        }
    }

    private void UnsubscribeInputActions()
    {
        if (!controlsInitialized) return;

        int index = playerIndex.Value;

        if (index == 0){
            playerInputActions.Player.P1Move.started -= playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled -= playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.started -= playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled -= playerController.P1ShootAction;
        }
        else if(index == 1) {
            playerInputActions.Player.P2Move.started -= playerController.P2MoveAction;
            playerInputActions.Player.P2Move.canceled -= playerController.P2MoveAction;

            playerInputActions.Player.P2Shoot.started -= playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled -= playerController.P2ShootAction;
        }
    }
    
    void Update()
    {
        if (!IsOwner) { return; }
        // Get mouse position in screen space and normalize
        mousePos = Input.mousePosition;
        mousePos.x = mousePos.x/Screen.width;
        mousePos.y = mousePos.y/Screen.height;
        //mouseNetPos.Value = mousePos;
        
        // Send mouse position to PlayerController
        if (isPlayerOne && !isPlayerTwo)
        {
            playerController.ProcessMouse1Input(mousePos);
        }
        else if(!isPlayerOne && isPlayerTwo)
        {
            playerController.ProcessMouse2Input(mousePos);
        }

        
    }

    [ServerRpc]
    private void SendMouseToServerRpc(Vector2 mousePos)
    {
        mouseNetPos.Value = mousePos;
        ApplyMouseInput(mousePos);
    }

    private void ApplyMouseInput(Vector2 mousePos)
    {
        if(playerIndex.Value == 0)
        {
            playerController.ProcessMouse1Input(mousePos);
        } else if(playerIndex.Value == 1)
        {
            playerController.ProcessMouse2Input(mousePos);
        }
    }
}