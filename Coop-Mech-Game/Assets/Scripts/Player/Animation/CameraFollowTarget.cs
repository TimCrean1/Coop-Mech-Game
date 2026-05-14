using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera _camera;
    [Tooltip("X is normalised time [0..1], Y is the interpolation weight [0..1]")]
    [SerializeField] private AnimationCurve approachCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Settings")]
    [SerializeField] private float approachDuration = 2f; //how long approach takes

    private float _elapsedTime = 0f;
    private bool _isApproaching = false;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
        _elapsedTime = 0f;
        _isApproaching = true;
    }

    private void FixedUpdate()
    {
        if (!_isApproaching || target == null) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / approachDuration);
        float curveValue = approachCurve.Evaluate(t);

        transform.position = Vector3.Lerp(_startPosition, target.position, curveValue);
    }
}
