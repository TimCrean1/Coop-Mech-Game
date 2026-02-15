using UnityEngine;

public class KillhouseStartScript : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private KillhouseManager killhouseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        killhouseManager = KillhouseManager.Instance;
        boxCollider = GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            killhouseManager.ResetPoints();
            if (killhouseManager.currentKHStatus != KillhouseManager.KillhouseStatus.Playing)
            {
                killhouseManager.StartTrial();
            }
            else
            {
                killhouseManager.CancelTrial();
            }
        }
    }
}
