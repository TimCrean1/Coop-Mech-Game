using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    public static LobbyUI Instance { get; private set; }

    [SerializeField] Button createLobbyButton;
    [SerializeField] Button joinLobbyButton;
    [SerializeField] Button refreshLobbyButton;


    private void Awake()
    {
        Instance = this;

        createLobbyButton.onClick.AddListener(() =>
        {
            TestLobby.Instance.CreateLobby();
            Hide();
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            TestLobby.Instance.QuickJoinLobby();
            Hide();
        });

        refreshLobbyButton.onClick.AddListener(() =>
        {
            //TestLobby.Instance.PrintPlayers();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
