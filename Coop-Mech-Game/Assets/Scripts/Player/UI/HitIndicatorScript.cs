using UnityEngine;
using UnityEngine.UI;

public class HitIndicatorScript : MonoBehaviour
{

    public Vector3 DamageLocation;
    public Transform PlayerObject;
    public Transform DamageImagePivot;

    public CanvasGroup DamageImageCanvas;
    public float FadeStartTime, FadeTime;
    float maxFadeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxFadeTime = FadeTime; 
    }

    // Update is called once per frame
    void Update()
    {

        if(FadeStartTime > 0)
        {
            FadeStartTime -= Time.deltaTime;
        }
        else
        {
            FadeTime -= Time.deltaTime;
            DamageImageCanvas.alpha = FadeTime / maxFadeTime;
            if(FadeTime <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        DamageLocation.y = PlayerObject.position.y;

        Vector3 Direction = (DamageLocation - PlayerObject.position).normalized;

        float angle = (Vector3.SignedAngle(Direction, PlayerObject.forward, Vector3.up));

        DamageImagePivot.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void CreateIndicator()
    {
        GameObject indicator = Instantiate(gameObject,
                                           transform.position,
                                           transform.rotation,
                                           transform.parent);
        indicator.SetActive(true);
    }
}
