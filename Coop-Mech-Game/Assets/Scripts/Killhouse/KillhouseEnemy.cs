using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class KillhouseEnemy : MonoBehaviour
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
        this.enabled = true;
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
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
        }
    }
}