using System.Collections.Generic;
using System;

public static partial class Util
{
    public class Cache<T>
    {
        Func<T> gen;
        T cache;
        bool cached;
        
        public Cache(Func<T> gen)
        {
            this.gen = gen; 
            cached = false;
        }
        
        public T value
        {
            get
            {
                if(!cached) return cache = gen();
                return cache;
            }
        }
        
        public void Clear() => cached = false;
    }
    
}
