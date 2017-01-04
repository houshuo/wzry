namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    public class CBattleSelectTarget : Singleton<CBattleSelectTarget>
    {
        private int curAd;
        private int curAp;
        private int curMaxHp;
        private int curMgDef;
        private int curPhyDef;
        private bool IsOpenForm;

        public void CloseForm()
        {
            this.IsOpenForm = false;
            this.curMaxHp = 0;
            this.curAd = 0;
            this.curAp = 0;
            this.curPhyDef = 0;
            this.curMgDef = 0;
        }

        private bool IsChangeAd(PoolObjHandle<ActorRoot> _target)
        {
            CrypticInt32 totalValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
            if (this.curAd != totalValue)
            {
                this.curAd = (int) totalValue;
                return true;
            }
            return false;
        }

        private bool IsChangeAp(PoolObjHandle<ActorRoot> _target)
        {
            CrypticInt32 totalValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
            if (this.curAp != totalValue)
            {
                this.curAp = (int) totalValue;
                return true;
            }
            return false;
        }

        private bool IsChangeMaxHp(PoolObjHandle<ActorRoot> _target)
        {
            CrypticInt32 totalValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
            if (this.curMaxHp != totalValue)
            {
                this.curMaxHp = (int) totalValue;
                return true;
            }
            return false;
        }

        private bool IsChangeMgDef(PoolObjHandle<ActorRoot> _target)
        {
            CrypticInt32 totalValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
            if (this.curMgDef != totalValue)
            {
                this.curMgDef = (int) totalValue;
                return true;
            }
            return false;
        }

        private bool IsChangePhyDef(PoolObjHandle<ActorRoot> _target)
        {
            CrypticInt32 totalValue = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
            if (this.curPhyDef != totalValue)
            {
                this.curPhyDef = (int) totalValue;
                return true;
            }
            return false;
        }

        public void OpenForm(PoolObjHandle<ActorRoot> _target)
        {
            if (_target != 0)
            {
                this.curMaxHp = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                this.curAd = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
                this.curAp = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
                this.curPhyDef = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
                this.curMgDef = _target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
                this.IsOpenForm = true;
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateHpInfo();
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateEpInfo();
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateAdValueInfo();
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateApValueInfo();
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdatePhyDefValueInfo();
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateMgcDefValueInfo();
            }
        }

        public void Update(PoolObjHandle<ActorRoot> _target)
        {
            if (this.IsOpenForm && (_target != 0))
            {
                if (this.IsChangeMaxHp(_target))
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.UpdateHpInfo();
                }
                if (this.IsChangeAd(_target))
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.UpdateAdValueInfo();
                }
                if (this.IsChangeAp(_target))
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.UpdateApValueInfo();
                }
                if (this.IsChangePhyDef(_target))
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.UpdatePhyDefValueInfo();
                }
                if (this.IsChangeMgDef(_target))
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.UpdateMgcDefValueInfo();
                }
            }
        }
    }
}

