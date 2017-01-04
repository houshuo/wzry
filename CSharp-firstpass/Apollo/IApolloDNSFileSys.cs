namespace Apollo
{
    using System;

    public interface IApolloDNSFileSys
    {
        bool Exist(string domainName);
        bool Read(string domainName, ref string buffer);
        bool Remove(string domainName);
        bool Write(string domainName, string buffer);
    }
}

