using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class SingleComboScript : MonoBehaviour
{
    #region Variables
    [Header("Combo Variables")]
    [SerializeField] private float currentPoints = 0;
    [SerializeField][Range(1,200)] private float maxPoints = 100;
    [SerializeField] private bool isComboFull = false;

    [Header("Decay Variables")]
    [SerializeField][Range(1,10)] private float decayRate = 2;
    [SerializeField][Range(0,5)] private float decayWindow = 2;
    [SerializeField] private float decayTimer = 0;

    [Header("UI Variables")]
    [SerializeField] private Image comboMeter;
    #endregion

    void FixedUpdate()
    {
        if (currentPoints < maxPoints)
        {
            decayTimer += Time.deltaTime;
            isComboFull = false;
        }
        else
        {
            isComboFull = true;
        }
        if (decayTimer > decayWindow)
        {
            currentPoints -= Time.deltaTime * decayRate;
        }
        float meterLerp = Mathf.Lerp(0,maxPoints,currentPoints);
        comboMeter.fillAmount = meterLerp;
    }

    public void AddPoints(float points)
    {
        currentPoints += points;
        decayTimer = 0;
    }

    public void UseMaxPoints()
    {
        currentPoints = 0;
        isComboFull = false;
    }

    public float GetCurrentPoints()
    {
        return currentPoints;
    }

    public bool GetIsComboFull()
    {
        return isComboFull;
    }
}