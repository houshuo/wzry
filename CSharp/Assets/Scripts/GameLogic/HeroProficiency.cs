namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;

    public class HeroProficiency
    {
        private bool m_needDeginShow = true;
        private int m_showTimes;
        public ObjWrapper m_wrapper;
        private int showTimeInterv;

        public void Init(ObjWrapper wrapper)
        {
            this.m_wrapper = wrapper;
            this.showTimeInterv = 0;
            this.m_needDeginShow = true;
            this.m_showTimes = 0;
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.AddEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
        }

        public void OnSettleCompleteShow(ref SettleEventParam prm)
        {
            if (((this != null) && (this.m_wrapper != null)) && (this.m_wrapper.actor != null))
            {
                COM_PLAYERCAMP hostPlayerCamp = Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
                if ((hostPlayerCamp == this.m_wrapper.actor.TheActorMeta.ActorCamp) && prm.isWin)
                {
                    this.m_showTimes++;
                    this.ShowProficiencyEffect();
                }
                else if ((hostPlayerCamp != this.m_wrapper.actor.TheActorMeta.ActorCamp) && !prm.isWin)
                {
                    this.m_showTimes++;
                    this.ShowProficiencyEffect();
                }
            }
        }

        public void OnShouldShowProficiencyEffect(ref DefaultGameEventParam prm)
        {
            if (prm.orignalAtker == this.m_wrapper.actorPtr)
            {
                this.m_showTimes++;
                this.ShowProficiencyEffect();
            }
        }

        public void ShowProficiencyEffect()
        {
            ActorServerData actorData = new ActorServerData();
            if (Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider).GetActorServerData(ref this.m_wrapper.actor.TheActorMeta, ref actorData) && this.m_wrapper.actor.HudControl.PlayProficiencyAni(actorData.TheProficiencyInfo.Level))
            {
                this.m_showTimes--;
            }
        }

        public void UnInit()
        {
            this.m_wrapper = null;
            this.showTimeInterv = 0;
            this.m_needDeginShow = true;
            this.m_showTimes = 0;
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
            Singleton<GameEventSys>.instance.RmvEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
        }

        public void UpdateLogic(int nDelta)
        {
            if (this.m_needDeginShow && Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
            {
                this.showTimeInterv += nDelta;
                if (this.showTimeInterv > 0x1388)
                {
                    this.m_showTimes++;
                    this.ShowProficiencyEffect();
                    this.showTimeInterv = 0;
                    this.m_needDeginShow = false;
                }
            }
            if (this.m_showTimes > 0)
            {
                this.ShowProficiencyEffect();
            }
        }
    }
}

