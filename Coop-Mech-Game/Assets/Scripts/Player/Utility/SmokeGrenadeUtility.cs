using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.VFX;

public class SmokeGrenadeUtility : BaseUtility
{
    /// <summary>
    /// This class is meant to be placed on an empty game object with the smoke VFX asset. <br/>
    /// It should also be at least a sibling object of the GameObject the UtilityManager is on, if position needs to be inherited, this should be a child object
    /// </summary>

    //[SerializeField] private GameObject smokeGrenadePrefab;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float smokeCooldown = 20f;
    [SerializeField] private float castDistance = 20f;

    private RaycastHit hit;
    private CharacterMovement _owningCharacter;

    void Start()
    {
        if (utilityManager == null)
        {
            Debug.LogError("UtilityManager reference is not set in the inspector.");
        }
        else
        {
            _owningCharacter = utilityManager.GetCharacterMovement();
        }
    }
    
    public override void ActivateUtilityRpc()
    {
        if (UtilityConditionsMet())
        {
            //set the position of this object. it will either be the hit point of a raycast (to avoid it entering another object), or at a point infront of the mech
            transform.position = Physics.Raycast(_owningCharacter.transform.position, _owningCharacter.transform.forward, out hit, castDistance) ? hit.point : _owningCharacter.transform.position + (_owningCharacter.transform.forward * castDistance);

            visualEffect.SendEvent("OnFire");

            //GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, smokePos, Quaternion.identity);
            //Destroy(smokeGrenade, smokeCooldown);
            StartCoroutine(UtilityCooldown(utilityCooldownTime));
        }
    }

    protected override bool UtilityConditionsMet()
    {
        return canActivateUtility;
    }

    
}