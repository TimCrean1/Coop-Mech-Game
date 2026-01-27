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
    [SerializeField] private MeshRenderer mainMeshRenderer;
    [SerializeField] private MeshRenderer childMeshRenderer1;
    [SerializeField] private MeshRenderer childMeshRenderer2;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private KillhouseManager killhouseManager;
    [Header("Instance Values")]
    [SerializeField] private float pointsValue;
    public bool isActive = true;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        mainMeshRenderer = GetComponent<MeshRenderer>();
        childMeshRenderer1 = transform.GetChild(0).GetComponent<MeshRenderer>();
        childMeshRenderer2 = transform.GetChild(1).GetComponent<MeshRenderer>();
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
        mainMeshRenderer.enabled = true;
        childMeshRenderer1.enabled = true;
        childMeshRenderer2.enabled = true;
        isActive = true;
    }
    public void Deactivate()
    {
        boxCollider.enabled = false;
        mainMeshRenderer.enabled = false;
        childMeshRenderer1.enabled = false;
        childMeshRenderer2.enabled = false;
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