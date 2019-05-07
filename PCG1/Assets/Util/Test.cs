using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    public bool edgeExtract;
    public bool linkedListStorage;
    public bool set;
    
    void RunIf(ref bool trigger, Action action)
    {
        if(trigger)
        {
            string name = action.Method.ReflectedType.Name + "." + action.Method.Name;
            Debug.Log("test " + name + " begin.");
            action();
            trigger = false;
            Debug.Log("test " + name + " end.");
        }
    }
    
    void Update()
    {
        RunIf(ref edgeExtract, Util.EdgeExtractTest);
        RunIf(ref linkedListStorage, Util.LinkedListTest);
        RunIf(ref set, Util.SetTest);
    }
    
}
