namespace com.tencent.pandora
{
    using System;

    internal class ClassGenerator
    {
        private Type klass;
        private ObjectTranslator translator;

        public ClassGenerator(ObjectTranslator translator, Type klass)
        {
            this.translator = translator;
            this.klass = klass;
        }

        public object extractGenerated(IntPtr luaState, int stackPos)
        {
            return CodeGeneration.Instance.GetClassInstance(this.klass, this.translator.getTable(luaState, stackPos));
        }
    }
}

