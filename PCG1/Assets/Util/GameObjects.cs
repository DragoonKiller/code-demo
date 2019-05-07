using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public static partial class Util
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForeachChild(this Transform t, Action<Transform> f)
    {
        for(int i=0; i<t.childCount; i++) f(t.GetChild(i));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForeachChild(this GameObject t, Action<GameObject> f)
        => ForeachChild(t.transform, (x) => f(x.gameObject));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForeachChild(this MonoBehaviour t, Action<GameObject> f)
        => ForeachChild(t.transform, (x) => f(x.gameObject));
}
