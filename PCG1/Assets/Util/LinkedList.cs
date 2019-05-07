using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.CompilerServices;

using UnityEngine;


public static partial class Util
{
    /// Store multiple linked lists.
    /// If you lose the reference of ListNode, The list is lost anyway.
    /// Not support removing.
    public class LinkedListStorage<T> where T : struct
    {
        public struct ListNode : IEquatable<ListNode>, IEnumerable<T>
        {
            struct Enumerator : IEnumerator<T>
            {
                public int begin;
                public int index;
                public LinkedListStorage<T> src;
                public T Current => src.storage[index].val;
                object IEnumerator.Current => throw new NotImplementedException();
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    // first move.
                    if(index == -1)
                    {
                        index = begin;
                        return true;
                    }
                    
                    // reach the end.
                    int v = src.storage[index].next;
                    if(v == -1) return false;
                    
                    // normal.
                    index = v;
                    return true;
                }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Reset() => index = -1;
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Dispose() { }
            }
            
            public int index;
            public LinkedListStorage<T> src;
            
            public ListNode next => new ListNode(src.storage[index].next, src);
            public ListNode prev => new ListNode(src.storage[index].prev, src);
            public T val => src.storage[index].val;
            
            public ListNode(int index, LinkedListStorage<T> src)
            {
                this.src = src;
                this.index = index;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ListNode other) => index == other.index && src == other.src;
            
            /// You can do foreach(var i in x) where x is ListNode.
            /// This operation steps along the list.
            /// return Enumerator begin *before* the first node.
            public IEnumerator<T> GetEnumerator() => new Enumerator(){ begin = index, index = -1, src = src };
            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }
        
        struct Data
        {
            public T val;
            public int next;
            public int prev;
        }
        
        readonly Data[] storage;
        int count;
        
        public LinkedListStorage(int capacity)
        {
            storage = new Data[capacity];
            count = 0;
        }
        
        public T this[ListNode c]
        {
            get => storage[c.index].val;
            set => storage[c.index].val = value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListNode NewListHead(T val) => new ListNode(Add(val), this);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListNode AddBefore(ListNode x, T val)
        {
            int i = x.index;
            int g = Add(val);
            storage[g].prev = storage[i].prev;
            storage[g].next = i;
            if(storage[i].prev != -1) storage[storage[i].prev].next = g;
            storage[i].prev = g;
            return new ListNode(g, this);
        }
        
        /// Return a valid node from the list that removed the element.
        ///     or an invalid node if the whole list is removed completely.
        public ListNode Remove(ListNode x)
        {
            int i = x.index;
            if(storage[i].prev != -1) storage[storage[i].prev].next = storage[i].next;
            if(storage[i].next != -1) storage[storage[i].next].prev = storage[i].prev;
            
            // Find an adjacent valid node using its previous data.
            return new ListNode(
                i != x.index ? x.index
                : storage[i].prev != -1 ? storage[i].prev
                : storage[i].next != -1 ? storage[i].next
                : -1,
                this);
        }
        
        // Return null if nothing is foind.
        public ListNode? FindFrom(ListNode x, T val)
        {
            int i = x.index;
            while(storage[i].prev != -1) i = storage[i].prev;
            while(i != -1)
            {
                if(storage[i].val.Equals(val)) return new ListNode(i, this);
                i = storage[i].next;
            }
            return null;
        }
        
        // Return null if nothing is removed.
        // Return a ListNode whose index is -1 for successfully removed the whole list.
        public ListNode? RemoveFrom(ListNode x, T val)
        {
            int i = x.index;
            while(storage[i].prev != -1) i = storage[i].prev;
            var res = FindFrom(x, val);
            if(res != null) return Remove(res.Value);
            return null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Add(T val)
        {
            storage[count].val = val;
            storage[count].prev = storage[count].next = -1;
            return count++;
        }
    }
    
    
    public static void LinkedListTest()
    {
        var storage = new LinkedListStorage<int>(20);
        
        var a = storage.NewListHead(10);
        a = storage.AddBefore(a, -10);
        a = storage.AddBefore(a, -100);
        
        Debug.Assert(a.prev.index == -1);
        Debug.Assert(a.val == -100);
        Debug.Assert(a.next.val == -10);
        Debug.Assert(a.next.next.val == 10);
        Debug.Assert(a.next.next.next.index == -1);
        
        var b = storage.NewListHead(1);
        storage.AddBefore(b, 3);
        storage.AddBefore(b, 2);
        
        Debug.Assert(b.next.index == -1);
        Debug.Assert(b.val == 1);
        Debug.Assert(b.prev.val == 2);
        Debug.Assert(b.prev.prev.val == 3);
        Debug.Assert(b.prev.prev.prev.index == -1);
        
        storage.Remove(b.prev); // remove a list.
        // The ListNode b is changed, but is still valid.
        // The return value of Remove might be either node adjacent to the removed node.
        
        // A shall not change anyway.
        Debug.Assert(a.prev.index == -1);
        Debug.Assert(a.val == -100);
        Debug.Assert(a.next.val == -10);
        Debug.Assert(a.next.next.val == 10);
        Debug.Assert(a.next.next.next.index == -1);
        
        Debug.Assert(b.next.index == -1);
        Debug.Assert(b.val == 1);
        Debug.Assert(b.prev.val == 3);
        Debug.Assert(b.prev.prev.index == -1);
        
        b = storage.RemoveFrom(b, 1).Value; // remove the list.
        
        Debug.Assert(b.next.index == -1);
        Debug.Assert(b.val == 3);
        Debug.Assert(b.prev.index == -1);
        
        b = storage.RemoveFrom(b, 3).Value;
        Debug.Assert(b.index == -1);
    }
    
}
