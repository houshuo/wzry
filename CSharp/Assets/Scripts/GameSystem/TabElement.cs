namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    public class TabElement
    {
        public byte camp;
        public uint cfgId;
        public string configContent;
        public string selfDefContent;
        public Type type;

        public TabElement(string selfDef = "")
        {
            this.type = Type.SelfDef;
            this.cfgId = 0;
            this.configContent = null;
            this.selfDefContent = selfDef;
        }

        public TabElement(uint cfgid, string configContent = "")
        {
            this.type = Type.Config;
            this.cfgId = cfgid;
            this.configContent = configContent;
            this.selfDefContent = null;
        }

        public TabElement Clone()
        {
            return new TabElement(string.Empty) { type = this.type, cfgId = this.cfgId, configContent = this.configContent, selfDefContent = this.selfDefContent, camp = this.camp };
        }

        public enum Type
        {
            None,
            Config,
            SelfDef
        }
    }
}

