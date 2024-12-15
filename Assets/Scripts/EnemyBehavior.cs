using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    public AudioSource wanderingAudio; // AudioSource for wandering
    public AudioSource nearbyAudio;    // AudioSource for nearby
    public AudioSource chasingAudio;   // AudioSource for chasing

    public Light targetLight;
    public float intensityThreshold = 5f;
    public float detectionRange = 10f;
    public float nearbyRange = 20f;
    public float moveSpeed = 3f;

    public Transform[] waypoints;
    public Transform[] altWaypoints;
    private bool useAltWaypoints = false;

    private int patrolIndex = 0;

    public float maxDistance = 20f;
    public LayerMask obstacleMask;
    public GameObject enemy;

    public float swayFrequency = 3f;

    public float swayMagnitude = 2f;

    private Vector3 direction;
    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 lastValidDirection;
    private bool isChasing = false;

    // AUDIO
    public float fadeSpeed = 2f;       // Speed for fading audio

    private enum AudioState { Wandering, Nearby, Chasing }
    private AudioState currentState = AudioState.Wandering;

    void Start()
    {
        // Ensure only the wandering audio is initially playing
        wanderingAudio.loop = true;
        nearbyAudio.loop = true;
        chasingAudio.loop = true;

        wanderingAudio.volume = 1f;
        nearbyAudio.volume = 0f;
        chasingAudio.volume = 0f;

        wanderingAudio.Play();
        nearbyAudio.Play();
        chasingAudio.Play();
    }

    void Update()
    {
        if (targetLight.enabled == true && (targetLight.transform.position - enemy.transform.position).magnitude < detectionRange)
        {
            direction = (targetLight.transform.position - enemy.transform.position).normalized;
            isChasing = true;

            if (CheckRayToTarget())
            {
                MoveToward(direction);
            }
            else
            {
                if (!CheckRayToTarget())
                {
                    if (AvoidObstacles(ref direction))
                    {
                        MoveToward(direction);
                    }
                    else
                    {
                        MoveToward(direction);
                    }
                }
            }
        }
        else
        {
            isChasing = false;
            MoveThroughWaypoints();
        }

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, targetLight.transform.position);

        AudioState desiredState = AudioState.Wandering;

        if (isChasing)
        {
            desiredState = AudioState.Chasing;
        }
        else if (distanceToPlayer <= nearbyRange)
        {
            desiredState = AudioState.Nearby;
        }
        else
        {
            desiredState = AudioState.Wandering;
        }

        if (desiredState != currentState)
        {
            currentState = desiredState;
        }

        FadeAudio();
    }

    bool CheckRayToTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(enemy.transform.position, direction, out hit, 30, obstacleMask))
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }

    bool AvoidObstacles(ref Vector3 currentDirection)
    {
        RaycastHit hit;
        float sphereRadius = 2f;

        if (Physics.SphereCast(enemy.transform.position, sphereRadius, currentDirection, out hit, maxDistance, obstacleMask))
        {
            Debug.DrawRay(enemy.transform.position, currentDirection * hit.distance, Color.red);

            Vector3 backtrackDirection = -currentDirection * 0.5f;
            MoveToward(backtrackDirection);

            Vector3[] directions = GetSurroundingDirections(currentDirection);

            foreach (var dir in directions)
            {
                if (!Physics.SphereCast(enemy.transform.position, sphereRadius, dir, out hit, maxDistance, obstacleMask))
                {
                    Debug.DrawRay(enemy.transform.position, dir * maxDistance, Color.green);
                    currentDirection = dir;
                    return true;
                }
            }
        }
        return false;
    }

    Vector3[] GetSurroundingDirections(Vector3 baseDirection)
    {
        return new Vector3[]
        {
            baseDirection,
            Quaternion.Euler(0, 45, 0) * baseDirection,
            Quaternion.Euler(0, -45, 0) * baseDirection,
            Quaternion.Euler(45, 0, 0) * baseDirection,
            Quaternion.Euler(-45, 0, 0) * baseDirection,
            Quaternion.Euler(0, 77, 0) * baseDirection,
            Quaternion.Euler(0, -77, 0) * baseDirection,
            Quaternion.Euler(77, 0, 0) * baseDirection,
            Quaternion.Euler(-77, 0, 0) * baseDirection,
            Quaternion.Euler(0, 90, 0) * baseDirection,
            Quaternion.Euler(0, -90, 0) * baseDirection,
            Quaternion.Euler(90, 0, 0) * baseDirection,
            Quaternion.Euler(-90, 0, 0) * baseDirection
        };
    }

    void MoveToward(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            Debug.LogWarning("Direction is zero! Skipping movement.");
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        float maxRotationSpeed = 180f;
        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
        );
        dir = dir.normalized;
        Vector3 swayDir = Vector3.Cross(dir, Mathf.Abs(dir.y) > 0.99f ? Vector3.forward : Vector3.up).normalized;

        float swayAmount = Mathf.Sin(Time.time * swayFrequency * Mathf.PI) * swayMagnitude;

        Vector3 finalMovement = dir * moveSpeed + swayDir * swayAmount;

        enemy.transform.position += finalMovement * moveSpeed * Time.deltaTime;
    }


    void MoveThroughWaypoints()
    {
        Transform[] currentWaypoints = useAltWaypoints ? altWaypoints : waypoints;

        if (currentWaypoints.Length == 0) return; // Handle empty waypoints

        Transform target = currentWaypoints[patrolIndex].transform;
        MoveTowardWayPoint(target.position);

        if (Vector3.Distance(enemy.transform.position, target.position) < 4f)
        {
            patrolIndex = (patrolIndex + 1) % currentWaypoints.Length;
        }
    }

    void MoveTowardWayPoint(Vector3 targetPosition)
    {
        Vector3 directionWP = (targetPosition - transform.position).normalized;
        if (CheckRayToTarget())
        {
            MoveToward(directionWP);
        }
        else
        {
            if (AvoidObstacles(ref directionWP))
            {
                MoveToward(directionWP);
            }
            else
            {
                MoveToward(directionWP);
            }
        }
    }

    public void ActivateAltRoute()
    {
        useAltWaypoints = true;
        patrolIndex = 0;
    }
    private void FadeAudio()
    {
        switch (currentState)
        {
            case AudioState.Wandering:
                wanderingAudio.volume = Mathf.Lerp(wanderingAudio.volume, 1f, Time.deltaTime * fadeSpeed);
                nearbyAudio.volume = Mathf.Lerp(nearbyAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                chasingAudio.volume = Mathf.Lerp(chasingAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                break;

            case AudioState.Nearby:
                wanderingAudio.volume = Mathf.Lerp(wanderingAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                nearbyAudio.volume = Mathf.Lerp(nearbyAudio.volume, 1f, Time.deltaTime * fadeSpeed);
                chasingAudio.volume = Mathf.Lerp(chasingAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                break;

            case AudioState.Chasing:
                wanderingAudio.volume = Mathf.Lerp(wanderingAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                nearbyAudio.volume = Mathf.Lerp(nearbyAudio.volume, 0f, Time.deltaTime * fadeSpeed);
                chasingAudio.volume = Mathf.Lerp(chasingAudio.volume, 1f, Time.deltaTime * fadeSpeed);
                break;
        }
    }
}