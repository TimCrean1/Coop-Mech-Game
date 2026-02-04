using System;
using Unity.Netcode;
using UnityEngine;
public enum LifeState
{
    Alive,
    Dead
}
public class NetworkHealthState : NetworkBehaviour
{
    public NetworkVariable<int> hitPoints = new NetworkVariable<int>();
    public NetworkVariable<int> shieldPoints = new NetworkVariable<int>();

    [SerializeField] NetworkVariable<LifeState> m_LifeState = new NetworkVariable<LifeState>(global::LifeState.Alive);

    public NetworkVariable<LifeState> LifeState => m_LifeState;

    public event Action HitPointsDepleted;

    private void OnEnable()
    {
        hitPoints.OnValueChanged += HitPointsChanged;
        //shieldPoints.OnValueChanged += 
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
