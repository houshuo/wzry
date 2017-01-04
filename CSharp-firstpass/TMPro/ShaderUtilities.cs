namespace TMPro
{
    using System;
    using UnityEngine;

    public static class ShaderUtilities
    {
        public static int ID_BevelAmount;
        public static int ID_EnvMap;
        public static int ID_EnvMatrix;
        public static int ID_EnvMatrixRotation;
        public static int ID_FaceColor;
        public static int ID_FaceDilate;
        public static int ID_FaceTex;
        public static int ID_GlowColor;
        public static int ID_GlowOffset;
        public static int ID_GlowOuter;
        public static int ID_GlowPower;
        public static int ID_GradientScale;
        public static int ID_LightAngle;
        public static int ID_MainTex;
        public static int ID_MaskCoord;
        public static int ID_MaskID;
        public static int ID_MaskSoftnessX;
        public static int ID_MaskSoftnessY;
        public static int ID_OutlineColor;
        public static int ID_OutlineSoftness;
        public static int ID_OutlineTex;
        public static int ID_OutlineWidth;
        public static int ID_PerspectiveFilter;
        public static int ID_ScaleRatio_A;
        public static int ID_ScaleRatio_B;
        public static int ID_ScaleRatio_C;
        public static int ID_ScaleX;
        public static int ID_ScaleY;
        public static int ID_ShaderFlags;
        public static int ID_Shininess;
        public static int ID_StencilComp;
        public static int ID_StencilID;
        public static int ID_TextureHeight;
        public static int ID_TextureWidth;
        public static int ID_UnderlayColor;
        public static int ID_UnderlayDilate;
        public static int ID_UnderlayOffsetX;
        public static int ID_UnderlayOffsetY;
        public static int ID_UnderlaySoftness;
        public static int ID_VertexOffsetX;
        public static int ID_VertexOffsetY;
        public static int ID_WeightBold;
        public static int ID_WeightNormal;
        public static bool isInitialized;
        public static string Keyword_Bevel = "BEVEL_ON";
        public static string Keyword_Glow = "GLOW_ON";
        public static string Keyword_MASK_HARD = "MASK_HARD";
        public static string Keyword_MASK_OFF = "MASK_OFF";
        public static string Keyword_MASK_SOFT = "MASK_SOFT";
        public static string Keyword_Ratios = "RATIOS_OFF";
        public static string Keyword_Underlay = "UNDERLAY_ON";
        private static float m_clamp = 1f;
        public static string ShaderTag_CullMode = "_CullMode";
        public static string ShaderTag_ZTestMode = "_ZTestMode";

        public static Vector4 GetFontExtent(Material material)
        {
            if ((material == null) || !material.HasProperty(ID_GradientScale))
            {
                return Vector4.zero;
            }
            float @float = material.GetFloat(ID_ScaleRatio_A);
            float num2 = material.GetFloat(ID_FaceDilate) * @float;
            float num3 = material.GetFloat(ID_OutlineWidth) * @float;
            float x = Mathf.Min((float) 1f, (float) (num2 + num3)) * material.GetFloat(ID_GradientScale);
            return new Vector4(x, x, x, x);
        }

        public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
        {
            if (!isInitialized)
            {
                GetShaderPropertyIDs();
            }
            if (material == null)
            {
                return 0f;
            }
            int num = !enableExtraPadding ? 0 : 4;
            if (!material.HasProperty(ID_GradientScale))
            {
                return (float) num;
            }
            Vector4 zero = Vector4.zero;
            Vector4 vector2 = Vector4.zero;
            float num2 = 0f;
            float num3 = 0f;
            float num4 = 0f;
            float num5 = 0f;
            float num6 = 0f;
            float num7 = 0f;
            float num8 = 0f;
            float num9 = 0f;
            float a = 0f;
            UpdateShaderRatios(material, isBold);
            string[] shaderKeywords = material.shaderKeywords;
            if (material.HasProperty(ID_ScaleRatio_A))
            {
                num5 = material.GetFloat(ID_ScaleRatio_A);
            }
            if (material.HasProperty(ID_FaceDilate))
            {
                num2 = material.GetFloat(ID_FaceDilate) * num5;
            }
            if (material.HasProperty(ID_OutlineSoftness))
            {
                num3 = material.GetFloat(ID_OutlineSoftness) * num5;
            }
            if (material.HasProperty(ID_OutlineWidth))
            {
                num4 = material.GetFloat(ID_OutlineWidth) * num5;
            }
            a = (num4 + num3) + num2;
            if (material.HasProperty(ID_GlowOffset) && StringContains(shaderKeywords, Keyword_Glow))
            {
                if (material.HasProperty(ID_ScaleRatio_B))
                {
                    num6 = material.GetFloat(ID_ScaleRatio_B);
                }
                num8 = material.GetFloat(ID_GlowOffset) * num6;
                num9 = material.GetFloat(ID_GlowOuter) * num6;
            }
            a = Mathf.Max(a, (num2 + num8) + num9);
            if (material.HasProperty(ID_UnderlaySoftness) && StringContains(shaderKeywords, Keyword_Underlay))
            {
                if (material.HasProperty(ID_ScaleRatio_C))
                {
                    num7 = material.GetFloat(ID_ScaleRatio_C);
                }
                float num11 = material.GetFloat(ID_UnderlayOffsetX) * num7;
                float num12 = material.GetFloat(ID_UnderlayOffsetY) * num7;
                float num13 = material.GetFloat(ID_UnderlayDilate) * num7;
                float num14 = material.GetFloat(ID_UnderlaySoftness) * num7;
                zero.x = Mathf.Max(zero.x, ((num2 + num13) + num14) - num11);
                zero.y = Mathf.Max(zero.y, ((num2 + num13) + num14) - num12);
                zero.z = Mathf.Max(zero.z, ((num2 + num13) + num14) + num11);
                zero.w = Mathf.Max(zero.w, ((num2 + num13) + num14) + num12);
            }
            zero.x = Mathf.Max(zero.x, a);
            zero.y = Mathf.Max(zero.y, a);
            zero.z = Mathf.Max(zero.z, a);
            zero.w = Mathf.Max(zero.w, a);
            zero.x += num;
            zero.y += num;
            zero.z += num;
            zero.w += num;
            zero.x = Mathf.Min(zero.x, 1f);
            zero.y = Mathf.Min(zero.y, 1f);
            zero.z = Mathf.Min(zero.z, 1f);
            zero.w = Mathf.Min(zero.w, 1f);
            vector2.x = (vector2.x >= zero.x) ? vector2.x : zero.x;
            vector2.y = (vector2.y >= zero.y) ? vector2.y : zero.y;
            vector2.z = (vector2.z >= zero.z) ? vector2.z : zero.z;
            vector2.w = (vector2.w >= zero.w) ? vector2.w : zero.w;
            float @float = material.GetFloat(ID_GradientScale);
            zero = (Vector4) (zero * @float);
            a = Mathf.Max(zero.x, zero.y);
            a = Mathf.Max(zero.z, a);
            return (Mathf.Max(zero.w, a) + 0.25f);
        }

        public static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
        {
            if (!isInitialized)
            {
                GetShaderPropertyIDs();
            }
            if (materials == null)
            {
                return 0f;
            }
            int num = !enableExtraPadding ? 0 : 4;
            if (!materials[0].HasProperty(ID_GradientScale))
            {
                return (float) num;
            }
            Vector4 zero = Vector4.zero;
            Vector4 vector2 = Vector4.zero;
            float num2 = 0f;
            float num3 = 0f;
            float num4 = 0f;
            float num5 = 0f;
            float num6 = 0f;
            float num7 = 0f;
            float num8 = 0f;
            float num9 = 0f;
            float a = 0f;
            for (int i = 0; i < materials.Length; i++)
            {
                UpdateShaderRatios(materials[i], isBold);
                string[] shaderKeywords = materials[i].shaderKeywords;
                if (materials[i].HasProperty(ID_ScaleRatio_A))
                {
                    num5 = materials[i].GetFloat(ID_ScaleRatio_A);
                }
                if (materials[i].HasProperty(ID_FaceDilate))
                {
                    num2 = materials[i].GetFloat(ID_FaceDilate) * num5;
                }
                if (materials[i].HasProperty(ID_OutlineSoftness))
                {
                    num3 = materials[i].GetFloat(ID_OutlineSoftness) * num5;
                }
                if (materials[i].HasProperty(ID_OutlineWidth))
                {
                    num4 = materials[i].GetFloat(ID_OutlineWidth) * num5;
                }
                a = (num4 + num3) + num2;
                if (materials[i].HasProperty(ID_GlowOffset) && StringContains(shaderKeywords, Keyword_Glow))
                {
                    if (materials[i].HasProperty(ID_ScaleRatio_B))
                    {
                        num6 = materials[i].GetFloat(ID_ScaleRatio_B);
                    }
                    num8 = materials[i].GetFloat(ID_GlowOffset) * num6;
                    num9 = materials[i].GetFloat(ID_GlowOuter) * num6;
                }
                a = Mathf.Max(a, (num2 + num8) + num9);
                if (materials[i].HasProperty(ID_UnderlaySoftness) && StringContains(shaderKeywords, Keyword_Underlay))
                {
                    if (materials[i].HasProperty(ID_ScaleRatio_C))
                    {
                        num7 = materials[i].GetFloat(ID_ScaleRatio_C);
                    }
                    float num12 = materials[i].GetFloat(ID_UnderlayOffsetX) * num7;
                    float num13 = materials[i].GetFloat(ID_UnderlayOffsetY) * num7;
                    float num14 = materials[i].GetFloat(ID_UnderlayDilate) * num7;
                    float num15 = materials[i].GetFloat(ID_UnderlaySoftness) * num7;
                    zero.x = Mathf.Max(zero.x, ((num2 + num14) + num15) - num12);
                    zero.y = Mathf.Max(zero.y, ((num2 + num14) + num15) - num13);
                    zero.z = Mathf.Max(zero.z, ((num2 + num14) + num15) + num12);
                    zero.w = Mathf.Max(zero.w, ((num2 + num14) + num15) + num13);
                }
                zero.x = Mathf.Max(zero.x, a);
                zero.y = Mathf.Max(zero.y, a);
                zero.z = Mathf.Max(zero.z, a);
                zero.w = Mathf.Max(zero.w, a);
                zero.x += num;
                zero.y += num;
                zero.z += num;
                zero.w += num;
                zero.x = Mathf.Min(zero.x, 1f);
                zero.y = Mathf.Min(zero.y, 1f);
                zero.z = Mathf.Min(zero.z, 1f);
                zero.w = Mathf.Min(zero.w, 1f);
                vector2.x = (vector2.x >= zero.x) ? vector2.x : zero.x;
                vector2.y = (vector2.y >= zero.y) ? vector2.y : zero.y;
                vector2.z = (vector2.z >= zero.z) ? vector2.z : zero.z;
                vector2.w = (vector2.w >= zero.w) ? vector2.w : zero.w;
            }
            float @float = materials[0].GetFloat(ID_GradientScale);
            zero = (Vector4) (zero * @float);
            a = Mathf.Max(zero.x, zero.y);
            a = Mathf.Max(zero.z, a);
            return (Mathf.Max(zero.w, a) + 0.25f);
        }

        public static void GetShaderPropertyIDs()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ID_MainTex = Shader.PropertyToID("_MainTex");
                ID_FaceTex = Shader.PropertyToID("_FaceTex");
                ID_FaceColor = Shader.PropertyToID("_FaceColor");
                ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
                ID_Shininess = Shader.PropertyToID("_FaceShininess");
                ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
                ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
                ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
                ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
                ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
                ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
                ID_WeightBold = Shader.PropertyToID("_WeightBold");
                ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
                ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
                ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
                ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
                ID_GradientScale = Shader.PropertyToID("_GradientScale");
                ID_ScaleX = Shader.PropertyToID("_ScaleX");
                ID_ScaleY = Shader.PropertyToID("_ScaleY");
                ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
                ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
                ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
                ID_BevelAmount = Shader.PropertyToID("_Bevel");
                ID_LightAngle = Shader.PropertyToID("_LightAngle");
                ID_EnvMap = Shader.PropertyToID("_Cube");
                ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
                ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
                ID_GlowColor = Shader.PropertyToID("_GlowColor");
                ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
                ID_GlowPower = Shader.PropertyToID("_GlowPower");
                ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
                ID_MaskID = Shader.PropertyToID("_MaskID");
                ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
                ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
                ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
                ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
                ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
                ID_StencilID = Shader.PropertyToID("_Stencil");
                ID_StencilComp = Shader.PropertyToID("_StencilComp");
                ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
                ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
                ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
                ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
            }
        }

        public static bool IsMaskingEnabled(Material material)
        {
            if ((material == null) || !material.HasProperty(ID_MaskCoord))
            {
                return false;
            }
            if (!StringContains(material.shaderKeywords, Keyword_MASK_SOFT) && !StringContains(material.shaderKeywords, Keyword_MASK_HARD))
            {
                return false;
            }
            return true;
        }

        public static bool StringContains(string[] strs, string key)
        {
            if (strs != null)
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    if (key == strs[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void UpdateShaderRatios(Material mat, bool isBold)
        {
            float num = 1f;
            float num2 = 1f;
            float num3 = 1f;
            bool flag = !StringContains(mat.shaderKeywords, Keyword_Ratios);
            float @float = mat.GetFloat(ID_GradientScale);
            float num5 = mat.GetFloat(ID_FaceDilate);
            float num6 = mat.GetFloat(ID_OutlineWidth);
            float num7 = mat.GetFloat(ID_OutlineSoftness);
            float num8 = isBold ? ((mat.GetFloat(ID_WeightBold) * 2f) / @float) : ((mat.GetFloat(ID_WeightNormal) * 2f) / @float);
            float num9 = Mathf.Max((float) 1f, (float) (((num8 + num5) + num6) + num7));
            num = !flag ? 1f : ((@float - m_clamp) / (@float * num9));
            mat.SetFloat(ID_ScaleRatio_A, num);
            if (mat.HasProperty(ID_GlowOffset))
            {
                float num10 = mat.GetFloat(ID_GlowOffset);
                float num11 = mat.GetFloat(ID_GlowOuter);
                float num12 = (num8 + num5) * (@float - m_clamp);
                num9 = Mathf.Max((float) 1f, (float) (num10 + num11));
                num2 = !flag ? 1f : (Mathf.Max((float) 0f, (float) ((@float - m_clamp) - num12)) / (@float * num9));
                mat.SetFloat(ID_ScaleRatio_B, num2);
            }
            if (mat.HasProperty(ID_UnderlayOffsetX))
            {
                float f = mat.GetFloat(ID_UnderlayOffsetX);
                float num14 = mat.GetFloat(ID_UnderlayOffsetY);
                float num15 = mat.GetFloat(ID_UnderlayDilate);
                float num16 = mat.GetFloat(ID_UnderlaySoftness);
                float num17 = (num8 + num5) * (@float - m_clamp);
                num9 = Mathf.Max((float) 1f, (float) ((Mathf.Max(Mathf.Abs(f), Mathf.Abs(num14)) + num15) + num16));
                num3 = !flag ? 1f : (Mathf.Max((float) 0f, (float) ((@float - m_clamp) - num17)) / (@float * num9));
                mat.SetFloat(ID_ScaleRatio_C, num3);
            }
        }
    }
}

