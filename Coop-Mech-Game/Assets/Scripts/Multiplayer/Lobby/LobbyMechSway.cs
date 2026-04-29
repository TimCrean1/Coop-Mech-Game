using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LobbyMechSway : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;    // How fast it moves
    [SerializeField] private float height = 0.5f;   // How far it moves up and down
    [SerializeField] private GameObject head;
    [SerializeField] private List<Transform> lookPoints;

    private Quaternion targetRotation;
    private bool isRotating = false;
    private Vector3 startPosition;
    private float count = 0;
    [SerializeField] private float lookInterval = 10;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
        //head.transform.LookAt(lookPoints[0]);
        
    }

    void Update()
    {
        count = (count + Time.deltaTime) % (lookInterval + 1);

        if (count > lookInterval && !isRotating)
        {
            targetRotation = Quaternion.Euler(0, Random.Range(140f, 170f), 0);
            isRotating = true;
        }

        if (isRotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                isRotating = false;
            }
        }

        // Calculate the new Y position using Sine
        float newY = Mathf.Sin(Time.time * speed) * height;

        // Apply the new position relative to the starting position
        transform.position = startPosition + new Vector3(0, newY, 0);
    }

   
}
