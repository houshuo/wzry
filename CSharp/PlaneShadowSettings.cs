using System;
using UnityEngine;

public class PlaneShadowSettings : MonoBehaviour
{
    public float _fadeBegin = defaultFadeBegin;
    public float _fadeExp = defaultFadeExp;
    public float _intensity = defaultIntensity;
    public static float defaultFadeBegin = 0.1f;
    public static float defaultFadeExp = 4f;
    public static float defaultIntensity = 0.45f;
    public static Vector3 defaultProjDir;
    public static Vector4 shadowParams;
    public static Vector3 shadowProjDir;

    static PlaneShadowSettings()
    {
        Vector3 vector = new Vector3(1f, -1f, 1f);
        defaultProjDir = vector.normalized;
        shadowParams = new Vector4(defaultFadeBegin, defaultFadeExp, defaultIntensity, 0f);
        shadowProjDir = defaultProjDir;
    }

    public static void Apply()
    {
        Shader.SetGlobalVector("_ShadowProjDir", shadowProjDir.toVec4(1f));
        Shader.SetGlobalVector("_ShadowFadeParams", shadowParams);
    }

    public void ApplySettings()
    {
        this._fadeBegin = Mathf.Clamp01(this._fadeBegin);
        this._fadeExp = Mathf.Clamp(this._fadeExp, 0.01f, 16f);
        this._intensity = Mathf.Clamp(this._intensity, 0.01f, 4f);
        shadowProjDir = base.transform.forward.normalized;
        shadowParams = new Vector4(this._fadeBegin, this._fadeExp, this._intensity);
        Apply();
    }

    public static void SetDefault()
    {
        shadowParams = new Vector4(defaultFadeBegin, defaultFadeExp, defaultIntensity, 0f);
        shadowProjDir = defaultProjDir;
        Apply();
    }

    private void Start()
    {
        this.ApplySettings();
    }
}

