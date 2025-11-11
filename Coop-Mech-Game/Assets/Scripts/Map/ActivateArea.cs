using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class ActivateArea : MonoBehaviour
{
    [SerializeField] private List<GameObject> toActivate = new List<GameObject>();
    [SerializeField] private bool overTime;
    [SerializeField] private float interval = 1f;
    [SerializeField] private BoxCollider trigger;
    private GameObject player;

    void Start()
    {
        foreach (GameObject go in toActivate)
        {
            go.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        player = GameState.Instance.PlayerObject;
        if (other.CompareTag("Player"))
        {
            trigger.enabled = false;

            if (overTime)
            {
                StartCoroutine(ActivateRoutineInterval());
            }
            else
            {
                StartCoroutine(Activate());
            }
        }
    }

    private IEnumerator ActivateRoutineInterval()
    {
        for(int i = 0; i<toActivate.Count; i++)
        {
            MovingAlien ma = toActivate[i].GetComponent<MovingAlien>();
            SentryAlien sa = toActivate[i].GetComponent<SentryAlien>();

            if(ma != null) 
            {
                //Debug.Log("ma" + i);

                ma.player = player;
                ma.gameObject.SetActive(true);
                StartCoroutine(ma.AqcuireTarget());
            }
            else if(sa != null) 
            {
                //Debug.Log("sa" + i);

                sa.player = player;
                sa.gameObject.SetActive(true);
                StartCoroutine(sa.FireRoutine());
            }

            yield return new WaitForSeconds(interval);
        }

        yield return null;
    }

    private IEnumerator Activate()
    {
        for(int i = 0; i<toActivate.Count; i++)
        {
            MovingAlien ma = toActivate[i].GetComponent<MovingAlien>();
            SentryAlien sa = toActivate[i].GetComponent<SentryAlien>();

            if(ma != null) 
            {
                //Debug.Log("ma" + i);

                ma.player = player;
                ma.gameObject.SetActive(true);
                StartCoroutine(ma.AqcuireTarget());
            }
            else if(sa != null) 
            {
                //Debug.Log("sa" + i);

                sa.player = player;
                sa.gameObject.SetActive(true);
                StartCoroutine(sa.FireRoutine());
            }

            yield return null;
        }
    }
}
