namespace Apollo
{
    using System;
    using System.Collections.Generic;

    public class ApolloWakeupInfo : ApolloStruct<ApolloWakeupInfo>
    {
        public string Country;
        private List<ApolloKVPair> extensionInfos;
        public string Lang;
        public string MediaTagName;
        public string MessageExt;
        public string OpenId;
        public ApolloPlatform Platform;
        public ApolloWakeState state;

        public override ApolloWakeupInfo FromString(string src)
        {
            ApolloStringParser parser = new ApolloStringParser(src);
            this.state = (ApolloWakeState) parser.GetInt("State");
            this.Platform = (ApolloPlatform) parser.GetInt("Platform");
            this.MediaTagName = parser.GetString("MediaTagName");
            this.OpenId = parser.GetString("OpenId");
            this.Lang = parser.GetString("Lang");
            this.Country = parser.GetString("Country");
            this.MessageExt = parser.GetString("MessageExt");
            string str = parser.GetString("ExtInfo");
            if ((str != null) && (string.Empty != str))
            {
                char[] separator = new char[] { ',' };
                string[] strArray = str.Split(separator);
                this.ExtensionInfo.Clear();
                foreach (string str2 in strArray)
                {
                    string str3 = ApolloStringParser.ReplaceApolloString(ApolloStringParser.ReplaceApolloString(str2));
                    ApolloKVPair item = new ApolloKVPair();
                    item.FromString(str3);
                    this.ExtensionInfo.Add(item);
                }
            }
            return this;
        }

        public List<ApolloKVPair> ExtensionInfo
        {
            get
            {
                if (this.extensionInfos == null)
                {
                    this.extensionInfos = new List<ApolloKVPair>();
                }
                return this.extensionInfos;
            }
        }
    }
}

