using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZigZag2D
{

    // Mount this to the main camera!
    public class ZigZag2D : MonoBehaviour
    {
        public int seed;
        [Range(1, 6)] public float pos;
        [Range(0, 12)] public int count;
        [Range(0, 1)] public float movePercentage;
        
        public Material lineMat;
        
        class Data
        {
            public float pos;
            public int seed;
            public int count;
            public float movePercentage;
            public readonly List<Vector2> verts = new List<Vector2>();
        }
        
        readonly Data data = new Data();
        
        
        // Start is called before the first frame update
        void Start()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
            if(pos != data.pos || seed != data.seed || count != data.count || movePercentage != data.movePercentage)
            {
                data.pos = pos;
                data.seed = seed;
                data.count = count;
                data.movePercentage = movePercentage;
                
                data.verts.Clear();
                data.verts.Add(new Vector2(-pos, -pos));
                data.verts.Add(new Vector2(pos, pos));
                
                Random.InitState(seed);
                for(int i=0; i<count; i++)
                {
                    Vector2 Rot(Vector2 v, float g)
                    {
                        return new Vector2(
                            v.x * Mathf.Cos(g) - v.y * Mathf.Sin(g),
                            v.x * Mathf.Sin(g) + v.y * Mathf.Cos(g) 
                        );
                    }
                    
                    var src = data.verts.ToArray();
                    
                    data.verts.Clear();
                    data.verts.Add(src[0]);
                    
                    for(int a = 0; a < src.Length - 1; a++)
                    {
                        int b = a + 1;
                        Vector2 s = src[a];
                        Vector2 t = src[b];
                        Vector2 mid = (s + t) * 0.5f;
                        Vector2 norm = Rot((t - s).normalized, Random.Range(0.0f, 90.0f) * Mathf.Deg2Rad);
                        
                        var (pa, pb) = (
                            mid - norm * movePercentage * (t - s).magnitude,
                            mid + norm * movePercentage * (t - s).magnitude
                        );
                        data.verts.Add(pa);
                        data.verts.Add(pb);
                        data.verts.Add(src[b]);
                    }
                }
            }
        }
        
        void OnPreRender()
        {
            
        }
        
        void OnPostRender()
        {
            GL.PushMatrix();
            
            lineMat.SetPass(0);
            GL.Begin(GL.LINE_STRIP);
            
            foreach(var i in data.verts) GL.Vertex(i);
            
            GL.End();
            
            GL.PopMatrix();
        }
    }


} // ZigZag2D
