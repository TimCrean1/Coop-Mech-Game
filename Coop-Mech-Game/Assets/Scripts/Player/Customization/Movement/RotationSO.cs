using UnityEngine;

[CreateAssetMenu(fileName = "RotationSO", menuName = "ScriptableObjects/Movement/RotationSO", order = 2)]
public class RotationSO : ScriptableObject
{
    public string rotationComponentName;
    [Range(0,10)] public float horizontalRotationRate;
    [Range(0,10)] public float verticalRotationRate;
}