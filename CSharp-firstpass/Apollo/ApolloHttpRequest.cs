namespace Apollo
{
    using apollo_http_object;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class ApolloHttpRequest : ApolloObject, IApolloHttpRequest
    {
        private IApolloConnector connector;
        private byte[] data = new byte[1];
        private bool gotTimeout;
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private byte[] method = Encoding.UTF8.GetBytes("GET");
        private ListView<ApolloHttpResponse> responses = new ListView<ApolloHttpResponse>();
        private IApolloTalker talker;
        private float timeout;
        private byte[] URL = new byte[1];
        private byte[] version = Encoding.UTF8.GetBytes("HTTP/1.1");

        public event OnRespondHandler ResponseEvent;

        public ApolloHttpRequest(IApolloConnector connector)
        {
            this.connector = connector;
            if (connector == null)
            {
                throw new Exception("Invalid Argument");
            }
            this.talker = IApollo.Instance.CreateTalker(connector);
            this.SetMethod("GET");
            this.SetHeader("apollo-server-ip", "127.0.0.1");
        }

        public string ByteArray2String(byte[] bytearray)
        {
            string str = Encoding.UTF8.GetString(bytearray);
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return str.Trim(new char[1]);
        }

        private void DealRsp(HttpRsp rsp, ApolloResult rst)
        {
            if (rst == ApolloResult.Success)
            {
                ApolloHttpResponse item = new ApolloHttpResponse();
                string version = this.ByteArray2String(rsp.stResponseStatus.szHttpVersion);
                item.SetHttpVersion(version);
                string status = this.ByteArray2String(rsp.stResponseStatus.szStatusCode);
                item.SetStatus(status);
                string msg = this.ByteArray2String(rsp.stResponseStatus.szReasonPhrase);
                item.SetStatusMessage(msg);
                for (int i = 0; i < rsp.stHttpHeaders.dwHeaderCount; i++)
                {
                    HeaderUnit unit = rsp.stHttpHeaders.astHeaderUnit[i];
                    string name = this.ByteArray2String(unit.szHeaderName);
                    string str5 = this.ByteArray2String(unit.szHeaderContent);
                    item.SetHeader(name, str5);
                }
                item.SetData(rsp.stResponseContent.szData, rsp.stResponseContent.dwDataLen);
                ADebug.Log("Get Result Response :" + item.ToString());
                this.responses.Add(item);
                if (this.ResponseEvent != null)
                {
                    IApolloHttpResponse response2 = item;
                    this.ResponseEvent(response2, rst);
                }
            }
            else if (rst == ApolloResult.Timeout)
            {
                this.gotTimeout = true;
            }
            else
            {
                ADebug.LogError("Got recv error :" + rst);
            }
        }

        public void EnableAutoUpdate(bool enable)
        {
            if (enable)
            {
                this.talker.AutoUpdate = true;
            }
            else
            {
                this.talker.AutoUpdate = false;
            }
        }

        public byte[] GetData()
        {
            return this.data;
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
            return this.ByteArray2String(this.version);
        }

        public string GetMethod()
        {
            return this.ByteArray2String(this.method);
        }

        public IApolloHttpResponse GetResponse()
        {
            if (this.responses.Count != 0)
            {
                IApolloHttpResponse response = this.responses[0];
                this.responses.RemoveAt(0);
                return response;
            }
            if (this.gotTimeout)
            {
                this.gotTimeout = false;
                throw new TimeoutException("ApolloHttpClient Get Response timeout!");
            }
            ADebug.Log("IApolloHttpResponse:Response is null");
            return null;
        }

        public IApolloTalker GetTalker()
        {
            return this.talker;
        }

        public string GetURL()
        {
            return this.ByteArray2String(this.URL);
        }

        public HttpReq PackRequest()
        {
            HttpHeaders headers;
            RequestLine line = new RequestLine {
                szRequestMethod = this.method,
                szRequestUri = this.URL,
                szHttpVersion = this.version
            };
            ListLinqView<HeaderUnit> view = new ListLinqView<HeaderUnit>();
            foreach (KeyValuePair<string, string> pair in this.headers)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(pair.Key);
                byte[] buffer2 = Encoding.UTF8.GetBytes(pair.Value);
                HeaderUnit item = new HeaderUnit {
                    szHeaderName = bytes,
                    szHeaderContent = buffer2
                };
                view.Add(item);
            }
            headers = new HttpHeaders {
                astHeaderUnit = view.ToArray(),
                dwHeaderCount = (uint) headers.astHeaderUnit.Length
            };
            RequestContent content = new RequestContent {
                szData = this.data,
                dwDataLen = (uint) this.data.Length
            };
            HttpReq req = new HttpReq {
                stRequestLine = line,
                stHttpHeaders = headers,
                stRequestContent = content
            };
            ADebug.Log("send request :" + this.ToString());
            return req;
        }

        public ApolloResult SendRequest()
        {
            ADebug.Log("Send request to tconnd through talker");
            HttpReq request = this.PackRequest();
            if (request == null)
            {
                ADebug.Log("httpReq is null");
            }
            return this.talker.Send<HttpRsp>(request, delegate (object req, TalkerEventArgs<HttpRsp> e) {
                ADebug.Log("Get Response form server");
                this.DealRsp(e.Response, e.Result);
            }, null, this.timeout);
        }

        public ApolloResult SetData(byte[] data)
        {
            if (data != null)
            {
                if ((data.Length == 0) || (data.Length >= 0x1fa0))
                {
                    return ApolloResult.HttpReqToLong;
                }
                this.data = data;
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
            if (!ApolloHttpVersion.Valied(version))
            {
                return ApolloResult.HttpBadVersion;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(version);
            this.version = bytes;
            return ApolloResult.Success;
        }

        public ApolloResult SetMethod(string method)
        {
            if (!ApolloHttpMethod.Valied(method))
            {
                return ApolloResult.HttpBadMethod;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(method);
            this.method = bytes;
            return ApolloResult.Success;
        }

        public void SetTimeout(float timeout)
        {
            this.timeout = timeout;
        }

        public ApolloResult SetURL(string URL)
        {
            if (!this.ValiedURL(URL))
            {
                return ApolloResult.HttpBadURL;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(URL);
            this.URL = bytes;
            return ApolloResult.Success;
        }

        public override string ToString()
        {
            string str = string.Empty;
            string str2 = this.ByteArray2String(this.method);
            string str3 = this.ByteArray2String(this.version);
            string str4 = this.ByteArray2String(this.URL);
            string str5 = str;
            string[] textArray1 = new string[] { str5, str2, " ", str4, " ", str3, " \r\n\r\n" };
            str = string.Concat(textArray1);
            foreach (KeyValuePair<string, string> pair in this.headers)
            {
                str5 = str;
                string[] textArray2 = new string[] { str5, pair.Key, " : ", pair.Value, "\n" };
                str = string.Concat(textArray2);
            }
            return (str + "\n" + this.data);
        }

        public override void Update()
        {
            if (this.connector != null)
            {
                this.talker.Update(1);
            }
        }

        private bool ValiedURL(string URL)
        {
            return (string.IsNullOrEmpty(URL) || (((URL.Length > 1) && ((URL.Substring(0, 1) == "/") || (URL.Substring(0, 1) == "?"))) || ((URL.Length > 7) && (URL.Substring(0, 7) == "http://"))));
        }
    }
}

