namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ApolloNoticeData : ApolloStruct<ApolloNoticeData>
    {
        private List<PictureData> picDataList;

        public void Dump()
        {
            Console.WriteLine("*******************notice data begin*****************");
            Console.WriteLine("MsgID:{0}", this.MsgID);
            Console.WriteLine("MsgUrl:{0}", this.MsgUrl);
            Console.WriteLine("MsgType:{0}", this.MsgType);
            Console.WriteLine("MsgScene:{0}", this.MsgScene);
            Console.WriteLine("StartTime:{0}", this.StartTime);
            Console.WriteLine("EndTime:{0}", this.EndTime);
            Console.WriteLine("ContentType:{0}", this.ContentType);
            Console.WriteLine("MsgTitle:{0}", this.MsgTitle);
            Console.WriteLine("MsgContent:{0}", this.MsgContent);
            Console.WriteLine("OpenID:{0}", this.OpenID);
            Console.WriteLine("picture data size:{0}", this.PicDataList.Count);
            Console.WriteLine("*******************notice data end*******************");
        }

        public override ApolloNoticeData FromString(string src)
        {
            char[] separator = new char[] { '&' };
            string[] strArray = src.Split(separator);
            this.PicDataList.Clear();
            foreach (string str in strArray)
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("MsgID") == 0)
                    {
                        this.MsgID = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("OpenID") == 0)
                    {
                        this.OpenID = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("MsgUrl") == 0)
                    {
                        this.MsgUrl = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("MsgType") == 0)
                    {
                        this.MsgType = (APOLLO_NOTICETYPE) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("ContentType") == 0)
                    {
                        this.ContentType = (APOLLO_NOTICE_CONTENTTYPE) int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("MsgScene") == 0)
                    {
                        this.MsgScene = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("StartTime") == 0)
                    {
                        this.StartTime = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("EndTime") == 0)
                    {
                        this.EndTime = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("ContentUrl") == 0)
                    {
                        this.ContentUrl = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("MsgTitle") == 0)
                    {
                        this.MsgTitle = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("MsgContent") == 0)
                    {
                        this.MsgContent = ApolloStringParser.ReplaceApolloString(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("PictureData") == 0)
                    {
                        char[] chArray3 = new char[] { ',' };
                        string[] strArray4 = ApolloStringParser.ReplaceApolloStringQuto(strArray3[1]).Split(chArray3);
                        this.PicDataList.Clear();
                        foreach (string str3 in strArray4)
                        {
                            PictureData item = new PictureData();
                            item.FromString(str3);
                            this.PicDataList.Add(item);
                        }
                    }
                }
            }
            this.Dump();
            return this;
        }

        public void Reset()
        {
            this.PicDataList.Clear();
            this.MsgID = string.Empty;
            this.OpenID = string.Empty;
            this.MsgUrl = string.Empty;
            this.MsgType = APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT;
            this.MsgScene = string.Empty;
            this.StartTime = string.Empty;
            this.EndTime = string.Empty;
            this.ContentType = APOLLO_NOTICE_CONTENTTYPE.APO_SCONTENTTYPE_TEXT;
            this.MsgTitle = string.Empty;
            this.MsgContent = string.Empty;
        }

        public APOLLO_NOTICE_CONTENTTYPE ContentType { get; set; }

        public string ContentUrl { get; set; }

        public string EndTime { get; set; }

        public string MsgContent { get; set; }

        public string MsgID { get; set; }

        public string MsgScene { get; set; }

        public string MsgTitle { get; set; }

        public APOLLO_NOTICETYPE MsgType { get; set; }

        public string MsgUrl { get; set; }

        public string OpenID { get; set; }

        public List<PictureData> PicDataList
        {
            get
            {
                if (this.picDataList == null)
                {
                    this.picDataList = new List<PictureData>();
                }
                return this.picDataList;
            }
        }

        public string StartTime { get; set; }
    }
}

