namespace com.tencent.pandora
{
    using System;

    public class LuaDelegate
    {
        public LuaFunction function = null;
        public Type[] returnTypes = null;

        public object callFunction(object[] args, object[] inArgs, int[] outArgs)
        {
            object obj2;
            int num;
            object[] objArray = this.function.call(inArgs, this.returnTypes);
            if (this.returnTypes[0] == typeof(void))
            {
                obj2 = null;
                num = 0;
            }
            else
            {
                obj2 = objArray[0];
                num = 1;
            }
            for (int i = 0; i < outArgs.Length; i++)
            {
                args[outArgs[i]] = objArray[num];
                num++;
            }
            return obj2;
        }
    }
}

