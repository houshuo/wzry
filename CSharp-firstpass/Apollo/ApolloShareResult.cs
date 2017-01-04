namespace Apollo
{
    using System;

    public class ApolloShareResult : ApolloStruct<ApolloShareResult>
    {
        public string drescription;
        public string extInfo;
        public ApolloPlatform platform;
        public ApolloResult result;

        public override ApolloShareResult FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("Platform") == 0)
                    {
                        this.platform = (ApolloPlatform) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("nResult") == 0)
                    {
                        this.result = (ApolloResult) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("szDescription") == 0)
                    {
                        this.drescription = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("szExt") == 0)
                    {
                        this.extInfo = strArray3[1];
                    }
                }
            }
            return this;
        }
    }
}

