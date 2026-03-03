using System; // Unnecessary using directive, can be removed
using TMPro; // For TextMeshProUGUI components
using Unity.VisualScripting; // Unnecessary using directive, can be removed
using UnityEngine; // For MonoBehaviour, SerializeField, etc.
using UnityEngine.UI; // For Image and Button components

// Displays a shop item in the UI
public class ShopItemDisplayScript : MonoBehaviour
{
    [Header("UI Object References")]
    [SerializeField] private Image itemImage; // Reference to the item's image
    [SerializeField] private TextMeshProUGUI itemName; // Reference to the item's name text
    [SerializeField] private TextMeshProUGUI itemDescription; // Reference to the item's description text
    [SerializeField] private Button button; // Reference to the button for interaction

    [Header("Manager Object References")]
    [SerializeField] private ShopManager shopManager; // Reference to the shop manager
    [SerializeField] private ShopItemSO item; // Reference to the shop item data

    // Initializes the display with item data and shop manager reference
    public void InitializeItem(ShopItemSO item, ShopManager shop)
    {
        shopManager = shop;
        itemImage.sprite = item.itemIcon;
        itemName.text = item.itemName;
        itemDescription.text = item.itemDescription;
        this.item = item;
    }

    // Called when the item is clicked
    public void OnClick()
    {
        shopManager.ShopItemClicked(item);
    }
}
