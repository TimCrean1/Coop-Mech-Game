using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    public NetworkVariable<int> hitPoints = new NetworkVariable<int>();

    public event Action HitPointsDepleted;

    private void OnEnable()
    {
        hitPoints.OnValueChanged += HitPointsChanged;
    }

    private void OnDisable()
    {
        hitPoints.OnValueChanged -= HitPointsChanged;
    }
    void HitPointsChanged(int prevVal, int newVal)
    {
        if(prevVal > 0 && newVal <= 0)
        {
            HitPointsDepleted?.Invoke();
        }
    }
}
