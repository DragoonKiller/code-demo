using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utils;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlowDirControl : MonoBehaviour
{
    public List<Vector2> points = new List<Vector2>();
    public float width;
    
    readonly List<Vector3> verts = new List<Vector3>();
    readonly List<Vector2> uvs = new List<Vector2>();
    readonly List<int> indices = new List<int>();
    
    
    
    void Update()
    {
        SyncMesh();
    }
    
    void SyncMesh()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        verts.Clear();
        uvs.Clear();
        indices.Clear();
        
        float sumLen = 0;
        for(int i = 1; i < points.Count; i++) sumLen += points[i].To(points[i - 1]).magnitude;
        
        float passedLen = 0;
        for(int i = 0; i < points.Count; i++)
        {
            float rate = passedLen / sumLen;
            var (a, b) = GetExtendPoints(i);
            verts.Add(a, b);
            uvs.Add(new Vector2(0, rate), new Vector2(1, rate));
            if(i != points.Count - 1) passedLen += points[i].To(points[i + 1]).magnitude;
        }
        
        for(int i = 1; i < points.Count; i++)
        {
            int lb = i * 2 - 2;
            int rb = i * 2 - 1;
            int lt = i * 2;
            int rt = i * 2 + 1;
            indices.Add(lb, lt, rt);
            indices.Add(lb, rb, rt);
        }
        
        mesh.SetVertices(verts);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
    }
    
    (Vector2 l, Vector2 r) GetExtendPoints(int i)
    {
        Vector2 cur = points[i], prev, next;
        if(i == 0)
        {
            next = points[i + 1];
            prev = 2 * cur - next;
        }
        else if(i == points.Count - 1)
        {
            prev = points[i - 1];
            next = 2 * cur - prev;
        }
        else
        {
            prev = points[i - 1];
            next = points[i + 1];
        }
        
        var angle = Mathf.Deg2Rad * 0.5f * Vector2.SignedAngle(cur.To(prev), cur.To(next));
        var xdir = cur.To(prev).normalized.Rot(angle);
        if(prev.To(cur).Cross(xdir) <= 0) xdir = -xdir;
        Debug.DrawLine(this.transform.position.ToVec2() + cur, this.transform.position.ToVec2() + cur + xdir);
        return (cur + width * xdir, cur - width * xdir);
    }
    
    void OnDrawGizmos()
    {
        var pos = this.transform.position.ToVec2();
        Gizmos.color = Color.red;
        for(int i = 1; i < points.Count; i++) Gizmos.DrawLine(pos + points[i-1], pos + points[i]);
        Gizmos.color = Color.yellow;
        for(int i = 0; i < points.Count; i++)
        {
            var (a, b) = GetExtendPoints(i);
            Gizmos.DrawLine(pos + a, pos + b);
        }
    }
    
}
