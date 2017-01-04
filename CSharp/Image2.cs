using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[AddComponentMenu("UI/Image2", 11)]
public class Image2 : Image
{
    [SerializeField]
    protected ImageAlphaTexLayout m_alphaTexLayout;
    private bool m_initialized;
    private static List<Component> s_components = new List<Component>();
    private static Material s_defaultMaterial;
    private static DictionaryObjectView<Material, Material> s_materialList = new DictionaryObjectView<Material, Material>();
    private static Vector2[] s_sizeScaling = new Vector2[] { new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(1f, 0.5f) };
    private static readonly Vector2[] s_Uv = new Vector2[4];
    private static readonly Vector2[] s_UVScratch = new Vector2[4];
    private static readonly Vector2[] s_VertScratch = new Vector2[4];
    private static readonly Vector2[] s_Xy = new Vector2[4];
    public bool WriteTexcoordToNormal;

    private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
    {
        v.position = new Vector3(posMin.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMin.y);
        vbo.Add(v);
        v.position = new Vector3(posMin.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMax.y);
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMax.y);
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMin.y);
        vbo.Add(v);
    }

    private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax, Vector2 offset)
    {
        v.position = new Vector3(posMin.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMin.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMin.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMax.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMax.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMin.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
    }

    private unsafe void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
    {
        if (base.fillAmount >= 0.001)
        {
            Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, s_sizeScaling[(int) this.alphaTexLayout]);
            Vector2 zero = Vector2.zero;
            Vector4 vector3 = Vector4.zero;
            if (base.overrideSprite != null)
            {
                vector3 = GetOuterUV(base.overrideSprite, this.alphaTexLayout, out zero);
            }
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.color = base.color;
            float x = vector3.x;
            float y = vector3.y;
            float z = vector3.z;
            float w = vector3.w;
            if ((base.fillMethod == Image.FillMethod.Horizontal) || (base.fillMethod == Image.FillMethod.Vertical))
            {
                if (base.fillMethod == Image.FillMethod.Horizontal)
                {
                    float num5 = (z - x) * base.fillAmount;
                    if (base.fillOrigin == 1)
                    {
                        drawingDimensions.x = drawingDimensions.z - ((drawingDimensions.z - drawingDimensions.x) * base.fillAmount);
                        x = z - num5;
                    }
                    else
                    {
                        drawingDimensions.z = drawingDimensions.x + ((drawingDimensions.z - drawingDimensions.x) * base.fillAmount);
                        z = x + num5;
                    }
                }
                else if (base.fillMethod == Image.FillMethod.Vertical)
                {
                    float num6 = (w - y) * base.fillAmount;
                    if (base.fillOrigin == 1)
                    {
                        drawingDimensions.y = drawingDimensions.w - ((drawingDimensions.w - drawingDimensions.y) * base.fillAmount);
                        y = w - num6;
                    }
                    else
                    {
                        drawingDimensions.w = drawingDimensions.y + ((drawingDimensions.w - drawingDimensions.y) * base.fillAmount);
                        w = y + num6;
                    }
                }
            }
            s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
            s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
            s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
            s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
            s_Uv[0] = new Vector2(x, y);
            s_Uv[1] = new Vector2(x, w);
            s_Uv[2] = new Vector2(z, w);
            s_Uv[3] = new Vector2(z, y);
            if (base.fillAmount < 1.0)
            {
                if (base.fillMethod == Image.FillMethod.Radial90)
                {
                    if (RadialCut(s_Xy, s_Uv, base.fillAmount, base.fillClockwise, base.fillOrigin))
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            simpleVert.position = *((Vector3*) &(s_Xy[j]));
                            simpleVert.uv0 = s_Uv[j];
                            simpleVert.uv1 = simpleVert.uv0 + zero;
                            vbo.Add(simpleVert);
                        }
                    }
                    return;
                }
                if (base.fillMethod == Image.FillMethod.Radial180)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        float num10;
                        float num11;
                        float num12;
                        float num13;
                        int num9 = (base.fillOrigin > 1) ? 1 : 0;
                        if ((base.fillOrigin == 0) || (base.fillOrigin == 2))
                        {
                            num10 = 0f;
                            num11 = 1f;
                            if (k == num9)
                            {
                                num12 = 0f;
                                num13 = 0.5f;
                            }
                            else
                            {
                                num12 = 0.5f;
                                num13 = 1f;
                            }
                        }
                        else
                        {
                            num12 = 0f;
                            num13 = 1f;
                            if (k == num9)
                            {
                                num10 = 0.5f;
                                num11 = 1f;
                            }
                            else
                            {
                                num10 = 0f;
                                num11 = 0.5f;
                            }
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num12);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num13);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num10);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num11);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(x, z, num12);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(x, z, num13);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(y, w, num10);
                        s_Uv[1].y = Mathf.Lerp(y, w, num11);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num14 = base.fillClockwise ? ((base.fillAmount * 2f) - k) : ((base.fillAmount * 2f) - (1 - k));
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num14), base.fillClockwise, ((k + base.fillOrigin) + 3) % 4))
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                simpleVert.position = *((Vector3*) &(s_Xy[m]));
                                simpleVert.uv0 = s_Uv[m];
                                simpleVert.uv1 = simpleVert.uv0 + zero;
                                vbo.Add(simpleVert);
                            }
                        }
                    }
                    return;
                }
                if (base.fillMethod == Image.FillMethod.Radial360)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        float num17;
                        float num18;
                        float num19;
                        float num20;
                        if (n < 2)
                        {
                            num17 = 0f;
                            num18 = 0.5f;
                        }
                        else
                        {
                            num17 = 0.5f;
                            num18 = 1f;
                        }
                        switch (n)
                        {
                            case 0:
                            case 3:
                                num19 = 0f;
                                num20 = 0.5f;
                                break;

                            default:
                                num19 = 0.5f;
                                num20 = 1f;
                                break;
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num17);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num18);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num19);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num20);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(x, z, num17);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(x, z, num18);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(y, w, num19);
                        s_Uv[1].y = Mathf.Lerp(y, w, num20);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num21 = base.fillClockwise ? ((base.fillAmount * 4f) - ((n + base.fillOrigin) % 4)) : ((base.fillAmount * 4f) - (3 - ((n + base.fillOrigin) % 4)));
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num21), base.fillClockwise, (n + 2) % 4))
                        {
                            for (int num22 = 0; num22 < 4; num22++)
                            {
                                simpleVert.position = *((Vector3*) &(s_Xy[num22]));
                                simpleVert.uv0 = s_Uv[num22];
                                simpleVert.uv1 = simpleVert.uv0 + zero;
                                vbo.Add(simpleVert);
                            }
                        }
                    }
                    return;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                simpleVert.position = *((Vector3*) &(s_Xy[i]));
                simpleVert.uv0 = s_Uv[i];
                simpleVert.uv1 = simpleVert.uv0 + zero;
                vbo.Add(simpleVert);
            }
        }
    }

    private void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
    {
        Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.color;
        Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
        Vector4 vector3 = (base.overrideSprite != null) ? DataUtility.GetOuterUV(base.overrideSprite) : Vector4.zero;
        float y = vector3.y;
        float w = vector3.w;
        float num3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
        float num4 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
        float x = vector3.x;
        float z = vector3.z;
        float num7 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
        float num8 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(x, y);
        simpleVert.uv1 = new Vector2(num8, num4);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(x, num3);
        simpleVert.uv1 = new Vector2(num8, w);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(num7, num3);
        simpleVert.uv1 = new Vector2(z, w);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(num7, y);
        simpleVert.uv1 = new Vector2(z, num4);
        vbo.Add(simpleVert);
    }

    private void GenerateSimpleSprite_Normal(List<UIVertex> vbo, bool preserveAspect)
    {
        Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.color;
        Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
        Vector4 vector3 = (base.overrideSprite != null) ? DataUtility.GetOuterUV(base.overrideSprite) : Vector4.zero;
        float y = vector3.y;
        float w = vector3.w;
        float num3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
        float num4 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
        float x = vector3.x;
        float z = vector3.z;
        float num7 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
        float num8 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(x, y);
        simpleVert.uv1 = new Vector2(num8, num4);
        simpleVert.normal = new Vector3(-1f, -1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(x, num3);
        simpleVert.uv1 = new Vector2(num8, w);
        simpleVert.normal = new Vector3(-1f, 1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(num7, num3);
        simpleVert.uv1 = new Vector2(z, w);
        simpleVert.normal = new Vector3(1f, 1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(num7, y);
        simpleVert.uv1 = new Vector2(z, num4);
        simpleVert.normal = new Vector3(1f, -1f, 0f);
        vbo.Add(simpleVert);
    }

    private void GenerateSlicedSprite(List<UIVertex> vbo)
    {
        if (!base.hasBorder)
        {
            this.GenerateSimpleSprite(vbo, false);
        }
        else
        {
            Vector4 vector;
            Vector4 innerUV;
            Vector4 padding;
            Vector4 border;
            Vector2 zero = Vector2.zero;
            if (base.overrideSprite != null)
            {
                vector = GetOuterUV(base.overrideSprite, this.alphaTexLayout, out zero);
                innerUV = GetInnerUV(base.overrideSprite, s_sizeScaling[(int) this.alphaTexLayout]);
                padding = DataUtility.GetPadding(base.overrideSprite);
                border = base.overrideSprite.border;
            }
            else
            {
                vector = Vector4.zero;
                innerUV = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            Vector4 adjustedBorders = this.GetAdjustedBorders((Vector4) (border / base.pixelsPerUnit), pixelAdjustedRect);
            Vector4 vector7 = (Vector4) (padding / base.pixelsPerUnit);
            s_VertScratch[0] = new Vector2(vector7.x, vector7.y);
            s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector7.z, pixelAdjustedRect.height - vector7.w);
            s_VertScratch[1].x = adjustedBorders.x;
            s_VertScratch[1].y = adjustedBorders.y;
            s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
            s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
            for (int i = 0; i < 4; i++)
            {
                s_VertScratch[i].x += pixelAdjustedRect.x;
                s_VertScratch[i].y += pixelAdjustedRect.y;
            }
            s_UVScratch[0] = new Vector2(vector.x, vector.y);
            s_UVScratch[1] = new Vector2(innerUV.x, innerUV.y);
            s_UVScratch[2] = new Vector2(innerUV.z, innerUV.w);
            s_UVScratch[3] = new Vector2(vector.z, vector.w);
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.color = base.color;
            for (int j = 0; j < 3; j++)
            {
                int index = j + 1;
                for (int k = 0; k < 3; k++)
                {
                    if ((base.fillCenter || (j != 1)) || (k != 1))
                    {
                        int num5 = k + 1;
                        this.AddQuad(vbo, simpleVert, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[index].x, s_VertScratch[num5].y), new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[index].x, s_UVScratch[num5].y), zero);
                    }
                }
            }
        }
    }

    private void GenerateTiledSprite(List<UIVertex> vbo)
    {
        Vector4 zero;
        Vector4 innerUV;
        Vector4 adjustedBorders;
        Vector2 size;
        Vector2 vector5;
        if (base.overrideSprite != null)
        {
            Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
            zero = GetOuterUV(base.overrideSprite, this.alphaTexLayout, out vector5);
            innerUV = GetInnerUV(base.overrideSprite, sizeScaling);
            adjustedBorders = base.overrideSprite.border;
            size = base.overrideSprite.rect.size;
            size.x *= sizeScaling.x;
            size.y *= sizeScaling.y;
        }
        else
        {
            zero = Vector4.zero;
            innerUV = Vector4.zero;
            adjustedBorders = Vector4.zero;
            size = (Vector2) (Vector2.one * 100f);
            vector5 = Vector2.zero;
        }
        Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
        float num = ((size.x - adjustedBorders.x) - adjustedBorders.z) / base.pixelsPerUnit;
        float num2 = ((size.y - adjustedBorders.y) - adjustedBorders.w) / base.pixelsPerUnit;
        adjustedBorders = this.GetAdjustedBorders((Vector4) (adjustedBorders / base.pixelsPerUnit), pixelAdjustedRect);
        Vector2 uvMin = new Vector2(innerUV.x, innerUV.y);
        Vector2 vector8 = new Vector2(innerUV.z, innerUV.w);
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.color;
        float x = adjustedBorders.x;
        float num4 = pixelAdjustedRect.width - adjustedBorders.z;
        float y = adjustedBorders.y;
        float num6 = pixelAdjustedRect.height - adjustedBorders.w;
        if (((num4 - x) > (num * 100.0)) || ((num6 - y) > (num2 * 100.0)))
        {
            num = (float) ((num4 - x) / 100.0);
            num2 = (float) ((num6 - y) / 100.0);
        }
        Vector2 uvMax = vector8;
        if (base.fillCenter)
        {
            for (float i = y; i < num6; i += num2)
            {
                float num8 = i + num2;
                if (num8 > num6)
                {
                    uvMax.y = uvMin.y + (((vector8.y - uvMin.y) * (num6 - i)) / (num8 - i));
                    num8 = num6;
                }
                uvMax.x = vector8.x;
                for (float j = x; j < num4; j += num)
                {
                    float num10 = j + num;
                    if (num10 > num4)
                    {
                        uvMax.x = uvMin.x + (((vector8.x - uvMin.x) * (num4 - j)) / (num10 - j));
                        num10 = num4;
                    }
                    this.AddQuad(vbo, simpleVert, new Vector2(j, i) + pixelAdjustedRect.position, new Vector2(num10, num8) + pixelAdjustedRect.position, uvMin, uvMax, vector5);
                }
            }
        }
        if (base.hasBorder)
        {
            Vector2 vector10 = vector8;
            for (float k = y; k < num6; k += num2)
            {
                float num12 = k + num2;
                if (num12 > num6)
                {
                    vector10.y = uvMin.y + (((vector8.y - uvMin.y) * (num6 - k)) / (num12 - k));
                    num12 = num6;
                }
                this.AddQuad(vbo, simpleVert, new Vector2(0f, k) + pixelAdjustedRect.position, new Vector2(x, num12) + pixelAdjustedRect.position, new Vector2(zero.x, uvMin.y), new Vector2(uvMin.x, vector10.y), vector5);
                this.AddQuad(vbo, simpleVert, new Vector2(num4, k) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num12) + pixelAdjustedRect.position, new Vector2(vector8.x, uvMin.y), new Vector2(zero.z, vector10.y), vector5);
            }
            vector10 = vector8;
            for (float m = x; m < num4; m += num)
            {
                float num14 = m + num;
                if (num14 > num4)
                {
                    vector10.x = uvMin.x + (((vector8.x - uvMin.x) * (num4 - m)) / (num14 - m));
                    num14 = num4;
                }
                this.AddQuad(vbo, simpleVert, new Vector2(m, 0f) + pixelAdjustedRect.position, new Vector2(num14, y) + pixelAdjustedRect.position, new Vector2(uvMin.x, zero.y), new Vector2(vector10.x, uvMin.y), vector5);
                this.AddQuad(vbo, simpleVert, new Vector2(m, num6) + pixelAdjustedRect.position, new Vector2(num14, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(uvMin.x, vector8.y), new Vector2(vector10.x, zero.w), vector5);
            }
            this.AddQuad(vbo, simpleVert, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(zero.x, zero.y), new Vector2(uvMin.x, uvMin.y), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(num4, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, new Vector2(vector8.x, zero.y), new Vector2(zero.z, uvMin.y), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(0f, num6) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(zero.x, vector8.y), new Vector2(uvMin.x, zero.w), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(num4, num6) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(vector8.x, vector8.y), new Vector2(zero.z, zero.w), vector5);
        }
    }

    private unsafe Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
    {
        for (int i = 0; i <= 1; i++)
        {
            float num2 = border[i] + border[i + 2];
            if ((rect.size[i] < num2) && (num2 != 0.0))
            {
                ref Vector4 vectorRef;
                int num4;
                ref Vector4 vectorRef2;
                float num3 = rect.size[i] / num2;
                float num5 = vectorRef[num4];
                (vectorRef = (Vector4) &border)[num4 = i] = num5 * num3;
                num5 = vectorRef2[num4];
                (vectorRef2 = (Vector4) &border)[num4 = i + 2] = num5 * num3;
            }
        }
        return border;
    }

    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect, Vector2 sizeScaling)
    {
        Vector4 vector = (base.overrideSprite == null) ? Vector4.zero : DataUtility.GetPadding(base.overrideSprite);
        Vector2 vector2 = (base.overrideSprite == null) ? Vector2.zero : new Vector2(base.overrideSprite.rect.width * sizeScaling.x, base.overrideSprite.rect.height * sizeScaling.y);
        Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
        int num = Mathf.RoundToInt(vector2.x);
        int num2 = Mathf.RoundToInt(vector2.y);
        Vector4 vector3 = new Vector4(vector.x / ((float) num), vector.y / ((float) num2), (num - vector.z) / ((float) num), (num2 - vector.w) / ((float) num2));
        if (shouldPreserveAspect && (vector2.sqrMagnitude > 0.0))
        {
            float num3 = vector2.x / vector2.y;
            float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
            if (num3 > num4)
            {
                float height = pixelAdjustedRect.height;
                pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
                pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
            }
            else
            {
                float width = pixelAdjustedRect.width;
                pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
                pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
            }
        }
        return new Vector4(pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.x), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.y), pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.z), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.w));
    }

    private static Vector4 GetInnerUV(Sprite sprite, Vector2 sizeScaling)
    {
        Vector4 vector3;
        Texture texture = sprite.texture;
        if (texture == null)
        {
            return new Vector4(0f, 0f, sizeScaling.x, sizeScaling.y);
        }
        Rect textureRect = sprite.textureRect;
        textureRect.width *= sizeScaling.x;
        textureRect.height *= sizeScaling.y;
        float num = 1f / ((float) texture.width);
        float num2 = 1f / ((float) texture.height);
        Vector4 padding = DataUtility.GetPadding(sprite);
        Vector4 border = sprite.border;
        float num3 = textureRect.x + padding.x;
        float num4 = textureRect.y + padding.y;
        return new Vector4 { x = num3 + border.x, y = num4 + border.y, z = (textureRect.x + textureRect.width) - border.z, w = (textureRect.y + textureRect.height) - border.w, x = vector3.x * num, y = vector3.y * num2, z = vector3.z * num, w = vector3.w * num2 };
    }

    private static Vector4 GetOuterUV(Sprite sprite, ImageAlphaTexLayout layout, out Vector2 offset)
    {
        Vector4 outerUV = DataUtility.GetOuterUV(sprite);
        offset = Vector2.zero;
        ImageAlphaTexLayout layout2 = layout;
        if (layout2 != ImageAlphaTexLayout.Horizonatal)
        {
            if (layout2 != ImageAlphaTexLayout.Vertical)
            {
                return outerUV;
            }
        }
        else
        {
            offset.x = (outerUV.z - outerUV.x) * 0.5f;
            outerUV.z = (outerUV.z + outerUV.x) * 0.5f;
            return outerUV;
        }
        offset.y = (outerUV.w - outerUV.y) * 0.5f;
        outerUV.w = (outerUV.w + outerUV.y) * 0.5f;
        return outerUV;
    }

    private int GetStencilForGraphic()
    {
        int num = 0;
        Transform parent = base.transform.parent;
        s_components.Clear();
        while (parent != null)
        {
            parent.GetComponents(typeof(IMask), s_components);
            for (int i = 0; i < s_components.Count; i++)
            {
                IMask mask = s_components[i] as IMask;
                if ((mask != null) && mask.MaskEnabled())
                {
                    num++;
                    num = Mathf.Clamp(num, 0, 8);
                    break;
                }
            }
            parent = parent.parent;
        }
        s_components.Clear();
        return num;
    }

    protected override void OnCanvasHierarchyChanged()
    {
    }

    protected override void OnDestroy()
    {
        base.sprite = null;
        base.overrideSprite = null;
    }

    protected override void OnFillVBO(List<UIVertex> vbo)
    {
        if ((base.overrideSprite == null) || ((this.alphaTexLayout == ImageAlphaTexLayout.None) && !this.WriteTexcoordToNormal))
        {
            base.OnFillVBO(vbo);
        }
        else
        {
            switch (base.type)
            {
                case Image.Type.Simple:
                    if (!this.WriteTexcoordToNormal)
                    {
                        this.GenerateSimpleSprite(vbo, base.preserveAspect);
                        break;
                    }
                    this.GenerateSimpleSprite_Normal(vbo, base.preserveAspect);
                    break;

                case Image.Type.Sliced:
                    this.GenerateSlicedSprite(vbo);
                    break;

                case Image.Type.Tiled:
                    this.GenerateTiledSprite(vbo);
                    break;

                case Image.Type.Filled:
                    this.GenerateFilledSprite(vbo, base.preserveAspect);
                    break;

                default:
                    DebugHelper.Assert(false);
                    break;
            }
        }
    }

    private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
    {
        if (fill < 0.001)
        {
            return false;
        }
        if ((corner & 1) == 1)
        {
            invert = !invert;
        }
        if (invert || (fill <= 0.999000012874603))
        {
            float num = Mathf.Clamp01(fill);
            if (invert)
            {
                num = 1f - num;
            }
            float f = num * 1.570796f;
            float cos = Mathf.Cos(f);
            float sin = Mathf.Sin(f);
            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
        }
        return true;
    }

    private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
    {
        int index = corner;
        int num2 = (corner + 1) % 4;
        int num3 = (corner + 2) % 4;
        int num4 = (corner + 3) % 4;
        if ((corner & 1) == 1)
        {
            if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                    xy[num3].x = xy[num2].x;
                }
            }
            else if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num3].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                    xy[num4].y = xy[num3].y;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (!invert)
            {
                xy[num4].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
            }
            else
            {
                xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
            }
        }
        else
        {
            if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                    xy[num3].y = xy[num2].y;
                }
            }
            else if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num3].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                    xy[num4].x = xy[num3].x;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (invert)
            {
                xy[num4].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
            }
            else
            {
                xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
            }
        }
    }

    public void SetMaterialVector(string name, Vector4 factor)
    {
        if (base.m_Material != null)
        {
            if (!base.m_Material.name.Contains("(Clone)"))
            {
                Material material = new Material(base.m_Material) {
                    name = base.m_Material.name + "(Clone)"
                };
                material.CopyPropertiesFromMaterial(base.m_Material);
                material.shaderKeywords = base.m_Material.shaderKeywords;
                material.SetVector(name, factor);
                this.material = material;
            }
            else
            {
                base.m_Material.SetVector(name, factor);
                this.SetMaterialDirty();
            }
        }
    }

    public override void SetNativeSize()
    {
        if (base.overrideSprite != null)
        {
            Vector2 vector = s_sizeScaling[(int) this.alphaTexLayout];
            float x = (base.overrideSprite.rect.width * vector.x) / base.pixelsPerUnit;
            float y = (base.overrideSprite.rect.height * vector.y) / base.pixelsPerUnit;
            base.rectTransform.anchorMax = base.rectTransform.anchorMin;
            base.rectTransform.sizeDelta = new Vector2(x, y);
            this.SetAllDirty();
        }
    }

    private void UpdateInternalState()
    {
        if (base.m_ShouldRecalculate)
        {
            base.m_StencilValue = this.GetStencilForGraphic();
            Transform parent = base.transform.parent;
            base.m_IncludeForMasking = false;
            s_components.Clear();
            while (base.maskable && (parent != null))
            {
                parent.GetComponents(typeof(IMask), s_components);
                if (s_components.Count > 0)
                {
                    base.m_IncludeForMasking = true;
                    break;
                }
                parent = parent.parent;
            }
            base.m_ShouldRecalculate = false;
            s_components.Clear();
        }
    }

    public ImageAlphaTexLayout alphaTexLayout
    {
        get
        {
            return this.m_alphaTexLayout;
        }
        set
        {
            if (this.m_alphaTexLayout != value)
            {
                this.m_alphaTexLayout = value;
                this.SetMaterialDirty();
            }
        }
    }

    public Material baseMaterial
    {
        get
        {
            if ((base.m_Material == null) || (base.m_Material == defaultMaterial))
            {
                return ((this.alphaTexLayout != ImageAlphaTexLayout.None) ? defaultMaterial : Graphic.defaultGraphicMaterial);
            }
            if (this.alphaTexLayout == ImageAlphaTexLayout.None)
            {
                return base.m_Material;
            }
            Material material = null;
            if (!s_materialList.TryGetValue(base.m_Material, out material))
            {
                material = new Material(base.m_Material) {
                    shaderKeywords = base.m_Material.shaderKeywords
                };
                material.EnableKeyword("_ALPHATEX_ON");
                s_materialList.Add(base.m_Material, material);
            }
            return material;
        }
    }

    public static Material defaultMaterial
    {
        get
        {
            if (s_defaultMaterial == null)
            {
                s_defaultMaterial = Resources.Load("Shaders/UI/Default2", typeof(Material)) as Material;
            }
            return s_defaultMaterial;
        }
    }

    public override Material material
    {
        get
        {
            Material baseMaterial = this.baseMaterial;
            this.UpdateInternalState();
            if (base.m_IncludeForMasking && (base.m_MaskMaterial == null))
            {
                base.m_MaskMaterial = StencilMaterial.Add(baseMaterial, (((int) 1) << base.m_StencilValue) - 1);
                if (base.m_MaskMaterial != null)
                {
                    base.m_MaskMaterial.shaderKeywords = baseMaterial.shaderKeywords;
                    return base.m_MaskMaterial;
                }
            }
            return baseMaterial;
        }
        set
        {
            base.material = value;
        }
    }

    public override float preferredHeight
    {
        get
        {
            float preferredHeight = base.preferredHeight;
            if (this.alphaTexLayout == ImageAlphaTexLayout.Vertical)
            {
                preferredHeight *= 0.5f;
            }
            return preferredHeight;
        }
    }

    public override float preferredWidth
    {
        get
        {
            float preferredWidth = base.preferredWidth;
            if (this.alphaTexLayout == ImageAlphaTexLayout.Horizonatal)
            {
                preferredWidth *= 0.5f;
            }
            return preferredWidth;
        }
    }
}

