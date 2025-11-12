using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryAlien : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private float range = 80f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject projectile;
    //private Vector3 target = Vector3.zero;
    private Vector3 dir;
    private Quaternion lookRot;
    [SerializeField] public bool isPartOfWave = false;
    [SerializeField] public Transform waveTarget;

    private void OnEnable()
    {
        //player = GameState.Instance.PlayerObject;

        //StartCoroutine(AquireTarget());
        StartCoroutine(MoveToPos());
        
    }

    private void Start()
    {
        if (isPartOfWave)
        {
            range = range * 3f;
        }
    }

    public void OnDisable()
    {
        StopCoroutine(FireRoutine());
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        //StopCoroutine(FireRoutine());
    }

    private IEnumerator MoveToPos()
    {
        float distToPos = Vector3.Distance(transform.position, waveTarget.transform.position);

        while(distToPos > 0.5f)
        {

            distToPos = Vector3.Distance(transform.position, waveTarget.transform.position);

            transform.position = Vector3.MoveTowards(transform.position, waveTarget.transform.position, 5 * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator FireRoutine()
    {
        //player = GameState.Instance.PlayerObject;
        //Debug.Log("FIRE ROUTINE START");

        while (true)
        {
            yield return new WaitForSeconds(fireInterval);

            //Debug.Log("IN FIRE WHILE ROUTINE");

            if(gameObject != null && Vector3.Distance(transform.position, player.transform.position) <= range)
            {
                dir = (player.transform.position - transform.position).normalized;
                lookRot = Quaternion.LookRotation(dir);
                rb.MoveRotation(lookRot);

                //Debug.Log("DISTA NCE GUD");

                Instantiate(projectile, transform.position + transform.forward * 2, gameObject.transform.rotation);
                

                yield return null;
            }

            yield return null;
        }
    }
}
