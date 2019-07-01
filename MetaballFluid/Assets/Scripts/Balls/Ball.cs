using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rd => this.GetComponent<Rigidbody2D>();
    public float trumble;

    void FixedUpdate()
    {
        // Vector2 f = trumble * Vector2.one.Rot(Random.Range(0f, Mathf.PI * 2));
        Vector2 f = trumble * Random.Range(-1f, 1f) * Vector2.right;
        rd.AddForce(f, ForceMode2D.Impulse);
    }
}
