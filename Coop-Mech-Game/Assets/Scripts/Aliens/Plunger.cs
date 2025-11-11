using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    [SerializeField] private GameObject leftLaser;
    [SerializeField] private GameObject rightLaser;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fist") || other.CompareTag("Projectile"))
        {
            leftLaser.SetActive(false); //TODO: play anims for this
            rightLaser.SetActive(false);
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }
    }
}
