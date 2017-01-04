namespace Apollo
{
    using System;

    public enum TdirResult
    {
        AllIpConnectFail = 0xc9,
        AllocateMemoryFail = 0xca,
        CfgFileNotFound = 2,
        GetObjectFailed = 1,
        InitTgcpApiFailed = 0xce,
        NeedInitBeforeQuery = 5,
        NeedRecvDoneStatus = 3,
        NeedWaitForRecvStatus = 4,
        NoServiceTable = 0xd3,
        ParamError = 0xd1,
        RecvDirFail = 0xcc,
        RecvDone = 0x66,
        SendTdirReqFail = 0xcb,
        StopSessionFailed = 0xcf,
        TDIR_ERROR = 200,
        TdirNoError = 0,
        TgcpapiError = 0xd0,
        UnInit = 0x67,
        UnpackFail = 0xcd,
        WaitForQuery = 100,
        WaitForRecv = 0x65,
        WaitSvrRepTimeout = 210
    }
}

