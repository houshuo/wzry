namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct HurtAttackerInfo
    {
        public int iActorLvl;
        public int iActorATT;
        public int iActorINT;
        public int iActorMaxHp;
        public int iDEFStrike;
        public int iRESStrike;
        public int iDEFStrikeRate;
        public int iRESStrikeRate;
        public int iFinalHurt;
        public int iCritStrikeRate;
        public int iCritStrikeValue;
        public int iReduceCritStrikeRate;
        public int iReduceCritStrikeValue;
        public int iCritStrikeEff;
        public int iPhysicsHemophagiaRate;
        public int iMagicHemophagiaRate;
        public int iPhysicsHemophagia;
        public int iMagicHemophagia;
        public int iHurtOutputRate;
        public ActorTypeDef actorType;
        public void Init(PoolObjHandle<ActorRoot> _atker, PoolObjHandle<ActorRoot> _target)
        {
            if (_atker != 0)
            {
                this.iActorLvl = _atker.handle.ValueComponent.mActorValue.actorLvl;
                this.iActorATT = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
                this.iActorINT = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
                this.iActorMaxHp = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                this.iDEFStrike = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYARMORHURT].totalValue;
                this.iRESStrike = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCARMORHURT].totalValue;
                this.iDEFStrikeRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE].totalValue;
                this.iRESStrikeRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MGCARMORHURT_RATE].totalValue;
                this.iFinalHurt = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURT].totalValue;
                this.iCritStrikeRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE].totalValue;
                this.iCritStrikeValue = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_CRITICAL].totalValue;
                this.iCritStrikeEff = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITEFT].totalValue;
                this.iMagicHemophagia = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAGICHEM].totalValue;
                this.iPhysicsHemophagia = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYSICSHEM].totalValue;
                this.iMagicHemophagiaRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCVAMP].totalValue;
                this.iPhysicsHemophagiaRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP].totalValue;
                this.iHurtOutputRate = _atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE].totalValue;
                this.actorType = _atker.handle.TheActorMeta.ActorType;
            }
            else if (_target != 0)
            {
                this.iReduceCritStrikeRate = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ANTICRIT].totalValue;
                this.iReduceCritStrikeValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_REDUCECRITICAL].totalValue;
            }
        }
    }
}

