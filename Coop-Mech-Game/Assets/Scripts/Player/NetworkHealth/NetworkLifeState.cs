using Unity.Netcode;
using UnityEngine;


public enum LifeState
{
    Alive,
    Dead
}
public class NetworkLifeState : NetworkBehaviour
{
    [SerializeField] NetworkVariable<LifeState> m_LifeState = new NetworkVariable<LifeState>(global::LifeState.Alive);


    public NetworkVariable<LifeState> LifeState => m_LifeState;
}
