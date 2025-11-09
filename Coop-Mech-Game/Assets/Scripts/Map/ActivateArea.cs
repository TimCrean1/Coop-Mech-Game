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

    void Start()
    {
        foreach (GameObject go in toActivate)
        {
            go.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
        foreach (GameObject go in toActivate)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(interval);
        }

        yield return null;
    }

    private IEnumerator Activate()
    {
        foreach (GameObject go in toActivate)
        {
            go.SetActive(true);
            yield return null;
        }
        yield return null;
    }
}
