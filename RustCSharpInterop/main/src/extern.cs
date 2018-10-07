using System;
using System.Runtime.InteropServices;
using System.Runtime;

public static class Lib
{
    [DllImport("logic", EntryPoint = "add")]
    public static extern int Add(int a, int b);
    
    [DllImport("logic", EntryPoint = "mult")]
    public static extern long Mult(int a, int b);
    
    [DllImport("logic", EntryPoint = "init")]
    public static extern void Init(ulong size);
    
    [DllImport("logic", EntryPoint = "add_up")]
    public static extern void Addup(ulong size);
    
    [DllImport("logic", EntryPoint = "set")]
    public static extern void Set(ulong id, int value);
    
    [DllImport("logic", EntryPoint = "index")]
    public static extern int Get(ulong id);
    
    [DllImport("logic", EntryPoint = "usize_length")]
    public static extern ulong GetUsizeLength();
    
    [DllImport("logic", EntryPoint = "get_pointer")]
    public static extern unsafe int* GetPointer();
    
}
