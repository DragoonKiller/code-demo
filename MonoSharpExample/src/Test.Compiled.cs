using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;

public partial class Test
{
    public void TestCompiled(int milliseconds)
    {
        CompilerSettings cts = new CompilerSettings();
        cts.Unsafe = true;
        CompilerContext ctx = new CompilerContext(cts, new ConsoleReportPrinter());
        Evaluator ev = new Evaluator(ctx);
        
        // ------------------------------------------------------------------------------------------------------------
        // A compiled expression will be simple sealed as a function.
        // THis method is parameterless.
        // ------------------------------------------------------------------------------------------------------------
        
        Stopwatch st = new Stopwatch();
        st.Reset();
        st.Start();
        
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
        
        // Notice this is not C# itself, it is an REPL style C#.
        // This command will "import" this class to ev's execution context.
        ev.Run(evClassDef);
        
        // Looks like holding the returning object...
        CompiledMethod res = ev.Compile(evExpression);
        
        // This will obviously not cause memory leak...
        while(st.ElapsedMilliseconds <= milliseconds)
        {
            object evtest = null;
            res.Invoke(ref evtest);
            int ans = (int)evtest.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, evtest, new object[]{1, 2});
            Debug.Assert(ans == 1 + 2);
        }
        
        // 64MB memory limitation.
        const int bytesLimit = 1<<26;
        using(var proc = Process.GetCurrentProcess())
        {
            Debug.Assert(Process.GetCurrentProcess().PrivateMemorySize64 <= bytesLimit);
        }
        
        Console.WriteLine("Test.Compile Finished.");
    }
}
