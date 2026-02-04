using System;
using Unity.Netcode;
using UnityEngine;

public class DamageReceiver : NetworkBehaviour, IDamagable
{
    public event Action<PlayerController, int> DamageReceived;

    //public event Action<Collision> CollisionEntered;
    [SerializeField] NetworkHealthState m_NetworkLifeState;

    public void RecieveHP(PlayerController inflicter, int HP)
    {
        if (IsDamageable())
        {
            DamageReceived?.Invoke(inflicter, HP);
        }
    }

    public bool IsDamageable()
    {
        return m_NetworkLifeState.LifeState.Value == LifeState.Alive;
    }
}
