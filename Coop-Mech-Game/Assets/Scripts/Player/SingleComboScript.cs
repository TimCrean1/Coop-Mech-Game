using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Netcode;

public class SingleComboScript : NetworkBehaviour
{
    #region Variables
    [Header("Combo Variables")]
    //[SerializeField] private float currentPoints = 0;
    [SerializeField] private NetworkVariable<float> currentPoints = new NetworkVariable<float>();
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
        if (currentPoints.Value < maxPoints)
        {
            decayTimer += Time.deltaTime;
            isComboFull = false;
        }
        else
        {
            isComboFull = true;
        }
        if (decayTimer > decayWindow && currentPoints.Value >= 0)
        {
            currentPoints.Value -= Time.deltaTime * decayRate;
        }
        if (currentPoints.Value < 0) currentPoints.Value = 0;
        
        comboMeter.fillAmount = currentPoints.Value / maxPoints;
    }
    [Rpc(SendTo.Server)]
    public void AddPointsRpc(float points)
    {
        currentPoints.Value += points;
        decayTimer = 0;
    }

    public void UseMaxPoints()
    {
        currentPoints.Value = 0;
        isComboFull = false;
    }
    #region Getters
    public float GetCurrentPoints()
    {
        return currentPoints.Value;
    }

    public bool GetIsComboFull()
    {
        return isComboFull;
    }
    #endregion
}