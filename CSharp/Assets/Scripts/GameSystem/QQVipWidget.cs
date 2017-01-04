namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class QQVipWidget : Singleton<QQVipWidget>
    {
        private GameObject m_BtnQQ;

        private void BuyPcikQQ(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (masterRoleInfo.HasVip(1))
                {
                    Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", "续费会员", 1);
                }
                else if (!masterRoleInfo.HasVip(1))
                {
                    Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", "购买会员", 1);
                }
            }
        }

        private void BuyPcikQQVip(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (masterRoleInfo.HasVip(0x10))
                {
                    Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", "续费超级会员", 1);
                }
                else if (!masterRoleInfo.HasVip(0x10))
                {
                    Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", "购买超级会员", 1);
                }
            }
        }

        public void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_QQ, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQ));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_QQVIP, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQVip));
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_QQ, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQ));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_QQVIP, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQVip));
        }

        public void SetData(GameObject root, CUIFormScript formScript)
        {
            ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey((uint) 0xa029);
            for (int i = 0; i < 3; i++)
            {
                string name = string.Format("Panel/QQVip/AwardGrid/QQ/ListElement{0}/ItemCell", i);
                GameObject gameObject = root.transform.FindChild(name).gameObject;
                ResDT_RandomRewardInfo info = dataByKey.astRewardDetail[i];
                CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) info.bItemType, (int) info.dwLowCnt, info.dwItemID);
                if (itemUseable != null)
                {
                    if (gameObject.GetComponent<CUIEventScript>() == null)
                    {
                        gameObject.AddComponent<CUIEventScript>();
                    }
                    CUICommonSystem.SetItemCell(formScript, gameObject, itemUseable, true, false);
                }
            }
            ResRandomRewardStore store2 = GameDataMgr.randomRewardDB.GetDataByKey((uint) 0xa02a);
            for (int j = 0; j < 3; j++)
            {
                string str2 = string.Format("Panel/QQVip/AwardGrid/QQVip/ListElement{0}/ItemCell", j);
                GameObject itemCell = root.transform.FindChild(str2).gameObject;
                ResDT_RandomRewardInfo info2 = store2.astRewardDetail[j];
                CUseable useable2 = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) info2.bItemType, (int) info2.dwLowCnt, info2.dwItemID);
                if (useable2 != null)
                {
                    if (itemCell.GetComponent<CUIEventScript>() == null)
                    {
                        itemCell.AddComponent<CUIEventScript>();
                    }
                    CUICommonSystem.SetItemCell(formScript, itemCell, useable2, true, false);
                }
            }
            this.m_BtnQQ = root.transform.FindChild("Panel/QQVip/AwardGrid/QQ/Button/").gameObject;
            Text componentInChildren = root.transform.FindChild("Panel/QQVip/AwardGrid/QQVip/Button/").gameObject.GetComponentInChildren<Text>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (masterRoleInfo.HasVip(0x10))
                {
                    if (this.m_BtnQQ.activeInHierarchy)
                    {
                        this.m_BtnQQ.GetComponentInChildren<Text>().text = "续费QQ会员";
                    }
                    componentInChildren.text = "续费超级会员";
                }
                else if (masterRoleInfo.HasVip(1))
                {
                    if (this.m_BtnQQ.activeInHierarchy)
                    {
                        this.m_BtnQQ.GetComponentInChildren<Text>().text = "续费QQ会员";
                    }
                    componentInChildren.text = "开通超级会员";
                }
                else if (!masterRoleInfo.HasVip(1))
                {
                    if (this.m_BtnQQ.activeInHierarchy)
                    {
                        this.m_BtnQQ.GetComponentInChildren<Text>().text = "开通QQ会员";
                    }
                    componentInChildren.text = "开通超级会员";
                }
            }
        }

        public override void UnInit()
        {
            this.Clear();
        }
    }
}

