using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Text;
using UnityEngine;

[MessageHandlerClass]
public class CRoleRegisterSys : Singleton<CRoleRegisterSys>
{
    private System.Random m_random = new System.Random();
    public static readonly string s_gameDifficultSelectFormPath = "UGUI/Form/System/RoleCreate/Form_GameDifficultSelect.prefab";
    public static readonly string s_roleCreateFormPath = "UGUI/Form/System/RoleCreate/Form_RoleCreate.prefab";
    public static readonly string s_videoBgFormPath = "UGUI/Form/System/RoleCreate/Form_VideoBg.prefab";

    public void CloseGameDifSelectForm()
    {
        CRoleRegisterView.CloseGameDifSelectForm();
    }

    public void CloseRoleCreateForm()
    {
        Singleton<CUIManager>.GetInstance().CloseForm(s_videoBgFormPath);
        Singleton<CUIManager>.GetInstance().CloseForm(s_roleCreateFormPath);
    }

    public string GetRandomName()
    {
        uint num = 0;
        uint num2 = 0;
        uint num3 = 0;
        ResRobotName dataByKey = null;
        ResRobotSubNameA ea = null;
        ResRobotSubNameB eb = null;
        while (dataByKey == null)
        {
            num = (uint) this.m_random.Next(0, GameDataMgr.robotName.count);
            dataByKey = GameDataMgr.robotName.GetDataByKey((uint) (num + 1));
        }
        while (ea == null)
        {
            num2 = (uint) this.m_random.Next(0, GameDataMgr.robotSubNameA.count);
            ea = GameDataMgr.robotSubNameA.GetDataByKey((uint) (num2 + 1));
        }
        while (eb == null)
        {
            num3 = (uint) this.m_random.Next(0, GameDataMgr.robotSubNameB.count);
            eb = GameDataMgr.robotSubNameB.GetDataByKey((uint) (num3 + 1));
        }
        return string.Format("{0}{1}{2}", Utility.UTF8Convert(dataByKey.szName), Utility.UTF8Convert(ea.szName), Utility.UTF8Convert(eb.szName));
    }

