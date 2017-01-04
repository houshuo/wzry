namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloWaitingInfo : ApolloStruct<ApolloWaitingInfo>
    {
        public override ApolloWaitingInfo FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("Pos") == 0)
                    {
                        this.Pos = uint.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("QueueLen") == 0)
                    {
                        this.QueueLen = uint.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("EstimateTime") == 0)
                    {
                        this.EstimateTime = uint.Parse(strArray3[1]);
                    }
                }
            }
            return this;
        }

        public uint EstimateTime { get; set; }

        public uint Pos { get; set; }

        public uint QueueLen { get; set; }
    }
}

