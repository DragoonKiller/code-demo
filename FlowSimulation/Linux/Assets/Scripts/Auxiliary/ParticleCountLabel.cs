using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(Text))]
public sealed class ParticleCountLabel : MonoBehaviour
{
    Text text;
    public Manager manager;
    
    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }
    
    void FixedUpdate()
    {
        text.text = "Particles count: " + manager.particleCount;
    }
}
