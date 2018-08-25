#if __WINDOWS__

using System;
using System.Runtime.InteropServices;
using System.IO;

using System.Collections.Generic;

public static class Native
{
    static readonly Dictionary<string, IntPtr> moduleCache = new Dictionary<string, IntPtr>();
    
    public static void ClearCache()
    {
        foreach(var i in moduleCache)
        {
            FreeLibrary(i.Value);
        }
        moduleCache.Clear();
    }
    
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool FreeLibrary(IntPtr module);
    
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern IntPtr LoadLibrary(string fileName);
    
    [DllImport("kernel32")]
    static extern IntPtr GetProcAddress(IntPtr module, string procName);
    
    
    public static T GetFunction<T>(string moduleName, string procName)
        where T : class
    {
        if(!moduleCache.ContainsKey(moduleName))
        {
            moduleCache.Add(moduleName, LoadLibrary(moduleName));
        }
        var handle = moduleCache[moduleName];
        IntPtr proc = GetProcAddress(handle, procName);
        return Marshal.GetDelegateForFunctionPointer<T>(proc);
    }
}

#endif
