namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloGroupResult : ApolloStruct<ApolloGroupResult>
    {
        public ApolloGroupInfo groupInfo;

        public override ApolloGroupResult FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("Result") == 0)
                    {
                        this.result = (ApolloResult) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("ErrorCode") == 0)
                    {
                        this.errorCode = int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("szDescription") == 0)
                    {
                        this.desc = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("GroupInfo") == 0)
                    {
                        string str2 = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                        this.groupInfo = new ApolloGroupInfo();
                        this.groupInfo.FromString(str2);
                    }
                }
            }
            return this;
        }

        public string desc { get; set; }

        public int errorCode { get; set; }

        public ApolloResult result { get; set; }
    }
}

