namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine.UI;

    internal class CBuyActPtPanel : CBuyPanelBase
    {
        private Text m_content;
        private int m_CostDiamond;
        private int m_GainActPt;

        public void Buy()
        {
            if (this.bCanBuy)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
                msg.stPkgData.stShopBuyReq = new CSPKG_CMD_SHOPBUY();
                msg.stPkgData.stShopBuyReq.iBuyType = 4;
                msg.stPkgData.stShopBuyReq.iBuySubType = this.CurBuyTime + 1;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            else if (!base.IsHaveEnoughDianQuan(1))
            {
                CUICommonSystem.OpenDianQuanNotEnoughTip();
            }
            else if (this.willOverMaxAP(1))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("将超过体力最大上限值！", false, 1.5f, null, new object[0]);
            }
        }

        public override void BuyRsp(CSPkg msg)
        {
            base.BuyRsp(msg);
        }

        private void calcComsume()
        {
            ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) ((400 + base.m_CurBuyTime) + 1)));
            if (dataByKey != null)
            {
                this.m_CostDiamond = (int) dataByKey.dwCoinPrice;
                this.m_GainActPt = (int) dataByKey.dwValue;
            }
            else
            {
                this.m_CostDiamond = 0;
                this.m_GainActPt = 0;
            }
        }

        protected override uint GetRequireDianquan(int times)
        {
            uint num = 0;
            uint dwCoinPrice = 0;
            for (int i = 0; i < times; i++)
            {
                dwCoinPrice = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) (((400 + base.m_CurBuyTime) + 1) + i))).dwCoinPrice;
                if ((dwCoinPrice + num) > 0xffffffffL)
                {
                    return uint.MaxValue;
                }
                num += dwCoinPrice;
            }
            return num;
        }

        public void init()
        {
            base.FormPath = "UGUI/Form/System/Purchase/Form_BuyActionPoint.prefab";
            base.m_MaxBuyTime = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x17).dwConfValue;
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new System.Action(this.onAtrrChange));
        }

        public override void initPanel(CUIFormScript form)
        {
            base.initPanel(form);
            this.m_content = base.m_FormScript.transform.Find("Content").GetComponent<Text>();
            base.m_Buttons = new GameObject[] { base.m_FormScript.transform.Find("ButtonGroup/Button_Confirm").gameObject, base.m_FormScript.transform.Find("ButtonGroup/Button_Cancel").gameObject };
            base.m_CheckRightButton = base.m_FormScript.transform.Find("ButtonGroup/Button_CheckRight").gameObject;
            this.RefreshUI();
        }

        private void onAtrrChange()
        {
            this.RefreshUI();
        }

        private void RefreshUI()
        {
            if (base.bOpen && !base.bShopping)
            {
                if (!base.bTimesOut)
                {
                    string[] args = new string[] { this.m_CostDiamond.ToString(), this.m_GainActPt.ToString(), this.CurBuyTime.ToString() };
                    this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_Confirm", args);
                }
                else
                {
                    string[] textArray2 = new string[] { this.CurBuyTime.ToString(), this.m_MaxBuyTime.ToString() };
                    this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_NoMore", textArray2);
                }
                this.updateButtonState();
            }
        }

        public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
        {
            this.CurBuyTime = base.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[1];
        }

        public void unInit()
        {
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("MasterAttributesChanged", new System.Action(this.onAtrrChange));
        }

        private void updateButtonState()
        {
            if (!base.bTimesOut)
            {
                base.showVipButton(false);
            }
            else
            {
                base.showVipButton(true);
            }
        }

        protected bool willOverMaxAP(int times)
        {
            int num = 0;
            for (int i = 0; i < times; i++)
            {
                num += (int) GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) ((400 + base.m_CurBuyTime) + 1))).dwValue;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 3).dwConfValue;
            return ((masterRoleInfo.CurActionPoint + num) > dwConfValue);
        }

        public override bool bCanBuy
        {
            get
            {
                return ((base.IsHaveEnoughTimes(1) && base.IsHaveEnoughDianQuan(1)) && !this.willOverMaxAP(1));
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
    }
}

