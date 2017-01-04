namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    internal class CHeroInfoData : IHeroData
    {
        private string m_imgPath;
        public CHeroInfo m_info;
        private string m_name;
        private string m_tilte;

        public CHeroInfoData(CHeroInfo info)
        {
            DebugHelper.Assert(info != null, "Create CHeroInfoData, CHeroInfo = null");
            if (info != null)
            {
                this.m_info = info;
                this.m_name = StringHelper.UTF8BytesToString(ref info.cfgInfo.szName);
                this.m_imgPath = StringHelper.UTF8BytesToString(ref info.cfgInfo.szImagePath);
                this.m_tilte = StringHelper.UTF8BytesToString(ref info.cfgInfo.szHeroTitle);
            }
        }

        public bool IsExperienceHero()
        {
            return ((this.m_info != null) && this.m_info.IsExperienceHero());
        }

        public bool IsValidExperienceHero()
        {
            return ((this.m_info != null) && this.m_info.IsValidExperienceHero());
        }

        public ResHeroPromotion promotion()
        {
            if (this.m_info.shopCfgInfo == null)
            {
                GameDataMgr.heroShopInfoDict.TryGetValue(this.m_info.cfgInfo.dwCfgID, out this.m_info.shopCfgInfo);
                if (this.m_info.shopCfgInfo == null)
                {
                    return null;
                }
            }
            for (int i = 0; i < this.m_info.shopCfgInfo.bPromotionCnt; i++)
            {
                uint key = this.m_info.shopCfgInfo.PromotionID[i];
                if ((key != 0) && GameDataMgr.heroPromotionDict.ContainsKey(key))
                {
                    ResHeroPromotion promotion = new ResHeroPromotion();
                    if ((GameDataMgr.heroPromotionDict.TryGetValue(key, out promotion) && (promotion.dwOnTimeGen <= CRoleInfo.GetCurrentUTCTime())) && (promotion.dwOffTimeGen >= CRoleInfo.GetCurrentUTCTime()))
                    {
                        return promotion;
                    }
                }
            }
            return null;
        }

        public bool bIsPlayerUse
        {
            get
            {
                return GameDataMgr.IsHeroAvailable(this.m_info.cfgInfo.dwCfgID);
            }
        }

        public bool bPlayerOwn
        {
            get
            {
                return !this.IsExperienceHero();
            }
        }

        public uint cfgID
        {
            get
            {
                if ((this.m_info != null) && (this.m_info.cfgInfo != null))
                {
                    return this.m_info.cfgInfo.dwCfgID;
                }
                return 0;
            }
        }

        public int combatEft
        {
            get
            {
                return this.m_info.GetCombatEft();
            }
        }

        public int curExp
        {
            get
            {
                return this.m_info.mActorValue.actorExp;
            }
        }

        public ResHeroCfgInfo heroCfgInfo
        {
            get
            {
                return this.m_info.cfgInfo;
            }
        }

        public string heroName
        {
            get
            {
                return this.m_name;
            }
        }

        public string heroTitle
        {
            get
            {
                string tilte = this.m_tilte;
                if (this.m_info.m_skinInfo != null)
                {
                    uint wearSkinId = this.m_info.m_skinInfo.GetWearSkinId();
                    if (wearSkinId != 0)
                    {
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_info.cfgInfo.dwCfgID, wearSkinId);
                        if (heroSkin != null)
                        {
                            tilte = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
                        }
                    }
                }
                return tilte;
            }
        }

        public int heroType
        {
            get
            {
                return this.m_info.cfgInfo.bMainJob;
            }
        }

        public string imagePath
        {
            get
            {
                string imgPath = this.m_imgPath;
                if (this.m_info.m_skinInfo != null)
                {
                    uint wearSkinId = this.m_info.m_skinInfo.GetWearSkinId();
                    if (wearSkinId != 0)
                    {
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_info.cfgInfo.dwCfgID, wearSkinId);
                        if (heroSkin != null)
                        {
                            imgPath = StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID);
                        }
                    }
                }
                return imgPath;
            }
        }

        public int level
        {
            get
            {
                return this.m_info.mActorValue.actorLvl;
            }
        }

        public int maxExp
        {
            get
            {
                return this.m_info.mActorValue.actorMaxExp;
            }
        }

        public uint proficiency
        {
            get
            {
                return this.m_info.m_Proficiency;
            }
        }

        public byte proficiencyLV
        {
            get
            {
                return this.m_info.m_ProficiencyLV;
            }
        }

        public int quality
        {
            get
            {
                return this.m_info.mActorValue.actorQuality;
            }
        }

        public ResDT_SkillInfo[] skillArr
        {
            get
            {
                return this.m_info.cfgInfo.astSkill;
            }
        }

        public uint skinID
        {
            get
            {
                uint wearSkinId = 0;
                if (this.m_info.m_skinInfo != null)
                {
                    wearSkinId = this.m_info.m_skinInfo.GetWearSkinId();
                }
                return wearSkinId;
            }
        }

        public uint sortId
        {
            get
            {
                return this.m_info.cfgInfo.dwShowSortId;
            }
        }

        public int star
        {
            get
            {
                return this.m_info.mActorValue.actorStar;
            }
        }

        public int subQuality
        {
            get
            {
                return this.m_info.mActorValue.actorSubQuality;
            }
        }
    }
}

