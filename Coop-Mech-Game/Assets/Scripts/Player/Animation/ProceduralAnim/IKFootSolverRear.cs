using UnityEngine;

public class IKFootSolverRear : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolverRear otherFoot = default;
    [SerializeField] Transform footHint;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;
    [SerializeField] Vector3 rayPosition = default;
    [SerializeField] bool rightFoot = false;
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
        transform.up = currentNormal;
        //footHint.position = new Vector3(transform.position.x,footHint.position.y,footHint.position.z);

        ray = new Ray((body.position - body.transform.forward * rayPosition.x) + (body.right * footSpacing * rayPosition.z), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            Vector3 horizontalNew = new Vector3(newPosition.x, 0, newPosition.z);
            Vector3 horizontalHit = new Vector3(info.point.x, 0, info.point.z);
            if (Vector3.Distance(horizontalNew, horizontalHit) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (-body.transform.forward * stepLength * direction) + (footOffset);
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
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
        Vector3 localVel = body.transform.InverseTransformDirection(velocity);

        Vector2 planar = new Vector2(localVel.x, localVel.z);

        // Idle
        if (planar.magnitude < 0.1f)
        {
            rayPosition = new Vector3(7f, 0, 1f);
            return;
        }

        float angle = Mathf.Atan2(planar.x, planar.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float speed = planar.magnitude;

        // 8-direction split (rear-tuned values)
        if (angle >= 337.5f || angle < 22.5f)
        {
            // Forward (rear legs compress inward)
            rayPosition = new Vector3(2f, 0, 0.8f);
        }
        else if (angle < 67.5f)
        {
            // Forward Right
            rayPosition = rightFoot
                ? new Vector3(4f, 0, 1.3f)
                : new Vector3(4f, 0, 0f);
        }
        else if (angle < 112.5f)
        {
            // Right
            rayPosition = rightFoot
                ? new Vector3(6.3f, 0, 1.3f)
                : new Vector3(6.3f, 0, 0f);
        }
        else if (angle < 157.5f)
        {
            // Back Right
            rayPosition = new Vector3(9f, 0, rightFoot ? 1.3f : 0f);
        }
        else if (angle < 202.5f)
        {
            // Backward (rear legs extend far back)
            rayPosition = new Vector3(11f, 0, 0.8f);
        }
        else if (angle < 247.5f)
        {
            // Back Left
            rayPosition = new Vector3(9f, 0, rightFoot ? 0f : 1.3f);
        }
        else if (angle < 292.5f)
        {
            // Left
            rayPosition = rightFoot
                ? new Vector3(6.3f, 0, 0f)
                : new Vector3(6.3f, 0, 1.3f);
        }
        else
        {
            // Forward Left
            rayPosition = rightFoot
                ? new Vector3(4f, 0, 0f)
                : new Vector3(4f, 0, 1.3f);
        }
    }
    //private void VelocityBasedTarget()
    //{
    //    Vector3 velocity = rb.linearVelocity;

    //    // converts velocity into local space
    //    Vector3 localVel = body.transform.InverseTransformDirection(velocity);
    //    Debug.Log("Rear Velocity =" + localVel);
    //    if (localVel.z == 0 && localVel.x == 0)
    //    {
    //        // We're moving forward tier 1
    //        //Debug.Log("Idle - rear");
    //        rayPosition = new Vector3(7f, 0, 1f);
    //    }
    //    if (localVel.z > 0.1f && localVel.z < 5f)
    //    {
    //        // We're moving forward tier 1
    //        //Debug.Log("Forward - rear");
    //        rayPosition = new Vector3(2f, 0, 0.8f);
    //    }
    //    if (localVel.x < 0 && localVel.z < -3)
    //    {
    //        //backward
    //        //Debug.Log("Backward");
    //        rayPosition = new Vector3(11f, 0, 0.8f);
    //    }
    //    if (localVel.x < -3 && localVel.z > 0)
    //    {
    //        // moving left
    //        if (rightFoot == false)
    //        {
    //            Debug.Log("Left (lf)");
    //            rayPosition = new Vector3(6.3f, 0, 1.3f);
    //        }
    //        if (rightFoot == true)
    //        {
    //            Debug.Log("Left (rf)");
    //            rayPosition = new Vector3(6.3f, 0, 0f);
    //        }
    //    }
    //    if (localVel.x > 3 && localVel.z > 0)
    //    {
    //        //moving right
    //        if (rightFoot == false)
    //        {
    //            Debug.Log("Right");
    //            rayPosition = new Vector3(6.3f, 0, 0f);
    //        }
    //        if (rightFoot == true)
    //        {
    //            Debug.Log("Right");
    //            rayPosition = new Vector3(6.3f, 0, 1.3f);
    //        }
    //    }

    //}
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(newPosition, 0.5f);
        Gizmos.DrawRay(ray);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }

}
