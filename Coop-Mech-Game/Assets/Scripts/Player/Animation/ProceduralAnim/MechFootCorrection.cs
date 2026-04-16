using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class MechFootCorrection : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform leg;
    [SerializeField] Vector3 rotationOffsetEuler = new Vector3(-90f, 0f, 0f);
    Vector3 oldNormal, currentNormal, newNormal;
    Ray ray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.up = currentNormal;
        //transform.rotation = meshRot.transform.rotation;
        //// get raycast down from the end of the foot so that the flat part is flush with the ground
        //ray = new Ray(transform.forward, Vector3.down);
        //if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        //{
        //    currentNormal = info.normal;
        //}
        // Cast ray straight down from foot position

        ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, terrainLayer))
        {
            currentNormal = hit.normal;

            // Project current forward onto the ground plane
            Vector3 forwardProjected = Vector3.ProjectOnPlane(-leg.forward, currentNormal);

            // Build rotation that matches ground
            if (forwardProjected != Vector3.zero)
            {

                Quaternion groundRotation = Quaternion.LookRotation(forwardProjected, currentNormal);
                transform.rotation = groundRotation * Quaternion.Euler(rotationOffsetEuler);
            }
        }
    }
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawRay(ray);
        Debug.DrawRay(transform.position, transform.forward, Color.blue); // where Unity thinks "forward" is
        Debug.DrawRay(transform.position, transform.up, Color.green);     // where Unity thinks "up" is
    }
}
