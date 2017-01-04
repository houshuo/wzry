namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;

    public class TowerHit
    {
        private bool bValid = false;
        public uint cd_time;
        private int cd_timer = -1;
        public string effect;
        public uint effect_last_time;
        public byte organ_type;
        public string voice;

        public TowerHit(RES_ORGAN_TYPE type)
        {
            TowerHitConf dataByKey = GameDataMgr.towerHitDatabin.GetDataByKey((uint) ((byte) type));
            DebugHelper.Assert(dataByKey != null, "TowerHit towerHitDatabin.GetDataByKey is null, type:" + type);
            if (dataByKey != null)
            {
                this.organ_type = (byte) type;
                this.cd_time = dataByKey.dwCdTime;
                this.voice = dataByKey.szVoice;
                this.effect = dataByKey.szEffect;
                this.effect_last_time = dataByKey.dwLastTime;
                if (this.cd_time > 0)
                {
                    this.cd_timer = Singleton<CTimerManager>.instance.AddTimer((int) this.cd_time, -1, new CTimer.OnTimeUpHandler(this.On_CD_Timer_Finish));
                    Singleton<CTimerManager>.instance.PauseTimer(this.cd_timer);
                }
                this.bValid = true;
            }
        }

        public static void _play_effect(string effect_name, float playTime, GameObject target)
        {
            if (target != null)
            {
                Vector2 sreenLoc = CUIUtility.WorldToScreenPoint(Singleton<CBattleSystem>.GetInstance().FormScript.GetCamera(), target.transform.position);
                Singleton<CUIParticleSystem>.instance.AddParticle(effect_name, playTime, sreenLoc);
            }
        }

        public void Clear()
        {
            if (this.cd_timer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.cd_timer);
            }
            this.cd_timer = -1;
        }

        private void On_CD_Timer_Finish(int index)
        {
            this.bValid = true;
            Singleton<CTimerManager>.instance.PauseTimer(this.cd_timer);
        }

        public void TryActive(GameObject target)
        {
            if ((target != null) && this.bValid)
            {
                if (!string.IsNullOrEmpty(this.effect))
                {
                    MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                    if ((theMinimapSys != null) && (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini))
                    {
                        _play_effect(this.effect, 2f, target);
                    }
                }
                if (!string.IsNullOrEmpty(this.voice))
                {
                    Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(this.voice);
                }
                Singleton<CTimerManager>.instance.ResumeTimer(this.cd_timer);
                this.bValid = false;
            }
        }
    }
}

