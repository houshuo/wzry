namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloLocation : ApolloStruct<ApolloLocation>
    {
        public override ApolloLocation FromString(string src)
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
                    else if (strArray3[0].CompareTo("longitude") == 0)
                    {
                        this.Longitude = double.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("latitude") == 0)
                    {
                        this.Latitude = double.Parse(strArray3[1]);
                    }
                }
            }
            return this;
        }

        public string Desc { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ApolloResult Result { get; set; }
    }
}

