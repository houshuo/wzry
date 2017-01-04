namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class KillNotify
    {
        private CUIAnimatorScript animatorScript;
        public static string blue_cannon_icon = "UGUI/Sprite/Battle/kn_Blue_Paoche";
        public static string blue_soldier_icon = "UGUI/Sprite/Battle/kn_Blue_Soldier";
        public static string building_icon = "UGUI/Sprite/Battle/kn_Tower";
        public static string dragon_icon = "UGUI/Sprite/Battle/kn_dragon";
        public static int HideTime = 0xdac;
        private int hideTimer;
        private bool IsPlaying;
        private GameObject killerHead;
        private Image KillerImg;
        private List<KillInfo> KillInfoList = new List<KillInfo>();
        public static Dictionary<KillDetailInfoType, byte> knPriority = new Dictionary<KillDetailInfoType, byte>();
        public static int max_count = 5;
        public static string monster_icon = "UGUI/Sprite/Battle/kn_Monster";
        private GameObject node;
        private int play_delta_timer;
        public static string red_cannon_icon = "UGUI/Sprite/Battle/kn_Red_Paoche";
        public static string red_soldier_icon = "UGUI/Sprite/Battle/kn_Red_Soldier";
        public static int s_play_deltaTime = 200;
        private FireHoleKillNotify sub_sys;
        private GameObject VictimHead;
        private Image VictimImg;
        public static string yeguai_icon = "UGUI/Sprite/Battle/kn_yeguai";

        private void _AddkillInfo(KillInfo killInfo)
        {
            if (this.IsDragonKilled(killInfo.MsgType))
            {
                this.KillInfoList.Insert(0, killInfo);
            }
            else if (killInfo.MsgType == KillDetailInfoType.Info_Type_AllDead)
            {
                this.KillInfoList.Add(killInfo);
                Debug.Log("---KN 添加团灭进队列");
            }
            else
            {
                if (this.KillInfoList.Count < max_count)
                {
                    this.KillInfoList.Add(killInfo);
                }
                else
                {
                    int bPriority = 0x2710;
                    int index = 0;
                    for (int j = 0; j < this.KillInfoList.Count; j++)
                    {
                        KillInfo info2 = this.KillInfoList[j];
                        if (info2.bPriority < bPriority)
                        {
                            KillInfo info3 = this.KillInfoList[j];
                            bPriority = info3.bPriority;
                            index = j;
                        }
                    }
                    if (((killInfo.bPriority >= bPriority) && (index >= 0)) && (index < this.KillInfoList.Count))
                    {
                        this.KillInfoList.RemoveAt(index);
                        this.KillInfoList.Add(killInfo);
                    }
                }
                int num4 = -1;
                for (int i = 0; i < this.KillInfoList.Count; i++)
                {
                    KillInfo info4 = this.KillInfoList[i];
                    if (info4.MsgType == KillDetailInfoType.Info_Type_AllDead)
                    {
                        num4 = i;
                    }
                }
                if ((num4 >= 0) && (num4 < this.KillInfoList.Count))
                {
                    KillInfo info = this.KillInfoList[num4];
                    this.KillInfoList[num4] = this.KillInfoList[this.KillInfoList.Count - 1];
                    this.KillInfoList[this.KillInfoList.Count - 1] = info;
                }
            }
        }

        public void AddKillInfo(KillDetailInfo info)
        {
            if (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
                this.AddKillInfo(KillNotifyUT.Convert_DetailInfo_KillInfo(info));
            }
        }

        public void AddKillInfo(KillInfo killInfo)
        {
            if (this.IsPlaying)
            {
                this._AddkillInfo(killInfo);
            }
            else
            {
                this.PlayKillNotify(killInfo.KillerImgSrc, killInfo.VictimImgSrc, killInfo.MsgType, killInfo.bSrcAllies, killInfo.bPlayerSelf_KillOrKilled, killInfo.actorType);
            }
        }

        public void Clear()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            this.KillInfoList.Clear();
            this.animatorScript = null;
            this.killerHead = (GameObject) (this.VictimHead = null);
            this.KillerImg = (Image) (this.VictimImg = null);
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.hideTimer);
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.play_delta_timer);
            this.IsPlaying = false;
            this.node = null;
            if (this.sub_sys != null)
            {
                this.sub_sys.Clear();
                this.sub_sys = null;
            }
        }

        public void ClearKillNotifyList()
        {
            if (this.KillInfoList != null)
            {
                this.KillInfoList.Clear();
            }
        }

        public static byte GetPriority(KillDetailInfoType type)
        {
            byte num;
            knPriority.TryGetValue(type, out num);
            return num;
        }

        public void Hide()
        {
            if (this.node != null)
            {
                CUICommonSystem.SetObjActive(this.node, false);
            }
            if (this.animatorScript != null)
            {
                this.animatorScript.SetAnimatorEnable(false);
            }
        }

        public void Init(CUIFormScript formObj)
        {
            this.IsPlaying = false;
            Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            this.node = Utility.FindChild(formObj.gameObject, "KillNotify_New");
            this.animatorScript = Utility.GetComponetInChild<CUIAnimatorScript>(this.node, "KillNotify_Sub");
            this.KillerImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/KillerHead/KillerImg");
            this.VictimImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/VictimHead/VictimImg");
            this.killerHead = Utility.FindChild(this.node, "KillNotify_Sub/KillerHead");
            this.VictimHead = Utility.FindChild(this.node, "KillNotify_Sub/VictimHead");
            this.Hide();
            this.hideTimer = Singleton<CTimerManager>.GetInstance().AddTimer(HideTime, -1, new CTimer.OnTimeUpHandler(this.OnPlayEnd));
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
            this.play_delta_timer = Singleton<CTimerManager>.GetInstance().AddTimer(s_play_deltaTime, -1, new CTimer.OnTimeUpHandler(this.On_Play_DeltaEnd));
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.play_delta_timer);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsFireHolePlayMode())
            {
                this.sub_sys = new FireHoleKillNotify();
            }
        }

        private bool IsDragonKilled(KillDetailInfoType type)
        {
            return (((type == KillDetailInfoType.Info_Type_KillDragon) || (type == KillDetailInfoType.Info_Type_KillBIGDRAGON)) || (type == KillDetailInfoType.Info_Type_KillBARON));
        }

        public static void LoadConfig()
        {
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_MaxCount"), out max_count))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_MaxCount 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_HideTime"), out HideTime))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_HideTime 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_Play_DeltaTime"), out s_play_deltaTime))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_Play_DeltaTime 好像不是整数哦， check out");
            }
            Array values = Enum.GetValues(typeof(KillDetailInfoType));
            for (int i = 0; i < values.Length; i++)
            {
                int num2 = (int) values.GetValue(i);
                if (num2 != 0)
                {
                    ResKNPriority dataByKey = GameDataMgr.killNotifyDatabin.GetDataByKey((long) num2);
                    DebugHelper.Assert(dataByKey != null, "播报配置找不到 配置项:" + ((KillDetailInfoType) num2) + ", 教练 检查下...");
                    if (dataByKey != null)
                    {
                        if (!knPriority.ContainsKey((KillDetailInfoType) num2))
                        {
                            knPriority.Add((KillDetailInfoType) num2, dataByKey.bPriority);
                        }
                        else
                        {
                            knPriority[(KillDetailInfoType) num2] = dataByKey.bPriority;
                        }
                    }
                }
            }
        }

        private void On_Play_DeltaEnd(int timerSequence)
        {
            UT.ResetTimer(this.play_delta_timer, true);
            if (this.KillInfoList.Count > 0)
            {
                KillInfo info = this.KillInfoList[0];
                this.KillInfoList.RemoveAt(0);
                this.PlayKillNotify(info.KillerImgSrc, info.VictimImgSrc, info.MsgType, info.bSrcAllies, info.bPlayerSelf_KillOrKilled, info.actorType);
            }
        }

        private void OnAchievementEvent(KillDetailInfo DetailInfo)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaMode())
            {
                this.AddKillInfo(DetailInfo);
            }
        }

        private void OnPlayEnd(int timerSequence)
        {
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
            UT.ResetTimer(this.play_delta_timer, false);
            this.Hide();
            this.IsPlaying = this.KillInfoList.Count > 0;
        }

        public void PlayAnimator(string strAnimName)
        {
            if (!string.IsNullOrEmpty(strAnimName) && (this.animatorScript != null))
            {
                this.Show();
                UT.ResetTimer(this.hideTimer, false);
                this.animatorScript.PlayAnimator(strAnimName);
                this.IsPlaying = true;
            }
        }

        private void PlayKillNotify(string KillerSrc, string VictimSrc, KillDetailInfoType Type, bool bSrcAllies, bool bSelfKillORKilled, ActorTypeDef actorType)
        {
            if (Type == KillDetailInfoType.Info_Type_AllDead)
            {
                Debug.Log("---KN 播团灭");
            }
            this.Show();
            UT.ResetTimer(this.hideTimer, false);
            string str = KillNotifyUT.GetSoundEvent(Type, bSrcAllies, bSelfKillORKilled, actorType);
            if (!string.IsNullOrEmpty(str))
            {
                Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(str);
            }
            string animation = KillNotifyUT.GetAnimation(Type, bSrcAllies);
            if (!string.IsNullOrEmpty(animation) && (this.animatorScript != null))
            {
                this.animatorScript.PlayAnimator(animation);
            }
            KillNotifyUT.SetImageSprite(this.KillerImg, KillerSrc);
            if (string.IsNullOrEmpty(KillerSrc))
            {
                this.SetKillerShow(false);
            }
            else
            {
                this.SetKillerShow(true);
            }
            bool flag = (((((Type == KillDetailInfoType.Info_Type_DestroyTower) || (Type == KillDetailInfoType.Info_Type_DestroyBase)) || ((Type == KillDetailInfoType.Info_Type_AllDead) || (Type == KillDetailInfoType.Info_Type_RunningMan))) || (((Type == KillDetailInfoType.Info_Type_Reconnect) || (Type == KillDetailInfoType.Info_Type_Disconnect)) || ((Type == KillDetailInfoType.Info_Type_KillDragon) || (Type == KillDetailInfoType.Info_Type_Game_Start_Wel)))) || (((Type == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3) || (Type == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5)) || ((Type == KillDetailInfoType.Info_Type_Soldier_Activate) || (Type == KillDetailInfoType.Info_Type_KillBIGDRAGON)))) || (Type == KillDetailInfoType.Info_Type_KillBARON);
            this.SetVictimShow(!flag);
            KillNotifyUT.SetImageSprite(this.VictimImg, VictimSrc);
            this.IsPlaying = true;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddSprite(building_icon);
            preloadTab.AddSprite(monster_icon);
            preloadTab.AddSprite(dragon_icon);
            preloadTab.AddSprite(yeguai_icon);
            preloadTab.AddSprite(blue_cannon_icon);
            preloadTab.AddSprite(red_cannon_icon);
            preloadTab.AddSprite(blue_soldier_icon);
            preloadTab.AddSprite(red_soldier_icon);
        }

        private void SetKillerShow(bool bShow)
        {
            if (this.killerHead != null)
            {
                this.killerHead.gameObject.CustomSetActive(bShow);
            }
        }

        private void SetVictimShow(bool bShow)
        {
            if (this.VictimHead != null)
            {
                this.VictimHead.gameObject.CustomSetActive(bShow);
            }
        }

        public void Show()
        {
            if (this.node != null)
            {
                CUICommonSystem.SetObjActive(this.node, true);
            }
            if (this.animatorScript != null)
            {
                this.animatorScript.SetAnimatorEnable(true);
            }
        }
    }
}

