using System;
using UnityEngine;
using System.Runtime;
using System.Runtime.CompilerServices;

// 
// 这个文件把许多有用的数学函数改成了后缀调用形式.
// Mathf.Max(a, b) ===> a.Max(b)
// 

namespace Utils
{

    public static partial class Maths
    {
        /// <summary>
        /// 返回数字 x 在剩余系 y 中的值. 这个值恒为非负数.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ModSys(this int x, int y) => x < 0 ? x % y + y : x % y;

        /// <summary>
        /// 返回数字 x 在剩余系 y 中的值. 这个值恒为非负数.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModSys(this long x, long y) => x < 0 ? x % y + y : x % y;

        /// <summary>
        /// 返回数字 x 在剩余系 y 中的值. 这个值恒为非负数.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ModSys(this float x, float y) => x < 0 ? x % y + y : x % y;

        /// <summary>
        /// 判断两个浮点数相等应当使用一个误差范围.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Feq(this float a, float b) => Mathf.Approximately(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LZ(this float x) => x <= -eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GZ(this float x) => x >= eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LEZ(this float x) => x <= eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GEZ(this float x) => x >= -eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EZ(this float x) => x.LEZ() && x.GEZ();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NZ(this float x) => x.LZ() || x.GZ();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LE(this float x, float y) => x <= y + eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GE(this float x, float y) => x >= y - eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool L(this float x, float y) => x <= y - eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool G(this float x, float y) => x >= y + eps;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int a, int l, int r) => a < l ? l : a > r ? r : a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float a, float l, float r) => a < l ? l : a > r ? r : a;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Xmap(this float x, float l, float r, float a, float b) => (x - l) / (r - l) * (b - a) + b;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float x) => Mathf.Abs(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int x) => Mathf.Abs(x);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdMax(this ref float x, float y) => x = Mathf.Max(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdMin(this ref float x, float y) => x = Mathf.Min(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this float x, float y) => Mathf.Max(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(this float x, float y) => Mathf.Min(x, y);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdMax(this ref int x, int y) => x = Mathf.Max(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdMin(this ref int x, int y) => x = Mathf.Min(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int x, int y) => Mathf.Max(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int x, int y) => Mathf.Min(x, y);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(this float x) => Mathf.Sin(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(this float x) => Mathf.Cos(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(this float x) => Mathf.Tan(x);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(this float x) => Mathf.Ceil(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float x) => Mathf.Round(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(this float x) => Mathf.Floor(x);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float x) => Mathf.CeilToInt(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(this float x) => Mathf.RoundToInt(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float x) => Mathf.FloorToInt(x);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float x, float y) => Mathf.Pow(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this int x, float y) => Mathf.Pow(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float x, int y) => Mathf.Pow(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow(this int x, int y) => (int)Math.Pow(x, y);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this float x) => Mathf.Sqrt(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this int x) => Mathf.Sqrt(x);
        


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqr(this float x) => x * x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqr(this int x) => x * x;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sgn(this float x) => x.LZ() ? -1 : x.GZ() ? 1 : 0;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(this Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 X(this Vector2 a, float x) => new Vector2(x, a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Y(this Vector2 a, float y) => new Vector2(a.x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 X(this Vector3 a, float x) => new Vector3(x, a.y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Y(this Vector3 a, float y) => new Vector3(a.x, y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Z(this Vector3 a, float z) => new Vector3(a.x, a.y, z);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVec2(this Vector3 a) => new Vector2(a.x, a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVec3(this Vector2 a) => new Vector3(a.x, a.y, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVec3(this Vector3 a, float z) => new Vector3(a.x, a.y, z);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 To(this Vector2 a, Vector2 b) => b - a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To(this Vector3 a, Vector3 b) => b - a;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Len(this Vector2 a, float z) => a.normalized * z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Angle(this Vector2 a, float v) => a.normalized.Rot(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Dir(this (Vector2 from, Vector2 to) a) => a.from.To(a.to);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2 a) => Mathf.Atan2(a.y, a.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rot(this Vector2 a, float t)
            => new Vector2(a.x * t.Cos() - a.y * t.Sin(), a.x * t.Sin() + a.y * t.Cos());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotHalfPi(this Vector2 a) => new Vector2(-a.y, a.x);

        /// <summary>
        /// 以 dir 为对称轴, dst 对应的对称点.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(this Vector2 dir, Vector2 dst)
            => Vector2.Reflect(dst, dir);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CloseTo(this Vector2 a, Vector2 b) => a.To(b).magnitude.LEZ();
    }
}
