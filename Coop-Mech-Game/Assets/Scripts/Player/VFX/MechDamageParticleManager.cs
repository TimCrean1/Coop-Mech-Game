using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using System.Collections;

public class MechDamageParticleManager : MonoBehaviour
{
    [SerializeField] private VisualEffect vfxDamage;
    [SerializeField] private VisualEffect vfxDeath;
    [SerializeField] private Image img;
    [SerializeField] private Color brightColor;
    [SerializeField] private int teamNum = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnTeamDamage.AddListener(TriggerTeamDamage);
        GameManager.Instance.OnTeamDeath.AddListener(TriggerTeamDeath);
        GameManager.Instance.OnBuyRoundStart.AddListener(LocalReset);

        if(teamNum == -1)
        {
            Debug.LogError("Team number not set! Particle events for damage and death will not work!");
        }

        if(img == null)
        {
            Debug.LogError("Image canvas not set! Death flashing will not work!");
        }

        if(brightColor == null || brightColor == Color.black)
        {
            brightColor = Color.white;
        }

        //guarantee that the canvas starts as transparent black, and the bright color can be visible
        Color col = Color.black;
        col.a = 0f;
        img.color = col;
        brightColor.a = 1f;
    }

    private void TriggerTeamDamage(int team, int tier)
    {
        if (team != teamNum) return;

        switch(tier)
        {
            case 1:
                vfxDamage.SendEvent("Tier1");
                break;
            case 2:
                vfxDamage.SendEvent("Tier2");
                break;
            case 3:
                vfxDamage.SendEvent("Tier3");
                break;
        }
    }

    private void TriggerTeamDeath(int team)
    {
        if (team != teamNum) return;
        vfxDeath.SendEvent("OnDeath");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        //wait to allow fire to be seen
        yield return new WaitForSeconds(0.8f);

        //cache original color and make sure that the black can be seen at end of round
        Color colOrig = img.color;
        colOrig.a = 1f;

        //after wait flash bright color for a short time to simulate explosion
        img.color = brightColor;
        yield return new WaitForSeconds(0.1f);

        //apply black, now fully opaque
        img.color = colOrig;

    }

    private void LocalReset()
    {
        //reset the image back to transparent black and stop particles
        Color col = Color.black;
        col.a = 0f;
        img.color = col;

        vfxDamage.SendEvent("OnStopParticles");
    }
}
