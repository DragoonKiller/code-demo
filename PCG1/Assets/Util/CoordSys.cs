using System;
using UnityEngine;

public struct CoordSys
{
    // Relative to "world" coordinate system.
    public Vector2 x;
    public Vector2 y;
    public CoordSys(Vector2 x, Vector2 y)
    {
        this.x = x;
        this.y = y;
    }
    
    public Vector2 LocalToWorld(Vector2 v) => v.x * x + v.y * y;
    public Vector2 WorldToLocal(Vector2 v) => new Vector2(v.Dot(x), v.Dot(y));
}
