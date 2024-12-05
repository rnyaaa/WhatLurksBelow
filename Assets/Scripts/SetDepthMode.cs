using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDepthMode : MonoBehaviour
{
    private Camera playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        playerCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
