using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubemapDepth : MonoBehaviour
{
    Camera c;
    
    void Start()
    {
        c = this.gameObject.GetComponent<Camera>();
        c.depthTextureMode = DepthTextureMode.Depth;
    }
    
    void Update()
    {
        
    }
}
