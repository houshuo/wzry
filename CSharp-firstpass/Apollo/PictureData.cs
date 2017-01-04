namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class PictureData : ApolloStruct<PictureData>
    {
        public void Dump()
        {
            Console.WriteLine("*******************picture data begin*****************");
            Console.WriteLine("ScreenDir:{0}", this.ScreenDir);
            Console.WriteLine("PicPath:{0}", this.PicPath);
            Console.WriteLine("HashValue:{0}", this.HashValue);
            Console.WriteLine("*******************picture data end*******************");
        }

        public override PictureData FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("PicPath") == 0)
                    {
                        this.PicPath = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("HashValue") == 0)
                    {
                        this.HashValue = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                }
            }
            this.Dump();
            return this;
        }

        public void Reset()
        {
            this.ScreenDir = APOLLO_NOTICE_SCREENDIR.APO_SCREENDIR_LANDSCAPE;
            this.PicPath = string.Empty;
            this.HashValue = string.Empty;
        }

        public string HashValue { get; set; }

        public string PicPath { get; set; }

        public APOLLO_NOTICE_SCREENDIR ScreenDir { get; set; }
    }
}

