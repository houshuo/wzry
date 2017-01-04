using Assets.Scripts.Framework;
using CSProtocol;
using System;

public abstract class CheatCommandNetworking : CheatCommandCommon
{
    private static CSDT_CHEATCMD_DETAIL DummyDetail = new CSDT_CHEATCMD_DETAIL();

    protected CheatCommandNetworking()
    {
    }

    protected sealed override string Execute(string[] InArguments)
    {
        throw new NotImplementedException();
    }

    protected abstract string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef);
    public override string StartProcess(string[] InArguments)
    {
        string str;
        if (!this.CheckArguments(InArguments, out str))
        {
            return str;
        }
        if (this.messageID == 0)
        {
            return this.Execute(InArguments, ref DummyDetail);
        }
        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f4);
        msg.stPkgData.stCheatCmd.iCheatCmd = this.messageID;
        msg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
        string str2 = this.Execute(InArguments, ref msg.stPkgData.stCheatCmd.stCheatCmdDetail);
        if (str2 == CheatCommandBase.Done)
        {
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }
        return str2;
    }
}

