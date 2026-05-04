
// Required namespaces
using System.Collections;
using UnityEngine;


// Manages player utility abilities, cooldowns, and activation sync
public class UtilityManagerScript : MonoBehaviour
{
    // Player 1's utility ability
    [SerializeField] private BaseUtility p1Utility;
    // Player 2's utility ability
    [SerializeField] private BaseUtility p2Utility;
    // If true, utility activation is synced (e.g., over network)
    [SerializeField] private bool utilityActivationSynced = true; // Set to false if utilities should activate immediately without waiting for network sync
    // Reference to the character movement script
    [SerializeField] private CharacterMovement characterMovement;
    // Is Player 1's utility ready to use?
    [SerializeField] private bool p1UtilReady = true;
    // Is Player 2's utility ready to use?
    [SerializeField] private bool p2UtilReady = true;
    // Cooldown time for Player 1's utility
    [SerializeField, Range(0.1f, 10)] private float p1UtilCooldownTime = 5;
    // Cooldown time for Player 2's utility
    [SerializeField, Range(0.1f, 10)] private float p2UtilCooldownTime = 5;
    // Reference to the audio manager for player sounds
    [SerializeField] private PlayerAudioManager audioManager;


    // Activates Player 1's utility if ready, then starts cooldown
    public void P1Utility()
    {
        if (p1UtilReady)
        {
            p1Utility.ActivateUtilityRpc(); // Activate the utility (possibly networked)
            p1UtilReady = false;
            StartCoroutine(P1CooldownCorotuine()); // Begin cooldown
            // audioManager.PlayP1UtilitySound();
        }
    }


    // Activates Player 2's utility if ready, then starts cooldown
    public void P2Utility()
    {
        if (p2UtilReady)
        {
            p2Utility.ActivateUtilityRpc(); // Activate the utility (possibly networked)
            p2UtilReady = false;
            StartCoroutine(P2CooldownCorotuine()); // Begin cooldown
            // audioManager.PlayP2UtilitySound();
        }
    }


    // Returns the CharacterMovement component reference
    public CharacterMovement GetCharacterMovement()
    {
        return characterMovement;
    }


    // Assigns a new utility to the specified player (0 = P1, 1 = P2)
    // TODO: Setup accessing cooldown time from object
    public void SetPlayerUtility(int playerNum, BaseUtility newUtility)
    {
        if (playerNum == 0)
        {
            p1Utility = newUtility;
        }
        else if (playerNum == 1)
        {
            p2Utility = newUtility;
        }
        else
        {
            Debug.LogError("Invalid player index");
        }
    }


    // Handles Player 1's utility cooldown
    private IEnumerator P1CooldownCorotuine()
    {
        yield return new WaitForSeconds(p1UtilCooldownTime);
        p1UtilReady = true;
    }


    // Handles Player 2's utility cooldown
    private IEnumerator P2CooldownCorotuine()
    {
        yield return new WaitForSeconds(p2UtilCooldownTime);
        p2UtilReady = true;
    }


    // Sets whether utility activation is synced
    public void SetUtilActivationSynced(bool synced)
    {
        utilityActivationSynced = synced;
    }


    // Returns whether utility activation is synced
    public bool IsUtilityActivationSynced()
    {
        return utilityActivationSynced;
    }
}
