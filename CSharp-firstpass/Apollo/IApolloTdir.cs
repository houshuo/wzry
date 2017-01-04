namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IApolloTdir
    {
        TdirResult DisableLog();
        TdirResult EnableLog();
        TdirResult GetErrorCode();
        string GetErrorString();
        TdirResult GetServiceTable(ref TdirServiceTable table);
        TdirResult GetTreeNodes(ref List<TdirTreeNode> nodeList);
        TdirResult Query(int appID, string[] ipList, int[] portList, string lastSuccessIP = "", int lastSuccessPort = 0, string openID = "", bool isOnlyTACC = false);
        TdirResult Recv(int timeout = 10);
        TdirResult SetSvrTimeout(int timeout = 0x1388);
        TdirResult Status();
    }
}

