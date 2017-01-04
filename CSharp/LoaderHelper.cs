using AGE;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LoaderHelper
{
    public Dictionary<string, bool> ageCheckerSet = new Dictionary<string, bool>();
    public Dictionary<long, Dictionary<object, AssetRefType>> ageRefAssets2 = new Dictionary<long, Dictionary<object, AssetRefType>>();
    public Dictionary<string, bool> behaviorXmlSet = new Dictionary<string, bool>();
    public Dictionary<int, bool> randomSkillCheckerSet = new Dictionary<int, bool>();
    private static int retCnt;

    public void AddPreloadActor(ref List<ActorPreloadTab> list, ref ActorMeta actorMeta, float spawnCnt, int ownerSkinID = 0)
    {
        ActorStaticData actorData = new ActorStaticData();
        IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
        actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
        ActorServerData data2 = new ActorServerData();
        IGameActorDataProvider provider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
        provider2.GetActorServerData(ref actorMeta, ref data2);
        CActorInfo actorInfo = CActorInfo.GetActorInfo(actorData.TheResInfo.ResPath, enResourceType.BattleScene);
        if (actorInfo != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ActorPreloadTab tab = list[i];
                if ((tab.theActor.ActorType == actorMeta.ActorType) && (tab.theActor.ConfigId == actorMeta.ConfigId))
                {
                    if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        if (this.GetActorMarkID(actorMeta) != tab.MarkID)
                        {
                            continue;
                        }
                        uint skillID = 0;
                        if (provider2.GetActorServerCommonSkillData(ref actorMeta, out skillID))
                        {
                            this.AnalyseSkill(ref tab, (int) skillID);
                        }
                    }
                    else if ((actorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (spawnCnt > 0f))
                    {
                        tab.spawnCnt += spawnCnt;
                        list[i] = tab;
                    }
                    retCnt++;
                    return;
                }
            }
            ActorPreloadTab loadInfo = new ActorPreloadTab {
                theActor = actorMeta
            };
            loadInfo.modelPrefab.assetPath = actorInfo.GetArtPrefabName((ownerSkinID == 0) ? ((int) data2.SkinId) : ownerSkinID, -1);
            loadInfo.modelPrefab.nInstantiate = 1;
            loadInfo.spawnCnt = spawnCnt;
            loadInfo.MarkID = this.GetActorMarkID(actorMeta);
            loadInfo.ageActions = new List<AssetLoadBase>();
            loadInfo.parPrefabs = new List<AssetLoadBase>();
            loadInfo.mesPrefabs = new List<AssetLoadBase>();
            if (data2.SkinId != 0)
            {
                actorInfo.PreLoadAdvanceSkin(loadInfo.mesPrefabs, data2.SkinId, -1);
            }
            loadInfo.spritePrefabs = new List<AssetLoadBase>();
            loadInfo.soundBanks = new List<AssetLoadBase>();
            loadInfo.behaviorXml = new List<AssetLoadBase>();
            ActorStaticSkillData skillData = new ActorStaticSkillData();
            for (int j = 0; j < 8; j++)
            {
                actorDataProvider.GetActorStaticSkillData(ref actorMeta, (ActorSkillSlot) j, ref skillData);
                this.AnalyseSkill(ref loadInfo, skillData.SkillId);
                this.AnalysePassiveSkill(ref loadInfo, skillData.PassiveSkillId);
            }
            if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                int num5;
                int num6;
                int num7;
                int num8;
                this.GetMapSkills(out num5, out num6, out num7, out num8);
                this.AnalyseSkill(ref loadInfo, num5);
                this.AnalyseSkill(ref loadInfo, num6);
                this.AnalyseSkill(ref loadInfo, num7);
                this.AnalysePassiveSkill(ref loadInfo, num8);
                uint num9 = 0;
                if (provider2.GetActorServerCommonSkillData(ref actorMeta, out num9))
                {
                    this.AnalyseSkill(ref loadInfo, (int) num9);
                }
                ResTalentHero dataByKey = GameDataMgr.talentHero.GetDataByKey((uint) actorMeta.ConfigId);
                if (dataByKey != null)
                {
                    this.AnalyseHeroTalent(ref loadInfo, dataByKey);
                }
                this.AnalyseHeroBornAndReviveAge(ref loadInfo, actorMeta.ConfigId);
                ResHeroCfgInfo info2 = GameDataMgr.heroDatabin.GetDataByKey((uint) actorMeta.ConfigId);
                if (info2 != null)
                {
                    this.AnalysePassiveSkill(ref loadInfo, info2.iPassiveID1);
                    this.AnalysePassiveSkill(ref loadInfo, info2.iPassiveID2);
                }
            }
            else if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                ActorStaticData data4 = new ActorStaticData();
                Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider).GetActorStaticData(ref actorMeta, ref data4);
                int randomPassiveSkillRule = data4.TheBaseAttribute.RandomPassiveSkillRule;
                if ((randomPassiveSkillRule > 0) && !this.randomSkillCheckerSet.ContainsKey(randomPassiveSkillRule))
                {
                    this.randomSkillCheckerSet.Add(randomPassiveSkillRule, true);
                    ResRandomSkillPassiveRule rule = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long) randomPassiveSkillRule);
                    if ((rule.astRandomSkillPassiveID1 != null) && (rule.astRandomSkillPassiveID1.Length > 0))
                    {
                        for (int k = 0; k < rule.astRandomSkillPassiveID1.Length; k++)
                        {
                            this.AnalysePassiveSkill(ref loadInfo, rule.astRandomSkillPassiveID1[k].iParam);
                        }
                    }
                    if ((rule.astRandomSkillPassiveID2 != null) && (rule.astRandomSkillPassiveID2.Length > 0))
                    {
                        for (int m = 0; m < rule.astRandomSkillPassiveID2.Length; m++)
                        {
                            this.AnalysePassiveSkill(ref loadInfo, rule.astRandomSkillPassiveID2[m].iParam);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(actorInfo.deadAgePath))
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = actorInfo.deadAgePath
                };
                loadInfo.ageActions.Add(item);
            }
            if (!string.IsNullOrEmpty(actorInfo.BtResourcePath) && !this.behaviorXmlSet.ContainsKey(actorInfo.BtResourcePath))
            {
                AssetLoadBase base3 = new AssetLoadBase {
                    assetPath = actorInfo.BtResourcePath
                };
                loadInfo.behaviorXml.Add(base3);
                this.behaviorXmlSet.Add(actorInfo.BtResourcePath, true);
            }
            loadInfo.soundBanks = new List<AssetLoadBase>();
            this.AnalyseSoundBanks(ref loadInfo, ref actorInfo, ref data2);
            list.Add(loadInfo);
            if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                this.CheckCallMonsterSkill(actorInfo, ref list, ref actorMeta, (int) data2.SkinId);
            }
            else if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                this.CheckOrganSoldierWave(ref list, ref actorMeta, actorData);
            }
        }
    }

    public List<ActorPreloadTab> AnalyseAgeRefAssets(Dictionary<long, Dictionary<object, AssetRefType>> refAssetsDict)
    {
        List<ActorPreloadTab> list = new List<ActorPreloadTab>();
        Dictionary<long, Dictionary<object, AssetRefType>>.Enumerator enumerator = refAssetsDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<long, Dictionary<object, AssetRefType>> current = enumerator.Current;
            long key = current.Key;
            int markID = (int) (key >> 0x20);
            int configID = (int) (((ulong) key) & 0xffffffffL);
            KeyValuePair<long, Dictionary<object, AssetRefType>> pair2 = enumerator.Current;
            Dictionary<object, AssetRefType> dictionary = pair2.Value;
            ActorPreloadTab item = new ActorPreloadTab {
                ageActions = new List<AssetLoadBase>(),
                parPrefabs = new List<AssetLoadBase>(),
                mesPrefabs = new List<AssetLoadBase>(),
                spritePrefabs = new List<AssetLoadBase>(),
                soundBanks = new List<AssetLoadBase>(),
                behaviorXml = new List<AssetLoadBase>(),
                MarkID = markID
            };
            item.theActor.ConfigId = configID;
            list.Add(item);
            Dictionary<object, AssetRefType>.Enumerator enumerator2 = dictionary.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                AssetLoadBase base3;
                int num4;
                AssetLoadBase base4;
                AssetLoadBase base5;
                int num5;
                AssetLoadBase base6;
                KeyValuePair<object, AssetRefType> pair3 = enumerator2.Current;
                AssetRefType type = pair3.Value;
                KeyValuePair<object, AssetRefType> pair4 = enumerator2.Current;
                object obj2 = pair4.Key;
                switch (type)
                {
                    case AssetRefType.Action:
                    {
                        AssetLoadBase base2 = new AssetLoadBase {
                            assetPath = obj2 as string
                        };
                        string checkerKey = this.GetCheckerKey(base2.assetPath, markID);
                        if (!this.ageCheckerSet.ContainsKey(checkerKey))
                        {
                            item.ageActions.Add(base2);
                            this.ageCheckerSet.Add(checkerKey, true);
                        }
                        continue;
                    }
                    case AssetRefType.SkillID:
                    {
                        KeyValuePair<object, AssetRefType> pair7 = enumerator2.Current;
                        int skillID = (int) pair7.Key;
                        this.AnalyseSkill(ref item, skillID);
                        continue;
                    }
                    case AssetRefType.SkillCombine:
                    {
                        KeyValuePair<object, AssetRefType> pair6 = enumerator2.Current;
                        int combineId = (int) pair6.Key;
                        this.AnalyseSkillCombine(ref item, combineId);
                        continue;
                    }
                    case AssetRefType.Prefab:
                        base3 = new AssetLoadBase {
                            assetPath = obj2 as string
                        };
                        item.mesPrefabs.Add(base3);
                        if (markID == 0)
                        {
                            continue;
                        }
                        num4 = 0;
                        goto Label_01CC;

                    case AssetRefType.Particle:
                    {
                        base5 = new AssetLoadBase();
                        KeyValuePair<object, AssetRefType> pair5 = enumerator2.Current;
                        base5.assetPath = pair5.Key as string;
                        item.parPrefabs.Add(base5);
                        if (markID == 0)
                        {
                            continue;
                        }
                        num5 = 0;
                        goto Label_024D;
                    }
                    case AssetRefType.Sound:
                    {
                        continue;
                    }
                    case AssetRefType.MonsterConfigId:
                    {
                        KeyValuePair<object, AssetRefType> pair8 = enumerator2.Current;
                        int num8 = (int) pair8.Key;
                        ActorMeta actorMeta = new ActorMeta {
                            ActorType = ActorTypeDef.Actor_Type_Monster,
                            ConfigId = num8,
                            ActorCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID
                        };
                        this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
            Label_0198:
                base4 = new AssetLoadBase();
                base4.assetPath = SkinResourceHelper.GetSkinResourceName(configID, markID, base3.assetPath, num4);
                item.mesPrefabs.Add(base4);
                num4++;
            Label_01CC:
                if (num4 < 3)
                {
                    goto Label_0198;
                }
                continue;
            Label_0219:
                base6 = new AssetLoadBase();
                base6.assetPath = SkinResourceHelper.GetSkinResourceName(configID, markID, base5.assetPath, num5);
                item.parPrefabs.Add(base6);
                num5++;
            Label_024D:
                if (num5 < 3)
                {
                    goto Label_0219;
                }
            }
        }
        return list;
    }

    public void AnalyseHeroBornAndReviveAge(ref ActorPreloadTab loadInfo, int configID)
    {
        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) configID);
        if (dataByKey != null)
        {
            AssetLoadBase item = new AssetLoadBase {
                assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szBorn_Age)
            };
            string checkerKey = this.GetCheckerKey(item.assetPath, loadInfo.MarkID);
            if (!this.ageCheckerSet.ContainsKey(checkerKey))
            {
                loadInfo.ageActions.Add(item);
                this.ageCheckerSet.Add(checkerKey, true);
            }
            AssetLoadBase base3 = new AssetLoadBase {
                assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szRevive_Age)
            };
            string key = this.GetCheckerKey(base3.assetPath, loadInfo.MarkID);
            if (!this.ageCheckerSet.ContainsKey(key))
            {
                loadInfo.ageActions.Add(base3);
                this.ageCheckerSet.Add(key, true);
            }
        }
    }

    public void AnalyseHeroTalent(ref ActorPreloadTab loadInfo, ResTalentHero talentCfg)
    {
        for (int i = 0; i < talentCfg.astTalentList.Length; i++)
        {
            RESDT_TALENT_DETAIL resdt_talent_detail = talentCfg.astTalentList[i];
            ResTalentLib dataByKey = GameDataMgr.talentLib.GetDataByKey(resdt_talent_detail.dwLvl1ID);
            if (dataByKey != null)
            {
                this.AnalyseSkill(ref loadInfo, (int) dataByKey.dwSkillID);
                this.AnalysePassiveSkill(ref loadInfo, (int) dataByKey.dwPassiveSkillID);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID1);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID2);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID3);
            }
            dataByKey = GameDataMgr.talentLib.GetDataByKey(resdt_talent_detail.dwLvl2ID);
            if (dataByKey != null)
            {
                this.AnalyseSkill(ref loadInfo, (int) dataByKey.dwSkillID);
                this.AnalysePassiveSkill(ref loadInfo, (int) dataByKey.dwPassiveSkillID);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID1);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID2);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID3);
            }
            dataByKey = GameDataMgr.talentLib.GetDataByKey(resdt_talent_detail.dwLvl3ID);
            if (dataByKey != null)
            {
                this.AnalyseSkill(ref loadInfo, (int) dataByKey.dwSkillID);
                this.AnalysePassiveSkill(ref loadInfo, (int) dataByKey.dwPassiveSkillID);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID1);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID2);
                this.AnalyseSkillCombine(ref loadInfo, (int) dataByKey.dwEffectID3);
            }
        }
    }

    public void AnalysePassiveSkill(ref ActorPreloadTab loadInfo, int passiveSkillID)
    {
        if (passiveSkillID > 0)
        {
            ResSkillPassiveCfgInfo dataByKey = GameDataMgr.skillPassiveDatabin.GetDataByKey((long) passiveSkillID);
            if (dataByKey != null)
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szActionName)
                };
                string checkerKey = this.GetCheckerKey(item.assetPath, loadInfo.MarkID);
                if (!this.ageCheckerSet.ContainsKey(checkerKey))
                {
                    loadInfo.ageActions.Add(item);
                    this.ageCheckerSet.Add(checkerKey, true);
                }
                if (dataByKey.dwPassiveEventType == 8)
                {
                    this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam4);
                }
            }
        }
    }

    public void AnalyseSkill(ref ActorPreloadTab loadInfo, int skillID)
    {
        if (skillID > 0)
        {
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) skillID);
            if (dataByKey != null)
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szPrefab)
                };
                string checkerKey = this.GetCheckerKey(item.assetPath, loadInfo.MarkID);
                if (!this.ageCheckerSet.ContainsKey(checkerKey))
                {
                    loadInfo.ageActions.Add(item);
                    this.ageCheckerSet.Add(checkerKey, true);
                    string str2 = StringHelper.UTF8BytesToString(ref dataByKey.szGuidePrefab);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        item.assetPath = str2;
                        item.nInstantiate = 1;
                        loadInfo.parPrefabs.Add(item);
                    }
                    str2 = StringHelper.UTF8BytesToString(ref dataByKey.szGuideWarnPrefab);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        item.assetPath = str2;
                        item.nInstantiate = 1;
                        loadInfo.parPrefabs.Add(item);
                    }
                    str2 = StringHelper.UTF8BytesToString(ref dataByKey.szEffectPrefab);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        item.assetPath = str2;
                        item.nInstantiate = 1;
                        loadInfo.parPrefabs.Add(item);
                    }
                    str2 = StringHelper.UTF8BytesToString(ref dataByKey.szEffectWarnPrefab);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        item.assetPath = str2;
                        item.nInstantiate = 1;
                        loadInfo.parPrefabs.Add(item);
                    }
                }
            }
        }
    }

    public void AnalyseSkillCombine(ref ActorPreloadTab loadInfo, int combineId)
    {
        if (combineId > 0)
        {
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) combineId);
            if (dataByKey != null)
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szPrefab)
                };
                string checkerKey = this.GetCheckerKey(item.assetPath, loadInfo.MarkID);
                if (!this.ageCheckerSet.ContainsKey(checkerKey))
                {
                    loadInfo.ageActions.Add(item);
                    this.ageCheckerSet.Add(checkerKey, true);
                }
                if ((dataByKey.bIsShowBuff == 1) && !string.IsNullOrEmpty(dataByKey.szIconPath))
                {
                    string path = CUIUtility.s_Sprite_Dynamic_Skill_Dir + dataByKey.szIconPath;
                    if (!loadInfo.IsExistsSprite(path))
                    {
                        loadInfo.AddSprite(path);
                    }
                }
                if ((dataByKey.astSkillFuncInfo != null) && (dataByKey.astSkillFuncInfo.Length > 0))
                {
                    for (int i = 0; i < dataByKey.astSkillFuncInfo.Length; i++)
                    {
                        ResDT_SkillFunc func = dataByKey.astSkillFuncInfo[i];
                        if (((((func.dwSkillFuncType == 0x1c) || (func.dwSkillFuncType == 0x21)) || (((func.dwSkillFuncType == 0x36) || (func.dwSkillFuncType == 0x37)) || (func.dwSkillFuncType == 0x1b))) && (func.astSkillFuncParam != null)) && (func.astSkillFuncParam.Length != 0))
                        {
                            int markId = 0;
                            int skillID = 0;
                            int iParam = 0;
                            if (func.astSkillFuncParam[0] != null)
                            {
                                markId = func.astSkillFuncParam[0].iParam;
                            }
                            if (func.astSkillFuncParam[1] != null)
                            {
                                skillID = func.astSkillFuncParam[1].iParam;
                            }
                            if (func.astSkillFuncParam[5] != null)
                            {
                                iParam = func.astSkillFuncParam[5].iParam;
                            }
                            if (func.dwSkillFuncType == 0x1c)
                            {
                                this.AnalyseSkillMark(ref loadInfo, markId);
                            }
                            else if (func.dwSkillFuncType == 0x21)
                            {
                                if (combineId != markId)
                                {
                                    this.AnalyseSkillCombine(ref loadInfo, markId);
                                }
                            }
                            else if (func.dwSkillFuncType == 0x36)
                            {
                                this.AnalyseSkillCombine(ref loadInfo, markId);
                            }
                            else if (func.dwSkillFuncType == 0x37)
                            {
                                this.AnalyseSkill(ref loadInfo, skillID);
                            }
                            else if (func.dwSkillFuncType == 0x1b)
                            {
                                this.AnalyseSkillCombine(ref loadInfo, iParam);
                            }
                        }
                    }
                }
            }
        }
    }

    public void AnalyseSkillCombine(ref List<string> ageList, int combineId)
    {
        if (combineId > 0)
        {
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) combineId);
            if (dataByKey != null)
            {
                ageList.Add(StringHelper.UTF8BytesToString(ref dataByKey.szPrefab));
            }
        }
    }

    public void AnalyseSkillMark(ref ActorPreloadTab loadInfo, int markId)
    {
        if (markId > 0)
        {
            ResSkillMarkCfgInfo dataByKey = GameDataMgr.skillMarkDatabin.GetDataByKey((long) markId);
            if (dataByKey != null)
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szActionName)
                };
                string checkerKey = this.GetCheckerKey(item.assetPath, loadInfo.MarkID);
                if (!this.ageCheckerSet.ContainsKey(checkerKey))
                {
                    loadInfo.ageActions.Add(item);
                    this.ageCheckerSet.Add(checkerKey, true);
                }
                this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName1);
                this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName2);
                this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName3);
                this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName4);
                this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName5);
            }
        }
    }

    public void AnalyseSkillMarkParticle(ref ActorPreloadTab loadInfo, string _resName)
    {
        AssetLoadBase item = new AssetLoadBase();
        string str = StringHelper.UTF8BytesToString(ref _resName);
        if (!string.IsNullOrEmpty(str))
        {
            item.assetPath = str;
            item.nInstantiate = 1;
            loadInfo.parPrefabs.Add(item);
            this.AnalyseSkillMarkParticleSkin(ref loadInfo, str);
        }
    }

    public void AnalyseSkillMarkParticleSkin(ref ActorPreloadTab loadInfo, string _resName)
    {
        for (int i = 0; i < 3; i++)
        {
            AssetLoadBase item = new AssetLoadBase {
                assetPath = SkinResourceHelper.GetSkinResourceName(loadInfo.theActor.ConfigId, loadInfo.MarkID, _resName, i),
                nInstantiate = 1
            };
            loadInfo.parPrefabs.Add(item);
        }
    }

    public void AnalyseSoundBanks(ref ActorPreloadTab loadInfo, ref CActorInfo charInfo, ref ActorServerData serverData)
    {
        if ((charInfo.SoundBanks != null) && (charInfo.SoundBanks.Length > 0))
        {
            for (int i = 0; i < charInfo.SoundBanks.Length; i++)
            {
                AssetLoadBase item = new AssetLoadBase {
                    assetPath = charInfo.SoundBanks[i]
                };
                loadInfo.soundBanks.Add(item);
            }
        }
        if (serverData.SkinId != 0)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint) loadInfo.theActor.ConfigId, serverData.SkinId);
            if ((heroSkin != null) && !string.IsNullOrEmpty(heroSkin.szSkinSoundResPack))
            {
                AssetLoadBase base3 = new AssetLoadBase {
                    assetPath = heroSkin.szSkinSoundResPack + "_SFX"
                };
                loadInfo.soundBanks.Add(base3);
                base3 = new AssetLoadBase {
                    assetPath = heroSkin.szSkinSoundResPack + "_VO"
                };
                loadInfo.soundBanks.Add(base3);
            }
        }
    }

    public void BuildActionHelper(ref ActorPreloadTab loadInfo)
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
                    object[] inParameters = new object[] { ((storage == null) || (storage.helperName == null)) ? "null" : storage.helperName, ((storage == null) || (storage.actionName == null)) ? "null" : storage.actionName };
                    DebugHelper.Assert((storage != null) && (storage.actionName != null), "storage is null or action name is null. storage = {0}, storage.actionName={1}", inParameters);
                    if (((storage != null) && !string.IsNullOrEmpty(storage.actionName)) && !this.ageCheckerSet.ContainsKey(storage.actionName))
                    {
                        AssetLoadBase item = new AssetLoadBase {
                            assetPath = storage.actionName
                        };
                        loadInfo.ageActions.Add(item);
                        this.ageCheckerSet.Add(storage.actionName, true);
                    }
                }
            }
        }
    }

    public void BuildActionTrigger(ref ActorPreloadTab loadInfo)
    {
        AreaEventTrigger[] triggerArray = UnityEngine.Object.FindObjectsOfType<AreaEventTrigger>();
        if ((triggerArray != null) && (triggerArray.Length > 0))
        {
            for (int i = 0; i < triggerArray.Length; i++)
            {
                AreaEventTrigger trigger = triggerArray[i];
                for (int j = 0; j < trigger.TriggerActions.Length; j++)
                {
                    TriggerActionWrapper triggerActionWrapper = trigger.TriggerActions[j];
                    if ((triggerActionWrapper != null) && (triggerActionWrapper.TimingActionsInter != null))
                    {
                        for (int k = 0; k < triggerActionWrapper.TimingActionsInter.Length; k++)
                        {
                            if (!string.IsNullOrEmpty(triggerActionWrapper.TimingActionsInter[k].ActionName) && !this.ageCheckerSet.ContainsKey(triggerActionWrapper.TimingActionsInter[k].ActionName))
                            {
                                AssetLoadBase item = new AssetLoadBase {
                                    assetPath = triggerActionWrapper.TimingActionsInter[k].ActionName
                                };
                                loadInfo.ageActions.Add(item);
                                this.ageCheckerSet.Add(triggerActionWrapper.TimingActionsInter[k].ActionName, true);
                            }
                        }
                    }
                    if ((triggerActionWrapper != null) && (triggerActionWrapper.TriggerType == EGlobalTriggerAct.TriggerShenFu))
                    {
                        Singleton<ShenFuSystem>.instance.PreLoadResource(triggerActionWrapper, ref loadInfo, this);
                    }
                }
                if ((trigger.PresetActWrapper != null) && (trigger.PresetActWrapper.TimingActionsInter != null))
                {
                    for (int m = 0; m < trigger.PresetActWrapper.TimingActionsInter.Length; m++)
                    {
                        if (!string.IsNullOrEmpty(trigger.PresetActWrapper.TimingActionsInter[m].ActionName) && !this.ageCheckerSet.ContainsKey(trigger.PresetActWrapper.TimingActionsInter[m].ActionName))
                        {
                            AssetLoadBase base3 = new AssetLoadBase {
                                assetPath = trigger.PresetActWrapper.TimingActionsInter[m].ActionName
                            };
                            loadInfo.ageActions.Add(base3);
                            this.ageCheckerSet.Add(trigger.PresetActWrapper.TimingActionsInter[m].ActionName, true);
                        }
                    }
                }
            }
        }
        GlobalTrigger[] triggerArray2 = UnityEngine.Object.FindObjectsOfType<GlobalTrigger>();
        if ((triggerArray2 != null) && (triggerArray2.Length > 0))
        {
            for (int n = 0; n < triggerArray2.Length; n++)
            {
                GlobalTrigger trigger2 = triggerArray2[n];
                if ((trigger2.TriggerMatches != null) && (trigger2.TriggerMatches.Length > 0))
                {
                    for (int num6 = 0; num6 < trigger2.TriggerMatches.Length; num6++)
                    {
                        CTriggerMatch match = trigger2.TriggerMatches[num6];
                        if (((match != null) && (match.ActionList != null)) && (match.ActionList.Length > 0))
                        {
                            for (int num7 = 0; num7 < match.ActionList.Length; num7++)
                            {
                                TriggerActionWrapper wrapper2 = match.ActionList[num7];
                                if ((wrapper2 != null) && (wrapper2.TimingActionsInter != null))
                                {
                                    for (int num8 = 0; num8 < wrapper2.TimingActionsInter.Length; num8++)
                                    {
                                        if (!string.IsNullOrEmpty(wrapper2.TimingActionsInter[num8].ActionName) && !this.ageCheckerSet.ContainsKey(wrapper2.TimingActionsInter[num8].ActionName))
                                        {
                                            AssetLoadBase base4 = new AssetLoadBase {
                                                assetPath = wrapper2.TimingActionsInter[num8].ActionName
                                            };
                                            loadInfo.ageActions.Add(base4);
                                            this.ageCheckerSet.Add(wrapper2.TimingActionsInter[num8].ActionName, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((trigger2.ActionList != null) && (trigger2.ActionList.Length > 0))
                {
                    for (int num9 = 0; num9 < trigger2.ActionList.Length; num9++)
                    {
                        TriggerActionWrapper wrapper3 = trigger2.ActionList[num9];
                        if ((wrapper3 != null) && (wrapper3.TimingActionsInter != null))
                        {
                            for (int num10 = 0; num10 < wrapper3.TimingActionsInter.Length; num10++)
                            {
                                if (!string.IsNullOrEmpty(wrapper3.TimingActionsInter[num10].ActionName) && !this.ageCheckerSet.ContainsKey(wrapper3.TimingActionsInter[num10].ActionName))
                                {
                                    AssetLoadBase base5 = new AssetLoadBase {
                                        assetPath = wrapper3.TimingActionsInter[num10].ActionName
                                    };
                                    loadInfo.ageActions.Add(base5);
                                    this.ageCheckerSet.Add(wrapper3.TimingActionsInter[num10].ActionName, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void BuildAreaTrigger(ref ActorPreloadTab loadInfo)
    {
        AreaTrigger[] triggerArray = UnityEngine.Object.FindObjectsOfType<AreaTrigger>();
        if ((triggerArray != null) && (triggerArray.Length != 0))
        {
            for (int i = 0; i < triggerArray.Length; i++)
            {
                AreaTrigger trigger = triggerArray[i];
                this.AnalyseSkillCombine(ref loadInfo, trigger.BuffID);
                this.AnalyseSkillCombine(ref loadInfo, trigger.UpdateBuffID);
                this.AnalyseSkillCombine(ref loadInfo, trigger.LeaveBuffID);
            }
        }
    }

    public void BuildCommonSpawnPoints(ref ActorPreloadTab loadInfo)
    {
        CommonSpawnPoint[] pointArray = UnityEngine.Object.FindObjectsOfType<CommonSpawnPoint>();
        if ((pointArray != null) && (pointArray.Length != 0))
        {
            for (int i = 0; i < pointArray.Length; i++)
            {
                CommonSpawnPoint point = pointArray[i];
                if (point != null)
                {
                    point.PreLoadResource(ref loadInfo, this);
                }
            }
        }
    }

    public void BuildCommonSpawnPoints(ref List<ActorPreloadTab> list)
    {
        CommonSpawnPoint[] pointArray = UnityEngine.Object.FindObjectsOfType<CommonSpawnPoint>();
        if ((pointArray != null) && (pointArray.Length != 0))
        {
            for (int i = 0; i < pointArray.Length; i++)
            {
                CommonSpawnPoint point = pointArray[i];
                if (point != null)
                {
                    point.PreLoadResource(ref list, this);
                }
            }
        }
    }

    public void BuildDynamicActor(ref List<ActorPreloadTab> list)
    {
        for (int i = 0; i < MonoSingleton<GameLoader>.instance.actorList.Count; i++)
        {
            ActorMeta actorMeta = MonoSingleton<GameLoader>.instance.actorList[i];
            this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
        }
    }

    private void BuildEquipActiveSkill(ref List<ActorPreloadTab> list)
    {
        int count = GameDataMgr.m_equipInBattleDatabin.count;
        ActorPreloadTab loadInfo = new ActorPreloadTab {
            ageActions = new List<AssetLoadBase>(),
            parPrefabs = new List<AssetLoadBase>(),
            mesPrefabs = new List<AssetLoadBase>(),
            spritePrefabs = new List<AssetLoadBase>(),
            soundBanks = new List<AssetLoadBase>(),
            behaviorXml = new List<AssetLoadBase>()
        };
        for (int i = 0; i < count; i++)
        {
            ResEquipInBattle dataByIndex = GameDataMgr.m_equipInBattleDatabin.GetDataByIndex(i);
            if ((dataByIndex != null) && (dataByIndex.dwActiveSkillID > 0))
            {
                this.AnalyseSkill(ref loadInfo, (int) dataByIndex.dwActiveSkillID);
                list.Add(loadInfo);
            }
        }
    }

    public void BuildEquipInBattle(ref ActorPreloadTab result)
    {
        Dictionary<long, object>.Enumerator enumerator = GameDataMgr.m_equipInBattleDatabin.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<long, object> current = enumerator.Current;
            ResEquipInBattle battle = current.Value as ResEquipInBattle;
            if (battle != null)
            {
                string path = CUIUtility.s_Sprite_System_BattleEquip_Dir + StringHelper.UTF8BytesToString(ref battle.szIcon);
                result.AddSprite(path);
                if ((battle.astEffectCombine != null) && (battle.astEffectCombine.Length > 0))
                {
                    for (int i = 0; i < battle.astEffectCombine.Length; i++)
                    {
                        uint dwID = battle.astEffectCombine[i].dwID;
                        this.AnalyseSkillCombine(ref result, (int) dwID);
                    }
                }
                if ((battle.astPassiveSkill != null) && (battle.astPassiveSkill.Length > 0))
                {
                    for (int j = 0; j < battle.astPassiveSkill.Length; j++)
                    {
                        if (battle.astPassiveSkill[j].dwID > 0)
                        {
                            this.AnalysePassiveSkill(ref result, (int) battle.astPassiveSkill[j].dwID);
                        }
                    }
                }
            }
        }
    }

    public void BuildSoldierRegions(ref List<ActorPreloadTab> list)
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
                        Assets.Scripts.GameLogic.SoldierWave wave = region.Waves[j];
                        if (((wave != null) && (wave.WaveInfo != null)) && ((wave.WaveInfo.astNormalSoldierInfo != null) && (wave.WaveInfo.astNormalSoldierInfo.Length > 0)))
                        {
                            for (int k = 0; k < wave.WaveInfo.astNormalSoldierInfo.Length; k++)
                            {
                                ResSoldierTypeInfo info = wave.WaveInfo.astNormalSoldierInfo[k];
                                if (info.dwSoldierID == 0)
                                {
                                    break;
                                }
                                ActorMeta actorMeta = new ActorMeta {
                                    ActorType = ActorTypeDef.Actor_Type_Monster,
                                    ConfigId = (int) info.dwSoldierID
                                };
                                this.AddPreloadActor(ref list, ref actorMeta, info.dwSoldierNum * 2f, 0);
                            }
                        }
                    }
                }
            }
        }
    }

    public void BuildSpawnPoints(ref List<ActorPreloadTab> list)
    {
        SpawnPoint[] pointArray = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
        if ((pointArray != null) && (pointArray.Length != 0))
        {
            for (int i = 0; i < pointArray.Length; i++)
            {
                SpawnPoint point = pointArray[i];
                if (point != null)
                {
                    point.PreLoadResource(ref list, this);
                }
            }
        }
    }

    public void BuildStaticActor(ref List<ActorPreloadTab> list)
    {
        DebugHelper.Assert(MonoSingleton<GameLoader>.instance.staticActors != null, "Static actors cannot be null");
        for (int i = 0; i < MonoSingleton<GameLoader>.instance.staticActors.Count; i++)
        {
            object[] inParameters = new object[] { i };
            DebugHelper.Assert(MonoSingleton<GameLoader>.instance.staticActors[i] != null, "actor config cannot be null at {0}", inParameters);
            if (MonoSingleton<GameLoader>.instance.staticActors[i] != null)
            {
                ActorMeta actorMeta = new ActorMeta {
                    ActorType = MonoSingleton<GameLoader>.instance.staticActors[i].ActorType,
                    ConfigId = MonoSingleton<GameLoader>.instance.staticActors[i].ConfigID,
                    ActorCamp = MonoSingleton<GameLoader>.instance.staticActors[i].CmpType
                };
                this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
            }
        }
    }

    private void CheckCallMonsterSkill(CActorInfo InActorInfo, ref List<ActorPreloadTab> InPreloadListRef, ref ActorMeta InParentActorMetaRef, int markID)
    {
        if ((InActorInfo != null) && (InActorInfo.callMonsterConfigID > 0))
        {
            ResCallMonster dataByKey = GameDataMgr.callMonsterDatabin.GetDataByKey((long) InActorInfo.callMonsterConfigID);
            if (dataByKey != null)
            {
                ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo((int) dataByKey.dwMonsterID, 1);
                if (dataCfgInfo != null)
                {
                    ActorMeta actorMeta = new ActorMeta {
                        ActorType = ActorTypeDef.Actor_Type_Monster,
                        ActorCamp = InParentActorMetaRef.ActorCamp,
                        ConfigId = dataCfgInfo.iCfgID
                    };
                    this.AddPreloadActor(ref InPreloadListRef, ref actorMeta, 2f, markID);
                }
            }
        }
    }

    private void CheckOrganSoldierWave(ref List<ActorPreloadTab> InPreloadListRef, ref ActorMeta InOrganActorMetaRef, ActorStaticData InStaticData)
    {
        if ((InOrganActorMetaRef.ActorType == ActorTypeDef.Actor_Type_Organ) && (InStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0))
        {
            ResSoldierWaveInfo dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey((uint) InStaticData.TheOrganOnlyInfo.DeadEnemySoldier);
            int num = 0;
            while ((dataByKey != null) && (++num < 100))
            {
                for (int i = 0; i < dataByKey.astNormalSoldierInfo.Length; i++)
                {
                    ResSoldierTypeInfo info2 = dataByKey.astNormalSoldierInfo[i];
                    if (info2.dwSoldierID == 0)
                    {
                        break;
                    }
                    ActorMeta actorMeta = new ActorMeta {
                        ActorType = ActorTypeDef.Actor_Type_Monster,
                        ConfigId = (int) info2.dwSoldierID
                    };
                    this.AddPreloadActor(ref InPreloadListRef, ref actorMeta, 2f, 0);
                }
                dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey(dataByKey.dwNextSoldierWaveID);
            }
        }
    }

    public int GetActorMarkID(ActorMeta actorMeta)
    {
        int skinCfgId = 0;
        IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
        ActorServerData actorData = new ActorServerData();
        if ((actorDataProvider != null) && actorDataProvider.GetActorServerData(ref actorMeta, ref actorData))
        {
            int skinId = (int) actorData.SkinId;
            if (skinId != 0)
            {
                skinCfgId = (int) CSkinInfo.GetSkinCfgId((uint) actorMeta.ConfigId, (uint) skinId);
            }
        }
        return skinCfgId;
    }

    public List<ActorPreloadTab> GetActorPreload()
    {
        List<ActorPreloadTab> list = new List<ActorPreloadTab>();
        this.BuildStaticActor(ref list);
        this.BuildDynamicActor(ref list);
        this.BuildSpawnPoints(ref list);
        this.BuildCommonSpawnPoints(ref list);
        this.BuildSoldierRegions(ref list);
        this.BuildEquipActiveSkill(ref list);
        return list;
    }

    public string GetCheckerKey(string assetPath, int markID)
    {
        string str = assetPath;
        if (markID != 0)
        {
            str = ("<<" + markID + ">>") + assetPath;
        }
        return str;
    }

    public ActorPreloadTab GetGlobalPreload()
    {
        ActorPreloadTab loadInfo = new ActorPreloadTab {
            ageActions = new List<AssetLoadBase>(),
            parPrefabs = new List<AssetLoadBase>(),
            mesPrefabs = new List<AssetLoadBase>(),
            spritePrefabs = new List<AssetLoadBase>(),
            soundBanks = new List<AssetLoadBase>(),
            behaviorXml = new List<AssetLoadBase>()
        };
        this.BuildAreaTrigger(ref loadInfo);
        this.BuildActionHelper(ref loadInfo);
        this.BuildActionTrigger(ref loadInfo);
        this.BuildCommonSpawnPoints(ref loadInfo);
        this.BuildEquipInBattle(ref loadInfo);
        EffectPlayComponent.Preload(ref loadInfo);
        HudComponent3D.Preload(ref loadInfo);
        CBattleFloatDigitManager.Preload(ref loadInfo);
        OrganHitEffect.Preload(ref loadInfo);
        OrganWrapper.Preload(ref loadInfo);
        KillNotify.Preload(ref loadInfo);
        CSignalTipShower.Preload(ref loadInfo);
        CSignal.Preload(ref loadInfo);
        SkillIndicateSystem.Preload(ref loadInfo);
        ObjAgent.Preload(ref loadInfo);
        CSkillButtonManager.Preload(ref loadInfo);
        UpdateShadowPlane.Preload(ref loadInfo);
        CBattleSystem.Preload(ref loadInfo);
        CSkillData.Preload(ref loadInfo);
        return loadInfo;
    }

    public void GetMapSkills(out int skillId, out int skillId2, out int ornamentSkillId, out int passiveSkillId)
    {
        int num2;
        passiveSkillId = num2 = 0;
        ornamentSkillId = num2 = num2;
        skillId = skillId2 = num2;
        SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
        if ((curLvelContext != null) && (curLvelContext.m_mapID > 0))
        {
            skillId = curLvelContext.m_extraSkillId;
            skillId2 = curLvelContext.m_extraSkill2Id;
            ornamentSkillId = curLvelContext.m_ornamentSkillId;
            passiveSkillId = curLvelContext.m_extraPassiveSkillId;
        }
    }

    public Dictionary<object, AssetRefType> GetRefAssets(int markID, int configID)
    {
        Dictionary<object, AssetRefType> dictionary = null;
        if (markID == 0)
        {
            configID = 0;
        }
        long key = (markID << 0x20) | configID;
        if (!this.ageRefAssets2.TryGetValue(key, out dictionary))
        {
            dictionary = new Dictionary<object, AssetRefType>();
            this.ageRefAssets2.Add(key, dictionary);
        }
        return dictionary;
    }
}

