using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using UnityEngine;

using static UnityEngine.Random;


[Serializable]
public abstract class GenElement : IComparable<GenElement>
{
    public int time;
    public Segment segment;
    public bool intercepted;
    
    public virtual Color color => Color.white;
    
    public abstract void Gen(SortedSet<GenElement> queue);
    
    public int CompareTo(GenElement x)
    {
        int v = time.CompareTo(x.time);
        if(v != 0) return v;
        v = segment.from.x.CompareTo(x.segment.from.x);
        if(v != 0) return v;
        v = segment.from.y.CompareTo(x.segment.from.y);
        if(v != 0) return v;
        v = segment.to.x.CompareTo(x.segment.to.x);
        if(v != 0) return v;
        return segment.to.y.CompareTo(x.segment.to.y);
    }
}

[Serializable]
public sealed class GenHighway : GenElement
{
    public override Color color => Color.yellow;
    
    [Serializable]
    public struct Config
    {
        public int maxHighwaySplit;
        
        public float sampleAngle;
        public int sampleCount;
        
        public float maxAngle;
        
        public int minForwawrdTime;
        public int maxForwardTime;
        
        public int minBranchCooldown;
        public int maxBranchCooldown;
        
        public float minLength;
        public float maxLength;
        
        public int randForwardTime => Range(minForwawrdTime, maxForwardTime + 1);
        public int randBranchCooldown => Range(minBranchCooldown, maxBranchCooldown + 1);
        public float randLen => Range(minLength, maxLength);
        public Vector2 RandRot(Vector2 v) => v.Rot(Range(-1f, 1f) * Mathf.Deg2Rad * maxAngle);
    }
    
    public float population;
    public int branchCooldown;
    
    public Config cfg => GlobalConfig.inst.highwayConfig;
    
    public GenHighway(Segment seg, float pop, int branchCooldown, int baseTime)
    {
        segment = seg;
        population = pop;
        this.branchCooldown = branchCooldown;
        base.time = baseTime;
    }
    
    public override void Gen(SortedSet<GenElement> queue)
    {
        if(intercepted) return;
        
        void BranchHighway(Vector2 dir, int cooldown, int time)
        {
            var nxtPos = segment.to + cfg.RandRot(dir);
            queue.Add(new GenHighway(new Segment(segment.to, nxtPos), GenUtil.GetPop(nxtPos), cooldown, this.time + time));
        }
        
        // ----------------------------------------------------------------------------------------
        // Left and right highway extension.
        // ----------------------------------------------------------------------------------------
        
        bool leftBranched = false;
        bool rightBranched = false;
        
        if(branchCooldown <= 0 && Range(0f, 2f / (-1 - branchCooldown)) <= population)
        {
            BranchHighway(segment.dir.RotHalfPi() * cfg.randLen, cfg.randBranchCooldown, cfg.randForwardTime);
            branchCooldown += cfg.randBranchCooldown;
            leftBranched = true;
        }
        
        if(branchCooldown <= 0 && Range(0f, 2f / (-1 - branchCooldown)) <= population)
        {
            BranchHighway((-segment.dir).RotHalfPi() * cfg.randLen, cfg.randBranchCooldown, cfg.randForwardTime);
            branchCooldown += cfg.randBranchCooldown;
            rightBranched = true;
        }
        
        // ----------------------------------------------------------------------------------------
        // Forward highway extension.
        // ----------------------------------------------------------------------------------------
        
        var lst = new List<(float pop, Vector2 pos)>(2 * cfg.sampleCount + 1);
        void SubmitToList(Vector2 nxtDir)
        {
            Vector2 nxtPos = segment.to + nxtDir;
            lst.Add((GenUtil.GetPop(nxtPos), nxtPos));
        }
        // Sample all potentional points, take the max 
        for(int i = -cfg.sampleCount; i <= cfg.sampleCount; i++)
        {
            SubmitToList(cfg.RandRot(segment.dir.Rot(cfg.sampleAngle * Mathf.Deg2Rad * i / cfg.sampleCount)));
        }
        // Sort for selection.
        lst.Sort((w, e) => (e.pop - population).Abs().CompareTo((w.pop - population).Abs()));
        // Add forward extension to the queue.
        BranchHighway(segment.to.To(lst[0].pos).Len(cfg.randLen), branchCooldown - 1, cfg.randForwardTime);
        
        // ----------------------------------------------------------------------------------------
        // Branch downtown roads.
        // ----------------------------------------------------------------------------------------
        
        if(!leftBranched && Range(0f, 1f) < population)
        {
            var nxtDir = segment.dir.RotHalfPi();
            queue.Add(new GenTown(segment.to, nxtDir, time));
        }
        
        if(!rightBranched && Range(0f, 1f) < population)
        {
            var nxtDir = (-segment.dir).RotHalfPi();
            queue.Add(new GenTown(segment.to, nxtDir, time));
        }
    }
}

