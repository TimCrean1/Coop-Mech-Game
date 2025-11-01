using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Alien : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRend;
    [SerializeField] private VisualEffect gradualEffect;
    [SerializeField] private VisualEffect instantEffect;
    [SerializeField] private VisualEffect smokeEffect;


    private float projectileDistance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            StartCoroutine(DistRoutine(other));
        }
    }

    private IEnumerator DistRoutine(Collider other)
    {
        yield return new WaitForSeconds(0.15f);

        projectileDistance = Vector3.Distance(transform.position, other.transform.position);

        if (projectileDistance <= 1.5f)
        {
            StartCoroutine(InstantDeathRoutine(instantEffect.GetVector2("LifetimeRange").y));
        }
        else
        {
            StartCoroutine(GradualDeathRoutine(gradualEffect.GetVector2("LifetimeRange").y));
        }
    }

    private IEnumerator InstantDeathRoutine(float waitTime)
    {
        instantEffect.SendEvent("PlayDeathInst");
        meshRend.enabled = false;
        yield return new WaitForSeconds(waitTime + 0.5f);
        gameObject.SetActive(false);
    }

    private IEnumerator GradualDeathRoutine(float waitTime)
    {
        gradualEffect.SendEvent("PlayDeathGrad");
        
        yield return new WaitForSeconds(waitTime - 0.1f);
        gradualEffect.SetBool("IsEnabled", false);
        meshRend.enabled = false;

        yield return null;
        gameObject.SetActive(false);
        yield return null;
    }
}
