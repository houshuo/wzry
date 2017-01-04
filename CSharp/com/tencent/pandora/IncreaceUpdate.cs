namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class IncreaceUpdate
    {
        private ArrayList aryRes = new ArrayList();
        private bool blIsInitFinish;
        private Dictionary<string, int> dicFileState = new Dictionary<string, int>();
        private Dictionary<string, bool> dicInitLua = new Dictionary<string, bool>();
        private Dictionary<string, bool> dicLoaded = new Dictionary<string, bool>();
        private int iLuaInited = 1;
        private int iLuaIniting = 2;
        private int iLuaNotRead = -1;
        private int iLuaWaitInit;
        private bool isInit;
        private bool isNeedLoadLua = true;
        private int iSTATE_DOWN = 2;
        private int iSTATE_FINISH = 3;
        private int iSTATE_LOCAL = 1;
        private int ISTATE_YZ;
        public static string m_strConfigInfo = string.Empty;

        public void AddDownSuccFile(string strFile)
        {
            try
            {
                Logger.d("下载成功:" + strFile);
                if (strFile.IndexOf(",") > 0)
                {
                    char[] separator = new char[] { ',' };
                    strFile = strFile.Split(separator)[0];
                }
                if (strFile != string.Empty)
                {
                    if (this.dicFileState.ContainsKey(strFile))
                    {
                        this.dicFileState[strFile] = this.iSTATE_LOCAL;
                    }
                    else
                    {
                        this.dicFileState.Add(strFile, this.iSTATE_LOCAL);
                    }
                }
                this.RefreshDownStatus(strFile);
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public bool CheckIsDown(int state)
        {
            if (((state != this.ISTATE_YZ) && (state != this.iSTATE_LOCAL)) && (state != this.iSTATE_FINISH))
            {
                return false;
            }
            return true;
        }

        private void Dump()
        {
        }

        private string GetLuaName(string strSourceName)
        {
            try
            {
                char[] separator = new char[] { '.' };
                strSourceName = strSourceName.Split(separator)[0];
                char[] chArray2 = new char[] { '_' };
                string[] strArray2 = strSourceName.Split(chArray2);
                strSourceName = strSourceName.Replace(strArray2[0] + "_", string.Empty);
                return strSourceName;
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
                return string.Empty;
            }
        }

        private void InitResouceData(string strConfigInfo)
        {
            try
            {
                char[] separator = new char[] { '|' };
                string[] strArray = strConfigInfo.Split(separator);
                for (int i = 0; i < strArray.Length; i++)
                {
                    char[] chArray2 = new char[] { ':' };
                    string[] strArray2 = strArray[i].Split(chArray2);
                    if (strArray2.Length >= 2)
                    {
                        string str = strArray2[0];
                        string str2 = strArray2[1];
                        CRes res = new CRes {
                            strResName = str
                        };
                        char[] chArray3 = new char[] { ',' };
                        string[] strArray3 = str2.Split(chArray3);
                        for (int j = 0; j < strArray3.Length; j++)
                        {
                            string key = strArray3[j].ToString();
                            if (!this.dicFileState.ContainsKey(key))
                            {
                                this.dicFileState[key] = this.iSTATE_LOCAL;
                                Logger.d("AddToDic:" + key + ",State:" + this.iSTATE_LOCAL.ToString());
                            }
                            int istate = -1;
                            if (this.dicFileState.ContainsKey(key))
                            {
                                istate = this.dicFileState[key];
                            }
                            bool flag = this.isLuaFile(strArray3[j]);
                            res.aryFileState.Add(new CFileState(strArray3[j], istate, flag));
                        }
                        this.aryRes.Add(res);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        private bool isLuaFile(string strFile)
        {
            return (strFile.IndexOf("_lua") > 0);
        }

        public void RefreshDownStatus(string strSuccFile)
        {
            try
            {
                Logger.d("刷新下载状态:" + strSuccFile);
                for (int i = 0; i < this.aryRes.Count; i++)
                {
                    CRes res = this.aryRes[i] as CRes;
                    bool flag = true;
                    for (int j = 0; j < res.aryFileState.Count; j++)
                    {
                        CFileState state = res.aryFileState[j] as CFileState;
                        if (state.strFileName.Equals(strSuccFile))
                        {
                            state.iDownState = this.iSTATE_LOCAL;
                        }
                        else if (!this.CheckIsDown(state.iDownState))
                        {
                            flag = false;
                        }
                        if ((this.CheckIsDown(state.iDownState) && this.isLuaFile(state.strFileName)) && !this.dicLoaded.ContainsKey(state.strFileName))
                        {
                            this.dicLoaded.Add(state.strFileName, true);
                            NotificationCenter.DefaultCenter().PostNotification(null, "OnUILuaLoaded", this.GetLuaName(state.strFileName));
                        }
                    }
                    if (flag)
                    {
                        res.iLoadState = this.iSTATE_LOCAL;
                    }
                    Logger.d("--Res:" + res.strResName + ",status:" + res.iLoadState.ToString());
                }
                this.Dump();
                this.SetLuaLoadSucc(string.Empty);
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public void RefreshLuaLoadStatus()
        {
            try
            {
                if (this.isNeedLoadLua && this.blIsInitFinish)
                {
                    for (int i = 0; i < this.aryRes.Count; i++)
                    {
                        CRes res = this.aryRes[i] as CRes;
                        if (((res.iLuaState != this.iLuaInited) && this.CheckIsDown(res.iLoadState)) && ((res.iLuaState != this.iLuaIniting) && this.CheckIsDown(res.iLoadState)))
                        {
                            bool flag = false;
                            bool flag2 = true;
                            for (int k = 0; k < res.aryFileState.Count; k++)
                            {
                                CFileState state = res.aryFileState[k] as CFileState;
                                string strFileName = state.strFileName;
                                if (state.isLua)
                                {
                                    flag = true;
                                    if (this.isLuaFile(strFileName) && (!this.CheckIsDown(state.iDownState) || (state.iLuaState == -1)))
                                    {
                                        flag2 = false;
                                    }
                                }
                            }
                            if (!flag)
                            {
                                Logger.d("res:" + res.strResName + " no lua");
                                res.iLuaState = this.iLuaInited;
                            }
                            if (flag2)
                            {
                                res.iLuaState = this.iLuaWaitInit;
                            }
                            if (res.iLuaState == this.iLuaWaitInit)
                            {
                                res.iLuaState = this.iLuaIniting;
                                for (int m = 0; m < res.aryFileState.Count; m++)
                                {
                                    CFileState state2 = res.aryFileState[m] as CFileState;
                                    string key = state2.strFileName;
                                    if (state2.isLua && !this.dicInitLua.ContainsKey(key))
                                    {
                                        this.dicInitLua.Add(key, true);
                                        Logger.d("开始加载-》" + key);
                                        string luaName = this.GetLuaName(key);
                                        LuaHelper.GetResManager().LuaInitToVm(luaName);
                                    }
                                }
                                res.iLuaState = this.iLuaInited;
                            }
                        }
                    }
                    bool flag3 = true;
                    for (int j = 0; j < this.aryRes.Count; j++)
                    {
                        CRes res2 = this.aryRes[j] as CRes;
                        if (res2.iLuaState != this.iLuaInited)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                    if (flag3)
                    {
                        Logger.d("init lua finish");
                        this.isNeedLoadLua = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public void SetLuaLoadSucc(string strFile)
        {
            try
            {
                Logger.d("读取成功:" + strFile);
                for (int i = 0; i < this.aryRes.Count; i++)
                {
                    CRes res = this.aryRes[i] as CRes;
                    bool flag = true;
                    for (int j = 0; j < res.aryFileState.Count; j++)
                    {
                        CFileState state = res.aryFileState[j] as CFileState;
                        if (state.strFileName.Equals(strFile))
                        {
                            state.iLuaState = this.iLuaWaitInit;
                        }
                        else if (state.isLua && (state.iLuaState != this.iLuaWaitInit))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        res.iLuaState = this.iLuaWaitInit;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace.ToString());
            }
        }

        public void SetUpdateInfo(string strConfigInfo, string strCurState)
        {
            if (!this.isInit)
            {
                this.isInit = true;
                m_strConfigInfo = strConfigInfo;
                Logger.d("content:" + strConfigInfo + ",state:" + strCurState);
                try
                {
                    char[] separator = new char[] { ';' };
                    string[] strArray = strCurState.Split(separator);
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        char[] chArray2 = new char[] { ',' };
                        string[] strArray2 = strArray[i].Split(chArray2);
                        string key = strArray2[0];
                        int num2 = -2;
                        try
                        {
                            if (strArray2.Length < 2)
                            {
                                continue;
                            }
                            num2 = Convert.ToInt32(strArray2[1]);
                        }
                        catch (Exception exception)
                        {
                            Logger.d(exception.ToString());
                        }
                        Logger.d("       file:" + strArray[i] + ", state:" + num2.ToString());
                        if ((strArray2.Length >= 2) && (num2 != -2))
                        {
                            if (this.dicFileState.ContainsKey(key))
                            {
                                this.dicFileState[key] = num2;
                            }
                            else
                            {
                                this.dicFileState.Add(key, num2);
                            }
                        }
                    }
                    this.InitResouceData(strConfigInfo);
                    this.blIsInitFinish = true;
                    this.RefreshDownStatus(string.Empty);
                }
                catch (Exception exception2)
                {
                    Logger.d(exception2.Message);
                }
            }
        }

        private class CFileState
        {
            public int iDownState = -1;
            public int iLuaState = -1;
            public bool isLua;
            public string strFileName = string.Empty;

            public CFileState(string file, int istate, bool _isLua)
            {
                this.strFileName = file;
                this.iDownState = istate;
                this.isLua = _isLua;
                Logger.d("    iState:" + this.iDownState.ToString());
            }
        }

        private class CRes
        {
            public ArrayList aryFileState = new ArrayList();
            public int iLoadState = -1;
            public int iLuaState = -1;
            public string strResName = string.Empty;
        }
    }
}

