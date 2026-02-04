using System;
using Unity.Netcode;
using UnityEngine;

public class DamageReceiver : NetworkBehaviour, IDamagable
{
    public event Action<PlayerController, int> DamageReceived;

    //public event Action<Collision> CollisionEntered;
    [SerializeField] NetworkHealthState m_NetworkHealthState;

    public void RecieveHP(PlayerController inflicter, int HP)
    {
        if (IsDamageable())
        {
            DamageReceived?.Invoke(inflicter, HP);
        }
    }

    public bool IsDamageable()
    {
        return m_NetworkHealthState.LifeState.Value == LifeState.Alive;
    }
}
