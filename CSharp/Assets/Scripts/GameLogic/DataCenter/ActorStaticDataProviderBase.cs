namespace Assets.Scripts.GameLogic.DataCenter
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    internal class ActorStaticDataProviderBase : ActorDataProviderBase
    {
        private readonly DictionaryView<uint, ActorDataBuilder> _actorDataBuilder = new DictionaryView<uint, ActorDataBuilder>();
        private readonly DictionaryView<uint, ActorPerLvDataBuilder> _actorPerStarLvDataBuilder = new DictionaryView<uint, ActorPerLvDataBuilder>();
        private readonly DictionaryView<uint, ActorSkillDataBuilder> _actorSkillDataBuilder = new DictionaryView<uint, ActorSkillDataBuilder>();

        public ActorStaticDataProviderBase()
        {
            this._actorDataBuilder.Add(0, new ActorDataBuilder(this.BuildHeroData));
            this._actorDataBuilder.Add(1, new ActorDataBuilder(this.BuildMonsterData));
            this._actorDataBuilder.Add(2, new ActorDataBuilder(this.BuildOrganData));
            this._actorDataBuilder.Add(3, new ActorDataBuilder(this.BuildMonsterData));
            this._actorSkillDataBuilder.Add(0, new ActorSkillDataBuilder(this.BuildHeroSkillData));
            this._actorSkillDataBuilder.Add(1, new ActorSkillDataBuilder(this.BuildMonsterSkillData));
            this._actorSkillDataBuilder.Add(2, new ActorSkillDataBuilder(this.BuildOrganSkillData));
            this._actorSkillDataBuilder.Add(3, new ActorSkillDataBuilder(this.BuildMonsterSkillData));
            this._actorPerStarLvDataBuilder.Add(0, new ActorPerLvDataBuilder(this.BuildHeroPerStarLvData));
            this._actorPerStarLvDataBuilder.Add(1, new ActorPerLvDataBuilder(this.BuildMonsterPerStarLvData));
            this._actorPerStarLvDataBuilder.Add(2, new ActorPerLvDataBuilder(this.BuildOrganPerStarLvData));
            this._actorPerStarLvDataBuilder.Add(3, new ActorPerLvDataBuilder(this.BuildMonsterPerStarLvData));
        }

        protected virtual bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            return false;
        }

        protected virtual bool BuildHeroPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) perStarLvData.TheActorMeta.ConfigId);
            if (dataByKey == null)
            {
                base.ErrorMissingHeroConfig((uint) perStarLvData.TheActorMeta.ConfigId);
                return false;
            }
            perStarLvData.PerLvHp = (int) dataByKey.iHpGrowth;
            perStarLvData.PerLvAd = (int) dataByKey.iAtkGrowth;
            perStarLvData.PerLvAp = (int) dataByKey.iSpellGrowth;
            perStarLvData.PerLvDef = (int) dataByKey.iDefGrowth;
            perStarLvData.PerLvRes = (int) dataByKey.iResistGrowth;
            return true;
        }

        protected virtual bool BuildHeroSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
        {
            int index = (int) skillSlot;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) skillData.TheActorMeta.ConfigId);
            if (dataByKey == null)
            {
                base.ErrorMissingHeroConfig((uint) skillData.TheActorMeta.ConfigId);
                return false;
            }
            if (index >= dataByKey.astSkill.Length)
            {
                return false;
            }
            skillData.PassiveSkillId = dataByKey.astSkill[index].iPassiveSkillID;
            skillData.SkillId = dataByKey.astSkill[index].iSkillID;
            return (skillData.SkillId > 0);
        }

        protected virtual bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            return false;
        }

        protected virtual bool BuildMonsterPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
        {
            return false;
        }

        protected virtual bool BuildMonsterSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
        {
            int index = (int) skillSlot;
            ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(skillData.TheActorMeta.ConfigId, skillData.TheActorMeta.Difficuty);
            if (dataCfgInfo == null)
            {
                base.ErrorMissingMonsterConfig((uint) skillData.TheActorMeta.ConfigId);
                return false;
            }
            if (index >= dataCfgInfo.SkillIDs.Length)
            {
                return false;
            }
            skillData.SkillId = dataCfgInfo.SkillIDs[index];
            skillData.PassiveSkillId = dataCfgInfo.PassiveSkillID[index];
            return ((skillData.SkillId > 0) || (skillData.PassiveSkillId > 0));
        }

        protected virtual bool BuildOrganData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            return false;
        }

        protected virtual bool BuildOrganPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
        {
            ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(perStarLvData.TheActorMeta.ConfigId);
            if (dataCfgInfoByCurLevelDiff == null)
            {
                base.ErrorMissingOrganConfig((uint) perStarLvData.TheActorMeta.ConfigId);
                return false;
            }
            perStarLvData.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
            perStarLvData.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
            perStarLvData.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
            perStarLvData.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
            perStarLvData.PerLvRes = dataCfgInfoByCurLevelDiff.iRESLvlup;
            return true;
        }

        protected virtual bool BuildOrganSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
        {
            int index = (int) skillSlot;
            ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(skillData.TheActorMeta.ConfigId);
            if (dataCfgInfoByCurLevelDiff == null)
            {
                base.ErrorMissingOrganConfig((uint) skillData.TheActorMeta.ConfigId);
                return false;
            }
            if (index >= dataCfgInfoByCurLevelDiff.SkillIDs.Length)
            {
                return false;
            }
            skillData.SkillId = dataCfgInfoByCurLevelDiff.SkillIDs[index];
            skillData.PassiveSkillId = 0;
            return (skillData.SkillId > 0);
        }

        public override bool GetActorStaticData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            actorData.TheActorMeta = actorMeta;
            ActorDataBuilder builder = null;
            this._actorDataBuilder.TryGetValue((uint) actorMeta.ActorType, out builder);
            return ((builder != null) && builder(ref actorMeta, ref actorData));
        }

        public override bool GetActorStaticPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData)
        {
            perStarLvData.TheActorMeta = actorMeta;
            perStarLvData.StarLv = starLv;
            ActorPerLvDataBuilder builder = null;
            this._actorPerStarLvDataBuilder.TryGetValue((uint) actorMeta.ActorType, out builder);
            return ((builder != null) && builder(ref actorMeta, starLv, ref perStarLvData));
        }

        public override bool GetActorStaticSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData)
        {
            skillData.TheActorMeta = actorMeta;
            skillData.SkillSlot = skillSlot;
            ActorSkillDataBuilder builder = null;
            this._actorSkillDataBuilder.TryGetValue((uint) actorMeta.ActorType, out builder);
            return ((builder != null) && builder(ref actorMeta, skillSlot, ref skillData));
        }

        protected delegate bool ActorDataBuilder(ref ActorMeta actorMeta, ref ActorStaticData actorInfo);

        protected delegate bool ActorPerLvDataBuilder(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData);

        protected delegate bool ActorSkillDataBuilder(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillInfo);
    }
}

