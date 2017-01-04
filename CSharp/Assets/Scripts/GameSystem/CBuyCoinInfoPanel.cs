namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CBuyCoinInfoPanel
    {
        private int m_CostDiamond;
        private int m_GainCoin;
        private int m_InfoNum;
        private GameObject m_InfoPanel;
        private int m_LimitInfoNum = 100;
        private List<BuyCoinInfo> m_stInfos = new List<BuyCoinInfo>();

        public void addInfo(BuyCoinInfo info)
        {
            if (this.InfoNum < this.m_LimitInfoNum)
            {
                this.m_stInfos.Add(info);
                this.InfoNum++;
            }
            else
            {
                this.m_stInfos.RemoveAt(0);
                this.m_stInfos.Add(info);
            }
            this.refreshPanel();
        }

        public void BuyCoinRsp(CSPkg msg)
        {
            for (int i = 0; i < msg.stPkgData.stCoinBuyRsp.stBuyList.bCoinGetCnt; i++)
            {
                BuyCoinInfo info = new BuyCoinInfo();
                this.calcComsume(msg.stPkgData.stCoinBuyRsp.wBuyStartFreq + i);
                info.m_CostDiamond = this.m_CostDiamond;
                info.m_GainCoin = (int) msg.stPkgData.stCoinBuyRsp.stBuyList.CoinGetVal[i];
                info.m_CritTime = (int) (msg.stPkgData.stCoinBuyRsp.stBuyList.CoinGetVal[i] / ((this.m_GainCoin != 0) ? this.m_GainCoin : 1));
                this.addInfo(info);
            }
        }

        private void calcComsume(int BuyTime)
        {
            ResCoinBuyInfo dataByKey = GameDataMgr.coninBuyDatabin.GetDataByKey((uint) ((ushort) BuyTime));
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

        private string GetDescribeStr(BuyCoinInfo info)
        {
            string str = null;
            if (info.m_CritTime == 1)
            {
                str = string.Format("使用{0}钻石获得{1}金币", info.m_CostDiamond, info.m_GainCoin);
            }
            if (info.m_CritTime >= 2)
            {
                str = string.Format("使用{0}钻石获得{1}金币 暴击*{2}", info.m_CostDiamond, info.m_GainCoin, info.m_CritTime);
            }
            return str;
        }

        public void initInfoPanel(GameObject infoPanel)
        {
            this.m_InfoPanel = infoPanel;
            this.refreshPanel();
        }

        private void refreshPanel()
        {
            Transform transform = this.m_InfoPanel.transform.Find("List");
            DebugHelper.Assert(transform != null);
            CUIListScript script = (transform == null) ? null : transform.GetComponent<CUIListScript>();
            DebugHelper.Assert(script != null);
            if (script != null)
            {
                DebugHelper.Assert(this.m_stInfos != null);
                script.SetElementAmount(this.m_stInfos.Count);
                bool flag = script.IsElementInScrollArea(script.m_elementAmount - 1);
                for (int i = 0; i < this.m_stInfos.Count; i++)
                {
                    script.GetElemenet(i).transform.Find("Describe").GetComponent<Text>().text = this.GetDescribeStr(this.m_stInfos[i]);
                }
                if (flag)
                {
                    script.MoveElementInScrollArea(script.m_elementAmount - 1, false);
                }
            }
        }

        private int InfoNum
        {
            get
            {
                return this.m_InfoNum;
            }
            set
            {
                this.m_InfoNum = value;
            }
        }
    }
}

