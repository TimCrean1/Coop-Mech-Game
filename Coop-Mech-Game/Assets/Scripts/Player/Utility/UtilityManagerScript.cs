using Unity.VisualScripting;
using UnityEngine;

public class UtilityManagerScript : MonoBehaviour
{
    [SerializeField] private BaseUtility p1Utility;
    [SerializeField] private BaseUtility p2Utility;
    [SerializeField] private CharacterMovement characterMovement;
    public void P1Utility()
    {
        p1Utility.ActivateUtilityRpc();
    }

    public void P2Utility()
    {
        p2Utility.ActivateUtilityRpc();
    }

    public CharacterMovement GetCharacterMovement()
    {
        return characterMovement;
    }

    public void SetPlayerUtility(int playerNum,  BaseUtility newUtility)
    {
        if(playerNum == 1)
        {
            p1Utility = newUtility;
        }
        else
        {
            p2Utility = newUtility;
        }
    }
}
