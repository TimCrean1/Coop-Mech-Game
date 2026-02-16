using UnityEngine;

[CreateAssetMenu(fileName = "MovementSO", menuName = "ScriptableObjects/Movement", order = 1)]
public class MovementSO : ScriptableObject
{
    public string movementComponentName;
    public float accelerationRate;
    public float decelerationRate;
    public float maxWalkSpeed;
}