using System;
using UnityEngine;

namespace MarchingCube
{
    [ExecuteAlways]
    public class BackgroundRenderer : MonoBehaviour
    {
        public Material backgroundMat;
        public ChunkConfig chunk;
        public GameObject cameraAttached;
        
        public Color empty;
        public Color filled;
        
        void Update()
        {
            backgroundMat.SetFloat("pi", Mathf.PI);
            backgroundMat.SetFloat("rdSeed", chunk.seed);
            backgroundMat.SetVector("resolution", (Vector2)chunk.resolution);
            backgroundMat.SetFloat("caveShrinkY", chunk.caveShrinkY);
            backgroundMat.SetFloat("caveEliminateY", chunk.caveEliminateY);
            backgroundMat.SetFloat("roughness", chunk.roughness);
            backgroundMat.SetVector("localScale", chunk.localScale);
            backgroundMat.SetVector("freq", chunk.frequency);
            backgroundMat.SetVector("amplitude", chunk.amplitude);
            backgroundMat.SetColor("emptyColor", empty);
            backgroundMat.SetColor("filledColor", filled);
            
            if(cameraAttached != null)
                this.gameObject.transform.position = new Vector3(
                    cameraAttached.transform.position.x,
                    cameraAttached.transform.position.y,
                    0
                );
                
        }
        
    }
    
    
}
