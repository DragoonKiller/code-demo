using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class MetaballEffect : MonoBehaviour
{
    
    Camera cam => this.GetComponent<Camera>();
    public RenderTexture fluidTexture
    {
        get
        {
            if(cam.targetTexture && cam.targetTexture.texelSize != new Vector2(Screen.width, Screen.height))
            {
                cam.targetTexture = null;
            }

            if(cam.targetTexture == null)
            {
                cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);
                cam.targetTexture.Create();
            }

            return cam.targetTexture;
        }
    }

    void Update()
    {
        // Get the texture to reset the size.
        var _ = fluidTexture;
    }
}
