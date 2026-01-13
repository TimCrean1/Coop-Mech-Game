using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunchingScript : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private CombatSFXManager combatSFXManager;
    [SerializeField] private BoxCollider hitDetectionTrigger;
    [SerializeField] private Animator playerAnimator;

    [Header("Punching Variables")]
    [SerializeField][Range(0, 10)] private float punchCooldown = 3f;
    private float punchTimer = 0f;

    private void Start()
    {
        hitDetectionTrigger.enabled = false;   
    }

    private void Update()
    {
        if (punchTimer > 0)
        {
            punchTimer -= Time.deltaTime;
            hitDetectionTrigger.enabled = true;
        }
        else
        {
            hitDetectionTrigger.enabled = false;
        }
    }

    public void Punch()
    {
        if (punchTimer > 0) return;
        if (combatSFXManager != null)
        {
            combatSFXManager.PlayPunchSound();
        }
        punchTimer = punchCooldown;

        playerAnimator.SetTrigger("Punch");
    }
}
