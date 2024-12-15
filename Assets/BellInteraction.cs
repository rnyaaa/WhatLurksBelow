using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BellInteraction : MonoBehaviour
{
    public GameObject player;
    public float range;

    [SerializeField] private Text warningText;

    [SerializeField] private Text controlsText;

    [SerializeField] private Text startText;
    public GameObject valveObject;
    private bool game_finished = false;

    void Update()
    {
        ValveInteraction valve = valveObject.GetComponent<ValveInteraction>();
        if ((player.transform.position - transform.position).magnitude < range)
        {
            if (!valve.finished)
            {
                warningText.gameObject.SetActive(true);
                controlsText.gameObject.SetActive(true);
                startText.gameObject.SetActive(true);

            }else{
                warningText.gameObject.SetActive(false);
                controlsText.gameObject.SetActive(false);
                startText.gameObject.SetActive(false);
                if (Input.GetKeyDown("e"))
                {
                    game_finished = true;
                }
            }
        }
        else
        {
            startText.gameObject.SetActive(false);
            warningText.gameObject.SetActive(false);
            controlsText.gameObject.SetActive(false);
        }
    }
}
