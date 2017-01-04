using AGE;
using Assets.Scripts.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    private DictionaryView<string, GameObject> cachedPrefabs = new DictionaryView<string, GameObject>();
    private object[] commonEffects;
    private string[] emptyActorPrefabs;
    private int[][] LIMIT_CONFIG;
    private const string LIMIT_CONFIG_FILE = "Config/ParticleLimit";
    public static string[] lod_postfix = new string[] { string.Empty, "_mid", "_low" };
    public bool m_dynamicLOD;
    private Dictionary<string, bool> m_resourcesNotExist = new Dictionary<string, bool>();
    private GameObject[] rootObjs;

    public SceneMgr()
    {
        int[][] numArrayArray1 = new int[2][];
        numArrayArray1[0] = new int[] { 30, 50 };
        int[] numArray2 = new int[2];
        numArray2[1] = 30;
        numArrayArray1[1] = numArray2;
        this.LIMIT_CONFIG = numArrayArray1;
        string[] textArray1 = new string[7];
        textArray1[0] = "Prefab_Characters/EmptyHero";
        textArray1[1] = "Prefab_Characters/EmptyMonster";
        textArray1[3] = "Prefab_Characters/EmptyEye";
        textArray1[4] = "Prefab_Characters/EmptyBullet";
        this.emptyActorPrefabs = textArray1;
        this.commonEffects = new object[] { "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", 3, "Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", 5, "prefab_skill_effects/tongyong_effects/tongyong_hurt/born_back_reborn/huicheng_tongyong_01", 5, "prefab_skill_effects/common_effects/jiasu_tongyong_01", 8 };
    }

    private GameObject _GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, bool useRotation, out bool isInit)
    {
        string realPath = null;
        isInit = false;
        if (this.m_resourcesNotExist.ContainsKey(prefabName))
        {
            return null;
        }
        this.GetPrefabLOD<GameObject>(prefabName, isParticle, out realPath);
        GameObject obj2 = null;
        if (useRotation)
        {
            obj2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(realPath, pos, rot, enResourceType.BattleScene, out isInit);
        }
        else
        {
            obj2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(realPath, pos, enResourceType.BattleScene, out isInit);
        }
        if (obj2 != null)
        {
            obj2.transform.SetParent(this.rootObjs[(int) sceneObjType].transform);
            obj2.transform.position = pos;
        }
        return obj2;
    }

    public GameObject AddCulling(GameObject obj, string name = "CullingParent")
    {
        if (null == obj)
        {
            return null;
        }
        GameObject obj2 = new GameObject(name);
        if (null == obj.transform.parent)
        {
            obj2.transform.position = obj.transform.position;
            obj2.transform.rotation = obj.transform.rotation;
        }
        else
        {
            obj2.transform.parent = obj.transform.parent;
            obj2.transform.localPosition = obj.transform.localPosition;
            obj2.transform.localRotation = obj.transform.localRotation;
        }
        obj.transform.parent = obj2.transform;
        ObjectCulling component = obj2.GetComponent<ObjectCulling>();
        if (null == component)
        {
            component = obj2.AddComponent<ObjectCulling>();
        }
        component.Init(obj);
        return obj2;
    }

    public void AddToRoot(GameObject obj, SceneObjType sceneObjType)
    {
        if (obj != null)
        {
            obj.transform.parent = this.rootObjs[(int) sceneObjType].transform;
        }
    }

    public void AddToRoot(GameObject obj, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        if (obj != null)
        {
            obj.transform.parent = this.rootObjs[(int) sceneObjType].transform;
            obj.transform.position = pos;
            obj.transform.rotation = rot;
        }
    }

    public void ClearAll()
    {
        if (ActionManager.Instance != null)
        {
            ActionManager.Instance.ForceStop();
        }
        if (this.rootObjs != null)
        {
            for (int i = 0; i < this.rootObjs.Length; i++)
            {
                this.ClearObjs((SceneObjType) i);
            }
        }
        this.cachedPrefabs.Clear();
        this.m_resourcesNotExist.Clear();
        UpdateShadowPlane.ClearCache();
        base.StartCoroutine(this.UnloadAssets_Coroutine());
    }

    private void ClearObjs(SceneObjType type)
    {
        GameObject obj2 = this.rootObjs[(int) type];
        while (obj2.transform.childCount > 0)
        {
            GameObject gameObject = obj2.transform.GetChild(obj2.transform.childCount - 1).gameObject;
            gameObject.transform.parent = null;
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    private int GetDynamicLod(int lod, bool isParticle)
    {
        if (((!this.m_dynamicLOD || !isParticle) || ((lod == 2) || (this.LIMIT_CONFIG == null))) || ((lod >= this.LIMIT_CONFIG.Length) || (lod < 0)))
        {
            return lod;
        }
        int[] numArray = this.LIMIT_CONFIG[lod];
        if (numArray == null)
        {
            return lod;
        }
        int particleActiveNumber = ParticleHelper.GetParticleActiveNumber();
        if (particleActiveNumber >= numArray[1])
        {
            return 2;
        }
        if (particleActiveNumber >= numArray[0])
        {
            return 1;
        }
        return 0;
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
    {
        bool isInit = false;
        return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        bool isInit = false;
        return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, out bool isInit)
    {
        return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, Quaternion.identity, false, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, out bool isInit)
    {
        return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, true, out isInit);
    }

    public UnityEngine.Object GetPrefabLOD(string path, bool isParticle, out string realPath)
    {
        return this.GetPrefabLOD<UnityEngine.Object>(path, isParticle, out realPath);
    }

    public T GetPrefabLOD<T>(string path, bool isParticle, out string realPath) where T: UnityEngine.Object
    {
        int dynamicLod = !isParticle ? GameSettings.ModelLOD : GameSettings.ParticleLOD;
        if ((Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null) && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
        {
            dynamicLod--;
        }
        dynamicLod = Mathf.Clamp(dynamicLod, 0, 2);
        if (GameSettings.DynamicParticleLOD)
        {
            dynamicLod = this.GetDynamicLod(dynamicLod, isParticle);
        }
        while (dynamicLod >= 0)
        {
            string str = path;
            string str2 = lod_postfix[dynamicLod];
            if (!string.IsNullOrEmpty(str2))
            {
                str = str + str2;
            }
            T local = this.LoadResource<T>(str);
            if (local != null)
            {
                realPath = str;
                return local;
            }
            dynamicLod--;
        }
        realPath = path;
        this.m_resourcesNotExist.Add(path, true);
        return null;
    }

    public GameObject GetRoot(SceneObjType sceneObjType)
    {
        return this.rootObjs[(int) sceneObjType];
    }

    protected override void Init()
    {
        this.InitObjs();
        this.InitConfig();
    }

    private void InitConfig()
    {
        CResource resource = Singleton<CResourceManager>.GetInstance().GetResource("Config/ParticleLimit", typeof(TextAsset), enResourceType.Numeric, false, true);
        if (resource != null)
        {
            CBinaryObject content = resource.m_content as CBinaryObject;
            if (null != content)
            {
                char[] separator = new char[] { '\r', '\n' };
                foreach (string str2 in StringHelper.ASCIIBytesToString(content.m_data).Split(separator))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        str2 = str2.Trim();
                        if (str2.Contains("//"))
                        {
                            str2 = str2.Substring(0, str2.IndexOf("//"));
                        }
                        str2 = str2.Trim();
                        if (!string.IsNullOrEmpty(str2))
                        {
                            char[] chArray2 = new char[] { ':', ',' };
                            string[] strArray2 = str2.Split(chArray2);
                            if ((strArray2 == null) || (strArray2.Length != 3))
                            {
                                return;
                            }
                            int[] numArray = new int[3];
                            for (int i = 0; i < strArray2.Length; i++)
                            {
                                numArray[i] = Mathf.Abs(int.Parse(strArray2[i]));
                            }
                            if ((numArray[0] != 0) && (numArray[0] != 1))
                            {
                                return;
                            }
                            if (numArray[1] >= numArray[2])
                            {
                                return;
                            }
                            this.LIMIT_CONFIG[numArray[0]] = new int[] { numArray[1], numArray[2] };
                        }
                    }
                }
            }
        }
    }

    private void InitObjs()
    {
        if (this.rootObjs == null)
        {
            string[] names = Enum.GetNames(typeof(SceneObjType));
            this.rootObjs = new GameObject[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                this.rootObjs[i] = new GameObject { transform = { parent = base.gameObject.transform }, name = names[i] };
            }
        }
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType)
    {
        string realPath = null;
        GameObject original = this.GetPrefabLOD<GameObject>(prefabName, isParticle, out realPath);
        if (original == null)
        {
            return null;
        }
        GameObject obj3 = UnityEngine.Object.Instantiate(original) as GameObject;
        if (obj3 != null)
        {
            obj3.transform.parent = this.rootObjs[(int) sceneObjType].transform;
        }
        return obj3;
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
    {
        GameObject obj2 = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
        if (obj2 != null)
        {
            obj2.transform.position = pos;
        }
        return obj2;
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        GameObject obj2 = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
        if (obj2 != null)
        {
            obj2.transform.position = pos;
            obj2.transform.rotation = rot;
        }
        return obj2;
    }

    private T LoadResource<T>(string path) where T: UnityEngine.Object
    {
        path = CFileManager.EraseExtension(path);
        return (Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(T), enResourceType.BattleScene, true, false).m_content as T);
    }

    public void PreloadCommonAssets()
    {
        if (!string.IsNullOrEmpty(this.emptyActorPrefabs[0]))
        {
            this.PrepareGameObjectLOD(this.emptyActorPrefabs[0], false, enResourceType.BattleScene, 6);
        }
        if (!string.IsNullOrEmpty(this.emptyActorPrefabs[1]))
        {
            this.PrepareGameObjectLOD(this.emptyActorPrefabs[1], false, enResourceType.BattleScene, 30);
        }
        if (!string.IsNullOrEmpty(this.emptyActorPrefabs[4]))
        {
            this.PrepareGameObjectLOD(this.emptyActorPrefabs[4], false, enResourceType.BattleScene, 50);
        }
        if (!string.IsNullOrEmpty(this.emptyActorPrefabs[3]))
        {
            this.PrepareGameObjectLOD(this.emptyActorPrefabs[3], false, enResourceType.BattleScene, 10);
        }
    }

    public void PreloadCommonEffects()
    {
        for (int i = 0; i < this.commonEffects.Length; i += 2)
        {
            this.PrepareGameObjectLOD((string) this.commonEffects[i], true, enResourceType.BattleScene, (int) this.commonEffects[i + 1]);
        }
    }

    public void PrepareGameObjectLOD(string path, bool isParticle, enResourceType type, int count)
    {
        string realPath = string.Empty;
        this.GetPrefabLOD(path, isParticle, out realPath);
        Singleton<CGameObjectPool>.instance.PrepareGameObject(realPath, type, count);
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType)
    {
        GameObject obj2;
        return new GameObject { name = string.Format("{0}({1})", name, obj2.GetInstanceID()), transform = { parent = this.rootObjs[(int) sceneObjType].transform } };
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Quaternion rotation)
    {
        GameObject obj2 = new GameObject {
            name = name
        };
        obj2.transform.position = position;
        obj2.transform.rotation = rotation;
        obj2.transform.parent = this.rootObjs[(int) sceneObjType].transform;
        return obj2;
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Vector3 forward)
    {
        GameObject obj2 = new GameObject {
            name = name
        };
        obj2.transform.position = position;
        obj2.transform.rotation = Quaternion.LookRotation(forward);
        obj2.transform.parent = this.rootObjs[(int) sceneObjType].transform;
        return obj2;
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, VInt3 position, VInt3 forward)
    {
        GameObject gameObject = null;
        string fullPathInResources = this.emptyActorPrefabs[(int) sceneObjType];
        if (fullPathInResources != null)
        {
            GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.BattleScene, true, false).m_content as GameObject;
            gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(fullPathInResources, enResourceType.BattleScene);
            if ((gameObject != null) && (content != null))
            {
                gameObject.layer = content.layer;
                gameObject.tag = content.tag;
            }
        }
        else
        {
            gameObject = new GameObject();
        }
        gameObject.name = name;
        gameObject.transform.position = (Vector3) position;
        gameObject.transform.rotation = Quaternion.LookRotation((Vector3) forward);
        gameObject.transform.parent = this.rootObjs[(int) sceneObjType].transform;
        return gameObject;
    }

    [DebuggerHidden]
    private IEnumerator UnloadAssets_Coroutine()
    {
        return new <UnloadAssets_Coroutine>c__IteratorA();
    }

    [CompilerGenerated]
    private sealed class <UnloadAssets_Coroutine>c__IteratorA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = 0;
                    this.$PC = 1;
                    goto Label_0062;

                case 1:
                    this.$current = Resources.UnloadUnusedAssets();
                    this.$PC = 2;
                    goto Label_0062;

                case 2:
                    GC.Collect();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0062:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

