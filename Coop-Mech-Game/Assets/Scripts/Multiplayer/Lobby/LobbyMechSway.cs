using UnityEngine;

public class LobbyMechSway : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;    // How fast it moves
    [SerializeField] private float height = 0.5f;   // How far it moves up and down

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using Sine
        float newY = Mathf.Sin(Time.time * speed) * height;

        // Apply the new position relative to the starting position
        transform.position = startPosition + new Vector3(0, newY, 0);
    }
}
