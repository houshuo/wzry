namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHead : MonoBehaviour
    {
        private HeroHeadHud _hudOwner;
        private PoolObjHandle<ActorRoot> _myHero;
        private HeadState _state;
        public Image HeroHeadImg;
        public Image HeroPickImg;
        public Image hpBarImgNormal;
        public Image hpBarImgPicked;
        public GameObject hpRadBar;
        public Text ReviveCdTxt;
        public GameObject SoulBgImgObj;
        public Text SoulLevelTxt;

        private void Awake()
        {
            if (this.ReviveCdTxt != null)
            {
                this.ReviveCdTxt.text = string.Empty;
            }
            this.HeroHeadImg = base.transform.Find("HeadMask/HeadImg").GetComponent<Image>();
            this.HeroPickImg = base.transform.Find("Select").GetComponent<Image>();
            this.SoulLevelTxt = base.transform.Find("soulLvlText").GetComponent<Text>();
            if (this.SoulLevelTxt != null)
            {
                this.SoulLevelTxt.text = "1";
            }
            this.hpRadBar = base.transform.Find("progressBar").gameObject;
            this.hpBarImgNormal = this.hpRadBar.transform.Find("Normal").GetComponent<Image>();
            this.hpBarImgPicked = this.hpRadBar.transform.Find("Picked").GetComponent<Image>();
            this.SoulBgImgObj = base.transform.Find("soulBgImg").gameObject;
            bool bActive = Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsSoulGrow();
            this.SoulLevelTxt.gameObject.CustomSetActive(bActive);
            this.SoulBgImgObj.gameObject.CustomSetActive(bActive);
        }

        public void Init(HeroHeadHud hudOwner, PoolObjHandle<ActorRoot> myHero)
        {
            if (myHero != 0)
            {
                this._hudOwner = hudOwner;
                this._myHero = myHero;
                this.SetState(HeadState.Normal);
                uint configId = (uint) myHero.handle.TheActorMeta.ConfigId;
                this.HeroHeadImg.SetSprite(CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic(configId, 0), Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false);
                this.OnHeroHpChange(myHero.handle.ValueComponent.actorHp, myHero.handle.ValueComponent.actorHpTotal);
            }
        }

        public void OnHeroHpChange(int curVal, int maxVal)
        {
            if (this.hpRadBar != null)
            {
                float num = ((float) curVal) / ((float) maxVal);
                this.hpBarImgNormal.CustomFillAmount(num);
                this.hpBarImgPicked.CustomFillAmount(num);
            }
        }

        public void OnHeroSoulLvlChange(int level)
        {
            if (this.SoulLevelTxt != null)
            {
                this.SoulLevelTxt.text = level.ToString();
            }
        }

        public void OnMyHeroDead()
        {
            if (this._myHero != 0)
            {
                this.SetState(HeadState.ReviveCDing);
                if (this.ReviveCdTxt != null)
                {
                    this.ReviveCdTxt.text = string.Format("{0}", Mathf.RoundToInt(this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f));
                    this.ReviveCdTxt.color = Color.white;
                    this.ReviveCdTxt.fontSize = 30;
                }
                if (this.HeroHeadImg != null)
                {
                    this.HeroHeadImg.color = new Color(0.3f, 0.3f, 0.3f);
                }
                if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() && !Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena())
                {
                    base.InvokeRepeating("UpdateReviveCd", 1f, 1f);
                }
                else
                {
                    this.ReviveCdTxt.text = Singleton<CTextManager>.GetInstance().GetText("PlayerHead_Dead");
                    this.ReviveCdTxt.color = Color.gray;
                    this.ReviveCdTxt.fontSize = 14;
                    this.HeroHeadImg.color = new Color(0.3f, 0.3f, 0.3f);
                }
            }
        }

        public void OnMyHeroRevive()
        {
            base.CancelInvoke("UpdateReviveCd");
            if (this.ReviveCdTxt != null)
            {
                this.ReviveCdTxt.text = string.Empty;
            }
            if (this.HeroHeadImg != null)
            {
                this.HeroHeadImg.color = new Color(1f, 1f, 1f);
            }
            this.SetState(HeadState.Normal);
        }

        public void SetPicked(bool isPicked)
        {
            this.hpBarImgNormal.gameObject.CustomSetActive(!isPicked);
            this.hpBarImgPicked.gameObject.CustomSetActive(isPicked);
            if (this.HeroPickImg != null)
            {
                this.HeroPickImg.gameObject.CustomSetActive(isPicked);
            }
            base.GetComponent<RectTransform>().localScale = !isPicked ? Vector3.one : this._hudOwner.pickedScale;
        }

        public void SetPrivates(HeadState inHeadState, PoolObjHandle<ActorRoot> inHero, HeroHeadHud inHudOwner)
        {
            this._myHero = inHero;
            this._hudOwner = inHudOwner;
            this.SetState(inHeadState);
        }

        private void SetState(HeadState hs)
        {
            if (hs != this._state)
            {
                this._state = hs;
                base.GetComponent<CUIEventScript>().enabled = (this._state == HeadState.Normal) || (this._state == HeadState.ReviveReady);
            }
        }

        private void UpdateReviveCd()
        {
            if ((this.MyHero != 0) && (this.MyHero.handle.ActorControl != null))
            {
                int num = Mathf.RoundToInt(this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f);
                if (num >= 0)
                {
                    if (this.ReviveCdTxt != null)
                    {
                        this.ReviveCdTxt.text = string.Format("{0}", num);
                    }
                    this.SetState(HeadState.ReviveCDing);
                }
                else if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode())
                {
                    Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this._myHero.handle.TheActorMeta.PlayerId);
                    if (player != null)
                    {
                        HeadState hs = !player.IsMyTeamOutOfBattle() ? HeadState.ReviveForbid : HeadState.ReviveReady;
                        if (hs != this._state)
                        {
                            this.SetState(hs);
                            if (this._state == HeadState.ReviveReady)
                            {
                                this.ReviveCdTxt.text = Singleton<CTextManager>.GetInstance().GetText("PlayerHead_dianji");
                                this.ReviveCdTxt.color = Color.green;
                                this.ReviveCdTxt.fontSize = 14;
                                this.HeroHeadImg.color = new Color(1f, 1f, 1f);
                            }
                            else
                            {
                                this.ReviveCdTxt.text = Singleton<CTextManager>.GetInstance().GetText("PlayerHead_tuozhan");
                                this.ReviveCdTxt.color = Color.gray;
                                this.ReviveCdTxt.fontSize = 14;
                                this.HeroHeadImg.color = new Color(0.3f, 0.3f, 0.3f);
                            }
                        }
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        if (((hostPlayer != null) && (this._state == HeadState.ReviveReady)) && hostPlayer.Captain.handle.ActorControl.m_isAutoAI)
                        {
                            this.MyHero.handle.ActorControl.Revive(false);
                        }
                    }
                }
            }
        }

        public HeroHeadHud HudOwner
        {
            get
            {
                return this._hudOwner;
            }
        }

        public PoolObjHandle<ActorRoot> MyHero
        {
            get
            {
                return this._myHero;
            }
        }

        public HeadState state
        {
            get
            {
                return this._state;
            }
        }

        public enum HeadState
        {
            Normal,
            ReviveCDing,
            ReviveReady,
            ReviveForbid
        }
    }
}

