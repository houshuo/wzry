using Assets.Scripts.Framework;
using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class OutlineFilter : MonoBehaviour
{
    public float blendFactor = 0.5f;
    public bool clearAlpha;
    private Material clearAlphaMat;
    public int filterType = 1;
    private Material material;
    [NonSerialized, HideInInspector]
    public Camera particlesCam;
    private static bool s_isRenderingParticles;

    private void ClearAlpha()
    {
        if ((this.clearAlphaMat != null) && (base.GetComponent<Camera>() != null))
        {
            GL.PushMatrix();
            this.clearAlphaMat.SetPass(0);
            GL.LoadOrtho();
            GL.Viewport(base.GetComponent<Camera>().pixelRect);
            GL.Begin(7);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(0f, 1f, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.End();
            GL.PopMatrix();
        }
    }

    public static void DisableOutlineFilter()
    {
        string[] layerNames = new string[] { "Particles" };
        int mask = LayerMask.GetMask(layerNames);
        foreach (Camera camera in UnityEngine.Object.FindObjectsOfType<Camera>())
        {
            if (camera != null)
            {
                OutlineFilter component = camera.GetComponent<OutlineFilter>();
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component);
                    camera.cullingMask |= mask;
                    foreach (Camera camera2 in camera.GetComponentsInChildren<Camera>())
                    {
                        if ((camera2 != null) && (camera2 != camera))
                        {
                            GameObject gameObject = camera2.gameObject;
                            UnityEngine.Object.Destroy(camera2);
                            UnityEngine.Object.Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }

    private void drawParticles(RenderTexture colorRT, RenderTexture depthRT)
    {
        if (this.particlesCam != null)
        {
            try
            {
                this.particlesCam.enabled = true;
                RenderTexture targetTexture = this.particlesCam.targetTexture;
                this.particlesCam.SetTargetBuffers(colorRT.colorBuffer, depthRT.depthBuffer);
                this.particlesCam.Render();
                this.particlesCam.targetTexture = targetTexture;
                this.particlesCam.enabled = false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    public static void EnableOutlineFilter()
    {
        string[] layerNames = new string[] { "Scene" };
        int mask = LayerMask.GetMask(layerNames);
        string[] textArray2 = new string[] { "Particles" };
        int num2 = LayerMask.GetMask(textArray2);
        foreach (Camera camera in UnityEngine.Object.FindObjectsOfType<Camera>())
        {
            if ((camera.cullingMask & mask) != 0)
            {
                OutlineFilter filter = camera.gameObject.AddComponent<OutlineFilter>();
                camera.cullingMask &= ~num2;
                Camera camera2 = new GameObject { transform = { parent = camera.transform, localPosition = Vector3.zero, localRotation = Quaternion.identity }, name = camera.name + " particles" }.AddComponent<Camera>();
                camera2.aspect = camera.aspect;
                camera2.backgroundColor = camera.backgroundColor;
                camera2.nearClipPlane = camera.nearClipPlane;
                camera2.farClipPlane = camera.farClipPlane;
                camera2.fieldOfView = camera.fieldOfView;
                camera2.orthographic = camera.orthographic;
                camera2.orthographicSize = camera.orthographicSize;
                camera2.pixelRect = camera.pixelRect;
                camera2.rect = camera.rect;
                camera2.clearFlags = CameraClearFlags.Nothing;
                camera2.cullingMask = num2;
                camera2.enabled = false;
                filter.particlesCam = camera2;
                filter.UpdateFilterType(false);
            }
        }
    }

    public static void EnableSurfaceShaderOutline(bool enable)
    {
        Shader.SetGlobalFloat("_SGamelGlobalAlphaModifier", !enable ? 1f : 0f);
    }

    public void LoadShaders()
    {
        if (this.material == null)
        {
            string name = "SGame_Post/OutlineFilter";
            Shader shader = Shader.Find(name);
            if (shader == null)
            {
            }
            this.material = new Material(shader);
            this.material.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
        }
        if (this.clearAlphaMat == null)
        {
            string str2 = "SGame_Post/ClearAlpha";
            Shader shader2 = Shader.Find(str2);
            if (shader2 == null)
            {
            }
            this.clearAlphaMat = new Material(shader2);
            this.clearAlphaMat.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!s_isRenderingParticles)
        {
            if (destination == null)
            {
                RenderTexture dest = RenderTexture.GetTemporary(source.width, source.height, 0);
                source.filterMode = FilterMode.Point;
                Graphics.Blit(source, dest, this.material, 0);
                this.drawParticles(dest, source);
                Graphics.Blit(dest, destination);
                RenderTexture.ReleaseTemporary(dest);
            }
            else
            {
                Graphics.Blit(source, destination, this.material, 0);
                this.drawParticles(destination, source);
            }
        }
    }

    private void Start()
    {
        this.LoadShaders();
        this.UpdateParameters();
    }

    public void UpdateFilterType(bool heroShow)
    {
        int num = Mathf.Max(Screen.width, Screen.height);
        int num2 = Mathf.Min(Screen.width, Screen.height);
        bool flag = ((num >= 0x640) || (num2 >= 900)) || (Screen.dpi >= 350f);
        bool flag2 = ((num >= 0x470) || (num2 >= 640)) || (Screen.dpi >= 300f);
        if (GameSettings.ShouldReduceResolution())
        {
            flag = false;
            flag2 = true;
        }
        if (flag)
        {
            this.filterType = 2;
            this.blendFactor = 0.55f;
        }
        else if (flag2)
        {
            this.filterType = 1;
            this.blendFactor = 0.5f;
        }
        else
        {
            this.filterType = 1;
            this.blendFactor = 0.5f;
        }
        if (heroShow)
        {
            this.filterType++;
        }
        this.LoadShaders();
        this.UpdateParameters();
    }

    public void UpdateParameters()
    {
        if ((this.material != null) && (Camera.main != null))
        {
            this.material.SetFloat("_BlendFactor", this.blendFactor);
            float z = 1f / Camera.main.get_pixelWidth();
            float w = 1f / Camera.main.get_pixelHeight();
            if (this.filterType == 0)
            {
                Vector4 vector = new Vector4(-z, 0f, z, 0f);
                Vector4 vector2 = new Vector4(0f, -w, 0f, w);
                this.material.SetVector("_TexelOffset0", vector);
                this.material.SetVector("_TexelOffset1", vector2);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 1)
            {
                Vector4 vector3 = new Vector4(-z, w, z, w);
                Vector4 vector4 = new Vector4(-z, -w, z, -w);
                this.material.SetVector("_TexelOffset0", vector3);
                this.material.SetVector("_TexelOffset1", vector4);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 2)
            {
                z *= 2f;
                w *= 2f;
                Vector4 vector5 = new Vector4(-z, 0f, z, 0f);
                Vector4 vector6 = new Vector4(0f, -w, 0f, w);
                this.material.SetVector("_TexelOffset0", vector5);
                this.material.SetVector("_TexelOffset1", vector6);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 3)
            {
                z *= 2f;
                w *= 2f;
                Vector4 vector7 = new Vector4(-z, w, z, w);
                Vector4 vector8 = new Vector4(-z, -w, z, -w);
                this.material.SetVector("_TexelOffset0", vector7);
                this.material.SetVector("_TexelOffset1", vector8);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else
            {
                Vector4 vector9 = new Vector4(-z, 0f, z, 0f);
                Vector4 vector10 = new Vector4(0f, -w, 0f, w);
                Vector4 vector11 = new Vector4(-z, w, z, w);
                Vector4 vector12 = new Vector4(-z, -w, z, -w);
                this.material.SetVector("_TexelOffset0", vector9);
                this.material.SetVector("_TexelOffset1", vector10);
                this.material.SetVector("_TexelOffset2", vector11);
                this.material.SetVector("_TexelOffset3", vector12);
                this.material.EnableKeyword("_HIGHQUALITY_ON");
            }
        }
    }
}

