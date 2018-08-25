using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(Text))]
public sealed class FPSLabel : MonoBehaviour
{
    public int frameCount;
    
    LinkedList<float> timeCounts = new LinkedList<float>();
    Text text;
    
    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }
    
    void Update()
    {
        int fc = Mathf.Clamp(frameCount, 1, 100);
        
        timeCounts.AddLast(Time.deltaTime);
        while(timeCounts.Count > fc)
            timeCounts.RemoveFirst();
        
        float total = 0;
        foreach(var i in timeCounts)
            total += i;
        
        text.text = "FPS: " + (timeCounts.Count / total).ToString("0.000000");
    }
}
