using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienMothership : MonoBehaviour
{
    [SerializeField] private GameObject damageArea;
    [SerializeField] private Material mat;

    void Start()
    {
        damageArea.SetActive(false);
    }

    public void OnWaveEnd()
    {
        damageArea.SetActive(true);
        mat.color = Color.red;
    }
}
