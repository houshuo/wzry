namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CBattleEquipSystem
    {
        public const int c_animationTime = 0x7d0;
        public const int c_equipLevelMaxCount = 3;
        private const int c_equipLineCnt = 14;
        public const int c_fadeTime = 0x2710;
        public const int c_maxEquipCntPerLevel = 8;
        public const uint c_quicklyBuyEquipCount = 2;
        private ListView<Transform> m_bagEquipItemList = new ListView<Transform>();
        private CUIFormScript m_battleFormScript;
        private bool m_bPlayAnimation;
        private enEquipUsage m_curEquipUsage;
        private ListView<CEquipBuyPrice> m_equipBuyPricePool = new ListView<CEquipBuyPrice>(5);
        private uint m_equipChangedFlags;
        private CUIFormScript m_equipFormScript;
        private Dictionary<ushort, CEquipInfo> m_equipInfoDictionary = Singleton<CEquipSystem>.GetInstance().GetEquipInfoDictionary();
        private ListView<Transform> m_equipItemList = new ListView<Transform>();
        private List<ushort>[][] m_equipList = Singleton<CEquipSystem>.GetInstance().GetEquipList();
        private CEquipRelationPath m_equipRelationPath = new CEquipRelationPath();
        private ListView<CExistEquipInfoSet> m_existEquipInfoSetPool = new ListView<CExistEquipInfoSet>(5);
        private PoolObjHandle<ActorRoot> m_hostCtrlHero;
        private bool m_hostCtrlHeroPermitedToBuyEquip;
        private Dictionary<ushort, CEquipBuyPrice> m_hostPlayerCachedEquipBuyPrice = new Dictionary<ushort, CEquipBuyPrice>();
        private ushort[] m_hostPlayerQuicklyBuyEquipIDs = new ushort[2];
        private bool m_isEnabled;
        private bool m_isInEquipLimitedLevel;
        private int m_selBagPos = -1;
        private CEquipInfo m_selEquipInfo;
        private Transform m_selEquipItemObj;
        private ushort[] m_tempQuicklyBuyEquipIDs = new ushort[2];
        private ListView<CEquipInfo> m_tempRelatedRecommondEquips = new ListView<CEquipInfo>();
        private int m_tickAnimationTime;
        private int m_tickFadeTime;
        private float m_uiEquipItemContentDefaultHeight;
        private float m_uiEquipItemHeight;
        public static string s_equipFormPath = "UGUI/Form/Battle/Form_Battle_Equip.prefab";
        private static int s_equipUsageAmount = Enum.GetValues(typeof(enEquipUsage)).Length;

        public CBattleEquipSystem()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                KeyValuePair<ushort, CEquipInfo> pair2 = enumerator.Current;
                this.m_hostPlayerCachedEquipBuyPrice.Add(current.Key, new CEquipBuyPrice((uint) pair2.Value.m_resEquipInBattle.dwBuyPrice));
            }
        }

        private void AddRecommendPreEquip(ushort equipId, bool bRootRecommend)
        {
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipId);
            if (((dataByKey != null) && (dataByKey.bInvalid <= 0)) && (this.m_equipList[0][dataByKey.bLevel - 1].Count < 8))
            {
                if (bRootRecommend || !this.m_equipList[0][dataByKey.bLevel - 1].Contains(equipId))
                {
                    this.m_equipList[0][dataByKey.bLevel - 1].Add(equipId);
                }
                for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
                {
                    this.AddRecommendPreEquip(dataByKey.PreEquipID[i], false);
                }
            }
        }

        private void BuyEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
            {
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                if (!this.m_isEnabled || !this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
                {
                    freeEquipBuyPrice.m_used = false;
                }
                else
                {
                    actor.handle.ValueComponent.ChangeGoldCoinInBattle((int) (freeEquipBuyPrice.m_buyPrice * -1), false, false, new Vector3(), false);
                    for (int i = 0; i < freeEquipBuyPrice.m_swappedPreEquipInfoCount; i++)
                    {
                        if ((freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID > 0) && (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0))
                        {
                            while (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0)
                            {
                                actor.handle.EquipComponent.RemoveEquip(freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID);
                                freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount--;
                            }
                        }
                    }
                    ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
                    if ((requiredEquipIDs != null) && (requiredEquipIDs.Length > 0))
                    {
                        for (int j = 0; j < requiredEquipIDs.Length; j++)
                        {
                            if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[j], 1))
                            {
                                actor.handle.EquipComponent.RemoveEquip(requiredEquipIDs[j]);
                                break;
                            }
                        }
                    }
                    actor.handle.EquipComponent.AddEquip(equipID);
                    actor.handle.EquipComponent.UpdateEquipEffect();
                    freeEquipBuyPrice.m_used = false;
                }
            }
        }

        private void BuyHorizonEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "BuyHorizonEquip GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                    {
                        SkillComponent skillControl = actor.handle.SkillControl;
                        if ((actor.handle.BuffHolderComp != null) && (skillControl != null))
                        {
                            SkillSlot slot = skillControl.SkillSlotArray[7];
                            if ((slot == null) || (slot.SkillObj == null))
                            {
                                skillControl.InitSkillSlot(7, (int) equipInfo.m_resEquipInBattle.dwActiveSkillID, 0);
                                if (actor.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_7, out slot))
                                {
                                    slot.SetSkillLevel(1);
                                    if ((actor == this.m_hostCtrlHero) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                                    {
                                        Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(actor);
                                    }
                                }
                            }
                            else
                            {
                                BuffChangeSkillRule changeSkillRule = actor.handle.BuffHolderComp.changeSkillRule;
                                if (changeSkillRule != null)
                                {
                                    changeSkillRule.ChangeSkillSlot(SkillSlotType.SLOT_SKILL_7, (int) equipInfo.m_resEquipInBattle.dwActiveSkillID, slot.SkillObj.SkillID);
                                }
                            }
                        }
                        if ((this.m_hostCtrlHero != 0) && (actor == this.m_hostCtrlHero))
                        {
                            this.SetEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnHeroEquipInBattleChanged));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
            this.m_battleFormScript = null;
            this.m_hostCtrlHero.Release();
            this.m_isEnabled = false;
            this.m_hostCtrlHeroPermitedToBuyEquip = false;
            this.m_curEquipUsage = enEquipUsage.Recommend;
            this.ClearEquipList(enEquipUsage.Recommend);
            this.ClearHostPlayerEquipInfo();
            this.m_tickFadeTime = 0;
            this.m_tickAnimationTime = 0;
            this.m_bPlayAnimation = false;
            Singleton<CUIManager>.GetInstance().CloseForm(s_equipFormPath);
        }

        private void ClearAllEquipSelect()
        {
            if (null != this.m_equipFormScript)
            {
                this.ClearEquipLevelPanelSelect();
                GameObject widget = this.m_equipFormScript.GetWidget(4);
                this.ClearEquipBagPanelSelect(widget.transform);
                this.m_equipRelationPath.Reset();
                this.m_selEquipInfo = null;
                this.m_selEquipItemObj = null;
            }
        }

        private void ClearCurSelectEquipItem()
        {
            if (this.m_selEquipItemObj != null)
            {
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, false);
                this.m_selEquipItemObj = null;
            }
            this.m_selEquipInfo = null;
            this.m_equipRelationPath.Reset();
        }

        public void ClearEquipBagPanelSelect(Transform bagPanel)
        {
            if (null != bagPanel)
            {
                for (int i = 0; i < 6; i++)
                {
                    Transform bagEquipItem = this.GetBagEquipItem(i);
                    this.SetEquipItemSelectFlag(bagEquipItem, false);
                }
            }
        }

        private void ClearEquipChangeFlag()
        {
            this.m_equipChangedFlags = 0;
        }

        private void ClearEquipLevelPanelSelect()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(1 + i);
                if (widget != null)
                {
                    Transform transform = widget.transform;
                    if (widget != null)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Transform equipItem = this.GetEquipItem(i, j);
                            this.SetEquipItemSelectFlag(equipItem, false);
                        }
                    }
                }
            }
        }

        private void ClearEquipList(enEquipUsage equipUsage)
        {
            for (int i = 0; i < 3; i++)
            {
                this.m_equipList[(int) equipUsage][i].Clear();
            }
        }

        private void ClearHostPlayerEquipInfo()
        {
            for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
            {
                this.m_hostPlayerQuicklyBuyEquipIDs[i] = 0;
            }
            this.m_hostPlayerCachedEquipBuyPrice.Clear();
        }

        public void CloseEquipFormRightPanel()
        {
            if (null != this.m_equipFormScript)
            {
                this.m_equipFormScript.GetWidget(6).CustomSetActive(false);
                this.m_equipFormScript.GetWidget(7).CustomSetActive(false);
                this.m_equipFormScript.GetWidget(5).CustomSetActive(false);
            }
        }

        public void ExecuteBuyEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            this.BuyEquip(equipID, ref actor);
        }

        public void ExecuteBuyHorizonEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            this.BuyHorizonEquip(equipID, ref actor);
        }

        public void ExecuteSellEquipFrameCommand(int equipIndex, ref PoolObjHandle<ActorRoot> actor)
        {
            this.SellEquip(equipIndex, ref actor);
        }

        private Transform GetBagEquipItem(int index)
        {
            if ((index >= 0) && (index < this.m_bagEquipItemList.Count))
            {
                return this.m_bagEquipItemList[index];
            }
            return null;
        }

        public void GetEquipBuyPrice(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CEquipBuyPrice equipBuyPrice)
        {
            equipBuyPrice.Clear();
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if (dataByKey != null)
                {
                    uint dwBuyPrice = (uint) dataByKey.dwBuyPrice;
                    uint num2 = 0;
                    CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
                    freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
                    freeExistEquipInfoSet.ResetCalculateAmount();
                    num2 = this.GetPreEquipSwappedPrice(equipID, ref freeExistEquipInfoSet, ref equipBuyPrice);
                    freeExistEquipInfoSet.m_used = false;
                    if (dwBuyPrice >= num2)
                    {
                        dwBuyPrice -= num2;
                    }
                    else
                    {
                        dwBuyPrice = 0;
                    }
                    equipBuyPrice.m_buyPrice = (CrypticInt32) dwBuyPrice;
                }
            }
        }

        private CEquipInfo GetEquipInfo(ushort equipID)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info;
            }
            return null;
        }

        private Transform GetEquipItem(int level, int index)
        {
            index = ((level - 1) * 8) + index;
            if ((index >= 0) && (index < this.m_equipItemList.Count))
            {
                return this.m_equipItemList[index];
            }
            return null;
        }

        private CEquipBuyPrice GetFreeEquipBuyPrice()
        {
            for (int i = 0; i < this.m_equipBuyPricePool.Count; i++)
            {
                if (!this.m_equipBuyPricePool[i].m_used)
                {
                    this.m_equipBuyPricePool[i].Clear();
                    this.m_equipBuyPricePool[i].m_used = true;
                    return this.m_equipBuyPricePool[i];
                }
            }
            CEquipBuyPrice item = new CEquipBuyPrice(0);
            this.m_equipBuyPricePool.Add(item);
            item.m_used = true;
            return item;
        }

        private CExistEquipInfoSet GetFreeExistEquipInfoSet()
        {
            for (int i = 0; i < this.m_existEquipInfoSetPool.Count; i++)
            {
                if (!this.m_existEquipInfoSetPool[i].m_used)
                {
                    this.m_existEquipInfoSetPool[i].Clear();
                    this.m_existEquipInfoSetPool[i].m_used = true;
                    return this.m_existEquipInfoSetPool[i];
                }
            }
            CExistEquipInfoSet item = new CExistEquipInfoSet();
            this.m_existEquipInfoSetPool.Add(item);
            item.m_used = true;
            return item;
        }

        public ushort[] GetHostCtrlHeroQuicklyBuyEquipList()
        {
            return this.m_hostPlayerQuicklyBuyEquipIDs;
        }

        private CEquipBuyPrice GetHostPlayerCachedEquipBuyPrice(ushort equipID)
        {
            CEquipBuyPrice price = null;
            if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(equipID, out price))
            {
                return price;
            }
            return null;
        }

        private uint GetPreEquipSwappedPrice(ushort equipID, ref CExistEquipInfoSet existEquipInfoSet, ref CEquipBuyPrice equipBuyPrice)
        {
            if (equipID == 0)
            {
                return 0;
            }
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if (dataByKey == null)
            {
                return 0;
            }
            uint num = 0;
            for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
            {
                if (dataByKey.PreEquipID[i] <= 0)
                {
                    continue;
                }
                bool flag = false;
                for (int j = 0; j < existEquipInfoSet.m_existEquipInfoCount; j++)
                {
                    if ((existEquipInfoSet.m_existEquipInfos[j].m_equipID == dataByKey.PreEquipID[i]) && (existEquipInfoSet.m_existEquipInfos[j].m_calculateAmount > 0))
                    {
                        flag = true;
                        num += (uint) existEquipInfoSet.m_existEquipInfos[j].m_unitBuyPrice;
                        existEquipInfoSet.m_existEquipInfos[j].m_calculateAmount--;
                        equipBuyPrice.AddSwappedPreEquipInfo(dataByKey.PreEquipID[i]);
                        break;
                    }
                }
                if (!flag)
                {
                    num += this.GetPreEquipSwappedPrice(dataByKey.PreEquipID[i], ref existEquipInfoSet, ref equipBuyPrice);
                }
            }
            return num;
        }

        public ushort[] GetQuicklyBuyEquipList(ref PoolObjHandle<ActorRoot> actor)
        {
            for (int i = 0; i < 2L; i++)
            {
                this.m_tempQuicklyBuyEquipIDs[i] = 0;
            }
            if ((actor != 0) && (actor.handle.EquipComponent != null))
            {
                ListView<CRecommendEquipInfo> recommendEquipInfos = actor.handle.EquipComponent.GetRecommendEquipInfos();
                Dictionary<ushort, uint> equipBoughtHistory = actor.handle.EquipComponent.GetEquipBoughtHistory();
                bool flag = false;
                List<ushort> usageLevelEquipList = this.GetUsageLevelEquipList(enEquipUsage.Move, 2);
                if (usageLevelEquipList != null)
                {
                    for (int k = 0; k < usageLevelEquipList.Count; k++)
                    {
                        if (equipBoughtHistory.ContainsKey(usageLevelEquipList[k]))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
                freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
                int index = 0;
                for (int j = 0; j < recommendEquipInfos.Count; j++)
                {
                    if (!recommendEquipInfos[j].m_hasBeenBought && ((recommendEquipInfos[j].m_resEquipInBattle.bUsage != 4) || !flag))
                    {
                        this.m_tempRelatedRecommondEquips.Clear();
                        freeExistEquipInfoSet.ResetCalculateAmount();
                        this.GetRelatedRecommondEquips(recommendEquipInfos[j].m_equipID, true, ref actor, ref freeExistEquipInfoSet, ref this.m_tempRelatedRecommondEquips);
                        for (int m = 0; m < index; m++)
                        {
                            for (int num6 = 0; num6 < this.m_tempRelatedRecommondEquips.Count; num6++)
                            {
                                if (this.m_tempQuicklyBuyEquipIDs[m] == this.m_tempRelatedRecommondEquips[num6].m_equipID)
                                {
                                    this.m_tempRelatedRecommondEquips.RemoveAt(num6);
                                    break;
                                }
                            }
                        }
                        if (this.m_tempRelatedRecommondEquips.Count == 0)
                        {
                            break;
                        }
                        this.m_tempRelatedRecommondEquips.Sort();
                        int num7 = Math.Min(this.m_tempRelatedRecommondEquips.Count, 2 - index);
                        for (int n = 0; n < num7; n++)
                        {
                            this.m_tempQuicklyBuyEquipIDs[index] = this.m_tempRelatedRecommondEquips[n].m_equipID;
                            index++;
                        }
                        if (index >= 2L)
                        {
                            break;
                        }
                    }
                }
                freeExistEquipInfoSet.m_used = false;
            }
            return this.m_tempQuicklyBuyEquipIDs;
        }

        public void GetRelatedRecommondEquips(ushort equipID, bool isRootEquip, ref PoolObjHandle<ActorRoot> actor, ref CExistEquipInfoSet existEquipInfoSet, ref ListView<CEquipInfo> relatedRecommondEquips)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                if (!isRootEquip)
                {
                    for (int i = 0; i < existEquipInfoSet.m_existEquipInfoCount; i++)
                    {
                        if ((existEquipInfoSet.m_existEquipInfos[i].m_equipID == equipID) && (existEquipInfoSet.m_existEquipInfos[i].m_calculateAmount > 0))
                        {
                            existEquipInfoSet.m_existEquipInfos[i].m_calculateAmount--;
                            return;
                        }
                    }
                }
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                if (this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
                {
                    if (!relatedRecommondEquips.Contains(info))
                    {
                        relatedRecommondEquips.Add(info);
                    }
                    freeEquipBuyPrice.m_used = false;
                }
                else
                {
                    freeEquipBuyPrice.m_used = false;
                    for (int j = 0; j < info.m_resEquipInBattle.PreEquipID.Length; j++)
                    {
                        this.GetRelatedRecommondEquips(info.m_resEquipInBattle.PreEquipID[j], false, ref actor, ref existEquipInfoSet, ref relatedRecommondEquips);
                    }
                }
            }
        }

        private ushort[] GetRequiredEquipIDs(ushort equipID)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info.m_requiredEquipIDs;
            }
            return null;
        }

        public List<ushort> GetUsageLevelEquipList(enEquipUsage equipUsage, int level)
        {
            if (((equipUsage > enEquipUsage.Recommend) && (equipUsage <= enEquipUsage.Jungle)) && ((level > 0) && (level <= 3)))
            {
                return this.m_equipList[(int) equipUsage][level - 1];
            }
            return null;
        }

        private bool HasEquipChangeFlag(enEquipChangeFlag flag)
        {
            return ((((enEquipChangeFlag) this.m_equipChangedFlags) & flag) > ((enEquipChangeFlag) 0));
        }

        private void InitBagEquipItemList()
        {
            this.m_bagEquipItemList.Clear();
            GameObject widget = this.m_equipFormScript.GetWidget(4);
            if (null != widget)
            {
                Transform transform = widget.transform;
                for (int i = 0; i < 6; i++)
                {
                    Transform item = transform.Find(string.Format("equipItem{0}", i));
                    this.m_bagEquipItemList.Add(item);
                }
            }
        }

        private void InitEquipItemHorizontalLine(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                Transform equipItem = null;
                Transform transform2 = null;
                Transform transform3 = null;
                int index = 0;
                for (int i = 0; i < 8; i++)
                {
                    equipItem = this.GetEquipItem(level, i);
                    if (null != equipItem)
                    {
                        transform2 = equipItem.Find("imgLineFront");
                        if (level <= 1)
                        {
                            transform2.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            index = (level <= 2) ? 0 : 1;
                            this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Right, transform2.gameObject);
                        }
                        transform3 = equipItem.Find("imgLineBack");
                        if (level >= 3)
                        {
                            transform3.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            index = (level >= 2) ? 1 : 0;
                            this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Left, transform3.gameObject);
                        }
                    }
                }
            }
        }

        private void InitEquipLevelPanel()
        {
            this.m_equipItemList.Clear();
            for (int i = 0; i < 3; i++)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(1 + i);
                if (widget != null)
                {
                    Transform transform = widget.transform;
                    for (int j = 0; j < 8; j++)
                    {
                        Transform item = transform.Find(string.Format("equipItem{0}", j));
                        this.m_equipItemList.Add(item);
                        if (item != null)
                        {
                            CanvasGroup component = item.GetComponent<CanvasGroup>();
                            if (component == null)
                            {
                                component = item.gameObject.AddComponent<CanvasGroup>();
                            }
                            component.alpha = 1f;
                            component.blocksRaycasts = true;
                        }
                    }
                }
            }
        }

        private void InitEquipPathLine()
        {
            if (this.m_equipFormScript != null)
            {
                this.m_equipRelationPath.Clear();
                Transform transform = this.m_equipFormScript.GetWidget(8).transform;
                for (int i = 0; i < 14; i++)
                {
                    Transform transform2 = transform.Find(string.Format("imgLine{0}", i));
                    int startRow = i % 7;
                    this.m_equipRelationPath.InitializeVerticalLine(i / 7, startRow, startRow + 1, transform2.gameObject);
                }
                GameObject widget = this.m_equipFormScript.GetWidget(1);
                this.InitEquipItemHorizontalLine(widget.transform, 1);
                GameObject obj4 = this.m_equipFormScript.GetWidget(2);
                this.InitEquipItemHorizontalLine(obj4.transform, 2);
                GameObject obj5 = this.m_equipFormScript.GetWidget(3);
                this.InitEquipItemHorizontalLine(obj5.transform, 3);
            }
        }

        public void Initialize(CUIFormScript battleFormScript, PoolObjHandle<ActorRoot> hostCtrlHero, bool enableEquipSystem, bool isInEquipLimitedLevel)
        {
            this.Clear();
            this.m_battleFormScript = battleFormScript;
            this.m_hostCtrlHero = hostCtrlHero;
            DebugHelper.Assert((bool) this.m_hostCtrlHero, "Initialize EquipSystem with null host ctrl hero.");
            this.m_isEnabled = enableEquipSystem;
            this.m_isInEquipLimitedLevel = isInEquipLimitedLevel;
            this.m_hostCtrlHeroPermitedToBuyEquip = false;
            EquipComponent.s_equipEffectSequence = 0;
            if (this.m_isEnabled)
            {
                this.RefreshHostPlayerCachedEquipBuyPrice();
                this.OnEquipFormOpen(null);
                Singleton<CUIManager>.GetInstance().CloseForm(s_equipFormPath);
                Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnHeroEquipInBattleChanged));
                Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
            }
        }

        private void InitializeRecommendEquipList()
        {
            this.ClearEquipList(enEquipUsage.Recommend);
            DebugHelper.Assert(this.m_hostCtrlHero == 1, "InitializeEquipList m_hostCtrlHero is null");
            if (this.m_hostCtrlHero != 0)
            {
                ListView<CRecommendEquipInfo> recommendEquipInfos = this.m_hostCtrlHero.handle.EquipComponent.GetRecommendEquipInfos();
                if (recommendEquipInfos != null)
                {
                    for (int i = 0; i < recommendEquipInfos.Count; i++)
                    {
                        this.AddRecommendPreEquip(recommendEquipInfos[i].m_equipID, true);
                    }
                }
            }
        }

        public bool IsEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CEquipBuyPrice equipBuyPrice)
        {
            if ((!this.m_isEnabled || (equipID == 0)) || (actor == 0))
            {
                return false;
            }
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if ((dataByKey == null) || (dataByKey.bInvalid > 0))
            {
                return false;
            }
            if ((dataByKey.bNeedPunish > 0) && !actor.handle.SkillControl.HasPunishSkill())
            {
                return false;
            }
            if ((dataByKey.wGroup > 0) && actor.handle.EquipComponent.HasEquipInGroup(dataByKey.wGroup))
            {
                return false;
            }
            ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
            if ((requiredEquipIDs != null) && (requiredEquipIDs.Length > 0))
            {
                bool flag = false;
                for (int i = 0; i < requiredEquipIDs.Length; i++)
                {
                    if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[i], 1))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (actor == this.m_hostCtrlHero)
            {
                equipBuyPrice.Clone(this.GetHostPlayerCachedEquipBuyPrice(equipID));
            }
            else
            {
                this.GetEquipBuyPrice(equipID, ref actor, ref equipBuyPrice);
            }
            if (((ulong) actor.handle.ValueComponent.GetGoldCoinInBattle()) < ((long) equipBuyPrice.m_buyPrice))
            {
                return false;
            }
            if (!actor.handle.EquipComponent.IsEquipCanAddedToGrid(equipID, ref equipBuyPrice))
            {
                return false;
            }
            return true;
        }

        public bool IsHorizonEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref int price)
        {
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if ((dataByKey == null) || (dataByKey.bInvalid > 0))
                {
                    return false;
                }
                price = (int) dataByKey.dwBuyPrice;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                {
                    SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[7];
                    if (((dataByKey.dwActiveSkillID > 0) && ((slot == null) || (dataByKey.dwActiveSkillID != slot.SkillObj.SkillID))) && (actor.handle.ValueComponent.GetGoldCoinInBattle() >= dataByKey.dwBuyPrice))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHorizonEquipOwn(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if ((dataByKey == null) || (dataByKey.bInvalid > 0))
                {
                    return false;
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                {
                    SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[7];
                    if ((slot != null) && (slot.SkillObj != null))
                    {
                        return (dataByKey.dwActiveSkillID == slot.SkillObj.SkillID);
                    }
                }
            }
            return false;
        }

        public bool IsHostCtrlHeroPermitedToBuyEquip()
        {
            return this.m_hostCtrlHeroPermitedToBuyEquip;
        }

        public bool IsInEquipLimitedLevel()
        {
            return this.m_isInEquipLimitedLevel;
        }

        public void OnActorGoldChange(int changeValue, int currentValue)
        {
            this.SetEquipChangeFlag(enEquipChangeFlag.GoldCoinInBattleChanged);
        }

        public void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.battleEquipPar.equipInfo != null)
            {
                this.SendBuyEquipFrameCommand(uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID, true);
            }
        }

        public void OnEquipBagItemSelect(CUIEvent uiEvent)
        {
            if (null != this.m_equipFormScript)
            {
                this.ClearCurSelectEquipItem();
                this.m_selBagPos = uiEvent.m_eventParams.battleEquipPar.pos;
                this.OnEquipItemSelectHandle(true, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_eventParams.battleEquipPar.equipItemObj);
            }
        }

        public void OnEquipBuyBtnClick(CUIEvent uiEvent)
        {
            if (this.m_selEquipInfo != null)
            {
                if (this.m_selEquipInfo.m_resEquipInBattle.bUsage == 6)
                {
                    this.SendBuyHorizonEquipFrameCommand(this.m_selEquipInfo.m_equipID);
                }
                else
                {
                    this.SendBuyEquipFrameCommand(this.m_selEquipInfo.m_equipID, false);
                }
            }
            this.CloseEquipFormRightPanel();
            this.ClearCurSelectEquipItem();
        }

        public void OnEquipFormClose(CUIEvent uiEvent)
        {
            this.m_equipRelationPath.Clear();
            this.m_equipFormScript = null;
            this.m_selEquipInfo = null;
            this.m_selEquipItemObj = null;
            this.m_selBagPos = -1;
            Singleton<CUIParticleSystem>.instance.Show(null);
        }

        public void OnEquipFormOpen(CUIEvent uiEvent)
        {
            this.m_equipFormScript = Singleton<CUIManager>.GetInstance().OpenForm(s_equipFormPath, true, true);
            if (this.m_equipFormScript != null)
            {
                if (this.m_uiEquipItemHeight == 0f)
                {
                    GameObject obj2 = this.m_equipFormScript.GetWidget(10);
                    if (obj2 != null)
                    {
                        this.m_uiEquipItemHeight = (obj2.transform as RectTransform).rect.height;
                    }
                }
                if (this.m_uiEquipItemContentDefaultHeight == 0f)
                {
                    GameObject obj3 = this.m_equipFormScript.GetWidget(11);
                    if (obj3 != null)
                    {
                        this.m_uiEquipItemContentDefaultHeight = (obj3.transform as RectTransform).rect.height;
                    }
                }
                this.InitEquipLevelPanel();
                this.InitBagEquipItemList();
                this.InitEquipPathLine();
                this.ClearAllEquipSelect();
                this.CloseEquipFormRightPanel();
                GameObject widget = this.m_equipFormScript.GetWidget(0);
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_bEnableShopHorizonTab)
                {
                    string[] titleList = new string[] { instance.GetText("Equip_Usage_Recommend"), instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle"), instance.GetText("Equip_Usage_Horizon") };
                    CUICommonSystem.InitMenuPanel(widget, titleList, (int) this.m_curEquipUsage);
                }
                else
                {
                    string[] strArray2 = new string[] { instance.GetText("Equip_Usage_Recommend"), instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle") };
                    CUICommonSystem.InitMenuPanel(widget, strArray2, (int) this.m_curEquipUsage);
                }
            }
            Singleton<CUIParticleSystem>.instance.Hide(null);
        }

        public void OnEquipItemSelect(CUIEvent uiEvent)
        {
            if (null != this.m_equipFormScript)
            {
                this.ClearCurSelectEquipItem();
                this.OnEquipItemSelectHandle(false, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_eventParams.battleEquipPar.equipItemObj);
            }
        }

        private void OnEquipItemSelectHandle(bool bBagClick, CEquipInfo equipInfo, Transform equipObj)
        {
            if (((equipInfo != null) && (null != equipObj)) && (null != this.m_equipFormScript))
            {
                int bUsage = equipInfo.m_resEquipInBattle.bUsage;
                if (!bBagClick || (bUsage == this.m_curEquipUsage))
                {
                    this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], this.m_equipInfoDictionary);
                }
                else
                {
                    GameObject obj2 = this.m_equipFormScript.GetWidget(0);
                    if (obj2 != null)
                    {
                        CUIListScript component = obj2.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SelectElement(bUsage, true);
                            this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], this.m_equipInfoDictionary);
                        }
                    }
                }
                this.m_selEquipInfo = equipInfo;
                this.m_selEquipItemObj = equipObj;
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, true);
                GameObject widget = this.m_equipFormScript.GetWidget(6);
                widget.CustomSetActive(!bBagClick);
                if (!bBagClick)
                {
                    this.RefreshEquipBuyPanel(widget.transform);
                }
                GameObject obj4 = this.m_equipFormScript.GetWidget(7);
                obj4.CustomSetActive(bBagClick);
                if (bBagClick)
                {
                    this.RefreshEquipSalePanel(obj4.transform);
                }
                GameObject obj5 = this.m_equipFormScript.GetWidget(5);
                obj5.CustomSetActive(true);
                this.RefreshEquipInfoPanel(obj5.transform);
            }
        }

        public void OnEquipSaleBtnClick(CUIEvent uiEvent)
        {
            if (this.m_selEquipInfo != null)
            {
                this.SendSellEquipFrameCommand(this.m_selBagPos);
            }
            this.CloseEquipFormRightPanel();
            this.ClearCurSelectEquipItem();
        }

        public void OnEquipTypeListSelect(CUIEvent uiEvent)
        {
            this.ClearCurSelectEquipItem();
            this.CloseEquipFormRightPanel();
            CUIListScript srcWidgetScript = (CUIListScript) uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curEquipUsage = (enEquipUsage) srcWidgetScript.GetSelectedIndex();
                this.RefreshEquipForm(true, true);
            }
        }

        private void OnHeroEquipInBattleChanged(uint actorObjectID, stEquipInfo[] equips)
        {
            if (this.m_isEnabled && ((this.m_hostCtrlHero != 0) && (actorObjectID == this.m_hostCtrlHero.handle.ObjID)))
            {
                this.RefreshHostPlayerCachedEquipBuyPrice();
                this.SetEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged);
            }
        }

        private void OnHeroRecommendEquipInit(uint actorObjectID)
        {
            if (this.m_isEnabled && ((this.m_hostCtrlHero != 0) && (actorObjectID == this.m_hostCtrlHero.handle.ObjID)))
            {
                this.InitializeRecommendEquipList();
            }
        }

        public void RefreshEquipBagPanel(Transform bagPanel)
        {
            if ((null != bagPanel) && (this.m_hostCtrlHero != 0))
            {
                stEquipInfo[] equips = this.m_hostCtrlHero._handleObj.EquipComponent.GetEquips();
                for (int i = 0; i < 6; i++)
                {
                    Transform bagEquipItem = this.GetBagEquipItem(i);
                    if (null != bagEquipItem)
                    {
                        Image component = bagEquipItem.Find("imgIcon").GetComponent<Image>();
                        CUIMiniEventScript script = bagEquipItem.GetComponent<CUIMiniEventScript>();
                        if (equips[i].m_amount >= 1)
                        {
                            script.enabled = true;
                            component.gameObject.CustomSetActive(true);
                            CEquipInfo equipInfo = this.GetEquipInfo(equips[i].m_equipID);
                            if (equipInfo != null)
                            {
                                component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false);
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.battleEquipPar.equipInfo = equipInfo;
                                eventParams.battleEquipPar.equipItemObj = bagEquipItem;
                                eventParams.battleEquipPar.pos = i;
                                script.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_BagItem_Select, eventParams);
                            }
                        }
                        else
                        {
                            script.enabled = false;
                            component.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void RefreshEquipBuyPanel(Transform buyPanel)
        {
            if ((null != buyPanel) && (this.m_selEquipInfo != null))
            {
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                bool flag = false;
                if (this.m_selEquipInfo.m_resEquipInBattle.bUsage == 6)
                {
                    int price = 0;
                    flag = this.IsHorizonEquipCanBought(this.m_selEquipInfo.m_equipID, ref this.m_hostCtrlHero, ref price);
                }
                else
                {
                    flag = this.IsEquipCanBought(this.m_selEquipInfo.m_equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
                }
                buyPanel.Find("buyPriceText").GetComponent<Text>().text = freeEquipBuyPrice.m_buyPrice.ToString();
                Button component = buyPanel.Find("buyBtn").GetComponent<Button>();
                CUICommonSystem.SetButtonEnableWithShader(component, flag && this.m_hostCtrlHeroPermitedToBuyEquip, true);
                CUIEventScript script = buyPanel.Find("buyBtn").GetComponent<CUIEventScript>();
                if (flag && this.m_hostCtrlHeroPermitedToBuyEquip)
                {
                    script.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_BuyBtn_Click);
                }
                GameObject gameObject = component.transform.FindChild("Text").gameObject;
                GameObject obj3 = component.transform.FindChild("CantBuyText").gameObject;
                gameObject.CustomSetActive(this.m_hostCtrlHeroPermitedToBuyEquip);
                obj3.CustomSetActive(!this.m_hostCtrlHeroPermitedToBuyEquip);
                freeEquipBuyPrice.m_used = false;
            }
        }

        public void RefreshEquipForm(bool bRefreshBagPanel, bool isSwichUsage)
        {
            if (null != this.m_equipFormScript)
            {
                if (this.m_hostCtrlHero == 0)
                {
                    DebugHelper.Assert(this.m_hostCtrlHero == 1, " RefreshEquipForm m_hostCtrlHero is null");
                }
                else
                {
                    GameObject widget = this.m_equipFormScript.GetWidget(1);
                    if (widget != null)
                    {
                        this.RefreshEquipLevelPanel(widget.transform, 1);
                    }
                    GameObject obj3 = this.m_equipFormScript.GetWidget(2);
                    if (obj3 != null)
                    {
                        this.RefreshEquipLevelPanel(obj3.transform, 2);
                    }
                    GameObject obj4 = this.m_equipFormScript.GetWidget(3);
                    if (obj4 != null)
                    {
                        this.RefreshEquipLevelPanel(obj4.transform, 3);
                    }
                    if (isSwichUsage && (this.m_equipList != null))
                    {
                        int count = 0;
                        List<ushort>[] listArray = this.m_equipList[(int) this.m_curEquipUsage];
                        if (listArray == null)
                        {
                            return;
                        }
                        for (int i = 0; i < listArray.Length; i++)
                        {
                            if (listArray[i].Count > count)
                            {
                                count = listArray[i].Count;
                            }
                        }
                        float num3 = this.m_uiEquipItemContentDefaultHeight - ((8 - count) * this.m_uiEquipItemHeight);
                        GameObject obj5 = this.m_equipFormScript.GetWidget(11);
                        if (obj5 != null)
                        {
                            RectTransform transform = obj5.transform as RectTransform;
                            transform.offsetMin = new Vector2(0f, -num3);
                            transform.offsetMax = new Vector2(0f, 0f);
                        }
                    }
                    if (bRefreshBagPanel)
                    {
                        GameObject obj6 = this.m_equipFormScript.GetWidget(4);
                        if (obj6 != null)
                        {
                            this.RefreshEquipBagPanel(obj6.transform);
                        }
                    }
                    if (this.m_selEquipInfo != null)
                    {
                        GameObject obj7 = this.m_equipFormScript.GetWidget(6);
                        if (obj7 != null)
                        {
                            this.RefreshEquipBuyPanel(obj7.transform);
                        }
                    }
                    GameObject obj8 = this.m_equipFormScript.GetWidget(9);
                    if (obj8 != null)
                    {
                        obj8.GetComponent<Text>().text = this.m_hostCtrlHero._handleObj.ValueComponent.GetGoldCoinInBattle().ToString();
                    }
                }
            }
        }

        public void RefreshEquipInfoPanel(Transform infoPanel)
        {
            if ((null != infoPanel) && (this.m_selEquipInfo != null))
            {
                infoPanel.Find("equipNameText").GetComponent<Text>().text = this.m_selEquipInfo.m_equipName;
                Text component = infoPanel.Find("Panel_equipPropertyDesc/equipPropertyDescText").GetComponent<Text>();
                component.text = this.m_selEquipInfo.m_equipPropertyDesc;
                RectTransform transform = component.transform as RectTransform;
                transform.anchoredPosition = new Vector2(0f, 0f);
            }
        }

        private void RefreshEquipItem(Transform equipItem, ushort equipID)
        {
            if (null != equipItem)
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    object[] inParameters = new object[] { equipID };
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = {0}", inParameters);
                }
                else if (this.m_hostCtrlHero == 0)
                {
                    DebugHelper.Assert(this.m_hostCtrlHero == 1, " RefreshEquipItem m_hostCtrlHero is null");
                }
                else if (equipInfo.m_resEquipInBattle != null)
                {
                    Image component = equipItem.Find("imgIcon").GetComponent<Image>();
                    component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false);
                    CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                    bool bActive = false;
                    bool flag2 = false;
                    Text text = equipItem.Find("priceText").GetComponent<Text>();
                    if (equipInfo.m_resEquipInBattle.bUsage == 6)
                    {
                        int price = 0;
                        flag2 = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref price);
                        bActive = this.IsHorizonEquipOwn(equipInfo.m_equipID, ref this.m_hostCtrlHero);
                        text.text = price.ToString();
                    }
                    else
                    {
                        flag2 = this.IsEquipCanBought(equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
                        bActive = this.m_hostCtrlHero._handleObj.EquipComponent.HasEquip(equipInfo.m_equipID, 1);
                        text.text = freeEquipBuyPrice.m_buyPrice.ToUintString();
                    }
                    component.color = (!flag2 && !bActive) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White;
                    equipItem.Find("nameText").GetComponent<Text>().text = equipInfo.m_equipName;
                    Transform transform = equipItem.Find("imgOwn");
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(bActive);
                    }
                    CUIMiniEventScript script = equipItem.GetComponent<CUIMiniEventScript>();
                    if (script != null)
                    {
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.battleEquipPar.equipInfo = equipInfo;
                        eventParams.battleEquipPar.equipItemObj = equipItem;
                        script.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_Item_Select, eventParams);
                    }
                    freeEquipBuyPrice.m_used = false;
                }
            }
        }

        private void RefreshEquipLevelPanel(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                List<ushort> list = this.m_equipList[(int) this.m_curEquipUsage][level - 1];
                int count = list.Count;
                int index = 0;
                index = 0;
                while ((index < 8) && (index < count))
                {
                    Transform equipItem = this.GetEquipItem(level, index);
                    this.RefreshEquipItem(equipItem, list[index]);
                    CanvasGroup component = equipItem.GetComponent<CanvasGroup>();
                    if (component != null)
                    {
                        component.alpha = 1f;
                        component.blocksRaycasts = true;
                    }
                    index++;
                }
                while (index < 8)
                {
                    Transform transform2 = this.GetEquipItem(level, index);
                    if (transform2 != null)
                    {
                        CanvasGroup group2 = transform2.GetComponent<CanvasGroup>();
                        if (group2 != null)
                        {
                            group2.alpha = 0f;
                            group2.blocksRaycasts = false;
                        }
                    }
                    index++;
                }
            }
        }

        public void RefreshEquipSalePanel(Transform salePanel)
        {
            if ((null != salePanel) && (this.m_selEquipInfo != null))
            {
                salePanel.Find("salePriceText").GetComponent<Text>().text = ((uint) this.m_selEquipInfo.m_resEquipInBattle.dwSalePrice).ToString();
                salePanel.Find("saleBtn").GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_SaleBtn_Click);
            }
        }

        private void RefreshHostPlayerCachedEquipBuyPrice()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                ushort key = current.Key;
                CEquipBuyPrice price = null;
                if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(key, out price))
                {
                    this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref price);
                }
                else
                {
                    price = new CEquipBuyPrice(0);
                    this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref price);
                    this.m_hostPlayerCachedEquipBuyPrice.Add(key, price);
                }
            }
        }

        private void RefreshHostPlayerQuicklyBuyEquipList()
        {
            if (this.m_hostCtrlHero != 0)
            {
                ushort[] quicklyBuyEquipList = this.GetQuicklyBuyEquipList(ref this.m_hostCtrlHero);
                bool flag = false;
                for (int i = 0; i < 2L; i++)
                {
                    if (this.m_hostPlayerQuicklyBuyEquipIDs[i] != quicklyBuyEquipList[i])
                    {
                        this.m_hostPlayerQuicklyBuyEquipIDs[i] = quicklyBuyEquipList[i];
                        if (!flag)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    this.RefreshQuicklyBuyPanel();
                }
            }
        }

        private void RefreshHostPlayerQuicklyBuyEquipPanel(bool hostCtrlHeroPermitedToBuyEquip)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                GameObject widget = fightFormScript.GetWidget(0x3a);
                if (widget != null)
                {
                    Image component = widget.GetComponent<Image>();
                    if (component != null)
                    {
                        component.color = new Color(!hostCtrlHeroPermitedToBuyEquip ? ((float) 0) : ((float) 1), 1f, 1f, 1f);
                    }
                }
                GameObject obj3 = fightFormScript.GetWidget(0x3b);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(hostCtrlHeroPermitedToBuyEquip);
                }
            }
        }

        private void RefreshQuicklyBuyPanel()
        {
            if (null != this.m_battleFormScript)
            {
                bool flag = false;
                for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
                {
                    ushort equipID = this.m_hostPlayerQuicklyBuyEquipIDs[i];
                    GameObject widget = this.m_battleFormScript.GetWidget(0x2f + i);
                    if (widget != null)
                    {
                        CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                        if (equipInfo != null)
                        {
                            flag = true;
                            widget.CustomSetActive(true);
                            Transform transform = widget.transform;
                            transform.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_battleFormScript, true, false, false);
                            transform.Find("Panel_Info/descText").GetComponent<Text>().text = string.Format("<color=#ffa500>{0}</color>\n{1}", equipInfo.m_equipName, equipInfo.m_equipDesc);
                            transform.Find("moneyText").GetComponent<Text>().text = ((uint) this.GetHostPlayerCachedEquipBuyPrice(equipInfo.m_equipID).m_buyPrice).ToString();
                            CUIMiniEventScript component = widget.GetComponent<CUIMiniEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.battleEquipPar.equipInfo = equipInfo;
                            eventParams.battleEquipPar.m_indexInQuicklyBuyList = i;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_RecommendEquip_Buy, eventParams);
                        }
                        else
                        {
                            widget.CustomSetActive(false);
                        }
                    }
                }
                this.m_tickFadeTime = !flag ? 0 : 0x2710;
                this.SetQuicklyBuyInfoPanelAlpha(1f);
            }
        }

        private void SellEquip(int equipIndexInGrid, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (equipIndexInGrid >= 0)) && (((equipIndexInGrid < 6) && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel)))
            {
                stEquipInfo[] equips = actor.handle.EquipComponent.GetEquips();
                if ((equips[equipIndexInGrid].m_equipID > 0) && (equips[equipIndexInGrid].m_amount > 0))
                {
                    ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equips[equipIndexInGrid].m_equipID);
                    if (dataByKey != null)
                    {
                        actor.handle.EquipComponent.RemoveEquip(equipIndexInGrid);
                        actor.handle.EquipComponent.UpdateEquipEffect();
                        actor.handle.ValueComponent.ChangeGoldCoinInBattle((int) dataByKey.dwSalePrice, false, false, new Vector3(), false);
                    }
                }
            }
        }

        public void SendBuyEquipFrameCommand(ushort equipID, bool isQuicklyBuy)
        {
            FrameCommand<PlayerBuyEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerBuyEquipCommand>();
            command.cmdData.m_equipID = equipID;
            command.Send();
            if (isQuicklyBuy)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_goumai", null);
                if (this.m_hostCtrlHero != 0)
                {
                    this.m_hostCtrlHero.handle.EquipComponent.m_iFastBuyEquipCount++;
                }
            }
            else if (this.m_hostCtrlHero != 0)
            {
                this.m_hostCtrlHero.handle.EquipComponent.m_iBuyEquipCount++;
            }
        }

        public void SendBuyHorizonEquipFrameCommand(ushort equipID)
        {
            FrameCommand<PlayerBuyHorizonEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerBuyHorizonEquipCommand>();
            command.cmdData.m_equipID = equipID;
            command.Send();
        }

        public void SendSellEquipFrameCommand(int equipIndexInGrid)
        {
            FrameCommand<PlayerSellEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerSellEquipCommand>();
            command.cmdData.m_equipIndex = equipIndexInGrid;
            command.Send();
        }

        private void SetEquipChangeFlag(enEquipChangeFlag flag)
        {
            this.m_equipChangedFlags = (uint) (((enEquipChangeFlag) this.m_equipChangedFlags) | flag);
        }

        private void SetEquipItemSelectFlag(Transform equipItemObj, bool bSelect)
        {
            if (equipItemObj != null)
            {
                Transform transform = equipItemObj.Find("selectImg");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(bSelect);
                }
            }
        }

        private void SetQuicklyBuyInfoPanelAlpha(float alphaValue)
        {
            if (null != this.m_battleFormScript)
            {
                for (int i = 0; i < 2L; i++)
                {
                    GameObject widget = this.m_battleFormScript.GetWidget(0x2f + i);
                    if ((widget != null) && widget.activeSelf)
                    {
                        Transform transform = widget.transform.Find("Panel_Info");
                        if (transform != null)
                        {
                            CanvasGroup component = transform.GetComponent<CanvasGroup>();
                            if (component != null)
                            {
                                component.alpha = alphaValue;
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (this.m_isEnabled)
            {
                if (this.HasEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged) || this.HasEquipChangeFlag(enEquipChangeFlag.GoldCoinInBattleChanged))
                {
                    if (this.m_hostCtrlHeroPermitedToBuyEquip)
                    {
                        this.RefreshHostPlayerQuicklyBuyEquipList();
                    }
                    this.RefreshEquipForm(this.HasEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged), false);
                }
                this.ClearEquipChangeFlag();
                if (this.m_hostCtrlHero != 0)
                {
                    bool flag = this.m_hostCtrlHero.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel);
                    if (this.m_hostCtrlHeroPermitedToBuyEquip != flag)
                    {
                        this.m_hostCtrlHeroPermitedToBuyEquip = flag;
                        if (this.m_hostCtrlHeroPermitedToBuyEquip)
                        {
                            this.RefreshHostPlayerQuicklyBuyEquipList();
                        }
                        this.RefreshHostPlayerQuicklyBuyEquipPanel(this.m_hostCtrlHeroPermitedToBuyEquip);
                        if ((this.m_equipFormScript != null) && (this.m_selEquipInfo != null))
                        {
                            GameObject widget = this.m_equipFormScript.GetWidget(6);
                            this.RefreshEquipBuyPanel(widget.transform);
                        }
                    }
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.m_tickFadeTime > 0)
            {
                this.m_tickFadeTime -= delta;
                if (this.m_tickFadeTime <= 0)
                {
                    this.m_bPlayAnimation = true;
                    this.m_tickAnimationTime = 0x7d0;
                }
            }
            if (this.m_bPlayAnimation)
            {
                this.m_tickAnimationTime -= delta;
                this.SetQuicklyBuyInfoPanelAlpha(((float) this.m_tickAnimationTime) / 2000f);
                this.m_bPlayAnimation = this.m_tickAnimationTime > 0;
            }
        }

        public class CEquipBuyPrice
        {
            public CrypticInt32 m_buyPrice;
            public int m_swappedPreEquipInfoCount;
            public stSwappedPreEquipInfo[] m_swappedPreEquipInfos;
            public bool m_used;

            public CEquipBuyPrice(uint buyPrice)
            {
                this.m_buyPrice = (CrypticInt32) buyPrice;
                this.m_swappedPreEquipInfos = new stSwappedPreEquipInfo[6];
                this.m_swappedPreEquipInfoCount = 0;
                this.m_used = true;
            }

            public void AddSwappedPreEquipInfo(ushort equipID)
            {
                for (int i = 0; i < this.m_swappedPreEquipInfoCount; i++)
                {
                    if (this.m_swappedPreEquipInfos[i].m_equipID == equipID)
                    {
                        this.m_swappedPreEquipInfos[i].m_swappedAmount++;
                        return;
                    }
                }
                if (this.m_swappedPreEquipInfoCount < 6)
                {
                    this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_equipID = equipID;
                    this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_swappedAmount = 1;
                    this.m_swappedPreEquipInfoCount++;
                }
            }

            public void Clear()
            {
                this.m_buyPrice = 0;
                for (int i = 0; i < 6; i++)
                {
                    this.m_swappedPreEquipInfos[i].m_equipID = 0;
                    this.m_swappedPreEquipInfos[i].m_swappedAmount = 0;
                }
                this.m_swappedPreEquipInfoCount = 0;
            }

            public void Clone(CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
            {
                if (equipBuyPrice != null)
                {
                    this.m_buyPrice = equipBuyPrice.m_buyPrice;
                    for (int i = 0; i < 6; i++)
                    {
                        this.m_swappedPreEquipInfos[i] = equipBuyPrice.m_swappedPreEquipInfos[i];
                    }
                    this.m_swappedPreEquipInfoCount = equipBuyPrice.m_swappedPreEquipInfoCount;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct stSwappedPreEquipInfo
            {
                public ushort m_equipID;
                public uint m_swappedAmount;
            }
        }

        private enum enEquipChangeFlag
        {
            EquipInBattleChanged = 2,
            GoldCoinInBattleChanged = 1
        }
    }
}

