namespace com.tencent.pandora
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MethodCache
    {
        private MethodBase _cachedMethod;
        public bool IsReturnVoid;
        public object[] args;
        public int[] outList;
        public MethodArgs[] argTypes;
        public MethodBase cachedMethod
        {
            get
            {
                return this._cachedMethod;
            }
            set
            {
                this._cachedMethod = value;
                MethodInfo info = value as MethodInfo;
                if (info != null)
                {
                    this.IsReturnVoid = info.ReturnType == typeof(void);
                }
            }
        }
    }
}

