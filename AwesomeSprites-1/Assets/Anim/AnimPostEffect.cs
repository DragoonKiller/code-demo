using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace Anim
{

public class AnimPostEffect : MonoBehaviour
{
    public Material drawCircleMat;
    public Color inColor;
    public Color outColor;
    
    
    public void OnPostRender()
    {
        foreach(var i in Component.FindObjectsOfType<AnimEffectorReceiver>())
        {
            foreach(var e in i.effectors)
            {
                float waveLength = 1.0f / e.frequency;
                float radiusBegin = e.time * e.speed - waveLength;
                float radiusEnd = e.time * e.speed;
                
                drawCircleMat.SetVector("_DrawRect", new Vector4(
                    Camera.main.ScreenToWorldPoint(Vector2.zero).x,
                    Camera.main.ScreenToWorldPoint(Vector2.zero).y,
                    Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x,
                    Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y
                ));
                
                drawCircleMat.SetColor("_OutColor", outColor);
                drawCircleMat.SetColor("_InColor", inColor);
                drawCircleMat.SetFloat("_InRadius",radiusBegin);
                drawCircleMat.SetFloat("_OutRadius",radiusEnd);
                drawCircleMat.SetVector("_Center", e.transform.position.ToVec2());
                Graphics.Blit(null, null, drawCircleMat);
            }
        }
        
    }
}

}
