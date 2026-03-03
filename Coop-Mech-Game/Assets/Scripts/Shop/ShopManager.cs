using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] private GameObject itemPrefab; // Prefab for shop items
    [SerializeField] private Button nextRoundButton; // Button to proceed to the next buy round

    [SerializeField] private CurrentBuyRound currentBuyRound = CurrentBuyRound.Weapons; // Current buy round type
    [SerializeField] private List<ShopItemSO> allItems; // All available shop items
    [SerializeField] private PlayerController assignedMech; // Player's mech assigned to the shop

    private List<ShopItemSO> displayedItems; // Items currently displayed in the shop
    private List<GameObject> displayedItemObjects; // UI objects for the displayed items

    // Initializes shop items on start
    void Start()
    {
        allItems = new List<ShopItemSO>();
        allItems.AddRange(Resources.LoadAll<ShopItemSO>("Shop Items"));
    }

    // Opens the shop UI and initializes items for the current round
    public void OpenShop()
    {
        shopCanvas.gameObject.SetActive(true);
        InitializeBuyRound(currentBuyRound);
        foreach (ShopItemSO item in displayedItems)
        {
            InitializeShopItem(item);
        }
    }

    // Closes the shop UI
    public void CloseShop()
    {
        shopCanvas.gameObject.SetActive(false);
    }

    // Instantiates and initializes a shop item UI element
    public void InitializeShopItem(ShopItemSO item)
    {
        GameObject newItem = Instantiate(itemPrefab, shopCanvas.transform);
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
    }

    // Handles logic when a shop item is clicked
    public void ShopItemClicked(ShopItemSO item)
    {
        Debug.Log("Clicked " + item.itemName);
    }

    public void NextRoundButtonClicked()
    {
        if (currentBuyRound == CurrentBuyRound.Weapons)
        {
            currentBuyRound = CurrentBuyRound.Utilities;
        }
        else
        {
            CloseShop();
            return;
        }
        InitializeBuyRound(currentBuyRound);
    }
}
