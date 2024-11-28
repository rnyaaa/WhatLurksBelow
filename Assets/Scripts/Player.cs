using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController character_controller;
    public float speed = 0.1f;
    public float sensitivity = 500f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private Vector3 gravity = new Vector3(0, -4.2f, 0);
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = velocity + transform.forward * Input.GetAxis("Vertical");
        velocity = velocity + transform.right * Input.GetAxis("Horizontal");
        velocity = velocity + gravity;
        character_controller.Move(velocity*Time.deltaTime*speed);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        velocity = velocity * 0.99f;
    }
}
