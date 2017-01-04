namespace com.tencent.pandora
{
    using System;

    internal class DelegateGenerator
    {
        private Type delegateType;
        private ObjectTranslator translator;

        public DelegateGenerator(ObjectTranslator translator, Type delegateType)
        {
            this.translator = translator;
            this.delegateType = delegateType;
        }

        public object extractGenerated(IntPtr luaState, int stackPos)
        {
            return CodeGeneration.Instance.GetDelegate(this.delegateType, this.translator.getFunction(luaState, stackPos));
        }
    }
}

