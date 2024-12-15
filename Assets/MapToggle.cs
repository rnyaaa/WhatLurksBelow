using UnityEngine;
using UnityEngine.UI;

public class MapToggleWithCoordinates : MonoBehaviour
{
    public Transform player;         // Reference to the player object
    public Canvas mapCanvas;         // Reference to the map canvas
    private Player playerScript;     // Reference to the Player movement script
    private float originalSpeed;
    private bool isMapVisible = false; // Track the visibility state of the map

    void Start()
    {
        // Ensure the map is hidden at the start
        if (mapCanvas != null)
        {
            mapCanvas.enabled = false;
        }        
        
        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                originalSpeed = playerScript.walkSpeed; // Store the original speed
            }
        }
    }

    void Update()
    {
        // Toggle map visibility with the 'M' key or 'Tab' key
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            isMapVisible = !isMapVisible;
            if (mapCanvas != null)
            {
                mapCanvas.enabled = isMapVisible;
            }

            // Adjust player speed based on map visibility
            if (playerScript != null)
            {
                playerScript.walkSpeed = (float)(isMapVisible ? 2 : originalSpeed);
            }
        }

        // Update coordinates and direction when the map is visible
        if (isMapVisible && player != null)
        {

            Vector3 position = player.position;
            string direction = GetFacingDirection(player.forward);
        }
    }

    string GetFacingDirection(Vector3 forward)
    {
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360; // Normalize to 0-360 degrees

        if (angle >= 337.5 || angle < 22.5) return "North";
        else if (angle >= 22.5 && angle < 67.5) return "Northeast";
        else if (angle >= 67.5 && angle < 112.5) return "East";
        else if (angle >= 112.5 && angle < 157.5) return "Southeast";
        else if (angle >= 157.5 && angle < 202.5) return "South";
        else if (angle >= 202.5 && angle < 247.5) return "Southwest";
        else if (angle >= 247.5 && angle < 292.5) return "West";
        else return "Northwest";
    }
}
