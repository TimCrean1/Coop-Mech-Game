using UnityEngine;

public enum ItemType
{
    Utility,
    Weapon
}

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public ItemType itemType;
    public GameObject itemPrefab;
    public Sprite itemIcon;
    public string itemDescription;
    public string itemName;
}
