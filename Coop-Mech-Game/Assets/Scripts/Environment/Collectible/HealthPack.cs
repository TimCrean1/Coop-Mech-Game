using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healAmount = 150f;
    [SerializeField] private BoxCollider coll;
    [SerializeField] private List<Vector3> locations = new List<Vector3>();
    [SerializeField] private float cooldown = 10f;

    private int idx = 0;

    private void Start()
    {
        StartCoroutine(NextPosRoutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TeamOne"))
        {
            Debug.Log("Collided with team one!");
            GameManager.Instance.HealTeamRpc(1, healAmount); //team, health

            StartCoroutine(NextPosRoutine());

        } else if (collision.gameObject.CompareTag("TeamTwo"))
        {
            Debug.Log("Collided with team two!");
            GameManager.Instance.HealTeamRpc(2, healAmount);

            StartCoroutine(NextPosRoutine());
        }
    }

    private IEnumerator NextPosRoutine()
    {
        gameObject.SetActive(false);

        yield return new WaitForSeconds(cooldown);

        transform.position = locations[idx];
        idx = (idx + 1) % locations.Count;

        gameObject.SetActive(true);
    }
}
