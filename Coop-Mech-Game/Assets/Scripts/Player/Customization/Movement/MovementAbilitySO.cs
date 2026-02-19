using UnityEngine;

[CreateAssetMenu(fileName = "MovementAbilitySO", menuName = "ScriptableObjects/Movement/MovementAbilitySO", order = 3)]
public abstract class MovementAbilitySO : ScriptableObject
{
    public string movementAbilityName;
    public abstract void ability();
}