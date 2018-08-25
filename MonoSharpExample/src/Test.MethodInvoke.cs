using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;

public partial class Test
{
    public void TestMethodInvoke()
    {
        CompilerSettings cts = new CompilerSettings();
        cts.Unsafe = true;
        CompilerContext ctx = new CompilerContext(cts, new ConsoleReportPrinter());
        Evaluator ev = new Evaluator(ctx);
        
        // ------------------------------------------------------------------------------------------------------------
        // There we use class object to execute "A + B".
        // ------------------------------------------------------------------------------------------------------------
        {
            const string evClassDef =
            @"
                public class EVTest
                {
                    public int Add(int a, int b)
                    {
                        return a + b;
                    }
                }
            ";
            
            const string evExpression = @"new EVTest();";
            
            // Class definitions and so on should be parsed in Run() instead of Evaluate().
            ev.Run(evClassDef);
            
            // We cannot simply convert the returned object into "EVTest" class
            // or we will just load this class into current AppDomain (context).
            object opt = ev.Evaluate(evExpression);
            
            // This dynamic type is "EVTest".
            Debug.Assert(opt.GetType().Name == "EVTest");
            // Notice the way to call functions with parameters.
            // In practice this would better be a pointer I think....
            int ans = (int)opt.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, opt, new object[]{2, 5});
            
            Debug.Assert(ans == 2 + 5);
            Console.WriteLine("Test.MethodInvoke Finished.");
        }
    }
}
