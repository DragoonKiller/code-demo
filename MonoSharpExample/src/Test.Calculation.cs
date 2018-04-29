using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;


public partial class Test
{
    public void TestCalculation()
    {
        CompilerSettings cts = new CompilerSettings();
        cts.Unsafe = true;
        CompilerContext ctx = new CompilerContext(cts, new ConsoleReportPrinter());
        Evaluator ev = new Evaluator(ctx);
        
        // ------------------------------------------------------------------------------------------------------------
        // Simply calculate something.
        // The expression style script in fact should be fully operational though.
        // ------------------------------------------------------------------------------------------------------------
        
        int ans = 0;
        ans = (int)ev.Evaluate("1 + 2");
        Debug.Assert(ans == 1 + 2);
        
        Console.WriteLine("Test.Calculation Finished.");
    }
    
    
    
    
}
