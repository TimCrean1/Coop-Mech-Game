using Unity.VisualScripting;
using UnityEngine;

public class UtilityManagerScript : MonoBehaviour
{
    [SerializeField] private BaseUtility p1Utility;
    [SerializeField] private BaseUtility p2Utility;
    [SerializeField] private CharacterMovement characterMovement;
    public void P1Utility()
    {
        p1Utility.ActivateUtility();
    }

    public void P2Utility()
    {
        p2Utility.ActivateUtility();
    }

    public CharacterMovement GetCharacterMovement()
    {
        return characterMovement;
    }
}
