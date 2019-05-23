using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime;
using System.Runtime.CompilerServices;

using UnityEngine;

public static partial class Collections
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T>(this IEnumerable<T> x)
    {
        var i = x.GetEnumerator();
        if(!i.MoveNext()) throw new ArgumentNullException();
        return i.Current;
    }
    
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<R, T>(this Dictionary<R, T> dict, R key, T defaultVal)
    {
        if(dict.TryGetValue(key, out T val)) return val;
        dict.Add(key, defaultVal);
        return default;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<R, T>(this Dictionary<R, T> dict, R key)
        where T : new()
    {
        if(dict.TryGetValue(key, out T val)) return val;
        var res = new T();
        dict.Add(key, res);
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddIntRange(this List<int> ls, int from, int to)
    {
        for(int i = from; i <= to; i++) ls.Add(i);
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Cnt<T>(this T[] s, Predicate<T> f)
    {
        int res = 0;
        foreach(var i in s) if(f(i)) res++;
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Cnt<T>(this ICollection<T> s, Predicate<T> f)
    {
        int res = 0;
        foreach(var i in s) if(f(i)) res++;
        return res;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Filter<T>(this T[] s, Predicate<T> f)
    {
        int cc = s.Cnt(f);
        var res = new T[cc];
        int cx = 0;
        foreach(var i in s) if(f(i)) res[cx++] = i;
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Filter<T>(this List<T> s, Predicate<T> f)
    {
        var res = new List<T>();
        foreach(var i in s) if(f(i)) res.Add(i);
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Map<T, F>(this F[] s, Func<F, T> f)
    {
        var res = new T[s.Length];
        for(int i=0; i<s.Length; i++) res[i] = f(s[i]);
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Map<T, F>(this List<F> s, Func<F, T> f)
    {
        var x = new List<T>();
        foreach(var i in s) x.Add(f(i));
        return x;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Map<F, T>(this ICollection<F> s, ICollection<T> t, Func<F, T> f)
    {
        foreach(var i in s) t.Add(f(i));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ICollection<T> Fold<F, T>(this ICollection<F> s, ICollection<T> t, Func<ICollection<T>, F, ICollection<T>> f)
    {
        foreach(var i in s) t = f(t, i);
        return t;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IList<T> Fold<F, T>(this IList<F> s, IList<T> t, Func<IList<T>, F, IList<T>> f)
    {
        foreach(var i in s) t = f(t, i);
        return t;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Fold<F, T>(this List<F> s, List<T> t, Func<List<T>, F, List<T>> f)
    {
        foreach(var i in s) t = f(t, i);
        return t;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Add<T>(this List<T> x, T a, T b, params T[] c)
    {
        // The single-parameter Add function will not match this.
        x.Add(a);
        x.Add(b);
        foreach(var i in c) x.Add(i);
        return x;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindOrDefault<T>(this T[] s, Predicate<T> f, T def)
    {
        for(int i=0; i<s.Length; i++) if(f(s[i])) return s[i];
        return def;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindOrNew<T>(this T[] s, Predicate<T> f) where T : new()
    {
        for(int i=0; i<s.Length; i++) if(f(s[i])) return s[i];
        return new T();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindIndex<T>(this T[] s, Predicate<T> f)
    {
        for(int i=0; i<s.Length; i++) if(f(s[i])) return i;
        return -1;
    }

}
