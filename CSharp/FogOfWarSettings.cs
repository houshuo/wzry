using System;
using UnityEngine;

public class FogOfWarSettings : MonoBehaviour
{
    public Color color = defaultColor;
    public static Color defaultColor = new Color(0.1686275f, 0.1058824f, 0.3607843f);
    public static float defaultFadeDistance = 20f;
    public static float defaultFadeThreshold = 0.5f;
    public static float defaultFowIntensity = 0.6f;
    public static float defaultIntensity = 0.4f;
    public float fadeDistance = defaultFadeDistance;
    public float fadeThreshold = defaultFadeThreshold;
    public float fowIntensity = defaultFowIntensity;
    public float intensity = defaultIntensity;

    private static void Apply(Color _color, float _distance, float _threshold, float _intensity, float _fowIntensity)
    {
        Vector4 vec = new Vector4 {
            x = _color.r,
            y = _color.g,
            z = _color.b,
            w = 1f
        };
        Shader.SetGlobalVector("_FOWColor", vec);
        vec.x = 1f / Math.Max(0.01f, _distance);
        vec.y = -Mathf.Clamp01(_threshold);
        vec.z = _intensity;
        vec.w = _fowIntensity;
        Shader.SetGlobalVector("_FOWParams", vec);
    }

    public void ApplySettings()
    {
        Apply(this.color, this.fadeDistance, this.fadeThreshold, this.intensity, this.fowIntensity);
    }

    public static void SetDefault()
    {
        Apply(defaultColor, defaultFadeDistance, defaultFadeThreshold, defaultIntensity, defaultFowIntensity);
    }

    private void Start()
    {
        this.ApplySettings();
    }
}

