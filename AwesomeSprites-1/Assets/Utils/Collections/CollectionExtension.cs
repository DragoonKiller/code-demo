using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Utils
{
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
        public static T GetOrDefault<R, T>(this SortedList<R, T> dict, R key, T defaultVal)
        {
            if(dict.TryGetValue(key, out T val)) return val;
            dict.Add(key, defaultVal);
            return defaultVal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<R, T>(this SortedList<R, T> dict, R key)
            where T : new()
        {
            if(dict.TryGetValue(key, out T val)) return val;
            var res = new T();
            dict.Add(key, res);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<R, T>(this Dictionary<R, T> dict, R key, T defaultVal)
        {
            if(dict.TryGetValue(key, out T val)) return val;
            dict.Add(key, defaultVal);
            return defaultVal;
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
            for(int i = 0; i < s.Length; i++) if(f(s[i])) return s[i];
            return def;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FindOrNew<T>(this T[] s, Predicate<T> f) where T : new()
        {
            for(int i = 0; i < s.Length; i++) if(f(s[i])) return s[i];
            return new T();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>(this T[] s, Predicate<T> f)
        {
            for(int i = 0; i < s.Length; i++) if(f(s[i])) return i;
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> v)
        {
            while(v.MoveNext())
            {
                yield return v.Current;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ToEnumerable<T>(this T[] arr, int begin, int end)
        {
            for(int i = begin; i < end; i++) yield return arr[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ToEnumerable<T>(this T[] arr, int end)
        {
            for(int i = 0; i < end; i++) yield return arr[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this List<T> lst)
        {
            return lst[lst.Count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveLast<T>(this List<T> lst)
        {
            lst.RemoveAt(lst.Count - 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Resize<T>(this List<T> lst, int cnt) where T : struct
        {
            while(lst.Count > cnt) lst.RemoveLast();
            while(lst.Count < cnt) lst.Add(new T());
            return lst;
        }


    }
}
