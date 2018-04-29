using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;

public partial class Test
{
    public void TestRepeatedCalculation(int milliseconds)
    {
        CompilerSettings cts = new CompilerSettings();
        cts.Unsafe = true;
        CompilerContext ctx = new CompilerContext(cts, new ConsoleReportPrinter());
        Evaluator ev = new Evaluator(ctx);
        
        // ------------------------------------------------------------------------------------------------------------
        // The "Evaluate" Function will create temporary memory.
        // This memory can not be cleaned if there's no force GC.
        // I still don't know why it uses this kind of memory, and does not provide even a "Dispose()".
        // Anyway, a force GC can deal with it and prevent memory leak.
        // ------------------------------------------------------------------------------------------------------------
        
        Stopwatch st = new Stopwatch();
        st.Reset();
        st.Start();
        
        while(st.ElapsedMilliseconds <= milliseconds)
        {
            int ans = 0;
            ans = (int)ev.Evaluate("1 + 2");
            
            // Notice this doesn't alloc memory in heap.
            // Otherwise this test is not reasonable.
            Debug.Assert(ans == 1 + 2);
            
            // Without this forced GC, the memory will simply break the assertion below.
            // With this forced GC the memory consumption is stable.
            GC.Collect();
        }
        
        // 64MB memory limitation.
        const int bytesLimit = 1<<26;
        using(var proc = Process.GetCurrentProcess())
        {
            Debug.Assert(Process.GetCurrentProcess().PrivateMemorySize64 <= bytesLimit);
        }
        
        Console.WriteLine("Test.RepeatedCalculation Finished.");
    }
}
