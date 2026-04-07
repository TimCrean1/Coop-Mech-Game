using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

#region Enums
public enum CurrentBuyRound
{
    Closed,
    Weapons,
    Utilities
}
#endregion

public class ShopManager : MonoBehaviour
{
    #region Fields

    [Header("UI Variables")]
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private Transform itemDisplayParent;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Button nextRoundButton;
    [SerializeField] private TextMeshProUGUI readyPlayersText;

    [SerializeField] private CurrentBuyRound currentBuyRound = CurrentBuyRound.Weapons;
    [SerializeField] public List<ShopItemSO> allItems;

    private List<ShopItemSO> displayedItems;
    private List<GameObject> displayedItemObjects;

    public UnityEvent OnChangeRound;
    public UnityEvent OnShopEnd;

    #endregion

    #region Singleton

    private static ShopManager _instance = null;

    public static ShopManager Instance => _instance;

    #endregion

    #region Unity Functions

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        currentBuyRound = CurrentBuyRound.Closed;
    }

    void Start()
    {
        shopCanvas.gameObject.SetActive(true);
        shopCanvas.enabled = false;

        nextRoundButton.gameObject.SetActive(true);
        nextRoundButton.enabled = true;
        nextRoundButton.onClick.AddListener(NextRoundButtonClicked);

        if (ShopNetworking.Instance.IsServer == false)
        {
            GameManager.Instance.OnBuyRoundStart.AddListener(OpenShopClient);
        }
        else {Debug.LogError("idk... client");}

        if (ShopNetworking.Instance.IsServer == true)
        {
            GameManager.Instance.OnBuyRoundStart.AddListener(() =>
            {
                OpenShop();
                ShopNetworking.Instance.OpenShopClientRpc();
            });
        }
        else {Debug.LogError("idk... server");}

        OnChangeRound.AddListener(ChangeRound);

        allItems = new List<ShopItemSO>();
        allItems.AddRange(Resources.LoadAll<ShopItemSO>("Shop Items"));

        displayedItems = new List<ShopItemSO>();
        displayedItemObjects = new List<GameObject>();
    }

    #endregion

    #region Networking Hooks (Called by ShopNetworking)

    public void UpdateReadyText(int count)
    {
        readyPlayersText.text = $"{count}/4 Players Ready";
    }

    public void TriggerRoundChange()
    {
        OnChangeRound?.Invoke();
    }

    public void OpenShopClient()
    {
        Debug.Log("Opening shop remotely");
        shopCanvas.enabled = true;
        currentBuyRound = CurrentBuyRound.Weapons;
        InitializeBuyRound(currentBuyRound);
    }

    public void CloseShopClient()
    {
        shopCanvas.enabled = false;
    }

    #endregion

    #region Shop UI

    public void OpenShop()
    {
        Debug.Log("Opening shop locally");
        shopCanvas.enabled = true;
        currentBuyRound = CurrentBuyRound.Weapons;
        InitializeBuyRound(currentBuyRound);
    }

    public void CloseShop()
    {
        shopCanvas.enabled = false;
        currentBuyRound = CurrentBuyRound.Closed;
        OnShopEnd.Invoke();
    }

    #endregion

    #region Shop Item Logic

    public void InitializeShopItem(ShopItemSO item)
    {
        GameObject newItem = Instantiate(itemPrefab, itemDisplayParent);
        newItem.GetComponent<ShopItemDisplayScript>().InitializeItem(item, this);
        displayedItemObjects.Add(newItem);
    }

    private void InitializeBuyRound(CurrentBuyRound round)
    {
        Tuple<int, PlayerController> playerData = GrabPlayerFunction();

        readyPlayersText.text = "0/4 Players Ready";

        displayedItems.Clear();
        displayedItemObjects.ForEach(item => Destroy(item));
        displayedItemObjects.Clear();

        nextRoundButton.enabled = true;

        if (round == CurrentBuyRound.Weapons)
        {
            foreach (ShopItemSO item in allItems)
            {
                if (item.itemType == ItemType.Weapon && item.playerID == playerData.Item1)
                {
                    displayedItems.Add(item);
                }
            }
        }
        else if (round == CurrentBuyRound.Utilities)
        {
            foreach (ShopItemSO item in allItems)
            {
                if (item.itemType == ItemType.Utility && item.playerID == playerData.Item1)
                {
                    displayedItems.Add(item);
                }
            }
        }

        foreach (ShopItemSO item in displayedItems)
        {
            InitializeShopItem(item);
        }
    }

    public void ShopItemClicked(ShopItemSO item)
    {
        Tuple<int, PlayerController> playerData = GrabPlayerFunction();

        if (item.itemType == ItemType.Weapon)
        {
            playerData.Item2.ChangeWeapon(item, playerData.Item1);
        }
        else if (item.itemType == ItemType.Utility)
        {
            playerData.Item2.ChangeUtility(item, playerData.Item1);
        }
        else
        {
            Debug.LogError($"Invalid item type on {item.itemName}");
        }
    }

    #endregion

    #region Player Logic

    private Tuple<int, PlayerController> GrabPlayerFunction()
    {
        var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];
        var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();

        string team = playerObject.GetPlayerTeam();
        string num = playerObject.GetPlayerNum();

        PlayerController controller;

        if (team == "Red")
        {
            controller = GameManager.Instance._playerControllers[0];
        }
        else if (team == "Blue")
        {
            controller = GameManager.Instance._playerControllers[1];
        }
        else
        {
            Debug.LogError($"Invalid team: {team}");
            return new Tuple<int, PlayerController>(-1, null);
        }

        if (num == "One")
        {
            return new Tuple<int, PlayerController>(0, controller);
        }
        else if (num == "Two")
        {
            return new Tuple<int, PlayerController>(1, controller);
        }
        else
        {
            Debug.LogError($"Invalid player number {num} on team {team}");
            return new Tuple<int, PlayerController>(-1, null);
        }
    }

    #endregion

    #region Round Logic

    public void NextRoundButtonClicked()
    {
        ShopNetworking.Instance.ChangeReadyPlayersServerRpc(1);
        nextRoundButton.enabled = false;
    }

    private void ChangeRound()
    {
        if (currentBuyRound == CurrentBuyRound.Closed)
        {
            currentBuyRound = CurrentBuyRound.Weapons;
        }
        else if (currentBuyRound == CurrentBuyRound.Weapons)
        {
            currentBuyRound = CurrentBuyRound.Utilities;
        }
        else
        {
            currentBuyRound = CurrentBuyRound.Weapons;

            CloseShop();
            ShopNetworking.Instance.CloseShopClientRpc();
            return;
        }

        InitializeBuyRound(currentBuyRound);
        nextRoundButton.enabled = true;
    }

    #endregion
}