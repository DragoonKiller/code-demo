using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;

public class __Main__
{

    /// <summary>
    /// Test for C# built-in SIMD.
    /// Notice this optimization requires
    /// (1) [not sure] Compile option: Optimize -> true.
    /// (2) Compile option: PlatformTarget -> x64.
    /// </summary>
    public static void Main(string[] args)
    {
        Console.WriteLine("Hardware accelerated: {0}", Vector.IsHardwareAccelerated);
        
        Test.Basic();
        Test.Sweep();
    }
    
}
