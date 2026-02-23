using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthPack : NetworkBehaviour
{
    [SerializeField] private float healAmount = 150f;
    [SerializeField] private BoxCollider coll;
    [SerializeField] private List<Vector3> locations = new List<Vector3>();
    [SerializeField] private float cooldown = 10f;

    private int idx = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TeamOne"))
        {
            Debug.Log("Collided with team one!");
            GameManager.Instance.HealTeamRpc(1, healAmount); //team, health

            //StartCoroutine(NextPosRoutine());
            gameObject.SetActive(false);

        }
        else if (collision.gameObject.CompareTag("TeamTwo"))
        {
            Debug.Log("Collided with team two!");
            GameManager.Instance.HealTeamRpc(2, healAmount);

            //StartCoroutine(NextPosRoutine());
            gameObject.SetActive(false);

        }
    }

    private IEnumerator NextPosRoutine()
    {
        Debug.Log("Entered pos routine; Waiting for seconds: " + cooldown);

        yield return new WaitForSeconds(cooldown);

        idx = (idx + 1) % locations.Count;
        transform.position = locations[idx];
        Debug.Log("Exiting pos routine; moving to pos: " + locations[idx]);

        gameObject.SetActive(true);
    }
}
