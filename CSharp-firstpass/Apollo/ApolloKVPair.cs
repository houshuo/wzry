namespace Apollo
{
    using System;

    public class ApolloKVPair : ApolloStruct<ApolloKVPair>
    {
        public string Key;
        public string Value;

        public override ApolloKVPair FromString(string src)
        {
            ApolloStringParser parser = new ApolloStringParser(src);
            this.Key = parser.GetString("key");
            this.Value = parser.GetString("value");
            return this;
        }
    }
}

