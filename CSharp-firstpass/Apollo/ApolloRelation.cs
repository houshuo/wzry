namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ApolloRelation : ApolloStruct<ApolloRelation>
    {
        private List<ApolloPerson> peirsons;

        public override ApolloRelation FromString(string src)
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
                        this.Result = (ApolloResult) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("Desc") == 0)
                    {
                        this.Desc = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("InfoList") == 0)
                    {
                        this.Persons.Clear();
                        if (!string.IsNullOrEmpty(strArray3[1]))
                        {
                            char[] chArray3 = new char[] { ',' };
                            foreach (string str2 in strArray3[1].Split(chArray3))
                            {
                                string str3 = ApolloStringParser.ReplaceApolloString(ApolloStringParser.ReplaceApolloString(str2));
                                ApolloPerson item = new ApolloPerson();
                                item.FromString(str3);
                                this.Persons.Add(item);
                            }
                        }
                    }
                    else if (strArray3[0].CompareTo("ExtInfo") == 0)
                    {
                        this.ExtInfo = strArray3[1];
                    }
                }
            }
            return this;
        }

        public string Desc { get; set; }

        public string ExtInfo { get; set; }

        public List<ApolloPerson> Persons
        {
            get
            {
                if (this.peirsons == null)
                {
                    this.peirsons = new List<ApolloPerson>();
                }
                return this.peirsons;
            }
        }

        public ApolloResult Result { get; set; }
    }
}

