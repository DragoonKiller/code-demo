using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircArray<T>
{
    public T[] data;
    public CircArray(int n) => data = new T[n];
    public int Length => data.Length;
    public ref T this[int k] => ref data[k.ModSys(data.Length)];
}
