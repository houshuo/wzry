namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class HeroInfoPanel
    {
        private const int HERO_MAX_NUM = 5;
        private CUIListScript heroInfoList;
        private List<PoolObjHandle<ActorRoot>> m_enemyHeroList = new List<PoolObjHandle<ActorRoot>>();
        private int[] m_heroInfoChangeBitsList = new int[4];
        private Dictionary<uint, int> m_HeroToIndexDic = new Dictionary<uint, int>();
        private ListView<Image> m_hpImageCacheList = new ListView<Image>();
        private ListView<Text> m_reviveTextCacheList = new ListView<Text>();
        private List<PoolObjHandle<ActorRoot>> m_teamHeroList = new List<PoolObjHandle<ActorRoot>>();

        private void AddHeroToDic(PoolObjHandle<ActorRoot> actor, int listIndex)
        {
            if (actor != 0)
            {
                if (!this.m_HeroToIndexDic.ContainsKey(actor.handle.ObjID))
                {
                    this.m_HeroToIndexDic.Add(actor.handle.ObjID, listIndex);
                }
                else
                {
                    this.m_HeroToIndexDic[actor.handle.ObjID] = listIndex;
                }
            }
        }

        public void Clear()
        {
            this.UnInitEventListener();
            this.m_HeroToIndexDic.Clear();
            this.m_enemyHeroList.Clear();
            this.m_hpImageCacheList.Clear();
            this.m_reviveTextCacheList.Clear();
        }

        private CUIListElementScript GetHeroListElement(int listIndex)
        {
            if (this.heroInfoList != null)
            {
                return this.heroInfoList.GetElemenet(listIndex);
            }
            return null;
        }

        private Image GetHpImage(int index)
        {
            if ((index >= 0) && (index < this.m_hpImageCacheList.Count))
            {
                return this.m_hpImageCacheList[index];
            }
            return null;
        }

        private Text GetReviveText(int index)
        {
            if ((index >= 0) && (index < this.m_reviveTextCacheList.Count))
            {
                return this.m_reviveTextCacheList[index];
            }
            return null;
        }

        public void Init(GameObject heroInfoPanelObj)
        {
            if (null != heroInfoPanelObj)
            {
                this.heroInfoList = heroInfoPanelObj.GetComponent<CUIListScript>();
                if (null != this.heroInfoList)
                {
                    this.heroInfoList.SetElementAmount(10);
                    this.InitTextImageCacheList();
                    Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if (hostPlayer == null)
                    {
                        DebugHelper.Assert(false, "HeroInfoPanel Init hostPlayer is null");
                    }
                    else
                    {
                        this.m_HeroToIndexDic.Clear();
                        this.m_enemyHeroList.Clear();
                        this.m_teamHeroList.Clear();
                        int index = 0;
                        int num2 = 10;
                        bool bActive = false;
                        GameObject widget = null;
                        GameObject obj3 = null;
                        GameObject obj4 = null;
                        GameObject obj5 = null;
                        index = 0;
                        while (index < num2)
                        {
                            bActive = index >= 5;
                            CUIListElementScript elemenet = this.heroInfoList.GetElemenet(index);
                            if (elemenet != null)
                            {
                                elemenet.gameObject.CustomSetActive(false);
                                widget = elemenet.GetWidget(1);
                                obj3 = elemenet.GetWidget(2);
                                widget.CustomSetActive(!bActive);
                                obj3.CustomSetActive(bActive);
                                obj4 = elemenet.GetWidget(6);
                                obj5 = elemenet.GetWidget(7);
                                obj4.CustomSetActive(false);
                                obj5.CustomSetActive(!bActive);
                            }
                            index++;
                        }
                        List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
                        int num3 = 0;
                        int listIndex = 0;
                        for (index = 0; index < allPlayers.Count; index++)
                        {
                            CUIListElementScript elementScript = null;
                            if ((allPlayers[index].PlayerCamp == hostPlayer.PlayerCamp) && (allPlayers[index] != hostPlayer))
                            {
                                listIndex = num3;
                                elementScript = this.heroInfoList.GetElemenet(num3);
                                if ((elementScript != null) && (allPlayers[index].Captain != 0))
                                {
                                    elementScript.gameObject.CustomSetActive(true);
                                    this.InitHeroItemData(elementScript, allPlayers[index].Captain);
                                    this.AddHeroToDic(allPlayers[index].Captain, listIndex);
                                    this.m_teamHeroList.Add(allPlayers[index].Captain);
                                }
                                num3++;
                            }
                        }
                        this.InitEventListener();
                    }
                }
            }
        }

        private void InitEventListener()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
        }

        private void InitHeroItemData(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
        {
            if (((null != elementScript) && (actor != 0)) && (actor.handle.ValueComponent != null))
            {
                bool bActive = actor.handle.IsHostCamp();
                elementScript.gameObject.CustomSetActive(bActive || actor.handle.ActorControl.IsDeadState);
                this.RefreshHeroIconImage(elementScript, actor);
                this.RefreshHeroIconImageColor(elementScript, actor);
                GameObject widget = elementScript.GetWidget(3);
                GameObject obj3 = elementScript.GetWidget(4);
                if ((widget != null) && (obj3 != null))
                {
                    widget.CustomSetActive(bActive);
                    obj3.CustomSetActive(bActive);
                    if (bActive)
                    {
                        widget.GetComponent<Image>().fillAmount = actor.handle.ValueComponent.GetHpRate().single;
                    }
                }
                GameObject obj4 = elementScript.GetWidget(5);
                if (obj4 != null)
                {
                    if (actor.handle.ActorControl.IsDeadState)
                    {
                        obj4.CustomSetActive(true);
                        obj4.GetComponent<Text>().text = string.Format("{0}", Mathf.RoundToInt(actor.handle.ActorControl.ReviveCooldown * 0.001f));
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                    }
                }
            }
        }

        private void InitTextImageCacheList()
        {
            this.m_reviveTextCacheList.Clear();
            this.m_hpImageCacheList.Clear();
            if (null != this.heroInfoList)
            {
                int elementAmount = this.heroInfoList.GetElementAmount();
                CUIListElementScript elemenet = null;
                GameObject widget = null;
                GameObject obj3 = null;
                for (int i = 0; i < elementAmount; i++)
                {
                    elemenet = this.heroInfoList.GetElemenet(i);
                    if (elemenet != null)
                    {
                        widget = elemenet.GetWidget(3);
                        if (widget != null)
                        {
                            this.m_hpImageCacheList.Add(widget.GetComponent<Image>());
                        }
                        obj3 = elemenet.GetWidget(5);
                        if (obj3 != null)
                        {
                            this.m_reviveTextCacheList.Add(obj3.GetComponent<Text>());
                        }
                    }
                }
            }
        }

        private void OnActorDead(ref GameDeadEventParam eventParam)
        {
            if ((eventParam.src != 0) && (eventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                bool flag = eventParam.src.handle.IsHostCamp();
                int num = 0;
                CUIListElementScript elementScript = null;
                if (!flag)
                {
                    this.m_enemyHeroList.Add(eventParam.src);
                    num = this.m_enemyHeroList.Count - 1;
                    elementScript = this.GetHeroListElement(num + 5);
                }
                if (this.m_HeroToIndexDic.TryGetValue(eventParam.src.handle.ObjID, out num))
                {
                    elementScript = this.GetHeroListElement(num);
                }
                if (elementScript != null)
                {
                    if (!flag)
                    {
                        elementScript.gameObject.CustomSetActive(true);
                        this.InitHeroItemData(elementScript, eventParam.src);
                    }
                    else
                    {
                        this.RefreshHeroIconImageColor(elementScript, eventParam.src);
                        elementScript.GetWidget(5).CustomSetActive(true);
                        this.SetHeroChangeFlag(eventParam.src, enHeroInfoChangeType.ReviveCntDown, true);
                    }
                }
            }
        }

        private void OnActorRevive(ref DefaultGameEventParam eventParam)
        {
            if ((eventParam.src != 0) && (eventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                bool flag = eventParam.src.handle.IsHostCamp();
                int num = 0;
                if (flag)
                {
                    if (this.m_HeroToIndexDic.TryGetValue(eventParam.src.handle.ObjID, out num))
                    {
                        CUIListElementScript heroListElement = this.GetHeroListElement(num);
                        if (heroListElement != null)
                        {
                            this.RefreshHeroIconImageColor(heroListElement, eventParam.src);
                            heroListElement.GetWidget(5).CustomSetActive(false);
                        }
                        this.SetHeroChangeFlag(eventParam.src, enHeroInfoChangeType.ReviveCntDown, false);
                    }
                }
                else
                {
                    this.m_enemyHeroList.Remove(eventParam.src);
                    this.RefreshEnemyHeroListUI();
                }
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> actor, int curHp, int totalHp)
        {
            if (((actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && actor.handle.IsHostCamp())
            {
                this.SetHeroChangeFlag(actor, enHeroInfoChangeType.Hp, true);
            }
        }

        private void RefreshEnemyHeroListUI()
        {
            CUIListElementScript elementScript = null;
            int count = this.m_enemyHeroList.Count;
            for (int i = 0; i < 5; i++)
            {
                elementScript = this.GetHeroListElement(i + 5);
                if (elementScript != null)
                {
                    elementScript.gameObject.CustomSetActive(i < count);
                    if (i < count)
                    {
                        this.RefreshHeroIconImage(elementScript, this.m_enemyHeroList[i]);
                        this.RefreshHeroReviveCntDown(elementScript, this.m_enemyHeroList[i]);
                    }
                }
            }
        }

        private void RefreshHeroIconImage(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
        {
            if ((null != elementScript) && (actor != 0))
            {
                GameObject widget = elementScript.GetWidget(0);
                if (widget != null)
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) actor.handle.TheActorMeta.ConfigId);
                    Image component = widget.GetComponent<Image>();
                    if ((component != null) && (dataByKey != null))
                    {
                        component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustCircleSmall_Dir, dataByKey.szImagePath), Singleton<CBattleSystem>.instance.FightFormScript, true, false, false);
                    }
                }
            }
        }

        private void RefreshHeroIconImageColor(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
        {
            if ((null != elementScript) && (actor != 0))
            {
                GameObject widget = elementScript.GetWidget(0);
                if (widget != null)
                {
                    Image component = widget.GetComponent<Image>();
                    if (actor.handle.IsHostCamp())
                    {
                        component.color = !actor.handle.ActorControl.IsDeadState ? CUIUtility.s_Color_White : CUIUtility.s_Color_GrayShader;
                    }
                    else
                    {
                        component.color = CUIUtility.s_Color_GrayShader;
                    }
                }
            }
        }

        private void RefreshHeroReviveCntDown(CUIListElementScript elementScript, PoolObjHandle<ActorRoot> actor)
        {
            if ((null != elementScript) && (actor != 0))
            {
                GameObject widget = elementScript.GetWidget(5);
                if ((widget != null) && actor.handle.ActorControl.IsDeadState)
                {
                    widget.GetComponent<Text>().text = SimpleNumericString.GetNumeric(Mathf.RoundToInt(actor.handle.ActorControl.ReviveCooldown * 0.001f));
                }
            }
        }

        private void SetHeroChangeFlag(PoolObjHandle<ActorRoot> actor, enHeroInfoChangeType changeType, bool bSetFlag = true)
        {
            int num = 0;
            if ((actor != 0) && this.m_HeroToIndexDic.TryGetValue(actor.handle.ObjID, out num))
            {
                int num2 = this.m_heroInfoChangeBitsList[num];
                if (bSetFlag)
                {
                    num2 |= ((int) 1) << changeType;
                }
                else
                {
                    num2 &= ~(((int) 1) << changeType);
                }
                this.m_heroInfoChangeBitsList[num] = num2;
            }
        }

        private void UnInitEventListener()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
        }

        public void Update()
        {
            if (null != this.heroInfoList)
            {
                int count = this.m_teamHeroList.Count;
                CUIListElementScript elemenet = null;
                int num2 = 0;
                int index = 0;
                index = 0;
                while (index < count)
                {
                    PoolObjHandle<ActorRoot> handle = this.m_teamHeroList[index];
                    elemenet = this.heroInfoList.GetElemenet(index);
                    if (((handle != 0) && (handle.handle.SkillControl != null)) && (null != elemenet))
                    {
                        SkillSlot slot = null;
                        if ((handle.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_3, out slot) && slot.IsUnLock()) && slot.IsCDReady)
                        {
                            elemenet.GetWidget(6).CustomSetActive(true);
                        }
                        else
                        {
                            elemenet.GetWidget(6).CustomSetActive(false);
                        }
                        num2 = this.m_heroInfoChangeBitsList[index];
                        if (num2 != 0)
                        {
                            if ((num2 & 1) != 0)
                            {
                                Image hpImage = this.GetHpImage(index);
                                if (hpImage != null)
                                {
                                    hpImage.fillAmount = handle.handle.ValueComponent.GetHpRate().single;
                                }
                                num2 &= -2;
                            }
                            Text reviveText = this.GetReviveText(index);
                            if ((reviveText != null) && ((num2 & 2) != 0))
                            {
                                reviveText.text = SimpleNumericString.GetNumeric(Mathf.RoundToInt(handle.handle.ActorControl.ReviveCooldown * 0.001f));
                            }
                            this.m_heroInfoChangeBitsList[index] = num2;
                        }
                    }
                    index++;
                }
                int num4 = this.m_enemyHeroList.Count;
                for (index = 0; index < num4; index++)
                {
                    if ((this.heroInfoList.GetElemenet(index + 5) != null) && (this.m_enemyHeroList[index] != 0))
                    {
                        Text text2 = this.GetReviveText(index + 5);
                        if (text2 != null)
                        {
                            PoolObjHandle<ActorRoot> handle2 = this.m_enemyHeroList[index];
                            text2.text = SimpleNumericString.GetNumeric(Mathf.RoundToInt(handle2.handle.ActorControl.ReviveCooldown * 0.001f));
                        }
                    }
                }
            }
        }

        private enum enHeroInfoChangeType
        {
            DaZhao = 2,
            Hp = 0,
            None = -1,
            ReviveCntDown = 1
        }

        private enum enHeroInfoItemWidget
        {
            HeroIcon = 0,
            HpImage = 3,
            HpImageBg = 4,
            IconCircle = 1,
            IconCircleEnemy = 2,
            None = -1,
            ReviveCntDownText = 5,
            SkillCanUse = 6,
            SkillCanUseBg = 7
        }
    }
}

