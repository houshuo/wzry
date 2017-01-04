namespace Apollo
{
    using System;

    public interface IApolloHttpRequest
    {
        event OnRespondHandler ResponseEvent;

        void EnableAutoUpdate(bool enable);
        byte[] GetData();
        string GetHeader(string name);
        string GetHttpVersion();
        string GetMethod();
        IApolloHttpResponse GetResponse();
        IApolloTalker GetTalker();
        string GetURL();
        ApolloResult SendRequest();
        ApolloResult SetData(byte[] data);
        ApolloResult SetHeader(string name, string value);
        ApolloResult SetHttpVersion(string version);
        ApolloResult SetMethod(string method);
        void SetTimeout(float timeout);
        ApolloResult SetURL(string URL);
        string ToString();
        void Update();
    }
}

