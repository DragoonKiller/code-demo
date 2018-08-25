#if __LINUX__

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

using System.Collections.Generic;

public static class Native
{
    static readonly Dictionary<string, IntPtr> moduleCache = new Dictionary<string, IntPtr>();
    
    public static void ClearCache()
    {
        foreach(var i in moduleCache)
        {
            if(i.Value != IntPtr.Zero)
            {
                int res = FreeLibrary(i.Value);
                if(res != 0)
                {
                    Console.WriteLine("Native: unload library failed! res = " + res);
                }
            }
        }
        moduleCache.Clear();
    }
    
    [DllImport("dl", EntryPoint = "dlclose")]
    static extern int FreeLibrary(IntPtr module);
    
    [DllImport("dl", EntryPoint = "dlopen")]
    static extern IntPtr LoadLibrary(string fileName, uint tag);
    
    [DllImport("dl", EntryPoint = "dlsym")]
    static extern IntPtr GetProcAddress(IntPtr module, string procName);
    
    public static T GetFunction<T>(string moduleName, string procName)
        where T : class
    {
        if(!moduleCache.ContainsKey(moduleName))
        {
            const uint RTLD_NOW = 0x002;
            const uint RTLD_LOCAL = 0;
            moduleCache.Add(moduleName, LoadLibrary(moduleName, RTLD_NOW | RTLD_LOCAL));
            Console.WriteLine("Module handle: " + moduleCache[moduleName]);
        }
        var handle = moduleCache[moduleName];
        IntPtr proc = GetProcAddress(handle, procName);
        return Marshal.GetDelegateForFunctionPointer<T>(proc);
    }
}

#endif
