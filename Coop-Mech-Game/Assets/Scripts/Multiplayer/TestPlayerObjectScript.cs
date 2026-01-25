using Unity.Netcode;
using UnityEngine;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public PlayerController playerController;

    private PlayerInputActions playerInputActions;

    private NetworkVariable<int> playerIndex = new NetworkVariable<int>();
    private NetworkVariable<Vector2> mouseNetPos = new NetworkVariable<Vector2>();

    private bool controlsInitialized = false;

    #region NETWORK SPAWN

    public override void OnNetworkSpawn()
    {
        // Server decides which player number this object is
        if (IsServer)
        {
            playerIndex.Value = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
        }

        // Listen for index being set on all clients
        playerIndex.OnValueChanged += OnPlayerIndexSet;

        if (!IsOwner) return;

        playerController = GameManager.Instance._playerControllers[0];
        playerInputActions = new PlayerInputActions();
    }

    private void OnPlayerIndexSet(int oldValue, int newValue)
    {
        if (!IsOwner || controlsInitialized) return;

        SubscribeInputActions(newValue);
        playerInputActions.Player.Enable();
        controlsInitialized = true;
    }

    #endregion

    #region INPUT SETUP

    private void SubscribeInputActions(int index)
    {
        if (index == 0) // Player 1
        {
            playerInputActions.Player.P1Move.started += playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled += playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.performed += playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled += playerController.P1ShootAction;
        }
        else if (index == 1) // Player 2
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

        if (index == 0)
        {
            playerInputActions.Player.P1Move.started -= playerController.P1MoveAction;
            playerInputActions.Player.P1Move.canceled -= playerController.P1MoveAction;

            playerInputActions.Player.P1Shoot.started -= playerController.P1ShootAction;
            playerInputActions.Player.P1Shoot.canceled -= playerController.P1ShootAction;
        }
        else if (index == 1)
        {
            playerInputActions.Player.P2Move.started -= playerController.P2MoveAction;
            playerInputActions.Player.P2Move.canceled -= playerController.P2MoveAction;

            playerInputActions.Player.P2Shoot.started -= playerController.P2ShootAction;
            playerInputActions.Player.P2Shoot.canceled -= playerController.P2ShootAction;
        }
    }

    private void OnDisable()
    {
        UnsubscribeInputActions();
    }

    #endregion

    #region ACTION MAP SWITCHING

    public void SwitchActionMap(EPlayerState state)
    {
        if (!IsOwner) return;

        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (state)
        {
            case EPlayerState.Moving:
                playerInputActions.Player.Enable();
                break;

            case EPlayerState.Paused:
                playerInputActions.UI.Enable();
                break;
        }
    }

    #endregion

    #region MOUSE NETWORKING

    void Update()
    {
        if (!IsOwner) return;

        Vector2 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        SendMouseToServerRpc(mousePos);
    }

    [ServerRpc]
    private void SendMouseToServerRpc(Vector2 mousePos)
    {
        mouseNetPos.Value = mousePos;
        ApplyMouseInput(mousePos);
    }

    private void ApplyMouseInput(Vector2 mousePos)
    {
        if (playerIndex.Value == 0)
            playerController.ProcessMouse1Input(mousePos);
        else if (playerIndex.Value == 1)
            playerController.ProcessMouse2Input(mousePos);
    }

    #endregion
}