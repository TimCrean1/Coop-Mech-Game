using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            gameObject.SetActive(false);
        }
    }
}
