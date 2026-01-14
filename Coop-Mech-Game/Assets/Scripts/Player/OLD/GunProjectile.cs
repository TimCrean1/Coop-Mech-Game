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

    [SerializeField] private bool isAlien = false;
    [SerializeField] private bool isKinematic = false;
    [SerializeField] private float kinematicSpeed = 5;
    private float damage;

    private ContactPoint contactPoint;
    private Vector3 point;

    private void Start()
    {
        gameObject.SetActive(true);
        
    }

    void OnEnable() //will play when this object is instantiated by gun when fired
    {
        if(isKinematic == false)
        {
            rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        }
        else
        {
            StartCoroutine(KinematicProjectile());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAlien == false && other.CompareTag("Alien"))
        {
            StartCoroutine(ExplosionRoutine(false));
        }
        else if(isAlien == true && other.CompareTag("Player"))
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
        } else if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ExplosionRoutine(false));
            GameManager.Instance.DamagePlayer(1);
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
        mRend.enabled = false;
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private IEnumerator KinematicProjectile()
    {
        rb.MovePosition(transform.position + transform.forward * Time.deltaTime);
        yield return null;
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
