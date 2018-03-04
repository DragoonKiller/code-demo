using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAround : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 axis;
    
    // Degree per sec.
    [SerializeField] float speed;
    
    Quaternion curRot;
    Vector3 curDir;
    
    void Start()
    {
        curRot = this.gameObject.transform.rotation;
        curDir = this.gameObject.transform.position - target.transform.position;
    }
    
    void Update()
    {
        Quaternion rotx = Quaternion.AngleAxis(Time.deltaTime * speed, axis);
        curRot = rotx * curRot;
        curDir = rotx * curDir;
        
        this.gameObject.transform.rotation = curRot;
        this.gameObject.transform.position = target.gameObject.transform.position + curDir;
    }
}
