namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BufConsumer
    {
        public int BuffID;
        public PoolObjHandle<ActorRoot> TargetActor;
        public PoolObjHandle<ActorRoot> SrcActor;
        public BuffFense buffSkill;
        public BufConsumer(int InBuffID, PoolObjHandle<ActorRoot> InTargetActor, PoolObjHandle<ActorRoot> inSrcActor)
        {
            this.BuffID = InBuffID;
            this.TargetActor = InTargetActor;
            this.SrcActor = inSrcActor;
            this.buffSkill = null;
        }

        public bool Use()
        {
            BuffSkill inBuff = ClassObjPool<BuffSkill>.Get();
            inBuff.Init(this.BuffID);
            if (inBuff.cfgData == null)
            {
                inBuff.Release();
                return false;
            }
            SkillUseParam param = new SkillUseParam();
            param.Init(SkillSlotType.SLOT_SKILL_VALID, this.TargetActor.handle.ObjID);
            param.SetOriginator(this.SrcActor);
            if (!inBuff.Use(this.SrcActor, ref param))
            {
                inBuff.Release();
                return false;
            }
            this.buffSkill = new BuffFense(inBuff);
            return true;
        }
    }
}

