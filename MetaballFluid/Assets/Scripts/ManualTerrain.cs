using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteAlways]
public class ManualTerrain : MonoBehaviour
{
    PolygonCollider2D cd => this.GetComponent<PolygonCollider2D>();
    MeshFilter ms => this.GetComponent<MeshFilter>();

    public bool refreshEveryFrame;
    public bool requireUpdate;

    void Start()
    {
    }

    void Update()
    {

#if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying && refreshEveryFrame) requireUpdate = true;
#endif // UNITY_EDITOR

        if(requireUpdate)
        {
            requireUpdate = false;
            UpdateMesh();
        }
    }

    void UpdateMesh()
    {
        var trs = new List<Vector2>(cd.GetPath(0)).Triangulation();
        var vts = new List<Vector3>(trs.Count * 3);
        var ids = new List<int>(trs.Count * 3);
        for(int i = 0; i < trs.Count; i++)
        {
            vts.Add(trs[i].a, trs[i].b, trs[i].c);
            ids.Add(i * 3, i * 3 + 1, i * 3 + 2);
        }
        var mesh = new Mesh();
        mesh.SetVertices(vts);
        mesh.SetIndices(ids.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.name = "Generated tarrain mesh";
        ms.mesh = mesh;
    }

}
