using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NetworkObject))]
    

public class KillhouseEnemy : NetworkBehaviour
{
    [Header("Component References")]
    [SerializeField] private KillhouseManager killhouseManager;
    [Header("Instance Values")]
    [SerializeField] private float pointsValue;
    [SerializeField] private bool isFriendly;
    public bool isActive = true;

    void Start()
    {
        killhouseManager = KillhouseManager.Instance;
        killhouseManager.PopulateEnemiesList(this);
        if (isFriendly)
        {
            pointsValue = -1;
        }
        else {pointsValue = 1;}
    }

    public void Activate()
    {
        GetComponent<MeshRenderer>().enabled = true;
        isActive = true;
    }
    [Rpc(SendTo.Everyone)]
    public void ActivateRpc()
    {
        Debug.Log("Activating On The Server");
        GetComponent<MeshRenderer>().enabled = true;
        isActive = true;
    }

    public void Deactivate()
    {
        GetComponent<MeshRenderer>().enabled = false;
        isActive = false;
    }
    [Rpc(SendTo.Everyone)]
    public void DeactivateRpc()
    {
        Debug.Log("Deactivating On The Server");
        GetComponent<MeshRenderer>().enabled = false;
        isActive = false;
    }
    void OnDisable()
    {
        if (killhouseManager.currentKHStatus == KillhouseManager.KillhouseStatus.Playing)
        {
            killhouseManager.UpdatePoints(pointsValue);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            killhouseManager.UpdatePoints(pointsValue);
            // audioSource.Play();
            Destroy(collision.gameObject);
            Deactivate();
            if (!IsServer)
            {
                DeactivateRpc();
            }
        }
    }
}