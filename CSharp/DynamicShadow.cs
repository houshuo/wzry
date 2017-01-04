using Assets.Scripts.Framework;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicShadow : MonoBehaviour
{
    private Material blurMat_;
    private Camera depthCam_;
    private Shader depthShader_;
    private RenderTexture[] depthTextures_;
    public LayerMask ShadowCastersMask = -1;
    public float ShadowIntensity = 0.5f;
    private static ListView<DynamicShadow> shadowList = new ListView<DynamicShadow>();
    private int shadowSize = 0x400;
    private bool supportShadow;
    private bool useDepthTex;

    private void CloseShadow()
    {
        if (this.depthTextures_ != null)
        {
            for (int i = 0; i < this.depthTextures_.Length; i++)
            {
                this.depthTextures_[i].Release();
                this.depthTextures_[i] = null;
            }
        }
        this.depthTextures_ = null;
        if (this.depthCam_ != null)
        {
            this.depthCam_.enabled = false;
        }
        this.depthCam_ = null;
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    public static void DisableAllDynamicShadows()
    {
        for (int i = 0; i < shadowList.Count; i++)
        {
            DynamicShadow shadow = shadowList[i];
            if (shadow != null)
            {
                shadow.CloseShadow();
            }
        }
        shadowList.Clear();
        Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
    }

    public static void EnableDynamicShow(GameObject go, bool enable)
    {
        if ((go != null) && (!enable || GameSettings.IsHighQuality))
        {
            DynamicShadow[] componentsInChildren = go.GetComponentsInChildren<DynamicShadow>();
            if ((componentsInChildren != null) && (componentsInChildren.Length != 0))
            {
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    DynamicShadow item = componentsInChildren[i];
                    if (enable)
                    {
                        item.InitShadow();
                        if (!shadowList.Contains(item))
                        {
                            shadowList.Add(item);
                        }
                    }
                    else
                    {
                        item.CloseShadow();
                        shadowList.Remove(item);
                    }
                }
                if (enable)
                {
                    Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                }
                else if (shadowList.Count == 0)
                {
                    Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                }
            }
        }
    }

    public static void InitDefaultGlobalVariables()
    {
        Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
        Shader.SetGlobalVector("_SGameShadowParams", new Vector4(-0.4862544f, -0.2707574f, 0.8308111f, 0.5f));
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    private void InitShadow()
    {
        if (this.depthTextures_ == null)
        {
            this.depthShader_ = Shader.Find("SGame_Post/ShadowDepth");
            this.blurMat_ = new Material(Shader.Find("SGame_Post/ShadowBlur"));
            this.useDepthTex = false;
            this.supportShadow = false;
            int num = 0;
            this.supportShadow = true;
            num = 1;
            if (this.supportShadow)
            {
                Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                RenderTextureFormat format = !this.useDepthTex ? RenderTextureFormat.ARGB32 : RenderTextureFormat.Depth;
                this.depthTextures_ = new RenderTexture[num];
                bool flag = this.SupportRGBABilinear();
                for (int i = 0; i < this.depthTextures_.Length; i++)
                {
                    this.depthTextures_[i] = new RenderTexture(this.shadowSize, this.shadowSize, 0x18, format, RenderTextureReadWrite.Default);
                    this.depthTextures_[i].generateMips = false;
                    this.depthTextures_[i].filterMode = !flag ? FilterMode.Point : FilterMode.Bilinear;
                }
                this.depthCam_ = base.GetComponent<Camera>();
                if (this.depthCam_ == null)
                {
                    this.depthCam_ = base.gameObject.AddComponent<Camera>();
                    this.depthCam_.transform.localPosition = new Vector3(0f, 0f, -5f);
                }
                this.depthCam_.enabled = true;
                this.depthCam_.targetTexture = this.depthTextures_[0];
                this.depthCam_.depth = -50f;
                this.depthCam_.farClipPlane = 100f;
                this.depthCam_.backgroundColor = new Color(1f, 1f, 1f, 1f);
                this.depthCam_.cullingMask = (int) this.ShadowCastersMask;
                this.depthCam_.clearFlags = CameraClearFlags.Color;
                this.depthCam_.SetReplacementShader(this.depthShader_, "RenderType");
            }
            else
            {
                Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
            }
        }
    }

    private void LateUpdate()
    {
        if (((this.depthCam_ != null) && (this.depthTextures_ != null)) && this.supportShadow)
        {
            Matrix4x4 mat = GL.GetGPUProjectionMatrix(this.depthCam_.projectionMatrix, false) * this.depthCam_.worldToCameraMatrix;
            Vector4 vec = base.transform.forward.normalized.toVec4(this.ShadowIntensity);
            Shader.SetGlobalMatrix("_SGameShadowMatrix", mat);
            Shader.SetGlobalVector("_SGameShadowParams", vec);
        }
    }

    private void OnDestroy()
    {
        this.CloseShadow();
    }

    private void OnPostRender()
    {
        if (((this.depthCam_ != null) && (this.depthTextures_ != null)) && this.supportShadow)
        {
            Shader.SetGlobalTexture("_SGameShadowTexture", this.depthTextures_[0]);
        }
    }

    private void OnPreRender()
    {
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    private void Start()
    {
    }

    private bool SupportHighpFloat()
    {
        string str = SystemInfo.graphicsDeviceName.ToLower();
        if (!str.Contains("adreno"))
        {
            if (str.Contains("powervr") || str.Contains("sgx"))
            {
                return true;
            }
            if (str.Contains("nvidia") || str.Contains("tegra"))
            {
                return true;
            }
            if (!str.Contains("mali") && !str.Contains("arm"))
            {
                return false;
            }
        }
        return true;
    }

    private bool SupportRGBABilinear()
    {
        string str = SystemInfo.graphicsDeviceName.ToLower();
        if (!str.Contains("adreno"))
        {
            if (str.Contains("powervr") || str.Contains("sgx"))
            {
                return false;
            }
            if (str.Contains("nvidia") || str.Contains("tegra"))
            {
                return false;
            }
            if (!str.Contains("mali") && !str.Contains("arm"))
            {
                return false;
            }
        }
        return true;
    }
}

