using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    void Start()
    {
        maxHealth = GameManager.Instance.GetMaxPlayerHealth();
        currentHealth = GameManager.Instance.GetPlayerHealth();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateBar();
    }

    private void UpdateBar()
    {
        fillBar.fillAmount = currentHealth / maxHealth;
    }
}
