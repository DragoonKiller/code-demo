using System;
using System.Text;

public class __Main__
{
    public static string LogStr(string s)
    {
        if(s == null || s.Length == 0) return "";
        StringBuilder sb = new StringBuilder();
        int cnt = 0;
        sb.Append(s[0]);
        for(int i=1; i<s.Length; i++)
        {
            if(s[i] == '}' && s[i-1] == '{') sb.Append(cnt++);
            sb.Append(s[i]);
        }
        return sb.ToString();
    }
    
    public static void LogLine(string s = "", params object[] args) => Console.WriteLine(string.Format(LogStr(s), args));
    public static void Log(string s = "", params object[] args) => Console.Write(string.Format(LogStr(s), args));
    
    public static void Main(String[] args)
    {
        LogLine("Test add (33, 79) : {}", Lib.Add(33, 79));
        LogLine("Test mul (153842372, 101743999) : {}", Lib.Mult(153842372, 101743999));
        LogLine("Test mul (-333, 1) : {}", Lib.Mult(-333, 1));
        LogLine("The usize value is represented in {} bytes.", Lib.GetUsizeLength());
        
        const ulong len = 20;
        LogLine("Test Init!");
        Lib.Init(len);
        LogLine("Init done!");
        
        var sets = new (ulong i, int v)[]{ (1, 3), (2, 1), (3, 7) };
        
        LogLine("Initial state:");
        for(ulong i=0; i<len; i++) Log("{} ", Lib.Get(i)); LogLine();
        LogLine("Setting begin:");
        foreach(var (i, v) in sets) { Lib.Set(i, v); }
        LogLine("After set:");
        for(ulong i=0; i<len; i++) Log("{} ", Lib.Get(i)); LogLine();
        LogLine("Addup half:");
        Lib.Addup(len / 2);
        for(ulong i=0; i<len; i++) Log("{} ", Lib.Get(i)); LogLine();
        LogLine("Add up:");
        Lib.Addup(len);
        for(ulong i=0; i<len; i++) Log("{} ", Lib.Get(i)); LogLine();
        LogLine("Modify with pointer directly:");
        unsafe
        {
            var newSets = new (ulong i, int v)[]{ (4, 3), (5, 7), (0, 1), (9, -13) };
            var p = Lib.GetPointer();
            foreach(var (i, v) in newSets) p[i] = v;
        }
        for(ulong i=0; i<len; i++) Log("{} ", Lib.Get(i)); LogLine();
    }
}
