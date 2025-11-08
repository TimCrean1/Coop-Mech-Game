using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] Transform body = default;      // The main body that this leg is positioned relative to
    [SerializeField] IKFootSolver otherFoot;        // A reference to the other foot so we know when it's moving
    [SerializeField] float stepSpeed = 4f;          // Foot movement speed
    [SerializeField] float stepDistance = 4f;       // Distance at which a step is started
    [SerializeField] float stepLength = 4f;         // How far to step towards the new position
    [SerializeField] float stepHeight = 1f;         // How high to lift the foot while stepping
    [SerializeField] Vector3 footOffset;            // Adjustable position offset for feet
    [SerializeField] float raycastDistance = 10f;   // The distance to raycast downwards when looking for ground

    float footSpacing;      // How far to the side the foot is located
    float stepInterp;       // Denotes how far we are through a step (0 = haven't started, <1 = in progress, >=1 = complete / not stepping)
    public bool IsMoving => (stepInterp < 1f);

    // Old = last concrete position, New = current desired position, Current = position interpolated between old and new
    Vector3 oldPosition, currentPosition, newPosition;  // Floats representing positions
    Vector3 oldNormal, currentNormal, newNormal;        // Floats representing normal vectors

    [SerializeField] LayerMask groundLayer = default;   // Specifies layers that can be detected for the Foot Raycast

    private void Start()
    {
        // Calculate starting variable values
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;

        // Initialize stepInterp to 1 so we initially move the foot into place
        stepInterp = 1;
    }

    // Update is called once per frame

    void Update()
    {

       

        // Align this foot with the current position and orientation
        transform.position = currentPosition;
        //transform.up = currentNormal;

        // If the other foot isn't moving and this foot isn't moving
        // try to find a new point to step to
        if (!otherFoot.IsMoving && !IsMoving)
        {
            // Start the ray above the foot position, pointing down
            Ray ray = new Ray(body.position + (body.right * footSpacing) + (body.up * (raycastDistance * 0.5f)), Vector3.down);

            // Visually represent the raycast with a ray
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.magenta);

            // Raycast downwards
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer.value))
            {
                // If our desired position is far away enough
                if (Vector3.Distance(newPosition, hit.point) > stepDistance)
                {
                    // Reset stepInterp, so we start moving
                    stepInterp = 0;

                    // Calculate the new desired position and orientation of the foot
                    int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                    newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
                    newNormal = hit.normal;
                    
                }
            }
        }

        // If we haven't completed our step
        if (IsMoving)
        {
            // Calculate our next in-between position
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, stepInterp);
            // Give it a vertical offset based on the amount of distance through the step
            tempPosition.y += Mathf.Sin(stepInterp * Mathf.PI) * stepHeight;

            // Update our current position and orientation
            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, stepInterp);

            // Continue through the step
            stepInterp += Time.deltaTime * stepSpeed;
        }
        else // If we have completed our step
        {
            // Store our new position and orientation for later
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        // Debug sphere at desired position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }
}
