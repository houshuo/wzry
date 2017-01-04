namespace Apollo
{
    using System;

    public interface IApolloHttpResponse
    {
        byte[] GetData();
        string GetHeader(string name);
        string GetHttpVersion();
        string GetStatus();
        string GetStatusMessage();
        string ToString();
    }
}

