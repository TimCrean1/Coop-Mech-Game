using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ScrambleUtility : BaseUtility
{

    [SerializeField] private float scrambleDuration = 5f;
    [SerializeField] private CharacterMovement _owningCharacter;
    [SerializeField] private PlayerController hitPlayerController;

    [Rpc(SendTo.ClientsAndHost)]
    public override void ActivateUtilityRpc()
    {
        if (!UtilityConditionsMet())
            return;

        Debug.Log("ScrambleUtility activated!");

        // Shoot a raycast forwards from the owning character's position
        if (_owningCharacter != null)
        {
            Vector3 origin = _owningCharacter.transform.position;
            Vector3 direction = _owningCharacter.transform.forward;
            float rayDistance = 100f;
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, rayDistance))
            {
                if (hit.collider.GetComponent<PlayerController>() != null)
                {
                    Debug.Log("ScrambleUtility hit a player! Applying scramble effect.");
                    hitPlayerController = hit.collider.GetComponent<PlayerController>();
                    StartCoroutine(ApplyScrambleEffect());
                }
                else
                {
                    Debug.Log("ScrambleUtility raycast hit something, but it was not a player.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
        else
        {
            Debug.LogWarning("ScrambleUtility: _owningCharacter is null, cannot shoot raycast.");
        }

        PlayUtilitySound();
    }

    protected override bool UtilityConditionsMet()
    {
        if (utilityManager == null)
        {
            Debug.LogError("ScrambleUtility: utilityManager is NULL.");
            return false;
        }
        else if (utilityManager.IsUtilityActivationSynced())
        {
            Debug.Log("ScrambleUtility: Utility activation is synced. Activating immediately.");
            return true;
        }
        else
        {
            Debug.Log("ScrambleUtility: Utility activation is not synced. Not activating utility.");
            return false;
        }
    }

    private IEnumerator ApplyScrambleEffect()
    {
        if (hitPlayerController != null)
        {
            hitPlayerController.SetScramble(true);
            yield return new WaitForSeconds(scrambleDuration);
            hitPlayerController.SetScramble(false);
            hitPlayerController = null;
        }
    }
}
