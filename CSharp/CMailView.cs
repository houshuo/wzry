using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CMailView
{
    private GameObject m_allDeleteMsgCenterButton;
    private GameObject m_allReceiveFriButton;
    private GameObject m_allReceiveSysButton;
    private CUIFormScript m_CUIForm;
    private CUIListScript m_CUIListScriptTab;
    private COM_MAIL_TYPE m_curMailtype = COM_MAIL_TYPE.COM_MAIL_SYSTEM;
    private int m_friUnReadNum;
    private int m_msgUnReadNum;
    private GameObject m_panelFri;
    private GameObject m_panelMsg;
    private GameObject m_panelSys;
    private GameObject m_SysDeleteBtn;
    private int m_sysUnReadNum;

    public void Close()
    {
        Singleton<CUIManager>.GetInstance().CloseForm(CMailSys.MAIL_FORM_PATH);
        this.OnClose();
    }

    public int GetFriendMailListSelectedIndex()
    {
        if (this.m_panelFri != null)
        {
            CUIListScript component = this.m_panelFri.transform.Find("List").GetComponent<CUIListScript>();
            if (component != null)
            {
                return component.GetSelectedIndex();
            }
        }
        return -1;
    }

    public void OnClose()
    {
        this.m_CUIForm = null;
        this.m_CUIListScriptTab = null;
        this.m_panelFri = null;
        this.m_panelSys = null;
        this.m_panelMsg = null;
        this.m_SysDeleteBtn = null;
        this.m_allReceiveSysButton = null;
        this.m_allReceiveFriButton = null;
        this.m_allDeleteMsgCenterButton = null;
    }

    public void Open(COM_MAIL_TYPE mailType)
    {
        this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_FORM_PATH, false, true);
        if (this.m_CUIForm != null)
        {
            this.m_CUIListScriptTab = this.m_CUIForm.transform.FindChild("TopCommon/Panel_Menu/ListMenu").GetComponent<CUIListScript>();
            this.m_CUIListScriptTab.SetElementAmount(3);
            this.m_CUIListScriptTab.GetElemenet(0).transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mail_Friend");
            this.m_CUIListScriptTab.GetElemenet(1).transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mail_System");
            this.m_CUIListScriptTab.GetElemenet(2).transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mail_MsgCenter");
            this.m_CUIListScriptTab.GetElemenet(0).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabFriend);
            this.m_CUIListScriptTab.GetElemenet(1).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabSystem);
            this.m_CUIListScriptTab.GetElemenet(2).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabMsgCenter);
            this.m_panelFri = this.m_CUIForm.transform.FindChild("PanelFriMail").gameObject;
            this.m_panelSys = this.m_CUIForm.transform.FindChild("PanelSysMail").gameObject;
            this.m_panelMsg = this.m_CUIForm.transform.FindChild("PanelMsgMail").gameObject;
            this.m_SysDeleteBtn = this.m_panelSys.transform.FindChild("ButtonGrid/DeleteButton").gameObject;
            this.m_allReceiveSysButton = this.m_panelSys.transform.FindChild("ButtonGrid/AllReceiveButton").gameObject;
            this.m_allReceiveFriButton = this.m_panelFri.transform.FindChild("AllReceiveButton").gameObject;
            this.m_allDeleteMsgCenterButton = this.m_panelMsg.transform.FindChild("AllDeleteButton").gameObject;
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friUnReadNum);
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysUnReadNum);
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgUnReadNum);
            this.CurMailType = mailType;
        }
    }

    public void SetActiveTab(int index)
    {
        if (index == 0)
        {
            this.m_panelFri.CustomSetActive(true);
            this.m_panelSys.CustomSetActive(false);
            this.m_panelMsg.CustomSetActive(false);
        }
        else if (index == 1)
        {
            this.m_panelFri.CustomSetActive(false);
            this.m_panelSys.CustomSetActive(true);
            this.m_panelMsg.CustomSetActive(false);
        }
        else if (index == 2)
        {
            this.m_panelFri.CustomSetActive(false);
            this.m_panelSys.CustomSetActive(false);
            this.m_panelMsg.CustomSetActive(true);
        }
    }

    public void SetUnReadNum(COM_MAIL_TYPE mailtype, int unReadNum)
    {
        if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
        {
            this.m_friUnReadNum = unReadNum;
        }
        else if (mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
        {
            this.m_sysUnReadNum = unReadNum;
        }
        else if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
        {
            this.m_msgUnReadNum = unReadNum;
        }
        if (this.m_CUIListScriptTab != null)
        {
            if ((mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND) && (this.m_CUIListScriptTab.GetElemenet(0) != null))
            {
                if (unReadNum > 9)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject, enRedDotPos.enTopRight, 0);
                }
                else if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject, enRedDotPos.enTopRight, unReadNum);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject);
                }
            }
            else if ((mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM) && (this.m_CUIListScriptTab.GetElemenet(1) != null))
            {
                if (unReadNum > 9)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject, enRedDotPos.enTopRight, 0);
                }
                else if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject, enRedDotPos.enTopRight, unReadNum);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject);
                }
            }
            else if ((mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE) && (this.m_CUIListScriptTab.GetElemenet(2) != null))
            {
                if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(2).gameObject, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(2).gameObject);
                }
            }
        }
    }

    public void UpdateListElenment(GameObject element, CMail mail)
    {
        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
        Text component = element.transform.FindChild("Title").GetComponent<Text>();
        Text text2 = element.transform.FindChild("MailTime").GetComponent<Text>();
        GameObject gameObject = element.transform.FindChild("New").gameObject;
        GameObject obj3 = element.transform.FindChild("ReadMailIcon").gameObject;
        GameObject obj4 = element.transform.FindChild("UnReadMailIcon").gameObject;
        GameObject obj5 = element.transform.FindChild("CoinImg").gameObject;
        Text text3 = element.transform.FindChild("From").GetComponent<Text>();
        CUIHttpImageScript script = element.transform.FindChild("HeadBg/imgHead").GetComponent<CUIHttpImageScript>();
        component.text = mail.subject;
        text2.text = Utility.GetTimeBeforString((long) mail.sendTime, (long) currentUTCTime);
        bool bActive = mail.mailState == COM_MAIL_STATE.COM_MAIL_UNREAD;
        gameObject.CustomSetActive(bActive);
        if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
        {
            obj3.CustomSetActive(!bActive);
            obj4.CustomSetActive(bActive);
            text3.text = string.Empty;
            script.gameObject.CustomSetActive(false);
            obj5.SetActive(false);
        }
        else if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
        {
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            text3.text = mail.from;
            script.gameObject.CustomSetActive(true);
            if (mail.subType == 3)
            {
                obj5.CustomSetActive(false);
                script.SetImageSprite(CGuildHelper.GetGuildHeadPath(), this.m_CUIForm);
            }
            else
            {
                obj5.CustomSetActive(true);
                if (!CSysDynamicBlock.bFriendBlocked)
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        string url = Utility.UTF8Convert(comdt_friend_info.szHeadUrl);
                        script.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
                    }
                }
            }
        }
        else if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
        {
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            text3.text = string.Empty;
            script.gameObject.CustomSetActive(true);
            obj5.SetActive(false);
            if (!CSysDynamicBlock.bFriendBlocked)
            {
                COMDT_FRIEND_INFO comdt_friend_info2 = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.GameFriend);
                if (comdt_friend_info2 == null)
                {
                    comdt_friend_info2 = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.SNS);
                }
                if (comdt_friend_info2 != null)
                {
                    string str2 = Utility.UTF8Convert(comdt_friend_info2.szHeadUrl);
                    script.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(str2));
                }
            }
        }
    }

    public void UpdateMailList(COM_MAIL_TYPE mailtype, ListView<CMail> mailList)
    {
        if ((this.m_CUIForm != null) && (mailList != null))
        {
            CUIListElementScript elemenet = null;
            CUIListScript component = null;
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            int num2 = -1;
            if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                component = this.m_CUIForm.transform.FindChild("PanelFriMail/List").GetComponent<CUIListScript>();
                component.SetElementAmount(mailList.Count);
                for (int i = 0; i < mailList.Count; i++)
                {
                    elemenet = component.GetElemenet(i);
                    if ((elemenet != null) && (elemenet.gameObject != null))
                    {
                        this.UpdateListElenment(elemenet.gameObject, mailList[i]);
                    }
                    if ((num2 == -1) && (mailList[i].subType == 1))
                    {
                        num2 = i;
                    }
                }
                this.m_allReceiveFriButton.CustomSetActive(num2 >= 0);
            }
            else if (mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                this.m_CUIForm.transform.FindChild("PanelSysMail/List").GetComponent<CUIListScript>().SetElementAmount(mailList.Count);
                for (int j = 0; j < mailList.Count; j++)
                {
                    if ((elemenet != null) && (elemenet.gameObject != null))
                    {
                        this.UpdateListElenment(elemenet.gameObject, mailList[j]);
                    }
                    if ((num2 == -1) && (mailList[j].subType == 2))
                    {
                        num2 = j;
                    }
                }
                this.m_allReceiveSysButton.CustomSetActive(num2 >= 0);
                this.m_SysDeleteBtn.CustomSetActive(mailList.Count > 0);
            }
            else if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                this.m_CUIForm.transform.FindChild("PanelMsgMail/List").GetComponent<CUIListScript>().SetElementAmount(mailList.Count);
                for (int k = 0; k < mailList.Count; k++)
                {
                    if ((elemenet != null) && (elemenet.gameObject != null))
                    {
                        this.UpdateListElenment(elemenet.gameObject, mailList[k]);
                    }
                }
                this.m_allDeleteMsgCenterButton.CustomSetActive(mailList.Count > 0);
            }
        }
    }

    public COM_MAIL_TYPE CurMailType
    {
        get
        {
            return this.m_curMailtype;
        }
        set
        {
            if (this.m_CUIListScriptTab != null)
            {
                this.m_curMailtype = value;
                if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
                {
                    this.SetActiveTab(0);
                    this.m_CUIListScriptTab.SelectElement(0, true);
                }
                else if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
                {
                    this.SetActiveTab(1);
                    this.m_CUIListScriptTab.SelectElement(1, true);
                }
                else if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
                {
                    this.SetActiveTab(2);
                    this.m_CUIListScriptTab.SelectElement(2, true);
                }
            }
        }
    }
}

