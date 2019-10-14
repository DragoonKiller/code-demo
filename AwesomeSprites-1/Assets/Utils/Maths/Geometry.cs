using System;
using UnityEngine;
using System.Collections.Generic;

using System.Runtime;
using System.Runtime.CompilerServices;

// 
// 这个文件定义各种基本的几何数据结构和它们的基础算法.
// 

namespace Utils
{

    public static partial class Maths
    {
        /// <summary>
        /// 用两个点表示一条直线.
        /// </summary>
        [Serializable]
        public struct Line : IEquatable<Line>
        {
            public readonly Vector2 from;
            public readonly Vector2 to;

            public Vector2 dir => to - from;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Line(Vector2 from, Vector2 to) => (this.from, this.to) = (from, to);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator Line((Vector2 from, Vector2 to) a) => new Line(a.from, a.to);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out Vector2 from, out Vector2 to)
            {
                from = this.from;
                to = this.to;
            }

            /// <summary>
            /// 判断点到直线距离.
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Dist(Vector2 v) => (dir.Cross(from.To(v)) / dir.magnitude).Abs();

            /// <summary>
            /// 判断点是否在直线上.
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Cover(Vector2 v) => Dist(v).LEZ();

            /// <summary>
            /// 点到该直线的投影.
            /// </summary>
            /// <returns>投影点</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 ProjectionOf(Vector2 p)
            {
                float h = dir.Cross(from.To(p)).Abs() / dir.magnitude;
                float r = (dir.sqrMagnitude - h * h).Abs();
                if(r.Feq(0)) return from;
                if(dir.Dot(from.To(p)) < 0) return from + dir.normalized * (-r);
                else return from + dir.normalized * r;
            }

            /// <summary>
            /// 判断是否与另一直线相交.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Intersects(Line b) => !0.0f.Feq(dir.Cross(b.dir));

            /// <summary>
            /// 获得两直线的交点. 已经假设这两条直线不平行.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Intersection(Line b)
            {
                float ax = from.To(b.from).Cross(b.dir) / dir.Cross(b.dir);
                return from + ax * dir;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Line x)
                => dir.Cross(x.dir) == 0
                && dir.Cross(from.To(x.to)) == 0
                && dir.Cross(to.To(x.to)) == 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Line a, Line b) => a.Equals(b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Line a, Line b) => !(a == b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                int a = from.GetHashCode();
                int b = to.GetHashCode();
                return a + b - (a ^ b);
            }

