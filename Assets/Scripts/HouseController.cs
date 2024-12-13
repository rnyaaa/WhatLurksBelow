using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    public GameObject house;
    public GameObject jellyfish;
    public ValveInteraction valve1;
    public ValveInteraction valve2;

    // Start is called before the first frame update
    void Start()
    {
        house.SetActive(false);
        jellyfish.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(valve1.finished && valve2.finished)
        {
            house.SetActive(true);  
            jellyfish.SetActive(true);
        }
    }
}
