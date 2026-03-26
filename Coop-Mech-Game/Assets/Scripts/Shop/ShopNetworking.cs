using Unity.Netcode;
using UnityEngine;

public class ShopNetworking : NetworkBehaviour
{
    public static ShopNetworking Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;
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

        ShopManager.Instance.UpdateReadyText(readyPlayerCount.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientRoundEventRpc()
    {
        ShopManager.Instance.TriggerRoundChange();
    }

    [ClientRpc]
    public void OpenShopClientRpc()
    {
        ShopManager.Instance.OpenShopClient();
    }

    [ClientRpc]
    public void CloseShopClientRpc()
    {
        ShopManager.Instance.CloseShopClient();
    }

    #endregion
}