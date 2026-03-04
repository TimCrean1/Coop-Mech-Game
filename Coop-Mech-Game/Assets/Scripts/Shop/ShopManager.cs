using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

// Enum representing the current buy round type in the shop
public enum CurrentBuyRound
{
    Weapons,
    Utilities
}

// Manages the shop UI and item logic
public class ShopManager : MonoBehaviour
{
    [Header("UI Variables")]
    [SerializeField] private Canvas shopCanvas; // Reference to the shop canvas
    [SerializeField] private Transform itemDisplayParent; // Parent transform for item UI elements
    [SerializeField] private GameObject itemPrefab; // Prefab for shop items
    [SerializeField] private ShopNetworkManager shopNetworkManager;

    [SerializeField] private CurrentBuyRound currentBuyRound = CurrentBuyRound.Weapons; // Current buy round type
    [SerializeField] private List<ShopItemSO> allItems; // All available shop items
    // [SerializeField] private PlayerController assignedMech; // Player's mech assigned to the shop

    private List<ShopItemSO> displayedItems; // Items currently displayed in the shop
    private List<GameObject> displayedItemObjects; // UI objects for the displayed items

    // void Awake()
    // {
    //     nextRoundButton.onClick.AddListener(NextRoundButtonClicked);
    // }
    void Start()
    {
        shopCanvas.gameObject.SetActive(true);
        shopCanvas.enabled = true;
        shopCanvas.enabled = false;

        allItems = new List<ShopItemSO>();
        allItems.AddRange(Resources.LoadAll<ShopItemSO>("Shop Items"));
        displayedItems = new List<ShopItemSO>();
        displayedItemObjects = new List<GameObject>();
    }

    // Opens the shop UI and initializes items for the current round
    public void OpenShop()
    {
        Debug.Log("Opening For Host");
        shopCanvas.enabled = true;
        InitializeBuyRound(currentBuyRound);
    }

    // Closes the shop UI
    public void CloseShop()
    {
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
    public void InitializeBuyRound(CurrentBuyRound round)
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

    #region Getters and Setters

    public void SetCurrentBuyRound(CurrentBuyRound newRound)
    {
        currentBuyRound = newRound;
    }

    public CurrentBuyRound GetCurrentBuyRound()
    {
        return currentBuyRound;
    }

    #endregion
}
