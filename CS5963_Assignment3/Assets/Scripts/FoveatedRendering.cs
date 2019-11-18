 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FoveatedRendering : MonoBehaviour
{

    [HideInInspector]
    public Shader FRShader;

    [NonSerialized]
    Material FRMaterial;

    


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (FRMaterial == null)
        {
            FRMaterial = new Material(FRShader);
            FRMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        float xPercent = .1f;
        float yPercent = .15f;

        RenderTextureFormat format = source.format;

        // USED TO REDUCE THE RESOLUTION BY 1/2
        RenderTexture frHalf_1 = RenderTexture.GetTemporary(source.width/2, source.height/2, 0, format);
        // USED TO SHOW COLORED REGIONS OF CHANGE
        RenderTexture frHalf_2 = RenderTexture.GetTemporary(source.width/2, source.height/2, 0, format);
        RenderTexture frHalf_ref = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
        // USED TO GO TO 1/4 RESOLUTION
        RenderTexture frFourth_1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
        // USED TO BRING BACK TO FULL RES
        RenderTexture frFull = RenderTexture.GetTemporary(source.width, source.height, 0, format);

        FRMaterial.SetFloat("_minX", source.width / 2 * xPercent);
        FRMaterial.SetFloat("_maxX", source.width / 2 * (1-xPercent));
        FRMaterial.SetFloat("_minY", source.height / 2 * yPercent);
        FRMaterial.SetFloat("_maxY", source.height / 2 * (1 - yPercent));

        // sset reference for blending lower res with higher
        FRMaterial.SetTexture("_frTex", frHalf_ref);

        // Get 1/2 Resolution
        Graphics.Blit(source, frHalf_1);

        // Get 1/4 resolution
        //Graphics.Blit(frHalf_1, frFourth_1);

        // back to 1/2 res keeping 1/2 at all but corners
        //Graphics.Blit(frFourth_1, frHalf_ref);
        //Graphics.Blit(frHalf_1, frHalf_2, FRMaterial, 4);  // Horizotnal pass
        //Graphics.Blit(frHalf_2, frHalf_1, FRMaterial, 5);  // Vertical pass

        // For Visual Sake, add colors
        //Graphics.Blit(frHalf_1, frHalf_2, FRMaterial, 0);
        //Graphics.Blit(frHalf_2, frHalf_1, FRMaterial, 1);

        // back up to full resolution
        Graphics.Blit(frHalf_1, frFull);

        FRMaterial.SetFloat("_minX", source.width * xPercent);
        FRMaterial.SetFloat("_maxX", source.width * (1-xPercent));
        FRMaterial.SetFloat("_minY", source.height * yPercent);
        FRMaterial.SetFloat("_maxY", source.height * (1 - yPercent));
        FRMaterial.SetTexture("_frTex", frFull);

        // BLEND
        RenderTexture fr_full_2 = RenderTexture.GetTemporary(source.width, source.height, 0, format);

        // Render at full res blending the low res edges to full res
        Graphics.Blit(source, fr_full_2, FRMaterial, 2);
        Graphics.Blit(fr_full_2, destination, FRMaterial, 3);
        //Graphics.Blit(source, destination, FRMaterial, 2);
        //Graphics.Blit(source, destination, FRMaterial, 2);
        //Graphics.Blit(frFull, destination);
      

        RenderTexture.ReleaseTemporary(frHalf_1);
        RenderTexture.ReleaseTemporary(frHalf_2);
        RenderTexture.ReleaseTemporary(frFull);
        RenderTexture.ReleaseTemporary(fr_full_2);
        RenderTexture.ReleaseTemporary(frFourth_1);
        RenderTexture.ReleaseTemporary(frHalf_ref);
    }
}
