using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class SmokeGrenadeUtility : BaseUtility
{
    [SerializeField] private GameObject smokeGrenadePrefab;
    [SerializeField] private float smokeDuration = 5f;
    void Start()
    {
        if (utilityManager == null)
        {
            Debug.LogError("UtilityManager reference is not set in the inspector.");
        }
    }
    public override void ActivateUtility()
    {
        if (UtilityConditionsMet())
        {
            GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, transform.position, Quaternion.identity);
            Destroy(smokeGrenade, smokeDuration);
            StartCoroutine(UtilityCooldown(utilityCooldownTime));
        }
    }

    protected override bool UtilityConditionsMet()
    {
        return canActivateUtility;
    }
}
