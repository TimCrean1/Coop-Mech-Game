using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ShopNetworking : NetworkBehaviour
{
    public static ShopNetworking Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();
    public bool isTestScene;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (isTestScene && !NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!isTestScene)
        {
            if (!IsServer)
            {
                GameManager.Instance.OnBuyRoundStart.AddListener(OpenShopClientRpc);
            }
            // else { Debug.LogError("idk... client"); }

            if (IsServer)
            {
                GameManager.Instance.OnBuyRoundStart.AddListener(() =>
                {
                    ShopManager.Instance.OpenShop();
                    OpenShopClientRpc();
                });
            }
        }
        else if (isTestScene)
        {
            if (!IsServer)
            {
                OpenShopClientRpc();
            }
            if (IsServer)
            {
                ShopManager.Instance.OpenShop();
                OpenShopClientRpc();
            }

        }
        // else { Debug.LogError("idk... server"); }
    }

    #region RPCs

    [Rpc(SendTo.Server)]
    public void ChangeReadyPlayersServerRpc(int addNum)
    {
        readyPlayerCount.Value += addNum;

        Debug.Log(readyPlayerCount.Value + "/4 players ready");

        if (readyPlayerCount.Value >= 4)
        {
            ClientRoundEventRpc();
            readyPlayerCount.Value = 0;
        }

        // ShopManager.Instance.UpdateReadyText(readyPlayerCount.Value);
        UpdateReadyTextClientRpc(readyPlayerCount.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientRoundEventRpc()
    {
        ShopManager.Instance.TriggerRoundChange();
    }

    [Rpc(SendTo.NotServer)]
    public void OpenShopClientRpc()
    {
        ShopManager.Instance.OpenShopClient();
    }

    [Rpc(SendTo.NotServer)]
    public void CloseShopClientRpc()
    {
        ShopManager.Instance.CloseShopClient();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateReadyTextClientRpc(int count)
    {
        ShopManager.Instance.UpdateReadyText(count);
    }

    #endregion
}