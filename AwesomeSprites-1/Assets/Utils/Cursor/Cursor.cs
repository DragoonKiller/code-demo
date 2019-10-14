using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class ExCursor
    {
        public static Vector2 worldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
