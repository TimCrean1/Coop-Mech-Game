using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AlienGradualEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    [SerializeField] private GameObject parent;
    private MeshRenderer mRend;

    private void OnEnable()
    {
        mRend = parent.GetComponent<MeshRenderer>();

        StartCoroutine(GradualDeathRoutine(effect.GetVector2("LifetimeRange").y));
    }

    private IEnumerator GradualDeathRoutine(float waitTime)
    {
        Debug.Log("gradual routine called");

        float addit = effect.GetFloat("EndDelay");
        effect.SendEvent("PlayDeathGrad");

        yield return new WaitForSeconds(addit);
        effect.SetBool("IsEnabled", false);
        mRend.enabled = false;

        yield return new WaitForSeconds(waitTime);

        yield return null;
        gameObject.SetActive(false);
        parent.SetActive(false);
        yield return null;
    }
}
