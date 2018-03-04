using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text;

[Serializable]
public class VolumnLight : MonoBehaviour
{
    [SerializeField] GameObject lightObject;
    
    [SerializeField] Material mat;
    [SerializeField] Material add;
    
    [SerializeField] Material blurHorizontal;
    [SerializeField] Material blurVertical;
    
    [SerializeField] bool useGaussianBlur;
    
    Camera c;
    
    RenderTexture blurAuxTex;
    RenderTexture blurAuxTexHorz;
    RenderTexture blurAuxTexVert;
    
    void Start()
    {
        c = this.gameObject.GetComponent<Camera>();
        c.depthTextureMode = DepthTextureMode.Depth;
        
        blurAuxTex = new RenderTexture(Screen.width, Screen.height, -2, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        blurAuxTexHorz = new RenderTexture(Screen.width, Screen.height, -2, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        blurAuxTexVert = new RenderTexture(Screen.width, Screen.height, -2, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
    }

    void Update()
    {
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        // Notice the aspect ratio is linear ratio of the screen size, not to do with angles.
        // This is horrible...
        Vector2 camField = new Vector2(
            Mathf.Atan(Mathf.Tan(0.5f * c.fieldOfView * Mathf.Deg2Rad) * c.aspect),
            0.5f * c.fieldOfView * Mathf.Deg2Rad);
        
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        
        mat.SetVector("_TexSize", screenSize);
        mat.SetMatrix("_CameraTrans", c.worldToCameraMatrix);
        mat.SetVector("_CameraField", new Vector4(camField.x, camField.y, c.nearClipPlane, c.farClipPlane));
        Vector3 lx = lightObject.transform.position;
        mat.SetVector("_Location", c.worldToCameraMatrix * new Vector4(lx.x, lx.y, lx.z, 1.0f));
        mat.SetVector("_PX", c.worldToCameraMatrix * new Vector4(1f, 0f, 0f, 0f));
        mat.SetVector("_PY", c.worldToCameraMatrix * new Vector4(0f, 1f, 0f, 0f));
        mat.SetVector("_PZ", c.worldToCameraMatrix * new Vector4(0f, 0f, 1f, 0f));
        mat.SetVector("_NX", c.worldToCameraMatrix * new Vector4(-1f, 0f, 0f, 0f));
        mat.SetVector("_NY", c.worldToCameraMatrix * new Vector4(0f, -1f, 0f, 0f));
        mat.SetVector("_NZ", c.worldToCameraMatrix * new Vector4(0f, 0f, -1f, 0f));
        
        if(useGaussianBlur)
        {
            Graphics.Blit(src, blurAuxTex, mat);
            blurHorizontal.SetVector("_ScreenSize", screenSize);
            Graphics.Blit(blurAuxTex, blurAuxTexHorz, blurHorizontal);
            blurVertical.SetVector("_ScreenSize", screenSize);
            Graphics.Blit(blurAuxTexHorz, blurAuxTexVert, blurVertical);
            add.SetTexture("_SrcTex", src);
            Graphics.Blit(blurAuxTexVert, dst, add);
        }
        else
        {
            Graphics.Blit(src, blurAuxTex, mat);
            add.SetTexture("_SrcTex", src);
            Graphics.Blit(blurAuxTex, dst, add);
        }
        
        // Graphics.Blit(src, dst);
    }
}
