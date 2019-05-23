using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class  Util
{
    static void AddTo(this Segment x, List<Vector3> verts, List<int> indices)
    {
        verts.Add(x.from);
        verts.Add(x.to);
        indices.Add(verts.Count - 2);
        indices.Add(verts.Count - 1);
    }
    
    public static void AddMeshes(this List<Mesh> meshes, List<Segment> segments, int maxVertexCount = 4096)
    {
        const int vertPerSegment = 2;
        var verts = new List<Vector3>();
        var indices = new List<int>();
        foreach(var i in segments)
        {
            if(verts.Count + vertPerSegment <= maxVertexCount)
            {
                var mesh = new Mesh();
                mesh.SetVertices(verts);
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
                meshes.Add(mesh);
                verts.Clear();
                indices.Clear();
                mesh.RecalculateBounds();
            }
            
            i.AddTo(verts, indices);
        }
    }
    
    public static void AddSegment(this Mesh mesh, Segment segment)
    {
        var verts = new List<Vector3>(mesh.vertices);
        var indices = new List<int>(mesh.GetIndices(0));
        var color = new List<Color>(mesh.colors);
        segment.AddTo(verts, indices);
        var cc = new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), 1f);
        color.Add(cc); color.Add(cc);
        mesh.SetVertices(verts);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.SetColors(color);
        mesh.RecalculateBounds();
    }
    
    public static void AddSegment(this Mesh mesh, List<Segment> segments)
    {
        var verts = new List<Vector3>(mesh.vertices);
        var indices = new List<int>(mesh.GetIndices(0));
        foreach(var i in segments) i.AddTo(verts, indices);
        mesh.SetVertices(verts);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
    }
}
