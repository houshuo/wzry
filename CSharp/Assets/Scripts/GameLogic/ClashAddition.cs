namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class ClashAddition
    {
        private DictionaryView<uint, DictionaryView<uint, ResClashAddition>> _dataDict;

        public int CalcDamageAddition(uint attackerMark, uint suffererMark)
        {
            if ((this._dataDict != null) && this._dataDict.ContainsKey(attackerMark))
            {
                DictionaryView<uint, ResClashAddition> view = this._dataDict[attackerMark];
                if (view.ContainsKey(suffererMark))
                {
                    ResClashAddition addition = view[suffererMark];
                    return Singleton<BattleLogic>.GetInstance().dynamicProperty.GetDynamicDamage(addition.dwDynamicConfig, (int) addition.dwDamageAddition);
                }
            }
            return 0x2710;
        }

        public void FightOver()
        {
            this._dataDict = null;
        }

        public void FightStart()
        {
            this._dataDict = new DictionaryView<uint, DictionaryView<uint, ResClashAddition>>();
            object[] rawDatas = GameDataMgr.clashAdditionDB.RawDatas;
            for (int i = 0; i < rawDatas.Length; i++)
            {
                DictionaryView<uint, ResClashAddition> view = null;
                ResClashAddition addition = rawDatas[i] as ResClashAddition;
                if (!this._dataDict.ContainsKey(addition.dwAttackerMark))
                {
                    view = new DictionaryView<uint, ResClashAddition>();
                    this._dataDict.Add(addition.dwAttackerMark, view);
                }
                else
                {
                    view = this._dataDict[addition.dwAttackerMark];
                }
                if (!view.ContainsKey(addition.dwSuffererMark))
                {
                    view.Add(addition.dwSuffererMark, addition);
                }
            }
        }
    }
}

