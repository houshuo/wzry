namespace Apollo.Plugins.WTLoginAdapter
{
    using Apollo;
    using System;

    public class WTLoginAdapter : PluginBase
    {
        public static Apollo.Plugins.WTLoginAdapter.WTLoginAdapter Instance = new Apollo.Plugins.WTLoginAdapter.WTLoginAdapter();
        private const string pluginName = "WTLoginAdapter";

        private WTLoginAdapter()
        {
        }

        public override string GetPluginName()
        {
            return "WTLogin";
        }

        public override bool Install()
        {
            return true;
        }
    }
}

