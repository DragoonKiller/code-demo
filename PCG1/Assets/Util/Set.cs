using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static partial class Util
{
    /// A hash set with an unchanged size.
    [Serializable]
    public class Set<T>
        where T : struct, IEquatable<T>
    {
        readonly LinkedListStorage<T> lists;
        readonly LinkedListStorage<T>.ListNode?[] slots;
        
        public int dupCount
        {
            get
            {
                int cc = 0;
               foreach(var i in slots) if(i != null && i.Value.next.index != -1) cc++;
               return cc;
            }
        }
        
        public int maxLen
        {
            get
            {
                int cc = 0;
                foreach(var i in slots) if(i != null)
                {
                    int cx = 0;
                    var cur = i.Value;
                    while(cur.index != -1)
                    {
                        cx += 1;
                        cur = cur.next;
                    }
                    cc = Mathf.Max(cx, cc);
                }
                return cc;
            }
        }
        
        public Set(int capacity)
        {
            lists = new LinkedListStorage<T>(capacity);
            slots = new LinkedListStorage<T>.ListNode?[(int)(capacity * 1.371)];
        }
        
        public void Add(T val)
        {
            int hash = Util.ModSys(val.GetHashCode(), slots.Length);
            if(slots[hash] == null) { slots[hash] = lists.NewListHead(val); return; }
            bool found = false;
            foreach(var i in slots[hash]) if(i.Equals(val)) { found = true; break; }
            if(!found) lists.AddBefore(slots[hash].Value, val);
        }
        
        public bool Remove(T val)
        {
            int hash = Util.ModSys(val.GetHashCode(), slots.Length);
            
            // no list for this value.
            if(slots[hash] == null) return false;
            
            var res = lists.RemoveFrom(slots[hash].Value, val);
            
            // list does not contain this.
            if(res == null) return false;
            
            // removed the single point of the list.
            if(res.Value.index == -1) slots[hash] = null;
            
            // removed the point and get another list handle.
            else slots[hash] = res;
            return true;
        }
        
        public bool Contains(T val)
        {
            int hash = Util.ModSys(val.GetHashCode(), slots.Length);
            if(slots[hash] == null) return false;
            var res = lists.FindFrom(slots[hash].Value, val);
            if(res == null) return false;
            return true;
        }
        
        public void Foreach(Action<T> f)
        {
            foreach(var slot in slots) if(slot != null)
            {
                var cur = slot.Value;
                while(cur.prev.index != -1) cur = cur.prev;
                foreach(var node in cur) f(node);
            }
        }
    }
    
    
    public static void SetTest()
    {
        var X = new HashSet<int>();
        var A = new Set<int>(5);
        void Add(int a) { X.Add(a); A.Add(a); }
        void Remove(int a) { X.Remove(a); A.Remove(a); }
        void Contains(int a) { Debug.Assert(X.Contains(a) && A.Contains(a)); }
        void NotContains(int a) { Debug.Assert(!X.Contains(a) && !A.Contains(a)); }
        
        Add(1);
        Add(3);
        Add(5);
        Add(7);
        Add(9);
        Contains(3);
        Contains(5);
        NotContains(6);
        Add(5);
        Add(7);
        Add(3);
        Remove(3);
        Contains(1);
        Contains(5);
        NotContains(2);
        NotContains(4);
    }
}
