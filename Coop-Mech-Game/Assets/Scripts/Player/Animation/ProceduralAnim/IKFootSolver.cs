using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] GameObject body = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] Transform footHint;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 rayPosition = default;
    [SerializeField] Vector3 footOffset = default;
    public bool sidewaysControl = false;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;
    Ray ray;
    Rigidbody rb;
   
    
    private void Start()
    {
        
        rb = body.GetComponentInParent<Rigidbody>();
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame

    void Update()
    {
        VelocityBasedTarget();
        transform.position = currentPosition;
        //transform.up = currentNormal;
        Vector3 forward = Vector3.ProjectOnPlane(body.transform.forward, currentNormal).normalized;
        transform.rotation = Quaternion.LookRotation(forward, currentNormal);
        //footHint.position = new Vector3(transform.position.x, footHint.position.y, footHint.position.z);
        
        ray = new Ray((body.transform.position + body.transform.forward * rayPosition.x) + (body.transform.right * footSpacing * rayPosition.z), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.transform.InverseTransformPoint(info.point).z > body.transform.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.transform.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Slerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void VelocityBasedTarget()
    {
        Vector3 velocity = rb.linearVelocity;

        // converts velocity into local space
        Vector3 localVel = body.transform.InverseTransformDirection(velocity);
        //Debug.Log("Velocity =" + localVel);
        if (localVel.z == 0 && localVel.z == 0)
        {
            // We're moving forward tier 1
            Debug.Log("Idle");
            rayPosition = new Vector3(5f, 0, 0.9f);
        }
        if (localVel.z > 0.1f && localVel.z < 5f)
        {
            // We're moving forward tier 1
            Debug.Log("Forward");
            rayPosition = new Vector3(6f,0,0.6f);
        }
        if (localVel.x < 0 && localVel.z < -3)
        {
            //backward
            Debug.Log("Backward");
            rayPosition = new Vector3(2.2f, 0, 1f);
        }
        if (localVel.z >= 5f)
        {
            // We're moving forward tier 2
            Debug.Log("Faster");
            rayPosition = new Vector3(8f, 0, 0.6f);
        }
        if(localVel.x < -3 && localVel.z < 0){
            Debug.Log("Left");
            rayPosition = new Vector3(3f, 0, 0.6f);
        }
        if (localVel.x > 3 && localVel.z < 0)
        {
            Debug.Log("Right");
            rayPosition = new Vector3(3f, 0, 0.6f);
        }
        

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
        Gizmos.DrawRay(ray);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }



}
