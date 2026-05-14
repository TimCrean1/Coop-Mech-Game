using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera;
    [SerializeField] private AnimationCurve _followCurve;

    [Header("Stiffness/Damping")]
    [SerializeField] private float _posStiffness = 100f;
    [SerializeField] private float _posDamping = 80f;
    [SerializeField] private float _posMaxSpeed = 80f;
    [SerializeField] private float _rotStiffness = 100f;
    [SerializeField] private float _rotDamping = 80f;
    [SerializeField] private float _rotMaxSpeed = 80f;

    private Vector3 _currentVel;
    private float _rotVel;

    private float _posVel;
    private Vector3 _targetRot;

    private void FixedUpdate()
    {
        HelperExtensions.StepSpringAngle(ref _currentVel.x, _target.transform.position.x, ref _posVel, _posStiffness, _posDamping, _posMaxSpeed);
        HelperExtensions.StepSpringAngle(ref _currentVel.y, _target.transform.position.y, ref _posVel, _posStiffness, _posDamping, _posMaxSpeed);
        HelperExtensions.StepSpringAngle(ref _currentVel.z, _target.transform.position.z, ref _posVel, _posStiffness, _posDamping, _posMaxSpeed);

        _camera.transform.position = _currentVel;
    }
}
