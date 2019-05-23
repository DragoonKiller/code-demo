using System;
using UnityEngine;

public static partial class Util
{
    public static Vector2 cursorWorldPosition
    {
        get
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mousePos = ray.origin + ray.direction * (-ray.origin.z / Vector3.Dot(ray.direction, Vector3.forward));
            return mousePos;
        }
    }
}
