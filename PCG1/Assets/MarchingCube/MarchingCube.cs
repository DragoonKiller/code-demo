using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace MarchingCube
{
    public static class MarchingCube
    {
        // Marching cube.
        // Basic rule:
        // 0 : bottom-left
        // 1 : bottom-right
        // 2 : top-left
        // 3 : top-right
        
        static class MarchingCubeUtil
        {
            public static Action<List<Segment>, Vector2>[] MarchCube;
            
            public static void MarchingCubeInit()
            {
                MarchCube = new Action<List<Segment>, Vector2>[16];
                
                Vector2[] ownLeft = new Vector2[4] {
                    new Vector2(0   , 0.5f),
                    new Vector2(0.5f, 0   ),
                    new Vector2(0.5f, 1.0f),
                    new Vector2(1.0f, 0.5f),
                };
                Vector2[] ownRight = new Vector2[4] {
                    new Vector2(0.5f, 0   ),
                    new Vector2(1.0f, 0.5f),
                    new Vector2(0   , 0.5f),
                    new Vector2(0.5f, 1.0f),
                };
                
                // For the use of stack allocated array.
                unsafe
                {
                    int* opposite = stackalloc int[4] { 3, 2, 1, 0 };
                    int* left = stackalloc int[4]{ 2, 0, 3, 1 };
                    int* right = stackalloc int[4]{1, 3, 0, 2 };
                    
                    // All verteces covered.
                    // Do nothing...
                    MarchCube[15] = (v, x) => { };
                    
                    // All verteces empty.
                    // Do nothing...
                    MarchCube[0] = (v, x) => { };
                    
                    // Single point covered.
                    for(int t=0; t<4; t++)
                    {
                        int f = t;
                        int vid = 1 << f;
                        MarchCube[vid] = (v, x) => v.Add(new Segment(ownRight[f] + x, ownLeft[f] + x));
                    }
                    
                    // Adjacent point covered.
                    for(int t=0; t<4; t++)
                    {
                        int f = t;
                        int h = left[f];
                        int vid = (1 << f) | (1 << h);
                        MarchCube[vid] = (v, x) => v.Add(new Segment(ownRight[f] + x, ownLeft[h] + x));
                    }
                    
                    // Opposite point covered.
                    for(int t=0; t<4; t++)
                    {
                        int f = t;
                        int h = opposite[f];
                        int vid = (1 << f) | (1 << h);
                        MarchCube[vid] = (v, x) =>
                        {
                            v.Add(new Segment(ownLeft[f] + x, ownRight[h] + x));
                            v.Add(new Segment(ownLeft[h] + x, ownRight[f] + x));
                        };
                    }
                    
                    // Single point empty.
                    for(int t=0; t<4; t++)
                    {
                        int f = t;
                        int h = left[f];
                        int g = right[f];
                        int vid = (1 << f) | (1 << h) | (1 << g);
                        MarchCube[vid] = (v, x) => v.Add(new Segment(ownRight[g] + x, ownLeft[h] + x));
                    }
                
                }
            }
        }
        
        /// val using bit masks to store the grid info.
        /// offset: position relative to chunk origin.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Get(int val, Vector2 offset, List<Segment> verts)
        {
            // Initialize maintaince function.
            if(MarchingCubeUtil.MarchCube == null) MarchingCubeUtil.MarchingCubeInit();
            
            MarchingCubeUtil.MarchCube[val](verts, offset);
        }
    }
}
