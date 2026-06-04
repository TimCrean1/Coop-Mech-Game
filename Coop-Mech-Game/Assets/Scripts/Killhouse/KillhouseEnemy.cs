using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class KillhouseEnemy : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Instance Values")]
    [SerializeField] private float deactivationTime = 7f;

    private void Awake()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    public void Activate()
    {
        meshRenderer.enabled = true;
    }

    public void Deactivate()
    {
        meshRenderer.enabled = false;
    }

    public void Hit()
    {
        Debug.Log("Target hit, starting deactivate routine on: " + gameObject.name);
        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(deactivationTime);

        meshRenderer.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);

            Hit();
        }
    }
}