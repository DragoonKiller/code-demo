using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Utils;
namespace Anim
{

[ExecuteAlways]
[RequireComponent(typeof(AnimEffectorReceiver))]
public class AnimMeshControlRect : MonoBehaviour
{
    public bool updateEveryFrame;
    
    public Rect rect;
    public Vector2Int split;
    
    
    public float windPhase;
    public float windStrength;
    public float windFrequency;
    
    public Vector2 maxOffset;
    public Transform skeletonBase;
    public Transform skeleton;
    
    Vector2Int cacheSplit;
    
    readonly List<Vector3> verts = new List<Vector3>();
    readonly List<Vector3> offsets = new List<Vector3>();
    readonly List<int> trIndices = new List<int>();
    readonly List<int> lineIndices = new List<int>();
    readonly List<Vector2> uvs = new List<Vector2>();
    readonly List<Vector3> tempVerts = new List<Vector3>();
    
    int pointCnt => (split.x + 1) * (split.y + 1);
    Vector2 size => rect.size / split;
    Vector2 uvSize => Vector2.one / split;
    Vector2 baseOffset => rect.min;
    
    Mesh mesh
    {
        get
        {
            var meshFilter = this.GetComponent<MeshFilter>();
            if(meshFilter == null) throw new System.Exception();
            if(Application.isPlaying) return meshFilter.mesh;
            
            var mesh = meshFilter.sharedMesh;
            if(mesh == null)
            {
                mesh = meshFilter.mesh = new Mesh();
                mesh.name = $"Generated mesh {mesh.GetInstanceID()}";
            }
            return mesh;
        }
    }
    
    void Srart()
    {
        Setup();
    }
    
    void Update()
    {
        if(updateEveryFrame || TestMeshChanged()) Setup();
        if(verts.Count != pointCnt) return;
        SkeletonBlend();
        SyncOffset();
        SyncMesh();
        DebugDraw();
    }
    
    void SkeletonBlend()
    {
        // 横向的, 随时间变化的风.
        var biasTime = Time.time + windPhase;
        var windForce = Vector2.right * (biasTime * windFrequency).Sin() * windStrength;
        
        // 各个 effector 造成的影响.
        var effectorForce = Vector2.zero;
        var effectorReceiver = this.GetComponent<AnimEffectorReceiver>();
        foreach(var e in effectorReceiver.effectors) effectorForce += e.GetOffset(skeletonBase.position);
        
        // 算出实际的骨骼节点位移.
        var sumForce = effectorForce + windForce;
        var sumOffset = maxOffset * sumForce.normalized * (1 - Mathf.Exp(-sumForce.magnitude));
        
        // 应用位移.
        skeleton.transform.position = skeletonBase.transform.position + (Vector3)sumOffset;
    }
    
    void SyncOffset()
    {
        offsets.Resize(verts.Count);
        
        var sk = skeleton.transform.position.ToVec2();
        var sbase = skeletonBase.transform.position.ToVec2();
        var delta = sk - sbase;
        for(int j = 0; j <= split.y; j++) for(int i = 0; i <= split.x; i++)
        {
            var weight = new Vector2(
                ((float)j / split.y).Pow(1.5f),
                ((float)j / split.y).Pow(1.5f)
            );
            offsets[PointId(i, j)] = weight * delta;
        }
    }
    
    bool TestMeshChanged()
    {
        bool changed = false;
        if(cacheSplit != split) changed = true;
        cacheSplit = split;
        return changed;
    }
    
    void DebugDraw()
    {
        if(skeleton != null)
        {
            Debug.DrawLine(this.transform.position, this.skeletonBase.position, Color.red);
            Debug.DrawLine(this.skeletonBase.position, this.skeleton.position, Color.gray);
            Debug.DrawLine(this.transform.position, this.skeleton.position, Color.green);
        }
    }
    
    void Setup()
    {
        verts.Clear();
        trIndices.Clear();
        lineIndices.Clear();
        uvs.Clear();
        
        // 横纵分成 split.x * split.y 个矩形.
        for(int j = 0; j <= split.y; j++) for(int i = 0; i <= split.x; i++)
        {
            verts.Add(OriginalOffset(i, j));
            uvs.Add(OriginalUVOffset(i, j));
        }
        
        for(int j = 0; j < split.y; j++) for(int i = 0; i < split.x; i++)
        {
            int baseId = PointId(i, j);
            int topId = PointId(i, j + 1);
            int rightId = PointId(i + 1, j);
            int topRightId = PointId(i + 1, j + 1);
            // 三角形形式提交.
            trIndices.Add(baseId, topRightId, topId);
            trIndices.Add(baseId, rightId, topRightId);
            // 线框形式提交.
            lineIndices.Add(baseId, topId);
            lineIndices.Add(baseId, rightId);
            lineIndices.Add(topRightId, topId);
            lineIndices.Add(topRightId, rightId);
        }
    }
    
    void SyncMesh()
    {
        mesh.Clear();
        mesh.SetVertices(verts.Zip(offsets, (a, b) => a + b).ToList());
        mesh.SetUVs(0, uvs);
        mesh.subMeshCount = 2;
        mesh.SetIndices(trIndices.ToArray(), MeshTopology.Triangles, 0);
        mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 1);
        mesh.RecalculateBounds();
    }
    
    void SetPerlinOffset()
    {
        for(int j = 0; j <= split.y; j++) for(int i = 0; i <= split.x; i++)
        {
            int id = PointId(i, j);
            Vector2 samplePosA = new Vector2(0.133f * i, 0.166f * j + Time.time);
            Vector2 samplePosB = new Vector2(0.133f * i + 133.71765f, 0.166f * j + Time.time + 773.68424f);
            var bias = new Vector2(Mathf.PerlinNoise(samplePosA.x, samplePosA.y), Mathf.PerlinNoise(samplePosB.x, samplePosB.y));
            bias = bias * 2 - Vector2.one;
            bias *= 0.1f * (float)j / split.y;
            offsets[id] = bias;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Vector2 OriginalOffset(int x, int y) => baseOffset + new Vector2(size.x * x, size.y * y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Vector2 OriginalUVOffset(int x, int y) => new Vector2(uvSize.x * x, uvSize.y * y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int PointId(int x, int y) => y * (split.x + 1) + x;
}

}
