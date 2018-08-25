using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(Text))]
public sealed class ParticleDisplayLabel : MonoBehaviour
{
    Text text;
    public Manager manager;
    
    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }
    
    void FixedUpdate()
    {
        text.text = "Particles display: " + manager.particleDisplay;
    }
}
