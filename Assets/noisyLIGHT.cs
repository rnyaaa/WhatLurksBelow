using UnityEngine;

public class BoundedNoisyLightWithColorChange : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float noiseIntensity = 1f; // Maximum noise intensity per axis (degrees)
    public float noiseFrequency = 1.0f; // How quickly the noise changes
    public Vector3 baseRotationSpeed = Vector3.zero; // Optional base rotation

    [Header("Color Settings")]
    public Transform player; // Reference to the player object
    public Gradient colorGradient; // Gradient for color based on Y position
    public float minY = 0f; // Minimum Y position of the player
    public float maxY = 10f; // Maximum Y position of the player

    private Light directionalLight;
    private Quaternion initialRotation; // Store the initial rotation of the light
    private float noiseTimeX, noiseTimeY, noiseTimeZ;

    void Start()
    {
        // Get the Light component on this object
        directionalLight = GetComponent<Light>();
        if (directionalLight == null)
        {
            Debug.LogError("No Light component found on this object.");
        }

        // Store the initial rotation
        initialRotation = transform.rotation;

        // Initialize noise times with offsets
        noiseTimeX = Random.value * 100f;
        noiseTimeY = Random.value * 100f;
        noiseTimeZ = Random.value * 100f;
    }

    void Update()
    {
        // Update noise time
        noiseTimeX += Time.deltaTime * noiseFrequency;
        noiseTimeY += Time.deltaTime * noiseFrequency;
        noiseTimeZ += Time.deltaTime * noiseFrequency;

        // Calculate noisy offsets
        float xOffset = (Mathf.PerlinNoise(noiseTimeX, 0f) - 0.5f) * 2f * noiseIntensity;
        float yOffset = (Mathf.PerlinNoise(noiseTimeY, 0f) - 0.5f) * 2f * noiseIntensity;
        float zOffset = (Mathf.PerlinNoise(noiseTimeZ, 0f) - 0.5f) * 2f * noiseIntensity;

        // Apply rotation within bounds of the initial rotation
        Quaternion noiseRotation = Quaternion.Euler(xOffset, yOffset, zOffset);
        transform.rotation = initialRotation * noiseRotation;

        // Optional base rotation
        transform.Rotate(baseRotationSpeed * Time.deltaTime, Space.Self);

        // Color change based on player Y position
        if (player != null)
        {
            float playerY = Mathf.Clamp(player.position.y, minY, maxY);
            float t = Mathf.InverseLerp(minY, maxY, playerY); // Normalize Y position to [0, 1]
            if (directionalLight != null)
            {
                directionalLight.color = colorGradient.Evaluate(t);
            }
        }
    }
}
