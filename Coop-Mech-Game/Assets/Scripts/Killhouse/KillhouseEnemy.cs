using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class KillhouseEnemy : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private KillhouseManager killhouseManager;
    [Header("Instance Values")]
    [SerializeField] private float pointsValue;
    public bool isActive = true;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        killhouseManager = KillhouseManager.Instance;
        killhouseManager.PopulateEnemiesList(this);
    }

    public void Activate()
    {
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
        isActive = true;
    }
    public void Deactivate()
    {
        boxCollider.enabled = false;
        meshRenderer.enabled = false;
        isActive = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            killhouseManager.UpdatePoints(pointsValue);
            audioSource.Play();
            Destroy(collision.gameObject);
            Deactivate();
        }
    }
}