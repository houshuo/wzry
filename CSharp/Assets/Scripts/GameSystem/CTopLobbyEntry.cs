namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;

    public class CTopLobbyEntry : Singleton<CTopLobbyEntry>
    {
        public void CloseForm()
        {
            Singleton<CLobbySystem>.instance.SetTopBarPriority(enFormPriority.Priority1);
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Lobby_OpenTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTopLobbyForm));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Lobby_CloseTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTopLobbyForm));
        }

        private void OnCloseTopLobbyForm(CUIEvent cuiEvent)
        {
            this.CloseForm();
        }

        private void OnOpenTopLobbyForm(CUIEvent cuiEvent)
        {
            this.OpenForm();
        }

        public void OpenForm()
        {
            Singleton<CLobbySystem>.instance.SetTopBarPriority(enFormPriority.Priority4);
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Lobby_OpenTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTopLobbyForm));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Lobby_CloseTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTopLobbyForm));
        }
    }
}

