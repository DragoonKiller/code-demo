using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MarchingCube
{
    public static class ChunkUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int DecodeChunkOffset(this string s)
        {
            var coordStr = s.Split('.');
            var (x, y) = (int.Parse(coordStr[0]), int.Parse(coordStr[1]));
            return new Vector2Int(x, y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EncodeChunkOffset(this Vector2Int c)
            => c.x + "." + c.y;
        
    }
    
    [CreateAssetMenu(fileName = "MeshConfig", menuName = "ScriptableObjects/MeshConfig", order = 1)]
    public class ChunkConfig : ScriptableObject
    {
        /// GameObject template for chunks.
        public GameObject chunkTemplate;
        
        /// Sample a built-in function to generate the terrain texture of a chunk.
        /// This texture will be sampled again to determine the mesh terrain.
        public ComputeShader terrainGen;
        
        /// Resolution, which defines the generated texture's size.
        public Vector2Int resolution;
        
        /// Chunk size in world space.
        public Vector2Int size;
        
        /// Frequency of underlaying terrain function.
        public Vector2 frequency;
        
        /// Roughness of surface.
        public float roughness;
        
        /// from these baseline caves will be shinked and canceled.
        public float caveShrinkY;
        public float caveEliminateY;
        
        /// Random seed of underlaying terrain function.
        public float seed;
        
        /// Amplitude of underlaying tarrain function.
        public Vector2 amplitude;
        
        // For how far we shall render a chunk.
        public float maxChunkDistance;
        
        // Foreach frame how many chunks will be updated at most.
        public int maxUpdateChunk;
        
        // Show debug info or not.
        public bool debugInfo;
        
        public Vector2 localScale => size / (Vector2)resolution;
        
        /// Count of pixels/texels/grids.
        public int gridCount => resolution.x * resolution.y;
        
        /// Count of points that build all grids.
        public int pointCount => (resolution.x + 1) * (resolution.y + 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 OffsetCenter(Vector2Int loc)
            => (new Vector2(loc.x, loc.y) + Vector2.one * 0.5f) * size;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 OffsetBottomLeft(Vector2Int loc)
            => (Vector2)loc * (Vector2)size;
        
        [ThreadStatic] static List<Vector2Int> reqChunksCache;
        public List<Vector2Int> ChunksNearby(Vector2 pos)
        {
            // position in chunk-grids coordinates.
            var loc = new Vector2Int((int)(pos.x / size.x), (int)(pos.y / size.y));
            
            Vector2Int limit = new Vector2Int(
                Mathf.CeilToInt(maxChunkDistance / size.x) + 1,
                Mathf.CeilToInt(maxChunkDistance / size.y) + 1
            );
            
            if(reqChunksCache == null) reqChunksCache = new List<Vector2Int>();
            reqChunksCache.Clear();
            for(int x = loc.x - limit.x; x <= loc.x + limit.x; x++)
                for(int y = loc.y - limit.y; y <= loc.y + limit.y; y++)
                {
                    // Is within the max distance.
                    if((OffsetCenter(new Vector2Int(x, y))).To(pos).magnitude <= maxChunkDistance)
                        reqChunksCache.Add(new Vector2Int(x, y));
                }
            
            return reqChunksCache;
        }
    }
}
