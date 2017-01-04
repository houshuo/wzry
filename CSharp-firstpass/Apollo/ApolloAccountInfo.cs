namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloAccountInfo : ApolloStruct<ApolloAccountInfo>
    {
        private ListView<ApolloToken> tokenList;

        public override ApolloAccountInfo FromString(string src)
        {
            Console.WriteLine("ApolloLZK srccc {0}", src);
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("Platform") == 0)
                    {
                        this.Platform = (ApolloPlatform) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("OpenId") == 0)
                    {
                        this.OpenId = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("UserId") == 0)
                    {
                        this.UserId = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Uin") == 0)
                    {
                        this.Uin = ulong.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("TokenList") == 0)
                    {
                        char[] chArray3 = new char[] { ',' };
                        string[] strArray4 = strArray3[1].Split(chArray3);
                        this.TokenList.Clear();
                        foreach (string str2 in strArray4)
                        {
                            string str3 = ApolloStringParser.ReplaceApolloString(ApolloStringParser.ReplaceApolloString(str2));
                            ApolloToken item = new ApolloToken();
                            item.FromString(str3);
                            this.TokenList.Add(item);
                        }
                    }
                    else if (strArray3[0].CompareTo("Pf") == 0)
                    {
                        this.Pf = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("PfKey") == 0)
                    {
                        this.PfKey = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("STKey") == 0)
                    {
                        this.STKey = strArray3[1];
                    }
                }
            }
            return this;
        }

        public ApolloToken GetToken(ApolloTokenType type)
        {
            foreach (ApolloToken token in this.TokenList)
            {
                if (token.Type == type)
                {
                    return token;
                }
            }
            return null;
        }

        public void Reset()
        {
            this.Platform = ApolloPlatform.None;
            this.OpenId = string.Empty;
            this.Uin = 0L;
            this.TokenList.Clear();
            this.Pf = string.Empty;
            this.PfKey = string.Empty;
            this.STKey = string.Empty;
        }

        public string OpenId { get; set; }

        public string Pf { get; set; }

        public string PfKey { get; set; }

        public ApolloPlatform Platform { get; set; }

        public string STKey { get; set; }

        public ListView<ApolloToken> TokenList
        {
            get
            {
                if (this.tokenList == null)
                {
                    this.tokenList = new ListView<ApolloToken>();
                }
                return this.tokenList;
            }
        }

        public ulong Uin { get; set; }

        public string UserId { get; set; }
    }
}

