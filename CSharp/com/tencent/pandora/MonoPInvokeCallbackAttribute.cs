namespace com.tencent.pandora
{
    using System;

    public class MonoPInvokeCallbackAttribute : Attribute
    {
        private Type type;

        public MonoPInvokeCallbackAttribute(Type t)
        {
            this.type = t;
        }
    }
}

