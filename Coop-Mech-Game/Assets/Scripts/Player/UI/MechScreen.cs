using UnityEngine;
using TMPro;

public class MechScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textToChange;

    public void ChangeText(string newText, bool concatinate)
    {
        if (concatinate)
        {
            textToChange.text = textToChange.text + newText;
        }
        else
        {
            textToChange.text = newText;
        }
    }

}
