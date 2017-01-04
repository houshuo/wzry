using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UpdateShadowPlane : ActorComponent
{
    [CompilerGenerated]
    private static UnityExtension.FindChildDelegate <>f__am$cache5;
    [CompilerGenerated]
    private static UnityExtension.FindChildDelegate <>f__am$cache6;
    public const string c_planeShadowPath = "Prefab_Characters/PlaneShadow.prefab";
    public float height = 1.5f;
    private float lastUpdateGroundY = -1000f;
    private ListView<Material> mats;
    private static Dictionary<Mesh, float> meshHeightMap = new Dictionary<Mesh, float>();
    private GameObject shadowMesh;

    private void AddShadowedMat(ref ListView<Material> mats, Material m, bool useShadow)
    {
        if (useShadow)
        {
            if (mats == null)
            {
                mats = new ListView<Material>();
            }
            mats.Add(m);
        }
    }

    public void ApplyShadowSettings()
    {
        switch (GameSettings.ShadowQuality)
        {
            case SGameRenderQuality.High:
                this.EnableDynamicShadow();
                if ((this.shadowMesh != null) && (this.mats != null))
                {
                    this.shadowMesh.CustomSetActive(false);
                }
                break;

            case SGameRenderQuality.Medium:
                this.DisableDynamicShadow();
                this.EnablePlaneShadow();
                break;

            case SGameRenderQuality.Low:
                this.DisableDynamicShadow();
                if (this.shadowMesh != null)
                {
                    this.shadowMesh.CustomSetActive(false);
                }
                break;
        }
    }

    public override void Born(ActorRoot owner)
    {
        base.Born(owner);
        if (this.mats != null)
        {
            this.mats.Clear();
        }
        if (<>f__am$cache6 == null)
        {
            <>f__am$cache6 = delegate (GameObject obj) {
                if (!(obj.name == "Shadow") && (obj.name.IndexOf("Shadow_") != 0))
                {
                    return false;
                }
                return true;
            };
        }
        this.shadowMesh = base.gameObject.FindChildBFS(<>f__am$cache6);
        this.ApplyShadowSettings();
    }

    public void CalcHeight()
    {
        if ((GameSettings.IsHighQuality && (this.mats != null)) && (this.mats.Count != 0))
        {
            bool posUpdated = false;
            float y = base.gameObject.transform.lossyScale.y;
            float invScale = 1f / y;
            this.height = -1f;
            SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Mesh sharedMesh = componentsInChildren[i].sharedMesh;
                if (sharedMesh != null)
                {
                    float num4 = 0f;
                    if (!meshHeightMap.TryGetValue(sharedMesh, out num4))
                    {
                        this.CalcHeight(componentsInChildren[i], ref posUpdated, ref num4, invScale);
                    }
                    else
                    {
                        num4 *= y;
                    }
                    this.height = Mathf.Max(this.height, num4 * 1.5f);
                }
            }
            if (this.height < 0f)
            {
                this.height = 1.5f;
            }
        }
    }

    public void CalcHeight(SkinnedMeshRenderer renderer, ref bool posUpdated, ref float meshHeight, float invScale)
    {
        if (!posUpdated)
        {
            this.SetIdlePos();
            posUpdated = true;
        }
        Transform[] bones = renderer.bones;
        if ((bones != null) && (bones.Length != 0))
        {
            float y = base.gameObject.transform.position.y;
            float maxValue = float.MaxValue;
            float minValue = float.MinValue;
            float a = 0f;
            for (int i = 0; i < bones.Length; i++)
            {
                float num6 = bones[i].position.y;
                maxValue = Mathf.Min(num6, maxValue);
                minValue = Mathf.Max(num6, minValue);
                a = Mathf.Max(a, num6 - y);
            }
            a = Mathf.Max(a, minValue - maxValue);
            meshHeight = Mathf.Max(a, meshHeight);
            meshHeightMap.Add(renderer.sharedMesh, a * invScale);
        }
    }

    private void CheckMaterials()
    {
        if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))
        {
            SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if ((componentsInChildren != null) && (componentsInChildren.Length != 0))
            {
                try
                {
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        SkinnedMeshRenderer renderer = componentsInChildren[i];
                        if (!HeroMaterialUtility.IsHeroBattleShader(renderer.sharedMaterial))
                        {
                            Shader shader = Shader.Find("S_Game_Hero/Hero_Battle");
                            if (shader != null)
                            {
                                renderer.material.shader = shader;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public static void ClearCache()
    {
        meshHeightMap.Clear();
    }

    public void DisableDynamicShadow()
    {
        if (this.mats != null)
        {
            for (int i = 0; i < this.mats.Count; i++)
            {
                bool flag;
                bool flag2;
                bool flag3;
                Material material = this.mats[i];
                HeroMaterialUtility.GetShaderProperty(material.shader.name, out flag, out flag2, out flag3);
                Shader shader = Shader.Find(HeroMaterialUtility.MakeShaderName(material.shader.name, false, flag2, false));
                if (shader != null)
                {
                    material.shader = shader;
                }
            }
            this.mats = null;
        }
    }

    public void EnableDynamicShadow()
    {
        if (this.mats == null)
        {
            bool occlusion = false;
            bool shadow = true;
            foreach (Renderer renderer in base.gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer.gameObject != this.shadowMesh)
                {
                    Material sharedMaterial = renderer.sharedMaterial;
                    if (HeroMaterialUtility.IsHeroBattleShader(sharedMaterial))
                    {
                        bool flag3;
                        bool flag4;
                        bool flag5;
                        HeroMaterialUtility.GetShaderProperty(sharedMaterial.shader.name, out flag3, out flag4, out flag5);
                        if ((shadow != flag3) || (flag5 != occlusion))
                        {
                            Shader shader = Shader.Find(HeroMaterialUtility.MakeShaderName(sharedMaterial.shader.name, shadow, flag4, occlusion));
                            if (shader != null)
                            {
                                renderer.material.shader = shader;
                                this.AddShadowedMat(ref this.mats, renderer.material, shadow);
                            }
                        }
                        else
                        {
                            this.AddShadowedMat(ref this.mats, renderer.material, shadow);
                        }
                    }
                }
            }
            this.CalcHeight();
        }
    }

    private void EnablePlaneShadow()
    {
        if (this.shadowMesh == null)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (GameObject obj) {
                    if (!(obj.name == "Shadow") && (obj.name.IndexOf("Shadow_") != 0))
                    {
                        return false;
                    }
                    return true;
                };
            }
            this.shadowMesh = base.gameObject.FindChildBFS(<>f__am$cache5);
            if (this.shadowMesh == null)
            {
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/PlaneShadow.prefab", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
                this.shadowMesh = UnityEngine.Object.Instantiate(content) as GameObject;
                if (this.shadowMesh != null)
                {
                    Transform transform = this.shadowMesh.transform;
                    transform.SetParent(base.gameObject.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    if ((base.actor != null) && (base.actor.CharInfo != null))
                    {
                        float num = ((float) base.actor.CharInfo.iCollisionSize.x) / 400f;
                        num = Mathf.Clamp(num, 0.5f, 2f);
                        transform.localScale = new Vector3(num, num, num);
                    }
                }
            }
        }
        if (this.shadowMesh != null)
        {
            this.shadowMesh.CustomSetActive(true);
        }
    }

    public static void Preload(ref ActorPreloadTab result)
    {
        result.AddMesh("Prefab_Characters/PlaneShadow.prefab");
    }

    private bool SetIdlePos()
    {
        Animation componentInChildren = base.gameObject.GetComponentInChildren<Animation>();
        if (componentInChildren == null)
        {
            return false;
        }
        AnimationClip clip = componentInChildren.GetClip("Idle");
        if (clip == null)
        {
            return false;
        }
        base.gameObject.SampleAnimation(clip, 0f);
        return true;
    }

    public void Update()
    {
        if ((base.actor != null) && base.actor.Visible)
        {
            float scalar = base.actor.groundY.scalar;
            if (this.lastUpdateGroundY != scalar)
            {
                if ((this.shadowMesh != null) && this.shadowMesh.activeInHierarchy)
                {
                    Transform transform = this.shadowMesh.transform;
                    Vector3 position = transform.position;
                    position.y = scalar;
                    transform.position = position;
                }
                this.lastUpdateGroundY = scalar;
            }
            if ((this.mats != null) && (this.mats.Count > 0))
            {
                Vector4 vector2;
                vector2 = new Vector4(0f, 1f, 0f, 0f) {
                    w = scalar,
                    w = vector2.w + 0.05f
                };
                float num2 = Mathf.Abs((float) (PlaneShadowSettings.shadowProjDir.y / Mathf.Max(0.001f, this.height)));
                for (int i = 0; i < this.mats.Count; i++)
                {
                    Material material = this.mats[i];
                    material.SetVector("_ShadowPlane", vector2);
                    material.SetFloat("_ShadowInvLen", num2);
                    material.SetVector("_WorldPos", base.gameObject.transform.position.toVec4(1f));
                }
            }
        }
    }
}

