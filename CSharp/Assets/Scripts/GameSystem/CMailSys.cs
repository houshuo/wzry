namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CMailSys : Singleton<CMailSys>
    {
        private bool bReadFile;
        public const int c_driveingRefreshTimeDelta = 20;
        public const int c_refreshTimeDelta = 300;
        public const string FILE_FRIEND_INVITE_ARRAY = "SGAME_FILE_FRIEND_INVITE_ARRAY";
        public const int FILE_FRIEND_INVITE_ARRAY_LEN = 30;
        private ListView<CUseable> m_accessList = new ListView<CUseable>();
        private bool m_friAccessAll;
        private List<stFriendInvite> m_friendInviteList = new List<stFriendInvite>();
        private ListView<CMail> m_friMailList = new ListView<CMail>();
        private int m_friMailUnReadNum;
        public int m_lastOpenTime;
        private CMail m_mail;
        private uint m_mailFriVersion;
        private uint m_mailSysVersion;
        private CMailView m_mailView = new CMailView();
        private ListView<CMail> m_msgMailList = new ListView<CMail>();
        private int m_msgMailUnReadNum;
        private bool m_sysAccessAll;
        private ListView<CMail> m_sysMailList = new ListView<CMail>();
        private int m_sysMailUnReadNum;
        private CSysMailView m_sysMailView = new CSysMailView();
        public static readonly string MAIL_FORM_PATH = "UGUI/Form/System/Mail/Form_Mail.prefab";
        public static readonly string MAIL_SYSTEM_FORM_PATH = "UGUI/Form/System/Mail/Form_SysMail.prefab";
        public static readonly string MAIL_WRITE_FORM_PATH = "UGUI/Form/System/Mail/Form_Mail_Write.prefab";

        private void AddFriendInviteMail(ulong uid, uint time)
        {
            this.m_friendInviteList.Add(new stFriendInvite(uid, time));
            if (this.m_friendInviteList.Count > 30)
            {
                this.m_friendInviteList.RemoveAt(0);
            }
            this.WriteFriendInviteFile();
            this.m_msgMailList = this.ConvertToMailList();
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailList);
            this.m_msgMailUnReadNum = 1;
            this.UpdateUnReadNum();
        }

        public void AddMail(COM_MAIL_TYPE mailType, CMail mail)
        {
            ListView<CMail> friMailList = null;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                friMailList = this.m_friMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                friMailList = this.m_sysMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                friMailList = this.m_msgMailList;
            }
            for (int i = 0; i < friMailList.Count; i++)
            {
                if (mail.mailIndex == friMailList[i].mailIndex)
                {
                    friMailList.RemoveAt(i);
                    break;
                }
            }
            friMailList.Add(mail);
        }

        public void Clear()
        {
            this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND, 0);
            this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_SYSTEM, 0);
            this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, 0);
            this.m_friMailUnReadNum = 0;
            this.m_sysMailUnReadNum = 0;
            this.m_msgMailUnReadNum = 0;
            this.m_mailFriVersion = 0;
            this.m_mailSysVersion = 0;
            this.m_lastOpenTime = 0;
            this.m_friAccessAll = false;
            this.m_sysAccessAll = false;
            this.m_friMailList.Clear();
            this.m_sysMailList.Clear();
            this.m_msgMailList.Clear();
            this.m_friendInviteList.Clear();
            this.bReadFile = false;
            Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.RepeatReqMailList));
        }

        private int Comparison(CMail mailA, CMail mailB)
        {
            if (mailA.mailState != mailB.mailState)
            {
                return (int) ((mailA.mailState - mailB.mailState) * ((COM_MAIL_STATE) 0x2710));
            }
            return (int) (mailB.sendTime - mailA.sendTime);
        }

        public static void ConnectVirtualList(ref ListView<CUseable> srcList1, CUseable[] srcList2)
        {
            CVirtualItem item = null;
            CVirtualItem item2 = null;
            for (int i = 0; i < srcList2.Length; i++)
            {
                bool flag = false;
                item2 = srcList2[i] as CVirtualItem;
                if (item2 != null)
                {
                    for (int j = 0; j < srcList1.Count; j++)
                    {
                        item = srcList1[j] as CVirtualItem;
                        if ((item != null) && (item.m_virtualType == item2.m_virtualType))
                        {
                            item.m_stackCount += item2.m_stackCount;
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    srcList1.Add(srcList2[i]);
                }
            }
        }

        private ListView<CMail> ConvertToMailList()
        {
            ListView<CMail> view = new ListView<CMail>();
            for (int i = 0; i < this.m_friendInviteList.Count; i++)
            {
                stFriendInvite invite = this.m_friendInviteList[i];
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(invite.uid, CFriendModel.FriendType.GameFriend);
                if (comdt_friend_info == null)
                {
                    stFriendInvite invite2 = this.m_friendInviteList[i];
                    comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(invite2.uid, CFriendModel.FriendType.SNS);
                }
                if (comdt_friend_info != null)
                {
                    CMail item = new CMail {
                        mailType = COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE
                    };
                    stFriendInvite invite3 = this.m_friendInviteList[i];
                    item.sendTime = invite3.time;
                    string[] args = new string[] { Utility.UTF8Convert(comdt_friend_info.szUserName) };
                    item.subject = Singleton<CTextManager>.instance.GetText("Mail_MsgCenter_Tips_1", args);
                    view.Add(item);
                }
            }
            return view;
        }

        public bool DeleteMail(COM_MAIL_TYPE mailType, int index)
        {
            ListView<CMail> friMailList = null;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                friMailList = this.m_friMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                friMailList = this.m_sysMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                friMailList = this.m_msgMailList;
            }
            for (int i = 0; i < friMailList.Count; i++)
            {
                if (friMailList[i].mailIndex == index)
                {
                    friMailList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public CMail GetMail(COM_MAIL_TYPE mailType, int index)
        {
            ListView<CMail> friMailList = null;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                friMailList = this.m_friMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                friMailList = this.m_sysMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                friMailList = this.m_msgMailList;
            }
            if (friMailList != null)
            {
                for (int i = 0; i < friMailList.Count; i++)
                {
                    if (friMailList[i].mailIndex == index)
                    {
                        return friMailList[i];
                    }
                }
            }
            return null;
        }

        private ListView<CMail> GetMailList(COM_MAIL_TYPE mailType)
        {
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                return this.m_friMailList;
            }
            if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                return this.m_sysMailList;
            }
            return this.m_msgMailList;
        }

        private int GetNearAccessIndex(COM_MAIL_TYPE mailType)
        {
            int num = -1;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                if (this.m_friMailList != null)
                {
                    for (int i = 0; i < this.m_friMailList.Count; i++)
                    {
                        if (!this.m_friMailList[i].isAccess && (this.m_friMailList[i].subType == 1))
                        {
                            return i;
                        }
                    }
                }
                return num;
            }
            if ((mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM) && (this.m_sysMailList != null))
            {
                for (int j = 0; j < this.m_sysMailList.Count; j++)
                {
                    if (!this.m_sysMailList[j].isAccess && (this.m_sysMailList[j].subType == 2))
                    {
                        return j;
                    }
                }
            }
            return num;
        }

        public int GetUnReadMailCount(bool bIgnoreFriend = false)
        {
            if (!bIgnoreFriend)
            {
                return ((this.m_friMailUnReadNum + this.m_sysMailUnReadNum) + this.m_msgMailUnReadNum);
            }
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().bCanRecvCoin)
            {
                return ((this.m_friMailUnReadNum + this.m_sysMailUnReadNum) + this.m_msgMailUnReadNum);
            }
            return (this.m_sysMailUnReadNum + this.m_msgMailUnReadNum);
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMailForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseMailForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendRead, new CUIEventManager.OnUIEventHandler(this.OnFriMailRead));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysRead, new CUIEventManager.OnUIEventHandler(this.OnSysMailRead));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseFriMailFrom));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSysMailFrom));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendAccessAll, new CUIEventManager.OnUIEventHandler(this.OnFriAccessAll));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysAccess, new CUIEventManager.OnUIEventHandler(this.OnSysAccess));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendAccess, new CUIEventManager.OnUIEventHandler(this.OnFriendAccess));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabFriend, new CUIEventManager.OnUIEventHandler(this.OnTabFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabSystem, new CUIEventManager.OnUIEventHandler(this.OnTabSysem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnTabMsgCenter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysDelete, new CUIEventManager.OnUIEventHandler(this.OnSystemMailDelete));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysAccessAll, new CUIEventManager.OnUIEventHandler(this.OnSysAccessAll));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_ListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnMailListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_AddToMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnAddToMsgCenter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_MsgCenterDeleteAll, new CUIEventManager.OnUIEventHandler(this.OnMsgCenterDeleteAll));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_JumpForm, new CUIEventManager.OnUIEventHandler(this.OnJumpForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_JumpUrl, new CUIEventManager.OnUIEventHandler(this.OnJumpUrl));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Form_OnClose, new CUIEventManager.OnUIEventHandler(this.OnMailFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Open_Mail_Write_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMailWriteForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Send_Guild_Mail, new CUIEventManager.OnUIEventHandler(this.OnSendGuildMail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendDelete, new CUIEventManager.OnUIEventHandler(this.OnFriendMailDelete));
        }

        public void InitLoginRsp(SCPKG_CMD_GAMELOGINRSP rsp)
        {
            Singleton<CTimerManager>.instance.AddTimer(0x493e0, -1, new CTimer.OnTimeUpHandler(this.RepeatReqMailList));
        }

        private CMail MailPop(COM_MAIL_TYPE mailType)
        {
            ListView<CMail> friMailList = null;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                friMailList = this.m_friMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                friMailList = this.m_sysMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                friMailList = this.m_msgMailList;
            }
            CMail item = null;
            for (int i = 0; i < friMailList.Count; i++)
            {
                if ((item == null) || (item.sendTime > friMailList[i].sendTime))
                {
                    item = friMailList[i];
                }
            }
            if (item != null)
            {
                friMailList.Remove(item);
            }
            return item;
        }

        private void OnAddToMsgCenter(CUIEvent uiEvent)
        {
            COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(uiEvent.m_eventParams.tagStr, CFriendModel.FriendType.GameFriend);
            if (comdt_friend_info == null)
            {
                comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(uiEvent.m_eventParams.tagStr, CFriendModel.FriendType.SNS);
            }
            if (comdt_friend_info != null)
            {
                this.AddFriendInviteMail(comdt_friend_info.stUin.ullUid, (uint) CRoleInfo.GetCurrentUTCTime());
            }
        }

        private void OnCloseFriMailFrom(CUIEvent uiEvent)
        {
        }

        private void OnCloseMailForm(CUIEvent uiEvent)
        {
            this.m_friAccessAll = false;
            this.m_sysAccessAll = false;
            this.m_mailView.Close();
        }

        private void OnCloseSysMailFrom(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(MAIL_SYSTEM_FORM_PATH);
        }

        private void OnFriAccessAll(CUIEvent uiEvent)
        {
            this.m_accessList.Clear();
            int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_FRIEND);
            if (nearAccessIndex >= 0)
            {
                this.m_friAccessAll = true;
                this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[nearAccessIndex].mailIndex, 0);
            }
            else
            {
                this.m_friAccessAll = false;
            }
        }

        private void OnFriendAccess(CUIEvent uiEvent)
        {
            int mailIndex = this.m_mail.mailIndex;
            this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, mailIndex, 0);
        }

        private void OnFriendMailDelete(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(MAIL_SYSTEM_FORM_PATH);
            if (((this.m_friMailList != null) && (this.m_friMailList.Count > 0)) && (Singleton<CUIManager>.GetInstance().GetForm(MAIL_FORM_PATH) != null))
            {
                int friendMailListSelectedIndex = this.m_mailView.GetFriendMailListSelectedIndex();
                if ((friendMailListSelectedIndex >= 0) && (friendMailListSelectedIndex < this.m_friMailList.Count))
                {
                    this.ReqDeleteMail(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[friendMailListSelectedIndex].mailIndex);
                }
            }
        }

        private void OnFriMailRead(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            this.OnMailRead(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[srcWidgetIndexInBelongedList].mailIndex);
        }

        private void OnJumpForm(CUIEvent uiEvent)
        {
            CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) uiEvent.m_eventParams.tag);
        }

        private void OnJumpUrl(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl(uiEvent.m_eventParams.tagStr, true);
        }

        public void OnMailDeleteRes(CSPkg msg)
        {
            COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE) msg.stPkgData.stMailOptRes.stOptInfo.stDelMail.bMailType;
            this.DeleteMail(bMailType, msg.stPkgData.stMailOptRes.stOptInfo.stDelMail.iMailIndex);
            ListView<CMail> mailList = null;
            switch (bMailType)
            {
                case COM_MAIL_TYPE.COM_MAIL_FRIEND:
                    mailList = this.m_friMailList;
                    break;

                case COM_MAIL_TYPE.COM_MAIL_SYSTEM:
                    mailList = this.m_sysMailList;
                    break;
            }
            this.UpdateUnReadNum();
            this.m_mailView.UpdateMailList(bMailType, mailList);
        }

        private void OnMailFormClose(CUIEvent uiEvent)
        {
            this.m_mailView.OnClose();
        }

        public void OnMailGetAccess(CSPkg msg)
        {
            switch (msg.stPkgData.stMailOptRes.stOptInfo.stGetAccess.bResult)
            {
                case 1:
                {
                    CSDT_MAILOPTRES_GETACCESS stGetAccess = msg.stPkgData.stMailOptRes.stOptInfo.stGetAccess;
                    COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE) stGetAccess.bMailType;
                    int iMailIndex = stGetAccess.iMailIndex;
                    CMail mail = this.GetMail(bMailType, iMailIndex);
                    if (mail == null)
                    {
                        break;
                    }
                    if (bMailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
                    {
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                        if (masterRoleInfo != null)
                        {
                            masterRoleInfo.getFriendCoinCnt++;
                        }
                    }
                    mail.isAccess = true;
                    CUseable[] items = LinqS.ToArray<CUseable>(StAccessToUseable(stGetAccess.astAccess, stGetAccess.astAccessFrom, stGetAccess.bAccessCnt));
                    this.OnCloseSysMailFrom(null);
                    enUIEventID none = enUIEventID.None;
                    if (!this.m_friAccessAll)
                    {
                        if (this.m_sysAccessAll)
                        {
                            none = enUIEventID.Mail_SysAccessAll;
                        }
                        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                        Singleton<CUIManager>.instance.OpenAwardTip(items, null, false, none, true, false, "Form_Award");
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i] == null)
                            {
                                continue;
                            }
                            if (items[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                            {
                                CHeroItem item = items[i] as CHeroItem;
                                if ((item == null) || (item is CExpHeroItem))
                                {
                                    goto Label_0247;
                                }
                                CUICommonSystem.ShowNewHeroOrSkin(item.m_baseID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
                                break;
                            }
                            if (items[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                            {
                                CHeroSkin skin = items[i] as CHeroSkin;
                                if ((skin != null) && !(skin is CExpHeroSkin))
                                {
                                    CUICommonSystem.ShowNewHeroOrSkin(skin.m_heroId, skin.m_skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                                    break;
                                }
                            }
                        Label_0247:
                            if (items[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                            {
                                if (items[i].ExtraFromType == 1)
                                {
                                    CUICommonSystem.ShowNewHeroOrSkin((uint) items[i].ExtraFromData, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint) items[i].m_stackCount, 0);
                                }
                                else if (items[i].ExtraFromType == 2)
                                {
                                    int extraFromData = items[i].ExtraFromData;
                                    CUICommonSystem.ShowNewHeroOrSkin(0, (uint) extraFromData, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint) items[i].m_stackCount, 0);
                                }
                            }
                        }
                        break;
                    }
                    ConnectVirtualList(ref this.m_accessList, items);
                    int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_FRIEND);
                    if (nearAccessIndex >= 0)
                    {
                        this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[nearAccessIndex].mailIndex, 0);
                    }
                    else
                    {
                        Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
                        this.m_accessList.Clear();
                        this.m_friAccessAll = false;
                        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                    }
                    this.UpdateUnReadNum();
                    return;
                }
                case 4:
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_PackageClean"), false, 1.5f, null, new object[0]);
                    this.m_sysAccessAll = false;
                    break;

                case 5:
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
                    this.m_sysAccessAll = false;
                    break;

                case 8:
                    if (!this.m_friAccessAll)
                    {
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
                        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                        break;
                    }
                    if (this.m_accessList.Count > 0)
                    {
                        Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
                    }
                    this.m_accessList.Clear();
                    this.m_friAccessAll = false;
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
                    break;
            }
            this.UpdateUnReadNum();
        }

        public void OnMailGetListRes(CSPkg msg)
        {
            ListView<CMail> mailList = null;
            COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE) msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bMailType;
            int num = 0;
            switch (bMailType)
            {
                case COM_MAIL_TYPE.COM_MAIL_FRIEND:
                    mailList = this.m_friMailList;
                    num = 30;
                    break;

                case COM_MAIL_TYPE.COM_MAIL_SYSTEM:
                    mailList = this.m_sysMailList;
                    num = 100;
                    break;
            }
            if (msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bIsDiff == 0)
            {
                mailList.Clear();
            }
            for (int i = 0; i < msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bCnt; i++)
            {
                this.AddMail(bMailType, new CMail(bMailType, ref msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.astMailInfo[i]));
            }
            while (mailList.Count > num)
            {
                this.MailPop(bMailType);
            }
            switch (bMailType)
            {
                case COM_MAIL_TYPE.COM_MAIL_FRIEND:
                    this.m_mailFriVersion = msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.dwVersion;
                    break;

                case COM_MAIL_TYPE.COM_MAIL_SYSTEM:
                    this.m_mailSysVersion = msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.dwVersion;
                    break;
            }
            this.SortMailList(bMailType);
            this.UpdateUnReadNum();
            this.m_mailView.UpdateMailList(bMailType, mailList);
        }

        private void OnMailListElementEnable(CUIEvent uiEvent)
        {
            COM_MAIL_TYPE curMailType = this.m_mailView.CurMailType;
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            ListView<CMail> mailList = this.GetMailList(curMailType);
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < mailList.Count))
            {
                this.m_mailView.UpdateListElenment(uiEvent.m_srcWidget, mailList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnMailRead(COM_MAIL_TYPE mailType, int index)
        {
            CMail mail = this.GetMail(mailType, index);
            object[] inParameters = new object[] { mailType, index };
            DebugHelper.Assert(mail != null, "mail cannot be find. mailType[{0}] , index[{1}]", inParameters);
            if (mail != null)
            {
                if (mail.isReceive)
                {
                    this.OnOpenSysMailForm(mail, mailType);
                }
                else
                {
                    this.ReqReadMail(mailType, index);
                    this.OnOpenSysMailForm(mail, mailType);
                }
            }
        }

        public void OnMailReadMailRes(CSPkg msg)
        {
            COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE) msg.stPkgData.stMailOptRes.stOptInfo.stReadMail.bMailType;
            int iMailIndex = msg.stPkgData.stMailOptRes.stOptInfo.stReadMail.iMailIndex;
            CMail mail = this.GetMail(bMailType, iMailIndex);
            object[] inParameters = new object[] { bMailType, iMailIndex };
            DebugHelper.Assert(mail != null, "mail cannot be find. mailType[{0}] , index[{1}]", inParameters);
            if (mail != null)
            {
                mail.Read(msg.stPkgData.stMailOptRes.stOptInfo.stReadMail);
                this.m_sysMailView.Mail = mail;
                this.m_mail = mail;
                this.SortMailList(bMailType);
                switch (bMailType)
                {
                    case COM_MAIL_TYPE.COM_MAIL_FRIEND:
                        this.m_mailView.UpdateMailList(bMailType, this.m_friMailList);
                        break;

                    case COM_MAIL_TYPE.COM_MAIL_SYSTEM:
                        this.m_mailView.UpdateMailList(bMailType, this.m_sysMailList);
                        break;
                }
                this.UpdateUnReadNum();
            }
        }

        [MessageHandler(0x579)]
        public static void OnMailRes(CSPkg msg)
        {
            switch (msg.stPkgData.stMailOptRes.bOptType)
            {
                case 1:
                    Singleton<CMailSys>.instance.OnMailGetListRes(msg);
                    break;

                case 2:
                    Singleton<CMailSys>.instance.OnMailSendMailRes(msg);
                    break;

                case 3:
                    Singleton<CMailSys>.instance.OnMailReadMailRes(msg);
                    break;

                case 4:
                    Singleton<CMailSys>.instance.OnMailDeleteRes(msg);
                    break;

                case 5:
                    Singleton<CMailSys>.instance.OnMailGetAccess(msg);
                    break;

                case 6:
                    Singleton<CMailSys>.instance.OnMailUnReadRes(msg);
                    break;
            }
        }

        public void OnMailSendMailRes(CSPkg msg)
        {
            MAIL_OPT_MAILLISTTYPE mail_opt_maillisttype;
            COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE) msg.stPkgData.stMailOptRes.stOptInfo.stSendMail.bMailType;
            switch (msg.stPkgData.stMailOptRes.stOptInfo.stSendMail.bResult)
            {
                case 1:
                case 2:
                case 7:
                    return;

                case 3:
                    mail_opt_maillisttype = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTFRIEND;
                    switch (bMailType)
                    {
                        case COM_MAIL_TYPE.COM_MAIL_FRIEND:
                            mail_opt_maillisttype = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTFRIEND;
                            goto Label_0088;

                        case COM_MAIL_TYPE.COM_MAIL_SYSTEM:
                            mail_opt_maillisttype = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTSYS;
                            goto Label_0088;
                    }
                    break;

                case 4:
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
                    this.m_sysAccessAll = false;
                    return;

                case 5:
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
                    this.m_sysAccessAll = false;
                    return;

                case 6:
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_ListGeting"), false, 1.5f, null, new object[0]);
                    return;

                case 8:
                    if (this.m_friAccessAll)
                    {
                        Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
                        this.m_accessList.Clear();
                        this.m_friAccessAll = false;
                        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
                    }
                    return;

                default:
                    return;
            }
        Label_0088:
            this.ReqMailList(bMailType, mail_opt_maillisttype, false, 0);
        }

        public void OnMailUnReadRes(CSPkg msg)
        {
            this.ReqMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND, MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTFRIEND, false, 0);
            this.ReqMailList(COM_MAIL_TYPE.COM_MAIL_SYSTEM, MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTSYS, false, 0);
        }

        private void OnMsgCenterDeleteAll(CUIEvent uiEvent)
        {
            this.m_friendInviteList.Clear();
            this.WriteFriendInviteFile();
            this.m_msgMailList = this.ConvertToMailList();
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailList);
            this.m_msgMailUnReadNum = 0;
            this.UpdateUnReadNum();
        }

        private void OnOpenMailForm(CUIEvent uiEvent)
        {
            this.m_friAccessAll = false;
            this.m_sysAccessAll = false;
            this.m_mailView.Open(COM_MAIL_TYPE.COM_MAIL_FRIEND);
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList);
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList);
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailList);
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            if ((currentUTCTime - this.m_lastOpenTime) > 20)
            {
                this.ReqMailList(COM_MAIL_TYPE.COM_MAIL_SYSTEM, MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTREFRESH, true, 0);
                this.m_lastOpenTime = currentUTCTime;
            }
            if (!this.bReadFile)
            {
                this.ReadFriendInviteFile();
                this.bReadFile = true;
            }
        }

        private void OnOpenMailWriteForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(MAIL_WRITE_FORM_PATH, false, true);
            if (script != null)
            {
                Text component = script.GetWidget(2).GetComponent<Text>();
                uint sendGuildMailCnt = CGuildHelper.GetSendGuildMailCnt();
                uint sendGuildMailLimit = CGuildHelper.GetSendGuildMailLimit();
                string[] args = new string[] { sendGuildMailCnt.ToString(), sendGuildMailLimit.ToString() };
                component.text = Singleton<CTextManager>.GetInstance().GetText("Mail_Today_Send_Num", args);
            }
        }

        private void OnOpenSysMailForm(CMail mail, COM_MAIL_TYPE mailType)
        {
            this.m_friAccessAll = false;
            this.m_sysAccessAll = false;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(MAIL_SYSTEM_FORM_PATH, false, true);
            GameObject widget = script.GetWidget(0);
            GameObject obj3 = script.GetWidget(1);
            if ((mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND) && (mail.subType == 3))
            {
                script.GetWidget(2).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mail_Guild_Mail");
                widget.CustomSetActive(false);
                obj3.CustomSetActive(true);
            }
            else
            {
                obj3.CustomSetActive(false);
            }
            this.m_sysMailView.Form = script;
            this.m_sysMailView.mailType = mailType;
            this.m_sysMailView.Mail = mail;
            this.m_mail = mail;
        }

        private void OnSendGuildMail(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(MAIL_WRITE_FORM_PATH);
            if (form != null)
            {
                string str = CUIUtility.RemoveEmoji(form.GetWidget(1).GetComponent<InputField>().text).Trim();
                if (string.IsNullOrEmpty(str))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Mail_Content_Cannot_Be_Empty", true, 1.5f, null, new object[0]);
                }
                else
                {
                    string text = CUIUtility.RemoveEmoji(form.GetWidget(0).GetComponent<InputField>().text).Trim();
                    if (string.IsNullOrEmpty(text) && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
                    {
                        string[] args = new string[] { CGuildHelper.GetGuildName() };
                        text = Singleton<CTextManager>.GetInstance().GetText("Mail_Default_Guild_Mail_Title", args);
                    }
                    Singleton<CGuildInfoController>.GetInstance().ReqSendGuildMail(text, str);
                }
            }
        }

        private void OnSysAccess(CUIEvent uiEvent)
        {
            int mailIndex = this.m_mail.mailIndex;
            this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_SYSTEM, mailIndex, 0);
        }

        private void OnSysAccessAll(CUIEvent uiEvent)
        {
            int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_SYSTEM);
            if (nearAccessIndex >= 0)
            {
                this.m_sysAccessAll = true;
                this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList[nearAccessIndex].mailIndex, 0);
            }
            else
            {
                this.m_sysAccessAll = false;
            }
        }

        private void OnSysMailRead(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_sysMailList.Count))
            {
                this.OnMailRead(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList[srcWidgetIndexInBelongedList].mailIndex);
            }
        }

        private void OnSystemMailDelete(CUIEvent uiEvent)
        {
            if ((this.m_sysMailList != null) && (this.m_sysMailList.Count > 0))
            {
                int canBeDeleted = this.m_sysMailList[0].CanBeDeleted;
                if (canBeDeleted == 0)
                {
                    this.ReqDeleteMail(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList[0].mailIndex);
                }
                else
                {
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText(canBeDeleted.ToString()), false, 1.5f, null, new object[0]);
                }
            }
        }

        private void OnTabFriend(CUIEvent uiEvent)
        {
            this.m_mailView.CurMailType = COM_MAIL_TYPE.COM_MAIL_FRIEND;
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList);
            this.m_mailView.SetActiveTab(0);
        }

        private void OnTabMsgCenter(CUIEvent uiEvent)
        {
            this.m_mailView.CurMailType = COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE;
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailList);
            this.m_mailView.SetActiveTab(2);
            this.m_msgMailUnReadNum = 0;
            this.UpdateUnReadNum();
        }

        private void OnTabSysem(CUIEvent uiEvent)
        {
            this.m_mailView.CurMailType = COM_MAIL_TYPE.COM_MAIL_SYSTEM;
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList);
            this.m_mailView.SetActiveTab(1);
        }

        private void ReadFriendInviteFile()
        {
            byte[] bytes = Utility.ReadFile(CFileManager.GetCachePath(string.Format("{0}_{1}", "SGAME_FILE_FRIEND_INVITE_ARRAY", Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)));
            List<stFriendInvite> list = null;
            if (bytes != null)
            {
                list = Utility.BytesToObject(bytes) as List<stFriendInvite>;
            }
            if (list != null)
            {
                this.m_friendInviteList = list;
            }
            this.m_msgMailList = this.ConvertToMailList();
            this.m_mailView.UpdateMailList(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailList);
        }

        private void RepeatReqMailList(int timerSequence)
        {
            if (!Singleton<BattleLogic>.instance.isRuning && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null))
            {
                this.ReqMailList(COM_MAIL_TYPE.COM_MAIL_SYSTEM, MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTREFRESH, false, 0);
            }
        }

        public void ReqDeleteMail(COM_MAIL_TYPE mailType, int index)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x578);
            msg.stPkgData.stMailOptReq.bOptType = 4;
            msg.stPkgData.stMailOptReq.stOptInfo.stDelMail = new CSDT_MAILOPTREQ_DELMAIL();
            msg.stPkgData.stMailOptReq.stOptInfo.stDelMail.bMailType = (byte) mailType;
            msg.stPkgData.stMailOptReq.stOptInfo.stDelMail.iMailIndex = index;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void ReqMailGetAccess(COM_MAIL_TYPE mailType, int index, int getAll = 0)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x578);
            msg.stPkgData.stMailOptReq.bOptType = 5;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetAccess = new CSDT_MAILOPTREQ_GETACCESS();
            msg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.bMailType = (byte) mailType;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.iMailIndex = index;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.bGetAll = (byte) getAll;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void ReqMailList(COM_MAIL_TYPE mailType, MAIL_OPT_MAILLISTTYPE optType, bool isDiff = false, int startPos = 0)
        {
            uint mailFriVersion = 0;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                mailFriVersion = this.m_mailFriVersion;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                mailFriVersion = this.m_mailSysVersion;
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x578);
            msg.stPkgData.stMailOptReq.bOptType = 1;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetMailList = new CSDT_MAILOPTREQ_GETMAILLIST();
            msg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bReqType = (byte) optType;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.dwVersion = mailFriVersion;
            msg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bIsDiff = !isDiff ? ((byte) 0) : ((byte) 1);
            msg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bStartPos = (byte) startPos;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void ReqReadMail(COM_MAIL_TYPE mailType, int index)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x578);
            msg.stPkgData.stMailOptReq.bOptType = 3;
            msg.stPkgData.stMailOptReq.stOptInfo.stReadMail = new CSDT_MAILOPTREQ_READMAIL();
            msg.stPkgData.stMailOptReq.stOptInfo.stReadMail.bMailType = (byte) mailType;
            msg.stPkgData.stMailOptReq.stOptInfo.stReadMail.iMailIndex = index;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void ReqSendMail(COM_MAIL_TYPE mailType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x578);
            msg.stPkgData.stMailOptReq.bOptType = 2;
            msg.stPkgData.stMailOptReq.stOptInfo.stSendMail = new CSDT_MAILOPTREQ_SENDMAIL();
            msg.stPkgData.stMailOptReq.stOptInfo.stSendMail.bMailType = (byte) mailType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SortMailList(COM_MAIL_TYPE mailType)
        {
            ListView<CMail> friMailList = null;
            if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                friMailList = this.m_friMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                friMailList = this.m_sysMailList;
            }
            else if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                friMailList = this.m_msgMailList;
            }
            if (friMailList != null)
            {
                friMailList.Sort(new Comparison<CMail>(this.Comparison));
            }
        }

        public static ListView<CUseable> StAccessToUseable(COMDT_MAILACCESS[] stAccess, CSDT_MAILACCESS_FROM[] stAccessFrom, int count)
        {
            ListView<CUseable> view = new ListView<CUseable>();
            for (int i = 0; i < count; i++)
            {
                COM_MAILACCESS_TYPE bAccessType = (COM_MAILACCESS_TYPE) stAccess[i].bAccessType;
                CUseable item = null;
                switch (bAccessType)
                {
                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_HEART:
                        item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enHeart, (int) stAccess[i].stAccessInfo.stHeart.dwHeart);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_RONGYU:
                        item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int) stAccess[i].stAccessInfo.stRongYu.dwRongYuPoint);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_MONEY:
                        if (stAccess[i].stAccessInfo.stMoney.bType == 1)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enNoUsed, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 7)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 3)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enArenaCoin, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 4)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enBurningCoin, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 5)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGuildConstruct, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 6)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        else if (stAccess[i].stAccessInfo.stMoney.bType == 2)
                        {
                            item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int) stAccess[i].stAccessInfo.stMoney.dwMoney);
                        }
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_PROP:
                        item = CUseableManager.CreateUseable((COM_ITEM_TYPE) stAccess[i].stAccessInfo.stProp.wPropType, 0L, stAccess[i].stAccessInfo.stProp.dwPropID, stAccess[i].stAccessInfo.stProp.iPropNum, 0);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_EXP:
                        item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, (int) stAccess[i].stAccessInfo.stExp.dwExp);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_HERO:
                        item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0L, stAccess[i].stAccessInfo.stHero.dwHeroID, 1, 0);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_PIFU:
                        item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0L, stAccess[i].stAccessInfo.stPiFu.dwSkinID, 1, 0);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_EXPHERO:
                        item = CUseableManager.CreateExpUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0L, stAccess[i].stAccessInfo.stExpHero.dwHeroID, stAccess[i].stAccessInfo.stExpHero.dwExpDays, 1, 0);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_EXPSKIN:
                        item = CUseableManager.CreateExpUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0L, stAccess[i].stAccessInfo.stExpSkin.dwSkinID, stAccess[i].stAccessInfo.stExpSkin.dwExpDays, 1, 0);
                        break;

                    case COM_MAILACCESS_TYPE.COM_MAILACCESS_HEADIMG:
                        item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, 0L, stAccess[i].stAccessInfo.stHeadImg.dwHeadImgID, 0, 0);
                        break;
                }
                if (item != null)
                {
                    if ((stAccessFrom != null) && (i < stAccessFrom.Length))
                    {
                        CSDT_MAILACCESS_FROM csdt_mailaccess_from = stAccessFrom[i];
                        if (csdt_mailaccess_from != null)
                        {
                            if (csdt_mailaccess_from.bFromType == 1)
                            {
                                item.ExtraFromType = csdt_mailaccess_from.bFromType;
                                item.ExtraFromData = (int) csdt_mailaccess_from.stFromInfo.stHeroInfo.dwHeroID;
                            }
                            else if (csdt_mailaccess_from.bFromType == 2)
                            {
                                item.ExtraFromType = csdt_mailaccess_from.bFromType;
                                item.ExtraFromData = (int) csdt_mailaccess_from.stFromInfo.stSkinInfo.dwSkinID;
                            }
                        }
                    }
                    view.Add(item);
                }
            }
            return view;
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMailForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseMailForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendRead, new CUIEventManager.OnUIEventHandler(this.OnFriMailRead));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysRead, new CUIEventManager.OnUIEventHandler(this.OnSysMailRead));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseFriMailFrom));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSysMailFrom));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendAccessAll, new CUIEventManager.OnUIEventHandler(this.OnFriAccessAll));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysAccess, new CUIEventManager.OnUIEventHandler(this.OnSysAccess));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendAccess, new CUIEventManager.OnUIEventHandler(this.OnFriendAccess));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabFriend, new CUIEventManager.OnUIEventHandler(this.OnTabFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabSystem, new CUIEventManager.OnUIEventHandler(this.OnTabSysem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnTabMsgCenter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysDelete, new CUIEventManager.OnUIEventHandler(this.OnSystemMailDelete));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysAccessAll, new CUIEventManager.OnUIEventHandler(this.OnSysAccessAll));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_ListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnMailListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_AddToMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnAddToMsgCenter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_MsgCenterDeleteAll, new CUIEventManager.OnUIEventHandler(this.OnMsgCenterDeleteAll));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_JumpForm, new CUIEventManager.OnUIEventHandler(this.OnJumpForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_JumpUrl, new CUIEventManager.OnUIEventHandler(this.OnJumpUrl));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_Form_OnClose, new CUIEventManager.OnUIEventHandler(this.OnMailFormClose));
            base.UnInit();
        }

        public void UpdateUnReadNum()
        {
            this.m_friMailUnReadNum = 0;
            if (this.m_friMailList != null)
            {
                for (int i = 0; i < this.m_friMailList.Count; i++)
                {
                    if (this.m_friMailList[i].mailState == COM_MAIL_STATE.COM_MAIL_UNREAD)
                    {
                        this.m_friMailUnReadNum++;
                    }
                }
            }
            this.m_sysMailUnReadNum = 0;
            if (this.m_sysMailList != null)
            {
                for (int j = 0; j < this.m_sysMailList.Count; j++)
                {
                    if (this.m_sysMailList[j].mailState == COM_MAIL_STATE.COM_MAIL_UNREAD)
                    {
                        this.m_sysMailUnReadNum++;
                    }
                }
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if ((masterRoleInfo != null) && !masterRoleInfo.bCanRecvCoin)
            {
                this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND, 0);
            }
            else
            {
                this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailUnReadNum);
            }
            this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailUnReadNum);
            this.m_mailView.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgMailUnReadNum);
            Singleton<EventRouter>.instance.BroadCastEvent("MailUnReadNumUpdate");
        }

        private void WriteFriendInviteFile()
        {
            string cachePath = CFileManager.GetCachePath(string.Format("{0}_{1}", "SGAME_FILE_FRIEND_INVITE_ARRAY", Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID));
            byte[] bytes = Utility.ObjectToBytes(this.m_friendInviteList);
            Utility.WriteFile(cachePath, bytes);
        }

        public enum enMailSysFormWidget
        {
            GetAccessBtn,
            DeleteBtn,
            TitleText
        }

        public enum enMailWriteFormWidget
        {
            Mail_Title_Input,
            Mail_Content_Input,
            Mail_Send_Num_Text
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct stFriendInvite
        {
            public ulong uid;
            public uint time;
            public stFriendInvite(ulong uid, uint time)
            {
                this.uid = uid;
                this.time = time;
            }
        }
    }
}

