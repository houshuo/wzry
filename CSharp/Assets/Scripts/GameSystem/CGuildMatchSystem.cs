namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;

    public class CGuildMatchSystem : Singleton<CGuildMatchSystem>
    {
        public static readonly string GuildMatchFormPath = "UGUI/Form/System/Guild/Form_Guild_Match.prefab";
        public static readonly string GuildMatchRecordFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_Record.prefab";

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchRecordForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchRecordForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_StartGame, new CUIEventManager.OnUIEventHandler(this.OnStartGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OBGame, new CUIEventManager.OnUIEventHandler(this.OnOBGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_TeamListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnTeamListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RankTabChanged, new CUIEventManager.OnUIEventHandler(this.OnRankTabChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_GuildScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnGuildScoreListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_MemberScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnMemberScoreListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Invite, new CUIEventManager.OnUIEventHandler(this.OnInvitem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Kick, new CUIEventManager.OnUIEventHandler(this.OnKick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AppointLeader, new CUIEventManager.OnUIEventHandler(this.OnAppointLeader));
        }

        private void OnAppointLeader(CUIEvent uiEvent)
        {
        }

        private void OnGuildScoreListElementEnabled(CUIEvent uiEvent)
        {
        }

        private void OnInvitem(CUIEvent uiEvent)
        {
        }

        private void OnKick(CUIEvent uiEvent)
        {
        }

        private void OnMemberScoreListElementEnabled(CUIEvent uiEvent)
        {
        }

        private void OnOBGame(CUIEvent uiEvent)
        {
        }

        private void OnOpenMatchForm(CUIEvent uiEvent)
        {
        }

        private void OnOpenMatchRecordForm(CUIEvent uiEvent)
        {
        }

        private void OnRankTabChanged(CUIEvent uiEvent)
        {
        }

        private void OnStartGame(CUIEvent uiEvent)
        {
        }

        private void OnTeamListElementEnabled(CUIEvent uiEvent)
        {
        }

        public enum enGuildMatchFormWidget
        {
            GuildHead_Image,
            GuildName_Text,
            GuildMatchScore_Text,
            Team_List,
            RankTab_List,
            OnlineNum_Text,
            GuildScore_List,
            MemberScore_List,
            Invite_List,
            SelfRank_Panel,
            SelfRankRank_Panel,
            SelfRankHead_Panel,
            SelfRankName_Panel,
            SelfRankScore_Panel,
            MatchOpenTime_Panel
        }

        public enum enGuildMatchRecordFormWidget
        {
            Record_List
        }
    }
}

