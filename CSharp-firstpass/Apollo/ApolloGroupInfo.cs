namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloGroupInfo : ApolloStruct<ApolloGroupInfo>
    {
        public override ApolloGroupInfo FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("GroupName") == 0)
                    {
                        this.groupName = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("FingerMemo") == 0)
                    {
                        this.fingerMemo = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("MemberNum") == 0)
                    {
                        this.memberNum = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("MaxNum") == 0)
                    {
                        this.maxNum = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("OwnerOpenid") == 0)
                    {
                        this.ownerOpenid = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Unionid") == 0)
                    {
                        this.unionid = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Zoneid") == 0)
                    {
                        this.zoneid = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("AdminOpenids") == 0)
                    {
                        this.adminOpenids = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("GroupOpenid") == 0)
                    {
                        this.groupOpenid = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("GroupKey") == 0)
                    {
                        this.groupKey = strArray3[1];
                    }
                }
            }
            return this;
        }

        public string adminOpenids { get; set; }

        public string fingerMemo { get; set; }

        public string groupKey { get; set; }

        public string groupName { get; set; }

        public string groupOpenid { get; set; }

        public string maxNum { get; set; }

        public string memberNum { get; set; }

        public string ownerOpenid { get; set; }

        public string unionid { get; set; }

        public string zoneid { get; set; }
    }
}

