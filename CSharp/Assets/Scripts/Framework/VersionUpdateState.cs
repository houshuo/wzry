namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    [GameState]
    public class VersionUpdateState : BaseState
    {
        private void OnExitGame(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CheckDevice_Quit, new CUIEventManager.OnUIEventHandler(this.OnExitGame));
            CVersionUpdateSystem.QuitApp();
        }

        public override void OnStateEnter()
        {
            if (DeviceCheckSys.CheckDeviceIsValid())
            {
                if (!DeviceCheckSys.CheckAvailMemory())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("CheckDevice_QuitGame_CurMemNotEnough"), false, 1.5f, null, new object[0]);
                }
                Singleton<ResourceLoader>.GetInstance().LoadScene("EmptySceneWithCamera", null);
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CheckDevice_Quit, new CUIEventManager.OnUIEventHandler(this.OnExitGame));
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_NetworkUnReachable"), enUIEventID.CheckDevice_Quit, false);
                }
                else
                {
                    MonoSingleton<CVersionUpdateSystem>.GetInstance().StartVersionUpdate(new Assets.Scripts.GameSystem.CVersionUpdateSystem.OnVersionUpdateComplete(this.OnVersionUpdateComplete));
                }
            }
            else
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CheckDevice_Quit, new CUIEventManager.OnUIEventHandler(this.OnExitGame));
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("CheckDevice_QuitGame"), enUIEventID.CheckDevice_Quit, false);
            }
        }

        public override void OnStateLeave()
        {
        }

        private void OnVersionUpdateComplete()
        {
            Singleton<GameStateCtrl>.GetInstance().GotoState("MovieState");
        }
    }
}

