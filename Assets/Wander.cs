using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 goal;
    private Vector3 velocity;
    private bool goalReached = true;

    public int range = 5;    // Range for wandering
    public float speed = 1;  // Movement speed

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // Use lowercase transform
    }

    void FindNewGoal()
    {
        // Generate a random direction
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        // Random distance within the range
        float distance = Random.Range(1, range);

        // Set a new goal position
        goal = startPosition + randomDir * distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (goalReached)
        {
            FindNewGoal();
            goalReached = false;
        }

        // Check if the goal is reached
        if (Vector3.Distance(transform.position, goal) < 0.5f) // Threshold for "goal reached"
        {
            goalReached = true;
        }

        // Calculate velocity and move towards the goal
        velocity = (goal - transform.position).normalized;

        transform.position += velocity * speed * Time.deltaTime;

        // Make the object face the direction of movement
        if (velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
