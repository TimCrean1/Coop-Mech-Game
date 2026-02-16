using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static LobbyManager;

public class BootstrapScript : MonoBehaviour
{
    [SerializeField] public string playerIndex;
    [SerializeField] public string playerTeam;
    [SerializeField] public string playerNumber;
    private Lobby lobby;
    private Player localPlayer;
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
        LobbyManager.Instance.OnLobbyStartGame += GetDataOnStart;
    }

    public void GetDataOnStart(object sender, LobbyManager.LobbyEventArgs e)
    {
        lobby = LobbyManager.Instance.GetJoinedLobby();

        localPlayer = lobby.Players.Find(p =>
        p.Id == AuthenticationService.Instance.PlayerId);

        playerIndex = AuthenticationService.Instance.PlayerId;

        playerTeam = localPlayer.Data[KEY_PLAYER_TEAM].Value;

        playerNumber = localPlayer.Data[KEY_PLAYER_NUMBER].Value;
    }
}
