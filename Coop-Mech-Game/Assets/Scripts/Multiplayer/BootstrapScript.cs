using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static LobbyManager;

public class BootstrapScript : MonoBehaviour
{
    [SerializeField] private string playerIndex;
    [SerializeField] private string playerTeam;
    [SerializeField] private string playerNumber;
    public static BootstrapScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

        Player localPlayer = lobby.Players.Find(p =>
        p.Id == AuthenticationService.Instance.PlayerId);

        playerIndex = AuthenticationService.Instance.PlayerId;

        playerTeam = localPlayer.Data[KEY_PLAYER_TEAM].Value;

        playerNumber = localPlayer.Data[KEY_PLAYER_NUMBER].Value;
    }
}
