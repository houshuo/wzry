namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ApolloHttpResponse : ApolloObject, IApolloHttpResponse
    {
        private byte[] data = new byte[0x100000];
        private uint datalen;
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private string status;
        private string statusMsg;
        private string version;

        public byte[] GetData()
        {
            byte[] destinationArray = new byte[this.datalen];
            Array.Copy(this.data, destinationArray, (long) this.datalen);
            return destinationArray;
        }

        public string GetHeader(string name)
        {
            if (!this.headers.ContainsKey(name))
            {
                return string.Empty;
            }
            return this.headers[name];
        }

        public string GetHttpVersion()
        {
            return this.version;
        }

        public string GetStatus()
        {
            return this.status;
        }

        public string GetStatusMessage()
        {
            return this.statusMsg;
        }

        public ApolloResult SetData(byte[] data, uint datalen)
        {
            if (datalen >= 0x100000)
            {
                ADebug.Log("ApolloHttpResponse data is too long, data is to cut off");
                Array.Copy(this.data, 0, data, 0, 0x100000);
                this.datalen = datalen;
            }
            else
            {
                this.data = data;
                this.datalen = datalen;
            }
            return ApolloResult.Success;
        }

        public ApolloResult SetHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
            {
                return ApolloResult.HttpBadHeader;
            }
            try
            {
                this.headers.Add(name, value);
            }
            catch (ArgumentException)
            {
                this.headers[name] = value;
            }
            return ApolloResult.Success;
        }

        public ApolloResult SetHttpVersion(string version)
        {
            this.version = version;
            return ApolloResult.Success;
        }

        public ApolloResult SetStatus(string status)
        {
            this.status = status;
            return ApolloResult.Success;
        }

        public ApolloResult SetStatusMessage(string msg)
        {
            this.statusMsg = msg;
            return ApolloResult.Success;
        }

        public override string ToString()
        {
            string str = (((string.Empty + this.version) + " " + this.status) + " " + this.statusMsg) + "\r\n" + "\r\n";
            foreach (KeyValuePair<string, string> pair in this.headers)
            {
                string str3 = str;
                string[] textArray1 = new string[] { str3, pair.Key, " : ", pair.Value, "\n" };
                str = string.Concat(textArray1);
            }
            str = str + "\n";
            string str2 = Encoding.ASCII.GetString(this.data, 0, Convert.ToInt32(this.datalen));
            return (str + str2);
        }
    }
}

