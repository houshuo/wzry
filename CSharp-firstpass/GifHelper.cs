using Gif;
using System;
using System.IO;
using UnityEngine;

public class GifHelper
{
    public static Texture2D GifToTexture(Stream stream, int frameIndex)
    {
        Loader loader = new Loader();
        if (!loader.Load(stream))
        {
            return null;
        }
        int width = loader._logical_screen_desc.image_width;
        int height = loader._logical_screen_desc.image_height;
        int num3 = width * height;
        UnityEngine.Color[] colors = new UnityEngine.Color[num3];
        Gif.Color[] colorArray2 = loader._global_color_table;
        int index = loader._logical_screen_desc.background_color;
        bool flag = loader._frames[0]._GCE_data.transparent_color_flag;
        UnityEngine.Color clear = UnityEngine.Color.clear;
        if ((!flag && (colorArray2 != null)) && (index < colorArray2.Length))
        {
            clear.r = ((float) colorArray2[index].r) / 255f;
            clear.g = ((float) colorArray2[index].g) / 255f;
            clear.b = ((float) colorArray2[index].b) / 255f;
            clear.a = 1f;
        }
        for (int i = 0; i < num3; i++)
        {
            colors[i] = clear;
        }
        GifFrame frame = loader._frames[frameIndex];
        int num6 = frame._image.desc.image_left;
        int num7 = frame._image.desc.image_top;
        int num8 = frame._image.desc.image_width;
        int num9 = frame._image.desc.image_height;
        if (frame._image.desc.local_color_table_flag)
        {
            colorArray2 = frame._image.desc.local_color_table;
        }
        int length = frame._image.data.Length;
        bool flag2 = frame._GCE_data.transparent_color_flag;
        int num11 = frame._GCE_data.transparent_color;
        for (int j = 0; j < length; j++)
        {
            int num13 = frame._image.data[j];
            if (!flag2 || (num11 != num13))
            {
                Gif.Color color2 = colorArray2[num13];
                colors[j].r = ((float) color2.r) / 255f;
                colors[j].g = ((float) color2.g) / 255f;
                colors[j].b = ((float) color2.b) / 255f;
                colors[j].a = 1f;
            }
        }
        Texture2D textured = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textured.SetPixels(colors);
        textured.Apply();
        return textured;
    }
}

