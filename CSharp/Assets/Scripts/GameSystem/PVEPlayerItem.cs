namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    internal class PVEPlayerItem : PVEExpItemBase
    {
        public PVEPlayerItem(GameObject playerItem)
        {
            base.m_Root = playerItem;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            base.m_Name = masterRoleInfo.Name;
            base.m_NameText = base.m_Root.transform.Find("Name").GetComponent<Text>();
            base.m_LevelTxt = base.m_Root.transform.Find("Lv").GetComponent<Text>();
            base.m_ExpTxt = base.m_Root.transform.Find("Exp_Bar/Bar_Value").GetComponent<Text>();
            base.m_ExpBar1 = base.m_Root.transform.Find("Exp_Bar/Bar_Img").GetComponent<Image>();
            GameObject gameObject = base.m_Root.transform.Find("Player_Pic").gameObject;
            if ((gameObject != null) && !string.IsNullOrEmpty(masterRoleInfo.HeadUrl))
            {
                gameObject.GetComponent<CUIHttpImageScript>().SetImageUrl(masterRoleInfo.HeadUrl);
            }
        }

        public override void addExp(uint addVal)
        {
            CRoleInfo.GetPlayerPreLevleAndExp(addVal, out this.m_level, out this.m_exp);
            base.addExp(addVal);
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
        }

        protected override uint calcMaxExp()
        {
            return GameDataMgr.acntExpDatabin.GetDataByKey((uint) base.m_level).dwNeedExp;
        }

        protected override void TweenEnd(float val)
        {
            if (base.m_maxExp == base.m_exp)
            {
                if (base.Level >= GameDataMgr.acntExpDatabin.GetDataByKey(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Level).dwLimitHeroLevel)
                {
                    return;
                }
                CUIEvent uiEvent = new CUIEvent {
                    m_eventID = enUIEventID.Settle_OpenLvlUp
                };
                uiEvent.m_eventParams.tag = base.Level;
                uiEvent.m_eventParams.tag2 = base.Level + 1;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
            base.TweenEnd(val);
        }
    }
}

