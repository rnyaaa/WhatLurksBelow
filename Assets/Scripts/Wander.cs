using UnityEngine;

public class WanderBehavior : MonoBehaviour
{
    // Wander parameters
    public float wanderRadius = 10.0f;    // Maximum distance from the starting point
    public float wanderSpeed = 2.0f;      // Speed of movement
    public float turnSpeed = 1.0f;        // Speed of turning toward the target
    public float targetThreshold = 1.0f; // Distance to consider the target reached

    private Vector3 startPosition;        // Initial position of the object
    private Vector3 currentTarget;        // Current wander target
    private Vector3 currentDirection;     // Current facing direction in XZ plane

    void Start()
    {
        // Save the starting position
        startPosition = transform.position;

        // Generate the first random target
        GenerateNewTarget();

        // Initialize current direction
        currentDirection = new Vector3(1, 0, 0); // Default to facing along the X-axis
    }

    void Update()
    {
        // Move toward the current target
        MoveTowardsTarget();

        // Check if the target has been reached
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                             new Vector3(currentTarget.x, 0, currentTarget.z)) < targetThreshold)
        {
            // Generate a new random target
            GenerateNewTarget();
        }
    }

    private void GenerateNewTarget()
    {
        // Generate a random point within the wander radius
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        currentTarget = startPosition + new Vector3(randomPoint.x, 0, randomPoint.y);

        Debug.Log($"New target generated: {currentTarget}");
    }

    private void MoveTowardsTarget()
    {
        // Calculate the direction to the target in the XZ plane
        Vector3 targetDirection = new Vector3(
            currentTarget.x - transform.position.x,
            0, // Keep Y-axis constant
            currentTarget.z - transform.position.z
        ).normalized;

        // Gradually rotate the current direction to face the target direction
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, turnSpeed * Time.deltaTime).normalized;

        // Move along the current direction in the XZ plane
        Vector3 movement = currentDirection * wanderSpeed * Time.deltaTime;
        transform.position += new Vector3(movement.x, 0, movement.z); // Keep Y constant

        // Debug visualization
        Debug.DrawLine(transform.position, currentTarget, Color.green); // To target
        Debug.DrawRay(transform.position, currentDirection * 2, Color.blue); // Facing direction
    }
}
