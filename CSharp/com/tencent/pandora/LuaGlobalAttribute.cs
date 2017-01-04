namespace com.tencent.pandora
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class LuaGlobalAttribute : Attribute
    {
        private string descript;
        private string name;

        public string Description
        {
            get
            {
                return this.descript;
            }
            set
            {
                this.descript = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}

