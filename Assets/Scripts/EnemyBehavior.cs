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

    private int patrolIndex = 0;

    public float maxDistance = 20f;
    public LayerMask obstacleMask;
    public GameObject enemy;

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
        if (targetLight.enabled == true)
        {
            direction = (targetLight.transform.position - enemy.transform.position).normalized;
            isChasing = true;

            if (CheckRayToTarget())
            {
                MoveToward(direction);
            }
            else
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
        else
        {
            isChasing = false;
            MoveThroughWaypoints();
        }


        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, targetLight.transform.position);

        // Determine the desired audio state
        AudioState desiredState = AudioState.Wandering;

        if (isChasing)
        {
            desiredState = AudioState.Chasing;
        }
        else if (distanceToPlayer <= nearbyRange)
        {
            desiredState = AudioState.Nearby;
        } else 
        {
            desiredState = AudioState.Wandering;
        }

        // Handle state transitions
        if (desiredState != currentState)
        {
            currentState = desiredState;
        }

        // Fade audio based on the current state
        FadeAudio();
    }

    bool CheckRayToTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(enemy.transform.position, direction, out hit, maxDistance, obstacleMask))
        {
            if (!hit.collider.gameObject.CompareTag("TargetLight"))
            {
                return false;
            }
        }

        return true;
    }

    bool AvoidObstacles(ref Vector3 currentDirection)
    {
        RaycastHit hit;
        if (Physics.Raycast(enemy.transform.position, currentDirection, out hit, maxDistance, obstacleMask))
        {
            Debug.DrawRay(enemy.transform.position, currentDirection * hit.distance, Color.red);

            Vector3 backtrackDirection = -currentDirection;
            MoveToward(backtrackDirection * 0.5f);

            Vector3[] directions = GetSurroundingDirections(currentDirection);

            foreach (var dir in directions)
            {
                if (!Physics.Raycast(enemy.transform.position, dir, maxDistance, obstacleMask))
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
            Quaternion.Euler(0, 90, 0) * baseDirection,
            Quaternion.Euler(0, -90, 0) * baseDirection,
            Quaternion.Euler(0, 135, 0) * baseDirection,
            Quaternion.Euler(0, -135, 0) * baseDirection,
            Quaternion.Euler(0, 180, 0) * baseDirection,
            Quaternion.Euler(0, 10, 0) * baseDirection,
            Quaternion.Euler(0, -10, 0) * baseDirection,
            Quaternion.Euler(0, 20, 0) * baseDirection,
            Quaternion.Euler(0, -20, 0) * baseDirection,
            Quaternion.Euler(0, 30, 0) * baseDirection,
            Quaternion.Euler(0, -30, 0) * baseDirection,
            Quaternion.Euler(0, 60, 0) * baseDirection,
            Quaternion.Euler(0, -60, 0) * baseDirection

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
        velocity += dir;
        enemy.transform.position += velocity * moveSpeed * Time.deltaTime;
        velocity *= 0.99f;
    }

    
    void MoveThroughWaypoints()
    {
        Transform target = waypoints[patrolIndex].transform;
        MoveTowardWayPoint(target.position);

        if (Vector3.Distance(enemy.transform.position, target.position) < 4f)
        {
            patrolIndex = (patrolIndex + 1) % waypoints.Length;
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