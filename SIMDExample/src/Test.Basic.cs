using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;

partial class Test 
{

    /// <summary>
    /// Test for C# built-in SIMD.
    /// Notice this optimization requires
    /// (1) Compile option: Optimize -> true.
    /// (2) [not sure] Compile option: PlatformTarget -> x64.
    /// (3) [not sure] Compile option: DebugType -> none.
    /// </summary>
    
    public static void Basic()
    {
        // Local functions are only available in C# 7 or later.
        void LessThan(uint[] target, uint[] L, uint[] R, int length)
        {
            int cnt = Vector<uint>.Count;
            int begin = 0;
            while(true)
            {
                if(begin + cnt <= length)
                {
                    Vector<uint> x = new Vector<uint>(L, begin);
                    Vector<uint> y = new Vector<uint>(R, begin);
                    // LessThan for uint: -1 for true, 0 for false.
                    Vector<uint> r = Vector.LessThan(x, y);
                    r.CopyTo(target, begin);
                }
                else break;
                begin += cnt;
            }
            for(int i=begin; i<length; i++)
            {
                target[i] = L[i] < R[i] ? 0xFFFFFFFF : 0x0;
            }
        }
    
        
        uint[] A = new uint[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
        uint[] B = new uint[]{5, 2, 4, 9, 3 ,3, 5, 1, 4, 10, 12, 19, 1, 13};
        uint[] C = new uint[20];
        
        LessThan(C, A, B, A.Length);
        
        Console.Write("A\t"); for(int i=0; i<A.Length; i++) Console.Write("{0}\t", A[i]); Console.WriteLine();
        Console.Write("B\t"); for(int i=0; i<A.Length; i++) Console.Write("{0}\t", B[i]); Console.WriteLine();
        Console.Write("C\t"); for(int i=0; i<A.Length; i++) Console.Write("{0}\t", C[i]); Console.WriteLine();
        
        for(int i=0; i<A.Length; i++) Debug.Assert(C[i] == (A[i] < B[i] ? 0xFFFFFFFF : 0));
    }
    
}
