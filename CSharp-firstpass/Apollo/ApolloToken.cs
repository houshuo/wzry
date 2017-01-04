namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloToken : ApolloStruct<ApolloToken>
    {
        public override ApolloToken FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("Type") == 0)
                    {
                        this.Type = (ApolloTokenType) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("Value") == 0)
                    {
                        this.Value = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("Expire") == 0)
                    {
                        this.Expire = long.Parse(strArray3[1]);
                    }
                }
            }
            return this;
        }

        public long Expire { get; set; }

        public ApolloTokenType Type { get; set; }

        public string Value { get; set; }
    }
}

