namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BuffClearRule
    {
        private BuffHolderComponent buffHolder;
        private List<int> CacheBufferList = new List<int>();
        private List<BuffSkill> TempBuffList = new List<BuffSkill>(4);

        public void CacheClearBuff(BuffSkill _buff, RES_SKILLFUNC_CLEAR_RULE _rule)
        {
            if ((_buff.cfgData.dwClearRule == ((long) _rule)) && (_buff.cfgData.dwEffectType == 3))
            {
                this.CacheBufferList.Add(_buff.cfgData.iCfgID);
            }
        }

        public void CheckBuffClear(RES_SKILLFUNC_CLEAR_RULE _rule)
        {
            if (this.buffHolder.SpawnedBuffList.Count != 0)
            {
                this.CopyList(this.buffHolder.SpawnedBuffList, this.TempBuffList);
                for (int i = 0; i < this.TempBuffList.Count; i++)
                {
                    BuffSkill skill = this.TempBuffList[i];
                    if (skill.cfgData.dwClearRule == ((long) _rule))
                    {
                        skill.Stop();
                    }
                }
                this.TempBuffList.Clear();
            }
        }

        public void CheckBuffNoClear(RES_SKILLFUNC_CLEAR_RULE _rule)
        {
            if (this.buffHolder.SpawnedBuffList.Count != 0)
            {
                this.CopyList(this.buffHolder.SpawnedBuffList, this.TempBuffList);
                for (int i = 0; i < this.TempBuffList.Count; i++)
                {
                    BuffSkill skill = this.TempBuffList[i];
                    if ((skill.cfgData.dwClearRule != ((long) _rule)) && (skill.cfgData.dwEffectType != 3))
                    {
                        skill.Stop();
                    }
                }
                this.TempBuffList.Clear();
            }
        }

        private void CopyList(List<BuffSkill> Source, List<BuffSkill> Dest)
        {
            Dest.Clear();
            for (int i = 0; i < Source.Count; i++)
            {
                Dest.Add(Source[i]);
            }
        }

        public void Init(BuffHolderComponent _buffHolder)
        {
            this.buffHolder = _buffHolder;
        }

        public void RecoverClearBuff()
        {
            int inSkillCombineId = 0;
            for (int i = 0; i < this.CacheBufferList.Count; i++)
            {
                inSkillCombineId = this.CacheBufferList[i];
                SkillUseParam inParam = new SkillUseParam();
                inParam.SetOriginator(this.buffHolder.actorPtr);
                this.buffHolder.actor.SkillControl.SpawnBuff(this.buffHolder.actorPtr, ref inParam, inSkillCombineId, false);
            }
            this.CacheBufferList.Clear();
        }
    }
}

