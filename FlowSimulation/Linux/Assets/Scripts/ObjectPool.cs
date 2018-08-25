using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject asset;
    public Vector3 farAwayPosition;
    
    public IEnumerable<GameObject> idleObjects { get { return idle; } }
    public IEnumerable<GameObject> busyObjects { get { return busy; } }
    
    readonly HashSet<GameObject> idle = new HashSet<GameObject>();
    readonly HashSet<GameObject> busy = new HashSet<GameObject>();
    
    public int idleCount { get { return idle.Count; } }
    public int busyCount { get { return busy.Count; } }
    public int totalCount { get { return idle.Count + busy.Count; } }
    
    
    public void Preserve(int sz)
    {
        while(idle.Count < sz)
        {
            int c = Mathf.FloorToInt(totalCount * 0.5f) + 1;
            Debug.LogFormat("extend pool size: {0} -> {1}", totalCount, totalCount + c);
            for(int i=0; i<c; i++)
            {
                var g = GameObject.Instantiate(asset, farAwayPosition, Quaternion.identity, this.transform);
                idle.Add(g);
            }
        }
    }
    
    public void SetBusy(int sz)
    {
        while(busy.Count < sz) Aquire();
        while(busy.Count > sz) Retire(GetFirst(busy));
    }
    
    public GameObject Aquire()
    {
        Preserve(1);
        var g = GetFirst(idle);
        idle.Remove(g);
        busy.Add(g);
        SetBusyState(g);
        return g;
    }
    
    public void Retire(GameObject x)
    {
        if(busy.Contains(x))
        {
            busy.Remove(x);
            idle.Add(x);
            SetIdleState(x);
        }
        else
        {
            throw new Exception("Try to remove a non-exist object from object pool.");
        }
    }
    
    // ================================================================================================================
    // ================================================================================================================
    // ================================================================================================================
    
    T GetFirst<T>(IEnumerable<T> s)
    {
        var iter = s.GetEnumerator();
        iter.MoveNext();
        var g = iter.Current;
        return g;
    }
    
    void SetBusyState(GameObject x)
    {
        // do nothing...
    }
    
    void SetIdleState(GameObject x)
    {
        x.transform.position = farAwayPosition;
    }
}
