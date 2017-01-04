using System;
using System.Collections.Generic;
using UnityEngine;

public class NcEffectBehaviour : MonoBehaviour
{
    private static bool m_bShuttingDown;
    public float m_fUserTag;
    protected MeshFilter m_MeshFilter = null;
    protected Renderer m_renderer;
    private static GameObject m_RootInstance;
    protected ListView<Material> m_RuntimeMaterials;

    protected void AddRuntimeMaterial(Material addMaterial)
    {
        if (this.m_RuntimeMaterials == null)
        {
            this.m_RuntimeMaterials = new ListView<Material>();
        }
        if (!this.m_RuntimeMaterials.Contains(addMaterial))
        {
            this.m_RuntimeMaterials.Add(addMaterial);
        }
    }

    public static void AdjustSpeedRuntime(GameObject target, float fSpeedRate)
    {
        foreach (NcEffectBehaviour behaviour in target.GetComponentsInChildren<NcEffectBehaviour>(true))
        {
            behaviour.OnUpdateEffectSpeed(fSpeedRate, true);
        }
    }

    protected void ChangeParent(Transform newParent, Transform child, bool bKeepingLocalTransform, Transform addTransform)
    {
        NcTransformTool tool = null;
        if (bKeepingLocalTransform)
        {
            tool = new NcTransformTool(child.transform);
            if (addTransform != null)
            {
                tool.AddTransform(addTransform);
            }
        }
        child.parent = newParent;
        if (bKeepingLocalTransform)
        {
            tool.CopyToLocalTransform(child.transform);
        }
        if (bKeepingLocalTransform)
        {
        }
    }

    protected GameObject CreateEditorGameObject(GameObject srcGameObj)
    {
        return srcGameObj;
    }

    public GameObject CreateGameObject(string name)
    {
        if (!IsSafe())
        {
            return null;
        }
        return this.CreateEditorGameObject(new GameObject(name));
    }

    public GameObject CreateGameObject(GameObject original)
    {
        if (!IsSafe())
        {
            return null;
        }
        return this.CreateEditorGameObject((GameObject) UnityEngine.Object.Instantiate(original));
    }

