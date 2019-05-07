using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public bool active;
    public GameObject target;
    [Range(0, 1)] public float closeRate;
    public Vector3 offset;
    public float minSpeed;
    void Update()
    {
        if(active)
        {
            var cur = this.transform.position;
            var tgt = target.transform.position + (Vector3)offset;
            var dir = (tgt - cur).normalized;
            var dst = (tgt - cur).magnitude;
            var mov = dst - dst * Mathf.Pow(closeRate, Time.deltaTime);
            mov = Mathf.Max(mov, minSpeed * Time.deltaTime);
            if(dst <= mov) this.transform.position = tgt;
            else this.transform.position = cur + dir * mov;
        }
    }
    
}
