using UnityEngine;



public class DepthSampler : MonoBehaviour
{
    public Material depthSampler;
    
    public RenderTexture source;
    public RenderTexture destination;
    
    void Update()
    {
        Graphics.Blit(source, destination, depthSampler);
    }
    
}
