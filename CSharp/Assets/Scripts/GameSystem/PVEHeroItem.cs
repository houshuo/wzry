namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using System;
    using UnityEngine;

    internal class PVEHeroItem : PVEExpItemBase
    {
        private CHeroInfo heroInfo;
        private uint m_HeroId;

        public PVEHeroItem(GameObject heroItem, uint heroId)
        {
            base.m_Root = heroItem;
            this.m_HeroId = heroId;
            if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfoDic().TryGetValue(heroId, out this.heroInfo))
            {
                DebugHelper.Assert(false);
            }
            else
            {
                base.m_Name = StringHelper.UTF8BytesToString(ref this.heroInfo.cfgInfo.szName);
                base.m_NameText = base.m_Root.transform.Find("Name").GetComponent<Text>();
                base.m_LevelTxt = base.m_Root.transform.Find("Lv").GetComponent<Text>();
                base.m_ExpTxt = base.m_Root.transform.Find("Exp_Bar/Bar_Value").GetComponent<Text>();
                base.m_ExpBar1 = base.m_Root.transform.Find("Exp_Bar/Bar_Img").GetComponent<Image>();
            }
        }

        public override void addExp(uint addVal)
        {
            CRoleInfo.GetHeroPreLevleAndExp(this.m_HeroId, addVal, out this.m_level, out this.m_exp);
            base.addExp(addVal);
        }

        protected override uint calcMaxExp()
        {
            return GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint) base.m_level).dwExp;
        }

        protected override void SetUI()
        {
            base.SetUI();
            for (int i = 0; i < 5; i++)
            {
                GameObject gameObject = base.m_Root.transform.Find(string.Format("starPanel/imageStar{0}", i)).gameObject;
                if (this.heroInfo.mActorValue.actorStar > i)
                {
                    gameObject.CustomSetActive(true);
                }
                else
                {
                    gameObject.CustomSetActive(false);
                }
            }
        }
    }
}

