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

    int CurrentMode = 1;

    float xPercent = .1f;
    float yPercent = .1f;

    bool Disabled = false;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Disabled)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Low
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
        // Medium
        else if (CurrentMode == 1)
        {
            if (FRMaterial_Medium == null)
            {
                FRMaterial_Medium = new Material(FRShader_Medium);
                FRMaterial_Medium.hideFlags = HideFlags.HideAndDontSave;
            }

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
            Graphics.Blit(frFourth_H, frFourth_V, FRMaterial_Medium, 0);
            // Vertical 1/8 Pass
            Graphics.Blit(frFourth_V, frFourth_H, FRMaterial_Medium, 1);
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
            Graphics.Blit(frHalf_H, frHalf_V, FRMaterial_Medium, 2);
            // Vertical 1/4 Pass
            Graphics.Blit(frHalf_V, frHalf_H, FRMaterial_Medium, 3);
            // Back to full Res
            Graphics.Blit(frHalf_H, frFull_ref);

            // RESET RANGES FOR 1/2
            // Set X Ranges
            FRMaterial_Medium.SetFloat("_minX_1", source.width * xPercent);
            FRMaterial_Medium.SetFloat("_minX_2", source.width * xPercent * 2);
            FRMaterial_Medium.SetFloat("_maxX_1", source.width * (1 - xPercent*2));
            FRMaterial_Medium.SetFloat("_maxX_2", source.width * (1 - xPercent));

            // Set Y Ranges
            FRMaterial_Medium.SetFloat("_minY_1", 0);
            FRMaterial_Medium.SetFloat("_minY_2", source.height * yPercent);
            FRMaterial_Medium.SetFloat("_maxY_1", source.height * (1 - yPercent * 2.25f));
            FRMaterial_Medium.SetFloat("_maxY_2", source.height * (1 - yPercent));

            FRMaterial_Medium.SetTexture("_frTex", frFull_ref);

            // Horizontal 1/2 Pass
            Graphics.Blit(source, frFull, FRMaterial_Medium, 4);
            // Vertical 1/2 Pass give to Dest
            Graphics.Blit(frFull, destination, FRMaterial_Medium, 5);

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
        // High
        else if (CurrentMode == 2)
        {
            if (FRMaterial_High == null)
            {
                FRMaterial_High = new Material(FRShader_High);
                FRMaterial_High.hideFlags = HideFlags.HideAndDontSave;
            }


            RenderTextureFormat format = source.format;
            RenderTexture frFull_ref = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frFull = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture frHalf_H = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_V = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frHalf_Reference = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
            RenderTexture frFourth_H = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frFourth_V = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frFourth_Reference = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
            RenderTexture frEighth_H = RenderTexture.GetTemporary(source.width / 8, source.height / 8, 0, format);
            RenderTexture frEighth_V = RenderTexture.GetTemporary(source.width / 8, source.height / 8, 0, format);
            RenderTexture frEighth_Ref = RenderTexture.GetTemporary(source.width / 8, source.height / 8, 0, format);
            RenderTexture frSixteenth = RenderTexture.GetTemporary(source.width / 16, source.height / 16, 0, format);

            // Down to 1/16 Resolution and back
            Graphics.Blit(source, frHalf_H);
            Graphics.Blit(frHalf_H, frFourth_H);
            Graphics.Blit(frFourth_H, frEighth_H);
            Graphics.Blit(frEighth_H, frSixteenth);
            Graphics.Blit(frSixteenth, frEighth_Ref);

            // SET VALUES
            // SET X
            // Set X Ranges
            FRMaterial_High.SetFloat("_minX_1", 0);
            FRMaterial_High.SetFloat("_minX_2", source.width / 8 * xPercent);
            FRMaterial_High.SetFloat("_maxX_1", source.width / 8 * (1 - xPercent));
            FRMaterial_High.SetFloat("_maxX_2", source.width / 8);

            // Set Y Ranges
            FRMaterial_High.SetFloat("_minY_1", 0);
            FRMaterial_High.SetFloat("_minY_2", source.height / 8 * yPercent);
            FRMaterial_High.SetFloat("_maxY_1", source.height / 8 * (1 - yPercent));
            FRMaterial_High.SetFloat("_maxY_2", source.height / 8);

            FRMaterial_High.SetTexture("_frTex", frEighth_Ref);

            // Horizontal 1/16
            Graphics.Blit(frEighth_H, frEighth_V, FRMaterial_High, 0);
            // Horiztonal 1/16
            Graphics.Blit(frEighth_V, frEighth_H, FRMaterial_High, 1);
            Graphics.Blit(frEighth_H, frFourth_Reference);

            //===========================================================================
            // SET VALUES
            // SET X
            // Set X Ranges
            FRMaterial_High.SetFloat("_minX_1", 0);
            FRMaterial_High.SetFloat("_minX_2", source.width / 4 * xPercent);
            FRMaterial_High.SetFloat("_maxX_1", source.width / 4 * (1 - xPercent));
            FRMaterial_High.SetFloat("_maxX_2", source.width / 4);

            // Set Y Ranges
            FRMaterial_High.SetFloat("_minY_1", 0);
            FRMaterial_High.SetFloat("_minY_2", source.height / 4 * yPercent);
            FRMaterial_High.SetFloat("_maxY_1", source.height / 4 * (1 - yPercent));
            FRMaterial_High.SetFloat("_maxY_2", source.height / 4);

            FRMaterial_High.SetTexture("_frTex", frFourth_Reference);

           
            // Horizontal 1/8
            Graphics.Blit(frFourth_H, frFourth_V, FRMaterial_High, 2);
            // Vertical 1/8
            Graphics.Blit(frFourth_V, frFourth_H, FRMaterial_High, 3);
            Graphics.Blit(frFourth_H, frHalf_Reference);

            //===========================================================================
            // SET VALUES
            // SET X
            // Set X Ranges
            FRMaterial_High.SetFloat("_minX_1", source.width / 2 * xPercent);
            FRMaterial_High.SetFloat("_minX_2", source.width / 2 * xPercent * 2);
            FRMaterial_High.SetFloat("_maxX_1", source.width / 2 * (1 - xPercent * 2));
            FRMaterial_High.SetFloat("_maxX_2", source.width / 2 * (1 - xPercent));

            // Set Y Ranges
            FRMaterial_High.SetFloat("_minY_1", 0);
            FRMaterial_High.SetFloat("_minY_2", source.height / 2 * yPercent);
            FRMaterial_High.SetFloat("_maxY_1", source.height / 2 * (1 - yPercent * 2.25f));
            FRMaterial_High.SetFloat("_maxY_2", source.height / 2 * (1 - yPercent));

            FRMaterial_High.SetTexture("_frTex", frHalf_Reference);


            // FIX THIS ONE -------------------------------------
            // Horizontal 1/4
            Graphics.Blit(frHalf_H, frHalf_V, FRMaterial_High, 5);
            // Vertical 1/4
            //Graphics.Blit(frHalf_V, frHalf_H, FRMaterial_High, 5);
            Graphics.Blit(frHalf_V, frFull_ref);

            //===========================================================================
            // SET VALUES
            // SET X
            // Set X Ranges
            FRMaterial_High.SetFloat("_minX_1", source.width  * xPercent);
            FRMaterial_High.SetFloat("_minX_2", source.width  * xPercent * 2);
            FRMaterial_High.SetFloat("_maxX_1", source.width  * (1 - xPercent * 2));
            FRMaterial_High.SetFloat("_maxX_2", source.width  * (1 - xPercent));

            // Set Y Ranges
            FRMaterial_High.SetFloat("_minY_1", 0);
            FRMaterial_High.SetFloat("_minY_2", source.height * yPercent);
            FRMaterial_High.SetFloat("_maxY_1", source.height * yPercent);
            FRMaterial_High.SetFloat("_maxY_2", source.height * (1 - yPercent * 2.25f));

            FRMaterial_High.SetTexture("_frTex", frFull_ref);

            // Horizontal 1/2
            Graphics.Blit(frHalf_H, frHalf_V, FRMaterial_High, 6);
            // VERTICAL 1/2 and place on source
            Graphics.Blit(frHalf_V, destination, FRMaterial_High, 7);


            RenderTexture.ReleaseTemporary(frHalf_H);
            RenderTexture.ReleaseTemporary(frHalf_V);
            RenderTexture.ReleaseTemporary(frHalf_Reference);
            RenderTexture.ReleaseTemporary(frFourth_H);
            RenderTexture.ReleaseTemporary(frFourth_V);
            RenderTexture.ReleaseTemporary(frFourth_Reference);
            RenderTexture.ReleaseTemporary(frFull_ref);
            RenderTexture.ReleaseTemporary(frFull);
            RenderTexture.ReleaseTemporary(frEighth_H);
            RenderTexture.ReleaseTemporary(frEighth_V);
            RenderTexture.ReleaseTemporary(frEighth_Ref);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentMode = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentMode = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentMode = 2;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Disabled = !Disabled;
        }
    }
}
