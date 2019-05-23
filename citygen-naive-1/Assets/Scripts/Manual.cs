using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;

using UnityEngine;


public class Manual : MonoBehaviour
{
    public Vector2 from;
    public Vector2 to;
    
    public SpriteRenderer fromTag;
    public SpriteRenderer toTag;
    public LineRenderer line;
    
    Mesh mesh => this.GetComponent<MeshFilter>().mesh;
    readonly List<Segment> storage = new List<Segment>();
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            fromTag.enabled = true;
            toTag.enabled = true;
            line.enabled = true;
            fromTag.transform.position = from = Util.cursorWorldPosition;
        }
        
        if(toTag.enabled) toTag.transform.position = to = Util.cursorWorldPosition;
        if(line.enabled) line.SetPositions(new Vector3[]{ from, to });
        
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            var gen = new ManualGenerator() {
                maxNodeConnection = 100,
                mergeDist = Util.eps
            };
            
            var seg = gen.Validate(new Segment(from, to), storage);
            if(seg != null)
            {
                mesh.AddSegment(seg.Value);
                storage.Add(seg.Value);
            }
            fromTag.enabled = false;
            toTag.enabled = false;
            line.enabled = false;
        }
    }
    
}

public class ManualGenerator
{
    public int maxNodeConnection;
    public float mergeDist;
    public Segment? Validate(Segment x, List<Segment> storage)
    {
        // Count point usage.
        var usedTime = new Dictionary<Vector2, int>();
        foreach(var s in storage)
        {
            if(!usedTime.ContainsKey(s.from)) usedTime.Add(s.from, 0);
            if(!usedTime.ContainsKey(s.to)) usedTime.Add(s.to, 0);
            usedTime[s.from] += 1;
            usedTime[s.to] += 1;
        }
        
        // Remove too busy node.
        if(usedTime.ContainsKey(x.from) && usedTime[x.from] >= maxNodeConnection) return null;
        
        // CommonEndpointMerge.
        foreach(var s in storage)
        {
            if(AbleToMerge(x.from, s.from)) x.from = s.from;
            if(AbleToMerge(x.from, s.to)) x.from = s.to;
            if(AbleToMerge(x.to, s.from)) x.to = s.from;
            if(AbleToMerge(x.to, s.to)) x.to = s.to;
        }
        
        if(AbleToMerge(x.from, x.to)) return null;
        
        // Intersection to other segments.
        foreach(var s in storage)
        {
            if(x.HasCommonEndpoint(s)) continue;
            if(x.asLine.Intersects(s.asLine))
            {
                var itsc = x.asLine.Intersection(s.asLine);
                if(s.Cover(itsc, true) && x.Cover(itsc, false)) x.to = itsc;
            }
        }
        
        // Intersection by other segments.
        foreach(var s in storage)
        {
            if(x.HasCommonEndpoint(s)) continue;
            if(x.Dist(s.from).LEZ()) x.to = s.from;
            if(x.Dist(s.to).LEZ()) x.to = s.to;
        }
        
        // Duplicate.
        if(storage.Contains(x)) return null;
        
        
        return x;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool AbleToMerge(Vector2 a, Vector2 b) => a.To(b).magnitude <= mergeDist;
}
