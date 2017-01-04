namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [EventCategory("MMGame/Skill")]
    public class ChangeSkillTriggerTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int attackTargetId = -1;
        public bool bAbort;
        public bool bCheckCondition;
        public bool bOvertimeCD = true;
        public bool bSendEvent = true;
        public bool bUseStop = true;
        [AssetReference(AssetRefType.SkillID)]
        public int changeSkillID;
        public int changeSkillID1Probability = 100;
        [AssetReference(AssetRefType.SkillID)]
        public int changeSkillID2;
        public int changeSkillID2Probability = 100;
        [AssetReference(AssetRefType.SkillID)]
        public int changeSkillID3;
        public int changeSkillID3Probability = 100;
        [AssetReference(AssetRefType.SkillID)]
        public int changeSkillID4;
        public int changeSkillID4Probability = 100;
        public int effectTime;
        private static List<SelectionData> randomList = new List<SelectionData>(4);
        public int recoverSkillID;
        public SkillSlotType slotType;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        private bool CheckChangeSkillCondition(AGE.Action _action)
        {
            if (!this.bCheckCondition)
            {
                return true;
            }
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.attackTargetId);
            return ((actorHandle != 0) && actorHandle.handle.ActorControl.IsDeadState);
        }

        public override BaseEvent Clone()
        {
            ChangeSkillTriggerTick tick = ClassObjPool<ChangeSkillTriggerTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ChangeSkillTriggerTick tick = src as ChangeSkillTriggerTick;
            this.targetId = tick.targetId;
            this.slotType = tick.slotType;
            this.effectTime = tick.effectTime;
            this.changeSkillID = tick.changeSkillID;
            this.changeSkillID2 = tick.changeSkillID2;
            this.changeSkillID3 = tick.changeSkillID3;
            this.changeSkillID4 = tick.changeSkillID4;
            this.recoverSkillID = tick.recoverSkillID;
            this.changeSkillID1Probability = tick.changeSkillID1Probability;
            this.changeSkillID2Probability = tick.changeSkillID2Probability;
            this.changeSkillID3Probability = tick.changeSkillID3Probability;
            this.changeSkillID4Probability = tick.changeSkillID4Probability;
            this.bCheckCondition = tick.bCheckCondition;
            this.attackTargetId = tick.attackTargetId;
            this.bOvertimeCD = tick.bOvertimeCD;
            this.bSendEvent = tick.bSendEvent;
            this.bAbort = tick.bAbort;
            this.bUseStop = tick.bUseStop;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.slotType = SkillSlotType.SLOT_SKILL_0;
            this.effectTime = 0;
            this.changeSkillID = 0;
            this.changeSkillID2 = 0;
            this.changeSkillID3 = 0;
            this.changeSkillID4 = 0;
            this.recoverSkillID = 0;
            this.changeSkillID1Probability = 100;
            this.changeSkillID2Probability = 100;
            this.changeSkillID3Probability = 100;
            this.changeSkillID4Probability = 100;
            this.bCheckCondition = false;
            this.bOvertimeCD = true;
            this.bSendEvent = true;
            this.bAbort = false;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                SkillComponent skillControl = actorHandle.handle.SkillControl;
                if (skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    SkillSlot slot;
                    int index = 0;
                    int num2 = this.RandomSkillID(out index);
                    if (index > 0)
                    {
                        _action.refParams.AddRefParam("SpecifiedSkillCombineIndex", index);
                    }
                    if ((num2 != 0) && (skillControl.TryGetSkillSlot(this.slotType, out slot) && (slot != null)))
                    {
                        if (!this.CheckChangeSkillCondition(_action))
                        {
                            slot.StartSkillCD();
                        }
                        else
                        {
                            int effectTime = this.effectTime;
                            slot.skillChangeEvent.Start(effectTime, num2, this.recoverSkillID, this.bOvertimeCD, this.bSendEvent, this.bAbort, this.bUseStop);
                        }
                    }
                }
            }
        }

        private int RandomSkillID(out int index)
        {
            randomList.Clear();
            index = 0;
            int num = 0;
            if ((this.changeSkillID != 0) && (this.changeSkillID1Probability > 0))
            {
                num += this.changeSkillID1Probability;
                SelectionData item = new SelectionData {
                    ID = this.changeSkillID,
                    ProbabilityBase = num
                };
                randomList.Add(item);
            }
            if ((this.changeSkillID2 != 0) && (this.changeSkillID2Probability > 0))
            {
                num += this.changeSkillID2Probability;
                SelectionData data2 = new SelectionData {
                    ID = this.changeSkillID2,
                    ProbabilityBase = num
                };
                randomList.Add(data2);
            }
            if ((this.changeSkillID3 != 0) && (this.changeSkillID3Probability > 0))
            {
                num += this.changeSkillID3Probability;
                SelectionData data3 = new SelectionData {
                    ID = this.changeSkillID3,
                    ProbabilityBase = num
                };
                randomList.Add(data3);
            }
            if ((this.changeSkillID4 != 0) && (this.changeSkillID4Probability > 0))
            {
                num += this.changeSkillID4Probability;
                SelectionData data4 = new SelectionData {
                    ID = this.changeSkillID4,
                    ProbabilityBase = num
                };
                randomList.Add(data4);
            }
            if (randomList.Count == 1)
            {
                SelectionData data5 = randomList[0];
                return data5.ID;
            }
            if (randomList.Count != 0)
            {
                int num2 = FrameRandom.Random((uint) num);
                for (int i = 0; i < randomList.Count; i++)
                {
                    SelectionData data6 = randomList[i];
                    if (num2 < data6.ProbabilityBase)
                    {
                        index = i + 1;
                        SelectionData data7 = randomList[i];
                        return data7.ID;
                    }
                }
            }
            return 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SelectionData
        {
            public int ID;
            public int ProbabilityBase;
        }
    }
}

