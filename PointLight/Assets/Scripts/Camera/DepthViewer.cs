 using UnityEngine;
 
 public class DepthViewer : MonoBehaviour
 {
    [SerializeField] Material mat;
    
    void Start()
    {
        this.gameObject.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, mat);
    }
     
 }
