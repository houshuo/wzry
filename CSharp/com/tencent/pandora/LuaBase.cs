namespace com.tencent.pandora
{
    using System;

    public abstract class LuaBase : IDisposable
    {
        private bool _Disposed;
        protected LuaState _Interpreter;
        protected int _Reference;
        private int count = 1;
        public string name;
        protected ObjectTranslator translator;

        public void AddRef()
        {
            this.count++;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!this._Disposed)
            {
                if ((this._Reference != 0) && (this._Interpreter != null))
                {
                    if (disposeManagedResources)
                    {
                        this._Interpreter.dispose(this._Reference);
                        this._Reference = 0;
                    }
                    else if (this._Interpreter.L != IntPtr.Zero)
                    {
                        LuaScriptMgr.refGCList.Enqueue(new LuaRef(this._Interpreter.L, this._Reference));
                        this._Reference = 0;
                    }
                }
                this._Interpreter = null;
                this._Disposed = true;
            }
        }

        public override bool Equals(object o)
        {
            if (o is LuaBase)
            {
                LuaBase base2 = (LuaBase) o;
                return this._Interpreter.compareRef(base2._Reference, this._Reference);
            }
            return false;
        }

        ~LuaBase()
        {
            this.Dispose(false);
        }

        public override int GetHashCode()
        {
            return this._Reference;
        }

        protected void PushArgs(IntPtr L, object o)
        {
            LuaScriptMgr.PushVarObject(L, o);
        }

        public void Release()
        {
            if (this._Disposed || (this.name == null))
            {
                this.Dispose();
            }
            else
            {
                this.count--;
                if (this.count <= 0)
                {
                    if (this.name != null)
                    {
                        LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(this._Interpreter.L);
                        if (mgrFromLuaState != null)
                        {
                            mgrFromLuaState.RemoveLuaRes(this.name);
                        }
                    }
                    this.Dispose();
                }
            }
        }
    }
}

