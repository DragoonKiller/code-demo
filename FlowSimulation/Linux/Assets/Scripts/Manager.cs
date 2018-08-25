using UnityEngine;
using System;
using System.IO;
using Unity.Collections;
using Unity.Jobs;

public class Manager : MonoBehaviour
{
    public ObjectPool pool;
    
    [Header("Physics parameters.")]
    public float mass;
    public float repulsionCoincident;
    public float repulsionLevel;
    public float attractionCoincident;
    public float attractionLevel;
    public float limitCoincident;
    public float limitLevel;
    public float maxForce;
    
    [Header("System parameters.")]
    public float generateDelay;
    
    [Header("Particle properties.")]
    public Vector2 initVelocirty;
    public Vector2 generateLineFrom;
    public Vector2 generateLineTo;
    public Rect limitArea;
    public int iteration;
    
    [Header("Display settings.")]
    public bool selectiveShow;
    public int selectiveConst;
    
    void Start()
    {
        Debug.Log("Manager Enter.");
        UnitySystemConsoleRedirector.Redirect();
        
        SafeExecute(() => Environment.Init());
    }
    
    void Update()
    {
        SafeExecute(() => UpdateConfig());
        SafeExecute(() => SyncData());
        SafeExecute(() => Step(iteration));
    }
    
    void OnDestroy()
    {
        Environment.Dispose();
        Console.Out.Close();
        Debug.Log("Manager Exit.");
    }
    
    // ================================================================================================================
    // Local functions.
    // ================================================================================================================
    
    void SafeExecute(Action action)
    {
        try
        {
            action();
        }
        catch(Exception e)
        {
            Debug.LogError("An exception has been thrown from Environment.");
            Debug.LogError("Exception : " + e.Message);
            DestroyImmediate(this);
        }
    }
    
    void UpdateConfig()
    {
        UpdateParameter("Mass", mass);
        UpdateParameter("GeneratingDelay", generateDelay);
        UpdateParameter("RepulsionCoincident", repulsionCoincident);
        UpdateParameter("RepulsionLevel", repulsionLevel);
        UpdateParameter("AttractionCoincident", attractionCoincident);
        UpdateParameter("AttractionLevel", attractionLevel);
        UpdateParameter("LimitCoincident", limitCoincident);
        UpdateParameter("LimitLevel", limitLevel);
        UpdateParameter("MaxForce", maxForce);
        
        Vector2 curInitVelocity = Environment.GetInitialVelocity().vector;
        if(curInitVelocity != initVelocirty)
        {
            Environment.SetInitialVelocity(new PIVector2(initVelocirty));
        }
        
        Rect curGenerateLine = Environment.GetGeneratingLine().rect;
        Rect confGenerateLine = new Rect(generateLineFrom, generateLineTo - generateLineFrom);
        if(curGenerateLine != confGenerateLine)
        {
            Environment.SetGeneratingLine(new PIRect(confGenerateLine));
        }
        
        Rect curLimitArea = Environment.GetLimitArea().rect;
        if(curLimitArea != limitArea)
        {
            Environment.SetLimitArea(new PIRect(limitArea));
        }
        
        Environment.SetSelectiveConstant(selectiveConst);
    }
    
    void SyncData()
    {
        selectiveConst = Mathf.Clamp(selectiveConst, 1, int.MaxValue);
        
        int cnt = Environment.GetParticlesCount(selectiveShow);
        pool.SetBusy(cnt);
        
        unsafe
        {
            PIVector2* p = (PIVector2*)Environment.GetParticles(selectiveShow).ToPointer();
            int cc = 0;
            
            foreach(var obj in pool.busyObjects)
            {
                obj.transform.position = p[cc].vector;
                cc += 1;
            };
        }
        
        particleCount = selectiveShow ? Environment.GetParticlesCount(false) : cnt;
        particleDisplay = cnt;
    }
    
    void Step(int iter)
    {
        for(int i=0; i<iter; i++)
        {
            Environment.Step(Time.deltaTime / iteration);
        }
    }
    
    void UpdateParameter(string s, float val)
    {
        if(Environment.GetParameter(s) != val)
        {
            Debug.LogFormat("reset {0} : {1} -> {2}", s, Environment.GetParameter(s), val);
            Environment.SetParameter(s, val);
        }
    }
    
    // ================================================================================================================
    // Variables that only for displaying purpose.
    // ================================================================================================================
    
    public int particleDisplay;
    public int particleCount;
    
}
