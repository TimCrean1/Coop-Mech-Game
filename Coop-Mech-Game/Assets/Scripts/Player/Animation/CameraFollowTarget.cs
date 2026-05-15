using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera _camera;
    [Tooltip("X is normalised time [0..1], Y is the interpolation weight [0..1]")]
    [SerializeField] private AnimationCurve approachCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Settings")]
    [SerializeField] private float maxDistance = 0.1f; //distance at which the curve input x is 1
    [SerializeField] private float followSpeed = 1f;

    private float _elapsedTime = 0f;
    private float _minElapsedTime = 0.15f;
    private Vector3 _startPosition;
    private Vector3 _lastPos;

    private void Start()
    {
        _startPosition = transform.position;
        _lastPos = target.position;
        _elapsedTime = 0f;
    }

    private void FixedUpdate()
    {
        if (target == null) Debug.LogError("Camera has no target to follow: " + _camera);

        float distance = Vector3.Distance(transform.position, target.position);

        // Normalise distance into 0..1 to sample the curve
        float t = Mathf.Clamp01(distance / maxDistance);
        t.MapRange(0f, 1f, 1f, 0f);
        float curveValue = approachCurve.Evaluate(t);

        transform.position = Vector3.MoveTowards(transform.position, target.position, curveValue * followSpeed * Time.fixedDeltaTime);

    }
}
