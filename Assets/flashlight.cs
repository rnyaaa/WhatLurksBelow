using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogController : MonoBehaviour
{
    public GameObject flashlightControllerObject; // Reference to the GameObject with the FlashlightController
    public float nearAttenuationDistance = 5f;  // Distance when flashlight is on
    public float farAttenuationDistance = 20f; // Distance when flashlight is off

    private Volume volume; // Reference to the Volume component
    private VolumetricFogVolumeComponent fog; // Reference to the Fog component
    private FlashlightController flashlightController; // Reference to the FlashlightController script

    void Start()
    {
        // Get the Volume component
        volume = GetComponent<Volume>();
        if (volume == null)
        {
            Debug.LogError("No Volume component found on this GameObject.");
            return;
        }

        // Get the Fog component from the Volume Profile
        if (!volume.profile.TryGet(out fog))
        {
            Debug.LogError("No Fog property found in Volume Profile.");
            return;
        }

        // Get the FlashlightController script
        flashlightController = flashlightControllerObject.GetComponent<FlashlightController>();
        if (flashlightController == null)
        {
            Debug.LogError("FlashlightController script not found on the referenced GameObject.");
        }
    }

    void Update()
    {
        // Ensure references are valid
        if (flashlightController == null || fog == null)
            return;

        // Update the Attenuation Distance based on the flashlight state
        fog.attenuationDistance.value = flashlightController.isOn ? farAttenuationDistance : nearAttenuationDistance;
    }
}

