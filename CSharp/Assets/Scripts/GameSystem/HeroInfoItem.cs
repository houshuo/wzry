namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class HeroInfoItem
    {
        private GameObject _baseInfoItem;
        private GameObject _emptyGo;
        private Image[] _equipIcons;
        private GameObject _equipInfoItem;
        private Image _headImg;
        private HeroKDA _heroInfo;
        private Text _kdaText;
        private Text _levelText;
        private Text _moneyText;
        private Text _reviveTxt;
        public COM_PLAYERCAMP listCamp;
        public int listIndex;

        public HeroInfoItem(COM_PLAYERCAMP _listCamp, int _listIndex, HeroKDA _heroKDA, GameObject baseInfoItem, GameObject equipInfoItem)
        {
            this.listCamp = _listCamp;
            this.listIndex = _listIndex;
            this._heroInfo = _heroKDA;
            this._baseInfoItem = baseInfoItem;
            this._equipInfoItem = equipInfoItem;
            this._baseInfoItem.GetComponent<CUIEventScript>().enabled = true;
            this._headImg = Utility.GetComponetInChild<Image>(baseInfoItem, "Head");
            this._reviveTxt = Utility.GetComponetInChild<Text>(baseInfoItem, "Revive");
            this._levelText = Utility.GetComponetInChild<Text>(baseInfoItem, "Level");
            this._kdaText = Utility.GetComponetInChild<Text>(baseInfoItem, "KDA");
            this._moneyText = Utility.GetComponetInChild<Text>(baseInfoItem, "Money");
            this._emptyGo = Utility.FindChild(baseInfoItem, "Empty");
            this._emptyGo.CustomSetActive(false);
            this._equipInfoItem.GetComponent<CUIEventScript>().enabled = true;
            this._equipIcons = new Image[6];
            for (int i = 0; i < 6; i++)
            {
                this._equipIcons[i] = Utility.GetComponetInChild<Image>(equipInfoItem, "Icon_" + i);
                this._equipIcons[i].gameObject.CustomSetActive(true);
            }
            this._headImg.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustCircleSmall_Dir, CSkinInfo.GetHeroSkinPic((uint) this._heroInfo.HeroId, 0)), Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false);
            this.ValidateLevel();
            this.ValidateKDA();
            this.ValidateMoney();
            this.ValidateEquip();
            this.ValidateReviceCD();
        }

        public void LateUpdate()
        {
            this.ValidateReviceCD();
        }

        public static void MakeEmpty(GameObject baseInfoItem, GameObject equipInfoItem)
        {
            baseInfoItem.GetComponent<CUIEventScript>().enabled = false;
            Utility.FindChild(baseInfoItem, "Empty").CustomSetActive(true);
            equipInfoItem.GetComponent<CUIEventScript>().enabled = false;
            for (int i = 0; i < 6; i++)
            {
                Utility.FindChild(equipInfoItem, "Icon_" + i).CustomSetActive(false);
            }
        }

        public void ValidateEquip()
        {
            stEquipInfo[] equips = this._heroInfo.actorHero.handle.EquipComponent.GetEquips();
            for (int i = 0; i < 6; i++)
            {
                Image image = this._equipIcons[i];
                ushort equipID = equips[i].m_equipID;
                bool flag = false;
                if (equipID > 0)
                {
                    ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                    if (dataByKey != null)
                    {
                        image.gameObject.CustomSetActive(true);
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_System_BattleEquip_Dir, dataByKey.szIcon);
                        CUIUtility.SetImageSprite(image, prefabPath, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    image.SetSprite(string.Format((this.listCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "{0}EquipmentSpaceRed" : "{0}EquipmentSpace", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false);
                }
            }
        }

        public void ValidateKDA()
        {
            object[] objArray1 = new object[] { this._heroInfo.numKill, " / ", this._heroInfo.numDead, " / ", this._heroInfo.numAssist };
            this._kdaText.text = string.Concat(objArray1);
        }

        public void ValidateLevel()
        {
            this._levelText.text = this._heroInfo.actorHero.handle.ValueComponent.actorSoulLevel.ToString();
        }

        public void ValidateMoney()
        {
            this._moneyText.text = this._heroInfo.TotalCoin.ToString();
        }

        public void ValidateReviceCD()
        {
            if (((this._heroInfo != null) && (this._heroInfo.actorHero != 0)) && this._heroInfo.actorHero.handle.ActorControl.IsDeadState)
            {
                this._reviveTxt.text = string.Format("{0}", Mathf.FloorToInt(this._heroInfo.actorHero.handle.ActorControl.ReviveCooldown * 0.001f));
                this._headImg.color = CUIUtility.s_Color_Grey;
            }
            else
            {
                this._reviveTxt.text = string.Empty;
                this._headImg.color = CUIUtility.s_Color_Full;
            }
        }

        public HeroKDA HeroInfo
        {
            get
            {
                return this._heroInfo;
            }
        }
    }
}

