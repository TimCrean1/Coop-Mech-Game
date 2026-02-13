using UnityEngine;

public class KillhouseEndScript : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private KillhouseManager killhouseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        killhouseManager = KillhouseManager.Instance;
        boxCollider = GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        if (killhouseManager.currentKHStatus == KillhouseManager.KillhouseStatus.Playing)
        {
            boxCollider.isTrigger = true;
        }
        else
        {
            boxCollider.isTrigger = false;
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            killhouseManager.CompleteTrial();
        }
    }
}