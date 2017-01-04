namespace com.tencent.pandora
{
    using System;

    public class LuaClassHelper
    {
        public static object callFunction(LuaFunction function, object[] args, Type[] returnTypes, object[] inArgs, int[] outArgs)
        {
            object obj2;
            int num;
            object[] objArray = function.call(inArgs, returnTypes);
            if (returnTypes[0] == typeof(void))
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

        public static LuaFunction getTableFunction(LuaTable luaTable, string name)
        {
            object obj2 = luaTable.rawget(name);
            if (obj2 is LuaFunction)
            {
                return (LuaFunction) obj2;
            }
            return null;
        }
    }
}

