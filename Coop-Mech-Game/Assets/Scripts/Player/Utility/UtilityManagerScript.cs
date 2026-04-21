using System.Collections;
using UnityEngine;

public class UtilityManagerScript : MonoBehaviour
{
    [SerializeField] private BaseUtility p1Utility;
    [SerializeField] private BaseUtility p2Utility;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private bool p1UtilReady = true;
    [SerializeField] private bool p2UtilReady = true;
    [SerializeField, Range(0.1f, 10)] private float p1UtilCooldownTime = 5;
    [SerializeField, Range(0.1f, 10)] private float p2UtilCooldownTime = 5;

    public void P1Utility()
    {
        if (p1UtilReady)
        {
            p1Utility.ActivateUtilityRpc();
            p1UtilReady = false;
            StartCoroutine(P1CooldownCorotuine());
        }
    }

    public void P2Utility()
    {
        if (p2UtilReady)
        {
            p2Utility.ActivateUtilityRpc();
            p2UtilReady = false;
            StartCoroutine(P2CooldownCorotuine());
        }
    }

    public CharacterMovement GetCharacterMovement()
    {
        return characterMovement;
    }

    //TODO: Setup accessing cooldown time from object
    public void SetPlayerUtility(int playerNum, BaseUtility newUtility)
    {
        if (playerNum == 1)
        {
            p1Utility = newUtility;
        }
        else
        {
            p2Utility = newUtility;
        }
    }

    private IEnumerator P1CooldownCorotuine()
    {
        yield return new WaitForSeconds(p1UtilCooldownTime);
        p1UtilReady = true;
    }

    private IEnumerator P2CooldownCorotuine()
    {
        yield return new WaitForSeconds(p2UtilCooldownTime);
        p2UtilReady = true;
    }
}
