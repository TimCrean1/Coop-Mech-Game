using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAlien : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private Rigidbody rb;
    private Vector3 target = Vector3.zero;
    private Vector3 dir;
    private Quaternion lookRot;


    void Start()
    {
        gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        //target = player.transform.position;
        //StartCoroutine(AqcuireTarget());
    }

    private void OnDisable()
    {
        StopCoroutine(AqcuireTarget());
    }

    private void FixedUpdate()
    {
        dir = (target - transform.position).normalized;
        lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, lookSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + Vector3.forward * moveSpeed * Time.fixedDeltaTime);
    }

    public IEnumerator AqcuireTarget()
    {
        player = GameState.Instance.PlayerObject;

        while (true)
        {
            target = player.transform.position;
            yield return new WaitForSeconds(1f);
        }

    }

}
