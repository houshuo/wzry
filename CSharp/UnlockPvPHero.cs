using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[CheatCommand("英雄/解锁/UnlockPvPHero", "解锁PvP英雄(0表示所有)", 0x21), ArgumentDescription(0, typeof(int), "英雄id", new object[] {  })]
internal class UnlockPvPHero : CheatCommandNetworking
{
    public override bool CheckArguments(string[] InArguments, out string OutMessage)
    {
        if (!base.CheckArguments(InArguments, out OutMessage))
        {
            return false;
        }
        bool flag = false;
        uint num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        if (num == 0)
        {
            flag = true;
        }
        else
        {
            DictionaryView<uint, CHeroInfo>.Enumerator enumerator = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfoDic().GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                if (num == current.Key)
                {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag)
        {
            OutMessage = "错误的英雄ID";
            return false;
        }
        return true;
    }

    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        string outMessage = string.Empty;
        if (this.CheckArguments(InArguments, out outMessage))
        {
            CheatCmdRef.stUnlockHeroPVPMask = new CSDT_CHEAT_UNLOCK_HEROPVPMASK();
            CheatCmdRef.stUnlockHeroPVPMask.dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
            return CheatCommandBase.Done;
        }
        return outMessage;
    }
}

