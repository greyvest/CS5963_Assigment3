using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FullFoveatedRendering : MonoBehaviour { 

    [HideInInspector]
    public Shader FRShader_Low;
    [HideInInspector]
    public Shader FRShader_Medium;
    [HideInInspector]
    public Shader FRShader_High;


    [NonSerialized]
    Material FRMaterial_Low;
    [NonSerialized]
    Material FRMaterial_Medium;
    [NonSerialized]
    Material FRMaterial_High;

    int CurrentMode = 0;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        // Low Res
        if (CurrentMode == 0)
        {
            if (FRMaterial_Low == null)
            {
                FRMaterial_Low = new Material(FRShader_Low);
                FRMaterial_Low.hideFlags = HideFlags.HideAndDontSave;
            }

            float xPercent = .1f;
            float yPercent = .15f;

            RenderTextureFormat format = source.format;
            // USED TO REDUCE RESOLUTIONS
            RenderTexture frFull_ref = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frFull = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frHalf_H = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_V = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_Reference = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frFourth = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);

            // Down to 1/4 and then up to 1/2 again to keep the 1/4 blur
            Graphics.Blit(source, frHalf_H);
            Graphics.Blit(frHalf_H, frFourth);
            Graphics.Blit(frFourth, frHalf_Reference);

            xPercent = .07f;
            yPercent = .1f;

            // Set X Ranges
            FRMaterial_Low.SetFloat("_minX_1", 0);
            FRMaterial_Low.SetFloat("_minX_2", source.width / 2 * xPercent);
            FRMaterial_Low.SetFloat("_maxX_1", source.width / 2 * (1-xPercent));
            FRMaterial_Low.SetFloat("_maxX_2", source.width / 2);

            // Set Y Ranges
            FRMaterial_Low.SetFloat("_minY_1", 0);
            FRMaterial_Low.SetFloat("_minY_2", source.height / 2 * yPercent);
            FRMaterial_Low.SetFloat("_maxY_1", source.height / 2 * (1-yPercent));
            FRMaterial_Low.SetFloat("_maxY_2", source.height / 2);

            // Set the reference texture for the lower resolutoin
            FRMaterial_Low.SetTexture("_frTex", frHalf_Reference);

            // Horizontal 1/4 Pass
            Graphics.Blit(frHalf_H, frHalf_V, FRMaterial_Low, 0);
            // Vertical 1/4 Pass
            Graphics.Blit(frHalf_V, frHalf_H, FRMaterial_Low, 1);
            // Back up to full res
            Graphics.Blit(frHalf_H, frFull_ref);

            // Reset Ranges
            // Set X Ranges
            FRMaterial_Low.SetFloat("_minX_1", 0);
            FRMaterial_Low.SetFloat("_minX_2", source.width * xPercent);
            FRMaterial_Low.SetFloat("_maxX_1", source.width * (1 - xPercent));
            FRMaterial_Low.SetFloat("_maxX_2", source.width);

            // Set Y Ranges
            FRMaterial_Low.SetFloat("_minY_1", 0);
            FRMaterial_Low.SetFloat("_minY_2", source.height * yPercent);
            FRMaterial_Low.SetFloat("_maxY_1", source.height * (1 - yPercent));
            FRMaterial_Low.SetFloat("_maxY_2", source.height);

            FRMaterial_Low.SetTexture("_frTex", frFull_ref);

            // Horizontal 1/2 Pass
            Graphics.Blit(source, frFull, FRMaterial_Low, 2);
            // Vertical 1/2 Pass
            Graphics.Blit(frFull, destination, FRMaterial_Low, 3);

            // RELEASE THE TEMPORARIES
            RenderTexture.ReleaseTemporary(frHalf_H);
            RenderTexture.ReleaseTemporary(frHalf_V);
            RenderTexture.ReleaseTemporary(frHalf_Reference);
            RenderTexture.ReleaseTemporary(frFourth);
            RenderTexture.ReleaseTemporary(frFull_ref);
            RenderTexture.ReleaseTemporary(frFull);
        }

        else if (CurrentMode == 1)
        {
            if (FRMaterial_Medium == null)
            {
                FRMaterial_Medium = new Material(FRShader_Medium);
                FRMaterial_Medium.hideFlags = HideFlags.HideAndDontSave;
            }

            float xPercent = .1f;
            float yPercent = .15f;

            RenderTextureFormat format = source.format;
            // USED TO REDUCE RESOLUTIONS
            RenderTexture frFull_ref = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frFull = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frHalf_H = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_V = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_Reference = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frFourth_H = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frFourth_V = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frFourth_Reference = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frEighth = RenderTexture.GetTemporary(source.width / 8, source.height / 8, 0, format);

            Graphics.Blit(source, frHalf_H);
            Graphics.Blit(frHalf_H, frFourth_H);
            Graphics.Blit(frFourth_H, frEighth);
            Graphics.Blit(frEighth, frFourth_Reference);

            xPercent = .07f;
            yPercent = .1f;

            // Set X Ranges
            FRMaterial_Medium.SetFloat("_minX_1", 0);
            FRMaterial_Medium.SetFloat("_minX_2", source.width / 4 * xPercent);
            FRMaterial_Medium.SetFloat("_maxX_1", source.width / 4 * (1 - xPercent));
            FRMaterial_Medium.SetFloat("_maxX_2", source.width / 4);

            // Set Y Ranges
            FRMaterial_Medium.SetFloat("_minY_1", 0);
            FRMaterial_Medium.SetFloat("_minY_2", source.height / 4 * yPercent);
            FRMaterial_Medium.SetFloat("_maxY_1", source.height / 4 * (1 - yPercent));
            FRMaterial_Medium.SetFloat("_maxY_2", source.height / 4);

            FRMaterial_Medium.SetTexture("_frTex", frFourth_Reference);

            // Horizontal 1/8 Pass
            Graphics.Blit(frFourth_H, frFourth_V, FRMaterial_Medium, 4);
            // Vertical 1/8 Pass
            Graphics.Blit(frFourth_V, frFourth_H, FRMaterial_Medium, 5);
            // Back to 1/2 res
            Graphics.Blit(frFourth_H, frHalf_Reference);

            // RESET RANGES FOR 1/2
            // Set X Ranges
            FRMaterial_Medium.SetFloat("_minX_1", 0);
            FRMaterial_Medium.SetFloat("_minX_2", source.width / 2 * xPercent);
            FRMaterial_Medium.SetFloat("_maxX_1", source.width / 2 * (1 - xPercent));
            FRMaterial_Medium.SetFloat("_maxX_2", source.width / 2);

            // Set Y Ranges
            FRMaterial_Medium.SetFloat("_minY_1", 0);
            FRMaterial_Medium.SetFloat("_minY_2", source.height / 2 * yPercent);
            FRMaterial_Medium.SetFloat("_maxY_1", source.height / 2 * (1 - yPercent));
            FRMaterial_Medium.SetFloat("_maxY_2", source.height / 2);

            FRMaterial_Medium.SetTexture("_frTex", frHalf_Reference);

            // Horizontal 1/4 Pass
            Graphics.Blit(frHalf_H, frHalf_V, FRMaterial_Medium, 6);
            // Vertical 1/4 Pass
            Graphics.Blit(frHalf_V, frHalf_H, FRMaterial_Medium, 7);
            // Back to full Res
            Graphics.Blit(frHalf_H, frFull_ref);

            // RESET RANGES FOR 1/2
            // Set X Ranges
            FRMaterial_Medium.SetFloat("_minX_1", source.width * xPercent);
            FRMaterial_Medium.SetFloat("_minX_2", source.width * xPercent * 2);
            FRMaterial_Medium.SetFloat("_maxX_1", source.width * (1 - xPercent*2));
            FRMaterial_Medium.SetFloat("_maxX_2", source.width * (1 - xPercent));

            // Set Y Ranges
            FRMaterial_Medium.SetFloat("_maxY_1", source.height * (1 - yPercent * 2.25f));
            FRMaterial_Medium.SetFloat("_maxY_2", source.height * (1 - yPercent));

            FRMaterial_Medium.SetTexture("_frTex", frFull_ref);

            // Horizontal 1/2 Pass
            Graphics.Blit(source, frFull, FRMaterial_Medium, 8);
            // Vertical 1/2 Pass give to Dest
            Graphics.Blit(frFull, destination, FRMaterial_Medium, 9);

            // RELEASE THE TEMPORARIES
            RenderTexture.ReleaseTemporary(frHalf_H);
            RenderTexture.ReleaseTemporary(frHalf_V);
            RenderTexture.ReleaseTemporary(frHalf_Reference);
            RenderTexture.ReleaseTemporary(frFourth_H);
            RenderTexture.ReleaseTemporary(frFourth_V);
            RenderTexture.ReleaseTemporary(frFourth_Reference);
            RenderTexture.ReleaseTemporary(frFull_ref);
            RenderTexture.ReleaseTemporary(frFull);
            RenderTexture.ReleaseTemporary(frEighth);
        }

        /*
         * OLD CODE STYLE
         * 
        // USED TO REDUCE THE RESOLUTION BY 1/2
        RenderTexture frHalf_1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
        // USED TO SHOW COLORED REGIONS OF CHANGE
        RenderTexture frHalf_2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
        RenderTexture frHalf_ref = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
        // USED TO GO TO 1/4 RESOLUTION
        RenderTexture frFourth_1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
        // USED TO BRING BACK TO FULL RES
        RenderTexture frFull = RenderTexture.GetTemporary(source.width, source.height, 0, format);

        FRMaterial.SetFloat("_minX", source.width / 2 * xPercent);
        FRMaterial.SetFloat("_maxX", source.width / 2 * (1 - xPercent));
        FRMaterial.SetFloat("_minY", source.height / 2 * yPercent);
        FRMaterial.SetFloat("_maxY", source.height / 2 * (1 - yPercent));

        // sset reference for blending lower res with higher
        FRMaterial.SetTexture("_frTex", frHalf_ref);

        // Get 1/2 Resolution
        Graphics.Blit(source, frHalf_1);

        // Get 1/4 resolution
        Graphics.Blit(frHalf_1, frFourth_1);

        // back to 1/2 res keeping 1/2 at all but corners
        Graphics.Blit(frFourth_1, frHalf_ref);
        Graphics.Blit(frHalf_1, frHalf_2, FRMaterial, 4);  // Horizotnal pass
        Graphics.Blit(frHalf_2, frHalf_1, FRMaterial, 5);  // Vertical pass

        // For Visual Sake, add colors
        //Graphics.Blit(frHalf_1, frHalf_2, FRMaterial, 0);
        //Graphics.Blit(frHalf_2, frHalf_1, FRMaterial, 1);

        // back up to full resolution
        Graphics.Blit(frHalf_1, frFull);

        FRMaterial.SetFloat("_minX", source.width * xPercent);
        FRMaterial.SetFloat("_maxX", source.width * (1 - xPercent));
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
        */
    }
}
