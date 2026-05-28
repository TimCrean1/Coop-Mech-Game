using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    [SerializeField] private Canvas canvas;
    public bool isShowing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isShowing = canvas.enabled;
    }

    public void Show()
    {
        canvas.enabled = true;
        isShowing = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
        isShowing = false;
    }

    public void ResumeButtonCallback()
    {
        var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];
        if (client.PlayerObject.GetComponent<TestPlayerObjectScript>())
        {
            var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();
            playerObject.SwitchActionMap(EPlayerState.Moving);
        }
    }

    public void QuitButtonCallback()
    {
        GameManager.Instance.OnGameEndClientRpc();
        GameManager.Instance.OnGameEndServerRpc();
    }
}
