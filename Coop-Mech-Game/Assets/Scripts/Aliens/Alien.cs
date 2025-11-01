using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Alien : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRend;
    [SerializeField] private GameObject gradualEffect;
    [SerializeField] private GameObject instantEffect;


    private float projectileDistance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("pprojectile trigger entered by alien");

            StartCoroutine(DistRoutine(other));
        }
    }

    private IEnumerator DistRoutine(Collider other)
    {
        yield return new WaitForSeconds(0.15f);

        Debug.Log("Distance routine done waiting");

        projectileDistance = Vector3.Distance(transform.position, other.transform.position);

        if (projectileDistance <= 1.5f)
        {
            instantEffect.SetActive(true);
        }
        else
        {
            gradualEffect.SetActive(true);
        }
    }

    

    
}
