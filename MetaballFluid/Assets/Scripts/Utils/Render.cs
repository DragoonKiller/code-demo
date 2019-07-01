using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util
{
    public static void SetupTexture(ref RenderTexture tex)
    {
        if(tex != null && tex.texelSize != new Vector2(Screen.width, Screen.height))
            tex = null;

        if(tex == null)
        {
            tex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            tex.Create();
        }
    }
}
