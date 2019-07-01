using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera fluidCamera;
    public RenderTexture tex => fluidCamera.targetTexture;
    public Material clearMat;
    public Material copyMat;
    public Material spriteMat;
    public Material fluidRenderMat;

    public int dynamicBlendCount;


    RenderTexture sceneTexture;
    CircArray<RenderTexture> fluidTexture;
    RenderTexture metaballFluidTexture;

    int curTime = 0;
    void OnPostRender()
    {
        if(fluidTexture != null && dynamicBlendCount != fluidTexture.Length) fluidTexture = null;
        if(fluidTexture == null)
        {
            fluidTexture = new CircArray<RenderTexture>(dynamicBlendCount);
            for(int i = 0; i < dynamicBlendCount; i++)
            {
                Util.SetupTexture(ref fluidTexture[i]);
                fluidTexture[i].Create();
                Graphics.Blit(null, fluidTexture[i], clearMat);
            }
        }

        // The fluid things...
        {
            // The camera that only render fluids...
            fluidCamera.clearFlags = CameraClearFlags.Nothing;
            fluidCamera.backgroundColor = new Color(0f, 1f, 0f, 0f);

            fluidCamera.targetTexture = fluidTexture[curTime];
            
            // And render...
            Graphics.Blit(null, fluidTexture[curTime], clearMat);
            fluidCamera.Render();

            // The dynamic blending magic...
            Util.SetupTexture(ref metaballFluidTexture);
            for(int i = 0; i < fluidTexture.Length; i++)
            {
                float alpha = 1.0f - (float)i / fluidTexture.Length;
                copyMat.SetFloat("_Alpha", alpha);
                Graphics.Blit(fluidTexture[curTime - i], metaballFluidTexture, copyMat);
            }
        }

        // The scene things...
        {
            sceneCamera.clearFlags = CameraClearFlags.Nothing;
            sceneCamera.backgroundColor = new Color(0.2f, 0.2f, 0.8f, 0f);

            Util.SetupTexture(ref sceneTexture);

            sceneCamera.targetTexture = sceneTexture;
            Graphics.Blit(null, sceneCamera.targetTexture, clearMat);
            sceneCamera.Render();
        }

        // Render all to the main frame.
        Graphics.Blit(null, null, clearMat);
        Graphics.Blit(sceneCamera.targetTexture, null, spriteMat);

        // The metaball magic...
        Graphics.Blit(metaballFluidTexture, null, fluidRenderMat);

        curTime += 1;
    }

}
