using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healAmount = 150f;
    [SerializeField] private BoxCollider coll;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TeamOne"))
        {
            Debug.Log("Collided with team one!");
            GameManager.Instance.HealTeamRpc(1, healAmount); //team, health
        } else if (collision.gameObject.CompareTag("TeamTwo"))
        {
            Debug.Log("Collided with team two!");
            GameManager.Instance.HealTeamRpc(2, healAmount);
        }
    }
}
