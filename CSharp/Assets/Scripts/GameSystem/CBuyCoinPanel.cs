namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CBuyCoinPanel : CBuyPanelBase
    {
        private int m_CostDiamond;
        private Text m_CostDiamondText;
        private int m_GainCoin;
        private Text m_GainCoinText;
        private CBuyCoinInfoPanel m_InfoPanel;
        private Text m_TitleText;

        public void Buy(int Param)
        {
            if (Param == 1)
            {
                this.BuyOne();
            }
            else if (Param == 10)
            {
                this.BuyTen();
            }
        }

        public void BuyCoinRsp(CSPkg msg)
        {
            this.CurBuyTime = (msg.stPkgData.stCoinBuyRsp.wBuyStartFreq + msg.stPkgData.stCoinBuyRsp.stBuyList.bCoinGetCnt) - 1;
            this.m_InfoPanel.BuyCoinRsp(msg);
        }

        private void BuyOne()
        {
            if (this.bCanBuy)
            {
                this.sendBuyCoinMsg(0);
            }
            else if (!base.IsHaveEnoughDianQuan(1))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
            }
        }

        private void BuyTen()
        {
            if (this.bCanBuyTen)
            {
                this.sendBuyCoinMsg(1);
            }
            else if (!base.IsHaveEnoughDianQuan(10))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
            }
        }

        private void calcComsume()
        {
            ResCoinBuyInfo dataByKey = GameDataMgr.coninBuyDatabin.GetDataByKey((uint) ((ushort) (base.m_CurBuyTime + 1)));
            if (dataByKey != null)
            {
                this.m_CostDiamond = dataByKey.wCouponsCost;
                this.m_GainCoin = (int) this.calcGainCoin(dataByKey.dwCoinBase);
            }
            else
            {
                this.m_CostDiamond = 0;
                this.m_GainCoin = 0;
            }
        }

        private uint calcGainCoin(uint coinBase)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x1c).dwConfValue;
            return (coinBase + (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Level * dwConfValue));
        }

        protected override uint GetRequireDianquan(int times)
        {
            uint num = 0;
            uint wCouponsCost = 0;
            for (int i = 0; i < times; i++)
            {
                wCouponsCost = GameDataMgr.coninBuyDatabin.GetDataByKey((uint) ((ushort) ((base.m_CurBuyTime + 1) + i))).wCouponsCost;
                if ((wCouponsCost + num) > 0xffffffffL)
                {
                    return uint.MaxValue;
                }
                num += wCouponsCost;
            }
            return num;
        }

        public void init()
        {
            base.FormPath = "UGUI/Form/System/Purchase/Form_BuyCoin.prefab";
            this.m_InfoPanel = new CBuyCoinInfoPanel();
            base.m_MaxBuyTime = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x16).dwConfValue;
        }

        public override void initPanel(CUIFormScript form)
        {
            base.initPanel(form);
            this.m_TitleText = base.m_FormScript.transform.Find("Bg/Bg1/Title").GetComponent<Text>();
            base.m_Buttons = new GameObject[] { base.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyOne").gameObject, base.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyTen").gameObject };
            base.m_CheckRightButton = base.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_CheckRight").gameObject;
            this.m_CostDiamondText = base.m_FormScript.transform.Find("Bg/Bg1/pnlAction/DiamondNum").GetComponent<Text>();
            this.m_GainCoinText = base.m_FormScript.transform.Find("Bg/Bg1/pnlAction/CoinNum").GetComponent<Text>();
            this.m_InfoPanel.initInfoPanel(base.m_FormScript.transform.Find("Bg/Bg2").gameObject);
            this.RefreshUI();
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new System.Action(this.onAtrrChange));
        }

        private void onAtrrChange()
        {
            this.RefreshUI();
        }

        private void refreshButtonState(int timeToBuy)
        {
            if (base.m_FormScript != null)
            {
                GameObject gameObject = base.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyTen").gameObject;
                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                if (component == null)
                {
                    component = gameObject.AddComponent<CUIEventScript>();
                    component.Initialize(base.m_FormScript);
                }
                if (base.IsHaveEnoughTimes(timeToBuy))
                {
                    gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    eventParams.tag = 1;
                }
                else
                {
                    gameObject.GetComponent<Image>().color = new Color(0f, 1f, 1f, 1f);
                    eventParams.tag = 0;
                }
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Purchase_BuyCoinTen, eventParams);
            }
        }

        private void RefreshUI()
        {
            if (base.bOpen)
            {
                if (this.m_TitleText != null)
                {
                    this.m_TitleText.text = this.TitleDescribe;
                }
                this.m_CostDiamondText.text = this.m_CostDiamond.ToString();
                this.m_GainCoinText.text = this.m_GainCoin.ToString();
                this.updateButtonState();
            }
        }

        private void sendBuyCoinMsg(int BuyCoinType)
        {
            CSPkg msg = null;
            msg = NetworkModule.CreateDefaultCSPKG(0x45b);
            msg.stPkgData.stCoinBuyReq = new CSPKG_CMD_COINBUY();
            msg.stPkgData.stCoinBuyReq.bBuyType = (byte) BuyCoinType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
        {
            this.CurBuyTime = base.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[0];
        }

        public void unInit()
        {
            this.m_InfoPanel = null;
        }

        private void updateButtonState()
        {
            if (!base.bTimesOut)
            {
                if (base.m_FormScript != null)
                {
                    GameObject widget = base.m_FormScript.GetWidget(0);
                    if ((widget != null) && !widget.activeSelf)
                    {
                        widget.CustomSetActive(true);
                    }
                    GameObject obj3 = base.m_FormScript.GetWidget(1);
                    if ((obj3 != null) && obj3.activeSelf)
                    {
                        obj3.CustomSetActive(false);
                    }
                }
                base.showVipButton(false);
            }
            else
            {
                if (base.m_FormScript != null)
                {
                    GameObject obj4 = base.m_FormScript.GetWidget(0);
                    if ((obj4 != null) && obj4.activeSelf)
                    {
                        obj4.CustomSetActive(false);
                    }
                    GameObject obj5 = base.m_FormScript.GetWidget(1);
                    if ((obj5 != null) && !obj5.activeSelf)
                    {
                        obj5.CustomSetActive(true);
                    }
                }
                base.showVipButton(true);
            }
            this.refreshButtonState(10);
        }

        public override bool bCanBuy
        {
            get
            {
                return (base.IsHaveEnoughTimes(1) && base.IsHaveEnoughDianQuan(1));
            }
        }

        public bool bCanBuyTen
        {
            get
            {
                return (base.IsHaveEnoughTimes(10) && base.IsHaveEnoughDianQuan(10));
            }
        }

        public override int CurBuyTime
        {
            get
            {
                return base.m_CurBuyTime;
            }
            set
            {
                if (value <= base.m_MaxBuyTime)
                {
                    base.m_CurBuyTime = value;
                    this.calcComsume();
                    this.RefreshUI();
                }
            }
        }

        public string TitleDescribe
        {
            get
            {
                return string.Format("购买金币({0}/{1})", base.m_CurBuyTime, base.m_MaxBuyTime);
            }
        }

        public enum enBuyCoinPanelWidget
        {
            Action_Panel,
            Notice_Panel
        }
    }
}

