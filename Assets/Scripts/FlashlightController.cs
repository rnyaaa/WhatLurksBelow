using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    public bool isOn; // Track whether the flashlight is on or off

    void Start()
    {
        flashlight = GetComponent<Light>();
        isOn = flashlight.enabled; // Initialize state
    }

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            isOn = !isOn; // Toggle state
            flashlight.enabled = isOn; // Update the flashlight
        }
    }
}