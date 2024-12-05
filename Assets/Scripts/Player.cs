using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float moveSpeed = 0.95f;
    [SerializeField] float gravity = -4f;
    [SerializeField][Range(0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0f, 0.5f)] float mouseSmoothTime = 0.03f;

    bool isWalking = false;
    float cameraPitch = 0f;
    float velocityY = 0f;
    CharacterController controller = null;

    Vector2 currentDirection = Vector2.zero;
    Vector2 currentDirectionVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public AudioSource footstepsSound;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        Vector2 targeMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targeMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -70f, 70f);

        if(isWalking)
            if(!footstepsSound.isPlaying)
                footstepsSound.Play();

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        isWalking = targetDirection.x != 0 || targetDirection.y != 0;

        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

        if(controller.isGrounded)
            velocityY = -1f;
        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * walkSpeed + Vector3.up * velocityY;
        velocity *= moveSpeed;

        controller.Move(velocity * Time.deltaTime);
    }
}
