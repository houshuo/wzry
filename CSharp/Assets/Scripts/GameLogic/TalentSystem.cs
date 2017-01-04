namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class TalentSystem
    {
        private PoolObjHandle<ActorRoot> actor = new PoolObjHandle<ActorRoot>();
        private int talentCount;
        private PassiveSkill[] TalentObjArray = new PassiveSkill[10];

        public void ChangePassiveParam(int _id, int _index, int _value)
        {
            for (int i = 0; i < 10; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if ((skill != null) && (skill.SkillID == _id))
                {
                    skill.ChangeEventParam(_index, _value);
                }
            }
        }

        public int GetTalentCDTime(int _talentID)
        {
            for (int i = 0; i < 10; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if ((skill != null) && (skill.SkillID == _talentID))
                {
                    return skill.GetCDTime();
                }
            }
            return 0;
        }

        public void Init(PoolObjHandle<ActorRoot> _actor)
        {
            this.actor = _actor;
            this.talentCount = 0;
            for (int i = 0; i < 10; i++)
            {
                this.TalentObjArray[i] = null;
            }
        }

        public void InitTalent(int _talentID)
        {
            if (this.talentCount < 10)
            {
                PassiveSkill skill = new PassiveSkill(_talentID, this.actor);
                for (int i = 0; i < 10; i++)
                {
                    if (this.TalentObjArray[i] == null)
                    {
                        this.TalentObjArray[i] = skill;
                        this.talentCount++;
                        return;
                    }
                }
            }
        }

        public void InitTalent(int _talentID, int _cdTime)
        {
            if (this.talentCount < 10)
            {
                PassiveSkill skill = new PassiveSkill(_talentID, this.actor);
                for (int i = 0; i < 10; i++)
                {
                    if (this.TalentObjArray[i] == null)
                    {
                        this.TalentObjArray[i] = skill;
                        this.talentCount++;
                        skill.InitCDTime(_cdTime);
                        return;
                    }
                }
            }
        }

        public PassiveSkill[] QueryTalents()
        {
            return this.TalentObjArray;
        }

        public void RemoveTalent(int _talentID)
        {
            for (int i = 0; i < 10; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if ((skill != null) && (skill.SkillID == _talentID))
                {
                    if (skill.passiveEvent != null)
                    {
                        skill.passiveEvent.UnInit();
                    }
                    this.TalentObjArray[i] = null;
                    this.talentCount--;
                }
            }
        }

        public void Reset()
        {
            this.actor.Validate();
            for (int i = 0; i < 10; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if (skill != null)
                {
                    skill.Reset();
                }
            }
        }

        public void UnInit()
        {
            for (int i = 0; i < 10; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if ((skill != null) && (skill.passiveEvent != null))
                {
                    skill.passiveEvent.UnInit();
                }
            }
        }

        public void UpdateLogic(int nDelta)
        {
            for (int i = 0; i < this.talentCount; i++)
            {
                PassiveSkill skill = this.TalentObjArray[i];
                if (skill != null)
                {
                    skill.UpdateLogic(nDelta);
                }
            }
        }
    }
}

