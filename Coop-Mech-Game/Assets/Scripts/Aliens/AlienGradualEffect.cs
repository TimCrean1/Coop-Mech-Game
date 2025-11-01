using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AlienGradualEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    [SerializeField] private GameObject parent;
    private MeshRenderer mRend;

    private void Start()
    {
        mRend = parent.GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(GradualDeathRoutine(effect.GetVector2("LifetimeRange").y));
    }

    private IEnumerator GradualDeathRoutine(float waitTime)
    {
        Debug.Log("gradual routine called");

        float addit = effect.GetFloat("EndDelay");
        effect.SendEvent("PlayDeathGrad");

        yield return new WaitForSeconds(waitTime + (addit - 0.4f));
        effect.SetBool("IsEnabled", false);
        mRend.enabled = false;

        yield return new WaitForSeconds(waitTime);

        yield return null;
        gameObject.SetActive(false);
        parent.SetActive(false);
        yield return null;
    }
}
