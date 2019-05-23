using System;
using System.Collections.Generic;
using UnityEngine;

public static class GenUtil
{
    public static float GetPop(Vector2 pos)
        => Mathf.PerlinNoise(pos.x / GlobalConfig.inst.genConfig.sampleSize.x, pos.y / GlobalConfig.inst.genConfig.sampleSize.y);
        
    public static void SubmitToMesh(Mesh mesh, List<Segment> segs, List<Color> colors)
    {
        var points = new List<Vector3>(segs.Count * 2);
        foreach(var s in segs) points.Add(s.from, s.to);
        mesh.SetVertices(points);
        
        var indices = new int[segs.Count * 2];
        for(int i=0; i<indices.Length; i++) indices[i] = i;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        
        mesh.SetColors(colors);
        
        mesh.RecalculateBounds();
    }
    
    public static int Create(List<Vector2> points, Vector2 x)
    {
        points.Add(x);
        return points.Count - 1;
    }
    
    public static int GetIndexOrCreate(List<Vector2> points, Vector2 x, int original = -1)
    {
        if(0 <= original && original < points.Count && points[original] == x) return original;
        for(int i=0; i<points.Count; i++) if(x == points[i]) return i;
        return Create(points, x);
    }
    
    /// If next placement intersects with placed segments,
    ///   it will be cut to connect to that intersection.
    public static Segment IntersectionCut(List<Segment> segs, Segment x)
    {
        var itscDist = x.dir.magnitude;
        foreach(var s in segs)
        {
            if(!x.asLine.Intersects(s.asLine)) continue;
            var itsc = x.asLine.Intersection(s.asLine);
            if(x.Cover(itsc, false) && s.Cover(itsc, true)) itscDist = x.from.To(itsc).magnitude;
        }
        x.to = x.from + x.dir.Len(itscDist);
        return x;
    }
    
    /// If placed segments' endpoints are too closed to next placement,
    ///   we connect the next placement to the closest endpoint.
    public static Segment CloseEndpointCut(List<Segment> segs, Segment x, float closeDist)
    {
        foreach(var s in segs)
        {
            if(x.HasCommonEndpoint(s)) continue;
            if(x.Dist(s.from) <= closeDist) x.to = s.from;
            if(x.Dist(s.to) <= closeDist) x.to = s.to;
        }
        return x;
    }
    
    /// If next placement's endpoint is close to already placed endpoints,
    ///   connect the placement to that endpoint.
    public static Segment CloseEndpointMerge(List<Segment> segs, Segment x, float closeDist)
    {
        float dist = x.dir.magnitude;
        Vector2 res = x.to;
        void TestAndSubmit(Vector2 pos)
        {
            var curd = x.to.To(pos).magnitude;
            if(curd <= closeDist && curd < dist) { res = pos; dist = curd; }
        }
        foreach(var s in segs)
        {
            TestAndSubmit(s.from);
            TestAndSubmit(s.to);
        }
        
        x.to = res;
        return x;
    }
    
    /// Report if next placement is too close to already placed segments. 
    public static bool HasSimilarSegment(List<Segment> segs, Segment x, float closeDist)
    {
        foreach(var s in segs)
        {
            if(s.to.CloseTo(x.from)) continue;
            float dx = s.Dist(x.from).Max(s.Dist(x.to));
            float dy = x.Dist(s.from).Max(x.Dist(s.to));
            if(dx < closeDist && dy < closeDist) return true;
        }
        return false;
    }
}
