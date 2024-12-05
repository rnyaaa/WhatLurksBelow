using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValveInteraction : MonoBehaviour
{
    bool playerInRange = false;
    bool taskComplete = false;
    float speed = 2f;
    public Text interactionText;

    void Start()
    {
        interactionText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && Input.GetKeyDown("e") && !taskComplete)
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
            taskComplete = true;
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player" && !taskComplete) 
        {
            playerInRange = true;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "player") 
        {
            playerInRange = false;
            interactionText.gameObject.SetActive(false);
        }
    }

}
