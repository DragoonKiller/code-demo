using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Mathf;

namespace MarchingCube
{
    /// Chunk self-behaviour.
    [ExecuteAlways]
    public class Chunk : MonoBehaviour
    {
        List<Segment> segs;
        public ChunkConfig config;
        
        // Take configurations and setup all components of this GameObject.
        public GameObject Setup(ChunkConfig config, Vector2Int loc)
        {
            this.gameObject.name = loc.EncodeChunkOffset();
            
            // Setup verts and indices.
            SetupOutline(config, loc);
            
            // Prepare component builds.
            var verts = new List<Vector3>();
            foreach(var s in segs) { verts.Add(s.from); verts.Add(s.to); }
            
            // Build up mesh.
            // This mesh is used for rendering outlines.
            var ms = gameObject.GetComponent<MeshFilter>();
            var indices = new List<int>();
            indices.AddIntRange(0, verts.Count - 1);
            ms.mesh = new Mesh().SubmitData(verts, indices, MeshTopology.Lines);
            
            // Build up colliders.
            var lks = segs.LinkUp();
            var col = new List<EdgeCollider2D>(GetComponentsInChildren<EdgeCollider2D>());
            for(int i=0; i<lks.Count; i++)
            {
                if(col.Count <= i) col.Add(this.gameObject.AddComponent<EdgeCollider2D>());
                var cx = col[i];
                cx.points = lks[i].ToArray();
                cx.offset = Vector2.zero;
            }
            
            transform.position = config.OffsetBottomLeft(loc);
            this.config = config;
            
            return this.gameObject;
        }
        
        void Start()
        {
            
        }
        
        void Update()
        {
            ShowGroundDirection();
        }
        
        void ShowGroundDirection()
        {
            if(segs == null) return;
            if(config == null) return;
            
            var mr = GetComponent<MeshRenderer>();
            if(mr) mr.enabled = config.debugInfo;
        }
        
        [ThreadStatic] static float[] dataBuf;
        void SetupOutline(ChunkConfig chunk, Vector2Int loc)
        {
            var terrainGen = chunk.terrainGen;
            using(ComputeBuffer outBuf = new ComputeBuffer(chunk.gridCount, sizeof(float)))
            {
                // Setup compute shader.
                int k = terrainGen.FindKernel("Main");
                terrainGen.SetFloat("pi", PI);
                terrainGen.SetFloat("rdSeed", chunk.seed);
                terrainGen.SetFloats("resolution", chunk.resolution.x, chunk.resolution.y);
                terrainGen.SetFloat("caveShrinkY", chunk.caveShrinkY);
                terrainGen.SetFloat("caveEliminateY", chunk.caveEliminateY);
                terrainGen.SetFloats("roughness", chunk.roughness);
                terrainGen.SetFloats("localScale", chunk.localScale.x, chunk.localScale.y);
                terrainGen.SetFloats("freq", chunk.frequency.x, chunk.frequency.y);
                terrainGen.SetFloats("offset", chunk.OffsetBottomLeft(loc).x, chunk.OffsetBottomLeft(loc).y);
                terrainGen.SetFloats("amplitude", chunk.amplitude.x, chunk.amplitude.y);
                terrainGen.SetBuffer(k, "output", outBuf);
                
                // Draw chunk terrain.
                terrainGen.Dispatch(k, CeilToInt(chunk.resolution.x / 16.0f), CeilToInt(chunk.resolution.y / 16.0f), 1);
                
                // check the databuf and its size.
                if(dataBuf == null || dataBuf.Length != chunk.gridCount) dataBuf = new float[chunk.gridCount];
                
                // get data from GPU back into main memory.
                outBuf.GetData(dataBuf);
            }
            
            // Check chunk points.
            // Mesh vertices should be in *local* space that relative to the bottom-left corner.
            if(segs == null) segs = new List<Segment>();
            segs.Clear();
            
            for(int x=0; x<chunk.resolution.x; x++) for(int y=0; y<chunk.resolution.y; y++)
            {
                int val = RoundToInt(dataBuf[x + y * chunk.resolution.x]) ^ 0xF;
                MarchingCube.Get(val, new Vector2(x, y), segs);
            }
            
            for(int i=0; i<segs.Count; i++)
            {
                segs[i] = new Segment(segs[i].from * chunk.localScale, segs[i].to * chunk.localScale);
            }
        }
        
    }

}
