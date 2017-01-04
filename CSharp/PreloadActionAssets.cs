using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using behaviac;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PreloadActionAssets
{
    private DictionaryView<string, AGE.Action> actionDict = new DictionaryView<string, AGE.Action>();
    private Dictionary<string, bool> behaviourDict = new Dictionary<string, bool>();
    private float curTime;
    private float loadTime;
    private float MaxDurationPerFrame = 0.1f;
    private Dictionary<string, bool> prefabDict = new Dictionary<string, bool>();

    private void AddAction(DictionaryView<string, AGE.Action> actions, byte[] actionNameUtf8)
    {
        string key = StringHelper.UTF8BytesToString(ref actionNameUtf8);
        if (((key != null) && !this.actionDict.ContainsKey(key)) && !actions.ContainsKey(key))
        {
            actions.Add(key, null);
        }
    }

    private void AddAction(DictionaryView<string, AGE.Action> actions, string actionName)
    {
        if (((actionName != null) && !this.actionDict.ContainsKey(actionName)) && !actions.ContainsKey(actionName))
        {
            actions.Add(actionName, null);
        }
    }

    private void AddActionsFromActionHelper(DictionaryView<string, AGE.Action> actions)
    {
        ActionHelper[] helperArray = UnityEngine.Object.FindObjectsOfType<ActionHelper>();
        if ((helperArray != null) && (helperArray.Length != 0))
        {
            for (int i = 0; i < helperArray.Length; i++)
            {
                ActionHelper helper = helperArray[i];
                for (int j = 0; j < helper.actionHelpers.Length; j++)
                {
                    ActionHelperStorage storage = helper.actionHelpers[j];
                    this.AddAction(actions, storage.actionName);
                }
            }
        }
    }

    private void AddActionsFromActors(DictionaryView<string, AGE.Action> actions, List<PoolObjHandle<ActorRoot>> actors)
    {
        if (actors != null)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = actors[i];
                ActorRoot root = handle.handle;
                this.AddBehaviorTree(root.CharInfo);
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                ActorStaticSkillData skillData = new ActorStaticSkillData();
                for (int j = 0; j < 8; j++)
                {
                    actorDataProvider.GetActorStaticSkillData(ref root.TheActorMeta, (ActorSkillSlot) j, ref skillData);
                    if (skillData.SkillId != 0)
                    {
                        this.AddActionsFromSkill(actions, skillData.SkillId);
                        this.AddActionsFromPassiveSkill(actions, skillData.PassiveSkillId);
                    }
                }
            }
        }
    }

    private void AddActionsFromAreaTrigger(DictionaryView<string, AGE.Action> actions)
    {
        AreaTrigger[] triggerArray = UnityEngine.Object.FindObjectsOfType<AreaTrigger>();
        if ((triggerArray != null) && (triggerArray.Length != 0))
        {
            for (int i = 0; i < triggerArray.Length; i++)
            {
                AreaTrigger trigger = triggerArray[i];
                this.AddActionsFromSkillCombine(actions, trigger.BuffID);
                this.AddActionsFromSkillCombine(actions, trigger.UpdateBuffID);
            }
        }
    }

    private void AddActionsFromPassiveSkill(DictionaryView<string, AGE.Action> actions, int passiveSkillID)
    {
        if (passiveSkillID > 0)
        {
            ResSkillPassiveCfgInfo dataByKey = GameDataMgr.skillPassiveDatabin.GetDataByKey((long) passiveSkillID);
            if (dataByKey != null)
            {
                this.AddAction(actions, dataByKey.szActionName);
            }
        }
    }

    private void AddActionsFromRandPassiveSkill(DictionaryView<string, AGE.Action> actions, int randPassiveSkillID)
    {
        if (randPassiveSkillID > 0)
        {
            ResRandomSkillPassiveRule dataByKey = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long) randPassiveSkillID);
            if (dataByKey != null)
            {
                if ((dataByKey.astRandomSkillPassiveID1 != null) && (dataByKey.astRandomSkillPassiveID1.Length > 0))
                {
                    for (int i = 0; i < dataByKey.astRandomSkillPassiveID1.Length; i++)
                    {
                        this.AddActionsFromPassiveSkill(actions, dataByKey.astRandomSkillPassiveID1[i].iParam);
                    }
                }
                if ((dataByKey.astRandomSkillPassiveID2 != null) && (dataByKey.astRandomSkillPassiveID2.Length > 0))
                {
                    for (int j = 0; j < dataByKey.astRandomSkillPassiveID2.Length; j++)
                    {
                        this.AddActionsFromPassiveSkill(actions, dataByKey.astRandomSkillPassiveID2[j].iParam);
                    }
                }
            }
        }
    }

    private void AddActionsFromSkill(DictionaryView<string, AGE.Action> actions, int skillID)
    {
        if (skillID > 0)
        {
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) skillID);
            if (dataByKey != null)
            {
                this.AddAction(actions, dataByKey.szPrefab);
                string key = StringHelper.UTF8BytesToString(ref dataByKey.szGuidePrefab);
                if ((key != null) && !this.prefabDict.ContainsKey(key))
                {
                    this.prefabDict.Add(key, false);
                }
                if (dataByKey.dwIsBullet != 0)
                {
                    ResBulletCfgInfo info2 = GameDataMgr.bulletDatabin.GetDataByKey((long) dataByKey.iBulletID);
                    if (info2 != null)
                    {
                        this.AddAction(actions, info2.szPrefab);
                    }
                }
            }
        }
    }

    private void AddActionsFromSkillCombine(DictionaryView<string, AGE.Action> actions, int skillCombineID)
    {
        if (skillCombineID > 0)
        {
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) skillCombineID);
            if (dataByKey != null)
            {
                this.AddAction(actions, dataByKey.szPrefab);
            }
        }
    }

    private void AddActionsFromSoldier(DictionaryView<string, AGE.Action> actions, uint soldierID)
    {
        ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int) soldierID);
        if (dataCfgInfoByCurLevelDiff != null)
        {
            CActorInfo actorInfo = CActorInfo.GetActorInfo(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), enResourceType.BattleScene);
            if (actorInfo != null)
            {
                this.AddBehaviorTree(actorInfo);
                string artPrefabName = actorInfo.GetArtPrefabName(0, -1);
                if (!this.prefabDict.ContainsKey(artPrefabName))
                {
                    this.prefabDict.Add(artPrefabName, false);
                }
                if ((dataCfgInfoByCurLevelDiff.SkillIDs != null) && (dataCfgInfoByCurLevelDiff.SkillIDs.Length > 0))
                {
                    for (int i = 0; i < dataCfgInfoByCurLevelDiff.SkillIDs.Length; i++)
                    {
                        int skillID = dataCfgInfoByCurLevelDiff.SkillIDs[i];
                        this.AddActionsFromSkill(actions, skillID);
                    }
                }
            }
        }
    }

    private void AddActionsFromSoldierRegions(DictionaryView<string, AGE.Action> actions)
    {
        SoldierRegion[] regionArray = UnityEngine.Object.FindObjectsOfType<SoldierRegion>();
        if ((regionArray != null) && (regionArray.Length != 0))
        {
            for (int i = 0; i < regionArray.Length; i++)
            {
                SoldierRegion region = regionArray[i];
                if (((region != null) && (region.Waves != null)) && (region.Waves.Count > 0))
                {
                    for (int j = 0; j < region.Waves.Count; j++)
                    {
                        SoldierWave wave = region.Waves[j];
                        if (((wave != null) && (wave.WaveInfo != null)) && ((wave.WaveInfo.astNormalSoldierInfo != null) && (wave.WaveInfo.astNormalSoldierInfo.Length > 0)))
                        {
                            for (int k = 0; k < wave.WaveInfo.astNormalSoldierInfo.Length; k++)
                            {
                                ResSoldierTypeInfo info = wave.WaveInfo.astNormalSoldierInfo[k];
                                this.AddActionsFromSoldier(actions, info.dwSoldierID);
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddActionsFromSpawnPoints(DictionaryView<string, AGE.Action> actions)
    {
        SpawnPoint[] pointArray = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
        if ((pointArray != null) && (pointArray.Length != 0))
        {
            for (int i = 0; i < pointArray.Length; i++)
            {
                SpawnPoint point = pointArray[i];
                if ((point != null) && (point.TheActorsMeta.Length > 0))
                {
                    for (int j = 0; j < point.TheActorsMeta.Length; j++)
                    {
                        ActorMeta meta = point.TheActorsMeta[j];
                        if (meta.ConfigId > 0)
                        {
                            switch (meta.ActorType)
                            {
                                case ActorTypeDef.Actor_Type_Monster:
                                    this.AddActionsFromSoldier(actions, (uint) meta.ConfigId);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddBehaviorTree(CActorInfo info)
    {
        if ((((info != null) && (info.BtResourcePath != null)) && (info.BtResourcePath.Length != 0)) && !this.behaviourDict.ContainsKey(info.BtResourcePath))
        {
            this.behaviourDict.Add(info.BtResourcePath, false);
        }
    }

    private void AddReferencedAssets(DictionaryView<string, AGE.Action> actions, Dictionary<object, AssetRefType> actionAssets)
    {
        Dictionary<object, AssetRefType>.Enumerator enumerator = actionAssets.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<object, AssetRefType> current = enumerator.Current;
            AssetRefType type = current.Value;
            switch (type)
            {
                case AssetRefType.Action:
                {
                    KeyValuePair<object, AssetRefType> pair2 = enumerator.Current;
                    string key = pair2.Key as string;
                    if (!this.actionDict.ContainsKey(key) && !actions.ContainsKey(key))
                    {
                        actions.Add(key, null);
                    }
                    continue;
                }
                case AssetRefType.SkillID:
                {
                    KeyValuePair<object, AssetRefType> pair5 = enumerator.Current;
                    int skillID = (int) pair5.Key;
                    this.AddActionsFromSkill(actions, skillID);
                    continue;
                }
                case AssetRefType.SkillCombine:
                {
                    KeyValuePair<object, AssetRefType> pair4 = enumerator.Current;
                    int skillCombineID = (int) pair4.Key;
                    this.AddActionsFromSkillCombine(actions, skillCombineID);
                    continue;
                }
                case AssetRefType.Prefab:
                case AssetRefType.Particle:
                {
                    KeyValuePair<object, AssetRefType> pair3 = enumerator.Current;
                    string str2 = pair3.Key as string;
                    if (!this.prefabDict.ContainsKey(str2))
                    {
                        this.prefabDict.Add(str2, type == AssetRefType.Particle);
                    }
                    continue;
                }
                case AssetRefType.MonsterConfigId:
                {
                    KeyValuePair<object, AssetRefType> pair6 = enumerator.Current;
                    int num3 = (int) pair6.Key;
                    this.AddActionsFromSoldier(actions, (uint) num3);
                    continue;
                }
            }
        }
    }

    public void AddRefPrefab(string prefabName, bool isParticle)
    {
        prefabName = CFileManager.EraseExtension(prefabName);
        if (!this.prefabDict.ContainsKey(prefabName))
        {
            this.prefabDict.Add(prefabName, isParticle);
        }
    }

    [DebuggerHidden]
    public IEnumerator Exec()
    {
        return new <Exec>c__Iterator1B { <>f__this = this };
    }

    private bool shouldWaitForNextFrame()
    {
        float num = Time.realtimeSinceStartup - this.curTime;
        if (num >= this.MaxDurationPerFrame)
        {
            this.loadTime += num;
            return true;
        }
        return false;
    }

    [CompilerGenerated]
    private sealed class <Exec>c__Iterator1B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal PreloadActionAssets <>f__this;
        internal AGE.Action <action>__5;
        internal Dictionary<object, AssetRefType> <actionAssets>__2;
        internal string <actionName>__4;
        internal DictionaryView<string, AGE.Action> <actions>__1;
        internal DictionaryView<string, AGE.Action>.Enumerator <etr>__3;
        internal int <frameCount>__19;
        internal int <i>__16;
        internal int <i>__18;
        internal List<GameObject> <instantiatedObjs>__15;
        internal Vector3 <invalidPos>__14;
        internal bool <isParticle>__11;
        internal GameObject <obj>__17;
        internal string <path>__7;
        internal string <prefabName>__10;
        internal GameObject <prefabObj>__13;
        internal List<GameObject> <prefabObjs>__8;
        internal string <prefabRealPath>__12;
        internal Dictionary<string, bool>.Enumerator <r>__6;
        internal Dictionary<string, bool>.Enumerator <r>__9;
        internal int <startFrame>__0;

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
                    goto Label_05BC;

                case 1:
                    this.<>f__this.curTime = Time.realtimeSinceStartup;
                    this.<startFrame>__0 = Time.frameCount;
                    if (ActionManager.Instance == null)
                    {
                        goto Label_05B3;
                    }
                    this.<actions>__1 = new DictionaryView<string, AGE.Action>();
                    this.<actionAssets>__2 = new Dictionary<object, AssetRefType>();
                    this.<>f__this.AddActionsFromActors(this.<actions>__1, Singleton<GameObjMgr>.GetInstance().DynamicActors);
                    this.<>f__this.AddActionsFromActors(this.<actions>__1, Singleton<GameObjMgr>.GetInstance().StaticActors);
                    this.<>f__this.AddActionsFromSpawnPoints(this.<actions>__1);
                    this.<>f__this.AddActionsFromSoldierRegions(this.<actions>__1);
                    this.<>f__this.AddActionsFromAreaTrigger(this.<actions>__1);
                    this.<>f__this.AddActionsFromActionHelper(this.<actions>__1);
                    break;

                case 2:
                    goto Label_01CD;

                case 3:
                    this.<>f__this.curTime = Time.realtimeSinceStartup;
                    break;

                case 4:
                    this.<>f__this.curTime = Time.realtimeSinceStartup;
                    goto Label_029B;

                case 5:
                    goto Label_0304;

                case 6:
                    goto Label_03F0;

                case 7:
                    goto Label_04C8;

                case 8:
                    this.<i>__18 = 0;
                    while (this.<i>__18 < this.<instantiatedObjs>__15.Count)
                    {
                        UnityEngine.Object.Destroy(this.<instantiatedObjs>__15[this.<i>__18]);
                        this.<i>__18++;
                    }
                    this.<instantiatedObjs>__15.Clear();
                    this.$current = 0;
                    this.$PC = 9;
                    goto Label_05BC;

                case 9:
                    this.<>f__this.loadTime += Time.realtimeSinceStartup - this.<>f__this.curTime;
                    this.<frameCount>__19 = Time.frameCount - this.<startFrame>__0;
                    goto Label_05B3;

                default:
                    goto Label_05BA;
            }
            while (this.<actions>__1.Count > 0)
            {
                this.<etr>__3 = this.<actions>__1.GetEnumerator();
                while (this.<etr>__3.MoveNext())
                {
                    this.<actionName>__4 = this.<etr>__3.Current.Key;
                    if (this.<>f__this.actionDict.ContainsKey(this.<actionName>__4))
                    {
                        continue;
                    }
                    this.<action>__5 = ActionManager.Instance.LoadActionResource(this.<actionName>__4);
                    this.<>f__this.actionDict.Add(this.<actionName>__4, this.<action>__5);
                    if (this.<action>__5 != null)
                    {
                    }
                    if (!this.<>f__this.shouldWaitForNextFrame())
                    {
                        continue;
                    }
                    this.$current = 0;
                    this.$PC = 2;
                    goto Label_05BC;
                Label_01CD:
                    this.<>f__this.curTime = Time.realtimeSinceStartup;
                }
                this.<actions>__1.Clear();
                this.<>f__this.AddReferencedAssets(this.<actions>__1, this.<actionAssets>__2);
                this.<actionAssets>__2.Clear();
                if (this.<>f__this.shouldWaitForNextFrame())
                {
                    this.$current = 0;
                    this.$PC = 3;
                    goto Label_05BC;
                }
            }
            if (this.<>f__this.shouldWaitForNextFrame())
            {
                this.$current = 0;
                this.$PC = 4;
                goto Label_05BC;
            }
        Label_029B:
            this.<r>__6 = this.<>f__this.behaviourDict.GetEnumerator();
            while (this.<r>__6.MoveNext())
            {
                this.<path>__7 = this.<r>__6.Current.Key;
                Workspace.Load(this.<path>__7, false);
                if (!this.<>f__this.shouldWaitForNextFrame())
                {
                    continue;
                }
                this.$current = 0;
                this.$PC = 5;
                goto Label_05BC;
            Label_0304:
                this.<>f__this.curTime = Time.realtimeSinceStartup;
            }
            this.<prefabObjs>__8 = new List<GameObject>();
            this.<r>__9 = this.<>f__this.prefabDict.GetEnumerator();
            while (this.<r>__9.MoveNext())
            {
                this.<prefabName>__10 = this.<r>__9.Current.Key;
                this.<isParticle>__11 = this.<r>__9.Current.Value;
                this.<prefabRealPath>__12 = null;
                this.<prefabObj>__13 = MonoSingleton<SceneMgr>.GetInstance().GetPrefabLOD<GameObject>(this.<prefabName>__10, this.<isParticle>__11, out this.<prefabRealPath>__12);
                if (this.<prefabObj>__13 != null)
                {
                    this.<prefabObjs>__8.Add(this.<prefabObj>__13);
                }
                if (!this.<>f__this.shouldWaitForNextFrame())
                {
                    continue;
                }
                this.$current = 0;
                this.$PC = 6;
                goto Label_05BC;
            Label_03F0:
                this.<>f__this.curTime = Time.realtimeSinceStartup;
            }
            this.<invalidPos>__14 = new Vector3(9999f, 9999f, 9999f);
            this.<instantiatedObjs>__15 = new List<GameObject>();
            this.<i>__16 = 0;
            while (this.<i>__16 < this.<prefabObjs>__8.Count)
            {
                this.<obj>__17 = UnityEngine.Object.Instantiate(this.<prefabObjs>__8[this.<i>__16], this.<invalidPos>__14, Quaternion.identity) as GameObject;
                if (this.<obj>__17 != null)
                {
                    MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.<obj>__17, SceneObjType.Temp);
                    this.<instantiatedObjs>__15.Add(this.<obj>__17);
                }
                if (!this.<>f__this.shouldWaitForNextFrame())
                {
                    goto Label_04D8;
                }
                this.$current = 0;
                this.$PC = 7;
                goto Label_05BC;
            Label_04C8:
                this.<>f__this.curTime = Time.realtimeSinceStartup;
            Label_04D8:
                this.<i>__16++;
            }
            this.$current = 0;
            this.$PC = 8;
            goto Label_05BC;
        Label_05B3:
            this.$PC = -1;
        Label_05BA:
            return false;
        Label_05BC:
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

