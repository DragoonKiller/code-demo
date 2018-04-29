using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;

public partial class Test
{
    public void TestPerformance(int repeat)
    {
        CompilerSettings cts = new CompilerSettings();
        cts.Unsafe = true;
        CompilerContext ctx = new CompilerContext(cts, new ConsoleReportPrinter());
        
        // ------------------------------------------------------------------------------------------------------------
        // Commonly, if I want to use a string like a script, without loading it into currnet AppDomain,
        // I should do either
        // (1) manually manage the string, replace variables in it, and then evaluate.
        // (2) import a class written inside the string, then create an object for it, invoke method by reflection.
        // Notice the loop of evaluate will cause memory leak, so it's necessary to re-new a Evaluator.
        // Creating a evaluator is slow, ba careful when testing with other operations.
        // ------------------------------------------------------------------------------------------------------------
        
        const int MOD = 100;
        Stopwatch st = new Stopwatch();
        
        {
            const string evExpDef = @"a + b";
            
            st.Reset();
            st.Start();
            Random rd = new Random();
            for(int i=0; i<repeat; i++)
            {
                int a = rd.Next() % MOD;
                int b = rd.Next() % MOD;
                
                string def =
                    "int a = " + a.ToString() + ";" + "\n" +
                    "int b = " + b.ToString() + ";" + "\n";
                    
                Evaluator ev = new Evaluator(ctx);
                int ans = (int)ev.Evaluate(def + evExpDef);
                Debug.Assert(ans == a + b);
            }
            Console.WriteLine("Performance: string replacement time usage " + st.Elapsed.TotalMilliseconds / repeat + "ms per calc");
        }
        
        {
            // 32 MB memory limitation.
            const int bytesLimit = 1<<25;
            Debug.Assert(Process.GetCurrentProcess().PrivateMemorySize64 <= bytesLimit);
        }
        
        
        {
            Evaluator ev = new Evaluator(ctx);
            
            const string evClassDef =
            @"
                public class EVTest
                {
                    public int Add(int a, int b) { return a + b; }
                }
            ";
            
            const string evExpression = @"new EVTest();";
            
            ev.Run(evClassDef);
            var mt = ev.Compile(evExpression);
            object evTest = null;
            mt.Invoke(ref evTest);
            
            st.Reset();
            st.Start();
            Random rd = new Random();
            for(int i=0; i<repeat; i++)
            {
                int a = rd.Next() % MOD;
                int b = rd.Next() % MOD;
                int ans = (int)evTest.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, evTest, new object[]{a, b});
                Debug.Assert(ans == a + b);
            }
            Console.WriteLine("Performance: reflection time usage " + st.Elapsed.TotalMilliseconds / repeat + "ms per calc");
        }
        
        {
            // 32 MB memory limitation.
            const int bytesLimit = 1<<25;
            Debug.Assert(Process.GetCurrentProcess().PrivateMemorySize64 <= bytesLimit);
        }
        
        Console.WriteLine("Test.MethodInvoke Finished.");
    }
}
