using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider radius;
    [SerializeField] private BoxCollider proj;
    [SerializeField] private MeshRenderer mRend;

    [SerializeField] private float launchForce = 100f;
    [SerializeField] private GameObject groundExplosion;
    [SerializeField] private GameObject airExplosion;

    private ContactPoint contactPoint;
    private Vector3 point;

    void OnEnable() //will play when this object is instantiated by gun when fired
    {
        rb.AddForce(Vector3.forward * launchForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alien"))
        {
            StartCoroutine(ExplosionRoutine(false));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 3)
        {
            contactPoint = collision.contacts[0];
            point = contactPoint.point;

            StartCoroutine(ExplosionRoutine(true));
        }
    }

    private IEnumerator ExplosionRoutine(bool hitGround)
    {

        if (hitGround)
        {
            Instantiate(groundExplosion, point, Quaternion.identity);
        }
        else
        {
            Instantiate(airExplosion, transform.position, Quaternion.identity);
        }
        yield return null;

        Destroy(gameObject);
    }
}
