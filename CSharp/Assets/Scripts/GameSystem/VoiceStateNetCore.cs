namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class VoiceStateNetCore
    {
        [MessageHandler(0x12f3)]
        public static void On_NTF_VOICESTATE(CSPkg msg)
        {
            SCPKG_NTF_VOICESTATE stNtfVoiceState = msg.stPkgData.stNtfVoiceState;
            MonoSingleton<VoiceSys>.instance.SetVoiceState(stNtfVoiceState.ullAcntUid, (CS_VOICESTATE_TYPE) stNtfVoiceState.bVoiceState);
            if (Singleton<CBattleSystem>.instance.BattleStatView != null)
            {
                Singleton<CBattleSystem>.instance.BattleStatView.RefreshVoiceStateIfNess();
            }
        }

        public static void Send_Acnt_VoiceState(CS_VOICESTATE_TYPE type)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x12f2);
            msg.stPkgData.stAcntVoiceState.bVoiceState = (byte) type;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }
    }
}

