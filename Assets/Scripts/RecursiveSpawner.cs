using UnityEngine;

public class RecursiveSpawner : MonoBehaviour
{
    public GameObject modelToSpawn; // Model for segments
    public GameObject tailModel;    // Model for the tail
    private GameObject parentSegment; // Reference to the parent segment
    public int iterations = 10;       // Number of iterations
    public float scaleMultiplier = 0.8f; // Factor for size reduction
    public float followSpeed = 2f;      // Speed for following parent
    public float offsetDistance = 1.0f; // Distance between segments
    public float movementSpeed = 5f;    // Speed for the head's movement
    public float changeDirectionInterval = 3f; // Direction change interval
    public float maxTurnSpeed = 2f;     // Smooth turning speed
    private Vector3 targetDirection;    // Direction for the head movement
    private float changeDirectionTimer; // Timer for direction changes
    public bool isTail = false;         // Is this the tail segment?

    private int currentIteration = 1;   // Current recursion depth

    void Start()
    {
        // Recursive spawning logic
        if (currentIteration < iterations)
        {
            SpawnSegment(modelToSpawn, false);
        }
        else if (!isTail)
        {
            SpawnSegment(tailModel, true);
        }

        // Initialize direction for the head
        if (parentSegment == null)
        {
            ChangeDirection();
        }
    }

    void Update()
    {
        if (parentSegment != null)
        {
            FollowParent();
        }
        else
        {
            MoveRandomly();
        }
    }

    private void SpawnSegment(GameObject model, bool isTailSegment)
    {
        // Calculate spawn position and scale
        Vector3 newScale = transform.localScale * scaleMultiplier;
        Vector3 spawnPosition = transform.position - transform.forward * offsetDistance;

        // Instantiate the segment
        GameObject spawnedModel = Instantiate(model, spawnPosition, transform.rotation);
        spawnedModel.transform.localScale = newScale;
        spawnedModel.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        // Add this script to the spawned segment
        RecursiveSpawner spawner = spawnedModel.AddComponent<RecursiveSpawner>();
        spawner.modelToSpawn = modelToSpawn;
        spawner.tailModel = tailModel;
        spawner.iterations = iterations;
        spawner.scaleMultiplier = scaleMultiplier;
        spawner.offsetDistance = offsetDistance;
        spawner.followSpeed = followSpeed;
        spawner.movementSpeed = movementSpeed;
        spawner.changeDirectionInterval = changeDirectionInterval;
        spawner.maxTurnSpeed = maxTurnSpeed;
        spawner.currentIteration = currentIteration + 1;
        spawner.parentSegment = gameObject;
        spawner.isTail = isTailSegment;
    }

    private void FollowParent()
    {
        // Smoothly move toward parent's position
        Vector3 targetPosition = parentSegment.transform.position - parentSegment.transform.forward * offsetDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // Smoothly rotate to match parent's rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, parentSegment.transform.rotation, Time.deltaTime * followSpeed);
    }
    private void MoveRandomly()
    {
        // Update the timer for direction change
        changeDirectionTimer += Time.deltaTime;

        if (changeDirectionTimer >= changeDirectionInterval)
        {
            ChangeDirection();
            changeDirectionTimer = 0f;
        }

        // Raycast setup for terrain height check
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");
        RaycastHit hit;

        // Raycast downward to check the distance to the "Terrain" layer
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.distance < 3f) // Close to terrain
            {
                targetDirection.y = 1f; // Move upward
            }
            else if (hit.distance > 10f) // High altitude
            {
                targetDirection.y = -1f; // Move downward
            }
        }

        // Smoothly rotate and move in the random direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * maxTurnSpeed);
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }


    private void ChangeDirection()
    {
        // Generate a new random direction
        targetDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
        // Constrain to a specific plane if needed
        // Uncomment to constrain to the XY plane:
        // targetDirection.z = 0f;
    }
}
