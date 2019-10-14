using System;
using UnityEngine;

namespace Utils
{
    public struct CoordSys
    {
        public Vector2 x;
        public Vector2 y;

        public CoordSys(Vector2 x, Vector2 y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 LocalToWorld(Vector2 v) => v.x * x + v.y * y;

        public Vector2 WorldToLocal(Vector2 v)
            => new Vector2(
                v.Cross(y) / x.Cross(y),
                v.Cross(x) / y.Cross(x)
            );

        public override string ToString() => "CoordSys[" + x.ToString() + "," + y.ToString() + "]";
        public string ToString(string p) => "CoordSys[" + x.ToString(p) + "," + y.ToString(p) + "]";
    }

}