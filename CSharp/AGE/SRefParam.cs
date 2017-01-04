namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    public class SRefParam
    {
        public bool dirty;
        public object obj;
        public ValType type = ValType.Object;
        public SUnion union = new SUnion();

        public SRefParam Clone()
        {
            SRefParam param = SObjPool<SRefParam>.New();
            param.type = this.type;
            param.dirty = false;
            if (this.type < ValType.Object)
            {
                param.union._quat = this.union._quat;
                return param;
            }
            param.obj = this.obj;
            if (this.type == ValType.ActorRoot)
            {
                param.union._uint = this.union._uint;
            }
            return param;
        }

        public void Destroy()
        {
            this.obj = null;
            SObjPool<SRefParam>.Delete(this);
        }

        public PoolObjHandle<ActorRoot> handle
        {
            get
            {
                return new PoolObjHandle<ActorRoot> { _handleObj = this.obj as ActorRoot, _handleSeq = this.union._uint };
            }
            set
            {
                this.obj = value._handleObj;
                this.union._uint = value._handleSeq;
            }
        }

        public enum ValType
        {
            Bool,
            Int,
            UInt,
            Float,
            VInt3,
            Vector3,
            Quaternion,
            Object,
            ActorRoot
        }
    }
}