    public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj)
    {
        if (!IsSafe())
        {
            return null;
        }
        GameObject obj2 = this.CreateGameObject(prefabObj);
        if ((parentObj != null) && (obj2 != null))
        {
            this.ChangeParent(parentObj.transform, obj2.transform, true, null);
        }
        return obj2;
    }

    public GameObject CreateGameObject(GameObject parentObj, Transform parentTrans, GameObject prefabObj)
    {
        if (!IsSafe())
        {
            return null;
        }
        GameObject obj2 = this.CreateGameObject(prefabObj);
        if ((parentObj != null) && (obj2 != null))
        {
            this.ChangeParent(parentObj.transform, obj2.transform, true, parentTrans);
        }
        return obj2;
    }

    public GameObject CreateGameObject(GameObject prefabObj, Vector3 position, Quaternion rotation)
    {
        if (!IsSafe())
        {
            return null;
        }
        return this.CreateEditorGameObject((GameObject) UnityEngine.Object.Instantiate(prefabObj, position, rotation));
    }

    protected void DisableEmit()
    {
        foreach (ParticleSystem system in base.gameObject.GetComponentsInChildren<ParticleSystem>(true))
        {
            if (system != null)
            {
                system.enableEmission = false;
            }
        }
        foreach (ParticleEmitter emitter in base.gameObject.GetComponentsInChildren<ParticleEmitter>(true))
        {
            if (emitter != null)
            {
                emitter.emit = false;
            }
        }
    }

    public virtual int GetAnimationState()
    {
        return -1;
    }

    public static float GetEngineDeltaTime()
    {
        return Time.deltaTime;
    }

    public static float GetEngineTime()
    {
        if (Time.time == 0f)
        {
            return 1E-06f;
        }
        return Time.time;
    }

    public static string GetMaterialColorName(Material mat)
    {
        string[] strArray = new string[] { "_Color", "_TintColor", "_EmisColor" };
        if (mat != null)
        {
            foreach (string str in strArray)
            {
                if (mat.HasProperty(str))
                {
                    return str;
                }
            }
        }
        return null;
    }

    protected Renderer GetRenderer()
    {
        if (null == this.m_renderer)
        {
            this.m_renderer = base.GetComponent<Renderer>();
        }
        return this.m_renderer;
    }

    public static GameObject GetRootInstanceEffect()
    {
        if (!IsSafe())
        {
            return null;
        }
        if (m_RootInstance == null)
        {
            m_RootInstance = GameObject.Find("_InstanceObject");
            if (m_RootInstance == null)
            {
                m_RootInstance = new GameObject("_InstanceObject");
            }
        }
        return m_RootInstance;
    }

    public static void HideNcDelayActive(GameObject tarObj)
    {
        SetActiveRecursively(tarObj, false);
    }

    protected static bool IsActive(GameObject target)
    {
        return target.activeSelf;
    }

    public static bool IsSafe()
    {
        return !m_bShuttingDown;
    }

    public void OnApplicationQuit()
    {
        m_bShuttingDown = true;
    }

    protected virtual void OnDestroy()
    {
        if (this.m_RuntimeMaterials != null)
        {
            foreach (Material material in this.m_RuntimeMaterials)
            {
                UnityEngine.Object.Destroy(material);
            }
            this.m_RuntimeMaterials = null;
        }
    }

    public virtual void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
    }

    public virtual void OnUpdateToolData()
    {
    }

    private static Texture[] PreloadPrefab(GameObject tarObj, List<GameObject> parentPrefabList, bool bCheckDup)
    {
        if (parentPrefabList.Contains(tarObj))
        {
            if (bCheckDup)
            {
                string str = string.Empty;
                for (int i = 0; i < parentPrefabList.Count; i++)
                {
                    str = str + parentPrefabList[i].name + "/";
                }
                Debug.LogWarning("LoadError : Recursive Prefab - " + str + tarObj.name);
            }
            return null;
        }
        parentPrefabList.Add(tarObj);
        Texture[] textureArray = PreloadTexture(tarObj, parentPrefabList);
        parentPrefabList.Remove(tarObj);
        return textureArray;
    }

    public static Texture[] PreloadTexture(GameObject tarObj)
    {
        if (tarObj == null)
        {
            return new Texture[0];
        }
        List<GameObject> parentPrefabList = new List<GameObject> {
            tarObj
        };
        return PreloadTexture(tarObj, parentPrefabList);
    }

    private static Texture[] PreloadTexture(GameObject tarObj, List<GameObject> parentPrefabList)
    {
        if (!IsSafe())
        {
            return null;
        }
        Renderer[] componentsInChildren = tarObj.GetComponentsInChildren<Renderer>(true);
        ListView<Texture> inList = new ListView<Texture>();
        foreach (Renderer renderer in componentsInChildren)
        {
            if ((renderer.sharedMaterials != null) && (renderer.sharedMaterials.Length > 0))
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if ((material != null) && (material.mainTexture != null))
                    {
                        inList.Add(material.mainTexture);
                    }
                }
            }
        }
        foreach (NcSpriteTexture texture in tarObj.GetComponentsInChildren<NcSpriteTexture>(true))
        {
            if (texture.m_NcSpriteFactoryPrefab != null)
            {
                Texture[] collection = PreloadPrefab(texture.m_NcSpriteFactoryPrefab, parentPrefabList, false);
                if (collection != null)
                {
                    inList.AddRange(collection);
                }
            }
        }
        foreach (NcSpriteFactory factory in tarObj.GetComponentsInChildren<NcSpriteFactory>(true))
        {
            if (factory.m_SpriteList != null)
            {
                for (int i = 0; i < factory.m_SpriteList.Count; i++)
                {
                    if (factory.m_SpriteList[i].m_EffectPrefab != null)
                    {
                        Texture[] textureArray4 = PreloadPrefab(factory.m_SpriteList[i].m_EffectPrefab, parentPrefabList, true);
                        if (textureArray4 == null)
                        {
                            factory.m_SpriteList[i].m_EffectPrefab = null;
                        }
                        else
                        {
                            inList.AddRange(textureArray4);
                        }
                        if (factory.m_SpriteList[i].m_AudioClip != null)
                        {
                        }
                    }
                }
            }
        }
        return LinqS.ToArray<Texture>(inList);
    }

    protected static void SetActive(GameObject target, bool bActive)
    {
        target.SetActive(bActive);
    }

    protected static void SetActiveRecursively(GameObject target, bool bActive)
    {
        target.SetActive(bActive);
    }

    protected void UpdateMeshColors(Color color)
    {
        if (this.m_MeshFilter == null)
        {
            this.m_MeshFilter = (MeshFilter) base.gameObject.GetComponent(typeof(MeshFilter));
        }
        if (((this.m_MeshFilter != null) && (this.m_MeshFilter.sharedMesh != null)) && (this.m_MeshFilter.mesh != null))
        {
            Color[] colorArray = new Color[this.m_MeshFilter.mesh.vertexCount];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            this.m_MeshFilter.mesh.colors = colorArray;
        }
    }

    public class _RuntimeIntance
    {
        public GameObject m_ChildGameObject;
        public GameObject m_ParentGameObject;

        public _RuntimeIntance(GameObject parentGameObject, GameObject childGameObject)
        {
            this.m_ParentGameObject = parentGameObject;
            this.m_ChildGameObject = childGameObject;
        }
    }
}

