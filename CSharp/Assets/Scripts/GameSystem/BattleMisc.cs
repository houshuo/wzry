namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleMisc
    {
        private CUIAnimatorScript animationScript;
        private bool bIn10;
        private bool bIn30;
        private Image blood_eft;
        private Image bloodImage;
        private bool bNormal = true;
        private GameObject boss_hp_Node;
        private int boss_hp_timer = -1;
        private Image boss_icon;
        private PoolObjHandle<ActorRoot> bossActorRoot = new PoolObjHandle<ActorRoot>();
        public static PoolObjHandle<ActorRoot> BossRoot = new PoolObjHandle<ActorRoot>();
        private Text buff_cd_text;
        private GameObject buff_node;
        private PoolObjHandle<ActorRoot> eliteActor = new PoolObjHandle<ActorRoot>();
        private string eliteActorName;
        private const int EliteBarDurationDead = 0x9c4;
        private const int EliteBarDurationLost = 0x2710;
        private int eliteBarTimer = -1;
        private Text hpText;
        private CUIFormScript map_fromScript;
        private Text nameText;
        private int second_timer = -1;
        private int total_second;

        public void BindBossMonster(PoolObjHandle<ActorRoot> owner)
        {
            if (owner != 0)
            {
                this.bossActorRoot = owner;
                if (this.bossActorRoot.handle.ValueComponent != null)
                {
                    this.bossActorRoot.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnBossHpChange);
                }
                if (this.boss_hp_timer != -1)
                {
                    Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
                }
                this.boss_hp_timer = Singleton<CTimerManager>.instance.AddTimer(0x3e8, -1, new CTimer.OnTimeUpHandler(this.Check_Boss_InBattle));
            }
        }

        public void BindHP()
        {
            ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.instance.GetHostPlayer().GetAllHeroes();
            DebugHelper.Assert(allHeroes.isValidReference, "invalid all heros list.");
            if (allHeroes.isValidReference)
            {
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    if (allHeroes[i] != 0)
                    {
                        PoolObjHandle<ActorRoot> handle = allHeroes[i];
                        if (handle.handle.ValueComponent != null)
                        {
                            PoolObjHandle<ActorRoot> handle2 = allHeroes[i];
                            handle2.handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHPChange);
                        }
                        else
                        {
                            object[] inParameters = new object[] { i };
                            DebugHelper.Assert(false, "invalid player.valuecomponent in player list at index {0}", inParameters);
                        }
                    }
                    else
                    {
                        object[] objArray2 = new object[] { i };
                        DebugHelper.Assert(false, "invalid player in player list at index {0}", objArray2);
                    }
                }
            }
            this.Check_hp();
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ValueComponent != null))
            {
                hostPlayer.Captain.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHPChange);
            }
            else if (hostPlayer == null)
            {
                DebugHelper.Assert(false, "invalid host player");
            }
            else if (hostPlayer.Captain == 0)
            {
                DebugHelper.Assert(false, "invalid host player captain");
            }
            else if (hostPlayer.Captain.handle.ValueComponent == null)
            {
                DebugHelper.Assert(false, "invalid host player captain->valuecomponent");
            }
        }

        private bool BindNewElite(PoolObjHandle<ActorRoot> inMonster)
        {
            if (inMonster == 0)
            {
                return false;
            }
            DebugHelper.Assert(this.eliteActor == 0);
            this.eliteActor = inMonster;
            this.eliteActorName = this.QueryEliteName((ActorRoot) this.eliteActor);
            if (this.eliteActor.handle.ValueComponent != null)
            {
                this.eliteActor.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnEliteHpChange);
            }
            this.ChangeToElite();
            this.OnEliteHpChange();
            this.OpenEliteBar();
            this.ToCloseEliteBar(0x2710);
            return true;
        }

        private void ChangeToBoss()
        {
            if (this.nameText != null)
            {
                this.nameText.text = this.QueryBossName();
            }
            this.SetBossIcon((ActorRoot) this.bossActorRoot);
        }

        private void ChangeToElite()
        {
            if ((this.nameText != null) && !string.IsNullOrEmpty(this.eliteActorName))
            {
                this.nameText.text = this.eliteActorName;
            }
            this.SetBossIcon((ActorRoot) this.eliteActor);
        }

        private void Check_Boss_InBattle(int timer)
        {
            if (((this.bossActorRoot == 0) || (this.bossActorRoot.handle.ActorControl == null)) || (this.boss_hp_Node == null))
            {
                this.Clear();
            }
            else if (this.bossActorRoot.handle.ActorControl.IsInBattle)
            {
                this.UnbindCurrentElite();
                this.OnBossHpChange();
                this.OpenEliteBar();
                this.ChangeToBoss();
            }
        }

        private void Check_hp()
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer == null) || (hostPlayer.Captain == 0)) || ((hostPlayer.Captain.handle.ActorControl == null) || (hostPlayer.Captain.handle.ValueComponent == null)))
            {
                this.Clear();
            }
            else
            {
                VFactor factor = (VFactor) (hostPlayer.Captain.handle.ValueComponent.GetHpRate() * 100L);
                int roundInt = factor.roundInt;
                if (hostPlayer.Captain.handle.ActorControl.IsDeadState || (hostPlayer.Captain.handle.ValueComponent.actorHp <= 0))
                {
                    this.bIn10 = this.bIn30 = false;
                    this.bNormal = true;
                    if (this.animationScript != null)
                    {
                        this.animationScript.PlayAnimator("Rid_Close");
                        this.animationScript.gameObject.CustomSetActive(false);
                    }
                }
                else if (roundInt > 30)
                {
                    this.bIn10 = this.bIn30 = false;
                    if (!this.bNormal)
                    {
                        if (this.animationScript != null)
                        {
                            this.animationScript.PlayAnimator("Rid_Close");
                            this.animationScript.gameObject.CustomSetActive(false);
                        }
                        this.bNormal = true;
                    }
                }
                else if (roundInt <= 10)
                {
                    if (!this.bIn10)
                    {
                        this.bIn10 = true;
                        this.bNormal = false;
                        this.bIn30 = false;
                        if (this.animationScript != null)
                        {
                            this.animationScript.gameObject.CustomSetActive(true);
                            this.animationScript.PlayAnimator("Rid_02");
                        }
                    }
                }
                else if ((roundInt <= 30) && !this.bIn30)
                {
                    this.bIn30 = true;
                    this.bIn10 = false;
                    this.bNormal = false;
                    if (this.animationScript != null)
                    {
                        this.animationScript.gameObject.CustomSetActive(true);
                        this.animationScript.PlayAnimator("Rid_Stat");
                    }
                }
            }
        }

        public void Clear()
        {
            if (this.second_timer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.second_timer);
            }
            this.second_timer = -1;
            this.animationScript = null;
            this.buff_node = null;
            this.buff_cd_text = null;
            this.second_timer = -1;
            this.bIn10 = false;
            this.bIn30 = false;
            this.bNormal = true;
            this.bossActorRoot.Release();
            this.boss_hp_Node = null;
            if (this.boss_hp_timer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
            }
            this.boss_hp_timer = -1;
            this.hpText = null;
            this.bloodImage = null;
            this.blood_eft = null;
            this.boss_icon = null;
            BossRoot.Release();
            this.map_fromScript = null;
        }

        public void Init(GameObject node, CUIFormScript formScript)
        {
            this.map_fromScript = formScript;
            node.CustomSetActive(true);
            this.buff_node = Utility.FindChild(node, "buff");
            this.buff_cd_text = Utility.GetComponetInChild<Text>(this.buff_node, "Present/Text_Skill_1_CD");
            this.buff_node.CustomSetActive(false);
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitch));
            Transform transform = formScript.transform.Find("Flashing_RedHud");
            if (transform != null)
            {
                this.animationScript = transform.GetComponent<CUIAnimatorScript>();
            }
            this.bIn10 = false;
            this.bIn30 = false;
            this.bNormal = true;
            this.bossActorRoot.Release();
            this.boss_hp_Node = Utility.FindChild(node, "boss_blood");
            this.boss_hp_Node.CustomSetActive(false);
            if (this.boss_hp_timer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.boss_hp_timer);
            }
            this.boss_hp_timer = -1;
            this.hpText = Utility.GetComponetInChild<Text>(this.boss_hp_Node, "Bar/txt");
            this.nameText = Utility.GetComponetInChild<Text>(this.boss_hp_Node, "Bar/name");
            this.bloodImage = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Fore");
            this.blood_eft = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Fore/Effect_BloodBar_Front");
            this.boss_icon = Utility.GetComponetInChild<Image>(this.boss_hp_Node, "Bar/Icon");
            this.eliteActor.Release();
            this.eliteActorName = null;
            this.eliteBarTimer = -1;
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
        }

        private void On_Second_Time(int timer)
        {
            if (this.buff_cd_text == null)
            {
                this.Clear();
            }
            if (Singleton<CBattleSystem>.instance.FightForm != null)
            {
                this.total_second--;
                if (this.buff_cd_text != null)
                {
                    this.buff_cd_text.text = this.total_second.ToString();
                }
                if (this.total_second == 0)
                {
                    this.buff_node.CustomSetActive(false);
                    if (this.second_timer != -1)
                    {
                        Singleton<CTimerManager>.instance.RemoveTimer(this.second_timer);
                    }
                    this.second_timer = -1;
                }
            }
        }

        private void onActorDamage(ref HurtEventResultInfo prm)
        {
            if (((this.bossActorRoot == 0) || (this.bossActorRoot.handle.ActorControl == null)) || !this.bossActorRoot.handle.ActorControl.IsInBattle)
            {
                PoolObjHandle<ActorRoot> src = prm.src;
                PoolObjHandle<ActorRoot> atker = prm.atker;
                if (((src != 0) && (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)) && ((src.handle.HudControl != null) && src.handle.HudControl.bBossHpBar))
                {
                    if (src.handle.ActorControl.IsDeadState)
                    {
                        if (src == this.eliteActor)
                        {
                            this.UnbindCurrentElite();
                        }
                    }
                    else
                    {
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        if ((hostPlayer != null) && (atker == hostPlayer.Captain))
                        {
                            if (src != this.eliteActor)
                            {
                                this.UnbindCurrentElite();
                                this.BindNewElite(src);
                            }
                            else
                            {
                                this.ChangeToElite();
                                if (this.eliteBarTimer != -1)
                                {
                                    Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
                                    this.eliteBarTimer = -1;
                                }
                                else
                                {
                                    this.OnEliteHpChange();
                                    this.OpenEliteBar();
                                }
                                this.ToCloseEliteBar(0x2710);
                            }
                        }
                    }
                }
            }
        }

        private void OnBossHpChange()
        {
            if (((this.bossActorRoot == 0) || (this.bossActorRoot.handle.ValueComponent == null)) || (this.hpText == null))
            {
                this.Clear();
            }
            else
            {
                int actorHp = this.bossActorRoot.handle.ValueComponent.actorHp;
                int actorHpTotal = this.bossActorRoot.handle.ValueComponent.actorHpTotal;
                this.hpText.text = string.Format("{0}/{1}", actorHp, actorHpTotal);
                this.SetHp(((float) actorHp) / ((float) actorHpTotal));
            }
        }

        private void OnCaptainSwitch(ref DefaultGameEventParam prm)
        {
            this.BindHP();
        }

        private void OnCloseEliteBar(int timer)
        {
            if (this.eliteBarTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
                this.eliteBarTimer = -1;
            }
            if (this.boss_hp_Node != null)
            {
                this.boss_hp_Node.CustomSetActive(false);
            }
            this.RebindBoss();
        }

        private void OnEliteHpChange()
        {
            if (this.eliteActor != 0)
            {
                int actorHp = this.eliteActor.handle.ValueComponent.actorHp;
                int actorHpTotal = this.eliteActor.handle.ValueComponent.actorHpTotal;
                this.hpText.text = string.Format("{0}/{1}", actorHp, actorHpTotal);
                this.SetHp(((float) actorHp) / ((float) actorHpTotal));
            }
        }

        private void OnHPChange()
        {
            this.Check_hp();
        }

        private void OpenEliteBar()
        {
            if (this.eliteBarTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
                this.eliteBarTimer = -1;
            }
            if (this.boss_hp_Node != null)
            {
                this.boss_hp_Node.CustomSetActive(true);
            }
        }

        private string QueryBossName()
        {
            DebugHelper.Assert((bool) this.bossActorRoot);
            return UT.Bytes2String(MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(this.bossActorRoot.handle.TheActorMeta.ConfigId).szName);
        }

        private string QueryEliteName(ActorRoot inActor)
        {
            string str = null;
            if (inActor != null)
            {
                ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inActor.TheActorMeta.ConfigId);
                if (dataCfgInfoByCurLevelDiff == null)
                {
                    return str;
                }
                str = UT.Bytes2String(dataCfgInfoByCurLevelDiff.szName);
                if ((inActor.SkillControl == null) || (inActor.SkillControl.talentSystem == null))
                {
                    return str;
                }
                string str3 = string.Empty;
                PassiveSkill[] skillArray = inActor.SkillControl.talentSystem.QueryTalents();
                if (skillArray != null)
                {
                    foreach (PassiveSkill skill in skillArray)
                    {
                        if ((skill != null) && skill.bShowAsElite)
                        {
                            str3 = str3 + skill.PassiveSkillName + "之";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    str = str3 + str;
                }
            }
            return str;
        }

        public void RebindBoss()
        {
            if (BossRoot != 0)
            {
                this.BindBossMonster(BossRoot);
            }
        }

        private void SetBossIcon(ActorRoot inActor)
        {
            if ((inActor != null) && (this.map_fromScript != null))
            {
                ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inActor.TheActorMeta.ConfigId);
                if (dataCfgInfoByCurLevelDiff != null)
                {
                    if (!string.IsNullOrEmpty(dataCfgInfoByCurLevelDiff.szBossIcon))
                    {
                        this.boss_icon.SetSprite(dataCfgInfoByCurLevelDiff.szBossIcon, this.map_fromScript, true, false, false);
                    }
                    else
                    {
                        this.boss_icon.SetSprite("UGUI/Sprite/Dynamic/BustCircle/50001", this.map_fromScript, true, false, false);
                    }
                }
            }
        }

        private void SetHp(float a)
        {
            this.bloodImage.CustomFillAmount(a);
            RectTransform transform = this.blood_eft.transform as RectTransform;
            transform.anchoredPosition = new Vector2(a * (this.bloodImage.transform as RectTransform).sizeDelta.x, transform.anchoredPosition.y);
        }

        public void Show_BuffCD(int icon, int tsecond)
        {
            if (this.buff_cd_text == null)
            {
                this.Clear();
            }
            if (tsecond >= 0)
            {
                if (tsecond == 0)
                {
                    this.buff_node.CustomSetActive(true);
                    if (this.buff_cd_text != null)
                    {
                        this.buff_cd_text.gameObject.CustomSetActive(false);
                    }
                }
                else
                {
                    this.buff_node.CustomSetActive(true);
                    this.total_second = tsecond / 0x3e8;
                    if (this.buff_cd_text != null)
                    {
                        this.buff_cd_text.text = this.total_second.ToString();
                        this.buff_cd_text.gameObject.CustomSetActive(true);
                    }
                    if (this.total_second > 0)
                    {
                        this.second_timer = Singleton<CTimerManager>.instance.AddTimer(0x3e8, -1, new CTimer.OnTimeUpHandler(this.On_Second_Time));
                    }
                }
            }
        }

        private void ToCloseEliteBar(int inDelay)
        {
            if (this.eliteBarTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.eliteBarTimer);
                this.eliteBarTimer = -1;
            }
            if (inDelay > 0)
            {
                this.eliteBarTimer = Singleton<CTimerManager>.instance.AddTimer(inDelay, 1, new CTimer.OnTimeUpHandler(this.OnCloseEliteBar));
            }
            else
            {
                this.OnCloseEliteBar(-1);
            }
        }

        private void UnbindCurrentElite()
        {
            if (this.eliteActor != 0)
            {
                if (this.eliteActor.handle.ValueComponent != null)
                {
                    this.eliteActor.handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnEliteHpChange);
                }
                this.eliteActor.Release();
                this.eliteActorName = null;
            }
            this.ToCloseEliteBar(0x9c4);
        }

        public void Uninit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
            this.OnCloseEliteBar(-1);
            this.eliteActor.Release();
            this.eliteActorName = null;
            this.eliteBarTimer = -1;
        }
    }
}