            public override bool Equals(object o) => throw new InvalidOperationException();
        }


        /// <summary>
        /// 用两个点表示一条线段.
        /// </summary>
        [Serializable]
        public struct Segment : IEquatable<Segment>
        {
            public readonly Vector2 from;
            public readonly Vector2 to;
            public Vector2 dir => to - from;
            public Line asLine => new Line(from, to);
            public Vector2 center => (from + to) * 0.5f;
            public Rect AARect => new Rect(center, dir);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Segment(Vector2 from, Vector2 to) => (this.from, this.to) = (from, to);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator Segment((Vector2 from, Vector2 to) a) => new Segment(a.from, a.to);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out Vector2 from, out Vector2 to)
            {
                from = this.from;
                to = this.to;
            }

            /// <summary>
            /// 判断点是否在线段上.
            /// </summary>
            /// <param name="v"></param>
            /// <param name="strict">点在端点上时, 返回 !strict.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Cover(Vector2 v, bool strict = false)
            {
                var hf = from.To(v).Dot(dir.normalized);
                var ht = to.To(v).Dot(-dir.normalized);
                if(hf.GZ() && ht.GZ()) return asLine.Dist(v).LEZ();
                return strict ? false : from.To(v).magnitude.Min(to.To(v).magnitude).LEZ();
            }

            /// <summary>
            /// 点到线段上的任意点的最短距离.
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Dist(Vector2 v)
            {
                var hf = from.To(v).Dot(dir.normalized);
                var ht = to.To(v).Dot(-dir.normalized);
                if(hf.GZ() && ht.GZ()) return asLine.Dist(v);
                return from.To(v).magnitude.Min(to.To(v).magnitude);
            }

            /// <summary>
            /// 算出取到了"点到线段最短距离"的点.
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Nearest(Vector2 p)
            {
                Vector2 g = asLine.ProjectionOf(p);
                if(Cover(g)) return g;
                if(g.To(from).magnitude < g.To(to).magnitude) return from;
                return to;
            }

            /// <summary>
            /// 判断线段有公共交点.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasCommonEndpoint(Segment b)
                => from.CloseTo(b.from)
                || from.CloseTo(b.to)
                || b.from.CloseTo(from)
                || b.from.CloseTo(to);


            /// <summary>
            /// 判断线段是否相交.
            /// </summary>
            /// <param name="s"></param>
            /// <param name="strict">true时允许端点相交或端点在另一条线段上; 否则要求线段交点不能是端点.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Intersects(Segment s, bool strict = false)
            {
                // Fast elimination.
                if(!AARect.Overlaps(s.AARect)) return false;

                float a = dir.Cross(from.To(s.from));
                float b = dir.Cross(from.To(s.to));
                float c = s.dir.Cross(s.from.To(from));
                float d = s.dir.Cross(s.from.To(to));
                if(strict)
                    return ((a.LZ() && b.GZ()) || (a.GZ() && b.LZ())) &&
                           ((c.LZ() && d.GZ()) || (c.GZ() && d.LZ()));
                else
                    return ((a.LZ() && b.GZ()) || (a.GZ() && b.LZ()) || Cover(s.from, false) || Cover(s.to, false)) &&
                           ((c.LZ() && d.GZ()) || (c.GZ() && d.LZ()) || s.Cover(from, false) || s.Cover(to, false));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Segment x)
            {
                return (from == x.from && to == x.to)
                    || (from == x.to && to == x.from);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Segment a, Segment b) => a.Equals(b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Segment a, Segment b) => !(a == b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                int a = from.GetHashCode();
                int b = to.GetHashCode();
                return a + b - (a ^ b);
            }

            public override bool Equals(object o) => throw new InvalidOperationException();

            public override string ToString() => "Segment[" + from + "," + to + "]";
        }

        /// <summary>
        /// 用三个点表示一个三角形. 三个点的排列顺序是任意的.
        /// </summary>
        public struct Triangle
        {
            public readonly Vector2 a;
            public readonly Vector2 b;
            public readonly Vector2 c;

            /// <summary>
            /// 有向面积. 逆时针为正.
            /// </summary>
            public float area => a.To(b).Cross(a.To(c)) * 0.5f;

            /// <summary>
            /// 返回一个数字, 代表三个点 a -> b -> c 的顺序. <br/>
            /// 1 逆时针 <br/>
            /// -1 顺时针 <br/>
            /// 0 三点共线 <br/>
            /// </summary>
            public int sign => Mathf.RoundToInt(a.To(b).Cross(a.To(c)).Sgn());

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Triangle(Vector2 a, Vector2 b, Vector2 c) => (this.a, this.b, this.c) = (a, b, c);


            /// <summary>
            /// 判断点在三角形内.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="strict">是否允许点在三角形的边界上.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(Vector2 x, bool strict = false)
            {
                int ad = a.To(b).Cross(a.To(x)).Sgn();
                int bd = b.To(c).Cross(b.To(x)).Sgn();
                int cd = c.To(a).Cross(c.To(x)).Sgn();
                if(strict) return ad == bd && bd == cd;
                return (ad == bd && bd == cd)
                    || (ad == 0 && bd == cd)
                    || (bd == 0 && cd == ad)
                    || (cd == 0 && ad == bd)
                    || (ad.Abs() + bd.Abs() + cd.Abs() == 1); // one 1 or -1; two 0.
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator Triangle((Vector2 a, Vector2 b, Vector2 c) a) => new Triangle(a.a, a.b, a.c);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out Vector2 a, out Vector2 b, out Vector2 c)
            {
                a = this.a;
                b = this.b;
                c = this.c;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Triangle a, Triangle b) => a.Equals(b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Triangle a, Triangle b) => !(a == b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                int x = a.GetHashCode();
                int y = b.GetHashCode();
                int z = c.GetHashCode();
                return x + y + z - (x ^ y ^ z);
            }

            public override bool Equals(object o) => throw new InvalidOperationException();
        }
    }
}