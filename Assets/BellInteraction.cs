using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BellInteraction : MonoBehaviour
{
    public GameObject player;
    public float range;
    [SerializeField] private Text startText;
    [SerializeField] private Text finishText;
    public GameObject valveObject;
    private bool game_finished = false;

    void Update()
    {
        ValveInteraction valve = valveObject.GetComponent<ValveInteraction>();
        if ((player.transform.position - transform.position).magnitude < range)
        {
            if (!valve.finished)
            {
                startText.gameObject.SetActive(true);

            }else{
                startText.gameObject.SetActive(false);
                finishText.gameObject.SetActive(true);
                if (Input.GetKeyDown("e"))
                {
                    finishText.gameObject.SetActive(false);
                    game_finished = true;
                }
            }
        }
        else
        {
            finishText.gameObject.SetActive(false);
            startText.gameObject.SetActive(false);
        }
    }
}
