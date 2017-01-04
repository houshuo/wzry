namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    public class FireHoleKillNotify
    {
        private bool bFrist_Notifyed;
        private bool bSecond_Notifyed;
        private bool bThird_Notifyed;
        private int first_count;
        private int second_count;
        private int third_count;

        public FireHoleKillNotify()
        {
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new System.Action(this.OnBattleKDAChanged));
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_First_Notify_Num"), out this.first_count))
            {
                DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_First_Notify_Num 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_Second_Notify_Num"), out this.second_count))
            {
                DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_Second_Notify_Num 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_Third_Notify_Num"), out this.third_count))
            {
                DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_Third_Notify_Num 好像不是整数哦， check out");
            }
            this.bFrist_Notifyed = this.bSecond_Notifyed = this.bThird_Notifyed = false;
        }

        public void _broadcast(bool bSelfCamp_Notify, KillDetailInfoType type)
        {
            KillDetailInfo info = new KillDetailInfo {
                bSelfCamp = bSelfCamp_Notify,
                Type = type
            };
            Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
        }

        private bool _check(int count, out bool bSelfCamp_Notify)
        {
            int num = this.getCampKillCount(true);
            int num2 = this.getCampKillCount(false);
            if (num >= count)
            {
                bSelfCamp_Notify = true;
                return true;
            }
            if (num2 >= count)
            {
                bSelfCamp_Notify = false;
                return true;
            }
            bSelfCamp_Notify = false;
            return false;
        }

        public void Clear()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new System.Action(this.OnBattleKDAChanged));
        }

        private int getCampKillCount(bool bSelfCamp)
        {
            COM_PLAYERCAMP camp = !bSelfCamp ? this.getEnemyCamp() : Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
            return Singleton<BattleStatistic>.instance.m_playerKDAStat.GetTeamKillNum(camp);
        }

        private COM_PLAYERCAMP getEnemyCamp()
        {
            if (Singleton<GamePlayerCenter>.instance.hostPlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
            }
            if (Singleton<GamePlayerCenter>.instance.hostPlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
            {
                return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            }
            DebugHelper.Assert(false, "getEnemyCamp error check out");
            return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
        }

        private void OnBattleKDAChanged()
        {
            if (!this.bFrist_Notifyed)
            {
                bool flag;
                if (this._check(this.first_count, out flag))
                {
                    this.bFrist_Notifyed = true;
                    this._broadcast(flag, KillDetailInfoType.Info_Type_FireHole_first);
                }
            }
            else if (!this.bSecond_Notifyed)
            {
                bool flag2;
                if (this._check(this.second_count, out flag2))
                {
                    this.bSecond_Notifyed = true;
                    this._broadcast(flag2, KillDetailInfoType.Info_Type_FireHole_second);
                }
            }
            else
            {
                bool flag3;
                if (!this.bThird_Notifyed && this._check(this.third_count, out flag3))
                {
                    this.bThird_Notifyed = true;
                    this._broadcast(flag3, KillDetailInfoType.Info_Type_FireHole_third);
                }
            }
        }
    }
}

