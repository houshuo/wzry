namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class DynamicProperty : IComparer<object>
    {
        private ResBattleDynamicProperty _cprItem;
        private DictionaryView<uint, List<object>> _databin;
        private DictionaryView<uint, UpdatePropertyList> _updateDatabin;
        private uint dynamicPropertyConfig;
        private ulong lastSystemTime;

        public static int Adjustor(ValueDataInfo vdi)
        {
            int baseValue = vdi.baseValue;
            if (vdi.dynamicId < 1)
            {
                return baseValue;
            }
            ResBattleDynamicProperty config = Singleton<BattleLogic>.instance.dynamicProperty.GetConfig((uint) vdi.dynamicId, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
            int iAD = 0x2710;
            if (config != null)
            {
                switch (vdi.Type)
                {
                    case RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT:
                        iAD = config.iAD;
                        goto Label_00A7;

                    case RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT:
                        iAD = config.iAP;
                        goto Label_00A7;

                    case RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT:
                        iAD = config.iDef;
                        goto Label_00A7;

                    case RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT:
                        iAD = config.iRes;
                        goto Label_00A7;

                    case RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP:
                        iAD = config.iHP;
                        goto Label_00A7;
                }
                iAD = 0x2710;
            }
        Label_00A7:
            return ((baseValue * iAD) / 0x2710);
        }

        public int Compare(object l, object r)
        {
            return (int) (((ResBattleDynamicProperty) l).dwVarPara1 - ((ResBattleDynamicProperty) r).dwVarPara1);
        }

        public void FightOver()
        {
            this._databin = null;
            this._cprItem = null;
        }

        public void FightStart()
        {
            this._cprItem = new ResBattleDynamicProperty();
            this._databin = new DictionaryView<uint, List<object>>();
            this._updateDatabin = new DictionaryView<uint, UpdatePropertyList>();
            this.lastSystemTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
            this.ResetTimer();
            GameDataMgr.battleDynamicPropertyDB.Accept(new Action<ResBattleDynamicProperty>(this.OnVisit));
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext != null)
            {
                this.dynamicPropertyConfig = curLvelContext.m_dynamicPropertyConfig;
            }
        }

        public ResBattleDynamicProperty GetConfig(uint id, RES_BATTLE_DYNAMIC_PROPERTY_VAR dynVar)
        {
            if (((id == 0) || (this._databin == null)) || !this._databin.ContainsKey(id))
            {
                return null;
            }
            ResBattleDynamicProperty property = null;
            List<object> list = this._databin[id];
            if (dynVar != RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR)
            {
                return property;
            }
            this._cprItem.dwVarPara1 = this.m_frameTimer;
            int num = list.BinarySearch(this._cprItem, this);
            if (num < 0)
            {
                num = ~num;
                if (num > 0)
                {
                    return (ResBattleDynamicProperty) list[num - 1];
                }
                if (num != 0)
                {
                    return property;
                }
                return (ResBattleDynamicProperty) list[num];
            }
            return (ResBattleDynamicProperty) list[num];
        }

        public int GetDynamicDamage(uint id, int baseValue)
        {
            ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
            if (config != null)
            {
                baseValue = (baseValue * config.iDamage) / 0x2710;
            }
            return baseValue;
        }

        public uint GetDynamicGoldCoinInBattle(uint id, int baseValue, int floatRange)
        {
            ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
            if (config != null)
            {
                baseValue = (baseValue * config.wGoldCoinInBattleIncreaseRate) / 0x2710;
            }
            int num = 0;
            if (floatRange > 0)
            {
                num = FrameRandom.Random((uint) ((floatRange * 2) + 1)) - floatRange;
            }
            int num2 = baseValue + num;
            if (num2 < 0)
            {
                num2 = 0;
            }
            return (uint) num2;
        }

        public int GetDynamicReviveTime(uint id, int baseValue)
        {
            ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
            if (config != null)
            {
                baseValue = (baseValue * config.iReviveTime) / 0x2710;
            }
            return baseValue;
        }

        public uint GetDynamicSoulExp(uint id, int baseValue)
        {
            ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
            if (config != null)
            {
                baseValue = (baseValue * config.iSoulExp) / 0x2710;
            }
            return (uint) baseValue;
        }

        private void OnVisit(ResBattleDynamicProperty InProperty)
        {
            uint dwID = InProperty.dwID;
            if (InProperty.bVarType == 1)
            {
                List<object> list = null;
                if (this._databin.ContainsKey(dwID))
                {
                    list = this._databin[dwID];
                }
                else
                {
                    list = new List<object>();
                    this._databin.Add(dwID, list);
                }
                list.Add(InProperty);
            }
            else if (InProperty.bVarType == 2)
            {
                UpdatePropertyList list2;
                if (this._updateDatabin.ContainsKey(dwID))
                {
                    list2 = this._updateDatabin[dwID];
                }
                else
                {
                    list2 = new UpdatePropertyList {
                        deltaTime = 0,
                        propertyList = new List<object>()
                    };
                    this._updateDatabin.Add(dwID, list2);
                }
                list2.propertyList.Add(InProperty);
            }
        }

        public void ResetTimer()
        {
            this.m_frameTimer = 0;
        }

        public void UpdateActorProperty(ref ResBattleDynamicProperty _property)
        {
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
            for (int i = 0; i < heroActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                if ((_property.iSoulExp != 0) && (handle != 0))
                {
                    handle.handle.ValueComponent.AddSoulExp(_property.iSoulExp, false, AddSoulType.Dynamic);
                    Vector3 position = new Vector3();
                    handle.handle.ValueComponent.ChangeGoldCoinInBattle(_property.bGoldCoinInBattleAutoIncreaseValue, true, false, position, false);
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            this.m_frameTimer += (uint) delta;
            if (((this.dynamicPropertyConfig != 0) && (this._updateDatabin != null)) && (this._updateDatabin.ContainsKey(this.dynamicPropertyConfig) && Singleton<BattleLogic>.GetInstance().isFighting))
            {
                UpdatePropertyList list = this._updateDatabin[this.dynamicPropertyConfig];
                for (int i = 0; i < list.propertyList.Count; i++)
                {
                    ResBattleDynamicProperty property = (ResBattleDynamicProperty) list.propertyList[i];
                    ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    if ((logicFrameTick < property.dwVarPara1) || (i == (list.propertyList.Count - 1)))
                    {
                        list.deltaTime += (uint) (logicFrameTick - this.lastSystemTime);
                        this.lastSystemTime = logicFrameTick;
                        if (list.deltaTime >= property.dwVarPara2)
                        {
                            list.deltaTime -= property.dwVarPara2;
                            this.UpdateActorProperty(ref property);
                        }
                        return;
                    }
                }
            }
        }

        public uint m_frameTimer { get; private set; }

        private class UpdatePropertyList
        {
            public uint deltaTime;
            public List<object> propertyList;
        }
    }
}

