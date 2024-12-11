using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform player; // Reference to the player object.
    public GameObject compass; // Rerefence to the compass.
    private Player playerScript; // Reference to the player movement script.
    private float originalSpeed;
    private bool isCompassVisible; // tracks the visibility state of the compass

    
    void Start()
    {
        // ensures the compass is hidden at start.
        if (compass != null)
        {
            compass.SetActive(false);
        }

        // stores the original walkspeed.
        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                originalSpeed = playerScript.walkSpeed; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // opens compass with the 'C' key.
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            isCompassVisible = !isCompassVisible;
            if (compass != null)
            {
                compass.SetActive(isCompassVisible);
            }

            // Adjust player speed based on compass visibility
            if (playerScript != null)
            {
                playerScript.walkSpeed = (float)(isCompassVisible ? 2 : originalSpeed);
            }
        }

        compass.transform.localRotation = Quaternion.Euler(0, 360 - transform.root.eulerAngles.y, 0);

        
    }
}
