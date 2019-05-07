using System;
using System.Runtime.CompilerServices;

using UnityEngine;

public static partial class Util
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ModSys(int i, int mod)
    {
        if(i < 0) return i % mod == 0 ? 0 : i % mod + mod;
        if(i >= mod) return i % mod;
        return i;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(this float x) => Mathf.Sqrt(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(this int x) => Mathf.Sqrt(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sqrt(this double x) => Math.Sqrt(x);
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqr(this float x) => x * x;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sqr(this int x) => x * x;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sqr(this double x) => x * x;
    
    
    public static float eps => 1e-6f;
    
    /// Less than zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LZ(this float x) => x <= -eps;
    
    /// Greater than zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GZ(this float x) => x >= eps;
    
    /// Less or equal to zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LEZ(this float x) => x <= eps;
    
    /// Greater or equal to zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GEZ(this float x) => x >= -eps;
    
    /// Equal to zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EZ(this float x) => x.LEZ() && x.GEZ();
    
    /// Not equal to zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NZ(this float x) => x.LZ() || x.GZ();
    
    /// Less or equal to.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LE(this float x, float y) => x <= y + eps;
    
    /// Greater or equal to.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GE(this float x, float y) => x >= y - eps;
    
    /// Less.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool L(this float x, float y) => x <= y - eps;
    
    /// Greater.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool G(this float x, float y) => x >= y + eps;
    
    /// Float equal in linear space.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Feq(this float a, float b) => Mathf.Approximately(a, b);
    
}
