using System;
using UnityEngine;
using System.Collections.Generic;

using System.Runtime;
using System.Runtime.CompilerServices;

using static Util;


[Serializable]
public struct Line : IEquatable<Line>
{
    public Vector2 from;
    public Vector2 to;
    public Vector2 dir => to - from;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Line(Vector2 from, Vector2 to)
    {
        this.from = from;
        this.to = to;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Line((Vector2 from, Vector2 to) a) => new Line(a.from, a.to);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Vector2 from, out Vector2 to)
    {
        from = this.from;
        to = this.to;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Dist(Vector2 v) => (dir.Cross(from.To(v)) / dir.magnitude).Abs();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Cover(Vector2 v) => Dist(v).LEZ();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ProjectionOf(Vector2 p)
    {
        float h = dir.Cross(from.To(p)).Abs() / dir.magnitude;
        float r = (dir.sqrMagnitude - h * h).Abs();
        if(r.Feq(0)) return from;
        if(dir.Dot(from.To(p)) < 0) return from + dir.normalized * (-r);
        else return from + dir.normalized * r;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(Line b) => !0.0f.Feq(dir.Cross(b.dir));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Intersection(Line b)
    {
        float ax = from.To(b.from).Cross(b.dir) / dir.Cross(b.dir);
        return from + ax * dir;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Line x)
        => dir.Cross(x.dir) == 0
        && dir.Cross(from.To(x.to)) == 0
        && dir.Cross(to.To(x.to)) == 0;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(Line a, Line b) => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!=(Line a, Line b) => !(a == b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        int a = from.GetHashCode();
        int b = to.GetHashCode();
        return a + b - (a ^ b);
    }
    
    public override bool Equals(object o) => throw new InvalidOperationException();
}

[Serializable]
public struct Segment : IEquatable<Segment>
{
    public Vector2 from;
    public Vector2 to;
    public Vector2 dir => to - from;
    public Line asLine => new Line(from, to);
    public Vector2 center => (from + to) * 0.5f;
    public Rect AARect => new Rect(center, dir);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Segment(Vector2 from, Vector2 to)
    {
        this.from = from;
        this.to = to;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Segment((Vector2 from, Vector2 to) a) => new Segment(a.from, a.to);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Vector2 from, out Vector2 to)
    {
        from = this.from;
        to = this.to;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Cover(Vector2 v, bool strict = false)
    {
        var hf = from.To(v).Dot(dir.normalized);
        var ht = to.To(v).Dot(-dir.normalized);
        if(hf.GZ() && ht.GZ()) return asLine.Dist(v).LEZ();
        return strict ? false : from.To(v).magnitude.Min(to.To(v).magnitude).LEZ();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Dist(Vector2 v)
    {
        var hf = from.To(v).Dot(dir.normalized);
        var ht = to.To(v).Dot(-dir.normalized);
        if(hf.GZ() && ht.GZ()) return asLine.Dist(v);
        return from.To(v).magnitude.Min(to.To(v).magnitude);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Nearest(Vector2 p)
    {
        Vector2 g = asLine.ProjectionOf(p);
        if(Cover(g)) return g;
        if(g.To(from).magnitude < g.To(to).magnitude) return from;
        return to;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasCommonEndpoint(Segment b)
        => from.CloseTo(b.from)
        || from.CloseTo(b.to)
        || b.from.CloseTo(from)
        || b.from.CloseTo(to);
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(Segment s, bool strict = false)
    {
        // Fast elimination.
        if(!AARect.Overlaps(s.AARect)) return false;
        
        float a = dir.Cross(from.To(s.from));
        float b = dir.Cross(from.To(s.to));
        float c = s.dir.Cross(s.from.To(from));
        float d = s.dir.Cross(s.from.To(to));
        if(strict)
            return ((a.LZ() && b.GZ()) || (a.GZ() && b.LZ())) &&
                   ((c.LZ() && d.GZ()) || (c.GZ() && d.LZ()));
        else
            return ((a.LZ() && b.GZ()) || (a.GZ() && b.LZ()) || Cover(s.from, false) || Cover(s.to, false)) &&
                   ((c.LZ() && d.GZ()) || (c.GZ() && d.LZ()) || s.Cover(from, false) || s.Cover(to, false));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Segment x)
    {
        return (from == x.from && to == x.to)
            || (from == x.to && to == x.from);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(Segment a, Segment b) => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!=(Segment a, Segment b) => !(a == b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        int a = from.GetHashCode();
        int b = to.GetHashCode();
        return a + b - (a ^ b);
    }
    
    public override bool Equals(object o) => throw new InvalidOperationException();
    
    public override string ToString() => "Segment[" + from + "," + to + "]";
}

public struct Triangle
{
    public Vector2 a;
    public Vector2 b;
    public Vector2 c;
    
    /// Return the signed area of a triangle.
    /// The positive order is a->b->c counter-clockwise.
    public float area => a.To(b).Cross(a.To(c)) * 0.5f;
    
    /// return the order of a -> b -> c -> a.
    /// 1 : counter-clockwise
    /// -1 : clockwise
    /// 0 : degenerated
    public int sign => Mathf.RoundToInt(a.To(b).Cross(a.To(c)).Sgn());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 x, bool strict = false)
    {
        int ad = a.To(b).Cross(a.To(x)).Sgn();
        int bd = b.To(c).Cross(b.To(x)).Sgn();
        int cd = c.To(a).Cross(c.To(x)).Sgn();
        if(strict) return ad == bd && bd == cd;
        return (ad == bd && bd == cd)
            || (ad == 0 && bd == cd)
            || (bd == 0 && cd == ad)
            || (cd == 0 && ad == bd)
            || (ad.Abs() + bd.Abs() + cd.Abs() == 1); // one 1 or -1; two 0.
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Triangle((Vector2 a, Vector2 b, Vector2 c) a) => new Triangle(a.a, a.b, a.c);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Vector2 a, out Vector2 b, out Vector2 c)
    {
        a = this.a;
        b = this.b;
        c = this.c;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(Triangle a, Triangle b) => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!=(Triangle a, Triangle b) => !(a == b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        int x = a.GetHashCode();
        int y = b.GetHashCode();
        int z = c.GetHashCode();
        return x + y + z - (x ^ y ^ z);
    }
    
    public override bool Equals(object o) => throw new InvalidOperationException();
}

public struct CircleInterval
{
    // l <= r means the interval is [l, r]
    // r < l means the interval is [l, range] union [0, r]
    public float l;
    public float r;
    public float range;
    
    public float len => l < r ? r - l : r + range - l;
    
    public bool empty => range <= 0;
    
    public float midPoint => l <= r ? (l + r) * 0.5f : (l + r + range) * 0.5f % range;
    
    public static CircleInterval emptyInterval => new CircleInterval();
     
    bool crossOrigin => r < l;
    
    public CircleInterval(float l, float r, float range)
    {
        this.l = l % range;
        if(this.l < 0.0f) this.l += range;
        this.r = r % range;
        if(this.r < 0.0f) this.r += range;
        this.range = range;
    }
    
    // The intersection operation.
    public static CircleInterval operator&(CircleInterval a, CircleInterval b)
    {
        if(a.empty || b.empty) return CircleInterval.emptyInterval;
        if(a.range != b.range) throw new ArgumentException("range not equal");
        Debug.Log(a.l + " " + a.r + " " + a.len + " " + a.range * 0.5f);
        Debug.Log(b.l + " " + b.r + " " + b.len + " " + b.range * 0.5f);
        if(a.len.G(a.range * 0.5f) || b.len.G(b.range * 0.5f)) throw new ArgumentException("len must be smaller than 0.5 range");
        
        float range = a.range;
        
        if(a.crossOrigin && b.crossOrigin)
        {
            // They always have intersection because both cross the origin point. 
            a.r += range;
            b.r += range;
            return new CircleInterval(
                a.l.Max(b.l),
                a.r.Min(b.r) - range,
                range
            );
        }
        else if(a.crossOrigin)
        {
            a.r += a.range;
            if(b.l <= a.l && a.l <= b.r) return new CircleInterval(a.l, b.r, range);
            else if(b.l <= a.r - range && a.r - range <= b.r) return new CircleInterval(b.l, a.r, range);
            else return CircleInterval.emptyInterval;
        }
        else if(b.crossOrigin)
        {
            b.r += b.range;
            if(a.l <= b.l && b.l <= a.r) return new CircleInterval(b.l, a.r, range);
            else if(a.l <= b.r - range && b.r - range <= a.r) return new CircleInterval(a.l, b.r, range);
            else return CircleInterval.emptyInterval;
        }
        else
        {
            float l = a.l.Max(b.l);
            float r = a.r.Min(b.r);
            if(r < l) return CircleInterval.emptyInterval;
            return new CircleInterval(l, r, range);
        }
    }
}

public static partial class Util
{
    // ========================================================================
    // Rays
    // ========================================================================
    public static Segment ToSegment(this Ray x) => new Segment(x.origin, x.direction);
    public static Line ToLine(this Ray x) => new Line(x.origin, x.direction);
    
    // ========================================================================
    // Polygonal lines
    // ========================================================================
    
    public static Vector2 SmoothAverage(Vector2 center, Vector2 left, Vector2 right)
        => new Segment(center, new Segment(left, right).center).center;
    
    public static Vector2 SmoothShrink(Vector2 center, Vector2 left, Vector2 right)
    {
        if(left.To(right).RotHalfPi().Dot(center).LEZ()) return center;
        return new Segment(center, new Segment(left, right).center).center;
    }
    
    public static Vector2 SmoothFill(Vector2 center, Vector2 left, Vector2 right)
    {
        if(left.To(right).RotHalfPi().Dot(center).GEZ()) return center;
        return new Segment(center, new Segment(left, right).center).center;
    }
    
    // Reposition the vertices.
    // Return the source lists.
    public static List<List<Vector2>> Smooth(this List<List<Vector2>> src, Func<Vector2, Vector2, Vector2, Vector2> Smooth)
    {
        void SmoothWithoutEndpoints(List<Vector2> lst)
        {
            Vector2 prv = lst[0];
            Vector2 cur = lst[1];
            Vector2 nxt = lst[2];
            for(int i = 1; i < lst.Count - 1; i++)
            {
                var moved = Smooth(cur, prv, nxt);
                prv = lst[i];
                cur = lst[i + 1];
                nxt = lst[(i + 2).ModSys(lst.Count)];
                lst[i] = moved;
            }
        }
        
        foreach(var lst in src)
        {
            // The mesh is too small and is hard to smooth.
            if(lst.Count < 4) continue;
            
            // This list represents a closed polygon.
            if(lst[0] == lst[lst.Count - 1])
            {
                Vector2 sleft = lst[1];
                Vector2 sright = lst[lst.Count - 2];
                
                SmoothWithoutEndpoints(lst);
                
                lst[0] = lst[lst.Count - 1] = Smooth(lst[0], sleft, sright);
            }
            else // This list represents a segment line with two endpoints.
                 // We *DON'T* change endpoints for connection to adjacent links.
            {
                SmoothWithoutEndpoints(lst);
            }
        }
        
        return src;
    }
    
    /// Convert segments to lists.
    public static List<List<Vector2>> LinkUp(this List<Segment> sts)
    {
        var pts = new HashSet<Vector2>();
        foreach(var i in sts) { pts.Add(i.from); pts.Add(i.to); }
        
        var adj = new Dictionary<Vector2, HashSet<Vector2>>();
        foreach(var s in sts)
        {
            if(!adj.ContainsKey(s.from)) adj[s.from] = new HashSet<Vector2>();
            adj[s.from].Add(s.to);
            if(!adj.ContainsKey(s.to)) adj[s.to] = new HashSet<Vector2>();
            adj[s.to].Add(s.from);
        }
        
        var res = new List<List<Vector2>>();
        var used = new HashSet<Vector2>();
        
        void SetupList(Vector2 src)
        {
            used.Add(src);
            var lst = new List<Vector2>();
            res.Add(lst);
            var cur = src;
            
            // There shouldn't be any source point that equals to (NaN, NaN)...
            var prev = new Vector2(float.NaN, float.NaN);
            
            while(true)
            {
                lst.Add(cur);
                used.Add(cur);
                
                // Find the next point, which is not previous.
                Vector2? nxt = null;
                foreach(var i in adj[cur]) if(prev != i) { nxt = i; break; }
                
                // Non-closed path's end.
                if(nxt == null) break;
                
                // Repeat the source point if it is closed path.
                if(nxt == src) { lst.Add(src); break; }
                
                prev = cur;
                cur = nxt.Value;
            }
        }
        
        // Non-closed path should be in one collider
        //   so the start point of a non-closed path should be an end point.
        foreach(var src in pts) if(!used.Contains(src) && adj[src].Count == 1) SetupList(src);
        
        // Closed path's point is arbitary.
        foreach(var src in pts) if(!used.Contains(src)) SetupList(src);
        
        // Connect end-points to form a closed area.
        // This polygon then can be used in rendering.
        // TODO!
        
        return res;
    }
    
}
