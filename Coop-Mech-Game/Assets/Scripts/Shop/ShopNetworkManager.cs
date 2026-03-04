using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ShopNetworkManager : NetworkBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Button nextRoundButton;
    public NetworkVariable<int> readyPlayersCount = new NetworkVariable<int>();
    public void OnNextButtonClicked()
    {
        AddReadyPlayersRpc(1);
        nextRoundButton.enabled = false;

        if (readyPlayersCount.Value >= 4)
        {
            if (shopManager.GetCurrentBuyRound() == CurrentBuyRound.Weapons)
            {
                shopManager.SetCurrentBuyRound(CurrentBuyRound.Utilities);
            }
            else
            {
                shopManager.SetCurrentBuyRound(CurrentBuyRound.Weapons);
                shopManager.CloseShop();
                return;
            }
            shopManager.InitializeBuyRound(shopManager.GetCurrentBuyRound());
            nextRoundButton.enabled = true;
        }
    }

    [Rpc(SendTo.Server)]
    public void AddReadyPlayersRpc(int addNum)
    {
        readyPlayersCount.Value = readyPlayersCount.Value + addNum;
    }
}
