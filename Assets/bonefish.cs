using UnityEngine;

public class Bonefish : MonoBehaviour
{
    public GameObject modelToSpawn; // Model for segments
    public GameObject player;      // Reference to the player GameObject
    public float detectionRange = 10f; // Range to detect the player
    public float targetHeight = 5f;    // Height to rise to when staring at the player
    public float raiseSpeed = 1f;      // Speed of raising to the target height
    public int iterations = 10;       // Number of iterations
    public float scaleMultiplier = 0.8f; // Factor for size reduction
    public float followSpeed = 2f;      // Speed for following parent
    public float offsetDistance = 1.0f; // Distance between segments
    public float movementSpeed = 5f;    // Speed for the head's movement
    public float changeDirectionInterval = 3f; // Direction change interval
    public float maxTurnSpeed = 2f;     // Smooth turning speed

    private Vector3 targetDirection;    // Direction for the head movement
    private float changeDirectionTimer; // Timer for direction changes
    private int currentIteration = 1;   // Current recursion depth
    private GameObject parentSegment;   // Reference to the parent segment

    private bool playerDetected = false; // Whether the player is detected
    private Transform playerTransform;  // Reference to the player's transform
    private float groundHeight;         // Height of the ground beneath the object

    void Start()
    {
        // Recursive spawning logic
        if (currentIteration < iterations)
        {
            SpawnSegment(modelToSpawn);
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
            DetectPlayer();
            if (playerDetected)
            {
                FacePlayerAndRise();
            }
            else
            {
                MoveRandomly();
            }
        }
    }

    private void SpawnSegment(GameObject model)
    {
        // Calculate spawn position and scale
        Vector3 newScale = transform.localScale * scaleMultiplier;
        Vector3 spawnPosition = transform.position - transform.forward * offsetDistance;

        // Instantiate the segment
        GameObject spawnedModel = Instantiate(model, spawnPosition, transform.rotation);
        spawnedModel.transform.localScale = newScale;
        spawnedModel.GetComponent<Renderer>().material = GetComponent<Renderer>().material;

        // Add this script to the spawned segment
        Bonefish spawner = spawnedModel.AddComponent<Bonefish>();
        spawner.modelToSpawn = modelToSpawn;
        spawner.player = player;
        spawner.detectionRange = detectionRange;
        spawner.targetHeight = targetHeight;
        spawner.raiseSpeed = raiseSpeed;
        spawner.iterations = iterations;
        spawner.scaleMultiplier = scaleMultiplier;
        spawner.offsetDistance = offsetDistance;
        spawner.followSpeed = followSpeed;
        spawner.movementSpeed = movementSpeed;
        spawner.changeDirectionInterval = changeDirectionInterval;
        spawner.maxTurnSpeed = maxTurnSpeed;
        spawner.currentIteration = currentIteration + 1;
        spawner.parentSegment = gameObject;
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

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            groundHeight = hit.point.y;
        }

        // Smoothly rotate and move in the random direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * maxTurnSpeed);
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    private void DetectPlayer()
    {
        if (!player) return;

        // Check if the flashlight is on
        Transform flashlight = player.transform.Find("Camera/Flashlight");
        if (!flashlight || !flashlight.gameObject.activeSelf)
        {
            playerDetected = false;
            return;
        }

        // Check distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            playerTransform = player.transform;
        }
        else
        {
            playerDetected = false;
        }
    }

    private void FacePlayerAndRise()
    {
        if (!playerTransform) return;

        // Rotate to face the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * maxTurnSpeed);

        // Slowly rise to the target height above the ground
        Vector3 targetPosition = new Vector3(transform.position.x, groundHeight + targetHeight, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * raiseSpeed);
    }

    private void ChangeDirection()
    {
        // Generate a new random direction
        targetDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
        if(groundHeight > 3f)
        {
            targetDirection.z -= 0.5f;
        } else if (groundHeight < 1.5f)
        {
            targetDirection.z += 2f;
        }
    }
}
