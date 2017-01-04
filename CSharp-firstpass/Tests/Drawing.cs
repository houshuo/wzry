namespace Tests
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class Drawing
    {
        private static Texture2D aaLineTex = null;
        private static Material blendMaterial = null;
        private static Material blitMaterial = null;
        private static Rect lineRect = new Rect(0f, 0f, 1f, 1f);
        private static Texture2D lineTex = null;

        static Drawing()
        {
            Initialize();
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias = true)
        {
            float num = pointB.x - pointA.x;
            float num2 = pointB.y - pointA.y;
            float num3 = Mathf.Sqrt((num * num) + (num2 * num2));
            if (num3 >= 0.001f)
            {
                Texture2D aaLineTex;
                Material blendMaterial;
                if (antiAlias)
                {
                    width *= 3f;
                    aaLineTex = Drawing.aaLineTex;
                    blendMaterial = Drawing.blendMaterial;
                }
                else
                {
                    aaLineTex = lineTex;
                    blendMaterial = blitMaterial;
                }
                float num4 = (width * num2) / num3;
                float num5 = (width * num) / num3;
                Matrix4x4 identity = Matrix4x4.identity;
                identity.m00 = num;
                identity.m01 = -num4;
                identity.m03 = pointA.x + (0.5f * num4);
                identity.m10 = num2;
                identity.m11 = num5;
                identity.m13 = pointA.y - (0.5f * num5);
                GL.PushMatrix();
                GL.MultMatrix(identity);
                Graphics.DrawTexture(lineRect, aaLineTex, lineRect, 0, 0, 0, 0, color, blendMaterial);
                GL.PopMatrix();
            }
        }

        private static void Initialize()
        {
            if (lineTex == null)
            {
                lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                lineTex.SetPixel(0, 1, Color.white);
                lineTex.Apply();
            }
            if (aaLineTex == null)
            {
                aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
                aaLineTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 0f));
                aaLineTex.SetPixel(0, 1, Color.white);
                aaLineTex.SetPixel(0, 2, new Color(1f, 1f, 1f, 0f));
                aaLineTex.Apply();
            }
            blitMaterial = (Material) typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            blendMaterial = (Material) typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        }
    }
}

