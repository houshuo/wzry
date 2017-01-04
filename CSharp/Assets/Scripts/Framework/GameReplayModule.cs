namespace Assets.Scripts.Framework
{
    using Apollo;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    public class GameReplayModule : Singleton<GameReplayModule>, IGameModule
    {
        public const string ABC_EXT = ".abc";
        public const string ABS_EXT = ".abs";
        private bool bProfileReplay;
        private int bufferUsedSize;
        private uint endKFraqNo;
        public bool isReplay;
        private bool isReplayAbc = true;
        private MemoryStream recordStream;
        private BinaryWriter recordWriter;
        private BinaryReader replayReader;
        private FileStream replayStream;
        private byte[] streamBuffer = new byte[0x7d00];

        public void BattleStart()
        {
            if (this.isReplay)
            {
            }
        }

        private void BeginRecord(SCPKG_MULTGAME_BEGINLOAD beginLoadPkg)
        {
            this.ClearRecord();
            if (beginLoadPkg != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                    if (accountInfo != null)
                    {
                        uint dwHeroID = 0;
                        for (int i = 0; i < beginLoadPkg.astCampInfo.Length; i++)
                        {
                            CSDT_CAMPINFO csdt_campinfo = beginLoadPkg.astCampInfo[i];
                            for (uint j = 0; j < csdt_campinfo.dwPlayerNum; j++)
                            {
                                CSDT_CAMPPLAYERINFO csdt_campplayerinfo = csdt_campinfo.astCampPlayerInfo[j];
                                if (Utility.UTF8Convert(csdt_campplayerinfo.szOpenID) == accountInfo.OpenId)
                                {
                                    dwHeroID = csdt_campplayerinfo.stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                                    break;
                                }
                            }
                            if (dwHeroID > 0)
                            {
                                break;
                            }
                        }
                        if (dwHeroID > 0)
                        {
                            this.recordStream = new MemoryStream(0x100000);
                            this.recordWriter = new BinaryWriter(this.recordStream);
                            this.recordWriter.Write(CVersion.GetAppVersion());
                            this.recordWriter.Write(CVersion.GetUsedResourceVersion());
                            this.recordWriter.Write(dwHeroID);
                            this.recordWriter.Write(DateTime.Now.Ticks);
                            this.recordWriter.Write(beginLoadPkg.stDeskInfo.bMapType);
                            this.recordWriter.Write(beginLoadPkg.stDeskInfo.dwMapId);
                            this.recordWriter.Write(masterRoleInfo.Name);
                            this.recordWriter.Write(masterRoleInfo.HeadUrl);
                            this.recordWriter.Write(masterRoleInfo.m_rankGrade);
                            this.recordWriter.Write(masterRoleInfo.m_rankClass);
                        }
                    }
                }
            }
        }

        public bool BeginReplay(string path, bool generateReplayLog = true)
        {
            if (!File.Exists(path))
            {
                this.isReplay = false;
                return false;
            }
            try
            {
                this.streamPath = path;
                this.isReplayAbc = path.EndsWith(".abc");
                this.replayStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                this.replayReader = new BinaryReader(this.replayStream);
                if (this.isReplayAbc)
                {
                    string appVersion = this.replayReader.ReadString();
                    string usedResourceVersion = this.replayReader.ReadString();
                    if (!CVersion.IsSynchronizedVersion(appVersion, usedResourceVersion))
                    {
                        this.replayReader.Close();
                        this.replayStream.Close();
                        throw new Exception("ABC version not match!");
                    }
                    this.replayReader.ReadUInt32();
                    this.replayReader.ReadInt64();
                    this.replayReader.ReadByte();
                    this.replayReader.ReadUInt32();
                    this.replayReader.ReadString();
                    this.replayReader.ReadString();
                    this.replayReader.ReadByte();
                    this.replayReader.ReadUInt32();
                }
                else
                {
                    this.bufferUsedSize = 0;
                }
                this.isReplay = true;
                return true;
            }
            catch (Exception)
            {
                this.replayStream = null;
                this.replayReader = null;
                this.isReplay = false;
                return false;
            }
        }

        public void CacheRecord(object obj)
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                CSDT_FRAPBOOT_INFO csdt_frapboot_info = obj as CSDT_FRAPBOOT_INFO;
                int usedSize = 0;
                short num2 = 0;
                if (csdt_frapboot_info != null)
                {
                    if (((csdt_frapboot_info.pack(ref this.streamBuffer, this.streamBuffer.Length, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0)) && (usedSize < 0x7fff))
                    {
                        num2 = (short) usedSize;
                    }
                    this.endKFraqNo = 0;
                }
                else
                {
                    CSPkg pkg = obj as CSPkg;
                    if (pkg.stPkgHead.dwMsgID == 0x433)
                    {
                        this.BeginRecord(pkg.stPkgData.stMultGameBeginLoad);
                    }
                    if (((pkg.pack(ref this.streamBuffer, this.streamBuffer.Length, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0)) && (usedSize < 0x7fff))
                    {
                        num2 = (short) -usedSize;
                    }
                }
                if ((this.recordWriter != null) && (num2 != 0))
                {
                    this.recordWriter.Write(num2);
                    this.recordWriter.Write(this.streamBuffer, 0, usedSize);
                }
                else
                {
                    Debug.LogError("Record Msg Failed! usedSize=" + usedSize);
                }
            }
        }

        public void ClearRecord()
        {
            this.endKFraqNo = 0;
            if (this.recordWriter != null)
            {
                this.recordWriter.Close();
                this.recordWriter = null;
            }
            if (this.recordStream != null)
            {
                this.recordStream.Close();
                this.recordStream = null;
            }
        }

        public void ClearReplay()
        {
            if (this.replayReader != null)
            {
                this.replayReader.Close();
                this.replayReader = null;
            }
            if (this.replayStream != null)
            {
                this.replayStream.Close();
                this.replayStream = null;
            }
        }

        private void DirectHandleMsg(CSPkg msg)
        {
            NetMsgDelegate msgHandler = Singleton<NetworkModule>.GetInstance().GetMsgHandler(msg.stPkgHead.dwMsgID);
            if (msgHandler != null)
            {
                try
                {
                    msgHandler(msg);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public bool FlushRecord()
        {
            bool flag;
            if (!this.HasRecord)
            {
                return false;
            }
            try
            {
                if (this.endKFraqNo > 0)
                {
                    CSDT_FRAPBOOT_INFO csdt_frapboot_info = new CSDT_FRAPBOOT_INFO {
                        dwKFrapsNo = this.endKFraqNo,
                        bNum = 0
                    };
                    int usedSize = 0;
                    if (((csdt_frapboot_info.pack(ref this.streamBuffer, this.streamBuffer.Length, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0)) && (usedSize < 0x7fff))
                    {
                        this.recordWriter.Write((short) usedSize);
                        this.recordWriter.Write(this.streamBuffer, 0, usedSize);
                    }
                }
                this.streamPath = string.Format("{0}/{1}.abc", ReplayFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                FileStream stream = new FileStream(this.streamPath, FileMode.Create, FileAccess.Write);
                this.recordStream.WriteTo(stream);
                stream.Flush();
                stream.Close();
                flag = true;
            }
            catch
            {
                flag = false;
                if (File.Exists(this.streamPath))
                {
                    File.Delete(this.streamPath);
                }
            }
            this.ClearRecord();
            return flag;
        }

        public ListView<ReplayFileInfo> ListReplayFiles(bool removeObsolete = true)
        {
            ListView<ReplayFileInfo> view = new ListView<ReplayFileInfo>();
            DirectoryInfo info = new DirectoryInfo(ReplayFolder);
            if (info.Exists)
            {
                string[] strArray = Directory.GetFiles(info.FullName, "*.abc", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < strArray.Length; i++)
                {
                    try
                    {
                        string path = strArray[i];
                        FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read);
                        BinaryReader reader = new BinaryReader(input);
                        string appVersion = reader.ReadString();
                        string usedResourceVersion = reader.ReadString();
                        bool flag = false;
                        if (CVersion.IsSynchronizedVersion(appVersion, usedResourceVersion))
                        {
                            ReplayFileInfo item = new ReplayFileInfo {
                                path = path,
                                heroId = reader.ReadUInt32(),
                                startTime = reader.ReadInt64(),
                                mapType = reader.ReadByte(),
                                mapId = reader.ReadUInt32(),
                                userName = reader.ReadString(),
                                userHead = reader.ReadString(),
                                userRankGrade = reader.ReadByte(),
                                userRankClass = reader.ReadUInt32()
                            };
                            view.Add(item);
                            flag = true;
                        }
                        reader.Close();
                        input.Close();
                        if (!flag && removeObsolete)
                        {
                            File.Delete(path);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return view;
        }

        private bool LoadMsg(out CSPkg replayMsg, out CSDT_FRAPBOOT_INFO fraqBoot)
        {
            replayMsg = null;
            fraqBoot = null;
            try
            {
                if (this.isReplayAbc)
                {
                    if ((this.replayStream == null) || (this.replayStream.Position >= this.replayStream.Length))
                    {
                        return false;
                    }
                    short num = this.replayReader.ReadInt16();
                    bool flag = num > 0;
                    num = Math.Abs(num);
                    if ((this.replayStream.Position + num) > this.replayStream.Length)
                    {
                        return false;
                    }
                    if (this.replayReader.Read(this.streamBuffer, 0, num) != num)
                    {
                        return false;
                    }
                    int usedSize = 0;
                    if (flag)
                    {
                        fraqBoot = CSDT_FRAPBOOT_INFO.New();
                        if (fraqBoot.unpack(ref this.streamBuffer, num, ref usedSize, 0) != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        replayMsg = CSPkg.New();
                        if (replayMsg.unpack(ref this.streamBuffer, num, ref usedSize, 0) != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((this.bufferUsedSize > 0) || ((this.replayStream != null) && (this.replayStream.Position < this.replayStream.Length)))
                    {
                        if ((this.replayStream != null) && (this.replayStream.Position < this.replayStream.Length))
                        {
                            this.bufferUsedSize += this.replayReader.Read(this.streamBuffer, this.bufferUsedSize, this.streamBuffer.Length - this.bufferUsedSize);
                        }
                        replayMsg = CSPkg.New();
                        int num4 = 0;
                        if (((replayMsg.unpack(ref this.streamBuffer, this.bufferUsedSize, ref num4, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (0 < num4)) && (num4 <= this.bufferUsedSize))
                        {
                            this.bufferUsedSize -= num4;
                            Buffer.BlockCopy(this.streamBuffer, num4, this.streamBuffer, 0, this.bufferUsedSize);
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
            if (this.replayStream.Position >= this.replayStream.Length)
            {
                this.ClearReplay();
            }
            return true;
        }

        private void ObjToStr(StringBuilder sb, object obj, object prtObj = null)
        {
            if (obj == null)
            {
                sb.Append("null");
            }
            else
            {
                System.Type type = obj.GetType();
                if (type.IsArray)
                {
                    sb.Append("[");
                    Array array = obj as Array;
                    int length = array.Length;
                    if (prtObj != null)
                    {
                        FieldInfo field = prtObj.GetType().GetField("wLen");
                        if (field != null)
                        {
                            length = (ushort) field.GetValue(prtObj);
                        }
                    }
                    for (int i = 0; i < length; i++)
                    {
                        object obj2 = array.GetValue(i);
                        if (obj2 != null)
                        {
                            if (i > 0)
                            {
                                sb.Append(", ");
                            }
                            this.ObjToStr(sb, obj2, obj);
                        }
                    }
                    sb.Append("]");
                }
                else if (type.IsClass)
                {
                    sb.Append("{");
                    foreach (FieldInfo info2 in type.GetFields())
                    {
                        if (!info2.IsStatic && (info2.Name != "bReserve"))
                        {
                            sb.Append(info2.Name);
                            sb.Append(": ");
                            this.ObjToStr(sb, info2.GetValue(obj), obj);
                            sb.Append("; ");
                        }
                    }
                    sb.Append("}");
                }
                else
                {
                    sb.Append(obj.ToString());
                }
            }
        }

        public void OnGameEnd()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.isReplay = false;
            this.ClearReplay();
        }

        public void SetKFraqNo(uint kFraqNo)
        {
            this.endKFraqNo = kFraqNo;
        }

        public void StopReplay()
        {
            this.isReplay = false;
            this.ClearReplay();
        }

        public void UpdateFrame()
        {
            if (this.isReplay)
            {
                for (byte i = 0; i < Singleton<FrameSynchr>.GetInstance().FrameSpeed; i = (byte) (i + 1))
                {
                    if (!MonoSingleton<GameLoader>.GetInstance().isLoadStart)
                    {
                        CSPkg pkg;
                        CSDT_FRAPBOOT_INFO csdt_frapboot_info;
                        if (this.LoadMsg(out pkg, out csdt_frapboot_info))
                        {
                            if (csdt_frapboot_info != null)
                            {
                                uint dwKFrapsNo = csdt_frapboot_info.dwKFrapsNo;
                                FrameWindow.HandleFraqBootInfo(csdt_frapboot_info);
                                if (this.replayStream == null)
                                {
                                    Singleton<FrameSynchr>.GetInstance().SetKeyFrameIndex(dwKFrapsNo + 150);
                                }
                            }
                            else if (pkg != null)
                            {
                                this.DirectHandleMsg(pkg);
                            }
                            continue;
                        }
                        this.StopReplay();
                    }
                    break;
                }
            }
        }

        public bool HasRecord
        {
            get
            {
                return ((this.recordStream != null) && (this.recordStream.Length > 0L));
            }
        }

        public bool IsStreamEnd
        {
            get
            {
                return (null == this.replayStream);
            }
        }

        public static string ReplayFolder
        {
            get
            {
                return DebugHelper.logRootPath;
            }
        }

        public string streamPath { get; private set; }

        public class ReplayFileInfo
        {
            public uint heroId;
            public uint mapId;
            public byte mapType;
            public string path;
            public long startTime;
            public string userHead;
            public string userName;
            public uint userRankClass;
            public byte userRankGrade;
        }
    }
}

