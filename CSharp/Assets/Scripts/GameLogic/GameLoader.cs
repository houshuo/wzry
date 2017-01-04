namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using behaviac;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameLoader : MonoSingleton<GameLoader>
    {
        private int _nProgress;
        public List<ActorMeta> actorList = new List<ActorMeta>();
        private List<ActorPreloadTab> actorPreload;
        private float coroutineTime;
        public bool isLoadStart;
        private ArrayList levelArtistList = new ArrayList();
        private ArrayList levelDesignList = new ArrayList();
        private ArrayList levelList = new ArrayList();
        private LoadCompleteDelegate LoadCompleteEvent;
        private LoadProgressDelegate LoadProgressEvent;
        private static GameSerializer s_serializer = new GameSerializer();
        private static Dictionary<string, string> s_vertexShaderMap;
        private List<string> soundBankList = new List<string>();
        public ListView<ActorConfig> staticActors = new ListView<ActorConfig>();

        static GameLoader()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("S_Game_Scene/Cloth_Lightmap_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Lightmap_Wind");
            dictionary.Add("S_Game_Scene/Cloth_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Wind");
            dictionary.Add("S_Game_Effects/Scroll2TexBend_LightMap", "S_Game_Effects/Light_VertexLit/Scroll2TexBend_LightMap");
            dictionary.Add("S_Game_Effects/Scroll2TexBend", "S_Game_Effects/Light_VertexLit/Scroll2TexBend");
            dictionary.Add("S_Game_Scene/Diffuse_NotFog", "S_Game_Scene/Light_VertexLit/Diffuse_NotFog");
            s_vertexShaderMap = dictionary;
        }

        public void AddActor(ref ActorMeta actorMeta)
        {
            this.actorList.Add(actorMeta);
        }

        public void AddArtistSerializedLevel(string name)
        {
            this.levelArtistList.Add(name);
        }

        public void AddDesignSerializedLevel(string name)
        {
            this.levelDesignList.Add(name);
        }

        public void AddLevel(string name)
        {
            this.levelList.Add(name);
        }

        public void AddSoundBank(string name)
        {
            this.soundBankList.Add(name);
        }

        public void AddStaticActor(ActorConfig actor)
        {
            this.staticActors.Add(actor);
        }

        public void AdvanceStopLoad()
        {
            if (this.isLoadStart)
            {
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                GC.Collect();
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.ADVANCE_STOP_LOADING);
            }
            this.ResetLoader();
        }

        private static void ChangeToVertexLit()
        {
            foreach (Renderer renderer in GameObject.Find("Artist").GetComponentsInChildren<Renderer>())
            {
                if ((null != renderer) && (renderer.sharedMaterials != null))
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        if ((null != renderer.sharedMaterials[i]) && (null != renderer.sharedMaterials[i].shader))
                        {
                            string name = ChangeVertexShader(renderer.sharedMaterials[i].shader.name);
                            renderer.sharedMaterials[i].shader = Shader.Find(name);
                        }
                    }
                }
            }
        }

        private static string ChangeVertexShader(string oldShader)
        {
            if (s_vertexShaderMap.ContainsKey(oldShader))
            {
                return s_vertexShaderMap[oldShader];
            }
            if (oldShader.Contains("S_Game_Scene/Light/"))
            {
                return oldShader.Replace("S_Game_Scene/Light/", "S_Game_Scene/Light_VertexLit/");
            }
            return oldShader;
        }

        [DebuggerHidden]
        private IEnumerator CoroutineLoad()
        {
            return new <CoroutineLoad>c__Iterator1A { <>f__this = this };
        }

        public void Load(LoadProgressDelegate progress, LoadCompleteDelegate finish)
        {
            if (!this.isLoadStart)
            {
                UnityEngine.Debug.Log("GameLoader Start Load");
                this.LoadProgressEvent = progress;
                this.LoadCompleteEvent = finish;
                this._nProgress = 0;
                this.isLoadStart = true;
                base.StartCoroutine("CoroutineLoad");
            }
        }

        [DebuggerHidden]
        private IEnumerator LoadActorAssets(LHCWrapper InWrapper)
        {
            return new <LoadActorAssets>c__Iterator14 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadAgeRecursiveAssets(LHCWrapper InWrapper)
        {
            return new <LoadAgeRecursiveAssets>c__Iterator16 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadArtistLevel(LoaderHelperWrapper InWrapper)
        {
            return new <LoadArtistLevel>c__Iterator11 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonAssetBundle(LoaderHelperWrapper InWrapper)
        {
            return new <LoadCommonAssetBundle>c__IteratorF { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonAssets(LoaderHelperWrapper InWrapper)
        {
            return new <LoadCommonAssets>c__Iterator13 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonEffect(LoaderHelperWrapper InWrapper)
        {
            return new <LoadCommonEffect>c__Iterator10 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadDesignLevel(LoaderHelperWrapper InWrapper)
        {
            return new <LoadDesignLevel>c__Iterator12 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator LoadNoActorAssets(LHCWrapper InWrapper)
        {
            return new <LoadNoActorAssets>c__Iterator15 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PreSpawnSoldiers(LoaderHelperWrapper InWrapper)
        {
            return new <PreSpawnSoldiers>c__Iterator19 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        public void ResetLoader()
        {
            this.levelList.Clear();
            this.actorList.Clear();
            this.levelDesignList.Clear();
            this.levelArtistList.Clear();
            this.soundBankList.Clear();
            this.staticActors.Clear();
            this._nProgress = 0;
            if (this.isLoadStart)
            {
                base.StopCoroutine("PreSpawnSoldiers");
                base.StopCoroutine("SpawnDynamicActor");
                base.StopCoroutine("SpawnStaticActor");
                base.StopCoroutine("LoadAgeRecursiveAssets");
                base.StopCoroutine("LoadNoActorAssets");
                base.StopCoroutine("LoadActorAssets");
                base.StopCoroutine("LoadCommonAssets");
                base.StopCoroutine("LoadDesignLevel");
                base.StopCoroutine("LoadArtistLevel");
                base.StopCoroutine("LoadCommonAssetBundle");
                base.StopCoroutine("LoadCommonEffect");
                base.StopCoroutine("CoroutineLoad");
                this.isLoadStart = false;
            }
        }

        private bool ShouldYieldReturn()
        {
            return ((Time.realtimeSinceStartup - this.coroutineTime) > 0.08f);
        }

        [DebuggerHidden]
        private IEnumerator SpawnDynamicActor(LoaderHelperWrapper InWrapper)
        {
            return new <SpawnDynamicActor>c__Iterator18 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator SpawnStaticActor(LoaderHelperWrapper InWrapper)
        {
            return new <SpawnStaticActor>c__Iterator17 { InWrapper = InWrapper, <$>InWrapper = InWrapper, <>f__this = this };
        }

        private void UpdateProgress(LoaderHelperCamera lhc, int oldProgress, int duty, int index, int count)
        {
            this.coroutineTime = Time.realtimeSinceStartup;
            this.nProgress = oldProgress + ((duty * index) / count);
            this.LoadProgressEvent(this.nProgress * 0.0001f);
            if (lhc != null)
            {
                lhc.Update();
            }
        }

        public int nProgress
        {
            get
            {
                return this._nProgress;
            }
            set
            {
                if (value >= this._nProgress)
                {
                    this._nProgress = value;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CoroutineLoad>c__Iterator1A : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader <>f__this;
            internal Animator <animator>__7;
            internal List<string> <anims>__8;
            internal GameObject <go>__5;
            internal GameObject <go2>__6;
            internal int <i>__0;
            internal int <i>__1;
            internal int <i>__9;
            internal int <j>__10;
            internal LoaderHelperCamera <lhc>__3;
            internal LoaderHelper <loadHelper>__2;
            internal CUIFormScript <uiForm>__4;

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
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0A47;

                    case 1:
                    {
                        DynamicShadow.DisableAllDynamicShadows();
                        DynamicShadow.InitDefaultGlobalVariables();
                        Singleton<CUIManager>.GetInstance().ClearFormPool();
                        Singleton<CGameObjectPool>.GetInstance().ClearPooledObjects();
                        enResourceType[] resourceTypes = new enResourceType[5];
                        resourceTypes[1] = enResourceType.UI3DImage;
                        resourceTypes[2] = enResourceType.UIForm;
                        resourceTypes[3] = enResourceType.UIPrefab;
                        resourceTypes[4] = enResourceType.UISprite;
                        Singleton<CResourceManager>.GetInstance().RemoveCachedResources(resourceTypes);
                        this.<>f__this.nProgress = 200;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_0A47;
                    }
                    case 2:
                        this.<>f__this.nProgress = 300;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = null;
                        this.$PC = 3;
                        goto Label_0A47;

                    case 3:
                        this.<>f__this.nProgress = 400;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = null;
                        this.$PC = 4;
                        goto Label_0A47;

                    case 4:
                        if (this.<>f__this.levelList.Count == 0)
                        {
                            this.<>f__this.levelList.Add("EmptyScene");
                        }
                        PlaneShadowSettings.SetDefault();
                        FogOfWarSettings.SetDefault();
                        this.<>f__this.nProgress = 500;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = null;
                        this.$PC = 5;
                        goto Label_0A47;

                    case 5:
                        this.<i>__0 = 0;
                        break;

                    case 6:
                        this.<i>__0++;
                        break;

                    case 7:
                        if (((this.<>f__this.levelArtistList.Count > 0) || (this.<>f__this.levelDesignList.Count > 0)) && (Camera.allCameras != null))
                        {
                            this.<i>__1 = 0;
                            while (this.<i>__1 < Camera.allCameras.Length)
                            {
                                if (Camera.main != null)
                                {
                                    UnityEngine.Object.Destroy(Camera.allCameras[this.<i>__1].gameObject);
                                }
                                this.<i>__1++;
                            }
                        }
                        this.<>f__this.nProgress = 0x3e8;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 8;
                        goto Label_0A47;

                    case 8:
                    {
                        this.<loadHelper>__2 = new LoaderHelper();
                        this.<lhc>__3 = new LoaderHelperCamera();
                        GameLoader.LoaderHelperWrapper wrapper = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 350
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadCommonAssetBundle", wrapper);
                        this.$PC = 9;
                        goto Label_0A47;
                    }
                    case 9:
                    {
                        GameLoader.LoaderHelperWrapper wrapper2 = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 150
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadCommonEffect", wrapper2);
                        this.$PC = 10;
                        goto Label_0A47;
                    }
                    case 10:
                    {
                        GameLoader.LoaderHelperWrapper wrapper3 = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 0x3e8
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadArtistLevel", wrapper3);
                        this.$PC = 11;
                        goto Label_0A47;
                    }
                    case 11:
                    {
                        GameLoader.LoaderHelperWrapper wrapper4 = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 500
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadDesignLevel", wrapper4);
                        this.$PC = 12;
                        goto Label_0A47;
                    }
                    case 12:
                    {
                        GameLoader.LoaderHelperWrapper wrapper5 = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 500
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadCommonAssets", wrapper5);
                        this.$PC = 13;
                        goto Label_0A47;
                    }
                    case 13:
                    {
                        GameLoader.LHCWrapper wrapper6 = new GameLoader.LHCWrapper {
                            lhc = this.<lhc>__3,
                            loadHelper = this.<loadHelper>__2,
                            duty = 500
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadActorAssets", wrapper6);
                        this.$PC = 14;
                        goto Label_0A47;
                    }
                    case 14:
                    {
                        GameLoader.LHCWrapper wrapper7 = new GameLoader.LHCWrapper {
                            lhc = this.<lhc>__3,
                            loadHelper = this.<loadHelper>__2,
                            duty = 0x3e8
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadNoActorAssets", wrapper7);
                        this.$PC = 15;
                        goto Label_0A47;
                    }
                    case 15:
                    {
                        GameLoader.LHCWrapper wrapper8 = new GameLoader.LHCWrapper {
                            lhc = this.<lhc>__3,
                            loadHelper = this.<loadHelper>__2,
                            duty = 0xf3c
                        };
                        this.$current = this.<>f__this.StartCoroutine("LoadAgeRecursiveAssets", wrapper8);
                        this.$PC = 0x10;
                        goto Label_0A47;
                    }
                    case 0x10:
                    case 0x11:
                        if (!this.<lhc>__3.Update())
                        {
                            this.$current = 0;
                            this.$PC = 0x11;
                        }
                        else
                        {
                            this.<lhc>__3.Destroy();
                            this.<lhc>__3 = null;
                            GameLoader.LoaderHelperWrapper wrapper9 = new GameLoader.LoaderHelperWrapper {
                                loadHelper = this.<loadHelper>__2,
                                duty = 200
                            };
                            this.$current = this.<>f__this.StartCoroutine("SpawnStaticActor", wrapper9);
                            this.$PC = 0x12;
                        }
                        goto Label_0A47;

                    case 0x12:
                    {
                        GameLoader.LoaderHelperWrapper wrapper10 = new GameLoader.LoaderHelperWrapper {
                            loadHelper = this.<loadHelper>__2,
                            duty = 200
                        };
                        this.$current = this.<>f__this.StartCoroutine("SpawnDynamicActor", wrapper10);
                        this.$PC = 0x13;
                        goto Label_0A47;
                    }
                    case 0x13:
                        this.<>f__this.nProgress = 0x2648;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 20;
                        goto Label_0A47;

                    case 20:
                        if (GameSettings.AllowOutlineFilter)
                        {
                            OutlineFilter.EnableOutlineFilter();
                        }
                        this.$current = 0;
                        this.$PC = 0x15;
                        goto Label_0A47;

                    case 0x15:
                        Shader.WarmupAllShaders();
                        this.$current = 0;
                        this.$PC = 0x16;
                        goto Label_0A47;

                    case 0x16:
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                        Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                        this.<>f__this.nProgress = 0x26ac;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 0x17;
                        goto Label_0A47;

                    case 0x17:
                        this.<uiForm>__4 = Singleton<CBattleSystem>.GetInstance().LoadForm(!Singleton<WatchController>.GetInstance().IsWatching ? CBattleSystem.FormType.Fight : CBattleSystem.FormType.Watch);
                        if (this.<uiForm>__4 == null)
                        {
                            goto Label_0965;
                        }
                        this.<go>__5 = this.<uiForm>__4.gameObject.FindChildBFS("KillNotify_New");
                        this.<go2>__6 = this.<go>__5.FindChildBFS("KillNotify_Sub");
                        this.<animator>__7 = this.<go2>__6.GetComponent<Animator>();
                        this.<go>__5.CustomSetActive(true);
                        this.<anims>__8 = KillNotifyUT.GetAllAnimations();
                        this.<i>__9 = 0;
                        goto Label_094F;

                    case 0x18:
                        this.<i>__9++;
                        goto Label_094F;

                    case 0x19:
                        this.<>f__this.nProgress = 0x2710;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 0x1a;
                        goto Label_0A47;

                    case 0x1a:
                        this.<>f__this.actorPreload = null;
                        this.<>f__this.isLoadStart = false;
                        this.<>f__this.LoadCompleteEvent();
                        Singleton<GameDataMgr>.GetInstance().UnloadDataBin();
                        GC.Collect();
                        UnityEngine.Debug.Log("GameLoader Finish Load");
                        this.$PC = -1;
                        goto Label_0A45;

                    default:
                        goto Label_0A45;
                }
                if (this.<i>__0 < this.<>f__this.levelList.Count)
                {
                    this.$current = Application.LoadLevelAsync((string) this.<>f__this.levelList[this.<i>__0]);
                    this.$PC = 6;
                }
                else
                {
                    this.<>f__this.nProgress = 600;
                    this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                    this.$current = 0;
                    this.$PC = 7;
                }
                goto Label_0A47;
            Label_094F:
                if (this.<i>__9 < this.<anims>__8.Count)
                {
                    this.<animator>__7.Play(this.<anims>__8[this.<i>__9]);
                    this.<j>__10 = 0;
                    while (this.<j>__10 < 6)
                    {
                        this.<animator>__7.Update(0.5f);
                        this.<j>__10++;
                    }
                    this.$current = 0;
                    this.$PC = 0x18;
                    goto Label_0A47;
                }
            Label_0965:
                Singleton<BattleSkillHudControl>.CreateInstance();
                GameLoader.LoaderHelperWrapper wrapper11 = new GameLoader.LoaderHelperWrapper {
                    loadHelper = this.<loadHelper>__2,
                    duty = 100
                };
                this.$current = this.<>f__this.StartCoroutine("PreSpawnSoldiers", wrapper11);
                this.$PC = 0x19;
                goto Label_0A47;
            Label_0A45:
                return false;
            Label_0A47:
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

        [CompilerGenerated]
        private sealed class <LoadActorAssets>c__Iterator14 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal AGE.Action <action>__32;
            internal ActorMeta <actorMeta>__7;
            internal string <btPath>__34;
            internal int <configID>__29;
            internal int <count>__4;
            internal int <duty>__1;
            internal ResHeroCfgInfo <heroCfgInfo>__9;
            internal Player <hostPlayer>__8;
            internal int <i>__13;
            internal int <i>__15;
            internal int <i>__6;
            internal string <iconPath>__12;
            internal int <index>__5;
            internal int <j>__10;
            internal int <j>__20;
            internal int <j>__26;
            internal int <j>__27;
            internal int <j>__31;
            internal int <j>__33;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal ActorPreloadTab <loadInfo>__14;
            internal ActorPreloadTab <loadInfo>__16;
            internal int <lod>__23;
            internal int <markID>__28;
            internal int <oldProgress>__3;
            internal int <originalParticleLOD>__21;
            internal string <parPathKey>__24;
            internal string <parPathKey>__25;
            internal CResourcePackerInfo <pkger>__18;
            internal Dictionary<object, AssetRefType> <refAssets>__30;
            internal ResSkillCfgInfo <skillCfgInfo>__11;
            internal int <targetParticleOLD>__22;
            internal GameObject <tmpObj>__17;
            internal int <x>__19;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<oldProgress>__3 = this.<>f__this.nProgress;
                        this.<>f__this.actorPreload = this.InWrapper.loadHelper.GetActorPreload();
                        this.<count>__4 = this.<>f__this.actorPreload.Count;
                        this.<index>__5 = 0;
                        this.<i>__6 = 0;
                        while (this.<i>__6 < this.<>f__this.actorPreload.Count)
                        {
                            this.<actorMeta>__7 = this.<>f__this.actorPreload[this.<i>__6].theActor;
                            this.<hostPlayer>__8 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if (this.<hostPlayer>__8.PlayerId == this.<actorMeta>__7.PlayerId)
                            {
                                this.<heroCfgInfo>__9 = GameDataMgr.heroDatabin.GetDataByKey((long) this.<actorMeta>__7.ConfigId);
                                this.<j>__10 = 0;
                                while (this.<j>__10 < this.<heroCfgInfo>__9.astSkill.Length)
                                {
                                    this.<skillCfgInfo>__11 = GameDataMgr.skillDatabin.GetDataByKey((long) this.<heroCfgInfo>__9.astSkill[this.<j>__10].iSkillID);
                                    object[] inParameters = new object[] { this.<heroCfgInfo>__9.astSkill[this.<j>__10].iSkillID };
                                    DebugHelper.Assert(this.<skillCfgInfo>__11 != null, "Failed Found skill config id = {0}", inParameters);
                                    this.<iconPath>__12 = string.Empty;
                                    if (this.<skillCfgInfo>__11 != null)
                                    {
                                        this.<iconPath>__12 = StringHelper.UTF8BytesToString(ref this.<skillCfgInfo>__11.szIconPath);
                                    }
                                    if (!string.IsNullOrEmpty(this.<iconPath>__12))
                                    {
                                        Singleton<CResourceManager>.GetInstance().LoadAllResourceInResourcePackerInfo(Singleton<CResourceManager>.GetInstance().GetResourceBelongedPackerInfo(CUIUtility.s_Sprite_Dynamic_Skill_Dir + this.<iconPath>__12), enResourceType.UISprite);
                                    }
                                    this.<j>__10++;
                                }
                            }
                            this.<i>__6++;
                        }
                        this.<i>__13 = 0;
                        while (this.<i>__13 < this.<>f__this.actorPreload.Count)
                        {
                            this.<loadInfo>__14 = this.<>f__this.actorPreload[this.<i>__13];
                            this.<count>__4 += (((this.<loadInfo>__14.parPrefabs.Count + this.<loadInfo>__14.mesPrefabs.Count) + this.<loadInfo>__14.soundBanks.Count) + this.<loadInfo>__14.ageActions.Count) + this.<loadInfo>__14.behaviorXml.Count;
                            this.<i>__13++;
                        }
                        this.<i>__15 = 0;
                        while (this.<i>__15 < this.<>f__this.actorPreload.Count)
                        {
                            this.<loadInfo>__16 = this.<>f__this.actorPreload[this.<i>__15];
                            this.<tmpObj>__17 = null;
                            if ((this.<loadInfo>__16.modelPrefab.assetPath != null) && !this.InWrapper.lhc.HasLoaded(this.<loadInfo>__16.modelPrefab.assetPath))
                            {
                                this.<pkger>__18 = Singleton<CResourceManager>.instance.GetResourceBelongedPackerInfo(this.<loadInfo>__16.modelPrefab.assetPath);
                                if ((this.<pkger>__18 != null) && this.<pkger>__18.m_isAssetBundle)
                                {
                                    if (!this.<pkger>__18.IsAssetBundleLoaded())
                                    {
                                        this.<pkger>__18.LoadAssetBundle(CFileManager.GetIFSExtractPath());
                                    }
                                    this.<x>__19 = 0;
                                    while (this.<x>__19 < this.<pkger>__18.m_resourceInfos.Count)
                                    {
                                        stResourceInfo info = this.<pkger>__18.m_resourceInfos[this.<x>__19];
                                        if (string.Equals(info.m_extension, ".prefab", StringComparison.OrdinalIgnoreCase))
                                        {
                                            stResourceInfo info2 = this.<pkger>__18.m_resourceInfos[this.<x>__19];
                                            if (!info2.m_fullPathInResourcesWithoutExtension.Contains("UGUI/Sprite/Dynamic"))
                                            {
                                                stResourceInfo info3 = this.<pkger>__18.m_resourceInfos[this.<x>__19];
                                                if (!string.Equals(info3.m_fullPathInResourcesWithoutExtension, CFileManager.EraseExtension(this.<loadInfo>__16.modelPrefab.assetPath), StringComparison.OrdinalIgnoreCase))
                                                {
                                                    goto Label_04BE;
                                                }
                                            }
                                        }
                                        stResourceInfo info4 = this.<pkger>__18.m_resourceInfos[this.<x>__19];
                                        stResourceInfo info5 = this.<pkger>__18.m_resourceInfos[this.<x>__19];
                                        Singleton<CResourceManager>.instance.GetResource(info4.m_fullPathInResourcesWithoutExtension, Singleton<CResourceManager>.GetInstance().GetResourceContentType(info5.m_extension), enResourceType.BattleScene, true, false);
                                    Label_04BE:
                                        this.<x>__19++;
                                    }
                                    if (this.<loadInfo>__16.theActor.ActorType == ActorTypeDef.Actor_Type_Hero)
                                    {
                                        this.<pkger>__18.UnloadAssetBundle(false);
                                    }
                                }
                                this.<tmpObj>__17 = Singleton<CGameObjectPool>.instance.GetGameObject(this.<loadInfo>__16.modelPrefab.assetPath, enResourceType.BattleScene);
                                this.<lhc>__0.AddObj(this.<loadInfo>__16.modelPrefab.assetPath, this.<tmpObj>__17);
                            }
                            this.<j>__20 = 0;
                            while (this.<j>__20 < this.<loadInfo>__16.parPrefabs.Count)
                            {
                                AssetLoadBase base2 = this.<loadInfo>__16.parPrefabs[this.<j>__20];
                                if (!this.<lhc>__0.HasLoaded(base2.assetPath))
                                {
                                    if (GameSettings.DynamicParticleLOD)
                                    {
                                        this.<originalParticleLOD>__21 = GameSettings.ParticleLOD;
                                        this.<targetParticleOLD>__22 = this.<originalParticleLOD>__21;
                                        if (((Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer() != null) && (this.<loadInfo>__16.theActor.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId)) && (this.<targetParticleOLD>__22 > 1))
                                        {
                                            this.<targetParticleOLD>__22 = 1;
                                        }
                                        this.<lod>__23 = this.<targetParticleOLD>__22;
                                        while (this.<lod>__23 <= 2)
                                        {
                                            AssetLoadBase base3 = this.<loadInfo>__16.parPrefabs[this.<j>__20];
                                            this.<parPathKey>__24 = base3.assetPath + "_lod_" + this.<lod>__23;
                                            if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__24))
                                            {
                                                GameSettings.ParticleLOD = this.<lod>__23;
                                                AssetLoadBase base4 = this.<loadInfo>__16.parPrefabs[this.<j>__20];
                                                this.<tmpObj>__17 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, true, SceneObjType.ActionRes, Vector3.zero);
                                                this.<lhc>__0.AddObj(this.<parPathKey>__24, this.<tmpObj>__17);
                                            }
                                            this.<lod>__23++;
                                        }
                                        GameSettings.ParticleLOD = this.<originalParticleLOD>__21;
                                    }
                                    else
                                    {
                                        AssetLoadBase base5 = this.<loadInfo>__16.parPrefabs[this.<j>__20];
                                        this.<parPathKey>__25 = base5.assetPath;
                                        if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__25))
                                        {
                                            AssetLoadBase base6 = this.<loadInfo>__16.parPrefabs[this.<j>__20];
                                            this.<tmpObj>__17 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base6.assetPath, true, SceneObjType.ActionRes, Vector3.zero);
                                            this.<lhc>__0.AddObj(this.<parPathKey>__25, this.<tmpObj>__17);
                                        }
                                    }
                                }
                                this.<index>__5++;
                                if (!this.<>f__this.ShouldYieldReturn())
                                {
                                    goto Label_07B6;
                                }
                                this.$current = 0;
                                this.$PC = 1;
                                goto Label_0C9C;
                            Label_078D:
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            Label_07B6:
                                this.<j>__20++;
                            }
                            this.<j>__26 = 0;
                            while (this.<j>__26 < this.<loadInfo>__16.mesPrefabs.Count)
                            {
                                AssetLoadBase base7 = this.<loadInfo>__16.mesPrefabs[this.<j>__26];
                                if (!this.<lhc>__0.HasLoaded(base7.assetPath))
                                {
                                    AssetLoadBase base8 = this.<loadInfo>__16.mesPrefabs[this.<j>__26];
                                    this.<tmpObj>__17 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base8.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                                    AssetLoadBase base9 = this.<loadInfo>__16.mesPrefabs[this.<j>__26];
                                    this.<lhc>__0.AddObj(base9.assetPath, this.<tmpObj>__17);
                                }
                                this.<index>__5++;
                                if (!this.<>f__this.ShouldYieldReturn())
                                {
                                    goto Label_08DF;
                                }
                                this.$current = 0;
                                this.$PC = 2;
                                goto Label_0C9C;
                            Label_08B6:
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            Label_08DF:
                                this.<j>__26++;
                            }
                            this.<j>__27 = 0;
                            while (this.<j>__27 < this.<loadInfo>__16.soundBanks.Count)
                            {
                                AssetLoadBase base10 = this.<loadInfo>__16.soundBanks[this.<j>__27];
                                Singleton<CSoundManager>.instance.LoadBank(base10.assetPath, CSoundManager.BankType.Battle);
                                this.<index>__5++;
                                if (!this.<>f__this.ShouldYieldReturn())
                                {
                                    goto Label_099D;
                                }
                                this.$current = 0;
                                this.$PC = 3;
                                goto Label_0C9C;
                            Label_0974:
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            Label_099D:
                                this.<j>__27++;
                            }
                            this.<markID>__28 = this.<loadInfo>__16.MarkID;
                            this.<configID>__29 = this.<loadInfo>__16.theActor.ConfigId;
                            this.<refAssets>__30 = this.<loadHelper>__2.GetRefAssets(this.<markID>__28, this.<configID>__29);
                            this.<j>__31 = 0;
                            while (this.<j>__31 < this.<loadInfo>__16.ageActions.Count)
                            {
                                AssetLoadBase base11 = this.<loadInfo>__16.ageActions[this.<j>__31];
                                this.<action>__32 = MonoSingleton<ActionManager>.instance.LoadActionResource(base11.assetPath);
                                if (this.<action>__32 != null)
                                {
                                    this.<action>__32.GetAssociatedResources(this.<refAssets>__30, this.<markID>__28);
                                }
                                this.<index>__5++;
                                if (!this.<>f__this.ShouldYieldReturn())
                                {
                                    goto Label_0AC6;
                                }
                                this.$current = 0;
                                this.$PC = 4;
                                goto Label_0C9C;
                            Label_0A9D:
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            Label_0AC6:
                                this.<j>__31++;
                            }
                            this.<j>__33 = 0;
                            while (this.<j>__33 < this.<loadInfo>__16.behaviorXml.Count)
                            {
                                AssetLoadBase base12 = this.<loadInfo>__16.behaviorXml[this.<j>__33];
                                this.<btPath>__34 = base12.assetPath;
                                Workspace.Load(this.<btPath>__34, false);
                                this.<index>__5++;
                                if (!this.<>f__this.ShouldYieldReturn())
                                {
                                    goto Label_0B8C;
                                }
                                this.$current = 0;
                                this.$PC = 5;
                                goto Label_0C9C;
                            Label_0B63:
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            Label_0B8C:
                                this.<j>__33++;
                            }
                            this.<index>__5++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_0C14;
                            }
                            this.$current = 0;
                            this.$PC = 6;
                            goto Label_0C9C;
                        Label_0BEB:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                        Label_0C14:
                            this.<i>__15++;
                        }
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$current = 0;
                        this.$PC = 7;
                        goto Label_0C9C;

                    case 1:
                        goto Label_078D;

                    case 2:
                        goto Label_08B6;

                    case 3:
                        goto Label_0974;

                    case 4:
                        goto Label_0A9D;

                    case 5:
                        goto Label_0B63;

                    case 6:
                        goto Label_0BEB;

                    case 7:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0C9C:
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

        [CompilerGenerated]
        private sealed class <LoadAgeRecursiveAssets>c__Iterator16 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal AGE.Action <action>__32;
            internal string <assetPath>__19;
            internal int <configID>__16;
            internal int <count>__8;
            internal int <currentParticleLOD>__20;
            internal int <duty>__1;
            internal GameObject <gameObj>__29;
            internal int <i>__5;
            internal int <idx>__11;
            internal int <idx>__13;
            internal int <index>__9;
            internal int <j>__17;
            internal int <j>__24;
            internal int <j>__27;
            internal int <j>__31;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal int <lod>__21;
            internal int <markID>__15;
            internal int <numPasses>__3;
            internal int <oldProgress>__4;
            internal string <parPathKey>__22;
            internal string <parPathKey>__23;
            internal int <progress>__10;
            internal Dictionary<object, AssetRefType> <refAssets>__33;
            internal ActorPreloadTab <restAssets>__12;
            internal ActorPreloadTab <restAssets>__14;
            internal List<ActorPreloadTab> <restAssetsList>__7;
            internal SpriteRenderer <sr>__30;
            internal int <subDuty>__6;
            internal CResource <tempObj>__28;
            internal ListView<Texture> <textures>__26;
            internal GameObject <tmpObj>__18;
            internal GameObject <tmpObj>__25;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<numPasses>__3 = 10;
                        this.<oldProgress>__4 = this.<>f__this.nProgress;
                        this.<i>__5 = 0;
                        break;

                    case 1:
                        goto Label_0367;

                    case 2:
                        goto Label_0495;

                    case 3:
                        goto Label_062A;

                    case 4:
                        goto Label_0737;

                    case 5:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, 1, 1);
                        this.<i>__5++;
                        break;

                    case 6:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__4, this.<duty>__1, 1, 1);
                        this.$PC = -1;
                        goto Label_087F;

                    default:
                        goto Label_087F;
                }
                if (this.<i>__5 < this.<numPasses>__3)
                {
                    if (this.<i>__5 < 3)
                    {
                        this.<subDuty>__6 = this.<duty>__1 / 4;
                    }
                    else
                    {
                        this.<subDuty>__6 = this.<duty>__1 / (4 * (this.<numPasses>__3 - 3));
                    }
                    this.<restAssetsList>__7 = this.<loadHelper>__2.AnalyseAgeRefAssets(this.<loadHelper>__2.ageRefAssets2);
                    this.<loadHelper>__2.ageRefAssets2.Clear();
                    this.<count>__8 = 0;
                    this.<index>__9 = 0;
                    this.<progress>__10 = this.<>f__this.nProgress;
                    this.<idx>__11 = 0;
                    while (this.<idx>__11 < this.<restAssetsList>__7.Count)
                    {
                        this.<restAssets>__12 = this.<restAssetsList>__7[this.<idx>__11];
                        this.<count>__8 += (this.<restAssets>__12.parPrefabs.Count + this.<restAssets>__12.mesPrefabs.Count) + this.<restAssets>__12.ageActions.Count;
                        this.<idx>__11++;
                    }
                    this.<idx>__13 = 0;
                    while (this.<idx>__13 < this.<restAssetsList>__7.Count)
                    {
                        this.<restAssets>__14 = this.<restAssetsList>__7[this.<idx>__13];
                        this.<markID>__15 = this.<restAssets>__14.MarkID;
                        this.<configID>__16 = this.<restAssets>__14.theActor.ConfigId;
                        this.<j>__17 = 0;
                        while (this.<j>__17 < this.<restAssets>__14.parPrefabs.Count)
                        {
                            this.<tmpObj>__18 = null;
                            AssetLoadBase base2 = this.<restAssets>__14.parPrefabs[this.<j>__17];
                            this.<assetPath>__19 = base2.assetPath;
                            if (GameSettings.DynamicParticleLOD)
                            {
                                this.<currentParticleLOD>__20 = GameSettings.ParticleLOD;
                                this.<lod>__21 = this.<currentParticleLOD>__20;
                                while (this.<lod>__21 <= 2)
                                {
                                    this.<parPathKey>__22 = this.<assetPath>__19 + "_lod_" + this.<lod>__21;
                                    if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__22))
                                    {
                                        GameSettings.ParticleLOD = this.<lod>__21;
                                        this.<tmpObj>__18 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__19, true, SceneObjType.ActionRes, Vector3.zero);
                                        this.<lhc>__0.AddObj(this.<parPathKey>__22, this.<tmpObj>__18);
                                    }
                                    this.<lod>__21++;
                                }
                                GameSettings.ParticleLOD = this.<currentParticleLOD>__20;
                            }
                            else
                            {
                                this.<parPathKey>__23 = this.<assetPath>__19;
                                if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__23))
                                {
                                    this.<tmpObj>__18 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__19, true, SceneObjType.ActionRes, Vector3.zero);
                                    this.<lhc>__0.AddObj(this.<parPathKey>__23, this.<tmpObj>__18);
                                }
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_0390;
                            }
                            this.$current = 0;
                            this.$PC = 1;
                            goto Label_0881;
                        Label_0367:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, this.<index>__9, this.<count>__8);
                        Label_0390:
                            this.<j>__17++;
                        }
                        this.<j>__24 = 0;
                        while (this.<j>__24 < this.<restAssets>__14.mesPrefabs.Count)
                        {
                            this.<tmpObj>__25 = null;
                            AssetLoadBase base3 = this.<restAssets>__14.mesPrefabs[this.<j>__24];
                            if (!this.<lhc>__0.HasLoaded(base3.assetPath))
                            {
                                AssetLoadBase base4 = this.<restAssets>__14.mesPrefabs[this.<j>__24];
                                this.<tmpObj>__25 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                                AssetLoadBase base5 = this.<restAssets>__14.mesPrefabs[this.<j>__24];
                                this.<lhc>__0.AddObj(base5.assetPath, this.<tmpObj>__25);
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_04BE;
                            }
                            this.$current = 0;
                            this.$PC = 2;
                            goto Label_0881;
                        Label_0495:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, this.<index>__9, this.<count>__8);
                        Label_04BE:
                            this.<j>__24++;
                        }
                        this.<textures>__26 = new ListView<Texture>();
                        this.<j>__27 = 0;
                        while (this.<j>__27 < this.<restAssets>__14.spritePrefabs.Count)
                        {
                            AssetLoadBase base6 = this.<restAssets>__14.spritePrefabs[this.<j>__27];
                            this.<tempObj>__28 = Singleton<CResourceManager>.instance.GetResource(base6.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                            if (((this.<tempObj>__28 != null) && (this.<tempObj>__28.m_content != null)) && (this.<tempObj>__28.m_content is GameObject))
                            {
                                this.<gameObj>__29 = (GameObject) this.<tempObj>__28.m_content;
                                this.<sr>__30 = this.<gameObj>__29.GetComponent<SpriteRenderer>();
                                if (((this.<sr>__30 != null) && (this.<sr>__30.sprite != null)) && (this.<sr>__30.sprite.texture != null))
                                {
                                    this.<textures>__26.Add(this.<sr>__30.sprite.texture);
                                }
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_0653;
                            }
                            this.$current = 0;
                            this.$PC = 3;
                            goto Label_0881;
                        Label_062A:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, this.<index>__9, this.<count>__8);
                        Label_0653:
                            this.<j>__27++;
                        }
                        this.<textures>__26.Clear();
                        this.<j>__31 = 0;
                        while (this.<j>__31 < this.<restAssets>__14.ageActions.Count)
                        {
                            AssetLoadBase base7 = this.<restAssets>__14.ageActions[this.<j>__31];
                            this.<action>__32 = MonoSingleton<ActionManager>.instance.LoadActionResource(base7.assetPath);
                            if (this.<action>__32 != null)
                            {
                                this.<refAssets>__33 = this.<loadHelper>__2.GetRefAssets(this.<markID>__15, this.<configID>__16);
                                this.<action>__32.GetAssociatedResources(this.<refAssets>__33, this.<markID>__15);
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_0760;
                            }
                            this.$current = 0;
                            this.$PC = 4;
                            goto Label_0881;
                        Label_0737:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, this.<index>__9, this.<count>__8);
                        Label_0760:
                            this.<j>__31++;
                        }
                        this.<idx>__13++;
                    }
                    this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__10, this.<subDuty>__6, 1, 1);
                    this.$current = 0;
                    this.$PC = 5;
                }
                else
                {
                    this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__4, this.<duty>__1, 1, 1);
                    this.$current = 0;
                    this.$PC = 6;
                }
                goto Label_0881;
            Label_087F:
                return false;
            Label_0881:
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

        [CompilerGenerated]
        private sealed class <LoadArtistLevel>c__Iterator11 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal string <artAssetNameHigh>__2;
            internal string <artAssetNameLow>__4;
            internal string <artAssetNameMid>__3;
            internal GameObject <artRoot>__9;
            internal int <duty>__13;
            internal string <fullPath>__7;
            internal int <i>__1;
            internal int <j>__12;
            internal LevelResAsset <levelArtist>__6;
            internal string[] <levelNames>__5;
            internal int <lod>__8;
            internal int <oldProgress>__0;
            internal ParticleSystem[] <psArray>__11;
            internal Transform <staticRoot>__10;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        goto Label_034B;

                    case 1:
                        this.$PC = -1;
                        goto Label_03DF;

                    default:
                        goto Label_03DF;
                }
            Label_018B:
                if (null == this.<levelArtist>__6)
                {
                    UnityEngine.Debug.LogError("错误，没有找到导出的美术场景SceneExport/Artist/" + this.<>f__this.levelArtistList[this.<i>__1] + ".asset");
                }
                else
                {
                    this.<artRoot>__9 = GameLoader.s_serializer.Load(this.<levelArtist>__6);
                    if (null == this.<artRoot>__9)
                    {
                        UnityEngine.Debug.LogError("美术场景SceneExport/Artist/" + this.<>f__this.levelArtistList[this.<i>__1] + ".asset有错误！请检查！");
                    }
                    else
                    {
                        this.<staticRoot>__10 = this.<artRoot>__9.transform.Find("StaticMesh");
                        if (null != this.<staticRoot>__10)
                        {
                            StaticBatchingUtility.Combine(this.<staticRoot>__10.gameObject);
                        }
                        Singleton<CResourceManager>.GetInstance().RemoveCachedResource(this.<fullPath>__7);
                        this.<psArray>__11 = this.<artRoot>__9.GetComponentsInChildren<ParticleSystem>();
                        this.<j>__12 = 0;
                        while (this.<j>__12 < this.<psArray>__11.Length)
                        {
                            if (((this.<psArray>__11[this.<j>__12] != null) && this.<psArray>__11[this.<j>__12].gameObject.activeSelf) && (this.<psArray>__11[this.<j>__12].transform.parent != null))
                            {
                                MonoSingleton<SceneMgr>.GetInstance().AddCulling(this.<psArray>__11[this.<j>__12].transform.gameObject, "ParticleCulling_" + this.<j>__12);
                            }
                            this.<j>__12++;
                        }
                    }
                }
                this.<i>__1++;
            Label_034B:
                if (this.<i>__1 < this.<>f__this.levelArtistList.Count)
                {
                    this.<artAssetNameHigh>__2 = this.<>f__this.levelArtistList[this.<i>__1] + "/" + this.<>f__this.levelArtistList[this.<i>__1];
                    this.<artAssetNameMid>__3 = this.<artAssetNameHigh>__2.Replace("_High", "_Mid");
                    this.<artAssetNameLow>__4 = this.<artAssetNameHigh>__2.Replace("_High", "_Low");
                    this.<levelNames>__5 = new string[] { this.<artAssetNameHigh>__2, this.<artAssetNameMid>__3, this.<artAssetNameLow>__4 };
                    this.<levelArtist>__6 = null;
                    this.<fullPath>__7 = string.Empty;
                    this.<lod>__8 = GameSettings.ModelLOD;
                    this.<lod>__8 = Mathf.Clamp(this.<lod>__8, 0, 2);
                    while (this.<lod>__8 >= 0)
                    {
                        this.<fullPath>__7 = "SceneExport/Artist/" + this.<levelNames>__5[this.<lod>__8] + ".asset";
                        this.<levelArtist>__6 = (LevelResAsset) Singleton<CResourceManager>.GetInstance().GetResource(this.<fullPath>__7, typeof(LevelResAsset), enResourceType.BattleScene, false, false).m_content;
                        if (null != this.<levelArtist>__6)
                        {
                            break;
                        }
                        this.<lod>__8--;
                    }
                    goto Label_018B;
                }
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
                this.<duty>__13 = this.InWrapper.duty;
                this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__13;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$current = 0;
                this.$PC = 1;
                return true;
            Label_03DF:
                return false;
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

        [CompilerGenerated]
        private sealed class <LoadCommonAssetBundle>c__IteratorF : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__0;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Hero_CommonRes.assetbundle");
                        this.$current = 0;
                        this.$PC = 1;
                        goto Label_0168;

                    case 1:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect1.assetbundle");
                        this.$current = 0;
                        this.$PC = 2;
                        goto Label_0168;

                    case 2:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect2.assetbundle");
                        this.$current = 0;
                        this.$PC = 3;
                        goto Label_0168;

                    case 3:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Systems_Effects.assetbundle");
                        this.$current = 0;
                        this.$PC = 4;
                        goto Label_0168;

                    case 4:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Talent.assetbundle");
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Map.assetbundle");
                        this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 5;
                        goto Label_0168;

                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0168:
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

        [CompilerGenerated]
        private sealed class <LoadCommonAssets>c__Iterator13 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__2;
            internal int <i>__1;
            internal int <oldProgress>__0;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        while (this.<i>__1 < this.<>f__this.soundBankList.Count)
                        {
                            Singleton<CSoundManager>.instance.LoadBank(this.<>f__this.soundBankList[this.<i>__1], CSoundManager.BankType.Battle);
                            this.<i>__1++;
                        }
                        this.$current = 0;
                        this.$PC = 1;
                        goto Label_011A;

                    case 1:
                        MonoSingleton<SceneMgr>.instance.PreloadCommonAssets();
                        this.<duty>__2 = this.InWrapper.duty;
                        this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__2;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 2;
                        goto Label_011A;

                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_011A:
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

        [CompilerGenerated]
        private sealed class <LoadCommonEffect>c__Iterator10 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__0;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        MonoSingleton<SceneMgr>.instance.PreloadCommonEffects();
                        this.$current = 0;
                        this.$PC = 1;
                        goto Label_00C4;

                    case 1:
                        this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 2;
                        goto Label_00C4;

                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00C4:
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

        [CompilerGenerated]
        private sealed class <LoadDesignLevel>c__Iterator12 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal CBinaryObject <binaryObject>__6;
            internal string <desgineAssetNameHigh>__2;
            internal string <desgineAssetNameLow>__4;
            internal string <desgineAssetNameMid>__3;
            internal GameObject <designRoot>__9;
            internal int <duty>__11;
            internal string <fullPath>__7;
            internal int <i>__1;
            internal string[] <levelNames>__5;
            internal int <lod>__8;
            internal int <oldProgress>__0;
            internal Transform <staticRoot>__10;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        goto Label_026B;

                    case 1:
                        this.$PC = -1;
                        goto Label_0309;

                    default:
                        goto Label_0309;
                }
            Label_018B:
                if (null == this.<binaryObject>__6)
                {
                    UnityEngine.Debug.LogError("错误，没有找到导出的策划场景" + this.<desgineAssetNameHigh>__2);
                }
                else
                {
                    this.<designRoot>__9 = GameLoader.s_serializer.Load(this.<binaryObject>__6.m_data);
                    if (null == this.<designRoot>__9)
                    {
                        UnityEngine.Debug.LogError("策划场景SceneExport/Design/" + this.<>f__this.levelDesignList[this.<i>__1] + ".bytes有错误！请检查！");
                    }
                    else
                    {
                        this.<staticRoot>__10 = this.<designRoot>__9.transform.Find("StaticMesh");
                        if (null != this.<staticRoot>__10)
                        {
                            StaticBatchingUtility.Combine(this.<staticRoot>__10.gameObject);
                        }
                        Singleton<CResourceManager>.GetInstance().RemoveCachedResource(this.<fullPath>__7);
                    }
                }
                this.<i>__1++;
            Label_026B:
                if (this.<i>__1 < this.<>f__this.levelDesignList.Count)
                {
                    this.<desgineAssetNameHigh>__2 = this.<>f__this.levelDesignList[this.<i>__1] + "/" + this.<>f__this.levelDesignList[this.<i>__1];
                    this.<desgineAssetNameMid>__3 = this.<desgineAssetNameHigh>__2.Replace("_High", "_Mid");
                    this.<desgineAssetNameLow>__4 = this.<desgineAssetNameHigh>__2.Replace("_High", "_Low");
                    this.<levelNames>__5 = new string[] { this.<desgineAssetNameHigh>__2, this.<desgineAssetNameMid>__3, this.<desgineAssetNameLow>__4 };
                    this.<binaryObject>__6 = null;
                    this.<fullPath>__7 = string.Empty;
                    this.<lod>__8 = GameSettings.ModelLOD;
                    this.<lod>__8 = Mathf.Clamp(this.<lod>__8, 0, 2);
                    while (this.<lod>__8 >= 0)
                    {
                        this.<fullPath>__7 = "SceneExport/Design/" + this.<levelNames>__5[this.<lod>__8] + ".bytes";
                        this.<binaryObject>__6 = Singleton<CResourceManager>.GetInstance().GetResource(this.<fullPath>__7, typeof(TextAsset), enResourceType.BattleScene, false, false).m_content as CBinaryObject;
                        if (null != this.<binaryObject>__6)
                        {
                            break;
                        }
                        this.<lod>__8--;
                    }
                    goto Label_018B;
                }
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
                Singleton<SceneManagement>.GetInstance().InitScene();
                this.<duty>__11 = this.InWrapper.duty;
                this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__11;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$current = 0;
                this.$PC = 1;
                return true;
            Label_0309:
                return false;
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

        [CompilerGenerated]
        private sealed class <LoadNoActorAssets>c__Iterator15 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal AGE.Action <action>__25;
            internal string <assetPath>__12;
            internal int <count>__8;
            internal int <currentParticleLOD>__13;
            internal int <duty>__1;
            internal GameObject <gameObj>__22;
            internal int <i>__24;
            internal int <i>__5;
            internal int <index>__9;
            internal int <j>__10;
            internal int <j>__17;
            internal int <j>__20;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal int <lod>__14;
            internal ActorMeta <meta>__6;
            internal int <oldProgress>__3;
            internal ActorPreloadTab <otherLoad>__4;
            internal string <parPathKey>__15;
            internal string <parPathKey>__16;
            internal string <path>__7;
            internal Dictionary<object, AssetRefType> <refAssets>__26;
            internal SpriteRenderer <sr>__23;
            internal CResource <tempObj>__21;
            internal ListView<Texture> <textures>__19;
            internal GameObject <tmpObj>__11;
            internal GameObject <tmpObj>__18;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<oldProgress>__3 = this.<>f__this.nProgress;
                        this.<otherLoad>__4 = this.<loadHelper>__2.GetGlobalPreload();
                        this.<i>__5 = 0;
                        while (this.<i>__5 < this.<>f__this.actorList.Count)
                        {
                            this.<meta>__6 = this.<>f__this.actorList[this.<i>__5];
                            if (this.<meta>__6.ActorType == ActorTypeDef.Actor_Type_Hero)
                            {
                                this.<path>__7 = KillNotifyUT.GetHero_Icon((uint) this.<meta>__6.ConfigId, 0, false);
                                if (!string.IsNullOrEmpty(this.<path>__7))
                                {
                                    this.<otherLoad>__4.AddSprite(this.<path>__7);
                                }
                            }
                            this.<i>__5++;
                        }
                        this.<count>__8 = ((this.<otherLoad>__4.parPrefabs.Count + this.<otherLoad>__4.mesPrefabs.Count) + this.<otherLoad>__4.spritePrefabs.Count) + this.<otherLoad>__4.ageActions.Count;
                        this.<index>__9 = 0;
                        this.<j>__10 = 0;
                        while (this.<j>__10 < this.<otherLoad>__4.parPrefabs.Count)
                        {
                            this.<tmpObj>__11 = null;
                            AssetLoadBase base2 = this.<otherLoad>__4.parPrefabs[this.<j>__10];
                            this.<assetPath>__12 = base2.assetPath;
                            if (!this.<lhc>__0.HasLoaded(this.<assetPath>__12))
                            {
                                if (GameSettings.DynamicParticleLOD)
                                {
                                    this.<currentParticleLOD>__13 = GameSettings.ParticleLOD;
                                    this.<lod>__14 = this.<currentParticleLOD>__13;
                                    while (this.<lod>__14 <= 2)
                                    {
                                        this.<parPathKey>__15 = this.<assetPath>__12 + "_lod_" + this.<lod>__14;
                                        if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__15))
                                        {
                                            GameSettings.ParticleLOD = this.<lod>__14;
                                            this.<tmpObj>__11 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__12, true, SceneObjType.ActionRes, Vector3.zero);
                                            this.<lhc>__0.AddObj(this.<parPathKey>__15, this.<tmpObj>__11);
                                        }
                                        this.<lod>__14++;
                                    }
                                    GameSettings.ParticleLOD = this.<currentParticleLOD>__13;
                                }
                                else
                                {
                                    this.<parPathKey>__16 = this.<assetPath>__12;
                                    if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__16))
                                    {
                                        this.<tmpObj>__11 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__12, true, SceneObjType.ActionRes, Vector3.zero);
                                        this.<lhc>__0.AddObj(this.<parPathKey>__16, this.<tmpObj>__11);
                                    }
                                }
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_033D;
                            }
                            this.$current = 0;
                            this.$PC = 1;
                            goto Label_0786;
                        Label_0314:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__9, this.<count>__8);
                        Label_033D:
                            this.<j>__10++;
                        }
                        this.<j>__17 = 0;
                        while (this.<j>__17 < this.<otherLoad>__4.mesPrefabs.Count)
                        {
                            this.<tmpObj>__18 = null;
                            AssetLoadBase base3 = this.<otherLoad>__4.mesPrefabs[this.<j>__17];
                            if (!this.<lhc>__0.HasLoaded(base3.assetPath))
                            {
                                AssetLoadBase base4 = this.<otherLoad>__4.mesPrefabs[this.<j>__17];
                                this.<tmpObj>__18 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                                AssetLoadBase base5 = this.<otherLoad>__4.mesPrefabs[this.<j>__17];
                                this.<lhc>__0.AddObj(base5.assetPath, this.<tmpObj>__18);
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_046B;
                            }
                            this.$current = 0;
                            this.$PC = 2;
                            goto Label_0786;
                        Label_0442:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__9, this.<count>__8);
                        Label_046B:
                            this.<j>__17++;
                        }
                        this.<textures>__19 = new ListView<Texture>();
                        this.<j>__20 = 0;
                        while (this.<j>__20 < this.<otherLoad>__4.spritePrefabs.Count)
                        {
                            AssetLoadBase base6 = this.<otherLoad>__4.spritePrefabs[this.<j>__20];
                            this.<tempObj>__21 = Singleton<CResourceManager>.instance.GetResource(base6.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                            if (((this.<tempObj>__21 != null) && (this.<tempObj>__21.m_content != null)) && (this.<tempObj>__21.m_content is GameObject))
                            {
                                this.<gameObj>__22 = (GameObject) this.<tempObj>__21.m_content;
                                this.<sr>__23 = this.<gameObj>__22.GetComponent<SpriteRenderer>();
                                if (((this.<sr>__23 != null) && (this.<sr>__23.sprite != null)) && (this.<sr>__23.sprite.texture != null))
                                {
                                    this.<textures>__19.Add(this.<sr>__23.sprite.texture);
                                }
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_0600;
                            }
                            this.$current = 0;
                            this.$PC = 3;
                            goto Label_0786;
                        Label_05D7:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__9, this.<count>__8);
                        Label_0600:
                            this.<j>__20++;
                        }
                        this.<textures>__19.Clear();
                        this.<i>__24 = 0;
                        while (this.<i>__24 < this.<otherLoad>__4.ageActions.Count)
                        {
                            AssetLoadBase base7 = this.<otherLoad>__4.ageActions[this.<i>__24];
                            this.<action>__25 = MonoSingleton<ActionManager>.instance.LoadActionResource(base7.assetPath);
                            if (this.<action>__25 != null)
                            {
                                this.<refAssets>__26 = this.<loadHelper>__2.GetRefAssets(0, 0);
                                this.<action>__25.GetAssociatedResources(this.<refAssets>__26, 0);
                            }
                            this.<index>__9++;
                            if (!this.<>f__this.ShouldYieldReturn())
                            {
                                goto Label_06FE;
                            }
                            this.$current = 0;
                            this.$PC = 4;
                            goto Label_0786;
                        Label_06D5:
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__9, this.<count>__8);
                        Label_06FE:
                            this.<i>__24++;
                        }
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$current = 0;
                        this.$PC = 5;
                        goto Label_0786;

                    case 1:
                        goto Label_0314;

                    case 2:
                        goto Label_0442;

                    case 3:
                        goto Label_05D7;

                    case 4:
                        goto Label_06D5;

                    case 5:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0786:
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

        [CompilerGenerated]
        private sealed class <PreSpawnSoldiers>c__Iterator19 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal ActorMeta <actorMeta>__9;
            internal string <actorName>__11;
            internal int <count>__10;
            internal int <duty>__1;
            internal int <i>__3;
            internal int <i>__7;
            internal int <j>__12;
            internal PoolObjHandle<ActorRoot> <monster>__13;
            internal ActorRoot <monsterActor>__14;
            internal int <num>__5;
            internal int <oldProgress>__0;
            internal ActorPreloadTab <preloadTab>__4;
            internal ActorPreloadTab <preloadTab>__8;
            internal int <spawnCountTotal>__2;
            internal int <spawnIndex>__6;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<spawnCountTotal>__2 = 0;
                        this.<i>__3 = 0;
                        while (this.<i>__3 < this.<>f__this.actorPreload.Count)
                        {
                            this.<preloadTab>__4 = this.<>f__this.actorPreload[this.<i>__3];
                            this.<num>__5 = Mathf.Max(Mathf.RoundToInt(this.<preloadTab>__4.spawnCnt), 1);
                            if (this.<preloadTab>__4.theActor.ActorType == ActorTypeDef.Actor_Type_Monster)
                            {
                                this.<spawnCountTotal>__2 += this.<num>__5;
                            }
                            this.<i>__3++;
                        }
                        GameObjMgr.isPreSpawnActors = true;
                        this.<spawnIndex>__6 = 0;
                        this.<i>__7 = 0;
                        while (this.<i>__7 < this.<>f__this.actorPreload.Count)
                        {
                            this.<preloadTab>__8 = this.<>f__this.actorPreload[this.<i>__7];
                            this.<actorMeta>__9 = this.<preloadTab>__8.theActor;
                            if (this.<actorMeta>__9.ActorType == ActorTypeDef.Actor_Type_Monster)
                            {
                                this.<count>__10 = Mathf.Max(Mathf.RoundToInt(this.<preloadTab>__8.spawnCnt), 1);
                                this.<actorName>__11 = null;
                                this.<j>__12 = 0;
                                while (this.<j>__12 < this.<count>__10)
                                {
                                    this.<monster>__13 = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref this.<actorMeta>__9, VInt3.zero, VInt3.forward, false, true);
                                    if (this.<monster>__13 != 0)
                                    {
                                        this.<monsterActor>__14 = this.<monster>__13.handle;
                                        this.<monsterActor>__14.InitActor();
                                        this.<monsterActor>__14.PrepareFight();
                                        this.<monsterActor>__14.gameObject.name = this.<monsterActor>__14.TheStaticData.TheResInfo.Name;
                                        this.<monsterActor>__14.StartFight();
                                        if (this.<actorName>__11 == null)
                                        {
                                            this.<actorName>__11 = this.<monsterActor>__14.TheStaticData.TheResInfo.Name;
                                        }
                                        Singleton<GameObjMgr>.instance.AddToCache(this.<monster>__13);
                                    }
                                    if (!this.<>f__this.ShouldYieldReturn())
                                    {
                                        goto Label_0282;
                                    }
                                    this.$current = 0;
                                    this.$PC = 1;
                                    goto Label_033E;
                                Label_0253:
                                    this.<>f__this.UpdateProgress(null, this.<oldProgress>__0, this.<duty>__1, ++this.<spawnIndex>__6, this.<spawnCountTotal>__2);
                                Label_0282:
                                    this.<j>__12++;
                                }
                            }
                            this.<i>__7++;
                        }
                        this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__1;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = 0;
                        this.$PC = 2;
                        goto Label_033E;

                    case 1:
                        goto Label_0253;

                    case 2:
                        GameObjMgr.isPreSpawnActors = false;
                        HudComponent3D.PreallocMapPointer(20, 40);
                        SObjPool<SRefParam>.Alloc(0x400);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_033E:
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

        [CompilerGenerated]
        private sealed class <SpawnDynamicActor>c__Iterator18 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal PoolObjHandle<ActorRoot> <actor>__6;
            internal ActorMeta <actorMeta>__3;
            internal VInt3 <bornDir>__5;
            internal VInt3 <bornPos>__4;
            internal int <duty>__0;
            internal int <i>__2;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        this.<i>__2 = 0;
                        break;

                    case 1:
                        this.<i>__2++;
                        break;

                    default:
                        goto Label_01F1;
                }
                if (this.<i>__2 < this.<>f__this.actorList.Count)
                {
                    this.<actorMeta>__3 = this.<>f__this.actorList[this.<i>__2];
                    this.<bornPos>__4 = new VInt3();
                    this.<bornDir>__5 = new VInt3();
                    if (this.<actorMeta>__3.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        DebugHelper.Assert(Singleton<BattleLogic>.instance.mapLogic != null, "what? BattleLogic.instance.mapLogic==null");
                        Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.<actorMeta>__3, true, out this.<bornPos>__4, out this.<bornDir>__5);
                    }
                    this.<actor>__6 = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref this.<actorMeta>__3, this.<bornPos>__4, this.<bornDir>__5, false, true);
                    if (this.<actor>__6 != 0)
                    {
                        Singleton<GameObjMgr>.GetInstance().HoldDynamicActor(this.<actor>__6);
                    }
                    this.<>f__this.nProgress = this.<oldProgress>__1 + ((this.<duty>__0 * (this.<i>__2 + 1)) / this.<>f__this.actorList.Count);
                    this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                    this.$current = 0;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$PC = -1;
            Label_01F1:
                return false;
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

        [CompilerGenerated]
        private sealed class <SpawnStaticActor>c__Iterator17 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal PoolObjHandle<ActorRoot> <actor>__6;
            internal ActorMeta <actorMeta>__3;
            internal VInt3 <bornDir>__5;
            internal VInt3 <bornPos>__4;
            internal int <duty>__0;
            internal int <i>__2;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        this.<i>__2 = 0;
                        break;

                    case 1:
                        this.<i>__2++;
                        break;

                    case 2:
                        this.<>f__this.staticActors.Clear();
                        this.$PC = -1;
                        goto Label_0287;

                    default:
                        goto Label_0287;
                }
                if (this.<i>__2 < this.<>f__this.staticActors.Count)
                {
                    this.<actorMeta>__3 = new ActorMeta();
                    this.<actorMeta>__3.ActorType = this.<>f__this.staticActors[this.<i>__2].ActorType;
                    this.<actorMeta>__3.ConfigId = this.<>f__this.staticActors[this.<i>__2].ConfigID;
                    this.<actorMeta>__3.ActorCamp = this.<>f__this.staticActors[this.<i>__2].CmpType;
                    this.<bornPos>__4 = (VInt3) this.<>f__this.staticActors[this.<i>__2].transform.position;
                    this.<bornDir>__5 = (VInt3) this.<>f__this.staticActors[this.<i>__2].transform.forward;
                    this.<actor>__6 = Singleton<GameObjMgr>.instance.SpawnActorEx(this.<>f__this.staticActors[this.<i>__2].gameObject, ref this.<actorMeta>__3, this.<bornPos>__4, this.<bornDir>__5, false, true);
                    if (this.<actor>__6 != 0)
                    {
                        Singleton<GameObjMgr>.GetInstance().HoldStaticActor(this.<actor>__6);
                    }
                    this.<>f__this.nProgress = this.<oldProgress>__1 + ((this.<duty>__0 * (this.<i>__2 + 1)) / this.<>f__this.staticActors.Count);
                    this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                    this.$current = 0;
                    this.$PC = 1;
                }
                else
                {
                    this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                    this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                    this.$current = 0;
                    this.$PC = 2;
                }
                return true;
            Label_0287:
                return false;
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

        [StructLayout(LayoutKind.Sequential)]
        private struct LHCWrapper
        {
            public LoaderHelperCamera lhc;
            public LoaderHelper loadHelper;
            public int duty;
        }

        public delegate void LoadCompleteDelegate();

        [StructLayout(LayoutKind.Sequential)]
        private struct LoaderHelperWrapper
        {
            public LoaderHelper loadHelper;
            public int duty;
        }

        public delegate void LoadProgressDelegate(float progress);
    }
}

