using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[Serializable]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TrailFade : MonoBehaviour
{
    public float timePeriod;
    public float timeScale;
    
    
    MeshFilter meshFilter => this.GetComponent<MeshFilter>();
    Mesh mesh => meshFilter.mesh;
    RectTransform tr => this.GetComponent<RectTransform>();
    MeshRenderer meshRenderer => this.GetComponent<MeshRenderer>();
    Material mat => meshRenderer.material;
    
    readonly List<Vector3> verts = new List<Vector3>();
    readonly List<int> indices = new List<int>();
    readonly List<Vector2> uvs = new List<Vector2>();
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
        SyncMesh();
        SyncMat();
    }
    
    
    void SyncMesh()
    {
        mesh.Clear();
        var lb = tr.rect.min;
        var rt = tr.rect.max;
        var lt = new Vector2(lb.x, rt.y);
        var rb = new Vector2(rt.x, lb.y);
        verts.Clear();
        indices.Clear();
        uvs.Clear();
        verts.Add(lb, lt, rb, rt);
        indices.Add(0, 1, 2, 1, 2, 3);
        uvs.Add(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1));
        mesh.SetVertices(verts);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
    }
    
    void SyncMat()
    {
        var aspect = tr.rect.width / tr.rect.height;
        mat.SetFloat("_Aspect", aspect);
        mat.SetFloat("_Rate", (Time.time % timePeriod) * timeScale);
    }
    
    void OnDrawGizmos()
    {
        var lb = tr.rect.min;
        var rt = tr.rect.max;
        var lt = new Vector2(lb.x, rt.y);
        var rb = new Vector2(rt.x, lb.y);
        var curPos = tr.transform.position.ToVec2();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(curPos + lb, curPos + lt);
        Gizmos.DrawLine(curPos + rb, curPos + rt);
        Gizmos.DrawLine(curPos + lb, curPos + rb);
        Gizmos.DrawLine(curPos + lt, curPos + rt);
    }
}
