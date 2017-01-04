using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CSysMailView
{
    private CUIFormScript m_CUIForm;
    private CUIListScript m_CUIListScript;

    private void Draw(CMail mail)
    {
        if (this.m_CUIForm != null)
        {
            this.m_CUIForm.transform.FindChild("PanelAccess").gameObject.CustomSetActive(true);
            Text component = this.m_CUIForm.transform.FindChild("PanelAccess/MailContent").GetComponent<Text>();
            Text text2 = this.m_CUIForm.transform.FindChild("PanelAccess/MailTitle").GetComponent<Text>();
            component.text = mail.mailContent;
            text2.text = mail.subject;
            this.m_CUIListScript.SetElementAmount(mail.accessUseable.Count);
            for (int i = 0; i < mail.accessUseable.Count; i++)
            {
                GameObject itemCell = this.m_CUIListScript.GetElemenet(i).transform.FindChild("itemCell").gameObject;
                CUICommonSystem.SetItemCell(this.m_CUIForm, itemCell, mail.accessUseable[i], true, true);
            }
            GameObject gameObject = this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").gameObject;
            gameObject.CustomSetActive(mail.accessUseable.Count > 0);
            GameObject target = this.m_CUIForm.transform.FindChild("PanelAccess/CheckAccess").gameObject;
            if (CHyperLink.Bind(target, mail.mailHyperlink))
            {
                target.CustomSetActive(true);
                gameObject.CustomSetActive(false);
            }
            else
            {
                target.CustomSetActive(false);
            }
        }
    }

    public CUIFormScript Form
    {
        set
        {
            this.m_CUIForm = value;
            this.m_CUIListScript = this.m_CUIForm.transform.FindChild("PanelAccess/List").GetComponent<CUIListScript>();
        }
    }

    public CMail Mail
    {
        set
        {
            this.Draw(value);
        }
    }

    public COM_MAIL_TYPE mailType
    {
        set
        {
            if (this.m_CUIForm != null)
            {
                if (value == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
                {
                    this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_SysAccess;
                }
                else if (value == COM_MAIL_TYPE.COM_MAIL_FRIEND)
                {
                    this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_FriendAccess;
                }
            }
        }
    }
}

