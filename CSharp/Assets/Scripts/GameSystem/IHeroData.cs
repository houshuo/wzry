namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    public interface IHeroData
    {
        bool IsValidExperienceHero();
        ResHeroPromotion promotion();

        bool bIsPlayerUse { get; }

        bool bPlayerOwn { get; }

        uint cfgID { get; }

        int combatEft { get; }

        int curExp { get; }

        ResHeroCfgInfo heroCfgInfo { get; }

        string heroName { get; }

        string heroTitle { get; }

        int heroType { get; }

        string imagePath { get; }

        int level { get; }

        int maxExp { get; }

        uint proficiency { get; }

        byte proficiencyLV { get; }

        int quality { get; }

        ResDT_SkillInfo[] skillArr { get; }

        uint skinID { get; }

        uint sortId { get; }

        int star { get; }

        int subQuality { get; }
    }
}

