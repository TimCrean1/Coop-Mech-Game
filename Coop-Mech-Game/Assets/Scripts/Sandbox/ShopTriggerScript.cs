using UnityEngine;

public class ShopTriggerScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("TeamOne")){
            GameManager.Instance.OnRoundEnd.Invoke();
        }
        else if (collision.gameObject.CompareTag("TeamTwo")){
            GameManager.Instance.OnRoundEnd.Invoke();
        }
    }
}
