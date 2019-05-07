using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TexToMesh
{
    
    public class TexRender : MonoBehaviour
    {
        public ComputeShader texGen;
        
        
        public Texture2D tex;
        public SpriteRenderer displayer;
        public RenderTexture rdtex;
        
        public Action AfterRefresh = () => {};
        
        public Vector2Int size;
        public float seed;
        public Vector2 freq;
        
        struct Data
        {
            public Vector2Int size;
            public float seed;
            public Vector2 freq;
        }
        
        Data data;
        
        void Start()
        {
            
        }
        
        void Update()
        {
            if(data.size != size || data.seed != seed || data.freq != freq)
            {
                data.size = size;
                data.seed = seed;
                data.freq = freq;
                Refresh();
            }
        }
        
        void Refresh()
        {
            rdtex = new RenderTexture(data.size.x, data.size.y, 0);
            rdtex.enableRandomWrite = true;
            rdtex.Create();
            
            int k = texGen.FindKernel("Main");
            texGen.SetFloat("pi", Mathf.PI);
            texGen.SetFloat("rdSeed", data.seed);
            texGen.SetFloats("rdSize", data.size.x, data.size.y);
            texGen.SetFloats("grSize", data.freq.x, data.freq.y);
            texGen.SetTexture(k, "Output", rdtex);
            texGen.Dispatch(k, data.size.x / 16, data.size.y / 16, 1);
            
            var tmp = RenderTexture.active;
            RenderTexture.active = rdtex;
            
            if(tex == null) tex = new Texture2D(data.size.x, data.size.y, TextureFormat.RGBA32, false);
            tex.name = "$tex";
            tex.filterMode = FilterMode.Point;
            tex.ReadPixels(new Rect(0, 0, data.size.x, data.size.y), 0, 0);
            tex.Apply();
            if(displayer != null)
            {
                var spr = Sprite.Create(
                    tex,
                    new Rect(0, 0, data.size.x, data.size.y),
                    Vector2.one * 0.5f
                );
                spr.name = "#tex";
                displayer.sprite = spr;
            }
            RenderTexture.active = tmp;
            
            AfterRefresh();
        }
        
        
    }
    
    
} // namespace TexToMesh
