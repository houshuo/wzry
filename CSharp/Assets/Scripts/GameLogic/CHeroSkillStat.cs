namespace Assets.Scripts.GameLogic
{
    using System;

    public class CHeroSkillStat
    {
        public void Clear()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
        }

        private void OnActorBuffSkillChange(ref BuffChangeEventParam prm)
        {
            if (((((!prm.bIsAdd && ((prm.stBuffSkill != 0) && (prm.stBuffSkill.handle.skillContext != null))) && ((prm.stBuffSkill.handle.skillContext.Originator != 0) && (prm.stBuffSkill.handle.skillContext.TargetActor != 0))) && ((prm.stBuffSkill.handle.skillContext.SlotType >= SkillSlotType.SLOT_SKILL_1) && (prm.stBuffSkill.handle.skillContext.SlotType < SkillSlotType.SLOT_SKILL_COUNT))) && (prm.stBuffSkill.handle.cfgData.dwEffectType == 2)) && ((((prm.stBuffSkill.handle.cfgData.dwShowType == 1) || (prm.stBuffSkill.handle.cfgData.dwShowType == 3)) || ((prm.stBuffSkill.handle.cfgData.dwShowType == 4) || (prm.stBuffSkill.handle.cfgData.dwShowType == 5))) || (prm.stBuffSkill.handle.cfgData.dwShowType == 6)))
            {
                ulong num = Singleton<FrameSynchr>.GetInstance().LogicFrameTick - prm.stBuffSkill.handle.ulStartTime;
                if (prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl != null)
                {
                    prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl.stSkillStat.m_uiStunTime += (uint) num;
                }
                if (prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl != null)
                {
                    prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl.stSkillStat.m_uiBeStunnedTime += (uint) num;
                }
            }
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
        }
    }
}

