using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryAlien : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private float range = 40f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject projectile;
    private Vector3 target = Vector3.zero;
    private Vector3 dir;
    private Quaternion lookRot;
    [SerializeField] private bool isPartOfWave = false;
    [SerializeField] private GameObject waveTarget;


    private void OnEnable()
    {
        StartCoroutine(FireRoutine());
        if (isPartOfWave)
        {
            StartCoroutine(MoveToPos());
        }
    }

    private void OnDisable()
    {
        StopCoroutine(FireRoutine());
    }

    private IEnumerator MoveToPos()
    {
        float distToPos = Vector3.Distance(transform.position, waveTarget.transform.position);

        while(distToPos > 0.5f)
        {

            distToPos = Vector3.Distance(transform.position, waveTarget.transform.position);

            Vector3.MoveTowards(transform.position, waveTarget.transform.position, 5 * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);

            if(Vector3.Distance(transform.position, target) <= range)
            {
                dir = (target - transform.position).normalized;
                lookRot = Quaternion.LookRotation(dir);
                rb.MoveRotation(lookRot);

                yield return null;

                Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            }

            yield return null;
        }
    }
}
