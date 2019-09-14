using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float degreePerSec;
    void Update()
    {
        this.transform.rotation *= Quaternion.Euler(0, degreePerSec * Time.deltaTime, 0);
    }
}
