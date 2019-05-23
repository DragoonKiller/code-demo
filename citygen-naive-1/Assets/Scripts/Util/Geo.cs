using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;
using System.Runtime.CompilerServices;

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

public struct IndexSegment : IEquatable<IndexSegment>
{
    public int a;
    public int b;
    
    public Vector2 GetFrom(List<Vector2> x) => x[a];
    public Vector2 GetTo(List<Vector2> x) => x[b];
    public Segment Get(List<Vector2> x) => new Segment(x[a], x[b]); 
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IndexSegment(int from, int to)
    {
        this.a = from;
        this.b = to;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out int from, out int to)
    {
        from = a;
        to = b;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(IndexSegment a, IndexSegment b) => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!=(IndexSegment a, IndexSegment b) => !(a == b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IndexSegment v) => a == v.a && b == v.b;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        int x = a.GetHashCode();
        int y = b.GetHashCode();
        return x + y - (x ^ y);
    }
    
    public override bool Equals(object o) => throw new InvalidOperationException();
    
    public override string ToString() => "IndexSegment[" + a + "," + b + "]";
    public string ToString(List<Vector2> v) => "IndexSegment[" + v[a] + "," + v[b] + "]";
}


[Serializable]
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

// Axis aligned rectangle.
[Serializable]
public struct AARect
{
    public float top;
    public float bottom;
    public float left;
    public float right;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 a, bool strict = false) => strict ?
        left < a.x && a.x < right && bottom < a.y && a.y < top :
        left <= a.x && a.x <= right && bottom <= a.y && a.y <= top;
    
}


public static class GeoUtil
{
    public static List<Vector2> ToVecList(this List<Segment> segs)
    {
        var res = new List<Vector2>(segs.Count * 2);
        foreach(var s in segs) res.Add(s.from, s.to);
        return res; 
    }
}
