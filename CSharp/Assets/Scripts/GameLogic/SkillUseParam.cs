namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SkillUseParam
    {
        public SkillSlotType SlotType;
        public SkillRangeAppointType AppointType;
        public uint TargetID;
        public VInt3 UseVector;
        public bool bSpecialUse;
        public PoolObjHandle<ActorRoot> TargetActor;
        public PoolObjHandle<ActorRoot> Originator;
        public object Instigator;
        public void Reset()
        {
            this.SlotType = SkillSlotType.SLOT_SKILL_0;
            this.AppointType = SkillRangeAppointType.Auto;
            this.TargetID = 0;
            this.UseVector = VInt3.zero;
            this.bSpecialUse = false;
            this.TargetActor.Release();
            this.Originator.Release();
            this.Instigator = null;
        }

        private bool IsEquals(SkillUseParam rhs)
        {
            return (((((this.SlotType == rhs.SlotType) && (this.AppointType == rhs.AppointType)) && ((this.TargetID == rhs.TargetID) && (this.UseVector == rhs.UseVector))) && (((this.TargetActor == rhs.TargetActor) && (this.Originator == rhs.Originator)) && (this.Instigator == rhs.Instigator))) && (this.bSpecialUse == rhs.bSpecialUse));
        }

        public override bool Equals(object obj)
        {
            return (((obj != null) && (base.GetType() == obj.GetType())) && this.IsEquals((SkillUseParam) obj));
        }

        public void Init()
        {
            this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
            this.bSpecialUse = false;
        }

        public void Init(SkillSlotType InSlot)
        {
            this.SlotType = InSlot;
            this.bSpecialUse = false;
            this.AppointType = SkillRangeAppointType.Target;
        }

        public void Init(SkillSlotType InSlot, uint ObjID)
        {
            this.SlotType = InSlot;
            this.TargetID = ObjID;
            this.TargetActor = Singleton<GameObjMgr>.GetInstance().GetActor(this.TargetID);
            this.UseVector = (this.TargetActor == 0) ? VInt3.zero : this.TargetActor.handle.location;
            this.AppointType = SkillRangeAppointType.Target;
            this.bSpecialUse = false;
        }

        public void Init(SkillSlotType InSlot, VInt3 InVec)
        {
            this.SlotType = InSlot;
            this.UseVector = InVec;
            this.AppointType = SkillRangeAppointType.Pos;
            this.bSpecialUse = false;
        }

        public void Init(SkillSlotType InSlot, VInt3 InVec, bool bSpecial)
        {
            this.SlotType = InSlot;
            this.UseVector = InVec;
            this.AppointType = SkillRangeAppointType.Directional;
            this.bSpecialUse = bSpecial;
        }

        public void Init(SkillSlotType InSlot, PoolObjHandle<ActorRoot> InActorRoot)
        {
            this.SlotType = InSlot;
            this.TargetActor = InActorRoot;
            this.bSpecialUse = false;
        }

        public void SetOriginator(PoolObjHandle<ActorRoot> inOriginator)
        {
            this.Originator = inOriginator;
        }
    }
}

