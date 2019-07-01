using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util
{
    /// Convert polygon edges into triangles.
    /// The last point is not necessarily be the first.
    public static List<Triangle> Triangulation(this List<Vector2> a)
    {
        if(a.Count < 3) return new List<Triangle>();
        if(a[a.Count - 1] == a[0]) a.RemoveAt(a.Count - 1);
        if(a.Count < 3) return new List<Triangle>();
        
        var res = new List<Triangle>();
        
        // Ear cut algorithm.
        // O(n^2) time.
        
        (Vector2 lv, Vector2 cv, Vector2 rv) GetTriangle(int id)
        {
            Vector2 lv = a[(id - 1).ModSys(a.Count)];
            Vector2 cv = a[id];
            Vector2 rv = a[(id + 1).ModSys(a.Count)];
            return (lv, cv, rv);
        }
        
        // return 1 if it is counter-clockwise.
        // return -1 if it is clockwise.
        // return 0 if it is degenerated polygon.
        int CurveSign()
        {
            float area = 0;
            Vector2 g = a[0];
            for(int i = 2; i < a.Count; i++) area += g.To(a[i-1]).Cross(g.To(a[i]));
            return area.Sgn();
        }
        
        int curveSign = CurveSign();
        bool IsEar(int id)
        {
            var (lv, cv, rv) = GetTriangle(id);
            var tr = new Triangle(lv, cv, rv);
            
            // Color dc = tr.sign > 0 ? Color.red : Color.yellow;
            // Debug.DrawLine(tr.a, tr.b, dc);
            // Debug.DrawLine(tr.b, tr.c, dc);
            // Debug.DrawLine(tr.c, tr.a, dc);
            
            // An ear is not sagging;
            if(tr.sign != curveSign) return false;
            
            // An ear will not contains any points.
            for(int i = 2; i < a.Count - 1; i++)
            {
                if(tr.Contains(a[(id + i).ModSys(a.Count)], false)) return false; 
            }
            return true;
        }
        
        int triangleCount = a.Count - 2;
        for(int t=0; t<triangleCount; t++)
        {
            for(int i=0; i<a.Count; i++)
            {
                var (lv, cv, rv) = GetTriangle(i);
                if(IsEar(i))
                {
                    res.Add(new Triangle(lv, cv, rv));
                    a.RemoveAt(i);
                    break;
                }
            }
        }
        
        return res;
    }
    
    public static void TriangulationTest(PolygonCollider2D cd)
    {
        var vts = new List<Vector2>(cd.GetPath(0));
        var tri = vts.Triangulation();
        var cls = new Color[]{ Color.red, Color.blue, Color.yellow };
        int ci = 0;
        foreach(var i in tri)
        {
            ci ++;
            ci %= 3;
        }
    }
    
}
