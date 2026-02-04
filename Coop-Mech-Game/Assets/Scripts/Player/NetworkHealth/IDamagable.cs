using Unity.VisualScripting;
using UnityEngine;

public interface IDamagable
{

    // What damage is done Negative for damage positive for healing
    void RecieveHP(PlayerController inflicter, int HP);

    ulong NetworkObjectId { get; }

    Transform transform { get; }

    bool IsDamageable();
}
