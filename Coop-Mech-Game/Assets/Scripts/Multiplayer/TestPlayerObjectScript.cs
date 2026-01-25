using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public PlayerController playerController;
    [SerializeField]private bool isPlayerOne;
    private Vector2 mousePos;
    private float mouseX;
    private float mouseY;
    
    private NetworkVariable<Vector2> mouseNetPos = new NetworkVariable<Vector2>();
    private PlayerInputActions playerInputActions;

    public override void OnNetworkSpawn()
    {
        playerController = GameManager.Instance._playerControllers[0];

        if (GameManager.Instance._playerControllers[0].player1 == null)
        {
            GameManager.Instance._playerControllers[0].player1 = this;
            isPlayerOne = true;
        }
        else
        {
            GameManager.Instance._playerControllers[0].player2 = this;
            isPlayerOne = false;
        }
        playerInputActions = new PlayerInputActions();
        SubscribeInputActions();
        playerInputActions.Player.Enable();
    }

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
        if (isPlayerOne)
        {
            playerInputActions.Player.P1Move.started += playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled += playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.performed += playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled += playerController.P1ShootAction;
        }
        else
        {
            playerInputActions.Player.P2Move.started += playerController.P2MoveAction;
            playerInputActions.Player.P2Move.canceled += playerController.P2MoveAction;

            playerInputActions.Player.P2Shoot.started += playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled += playerController.P2ShootAction;
        }
    }

    private void UnsubscribeInputActions()
    {
        if (isPlayerOne){
            playerInputActions.Player.P1Move.started -= playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled -= playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.started -= playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled -= playerController.P1ShootAction;
        }
        else {
            playerInputActions.Player.P2Move.started -= playerController.P2MoveAction;
            playerInputActions.Player.P2Move.canceled -= playerController.P2MoveAction;

            playerInputActions.Player.P2Shoot.started -= playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled -= playerController.P2ShootAction;
        }
    }

    void Update()
    {
        // Get mouse position in screen space and normalize
        mousePos = Input.mousePosition;
        mousePos.x = mousePos.x/Screen.width;
        mousePos.y = mousePos.y/Screen.height;
        mouseNetPos.Value = mousePos;
        // Send mouse position to PlayerController
        if (isPlayerOne)
        {
            playerController.ProcessMouse1Input(mouseNetPos.Value);
        }
        else
        {
            playerController.ProcessMouse2Input(mouseNetPos.Value);
        }
    }
}