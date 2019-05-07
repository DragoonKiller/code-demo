using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

public static partial class Util
{
    /// For the use of Vector2 and Vector2Int
    struct Edge : IEquatable<Edge>
    {
        public Vector2 a, b;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Edge(Vector2 a, Vector2 b) { this.a = a; this.b = b; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => this.Equals((Edge)obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Edge y) => this.a.Equals(y.a) && this.b.Equals(y.b) || this.b.Equals(y.a) && this.a.Equals(y.b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(Edge x, Edge y) => x.a.Equals(y.a) && x.b.Equals(y.b) || x.b.Equals(y.a) && x.a.Equals(y.b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(Edge x, Edge y) => x.Equals(y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator!=(Edge x, Edge y) => !x.Equals(y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            int ax = Convert.ToInt32(a.x);
            int ay = Convert.ToInt32(a.y);
            int bx = Convert.ToInt32(b.x);
            int by = Convert.ToInt32(b.y);
            int ha = ax + ay + (ax * ay);
            int hb = bx + by + (bx * by);
            return (ha + hb) - (ha ^ hb);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => String.Format("Edge({0}, {1})", a, b);
    }
    
    /// Input format: each two vectors represents a segment from the beginning.
    /// Considering all triangles are filled and non-triangle structure is always empty.
    /// Extract the outlines (may have holes) and return the outline.
    /// Points in each edge is arranged where their left-side is filled.
    /// Notice edges are not in order, but two points in each edge has its order.
    public static List<Vector2> ExtractEdge(this List<Vector2> src)
    {
        var adj = new Dictionary<Vector2, List<Vector2>>();
        for(int i=0; i<src.Count; i += 2)
        {
            adj.GetOrDefault(src[i]).Add(src[i + 1]);
            adj.GetOrDefault(src[i + 1]).Add(src[i]);
        }
        
        // Sort the adjacent edges.
        foreach(var x in adj)
        {
            var curVert = x.Key;
            var adjList = x.Value;
            int Compare(Vector2 va, Vector2 vb)
            {
                Vector2 da = curVert.To(va);
                Vector2 db = curVert.To(vb);
                float aa = Mathf.Atan2(da.y, da.x);
                float ba = Mathf.Atan2(db.y, db.x);
                return aa < ba ? -1 : aa > ba ? 1 : 0;
            }
            adjList.Sort(Compare);
        }
        
        // output size should not exceeded input size.
        var rest = new Util.Set<Edge>(src.Count);
        
        foreach(var vert in src.Distinct().ToList())
        {
            var adx = adj[vert];
            for(int i=0; i<adx.Count; i++)
            {
                var from = adx[i];
                var to = adx[ModSys(i + 1, adx.Count)];
                
                // Exclude the edge if triangle edges are arranged clockwise.
                if(new Triangle(vert, from, to).area <= 0) continue;
                
                // Edges can either appear for 1 or 2 times.
                // Because an edge can only be owned by 1 or 2 triangles.
                // Use this to extract outlines, including outlines inside.
                var edge = new Edge(from, to);
                
                // take up about 200ms time when src.Length == 60000.
                if(rest.Contains(edge))
                {
                    rest.Remove(edge);
                }
                else
                {
                    rest.Add(edge);
                }
            }
        }
        
        var res = new List<Vector2>();
        rest.Foreach((i) => { res.Add(i.a); res.Add(i.b); });
        return res;
    }
    
    
    public static void EdgeExtractTest()
    {
        for(int t=0; t<4; t++)
        {
            var lst = new List<Vector2>{
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 1),
                new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(0, 1), new Vector2(1, 1),
            };
            
            if(t != 0)
            {
                // Random shuffle
                for(int i=0; i<8; i++)
                {
                    int a = UnityEngine.Random.Range(0, lst.Count / 2) * 2;
                    int b = UnityEngine.Random.Range(0, lst.Count / 2) * 2;
                    (lst[a], lst[b]) = (lst[b], lst[a]);
                    (lst[a + 1], lst[b + 1]) = (lst[b + 1], lst[a + 1]);
                }
            }
            
            var res = ExtractEdge(lst);
            Debug.Assert(res.Count == 8);
            List<Edge> valid = new List<Edge>{
                new Edge(new Vector2(0, 0), new Vector2(1, 0)),
                new Edge(new Vector2(1, 0), new Vector2(1, 1)),
                new Edge(new Vector2(1, 1), new Vector2(0, 1)),
                new Edge(new Vector2(0, 1), new Vector2(0, 0)),
            };
            
            for(int i=0; i<res.Count; i += 2)
            {
                var e = new Edge(res[i], res[i + 1]);
                Debug.Assert(valid.Contains(e), "result points error");
                var found = valid.Find((x) => x == e);
                
                // The order of a and b means which side of the edge is filled.
                // Although it does not considered in Edge.
                Debug.Assert(e.a == found.a && e.b == found.b, "result edge direction error");
            }
        }
        
        // Performance test.
        {
            const int n = 100; // (n + 1) * (n + 1) point grids.
            
            var lst = new List<Vector2>();
            void AddSegment(float x, float y, float dx, float dy)
            {
                lst.Add(new Vector2(x, y));
                lst.Add(new Vector2(x + dx, y + dy));
            }
            
            for(int i=0; i<n; i++) for(int j=0; j<n; j++)
            {
                AddSegment(i, j, 1, 0); // rightward.
                AddSegment(i, j, 0, 1); // upward.
                AddSegment(i, j, 1, 1); // diagonal.
            }
            // horizontal top fix.
            for(int i=0; i<n; i++) AddSegment(i, n, 1, 0);
            // vertical top fix.
            for(int j=0; j<n; j++) AddSegment(n, j, 0, 1);
            
            Debug.Assert(lst.Count == 2 * (n * n * 3 + 2 * n), "Wrong answer on points count of large data test.");
            
            var rec = DateTime.Now;
            
            var ans = ExtractEdge(lst);
            
            var res = (DateTime.Now - rec).TotalMilliseconds;
            
            float expcetedTime = 20 * (3 * n * n + 2 * n) / 3e4f;
            if(res > expcetedTime)
                Debug.LogWarning(
                    "Test performance for " + res + "ms, too slow! "
                    + expcetedTime.ToString("0.0") + "ms is expected."
                );
            
            var dst = new Dictionary<Edge, Edge>();
            for(int i=0; i<n; i++)
            {
                var a = new Edge(new Vector2(i, 0), new Vector2(i + 1, 0));
                dst.Add(a, a);
                var b = new Edge(new Vector2(i + 1, n), new Vector2(i, n));
                dst.Add(b, b);
            }
            for(int j=0; j<n; j++)
            {
                var a = new Edge(new Vector2(0, j + 1), new Vector2(0, j));
                dst.Add(a, a);
                var b = new Edge(new Vector2(n, j), new Vector2(n, j + 1));
                dst.Add(b, b);
            }
            
            for(int i=0; i<ans.Count; i += 2)
            {
                var a = ans[i];
                var b = ans[i + 1];
                var e = new Edge(a, b);
                if(dst.TryGetValue(e, out var t))
                {
                    if(e.a == t.a && e.b == t.b)
                    {
                        // test passed, do nothing...
                    }
                    else
                    {
                        Debug.LogAssertion("Wrong direction for large data test!");
                        break;
                    }
                }
                else
                {
                    Debug.LogAssertion("wrong answer for large data test!");
                    break;
                }
            }
        }
        
        GC.Collect();
    }
}
