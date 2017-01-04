namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class Pdr : MonoBehaviour
    {
        public static bool isInitUlua;

        public static void DoString(string str)
        {
            try
            {
                LuaScriptMgr.Instance.DoString(str);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("FERROR:" + exception.ToString());
            }
        }

        public static string GetSDKVer()
        {
            return Configer.strSDKVer;
        }

        public string GetTempPath()
        {
            return Application.temporaryCachePath;
        }

        public void Log(string strMsg)
        {
            com.tencent.pandora.Logger.d(strMsg);
        }

        public static void LuaReady(bool isReady)
        {
            if (isReady)
            {
                com.tencent.pandora.Logger.d("Lua Ready");
                isInitUlua = true;
            }
            else
            {
                isInitUlua = false;
            }
        }

        public static void SendPandoraLibCmd(int iCmdId, string sData, int iLength, int iFlag)
        {
            NetProxcy.SendPandoraLibCmd(iCmdId, sData, iLength, iFlag);
        }

        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}

