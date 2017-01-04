namespace Apollo
{
    using System;
    using System.Collections.Generic;

    public class ApolloNoticeInfo : ApolloStruct<ApolloNoticeInfo>
    {
        private List<ApolloNoticeData> dataList;

        public void Dump()
        {
            Console.WriteLine("*******************notice info begin*****************");
            Console.WriteLine("size of info list:{0}", this.DataList.Count);
            Console.WriteLine("*******************notice info end*******************");
        }

        public override ApolloNoticeInfo FromString(string src)
        {
            Console.WriteLine("src={0}", src);
            char[] separator = new char[] { ',' };
            string[] strArray = src.Split(separator);
            this.DataList.Clear();
            foreach (string str in strArray)
            {
                ApolloNoticeData item = new ApolloNoticeData();
                item.FromString(str);
                this.DataList.Add(item);
            }
            this.Dump();
            return this;
        }

        public void Reset()
        {
            this.dataList.Clear();
        }

        public List<ApolloNoticeData> DataList
        {
            get
            {
                if (this.dataList == null)
                {
                    this.dataList = new List<ApolloNoticeData>();
                }
                return this.dataList;
            }
        }
    }
}