    public override void Init()
    {
        Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreate));
        Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE_RANDOM, new CUIEventManager.OnUIEventHandler(this.OnRandomName));
        Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.GAME_DIFF_SELECT, new CUIEventManager.OnUIEventHandler(this.OnGameDifSelect));
        Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.GAME_DIFF_CONFIRM, new CUIEventManager.OnUIEventHandler(this.OnGameDifConfirm));
        Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE_TIMER_CHANGE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreateTimerChange));
        base.Init();
    }

    [MessageHandler(0x1007)]
    public static void OnAcntSnsNameNtf(CSPkg msg)
    {
        if (string.IsNullOrEmpty(CRoleRegisterView.RoleName))
        {
            CRoleRegisterView.RoleName = Utility.UTF8Convert(msg.stPkgData.stNtfAcntSnsName.szNickName);
        }
    }

    public void OnGameDifConfirm(CUIEvent cuiEvent)
    {
        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa30);
        msg.stPkgData.stSetAcntNewbieTypeReq.bAcntNewbieType = (byte) cuiEvent.m_eventParams.tag;
        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        LobbyLogic.ReqStartGuideLevel11(false);
    }

    public void OnGameDifSelect(CUIEvent cuiEvent)
    {
        CRoleRegisterView.SetGameDifficult(cuiEvent.m_eventParams.tag);
    }

    [MessageHandler(0xa31)]
    public static void OnGameDifSelect(CSPkg msg)
    {
    }

    [MessageHandler(0x3ef)]
    public static void OnNtfRegister(CSPkg msg)
    {
        Singleton<CUIManager>.instance.CloseSendMsgAlert();
        Debug.Log("receive message , id = CSProtocolMacros.SCID_NTF_ACNT_REGISTER ...");
        Singleton<GameStateCtrl>.GetInstance().GotoState("CreateRoleState");
    }

    public void OnRandomName(CUIEvent cuiEvent)
    {
        CRoleRegisterView.RoleName = this.GetRandomName();
    }

    [MessageHandler(0x3f1)]
    public static void OnRegisterRes(CSPkg msg)
    {
        Singleton<CUIManager>.instance.CloseSendMsgAlert();
        if (msg.stPkgData.stAcntRegisterRes.bErrCode != 0)
        {
            Singleton<CRoleRegisterSys>.instance.ShowErrorCode(msg.stPkgData.stAcntRegisterRes.bErrCode);
        }
        Debug.Log("receive message , id = CSProtocolMacros.SCID_ACNT_REGISTER_RES , err code = " + msg.stPkgData.stAcntRegisterRes.bErrCode.ToString());
    }

    private void OnRoleCreate(CUIEvent cuiEvent)
    {
        string roleName = CRoleRegisterView.RoleName;
        switch (Utility.CheckRoleName(roleName))
        {
            case Utility.NameResult.Vaild:
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f0);
                msg.stPkgData.stAcntRegisterReq.dwHeadId = 2;
                msg.stPkgData.stAcntRegisterReq.szUserName = Encoding.UTF8.GetBytes(roleName);
                msg.stPkgData.stAcntRegisterReq.iRegChannel = Singleton<ApolloHelper>.GetInstance().GetChannelID();
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                break;
            }
            case Utility.NameResult.Null:
                CRoleRegisterView.RoleName = string.Empty;
                Singleton<CUIManager>.instance.OpenTips("RoleRegister_InputName", true, 1.5f, null, new object[0]);
                break;

            case Utility.NameResult.OutOfLength:
                Singleton<CUIManager>.instance.OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
                break;

            case Utility.NameResult.InVaildChar:
                Singleton<CUIManager>.instance.OpenTips("RoleRegister_NameInvaildChar", true, 1.5f, null, new object[0]);
                break;
        }
    }

    private void OnRoleCreateTimerChange(CUIEvent cuiEvent)
    {
    }

    public void OpenGameDifSelectForm()
    {
        CRoleRegisterView.OpenGameDifSelectForm();
    }

    public void OpenRoleCreateForm()
    {
        Debug.Log("OpenRoleCreateForm...");
        Singleton<CUIManager>.GetInstance().OpenForm(s_videoBgFormPath, false, true);
        Singleton<CUIManager>.GetInstance().OpenForm(s_roleCreateFormPath, false, true);
    }

    public void SetRoleCreateFormVisible(bool bVisible)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_roleCreateFormPath);
        if (form != null)
        {
            form.gameObject.CustomSetActive(bVisible);
        }
    }

    public void ShowErrorCode(int errorCode)
    {
        if (errorCode == 6)
        {
            Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("RoleRegister_NameExists"), false, 1.5f, null, new object[0]);
        }
    }

    public void ShowErrorCode(byte[] szName)
    {
        string str = Utility.UTF8Convert(szName);
        string roleName = CRoleRegisterView.RoleName;
        string str3 = string.Empty;
        int num = Math.Max(str.Length, roleName.Length);
        for (int i = 0; i < num; i++)
        {
            char ch = str[i];
            if (!ch.Equals(roleName[i]))
            {
                str3 = string.Format("{0}{1}", str3, roleName[i]);
            }
        }
        Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("Register_Name_Invalid_Words_2"), str3), false);
    }

    public override void UnInit()
    {
        Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreate));
        Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE_RANDOM, new CUIEventManager.OnUIEventHandler(this.OnRandomName));
        Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.GAME_DIFF_SELECT, new CUIEventManager.OnUIEventHandler(this.OnGameDifSelect));
        Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.GAME_DIFF_CONFIRM, new CUIEventManager.OnUIEventHandler(this.OnGameDifConfirm));
        Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE_TIMER_CHANGE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreateTimerChange));
        base.UnInit();
    }
}

