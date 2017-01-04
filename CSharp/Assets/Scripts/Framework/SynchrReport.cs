namespace Assets.Scripts.Framework
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.IO;

    [MessageHandlerClass]
    public class SynchrReport
    {
        private static bool _isDeskUnsync;
        private static bool _isSelfUnsync;
        private static int _uploadIndex = -1;
        private static MemoryStream[] _uploadList;

        private static void CloseUpload()
        {
            if (_isDeskUnsync)
            {
            }
            _uploadList = null;
            _uploadIndex = -1;
            if (_isSelfUnsync)
            {
                if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync"), enUIEventID.Lobby_ConfirmErrExit, false);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync_Try"), enUIEventID.Battle_MultiHashInvalid, false);
                }
            }
        }

        [MessageHandler(0x504)]
        public static void OnHashCheckRsp(CSPkg pkg)
        {
            _isSelfUnsync = pkg.stPkgData.stRelayHashChkRsp.dwIsSelfNE != 0;
            _isDeskUnsync = pkg.stPkgData.stRelayHashChkRsp.dwIsDeskNE != 0;
            if (pkg.stPkgData.stRelayHashChkRsp.dwNeedUploadLog != 0)
            {
                Upload(0L);
            }
            else
            {
                CloseUpload();
            }
        }

        [MessageHandler(0x1476)]
        public static void OnUpload(CSPkg pkg)
        {
            Upload((long) pkg.stPkgData.stUploadCltlogReq.dwOffset);
        }

        private static void Upload(long offset)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1477);
            CSPKG_UPLOADCLTLOG_NTF stUploadCltlogNtf = msg.stPkgData.stUploadCltlogNtf;
            stUploadCltlogNtf.dwLogType = 0;
            bool flag = false;
            if (((_uploadList != null) && (_uploadIndex >= 0)) && (_uploadIndex < _uploadList.Length))
            {
                MemoryStream stream = _uploadList[_uploadIndex];
                if (offset < stream.Length)
                {
                    if (offset != stream.Position)
                    {
                        stream.Position = offset;
                    }
                    stUploadCltlogNtf.dwLogType = (uint) (_uploadIndex + 1);
                    stUploadCltlogNtf.dwBuffOffset = (uint) offset;
                    stUploadCltlogNtf.dwBufLen = (uint) stream.Read(stUploadCltlogNtf.szBuf, 0, stUploadCltlogNtf.szBuf.Length);
                    if (stream.Position >= stream.Length)
                    {
                        flag = ++_uploadIndex >= _uploadList.Length;
                        stUploadCltlogNtf.bThisLogOver = 1;
                        stUploadCltlogNtf.bAllLogOver = !flag ? ((byte) 0) : ((byte) 1);
                    }
                    else
                    {
                        stUploadCltlogNtf.bThisLogOver = 0;
                        stUploadCltlogNtf.bAllLogOver = 0;
                    }
                }
            }
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            if (flag || (stUploadCltlogNtf.dwLogType == 0))
            {
                CloseUpload();
            }
        }
    }
}

