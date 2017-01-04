using System;
using UnityEngine;

public class FadeMaterialUtility
{
    public const string FadeParamName = "_FadeFactor";
    public const string FadeToken = " (Fade)";

    public static ListView<Material> GetFadeMaterials(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
        if ((componentsInChildren == null) || (componentsInChildren.Length == 0))
        {
            return null;
        }
        ListView<Material> view = new ListView<Material>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            Renderer renderer = componentsInChildren[i];
            if ((renderer != null) && (renderer.sharedMaterial != null))
            {
                if (renderer.sharedMaterial.HasProperty("_FadeFactor"))
                {
                    view.Add(renderer.material);
                }
                else
                {
                    Shader fadeShader = GetFadeShader(renderer.sharedMaterial.shader, true);
                    if (fadeShader != null)
                    {
                        renderer.material.shader = fadeShader;
                        view.Add(renderer.material);
                    }
                }
            }
        }
        return ((view.Count <= 0) ? null : view);
    }

    public static Shader GetFadeShader(Shader shader, bool withFade)
    {
        if (shader == null)
        {
            return null;
        }
        if (withFade)
        {
            if (shader.name.Contains(" (Fade)"))
            {
                return shader;
            }
            return Shader.Find(shader.name + " (Fade)");
        }
        int index = shader.name.IndexOf(" (Fade)");
        if (index != -1)
        {
            return Shader.Find(shader.name.Remove(index, " (Fade)".Length));
        }
        return shader;
    }
}

