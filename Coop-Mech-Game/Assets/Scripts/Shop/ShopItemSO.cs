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
    public GameObject leftItemPrefab;
    public GameObject rightItemPrefab;
    public Sprite itemIcon;
    public string itemDescription;
    public string itemName;
    public int itemIndex;
}
