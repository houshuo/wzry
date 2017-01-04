namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CRoomView
    {
        public static void ResetSwapView()
        {
            SetChgEnable(true);
            SetSwapTimer(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
            ShowSwapMsg(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
        }

        public static void SetChgEnable(bool enable)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
            if (((form != null) && (form.gameObject != null)) && (Singleton<CRoomSystem>.instance.roomInfo != null))
            {
                MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
                int num = 1;
                int num2 = 2;
                GameObject obj2 = null;
                for (int i = num; i <= num2; i++)
                {
                    COM_PLAYERCAMP camp = (COM_PLAYERCAMP) i;
                    for (int j = 0; j < 5; j++)
                    {
                        bool flag = (masterMemberInfo.camp == camp) && (masterMemberInfo.dwPosOfCamp == j);
                        bool flag2 = false;
                        MemberInfo memberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo(camp, j);
                        if (memberInfo != null)
                        {
                            flag2 = memberInfo.RoomMemberType == 2;
                        }
                        string path = string.Format("Panel_Main/{0}{1}/Occupied/BtnSwap", (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "Right_Player" : "Left_Player", j + 1);
                        obj2 = Utility.FindChild(form.gameObject, path);
                        if (obj2 == null)
                        {
                            return;
                        }
                        obj2.CustomSetActive((!flag && !flag2) && enable);
                    }
                }
            }
        }

        public static void SetChgEnable(bool enable, COM_PLAYERCAMP camp, int pos)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
            if (((form != null) && (form.gameObject != null)) && (Singleton<CRoomSystem>.instance.roomInfo != null))
            {
                GameObject obj2 = null;
                string path = string.Format("Panel_Main/{0}{1}/Occupied/BtnSwap", (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "Right_Player" : "Left_Player", pos + 1);
                obj2 = Utility.FindChild(form.gameObject, path);
                if (obj2 != null)
                {
                    obj2.CustomSetActive(enable);
                }
            }
        }

        public static void SetComEnable(bool enable)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
            if ((form != null) && (form.gameObject != null))
            {
                Button componetInChild = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg1/LeftRobot");
                Button btn = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg2/RightRobot");
                CUICommonSystem.SetButtonEnable(componetInChild, enable, enable, true);
                CUICommonSystem.SetButtonEnable(btn, enable, enable, true);
            }
        }

        private static void SetPlayerSlotData(GameObject item, MemberInfo memberInfo, MemberInfo masterMemberInfo, COM_PLAYERCAMP camp, int pos, bool bAvailable)
        {
            if (bAvailable)
            {
                bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
                if (memberInfo == null)
                {
                    item.CustomSetActive(true);
                    item.transform.Find("Occupied").gameObject.CustomSetActive(false);
                    GameObject gameObject = item.transform.Find("BtnJoin").gameObject;
                    CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                    gameObject.CustomSetActive(true);
                    component.m_onClickEventID = enUIEventID.Room_ChangePos;
                    component.m_onClickEventParams.tag = (int) camp;
                    component.m_onClickEventParams.tag2 = pos;
                    component.m_onClickEventParams.tag3 = 1;
                }
                else
                {
                    item.CustomSetActive(true);
                    item.transform.Find("Occupied").gameObject.CustomSetActive(true);
                    item.transform.Find("BtnJoin").gameObject.CustomSetActive(false);
                    bool bActive = false;
                    bActive = Singleton<CRoomSystem>.GetInstance().roomInfo.roomOwner.ullUid == memberInfo.ullUid;
                    bool flag3 = false;
                    flag3 = Singleton<CRoomSystem>.GetInstance().roomInfo.selfInfo.ullUid == memberInfo.ullUid;
                    item.transform.Find("Occupied/imgOwner").gameObject.CustomSetActive(bActive);
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
                    bool flag4 = true;
                    if (form != null)
                    {
                        flag4 = form.GetWidget(7).GetComponent<CUIListScript>().GetSelectedIndex() == 0;
                    }
                    string str = !flag4 ? Singleton<CInviteSystem>.GetInstance().GetInviteGuildMemberName(memberInfo.ullUid) : Singleton<CInviteSystem>.GetInstance().GetInviteFriendName(memberInfo.ullUid, (uint) memberInfo.iLogicWorldID);
                    item.transform.Find("Occupied/txtPlayerName").GetComponent<Text>().text = !string.IsNullOrEmpty(str) ? str : memberInfo.MemberName;
                    GameObject obj3 = Utility.FindChild(item, "Occupied/BtnSwap");
                    Transform transform = item.transform.Find("Occupied/BtnAddFriend");
                    if (flag3)
                    {
                        if (!CSysDynamicBlock.bFriendBlocked)
                        {
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (masterRoleInfo != null)
                            {
                                item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<CUIHttpImageScript>().SetImageUrl(masterRoleInfo.HeadUrl);
                            }
                        }
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                        obj3.CustomSetActive(false);
                    }
                    else if (memberInfo.RoomMemberType == 1)
                    {
                        if (!string.IsNullOrEmpty(memberInfo.MemberHeadUrl))
                        {
                            if (!CSysDynamicBlock.bFriendBlocked)
                            {
                                item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(memberInfo.MemberHeadUrl));
                            }
                            else
                            {
                                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
                                if (formScript != null)
                                {
                                    item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustPlayer_Dir + "Common_PlayerImg", formScript, true, false, false);
                                }
                            }
                        }
                        if (masterMemberInfo.swapStatus == 0)
                        {
                            obj3.CustomSetActive(true);
                        }
                        else if (masterMemberInfo.swapStatus == 1)
                        {
                            obj3.CustomSetActive(false);
                        }
                        else if (masterMemberInfo.swapStatus == 2)
                        {
                            obj3.CustomSetActive(masterMemberInfo.swapUid != memberInfo.ullUid);
                        }
                        CUIEventScript script5 = obj3.GetComponent<CUIEventScript>();
                        script5.m_onClickEventID = enUIEventID.Room_ChangePos;
                        script5.m_onClickEventParams.tag = (int) camp;
                        script5.m_onClickEventParams.tag2 = pos;
                        script5.m_onClickEventParams.tag3 = 2;
                        if (!CSysDynamicBlock.bFriendBlocked)
                        {
                            if (Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(memberInfo.ullUid, (uint) memberInfo.iLogicWorldID) == null)
                            {
                                if (transform != null)
                                {
                                    transform.gameObject.CustomSetActive(true);
                                    CUIEventScript script6 = transform.GetComponent<CUIEventScript>();
                                    if (script6 != null)
                                    {
                                        script6.m_onClickEventParams.commonUInt64Param1 = memberInfo.ullUid;
                                        script6.m_onClickEventParams.commonUInt32Param1 = (uint) memberInfo.iLogicWorldID;
                                    }
                                }
                            }
                            else if (transform != null)
                            {
                                transform.gameObject.CustomSetActive(false);
                            }
                        }
                        else if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                    else if (memberInfo.RoomMemberType == 2)
                    {
                        CUIFormScript script7 = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
                        if (script7 != null)
                        {
                            item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustPlayer_Dir + "Img_ComputerHead", script7, true, false, false);
                        }
                        obj3.CustomSetActive(false);
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                    item.transform.Find("Occupied/BtnKick").gameObject.CustomSetActive(isSelfRoomOwner && !flag3);
                }
            }
            else
            {
                item.CustomSetActive(false);
            }
        }

        public static void SetRoomData(GameObject root, RoomInfo data)
        {
            SetStartBtnStatus(root, data);
            UpdateBtnStatus(root, data);
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId);
            int num = pvpMapCommonInfo.bMaxAcntNum / 2;
            root.transform.Find("Panel_Main/MapInfo/txtMapName").gameObject.GetComponent<Text>().text = pvpMapCommonInfo.szName;
            root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num));
            GameObject item = null;
            MemberInfo memberInfo = null;
            MemberInfo masterMemberInfo = data.GetMasterMemberInfo();
            DebugHelper.Assert(masterMemberInfo != null);
            for (int i = 1; i <= 5; i++)
            {
                item = root.transform.Find(string.Format("Panel_Main/Left_Player{0}", i)).gameObject;
                memberInfo = data.GetMemberInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_1, i - 1);
                SetPlayerSlotData(item, memberInfo, masterMemberInfo, COM_PLAYERCAMP.COM_PLAYERCAMP_1, i - 1, num >= i);
            }
            for (int j = 1; j <= 5; j++)
            {
                item = root.transform.Find(string.Format("Panel_Main/Right_Player{0}", j)).gameObject;
                memberInfo = data.GetMemberInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_2, j - 1);
                SetPlayerSlotData(item, memberInfo, masterMemberInfo, COM_PLAYERCAMP.COM_PLAYERCAMP_2, j - 1, num >= j);
            }
            SetComEnable(CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId).stPickRuleInfo.bPickType != 3);
        }

        public static void SetStartBtnStatus(GameObject root, RoomInfo data)
        {
            GameObject gameObject = root.transform.Find("Panel_Main/Btn_Start").gameObject;
            bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
            gameObject.CustomSetActive(isSelfRoomOwner);
            if (isSelfRoomOwner)
            {
                Button component = gameObject.GetComponent<Button>();
                bool flag2 = (data[COM_PLAYERCAMP.COM_PLAYERCAMP_1].Count > 0) && (data[COM_PLAYERCAMP.COM_PLAYERCAMP_2].Count > 0);
                component.interactable = flag2;
            }
        }

        public static void SetSwapTimer(int totalSec, COM_PLAYERCAMP camp = 1, int pos = 0)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
            if ((form != null) && (form.gameObject != null))
            {
                GameObject obj2 = null;
                int num = 1;
                int num2 = 2;
                for (int i = num; i <= num2; i++)
                {
                    COM_PLAYERCAMP com_playercamp = (COM_PLAYERCAMP) i;
                    for (int j = 0; j < 5; j++)
                    {
                        string path = string.Format("Panel_Main/{0}{1}/Occupied/TimerSwap", (com_playercamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "Right_Player" : "Left_Player", j + 1);
                        obj2 = Utility.FindChild(form.gameObject, path);
                        if (((camp == com_playercamp) && (pos == j)) && (totalSec > 0))
                        {
                            obj2.CustomSetActive(true);
                            CUITimerScript component = obj2.GetComponent<CUITimerScript>();
                            component.SetTotalTime((float) totalSec);
                            component.m_eventIDs[1] = enUIEventID.Room_ChangePos_TimeUp;
                            component.ReStartTimer();
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public static void ShowSwapMsg(int totalSec, COM_PLAYERCAMP camp = 1, int pos = 0)
        {
            if (totalSec > 0)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(CRoomSystem.PATH_ROOM_SWAP, false, true);
                if ((form != null) && (form.gameObject != null))
                {
                    GameObject obj2 = Utility.FindChild(form.gameObject, "SwapMessageBox");
                    form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
                    if ((form != null) && (form.gameObject != null))
                    {
                        GameObject obj3 = null;
                        if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            obj3 = Utility.FindChild(form.gameObject, string.Format("Panel_Main/Left_Player{0}", pos + 1));
                        }
                        else if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                        {
                            obj3 = Utility.FindChild(form.gameObject, string.Format("Panel_Main/Right_Player{0}", pos + 1));
                        }
                        if (obj3 != null)
                        {
                            obj2.CustomSetActive(true);
                            obj2.GetComponent<RectTransform>().anchoredPosition = obj3.GetComponent<RectTransform>().anchoredPosition;
                            CUITimerScript component = obj2.GetComponent<CUITimerScript>();
                            component.SetTotalTime((float) totalSec);
                            component.m_eventIDs[0] = enUIEventID.Room_ChangePos_Box_TimerChange;
                            component.m_eventIDs[2] = enUIEventID.Room_ChangePos_Box_TimerChange;
                            component.m_eventIDs[1] = enUIEventID.Room_ChangePos_TimeUp;
                            component.m_eventParams[0].tag = pos;
                            component.m_eventParams[2].tag = pos;
                            component.ReStartTimer();
                        }
                    }
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_ROOM_SWAP);
            }
        }

        public static void UpdateBtnStatus(GameObject root, RoomInfo data)
        {
            int bMaxAcntNum = CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId).bMaxAcntNum;
            bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
            GameObject gameObject = root.transform.Find("Panel_Main/bg1/LeftRobot").gameObject;
            GameObject obj3 = root.transform.Find("Panel_Main/bg2/RightRobot").gameObject;
            gameObject.CustomSetActive(false);
            obj3.CustomSetActive(false);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (isSelfRoomOwner && (masterRoleInfo != null))
            {
                MemberInfo memberInfo = data.GetMemberInfo(masterRoleInfo.playerUllUID);
                if (memberInfo != null)
                {
                    COM_PLAYERCAMP camp = (memberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    if (data.GetFreePos(memberInfo.camp, bMaxAcntNum) >= 0)
                    {
                        if (memberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            gameObject.CustomSetActive(true);
                        }
                        else
                        {
                            obj3.CustomSetActive(true);
                        }
                    }
                    if (data.GetFreePos(camp, bMaxAcntNum) >= 0)
                    {
                        if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            gameObject.CustomSetActive(true);
                        }
                        else
                        {
                            obj3.CustomSetActive(true);
                        }
                    }
                }
            }
            CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
            component.m_onClickEventID = enUIEventID.Room_AddRobot;
            component.m_onClickEventParams.tag = 1;
            component = obj3.GetComponent<CUIEventScript>();
            component.m_onClickEventID = enUIEventID.Room_AddRobot;
            component.m_onClickEventParams.tag = 2;
        }

        public static void UpdateSwapBox(int pos)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM_SWAP);
            if ((form != null) && (form.gameObject != null))
            {
                GameObject p = Utility.FindChild(form.gameObject, "SwapMessageBox");
                CUITimerScript component = p.GetComponent<CUITimerScript>();
                string[] args = new string[] { (pos + 1).ToString(), ((int) component.GetCurrentTime()).ToString() };
                Utility.GetComponetInChild<Text>(p, "Content").text = Singleton<CTextManager>.instance.GetText("Room_Change_Pos_Tip_3", args);
            }
        }
    }
}

