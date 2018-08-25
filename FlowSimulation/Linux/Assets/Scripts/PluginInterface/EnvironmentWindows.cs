#if __WINDOWS__

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

public struct PIVector2
{
    public float x;
    public float y;
    
    public Vector2 vector { get { return new Vector2(x, y); } }
    
    public PIVector2(Vector2 vec)
    {
        x = vec.x;
        y = vec.y;
    }
}


public struct PIRect
{
    public float x;
    public float y;
    public float w;
    public float h;
    
    public Rect rect { get { return new Rect(x, y, w, h); } }
    
    public PIRect(Rect rc)
    {
        x = rc.xMin;
        y = rc.yMin;
        w = rc.width;
        h = rc.height;
    }
}

public static class Environment
{
    delegate PIVector2 FuncVec();
    delegate void ActionInt(int x);
    delegate void ActionVec(PIVector2 v);
    delegate PIRect FuncRect();
    delegate void ActionRect(PIRect v);
    delegate float FuncFloat();
    delegate void ActionFloat(float v);
    delegate int FuncInt();
    delegate int FuncBoolInt(bool v);
    delegate IntPtr FuncPtr();
    delegate IntPtr FuncBoolPtr(bool v);
    delegate void ActionPtrInt(IntPtr p, int c);
    delegate float FuncStrFloat(string s);
    delegate void ActionStrFloat(string s, float t);
    
    public static void Init()
    {
        string libName = "environment";
        EnvInit = Native.GetFunction<Action>(libName, "EnvInit");
        EnvDispose = Native.GetFunction<Action>(libName, "EnvDispose");
        var _Step = Native.GetFunction<ActionFloat>(libName, "Step");
        Step = x => _Step(x);
        var _GetMeshVertexCount = Native.GetFunction<FuncInt>(libName, "GetMeshVertexCount");
        GetMeshVertexCount = () => _GetMeshVertexCount();
        var _GetMesh = Native.GetFunction<FuncPtr>(libName, "GetMesh");
        GetMesh = () => _GetMesh();
        var _SetMesh = Native.GetFunction<ActionPtrInt>(libName, "SetMesh");
        InnerSetMesh = (arr, cnt) => _SetMesh(arr, cnt);
        var _GetParticlesCount = Native.GetFunction<FuncBoolInt>(libName, "GetParticlesCount");
        GetParticlesCount = (x) => _GetParticlesCount(x);
        var _GetSelectiveConstant = Native.GetFunction<FuncInt>(libName, "GetSelectiveConstant");
        GetSelectiveConstant = () => _GetSelectiveConstant();
        var _SetSelectiveConstant = Native.GetFunction<ActionInt>(libName, "SetSelectiveConstant");
        SetSelectiveConstant = x => _SetSelectiveConstant(x);
        var _GetParticles = Native.GetFunction<FuncBoolPtr>(libName, "GetParticles");
        GetParticles = x => _GetParticles(x);
        var _RemoveAllParticles = Native.GetFunction<FuncPtr>(libName, "RemoveAllParticles");
        RemoveAllParticles = () => _RemoveAllParticles();
        var _SetInitialVelocity = Native.GetFunction<ActionVec>(libName, "SetInitialVelocity");
        SetInitialVelocity = x => _SetInitialVelocity(x);
        var _GetInitialVelocity = Native.GetFunction<FuncVec>(libName, "GetInitialVelocity");
        GetInitialVelocity = () => _GetInitialVelocity();
        var _SetGeneratingLine = Native.GetFunction<ActionRect>(libName, "SetGeneratingLine");
        SetGeneratingLine = (x) => _SetGeneratingLine(x);
        var _GetGeneratingLine = Native.GetFunction<FuncRect>(libName, "GetGeneratingLine");
        GetGeneratingLine = () => _GetGeneratingLine();
        var _SetLimitArea = Native.GetFunction<ActionRect>(libName, "SetLimitArea");
        SetLimitArea = (x) => _SetLimitArea(x);
        var _GetLimitArea = Native.GetFunction<FuncRect>(libName, "GetLimitArea");
        GetLimitArea = () => _GetLimitArea();
        var _GetParameter = Native.GetFunction<FuncStrFloat>(libName, "GetParameter");
        GetParameter = (x) => _GetParameter(x);
        var _SetParameter = Native.GetFunction<ActionStrFloat>(libName, "SetParameter");
        SetParameter = (s, t) => _SetParameter(s, t);
        
        EnvInit();
    }
    public static void Dispose()
    {
        EnvDispose();
        Native.ClearCache();
    }
    
    static Action EnvInit;
    static Action EnvDispose;
    public static Action<float> Step;
    public static Func<int> GetMeshVertexCount;
    public static unsafe Func<IntPtr> GetMesh;
    static unsafe Action<IntPtr, int> InnerSetMesh;
    public static void SetMesh(PIVector2[] arr) { SetMesh(arr, 0, arr.Length); }
    public static void SetMesh(PIVector2[] arr, int len) { SetMesh(arr, 0, len); }
    public static void SetMesh(PIVector2[] arr, int begin, int end)
    {
        unsafe
        {
            fixed(PIVector2* p = arr)
            {
                InnerSetMesh(new IntPtr(p + begin), end - begin);
            }
        }
    }
    public static Action<int> SetSelectiveConstant;
    public static Func<int> GetSelectiveConstant;
    public static Func<bool, int> GetParticlesCount;
    public static Func<bool, IntPtr> GetParticles;
    public static Action RemoveAllParticles;
    public static Action<PIVector2> SetInitialVelocity;
    public static Func<PIVector2> GetInitialVelocity;
    public static Action<PIRect> SetGeneratingLine;
    public static Func<PIRect> GetGeneratingLine;
    public static Action<PIRect> SetLimitArea;
    public static Func<PIRect> GetLimitArea;
    public static Func<string, float> GetParameter;
    public static Action<string, float> SetParameter;
    
}

#endif
