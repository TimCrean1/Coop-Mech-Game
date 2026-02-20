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
    [SerializeField] private Material comboMaterial;
    [SerializeField] private Color normalColor = Color.orangeRed;
    [SerializeField] private Color filledColor = Color.purple;
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
        if (decayTimer > decayWindow && currentPoints >= 0)
        {
            currentPoints -= Time.deltaTime * decayRate;
        }
        if (currentPoints < 0) currentPoints = 0;
        
        comboMeter.fillAmount = currentPoints / maxPoints;

        if (isComboFull)
        {
            comboMaterial.color = filledColor;
        }
        else
        {
            comboMaterial.color = normalColor;
        }
    }

    public void AddPoints(float points)
    {
        currentPoints += points;
        decayTimer = 0;
        if (currentPoints > maxPoints)
        {
            Debug.Log("Max Points!");
        }
    }

    public void UseMaxPoints()
    {
        currentPoints = 0;
        isComboFull = false;
    }
    #region Getters
    public float GetCurrentPoints()
    {
        return currentPoints;
    }

    public bool GetIsComboFull()
    {
        return isComboFull;
    }
    #endregion
}