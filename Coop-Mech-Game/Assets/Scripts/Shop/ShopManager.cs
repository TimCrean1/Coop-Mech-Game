using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.Events;

// Enum representing the current buy round type in the shop
public enum CurrentBuyRound
{
    Weapons,
    Utilities
}

// Manages the shop UI and item logic
public class ShopManager : NetworkBehaviour
{
    [Header("UI Variables")]
    [SerializeField] private Canvas shopCanvas; // Reference to the shop canvas
    [SerializeField] private Transform itemDisplayParent; // Parent transform for item UI elements
    [SerializeField] private GameObject itemPrefab; // Prefab for shop items
    [SerializeField] private Button nextRoundButton; // Button to proceed to the next buy round

    [SerializeField] private CurrentBuyRound currentBuyRound = CurrentBuyRound.Weapons; // Current buy round type
    [SerializeField] private List<ShopItemSO> allItems; // All available shop items
    // [SerializeField] private PlayerController assignedMech; // Player's mech assigned to the shop

    private List<ShopItemSO> displayedItems; // Items currently displayed in the shop
    private List<GameObject> displayedItemObjects; // UI objects for the displayed items

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    private UnityEvent OnChangeRound;
    // void Awake()
    // {
    //     nextRoundButton.onClick.AddListener(NextRoundButtonClicked);
    // }
    void Start()
    {
        shopCanvas.gameObject.SetActive(true);
        shopCanvas.enabled = true;
        shopCanvas.enabled = false;
        nextRoundButton.gameObject.SetActive(true);
        nextRoundButton.enabled = true;
        //nextRoundButton.enabled = false;

        allItems = new List<ShopItemSO>();
        allItems.AddRange(Resources.LoadAll<ShopItemSO>("Shop Items"));
        displayedItems = new List<ShopItemSO>();
        displayedItemObjects = new List<GameObject>();
        OnChangeRound.AddListener(ChangeRound);
        // GameManager.Instance.OnRoundEnd.AddListener(OpenShop);
        // GameManager.Instance.OnRoundEnd.AddListener(OpenShopClientRpc);
    }

    public override void OnNetworkSpawn()
    {
        nextRoundButton.onClick.AddListener(NextRoundButtonClicked);

        if (IsOwner) GameManager.Instance.OnRoundEnd.AddListener(OpenShop);
        GameManager.Instance.OnRoundEnd.AddListener(OpenShopClientRpc);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ChangeReadyPlayersServerRpc(int addNum)
    {
        readyPlayerCount.Value = readyPlayerCount.Value + addNum;
        Debug.Log(readyPlayerCount.Value + "/4 players ready");
        if(readyPlayerCount.Value >= 4)
        {
            OnChangeRound?.Invoke();
        }
    }

    // Opens the shop UI and initializes items for the current round
    public void OpenShop()
    {
        // if (IsOwner)
        // {
        //     ChangeReadyPlayersRpc(readyPlayerCount.Value * - 1);
        // }

        Debug.Log("Opening For Host");
        // shopCanvas.gameObject.SetActive(true);
        shopCanvas.enabled = true;
        InitializeBuyRound(currentBuyRound);
    }

    [ClientRpc]
    public void OpenShopClientRpc()
    {
        Debug.Log("Opening For Client");
        // shopCanvas.gameObject.SetActive(true);
        shopCanvas.enabled = true;
        InitializeBuyRound(currentBuyRound); 
    }

        
    

    // Closes the shop UI
    public void CloseShop()
    {
        // shopCanvas.gameObject.SetActive(false);
        shopCanvas.enabled = false;
    }
    [ClientRpc]
    public void CloseShopClientRpc()
    {
        // shopCanvas.gameObject.SetActive(false);
        shopCanvas.enabled = false;
    }

    // Instantiates and initializes a shop item UI element
    public void InitializeShopItem(ShopItemSO item)
    {
        GameObject newItem = Instantiate(itemPrefab, itemDisplayParent);
        newItem.GetComponent<ShopItemDisplayScript>().InitializeItem(item, this);
        displayedItemObjects.Add(newItem);
    }

    // Filters and sets displayed items based on the current buy round
    private void InitializeBuyRound(CurrentBuyRound round)
    {
        displayedItems.Clear();
        displayedItemObjects.ForEach(item => Destroy(item));
        displayedItemObjects.Clear();

        if (round == CurrentBuyRound.Weapons)
        {
            foreach (ShopItemSO item in allItems)
            {
                if (item.itemType == ItemType.Weapon)
                {
                    displayedItems.Add(item);
                }
            }
        }
        else if (round == CurrentBuyRound.Utilities)
        {
            foreach (ShopItemSO item in allItems)
            {
                if (item.itemType == ItemType.Utility)
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
        Debug.Log("Clicked " + item.itemName);
    }

    public void NextRoundButtonClicked()
    {
        ChangeReadyPlayersServerRpc(1);
        nextRoundButton.enabled = false;

    }

    private void ChangeRound()
    {
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
}
