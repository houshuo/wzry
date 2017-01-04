namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class PVESettleSys : Singleton<PVESettleSys>
    {
        private int _lastlvTarget = 1;
        private COMDT_SETTLE_RESULT_DETAIL m_SettleData;
        private Assets.Scripts.GameSystem.StarCondition[] m_WinConditions = new Assets.Scripts.GameSystem.StarCondition[3];
        public static readonly string PATH_EXP = "UGUI/Form/System/PvE/Settle/Form_PVEExpSettlement.prefab";
        public static readonly string PATH_ITEM = "UGUI/Form/System/PvE/Settle/Form_PVEAward.prefab";
        public static readonly string PATH_LEVELUP = "UGUI/Form/System/PvE/Settle/Form_PlayerLevelUp.prefab";
        public static readonly string PATH_LOSE = "UGUI/Form/System/PvE/Adv/Form_AdventureLose.prefab";
        public static readonly string PATH_STAR = "UGUI/Form/System/PvE/Settle/Form_PVEWinSettlement.prefab";

        private void BackToLobby(CUIEvent uiEvent)
        {
            this.CloseItemForm();
            if (!Singleton<CBattleGuideManager>.instance.bTrainingAdv)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.IsGameTypeAdventure())
                {
                    Singleton<CAdventureSys>.instance.OpenAdvForm(Singleton<CAdventureSys>.instance.currentChapter, Singleton<CAdventureSys>.instance.currentLevelSeq, Singleton<CAdventureSys>.instance.currentDifficulty);
                }
            }
        }

        private void BattleAgain(CUIEvent uiEvent)
        {
            this.CloseItemForm();
            if (!Singleton<CBattleGuideManager>.instance.bTrainingAdv)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.IsGameTypeAdventure())
                {
                    CUIEvent event2 = new CUIEvent();
                    Singleton<CAdventureSys>.instance.OpenAdvForm(curLvelContext.m_chapterNo, curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
                    event2.m_eventID = enUIEventID.Adv_OpenLevelForm;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
                }
            }
            else
            {
                CMatchingSystem.ReqStartTrainingLevel();
            }
        }

        private void calcStarConditions()
        {
            ListView<IStarEvaluation>.Enumerator enumerator = Singleton<StarSystem>.GetInstance().GetEnumerator();
            for (int i = 0; enumerator.MoveNext(); i++)
            {
                this.m_WinConditions[i].ConditionName = enumerator.Current.description;
                this.m_WinConditions[i].bCompelete = enumerator.Current.isSuccess;
            }
        }

        protected void CheckLevelUp()
        {
            if (this.m_SettleData.stAcntInfo.dwPvpExp < this.m_SettleData.stAcntInfo.dwPvpSettleExp)
            {
                CUIEvent event3 = new CUIEvent {
                    m_eventID = enUIEventID.Settle_OpenLvlUp
                };
                event3.m_eventParams.tag = ((int) this.m_SettleData.stAcntInfo.dwPvpLv) - 1;
                event3.m_eventParams.tag2 = (int) this.m_SettleData.stAcntInfo.dwPvpLv;
                CUIEvent uiEvent = event3;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
        }

        private void CloseItemForm()
        {
            Singleton<CShopSystem>.GetInstance().OpenMysteryShopActiveTip();
            PVESettleView.DoCoinTweenEnd();
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_ITEM);
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<GameBuilder>.instance.EndGame();
        }

        public Assets.Scripts.GameSystem.StarCondition[] GetCondition()
        {
            return this.m_WinConditions;
        }

        private void GotoNextLevel(CUIEvent uiEvent)
        {
            this.CloseItemForm();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsGameTypeAdventure())
            {
                int num = CAdventureSys.GetNextLevelId(curLvelContext.m_chapterNo, curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
                if (num != 0)
                {
                    CUIEvent event2 = new CUIEvent {
                        m_eventID = enUIEventID.Adv_OpenLevelForm
                    };
                    event2.m_eventParams.tag = num;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
                }
            }
            else if ((curLvelContext != null) && curLvelContext.IsGameTypeActivity())
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Explore_OpenForm);
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_EscapeAnim, new CUIEventManager.OnUIEventHandler(this.onEscapeAnim));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ShowExpForm, new CUIEventManager.OnUIEventHandler(this.ShowExpForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ShowRewardForm, new CUIEventManager.OnUIEventHandler(this.ShowRewardForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_BattleAgain, new CUIEventManager.OnUIEventHandler(this.BattleAgain));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_NextLevel, new CUIEventManager.OnUIEventHandler(this.GotoNextLevel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_BackToLobby, new CUIEventManager.OnUIEventHandler(this.BackToLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_CloseLvlUp, new CUIEventManager.OnUIEventHandler(this.OnCloseLvlUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_OpenLvlUp, new CUIEventManager.OnUIEventHandler(this.OnOpenLvlUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_Back, new CUIEventManager.OnUIEventHandler(this.OnCloseLoseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_OnGameEnd, new CUIEventManager.OnUIEventHandler(this.OnGameEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_AnimEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settle_ClickItemDetailEnd, new CUIEventManager.OnUIEventHandler(this.OnClickItemDetailEnd));
            base.Init();
        }

        private void OnAnimEnd(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_STAR);
            if (form != null)
            {
                if (uiEvent.m_eventParams.tagStr == "Win_Show")
                {
                    PVESettleView.OnStarWinAnimEnd(form, ref this.m_WinConditions);
                }
                else if (!uiEvent.m_eventParams.tagStr.Contains("Done"))
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
                }
            }
            if ((Singleton<CUIManager>.GetInstance().GetForm(PATH_ITEM) != null) && (uiEvent.m_eventParams.tagStr == "Box_Show_2"))
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
            }
        }

        public void OnAwardDisplayEnd()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(PATH_ITEM) != null)
            {
            }
        }

        private void OnClickItemDetailEnd(CUIEvent uiEvent)
        {
            this.ShowPveExp();
        }

        private void OnCloseLoseForm(CUIEvent uiEvent)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_LOSE);
            if ((!Singleton<CBattleGuideManager>.instance.bTrainingAdv && (curLvelContext != null)) && curLvelContext.IsGameTypeAdventure())
            {
                Singleton<CAdventureSys>.instance.OpenAdvForm(curLvelContext.m_chapterNo, curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
            }
        }

        private void OnCloseLvlUp(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_LEVELUP);
        }

        private void onEscapeAnim(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_STAR);
            if (form != null)
            {
                PVESettleView.StopStarAnim(form);
            }
            CUIFormScript expForm = Singleton<CUIManager>.GetInstance().GetForm(PATH_EXP);
            if (expForm != null)
            {
                PVESettleView.StopExpAnim(expForm);
            }
            CUIFormScript rewardForm = Singleton<CUIManager>.GetInstance().GetForm(PATH_ITEM);
            if (rewardForm != null)
            {
                PVESettleView.StopRewardAnim(rewardForm);
            }
        }

        private void OnGameEnd(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                srcFormScript.transform.Find("ClickBack").gameObject.CustomSetActive(true);
                srcFormScript.transform.Find("WaitNote").gameObject.CustomSetActive(false);
            }
            Singleton<GameBuilder>.instance.EndGame();
        }

        private void OnOpenLvlUp(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            int newLvl = uiEvent.m_eventParams.tag2;
            if (newLvl > this._lastlvTarget)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_LEVELUP, false, true);
                if (form != null)
                {
                    PVESettleView.ShowPlayerLevelUp(form, tag, newLvl);
                    uint outSkillId = 0;
                    if (CAddSkillView.NewPlayerLevelUnlockAddSkill(newLvl, tag, out outSkillId))
                    {
                        Transform transform = form.transform.FindChild("PlayerLvlUp/Panel/groupPanel/SkillPanel");
                        if (transform != null)
                        {
                            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(outSkillId);
                            if (dataByKey != null)
                            {
                                transform.gameObject.CustomSetActive(true);
                                Text component = transform.FindChild("Skill/SkillName").GetComponent<Text>();
                                if (component != null)
                                {
                                    component.text = Utility.UTF8Convert(dataByKey.szSkillName);
                                }
                                Image image = transform.FindChild("Skill/Icon").GetComponent<Image>();
                                if (image != null)
                                {
                                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                                    image.SetSprite(prefabPath, form, true, false, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        Transform transform2 = form.transform.FindChild("PlayerLvlUp/Panel/groupPanel/SkillPanel");
                        if (transform2 != null)
                        {
                            transform2.gameObject.CustomSetActive(false);
                        }
                    }
                }
                this._lastlvTarget = newLvl;
            }
        }

        public void OnStarWinAnimEnd()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_STAR);
            if (form != null)
            {
                PVESettleView.OnStarWinAnimEnd(form, ref this.m_WinConditions);
            }
        }

        private void openFormLose()
        {
            Singleton<CUIManager>.GetInstance().OpenForm(PATH_LOSE, false, true);
            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Defeat", null);
        }

        private void ShowExpForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_STAR);
            PVESettleView.SetExpFormData(Singleton<CUIManager>.GetInstance().OpenForm(PATH_EXP, false, true), this.m_SettleData);
        }

        protected void ShowPveExp()
        {
            PVESettleView.SetRewardFormData(Singleton<CUIManager>.GetInstance().OpenForm(PATH_ITEM, false, true), this.m_SettleData);
            if (this.m_SettleData != null)
            {
                uint iLevelID = (uint) this.m_SettleData.stGameInfo.iLevelID;
                uint bGameResult = this.m_SettleData.stGameInfo.bGameResult;
                uint[] param = new uint[] { iLevelID, bGameResult, 1, 1 };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.battleFin, param);
            }
        }

        private void ShowRewardForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_STAR);
            if (this.m_SettleData != null)
            {
                if (this.m_SettleData.stReward != null)
                {
                    COMDT_REWARD_DETAIL stReward = this.m_SettleData.stReward;
                    ListView<COMDT_REWARD_INFO> view = new ListView<COMDT_REWARD_INFO>();
                    for (int i = 0; i < stReward.bNum; i++)
                    {
                        COMDT_REWARD_INFO comdt_reward_info = stReward.astRewardDetail[i];
                        if (comdt_reward_info.bType == 6)
                        {
                            view.Add(comdt_reward_info);
                        }
                    }
                    if ((view.Count > 0) && (view[0].bType == 6))
                    {
                        CSymbolItem useable = (CSymbolItem) CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0L, view[0].stRewardInfo.stSymbol.dwSymbolID, (int) view[0].stRewardInfo.stSymbol.dwCnt, 0);
                        CUseableContainer container = new CUseableContainer(enCONTAINER_TYPE.ITEM);
                        container.Add(useable);
                        CUICommonSystem.ShowSymbol(container, enUIEventID.Settle_ClickItemDetailEnd);
                        MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.getSymbolReward, new uint[0]);
                        return;
                    }
                }
                this.ShowPveExp();
            }
        }

        public void StartSettle(COMDT_SETTLE_RESULT_DETAIL settleData = null, bool bFirstPass = true)
        {
            if ((settleData == null) || (settleData.stGameInfo.bGameResult == 2))
            {
                this.openFormLose();
            }
            else if (settleData.stGameInfo.bGameResult == 1)
            {
                this.m_SettleData = settleData;
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_STAR, false, true);
                Singleton<CSoundManager>.GetInstance().PostEvent("Set_Victor", null);
                this.calcStarConditions();
                PVESettleView.SetStarFormData(form, settleData, ref this.m_WinConditions);
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_EscapeAnim, new CUIEventManager.OnUIEventHandler(this.onEscapeAnim));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ShowExpForm, new CUIEventManager.OnUIEventHandler(this.ShowExpForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ShowRewardForm, new CUIEventManager.OnUIEventHandler(this.ShowRewardForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_BattleAgain, new CUIEventManager.OnUIEventHandler(this.BattleAgain));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_NextLevel, new CUIEventManager.OnUIEventHandler(this.GotoNextLevel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_BackToLobby, new CUIEventManager.OnUIEventHandler(this.BackToLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_CloseLvlUp, new CUIEventManager.OnUIEventHandler(this.OnCloseLvlUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_OpenLvlUp, new CUIEventManager.OnUIEventHandler(this.OnOpenLvlUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_Back, new CUIEventManager.OnUIEventHandler(this.OnCloseLoseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_OnGameEnd, new CUIEventManager.OnUIEventHandler(this.OnGameEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_AnimEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settle_ClickItemDetailEnd, new CUIEventManager.OnUIEventHandler(this.OnClickItemDetailEnd));
            base.UnInit();
        }
    }
}

