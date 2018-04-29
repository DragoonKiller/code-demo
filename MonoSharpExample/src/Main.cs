using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using Mono.CSharp;

public class __Main__
{
    public static void Main(string[] args)
    {
        Test test = new Test();
        test.TestCalculation();
        test.TestMethodInvoke();
        test.TestRepeatedCalculation(milliseconds: 10000);
        test.TestCompiled(milliseconds: 10000);
        test.TestPerformance(repeat: 100);
    }
}
