using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverse_Kinematics : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = true;
    public float movementThreshold = 4.0f; // Distance threshold for leg movement
    public LayerMask groundLayer; // Layer to identify the ground
    public float raycastDistance = 5.0f; // Distance for ground raycast

    // Leg transforms
    public Transform[] legs; // Add your leg transforms in the editor
    private Vector3[] initialPositions; // Initial positions of the legs
    private bool[] isLegMoving; // To track if a leg is currently moving
    private float legSpeed = 10f; // Speed of leg movement

    void Start()
    {
        Debug.Log("ENTER IK");
        animator = GetComponent<Animator>();

        // Initialize arrays
        initialPositions = new Vector3[legs.Length];
        isLegMoving = new bool[legs.Length];

        // Store initial positions
        for (int i = 0; i < legs.Length; i++)
        {
            initialPositions[i] = legs[i].position;
        }
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Debug.Log("Animator is in Idle state!");
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("ATTEMPTING ANIMATING");
        if (!animator || !ikActive) return;

        for (int i = 0; i < legs.Length; i++)
        {
            Debug.Log("ITERATING LEGS");
            if (!isLegMoving[i])
            {
                Debug.Log("LEG NOT MOVING");
                // Cast a ray to find the ground position
                if (Physics.Raycast(initialPositions[i] + Vector3.up * raycastDistance, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
                {
                    Debug.Log("RAYCAST HIT");
                    Vector3 targetPosition = hit.point;

                    // Check if the foot needs to move
                    if (Vector3.Distance(initialPositions[i], targetPosition) > movementThreshold && !IsAnotherLegMoving(i))
                    {
                        Debug.Log("MOVING LEG");
                        StartCoroutine(MoveLeg(i, targetPosition));
                    }
                    else
                    {
                        
                        Debug.Log("LEG ON GROUND");
                        // Keep the leg on the ground
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot + i, 1.0f); // Adjust index for IKGoal
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot + i, initialPositions[i]);
                    }
                }
            }
        }
    }

// Coroutine to move a leg
    private IEnumerator MoveLeg(int legIndex, Vector3 targetPosition)
    {
        Debug.Log("MOVING LEG");
        isLegMoving[legIndex] = true;

        Vector3 startPosition = initialPositions[legIndex];
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * legSpeed;
            initialPositions[legIndex] = Vector3.Lerp(startPosition, targetPosition, time);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot + legIndex, 1.0f); // Add weight here
            animator.SetIKPosition(AvatarIKGoal.LeftFoot + legIndex, initialPositions[legIndex]);

            yield return null;
        }

        initialPositions[legIndex] = targetPosition;
        isLegMoving[legIndex] = false;
    }


    // Check if any other leg is moving
    private bool IsAnotherLegMoving(int currentLegIndex)
    {
        for (int i = 0; i < isLegMoving.Length; i++)
        {
            if (i != currentLegIndex && isLegMoving[i]) return true;
        }
        return false;
    }
}
