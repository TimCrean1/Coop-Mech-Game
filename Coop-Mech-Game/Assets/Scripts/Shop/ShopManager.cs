using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

#region Enums
// Enum representing the current buy round type in the shop
public enum CurrentBuyRound
{
    Closed,
    Weapons,
    Utilities
}
#endregion

// Manages the shop UI and item logic
public class ShopManager : NetworkBehaviour
{
    #region Fields
    [Header("UI Variables")]
    [SerializeField] private Canvas shopCanvas; // Reference to the shop canvas
    [SerializeField] private Transform itemDisplayParent; // Parent transform for item UI elements
    [SerializeField] private GameObject itemPrefab; // Prefab for shop items
    [SerializeField] private Button nextRoundButton; // Button to proceed to the next buy round
    [SerializeField] private TextMeshProUGUI readyPlayersText;

    [SerializeField] private CurrentBuyRound currentBuyRound = CurrentBuyRound.Weapons; // Current buy round type
    [SerializeField] public List<ShopItemSO> allItems; // All available shop items

    private List<ShopItemSO> displayedItems; // Items currently displayed in the shop
    private List<GameObject> displayedItemObjects; // UI objects for the displayed items

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    public UnityEvent OnChangeRound;
    public UnityEvent OnShopEnd;
    #endregion
    #region Singleton

    private static ShopManager _instance = null;

    public static ShopManager Instance
    {
        get { return _instance; }
    }

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
        shopCanvas.enabled = true;
        shopCanvas.enabled = false;
        nextRoundButton.gameObject.SetActive(true);
        nextRoundButton.enabled = true;

        allItems = new List<ShopItemSO>();
        allItems.AddRange(Resources.LoadAll<ShopItemSO>("Shop Items"));
        displayedItems = new List<ShopItemSO>();
        displayedItemObjects = new List<GameObject>();
        OnChangeRound.AddListener(ChangeRound);     
    }

    public override void OnNetworkSpawn()
    {
        nextRoundButton.onClick.AddListener(NextRoundButtonClicked);

        if (IsOwner) GameManager.Instance.OnBuyRoundStart.AddListener(OpenShop);
        GameManager.Instance.OnBuyRoundStart.AddListener(OpenShopClientRpc);
    }
    #endregion

    #region RPCs
    [Rpc(SendTo.Server)]
    public void ChangeReadyPlayersServerRpc(int addNum)
    {
        readyPlayerCount.Value += addNum;
        Debug.Log(readyPlayerCount.Value + "/4 players ready");
        if(readyPlayerCount.Value >= 4)
        {
            ClientRoundEventRpc();
            readyPlayerCount.Value = 0;
        }
        readyPlayersText.text = $"{readyPlayerCount.Value}/4 Players Ready";
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientRoundEventRpc()
    {
        OnChangeRound?.Invoke();
    }

    /// <summary>
    /// Opens the shop UI for all clients and initializes the items for the current buy round. Called by the host at the end of each round and executed on all clients through an RPC.
    /// </summary>

    [ClientRpc]
    public void OpenShopClientRpc()
    {
        // Debug.Log("Opening For Client");
        // GameManager.Instance.DisablePlayerMovement();
        shopCanvas.enabled = true;
        currentBuyRound = CurrentBuyRound.Weapons;
        InitializeBuyRound(currentBuyRound); 
    }

    [ClientRpc]
    public void CloseShopClientRpc()
    {
        // GameManager.Instance.EnablePlayerMovement();
        shopCanvas.enabled = false;
    }
    #endregion

    #region Shop UI
    // Opens the shop UI and initializes items for the current round
    public void OpenShop()
    {
        // Debug.Log("Opening For Host");
        // GameManager.Instance.DisablePlayerMovement();
        shopCanvas.enabled = true;
        currentBuyRound = CurrentBuyRound.Weapons;
        InitializeBuyRound(currentBuyRound);
    }

    // Closes the shop UI
    public void CloseShop()
    {
        // GameManager.Instance.EnablePlayerMovement();
        shopCanvas.enabled = false;
        currentBuyRound = CurrentBuyRound.Closed;
        OnShopEnd.Invoke();
    }
    #endregion

    #region Shop Item Logic
    // Instantiates and initializes a shop item UI element
    public void InitializeShopItem(ShopItemSO item)
    {
        GameObject newItem = Instantiate(itemPrefab, itemDisplayParent);
        //newItem.GetComponent<NetworkObject>().Spawn(true);
        newItem.GetComponent<ShopItemDisplayScript>().InitializeItem(item, this);
        displayedItemObjects.Add(newItem);
    }

    /// <summary>
    /// Filters and sets displayed items based on the current buy round, then initializes the shop UI for those items.
    /// </summary>
    /// <param name="round">The current buy round.</param>
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

    // Handles logic when a shop item is clicked
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
            Debug.LogError("Clicked item '" + item.itemName + "' has invalid itemType: " + item.itemType);
        }
    }
    #endregion

    #region Player Logic
    /// <summary>
    /// Grabs the player number and team of the local player to determine which PlayerController to reference for shop interactions.
    /// </summary>
    /// <returns>The player number and PlayerController of the local player.</returns>
    private Tuple<int,PlayerController> GrabPlayerFunction()
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
            Debug.LogError("Invalid team! Team '" + team + "' does not exist!");
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
            Debug.LogError("Player number " + num + "does not exist on team " + team);
            return new Tuple<int, PlayerController>(-1, null);
        }
    }
    #endregion

    #region Round Logic
    public void NextRoundButtonClicked()
    {
        ChangeReadyPlayersServerRpc(1);
        nextRoundButton.enabled = false;
    }

    private void ChangeRound()
    {
        if (currentBuyRound == CurrentBuyRound.Closed)
        {
            currentBuyRound = CurrentBuyRound.Weapons;
        }
        if (currentBuyRound == CurrentBuyRound.Weapons)
        {
            currentBuyRound = CurrentBuyRound.Utilities;
        }
        else
        {
            currentBuyRound = CurrentBuyRound.Weapons;
            CloseShop();
            CloseShopClientRpc();
            return;
        }
        InitializeBuyRound(currentBuyRound);
        nextRoundButton.enabled = true;
    }
    #endregion
}
