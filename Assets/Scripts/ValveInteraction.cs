using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValveInteraction : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private Text interactionText;
    private bool inRange = false;

    void Start()
    {
        interactionText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(inRange && Input.GetKeyDown("e"))
        {
            FixValve();
        }
    }

    void FixValve()
    {
        transform.Rotate(Vector3.up, 90f, Space.Self);
        interactionText.gameObject.SetActive(false);
        inRange = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = true;
            interactionText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = false;
            interactionText.gameObject.SetActive(false);
        }
    }

}
