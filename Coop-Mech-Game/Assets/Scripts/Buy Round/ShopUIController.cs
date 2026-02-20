using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

enum whichTeam
{
    Red,
    Blue
}

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private ShopObjectSO[] shopOptions; // assign 4 in inspector
    private VisualElement root;
    private whichTeam whichTeam;

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        SetupCards();
    }

    void SetupCards()
    {
        for (int i = 0; i < shopOptions.Length; i++)
        {
            int index = i;

            var card = root.Q<Button>($"card{i}");
            var imageElement = root.Q<VisualElement>($"image{i}");
            var typeLabel = root.Q<Label>($"type{i}");
            var playerImageContainer = root.Q<VisualElement>($"playerImageContainer{i}");
            var player1Image = root.Q<Image>($"p1Image{i}");
            var player2Image = root.Q<Image>($"p2Image{i}");

            player1Image.SetEnabled(false);
            player2Image.SetEnabled(false);

            var shopObject = shopOptions[i];

            // Set Type Text
            typeLabel.text = shopObject.GetObjectType().ToString();

            // If you changed to Sprite:
            if (shopObject is ShopObjectSO so && so.GetItemSO() != null)
            {
                // If you replaced Image with Sprite:
                var spriteField = so.GetType().GetField("icon", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);

                if (spriteField != null)
                {
                    Sprite sprite = (Sprite)spriteField.GetValue(so);
                    if (sprite != null)
                        imageElement.style.backgroundImage = 
                            new StyleBackground(sprite);
                }
            }

            // Button click
            card.clicked += () => OnItemSelected(shopObject);
        }
    }

    void OnItemSelected(ShopObjectSO selected)
    {
        Debug.Log("Selected: " + selected.name);

        // Do your logic here:
        // Give item
        // Add stat
        // Equip weapon
        // etc.

        if (selected.GetObjectType() == ShopObjectType.Weapon)
        {
            
        }
        else if (selected.GetObjectType() == ShopObjectType.Utility)
        {
            
        }
        else if (selected.GetObjectType() == ShopObjectType.Stat)
        {
            
        }
        else if (selected.GetObjectType() == ShopObjectType.Special)
        {
            
        }
    }
}