public class GenTown : GenElement
{
    public override Color color => Color.green;
    
    [Serializable]
    public struct Config
    {
        public float minLength;
        public float maxLength;
        public float maxAngle;
        
        public float baseTime;
        public float minTime;
        public float popTimeCoefficient;
        
        public float randLen => Range(minLength, maxLength);
        public int GetTime(float pop) => (int)(Range(0f, 1f) + minTime.Max(baseTime - pop * popTimeCoefficient));
        public Vector2 RandRot(Vector2 x) => x.Rot(Range(-1f, 1f) * Mathf.Deg2Rad * maxAngle);
    }
    
    public Config cfg => GlobalConfig.inst.townConfig;
    
    public GenTown(Vector2 from, Vector2 dir, int baseTime)
    {
        segment.from = from;
        segment.to = from + dir.Len(cfg.randLen);
        this.time = baseTime + cfg.GetTime(GenUtil.GetPop(segment.to));
    }
    
    public override void Gen(SortedSet<GenElement> queue)
    {
        if(intercepted) return;
        
        void CheckAndSubmit(Vector2 dir)
        {
            Vector2 nxtPos = segment.to + cfg.RandRot(dir);
            float pop = GenUtil.GetPop(nxtPos);
            if(Range(0f, 1f) < pop) queue.Add(new GenTown(segment.to, dir, time));
        }
        
        CheckAndSubmit(segment.dir.Len(cfg.randLen));
        CheckAndSubmit(segment.dir.RotHalfPi().Len(cfg.randLen));
        CheckAndSubmit((-segment.dir).RotHalfPi().Len(cfg.randLen));
    }
}

public class Gen : MonoBehaviour
{
    [Serializable]
    public struct Info
    {
        public int currentHighwayCount;
        public int currentDowntownCount;
    }
    
    [Serializable]
    public struct Config
    {
        public int count;
        
        public Segment beginSegment;
        public float beginPopulation;
        
        public float segmentCloseDist;
        public float endpointCutDist;
        public float endpointMergeDist;
        public float removeLength;
        
        public Vector2 sampleSize;
    }
    
    public Info info;
    
    List<Segment> segs = new List<Segment>();
    List<Color> colors = new List<Color>();
    
    Mesh mesh => this.GetComponent<MeshFilter>().mesh;
    public Config cfg => GlobalConfig.inst.genConfig;
    
    /// The Generation Funtion
    IEnumerator GenStep()
    {
        var queue = new SortedSet<GenElement>();
        int hcount = 0;
        
        // ====================================================================
        // Highway generation phase.
        // ====================================================================
        
        // Initialization.
        // Simply create a highway segment.
        queue.Add(new GenHighway(cfg.beginSegment, cfg.beginPopulation, 0, 0));
        // Add a reversed begin segment.
        queue.Add(new GenHighway(
            new Segment(cfg.beginSegment.from, cfg.beginSegment.from - cfg.beginSegment.dir)
            , cfg.beginPopulation, 0, 0));
        
        for(int t=0; hcount < cfg.count && queue.Count != 0; t++)
        {
            info.currentHighwayCount = hcount + 1;
            var cur = queue.First();
            queue.Remove(cur);
            var x = cur.segment;
            
            var prevX = x;
            x = GenUtil.IntersectionCut(segs, x);
            x = GenUtil.CloseEndpointCut(segs, x, cfg.endpointCutDist);
            x = GenUtil.CloseEndpointMerge(segs, x, cfg.endpointMergeDist);
            if(GenUtil.HasSimilarSegment(segs, x, cfg.segmentCloseDist)) continue;
            if(x.dir.magnitude <= cfg.removeLength) continue;
            if(x != prevX) cur.intercepted = true;
            
            segs.Add(x);
            colors.Add(cur.color, cur.color);   // Two points per segment, so there're two colors.
            hcount += 1;
            
            cur.segment = x;
            cur.Gen(queue);
            
            GenUtil.SubmitToMesh(mesh, segs, colors);
            yield return null; // Next frame.
        }
        
        yield break; // Terminate.
    }
    
    void Start()
    {
        StartCoroutine("GenStep");
    }
    
    IEnumerator step;
    
    void Update()
    {
        
    }
}
