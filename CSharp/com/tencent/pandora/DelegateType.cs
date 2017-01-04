namespace com.tencent.pandora
{
    using System;

    public class DelegateType
    {
        public string name;
        public string strType = string.Empty;
        public Type type;

        public DelegateType(Type t)
        {
            this.type = t;
            this.strType = ToLuaExport.GetTypeStr(t);
            if (t.IsGenericType)
            {
                this.name = ToLuaExport.GetGenericLibName(t);
            }
            else
            {
                this.name = ToLuaExport.GetTypeStr(t);
                this.name = this.name.Replace(".", "_");
            }
        }

        public DelegateType SetName(string str)
        {
            this.name = str;
            return this;
        }
    }
}

