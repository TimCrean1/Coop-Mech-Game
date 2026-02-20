using UnityEngine.UIElements;
using UnityEngine;
using System.Reflection;

public enum ShopObjectType
{
    Weapon, Utility, Stat, Special
}

[CreateAssetMenu(fileName = "ShopObjectSO", menuName = "ScriptableObjects/ShopObjectSO")]

public class ShopObjectSO : ScriptableObject
{
    [SerializeField] private ShopObjectType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private ScriptableObject itemSO;

    public ShopObjectType GetObjectType()
    {
        return type;
    }
    public Sprite GetImage()
    {
        return icon;
    }
    public ScriptableObject GetItemSO()
    {
        return itemSO;
    }
}