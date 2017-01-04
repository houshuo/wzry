namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CPlayerPvpHistoryController : Singleton<CPlayerPvpHistoryController>
    {
        private bool bShowStatistics;
        private string DetailInfoFormPath = "UGUI/Form/System/Player/Form_History_Detail_Info";
        private uint m_dwLogicWorldID;
        private CUIFormScript m_form;
        private ListView<COMDT_PLAYER_FIGHT_RECORD> m_hostRecordList;
        private string m_LoseMVPPath = (CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Blue_Mvp");
        private CPlayerProfile m_profile;
        private ListView<COMDT_PLAYER_FIGHT_RECORD> m_recordList = new ListView<COMDT_PLAYER_FIGHT_RECORD>();
        private COMDT_PLAYER_FIGHT_RECORD m_showRecord;
        private ulong m_ullUid;
        private string m_WinMVPPath = (CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Red_Mvp");
        private const int MAX_RECORD_COUNT = 100;
        private const int MaxAchievement = 8;

        public void AddSelfRecordData(COMDT_PLAYER_FIGHT_RECORD record)
        {
            if (this.m_hostRecordList != null)
            {
                if (this.m_hostRecordList.Count >= 100)
                {
                    this.m_hostRecordList.RemoveAt(this.m_hostRecordList.Count - 1);
                }
                this.m_hostRecordList.Add(record);
                this.m_hostRecordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
            }
        }

        private void Clear()
        {
            this.m_ullUid = 0L;
            this.m_dwLogicWorldID = 0;
            this.m_recordList.Clear();
            this.m_profile = null;
            this.bShowStatistics = false;
            this.m_form = null;
        }

        public void ClearHostData()
        {
            if (this.m_hostRecordList != null)
            {
                this.m_hostRecordList.Clear();
                this.m_hostRecordList = null;
            }
        }

        public void CommitHistoryInfo(bool hostPlayerWin)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide())
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x5a0);
                this.CreatHistoryInfoRePortData(hostPlayerWin, ref msg.stPkgData.stFightHistoryReq.stFightRecord);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private static int ComparisonHistoryData(COMDT_PLAYER_FIGHT_RECORD a, COMDT_PLAYER_FIGHT_RECORD b)
        {
            if ((a != null) || (b != null))
            {
                if ((a != null) && (b == null))
                {
                    return -1;
                }
                if ((a == null) && (b != null))
                {
                    return 1;
                }
                if (a.dwGameStartTime > b.dwGameStartTime)
                {
                    return -1;
                }
                if (a.dwGameStartTime < b.dwGameStartTime)
                {
                    return 1;
                }
            }
            return 0;
        }

        private void CreateHistoryInfoPlayerData(PlayerKDA playerKDA, bool bWin, bool bWarmBattle, ref COMDT_PLAYER_FIGHT_DATA playerData)
        {
            if (playerKDA != null)
            {
                uint num = 0;
                uint num2 = 0;
                uint num3 = 0;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                int num8 = 0;
                bool bOpen = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                bool flag6 = false;
                bool flag7 = false;
                bool flag8 = false;
                ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    playerData.dwHeroID = (uint) enumerator.Current.HeroId;
                    playerData.bHeroLv = (byte) enumerator.Current.SoulLevel;
                    num4 += enumerator.Current.LegendaryNum;
                    num5 += enumerator.Current.PentaKillNum;
                    num6 += enumerator.Current.QuataryKillNum;
                    num7 += enumerator.Current.TripleKillNum;
                    num8 += enumerator.Current.DoubleKillNum;
                    num += (uint) enumerator.Current.hurtToEnemy;
                    num2 += (uint) enumerator.Current.hurtTakenByEnemy;
                    num3 += (uint) enumerator.Current.hurtToHero;
                    flag5 = flag5 || enumerator.Current.bHurtTakenMost;
                    flag4 = flag4 || enumerator.Current.bGetCoinMost;
                    flag3 = flag3 || enumerator.Current.bHurtToHeroMost;
                    flag7 = flag7 || enumerator.Current.bAsssistMost;
                    flag6 = flag6 || enumerator.Current.bKillMost;
                    flag8 = flag8 || enumerator.Current.bKillOrganMost;
                    stEquipInfo[] equips = enumerator.Current.Equips;
                    byte num9 = 0;
                    int index = 0;
                    int num11 = 0;
                    while (index < equips.Length)
                    {
                        if (equips[index].m_equipID != 0)
                        {
                            playerData.astEquipDetail[num11].bCnt = (byte) equips[index].m_amount;
                            playerData.astEquipDetail[num11].dwEquipID = equips[index].m_equipID;
                            num9 = (byte) (num9 + 1);
                            num11++;
                        }
                        index++;
                    }
                    playerData.bEquipNum = num9;
                }
                if (bWin)
                {
                    uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
                    if (mvpPlayer != 0)
                    {
                        bOpen = mvpPlayer == playerKDA.PlayerId;
                    }
                }
                else
                {
                    uint num13 = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
                    if (num13 != 0)
                    {
                        flag2 = num13 == playerKDA.PlayerId;
                    }
                }
                StringHelper.StringToUTF8Bytes(playerKDA.PlayerName, ref playerData.szPlayerName);
                playerData.bPlayerCamp = (byte) playerKDA.PlayerCamp;
                playerData.ullPlayerUid = playerKDA.PlayerUid;
                playerData.iPlayerLogicWorldID = playerKDA.WorldId;
                playerData.bPlayerLv = (byte) playerKDA.PlayerLv;
                playerData.bPlayerVipLv = (byte) playerKDA.PlayerVipLv;
                playerData.bKill = (byte) playerKDA.numKill;
                playerData.bDead = (byte) playerKDA.numDead;
                playerData.bAssist = (byte) playerKDA.numAssist;
                playerData.dwHurtToEnemy = num;
                playerData.dwHurtTakenByEnemy = num2;
                playerData.dwHurtToHero = num3;
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE, num4 > 0);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL, num5 > 0);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL, num6 > 0);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL, num7 > 0);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_DOUBLEKILL, num8 > 0);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST, flag5);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST, flag4);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST, flag3);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST, flag7);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST, flag6);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST, flag8);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP, bOpen);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP, flag2);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((playerKDA.PlayerUid == masterRoleInfo.playerUllUID) && (playerKDA.WorldId == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID))
                {
                    this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND, this.IsPlayWithFriend());
                }
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY, (playerKDA.bRunaway || playerKDA.bDisconnect) || playerKDA.bHangup);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER, playerKDA.IsComputer);
                this.SetFightAchive(ref playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE, bWarmBattle);
                PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerKDA.PlayerId).Captain;
                if ((captain != 0) && (captain.handle.ValueComponent != null))
                {
                    playerData.dwTotalCoin = (uint) captain.handle.ValueComponent.GetGoldCoinIncomeInBattle();
                }
            }
        }

        private void CreatHistoryInfoRePortData(bool bHostPlayerWin, ref COMDT_PLAYER_FIGHT_RECORD record)
        {
            COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            record.bGameType = Convert.ToByte(curLvelContext.GetGameType());
            if (bHostPlayerWin)
            {
                record.bWinCamp = Convert.ToByte(playerCamp);
            }
            else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                record.bWinCamp = Convert.ToByte(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            }
            else
            {
                record.bWinCamp = Convert.ToByte(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
            ScoreBoard scoreBoard = Singleton<CBattleSystem>.GetInstance().FightForm.scoreBoard;
            if (scoreBoard != null)
            {
                record.dwGameStartTime = (uint) scoreBoard.GetStartTime();
                record.dwGameTime = (uint) scoreBoard.GetDuration();
            }
            record.bMapType = (byte) curLvelContext.m_mapType;
            record.dwMapID = (uint) curLvelContext.m_mapID;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetEnumerator();
            byte index = 0;
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                PlayerKDA playerKDA = current.Value;
                bool bWin = (playerKDA.PlayerCamp != playerCamp) ? !bHostPlayerWin : bHostPlayerWin;
                this.CreateHistoryInfoPlayerData(playerKDA, bWin, curLvelContext.m_isWarmBattle, ref record.astPlayerFightData[index]);
                index = (byte) (index + 1);
            }
            record.bPlayerCnt = index;
            this.AddSelfRecordData(record);
        }

        public void Draw(CUIFormScript form)
        {
            if (form != null)
            {
                GameObject widget = form.GetWidget(9);
                if (widget != null)
                {
                    this.m_profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
                    this.m_form = form;
                    GameObject obj3 = Utility.FindChild(widget, "pnlPvPHistory");
                    obj3.CustomSetActive(true);
                    if ((this.m_ullUid == 0) && (this.m_dwLogicWorldID == 0))
                    {
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (((this.m_profile.m_uuid != masterRoleInfo.playerUllUID) || (MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID != this.m_profile.m_iLogicWorldId)) || (this.m_hostRecordList == null))
                        {
                            this.ReqHistoryInfo();
                            obj3.transform.FindChild("pnlContainer/HistoryList").GetComponent<CUIListScript>().SetElementAmount(0);
                            obj3.transform.FindChild("pnlContainer/HistoryList_node").gameObject.CustomSetActive(false);
                            Utility.FindChild(obj3, "pnlContainer/title").CustomSetActive(this.m_recordList.Count != 0);
                            return;
                        }
                        this.m_recordList.Clear();
                        this.m_recordList.AddRange(this.m_hostRecordList);
                    }
                    this.initHistoryList();
                }
            }
        }

        private ListView<COMDT_PLAYER_FIGHT_DATA> GetCampPlayerDataList(COM_PLAYERCAMP playerCamp, ref COMDT_PLAYER_FIGHT_RECORD record)
        {
            ListView<COMDT_PLAYER_FIGHT_DATA> view = new ListView<COMDT_PLAYER_FIGHT_DATA>();
            for (int i = 0; i < record.bPlayerCnt; i++)
            {
                if (record.astPlayerFightData[i].bPlayerCamp == ((byte) playerCamp))
                {
                    view.Add(record.astPlayerFightData[i]);
                }
            }
            return view;
        }

        private COMDT_PLAYER_FIGHT_DATA GetPlayerFightData(ulong uId, int iLogicWorldId, ref COMDT_PLAYER_FIGHT_RECORD record)
        {
            if (record != null)
            {
                for (int i = 0; i < record.bPlayerCnt; i++)
                {
                    COMDT_PLAYER_FIGHT_DATA comdt_player_fight_data = record.astPlayerFightData[i];
                    if (((comdt_player_fight_data.ullPlayerUid == uId) || (comdt_player_fight_data.ullPlayerUid == 0)) && (comdt_player_fight_data.iPlayerLogicWorldID == iLogicWorldId))
                    {
                        return comdt_player_fight_data;
                    }
                }
            }
            return null;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHistoryItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_DetailInfo, new CUIEventManager.OnUIEventHandler(this.OnClickDetailInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Close_Detail, new CUIEventManager.OnUIEventHandler(this.OnClickBackHistory));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Statistics, new CUIEventManager.OnUIEventHandler(this.OnClickStatistics));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnClickAddFriend));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.PlayerInfoSystem_Form_Close, new System.Action(this.Clear));
        }

        private void initHistoryList()
        {
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(9);
                if (widget != null)
                {
                    GameObject p = Utility.FindChild(widget, "pnlPvPHistory");
                    if (p != null)
                    {
                        p.transform.FindChild("pnlContainer/HistoryList").GetComponent<CUIListScript>().SetElementAmount(this.m_recordList.Count);
                        Utility.FindChild(p, "pnlContainer/title").CustomSetActive(this.m_recordList.Count != 0);
                    }
                }
            }
        }

        private void initSettleInfoPanel(CUIFormScript detailForm, int infoIndex, int hostPlayerCamp, bool bShowStatistics)
        {
            if (((detailForm != null) && (infoIndex < this.m_recordList.Count)) && (infoIndex >= 0))
            {
                Transform transform = detailForm.transform.FindChild("pnlContainer/Panel");
                this.m_showRecord = this.m_recordList[infoIndex];
                uint num = this.m_showRecord.dwGameTime / 60;
                uint num2 = this.m_showRecord.dwGameTime - (num * 60);
                CUICommonSystem.SetTextContent(transform.FindChild("Time/Duration"), string.Format("{0:D2}:{1:D2}", num, num2));
                bool bActive = this.m_showRecord.bWinCamp == hostPlayerCamp;
                transform.FindChild("WinLoseTitle/Win").gameObject.CustomSetActive(bActive);
                transform.FindChild("WinLoseTitle/Lose").gameObject.CustomSetActive(!bActive);
                transform.FindChild("SwitchStatistics").gameObject.CustomSetActive(!bShowStatistics);
                transform.FindChild("SwitchOverview").gameObject.CustomSetActive(bShowStatistics);
                transform.FindChild("LeftPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!bShowStatistics);
                transform.FindChild("LeftPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(bShowStatistics);
                transform.FindChild("RightPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!bShowStatistics);
                transform.FindChild("RightPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(bShowStatistics);
                ListView<COMDT_PLAYER_FIGHT_DATA> campPlayerDataList = this.GetCampPlayerDataList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ref this.m_showRecord);
                ListView<COMDT_PLAYER_FIGHT_DATA> view2 = this.GetCampPlayerDataList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ref this.m_showRecord);
                CUIListScript component = transform.FindChild("LeftPlayerPanel/LeftPlayerList").GetComponent<CUIListScript>();
                CUIListScript script2 = transform.FindChild("RightPlayerPanel/RightPlayerList").GetComponent<CUIListScript>();
                int num3 = 0;
                uint inTotalDamage = 0;
                uint inTotalToHeroDamage = 0;
                uint inTotalTakenDamage = 0;
                int num7 = 0;
                uint num8 = 0;
                uint num9 = 0;
                uint num10 = 0;
                for (int i = 0; i < campPlayerDataList.Count; i++)
                {
                    COMDT_PLAYER_FIGHT_DATA comdt_player_fight_data = campPlayerDataList[i];
                    num3 += comdt_player_fight_data.bKill;
                    inTotalDamage += comdt_player_fight_data.dwHurtToEnemy;
                    inTotalToHeroDamage += comdt_player_fight_data.dwHurtToHero;
                    inTotalTakenDamage += comdt_player_fight_data.dwHurtTakenByEnemy;
                }
                for (int j = 0; j < view2.Count; j++)
                {
                    COMDT_PLAYER_FIGHT_DATA comdt_player_fight_data2 = view2[j];
                    num7 += comdt_player_fight_data2.bKill;
                    num8 += comdt_player_fight_data2.dwHurtToEnemy;
                    num9 += comdt_player_fight_data2.dwHurtToHero;
                    num10 += comdt_player_fight_data2.dwHurtTakenByEnemy;
                }
                component.SetElementAmount(campPlayerDataList.Count);
                for (int k = 0; k < campPlayerDataList.Count; k++)
                {
                    COMDT_PLAYER_FIGHT_DATA playerData = campPlayerDataList[k];
                    CUIListElementScript elemenet = component.GetElemenet(k);
                    this.initSettleListItem(elemenet, playerData, inTotalDamage, inTotalTakenDamage, inTotalToHeroDamage);
                }
                script2.SetElementAmount(view2.Count);
                for (int m = 0; m < view2.Count; m++)
                {
                    COMDT_PLAYER_FIGHT_DATA comdt_player_fight_data4 = view2[m];
                    CUIListElementScript listItem = script2.GetElemenet(m);
                    this.initSettleListItem(listItem, comdt_player_fight_data4, num8, num10, num9);
                }
                CUICommonSystem.SetTextContent(transform.FindChild("TotalScore/LeftTotalKill"), num3.ToString());
                CUICommonSystem.SetTextContent(transform.FindChild("TotalScore/RightTotalKill"), num7.ToString());
            }
        }

        private void initSettleListItem(CUIListElementScript listItem, COMDT_PLAYER_FIGHT_DATA playerData, uint inTotalDamage, uint inTotalTakenDamage, uint inTotalToHeroDamage)
        {
            if (((listItem != null) && (playerData != null)) && (this.m_form != null))
            {
                SettlementHelper component = listItem.GetComponent<SettlementHelper>();
                component.Detail.CustomSetActive(true);
                component.Damage.CustomSetActive(false);
                this.UpdateEquip(component.Tianfu, playerData);
                this.UpdateAchievements(component.Achievements, playerData);
                if (this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP))
                {
                    component.Mvp.CustomSetActive(true);
                    string prefabPath = string.Empty;
                    prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, "Img_Icon_Red_Mvp");
                    component.Mvp.GetComponent<Image>().SetSprite(prefabPath, this.m_form, true, false, false);
                }
                else if (this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP))
                {
                    component.Mvp.CustomSetActive(true);
                    string str2 = string.Empty;
                    str2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, "Img_Icon_Blue_Mvp");
                    component.Mvp.GetComponent<Image>().SetSprite(str2, this.m_form, true, false, false);
                }
                else
                {
                    component.Mvp.CustomSetActive(false);
                }
                component.PlayerName.GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref playerData.szPlayerName);
                component.PlayerLv.GetComponent<Text>().text = string.Format("Lv.{0}", playerData.bPlayerLv);
                if (((playerData.ullPlayerUid == 0) || (playerData.ullPlayerUid == this.m_profile.m_uuid)) && !this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER))
                {
                    component.PlayerName.GetComponent<Text>().color = CUIUtility.s_Text_Color_Self;
                    component.PlayerLv.GetComponent<Text>().color = CUIUtility.s_Text_Color_Self;
                    component.ItsMe.CustomSetActive(true);
                    component.PlayerLv.CustomSetActive(true);
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                }
                else
                {
                    component.ItsMe.CustomSetActive(false);
                    component.PlayerLv.CustomSetActive(false);
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), playerData.bPlayerVipLv, false);
                    if (playerData.bPlayerCamp == 1)
                    {
                        component.PlayerName.GetComponent<Text>().color = CUIUtility.s_Text_Color_Camp_1;
                        component.PlayerLv.GetComponent<Text>().color = CUIUtility.s_Text_Color_Camp_1;
                    }
                    else
                    {
                        component.PlayerName.GetComponent<Text>().color = CUIUtility.s_Text_Color_Camp_2;
                        component.PlayerLv.GetComponent<Text>().color = CUIUtility.s_Text_Color_Camp_2;
                    }
                }
                if (((playerData.ullPlayerUid == 0) || (playerData.ullPlayerUid == this.m_profile.m_uuid)) || (Singleton<CFriendContoller>.instance.model.IsGameFriend(playerData.ullPlayerUid, (uint) playerData.iPlayerLogicWorldID) || (this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER) && !this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE))))
                {
                    component.AddFriend.CustomSetActive(false);
                    component.m_AddfriendBtnShow = false;
                }
                else
                {
                    component.AddFriend.CustomSetActive(true);
                    component.m_AddfriendBtnShow = true;
                    component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = playerData.ullPlayerUid;
                    component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) playerData.iPlayerLogicWorldID;
                }
                component.HeroIcon.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(playerData.dwHeroID, 0)), this.m_form, true, false, false);
                component.HeroLv.GetComponent<Text>().text = string.Format("{0}", playerData.bHeroLv);
                component.Kill.GetComponent<Text>().text = playerData.bKill.ToString();
                component.Death.GetComponent<Text>().text = playerData.bDead.ToString();
                component.Assist.GetComponent<Text>().text = playerData.bAssist.ToString();
                component.Coin.GetComponent<Text>().text = playerData.dwTotalCoin.ToString();
                uint num = 0;
                uint num2 = 0;
                uint num3 = 0;
                num = Math.Max(1, inTotalDamage);
                num2 = Math.Max(1, inTotalTakenDamage);
                num3 = Math.Max(1, inTotalToHeroDamage);
                component.Damage.transform.FindChild("TotalDamageBg/TotalDamage").gameObject.GetComponent<Text>().text = playerData.dwHurtToEnemy.ToString();
                component.Damage.transform.FindChild("TotalDamageBg/TotalDamageBar").gameObject.GetComponent<Image>().fillAmount = ((float) playerData.dwHurtToEnemy) / ((float) num);
                component.Damage.transform.FindChild("TotalDamageBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) playerData.dwHurtToEnemy) / ((float) num));
                component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamage").gameObject.GetComponent<Text>().text = playerData.dwHurtTakenByEnemy.ToString();
                component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamageBar").gameObject.GetComponent<Image>().fillAmount = ((float) playerData.dwHurtTakenByEnemy) / ((float) num2);
                component.Damage.transform.FindChild("TotalTakenDamageBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) playerData.dwHurtTakenByEnemy) / ((float) num2));
                component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHero").gameObject.GetComponent<Text>().text = playerData.dwHurtToHero.ToString();
                component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHeroBar").gameObject.GetComponent<Image>().fillAmount = ((float) playerData.dwHurtToHero) / ((float) num3);
                component.Damage.transform.FindChild("TotalDamageHeroBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) playerData.dwHurtToHero) / ((float) num3));
                component.Detail.CustomSetActive(true);
                component.Damage.CustomSetActive(false);
            }
        }

        private bool IsFightAchiveSet(uint achiveBits, FIGHT_HISTORY_ACHIVE_BIT achiveBit)
        {
            return ((achiveBits & (((int) 1) << achiveBit)) > 0L);
        }

        private bool IsPlayWithFriend()
        {
            List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
            int count = allPlayers.Count;
            for (int i = 0; i < count; i++)
            {
                Player player = allPlayers[i];
                if (!player.Computer && (Singleton<CFriendContoller>.instance.model.IsGameFriend(player.PlayerUId, (uint) player.LogicWrold) || Singleton<CFriendContoller>.instance.model.IsSnsFriend(player.PlayerUId, (uint) player.LogicWrold)))
                {
                    return true;
                }
            }
            return false;
        }

        public void Load(CUIFormScript form)
        {
            if (form != null)
            {
                CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/PvPHistory", "pnlPvPHistory", form.GetWidget(9), form);
            }
        }

        public bool Loaded(CUIFormScript form)
        {
            if (form == null)
            {
                return false;
            }
            GameObject widget = form.GetWidget(9);
            if (widget == null)
            {
                return false;
            }
            if (Utility.FindChild(widget, "pnlPvPHistory") == null)
            {
                return false;
            }
            return true;
        }

        private void OnClickAddFriend(CUIEvent uiEvent)
        {
            Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint) uiEvent.m_eventParams.commonUInt64Param2, false);
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        private void OnClickBackHistory(CUIEvent uiEvt)
        {
            Singleton<CUIManager>.instance.CloseForm(this.DetailInfoFormPath);
        }

        private void OnClickDetailInfo(CUIEvent uiEvt)
        {
            this.bShowStatistics = false;
            CUIFormScript detailForm = Singleton<CUIManager>.GetInstance().OpenForm(this.DetailInfoFormPath, true, true);
            this.initSettleInfoPanel(detailForm, uiEvt.m_eventParams.tag, uiEvt.m_eventParams.tag2, false);
        }

        private void OnClickStatistics(CUIEvent uiEvt)
        {
            Transform transform = uiEvt.m_srcFormScript.transform.FindChild("pnlContainer/Panel");
            if (transform != null)
            {
                this.bShowStatistics = !this.bShowStatistics;
                transform.FindChild("LeftPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!this.bShowStatistics);
                transform.FindChild("LeftPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(this.bShowStatistics);
                transform.FindChild("RightPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!this.bShowStatistics);
                transform.FindChild("RightPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(this.bShowStatistics);
                transform.FindChild("SwitchStatistics").gameObject.CustomSetActive(!this.bShowStatistics);
                transform.FindChild("SwitchOverview").gameObject.CustomSetActive(this.bShowStatistics);
                CUIListScript component = transform.FindChild("LeftPlayerPanel/LeftPlayerList").GetComponent<CUIListScript>();
                CUIListScript script2 = transform.FindChild("RightPlayerPanel/RightPlayerList").GetComponent<CUIListScript>();
                if (component != null)
                {
                    int elementAmount = component.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper helper = component.GetElemenet(i).gameObject.GetComponent<SettlementHelper>();
                        helper.Detail.CustomSetActive(!this.bShowStatistics);
                        helper.Damage.CustomSetActive(this.bShowStatistics);
                    }
                }
                if (script2 != null)
                {
                    int num3 = script2.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = script2.GetElemenet(j).gameObject.GetComponent<SettlementHelper>();
                        helper2.Detail.CustomSetActive(!this.bShowStatistics);
                        helper2.Damage.CustomSetActive(this.bShowStatistics);
                    }
                }
            }
        }

        private void OnHistoryItemEnable(CUIEvent uiEvt)
        {
            PvpHistoryItemHelper component = uiEvt.m_srcWidget.GetComponent<PvpHistoryItemHelper>();
            if (((component != null) && (uiEvt.m_srcWidgetIndexInBelongedList < this.m_recordList.Count)) && (uiEvt.m_srcWidgetIndexInBelongedList >= 0))
            {
                COMDT_PLAYER_FIGHT_RECORD record = this.m_recordList[uiEvt.m_srcWidgetIndexInBelongedList];
                COMDT_PLAYER_FIGHT_DATA playerData = this.GetPlayerFightData(this.m_profile.m_uuid, this.m_profile.m_iLogicWorldId, ref record);
                if (playerData != null)
                {
                    Image image = component.Mvp.GetComponent<Image>();
                    if (this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP))
                    {
                        image.SetSprite(this.m_WinMVPPath, uiEvt.m_srcFormScript, true, false, false);
                    }
                    else if (this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP))
                    {
                        image.SetSprite(this.m_LoseMVPPath, uiEvt.m_srcFormScript, true, false, false);
                    }
                    else
                    {
                        component.Mvp.CustomSetActive(false);
                    }
                    string strContent = (record.bWinCamp != playerData.bPlayerCamp) ? Singleton<CTextManager>.instance.GetText("GameResult_Lose") : Singleton<CTextManager>.instance.GetText("GameResult_Win");
                    CUICommonSystem.SetTextContent(component.reSesultText, strContent);
                    component.KDAText.GetComponent<Text>().text = string.Format("{0}  /  {1}  /  {2}", playerData.bKill, playerData.bDead, playerData.bAssist);
                    component.SetEuipItems(ref playerData, uiEvt.m_srcFormScript);
                    component.headObj.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(playerData.dwHeroID, 0)), this.m_form, true, false, false);
                    component.FriendItem.CustomSetActive(this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND));
                    COM_GAME_TYPE bGameType = (COM_GAME_TYPE) record.bGameType;
                    if (bGameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)
                    {
                        if (bGameType != COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM)
                        {
                            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(record.bMapType, record.dwMapID);
                            if (pvpMapCommonInfo != null)
                            {
                                component.MatchTypeText.GetComponent<Text>().text = pvpMapCommonInfo.szGameMatchName;
                            }
                        }
                        else
                        {
                            component.MatchTypeText.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("开房间");
                        }
                    }
                    else if (!this.IsFightAchiveSet(playerData.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE))
                    {
                        component.MatchTypeText.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("人机对战");
                    }
                    else
                    {
                        ResDT_LevelCommonInfo info = CLevelCfgLogicManager.GetPvpMapCommonInfo(record.bMapType, record.dwMapID);
                        if (info != null)
                        {
                            component.MatchTypeText.GetComponent<Text>().text = info.szGameMatchName;
                        }
                    }
                    DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                    DateTime time2 = Utility.ToUtcTime2Local((long) record.dwGameStartTime);
                    int num = time.Day - time2.Day;
                    Text text = component.time.transform.FindChild("Today").GetComponent<Text>();
                    string str2 = string.Empty;
                    switch (num)
                    {
                        case 0:
                            str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips3"), time2.Hour, time2.Minute);
                            break;

                        case 1:
                            str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips4"), time2.Hour, time2.Minute);
                            break;

                        case 2:
                            str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips5"), time2.Hour, time2.Minute);
                            break;

                        default:
                        {
                            object[] args = new object[] { time2.Month, time2.Day, time2.Hour, time2.Minute };
                            str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips6"), args);
                            break;
                        }
                    }
                    text.text = str2;
                    component.ShowDetailBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = uiEvt.m_srcWidgetIndexInBelongedList;
                    component.ShowDetailBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag2 = playerData.bPlayerCamp;
                }
            }
        }

        private void OnReciveFightData()
        {
            this.initHistoryList();
        }

        [MessageHandler(0x5a2)]
        public static void ReciveHistoryInfo(CSPkg msg)
        {
            if (msg.stPkgData.stFightHistoryListRsp.bErrorCode != 0)
            {
                goto Label_01C1;
            }
            CSDT_FIGHTHISTORY_RECORD_DETAIL_SUCC stSucc = msg.stPkgData.stFightHistoryListRsp.stRecordDetail.stSucc;
            COMDT_PLAYER_FIGHT_RECORD[] astRecord = msg.stPkgData.stFightHistoryListRsp.stRecordDetail.stSucc.stRecordList.astRecord;
            Singleton<CPlayerPvpHistoryController>.GetInstance().m_ullUid = stSucc.ullUid;
            Singleton<CPlayerPvpHistoryController>.GetInstance().m_dwLogicWorldID = stSucc.dwLogicWorldID;
            if ((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID != stSucc.ullUid) || (MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID != stSucc.dwLogicWorldID))
            {
                Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Clear();
                for (int i = 0; i < astRecord.Length; i++)
                {
                    if (astRecord[i].bGameType == 0)
                    {
                        break;
                    }
                    Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Add(astRecord[i]);
                }
            }
            else
            {
                if (Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList == null)
                {
                    Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList = new ListView<COMDT_PLAYER_FIGHT_RECORD>();
                }
                for (int j = 0; j < astRecord.Length; j++)
                {
                    if (astRecord[j].bGameType == 0)
                    {
                        break;
                    }
                    Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList.Add(astRecord[j]);
                }
                Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
                Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Clear();
                Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.AddRange(Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList);
                goto Label_01B7;
            }
            Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
        Label_01B7:
            Singleton<CPlayerPvpHistoryController>.instance.OnReciveFightData();
        Label_01C1:
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
        }

        private void ReqHistoryInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x5a1);
            msg.stPkgData.stFightHistoryListReq.ullUid = this.m_profile.m_uuid;
            msg.stPkgData.stFightHistoryListReq.dwLogicWorldID = (uint) this.m_profile.m_iLogicWorldId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetAchievementIcon(GameObject achievements, PvpAchievement type, int index)
        {
            if ((index <= 8) && (achievements != null))
            {
                Transform transform = achievements.transform.FindChild(string.Format("Achievement{0}", index));
                if (transform != null)
                {
                    if (type == PvpAchievement.NULL)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, type.ToString());
                        transform.gameObject.CustomSetActive(true);
                        transform.GetComponent<Image>().SetSprite(prefabPath, this.m_form, true, false, false);
                    }
                }
            }
        }

        private void SetFightAchive(ref uint achiveBits, FIGHT_HISTORY_ACHIVE_BIT achiveBit, bool bOpen)
        {
            if (bOpen)
            {
                achiveBits |= ((int) 1) << achiveBit;
            }
            else
            {
                achiveBits &= ~(((int) 1) << achiveBit);
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHistoryItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_DetailInfo, new CUIEventManager.OnUIEventHandler(this.OnClickDetailInfo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Close_Detail, new CUIEventManager.OnUIEventHandler(this.OnClickBackHistory));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Statistics, new CUIEventManager.OnUIEventHandler(this.OnClickStatistics));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnClickAddFriend));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerInfoSystem_Form_Close, new System.Action(this.Clear));
            this.m_form = null;
        }

        private void UpdateAchievements(GameObject achievements, COMDT_PLAYER_FIGHT_DATA kda)
        {
            int index = 1;
            bool flag = false;
            for (int i = 0; i < 12; i++)
            {
                switch (((FIGHT_HISTORY_ACHIVE_BIT) i))
                {
                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.Legendary, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL) && !flag)
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.PentaKill, index);
                            index++;
                            flag = true;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL) && !flag)
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.QuataryKill, index);
                            index++;
                            flag = true;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL) && !flag)
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.TripleKill, index);
                            index++;
                            flag = true;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.KillMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.HurtMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.HurtTakenMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.AsssistMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.GetCoinMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.KillOrganMost, index);
                            index++;
                        }
                        break;

                    case FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY:
                        if (this.IsFightAchiveSet(kda.dwRongyuFlag, FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY))
                        {
                            this.SetAchievementIcon(achievements, PvpAchievement.RunAway, index);
                            index++;
                        }
                        break;
                }
            }
            for (int j = index; j <= 8; j++)
            {
                this.SetAchievementIcon(achievements, PvpAchievement.NULL, j);
            }
        }

        private void UpdateEquip(GameObject equip, COMDT_PLAYER_FIGHT_DATA kda)
        {
            int num = 1;
            if ((equip != null) && (kda != null))
            {
                for (int i = 0; i < 6; i++)
                {
                    uint dwEquipID = kda.astEquipDetail[i].dwEquipID;
                    Transform transform = equip.transform.FindChild(string.Format("TianFu{0}", num));
                    if ((dwEquipID != 0) && (transform != null))
                    {
                        num++;
                        CUICommonSystem.SetEquipIcon((ushort) dwEquipID, transform.gameObject, this.m_form);
                    }
                }
                for (int j = num; j <= 6; j++)
                {
                    Transform transform2 = equip.transform.FindChild(string.Format("TianFu{0}", j));
                    if (transform2 != null)
                    {
                        transform2.gameObject.GetComponent<Image>().SetSprite(string.Format("{0}EquipmentSpace", CUIUtility.s_Sprite_Dynamic_Talent_Dir), this.m_form, true, false, false);
                    }
                }
            }
        }

        public enum FIGHT_HISTORY_ACHIVE_BIT
        {
            FIGHT_HISTORY_ACHIVE_GODLIKE,
            FIGHT_HISTORY_ACHIVE_PENTAKILL,
            FIGHT_HISTORY_ACHIVE_QUATARYKILL,
            FIGHT_HISTORY_ACHIVE_TRIPLEKILL,
            FIGHT_HISTORY_ACHIVE_DOUBLEKILL,
            FIGHT_HISTORY_ACHIVE_KILLMOST,
            FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST,
            FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST,
            FIGHT_HISTORY_ACHIVE_ASSISTMOST,
            FIGHT_HISTORY_ACHIVE_GETMOENYMOST,
            FIGHT_HISTORY_ACHIVE_KILLORGANMOST,
            FIGHT_HISTORY_ACHIVE_RUNAWAY,
            FIGHT_HISTORY_ACHIVE_FIGHTACHIVECOUNT,
            FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND,
            FIGHT_HISTORY_ACHIVE_WINMVP,
            FIGHT_HISTORY_ACHIVE_LOSEMVP,
            FIGHT_HISTORY_ACHIVE_ISCOMPUTER,
            FIGHT_HISTORY_ACHIVE_ISWARMBATTLE,
            FIGHT_HISTORY_ACHIVE_COUNT
        }
    }
}

