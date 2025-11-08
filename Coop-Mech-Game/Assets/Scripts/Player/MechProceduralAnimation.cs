using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechProceduralAnimation : MonoBehaviour
{

    class ProceduralLimb
    {

        public Transform IKTarget; //the reference to the transform of the IK target associated with this limb
        public Vector3 defaultPosition; //the rest position of the IK target (i.e. the "desired" position, considering the overall offset of the body)
        public Vector3 lastPosition; //the position of the IK target at the last frame (this will be useful for computing the direction towards the "desired" position)
        public bool moving = false;
    }

    [SerializeField] private Transform[] _limbTargets;

    private int _nLimbs;
    private ProceduralLimb[] _limbs;

    private Vector3 _lastBodyPosition;
    private Vector3 _velocity;

    [SerializeField] private float _stepSize = 1;

    [SerializeField] private LayerMask _groundLayerMask = default;
    [SerializeField] private float _raycastRange = 2;


    private void Start()
    {
        _nLimbs = _limbTargets.Length;
        _limbs = new ProceduralLimb[_nLimbs];
        Transform t;
        for (int i = 0; i < _nLimbs; i++)
        {
            t = _limbTargets[i];
            _limbs[i] = new ProceduralLimb()
            {
                IKTarget = t,
                defaultPosition = t.localPosition,
                lastPosition = t.position,
                moving = false
            };
        }
    }

    private void FixedUpdate()
    {
        _velocity = transform.position - _lastBodyPosition;
        _lastBodyPosition = transform.position;

        Vector3[] desiredPosition = new Vector3[_nLimbs];
        float greatestDistance = _stepSize;
        int limbToMove = 1;

        for (int i = 0; i < _nLimbs; i++)
        {
            if (_limbs[i].moving) continue;

            desiredPosition[i] = transform.TransformPoint(_limbs[i].defaultPosition);
            float dist = (desiredPosition[i] + _velocity - _limbs[i].lastPosition).magnitude;
            if (dist > greatestDistance)
            {
                greatestDistance = dist;
                limbToMove = i;
            }
        }

        // keep not moving limbs in place
        for (int i = 0; i < _nLimbs; ++i)
        {
            if (i != limbToMove) 
            {
                _limbs[i].IKTarget.position = _limbs[i].lastPosition;       
            }

            // move the selected leg to its "desired" place
            if (limbToMove != -1) 
            {
                Vector3 targetPoint = desiredPosition[limbToMove];
                _limbs[limbToMove].IKTarget.position = targetPoint;
                _limbs[limbToMove].lastPosition = targetPoint;
            }
        }

        if (limbToMove != -1)
        {
            Vector3 targetPoint = desiredPosition[limbToMove];
            targetPoint = _RaycastToGround(targetPoint, transform.up);
            _limbs[limbToMove].IKTarget.position = targetPoint;
            _limbs[limbToMove].lastPosition = targetPoint;
        }
    }

    private Vector3 _RaycastToGround(Vector3 pos, Vector3 up)
    {
        Vector3 point = pos;

        Ray ray = new Ray(pos + _raycastRange * up, -up);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f * _raycastRange, _groundLayerMask))
        {
            point = hit.point;

        }
        return point;
    }
}
