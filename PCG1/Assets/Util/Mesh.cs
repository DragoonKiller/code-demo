using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util
{
    public static Mesh SubmitData(
        this Mesh mesh,
        List<Vector3> verts,
        List<int> indexes,
        MeshTopology topology = MeshTopology.Triangles)
    {
        mesh.SetVertices(verts);
        mesh.SetIndices(indexes.ToArray(), topology, 0);
        if(topology != MeshTopology.Points) mesh.RecalculateBounds();
        return mesh;
    }
    
    public static Mesh SubmitData(
        this Mesh mesh,
        List<Vector2> vertsSrc,
        List<int> indexes,
        MeshTopology topology = MeshTopology.Triangles)
    {
        var verts = new List<Vector3>();
        for(int i=0; i<vertsSrc.Count; i++) verts.Add(vertsSrc[i]);
        return mesh.SubmitData(verts, indexes, topology);
    }
}
