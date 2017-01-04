using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class TdirMgr : MonoSingleton<TdirMgr>
{
    private int apolloTimeout = 0x2710;
    public bool EnableMarkLastLogin = true;
    private const int FailTimesDuration1 = 10;
    private const int FailTimesDuration2 = 300;
    private const int FailTimesLimit1 = 3;
    private const int FailTimesLimit2 = 6;
    private IAsyncResult getAddressResult;
    private const string LoginFailTimesKey = "LoginFailTimesKey";
    private const string LoginTimeKey = "LoginTimeKey";
    public int m_connectIndex = -1;
    private TdirResult m_GetinnerResult = TdirResult.NoServiceTable;
    private float m_GetTdirTime;
    private ApolloAccountInfo m_info;
    public string[] m_iplist;
    public int[] m_portlist;
    public float m_regetTime;
    private AsyncStatu mAsyncFinished;
    private const int MaxSyncTime = 3;
    private int mCurrentNodeIDFirst = 1;
    private int mFailTimes;
    private static bool mIsCheckVersion;
    private int mLastLoginNodeID;
    private TdirUrl mLastLoginUrl;
    private DictionaryView<string, List<TdirTreeNode>> mOpenIDTreeNodeDic = new DictionaryView<string, List<TdirTreeNode>>();
    private TdirSvrGroup mOwnRoleList;
    private ApolloPlatform mPlatform;
    private TdirSvrGroup mPrivateSvrList;
    private TdirSvrGroup mRecommondUrlList;
    private TdirUrl mSelectedTdir;
    private List<TdirSvrGroup> mSvrListGroup;
    private int mSyncTime;
    private ApolloTdir mTdir;
    private TdirEnv mTdirEnv = TdirEnv.Release;
    private TdirSvrGroup mTestSvrList;
    private List<TdirTreeNode> mTreeNodes;
    private const int PrivateNodeID = 1;
    private int recvTimeout = 10;
    public const int RegetTime = 200;
    private const int RootNodeID = 0;
    public static bool s_maintainBlock = true;
    private static RootNodeType SNodeType;
    public TdirManagerEvent SvrListLoaded;
    private int syncTimeOut = 12;
    private const int TestNodeID = 2;

    private void CheckAuditVersion()
    {
        IPAddress address;
        if (IPAddress.TryParse(GameFramework.AppVersion, out address))
        {
            int num = BitConverter.ToInt32(address.GetAddressBytes(), 0);
            object param = 0;
            for (int i = 0; i < this.mTreeNodes.Count; i++)
            {
                TdirTreeNode node = this.mTreeNodes[i];
                if (this.ParseNodeAppAttr(node.staticInfo.appAttr, TdirNodeAttrType.versionOnlyExcept, ref param) && (((int) param) == num))
                {
                    mIsCheckVersion = true;
                    break;
                }
            }
        }
    }

    private bool CheckEnterTdirUrl(TdirUrl tdirUrl, bool tips = false)
    {
        if (((tdirUrl.nodeID != 0) && (tdirUrl.statu != TdirSvrStatu.UNAVAILABLE)) && (tdirUrl.addrs != null))
        {
            return true;
        }
        if (tips)
        {
        }
        return false;
    }

    public bool CheckTdirUrlValid(TdirUrl url)
    {
        return ((url.nodeID != 0) && (url.addrs != null));
    }

    private bool CheckTreeNodeValid(TdirTreeNode node)
    {
        IPAddress address;
        if (node.staticInfo.cltAttr1 == 0)
        {
            return false;
        }
        object param = new object();
        int num = 0;
        if (IPAddress.TryParse(GameFramework.AppVersion, out address))
        {
            num = BitConverter.ToInt32(address.GetAddressBytes(), 0);
        }
        else
        {
            return true;
        }
        if (this.GetTreeNodeAttr(node, TdirNodeAttrType.versionDown, ref param) && (((int) param) > num))
        {
            return false;
        }
        if (this.GetTreeNodeAttr(node, TdirNodeAttrType.versionUp, ref param) && (((int) param) < num))
        {
            return false;
        }
        return true;
    }

    public void ChooseGameServer(TdirUrl tdirurl)
    {
        this.m_connectIndex = -1;
        if (((this.mCurrentNodeIDFirst != InternalNodeID) && (tdirurl.statu == TdirSvrStatu.UNAVAILABLE)) && s_maintainBlock)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Maintaining"), false);
        }
        else
        {
            this.mSelectedTdir = tdirurl;
            if ((tdirurl.addrs == null) || (tdirurl.addrs.Count <= 0))
            {
                object[] inParameters = new object[] { tdirurl.name };
                DebugHelper.Assert(false, "{0} 后台大爷给这个区配个gameserver的url呗", inParameters);
            }
            else
            {
                MonoSingleton<CTongCaiSys>.instance.isCanUseTongCai = true;
                object param = null;
                if (this.ParseNodeAppAttr(this.SelectedTdir.attr, TdirNodeAttrType.UseTongcai, ref param) && (((int) param) == 0))
                {
                    MonoSingleton<CTongCaiSys>.instance.isCanUseTongCai = false;
                }
                Singleton<ReconnectIpSelect>.instance.Reset();
                this.ConnectLobby();
            }
        }
    }

    public void ConnectLobby()
    {
        if (this.m_connectIndex < 0)
        {
            int num = 1;
            object param = null;
            if (this.ParseNodeAppAttr(this.SelectedTdir.attr, TdirNodeAttrType.UseDeviceIDChooseSvr, ref param))
            {
                num = (int) param;
            }
            if ((num == 1) && !string.IsNullOrEmpty(SystemInfo.deviceUniqueIdentifier))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(SystemInfo.deviceUniqueIdentifier);
                MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                provider.Initialize();
                provider.TransformFinalBlock(bytes, 0, bytes.Length);
                ulong num2 = (ulong) BitConverter.ToInt64(provider.Hash, 0);
                ulong num3 = (ulong) BitConverter.ToInt64(provider.Hash, 8);
                this.m_connectIndex = (int) (num2 ^ num3);
                if (this.m_connectIndex < 0)
                {
                    this.m_connectIndex *= -1;
                }
            }
            else
            {
                this.m_connectIndex = UnityEngine.Random.Range(0, 0x2710);
            }
            this.m_connectIndex = this.m_connectIndex % MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs.Count;
            Singleton<LobbySvrMgr>.GetInstance().ConnectServerWithTdirDefault(this.m_connectIndex, ChooseSvrPolicy.DeviceID);
        }
        else
        {
            this.m_connectIndex++;
            this.m_connectIndex = this.m_connectIndex % MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs.Count;
            Singleton<LobbySvrMgr>.GetInstance().ConnectServerWithTdirDefault(this.m_connectIndex, Singleton<LobbySvrMgr>.GetInstance().chooseSvrPol);
        }
    }

    public void ConnectLobbyRandomChooseSvr(ChooseSvrPolicy policy)
    {
        this.m_connectIndex = UnityEngine.Random.Range(0, 0x2710);
        this.m_connectIndex = this.m_connectIndex % MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs.Count;
        Singleton<LobbySvrMgr>.GetInstance().ConnectServerWithTdirDefault(this.m_connectIndex, policy);
    }

    public void ConnectLobbyWithSnsNameChooseSvr()
    {
        if (!string.IsNullOrEmpty(Singleton<CLoginSystem>.GetInstance().m_nickName))
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Singleton<CLoginSystem>.GetInstance().m_nickName);
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            provider.Initialize();
            provider.TransformFinalBlock(bytes, 0, bytes.Length);
            ulong num = (ulong) BitConverter.ToInt64(provider.Hash, 0);
            ulong num2 = (ulong) BitConverter.ToInt64(provider.Hash, 8);
            this.m_connectIndex = (int) (num ^ num2);
            if (this.m_connectIndex < 0)
            {
                this.m_connectIndex *= -1;
            }
        }
        else
        {
            this.m_connectIndex = UnityEngine.Random.Range(0, 0x2710);
        }
        this.m_connectIndex = this.m_connectIndex % MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs.Count;
        Singleton<LobbySvrMgr>.GetInstance().ConnectServerWithTdirDefault(this.m_connectIndex, ChooseSvrPolicy.NickName);
    }

    private bool CreateSvrGroup(TdirTreeNode node, ref TdirSvrGroup group)
    {
        if (!this.CheckTreeNodeValid(node))
        {
            return false;
        }
        group.nodeID = node.nodeID;
        group.name = node.name;
        group.appAttr = node.staticInfo.appAttr;
        group.tdirUrls = new List<TdirUrl>();
        return true;
    }

    private bool CreateSvrUrl(TdirTreeNode node, ref TdirUrl tdirUrl)
    {
        string connectUrl = node.dynamicInfo.connectUrl;
        if (string.IsNullOrEmpty(connectUrl))
        {
            return false;
        }
        if (!this.CheckTreeNodeValid(node))
        {
            return false;
        }
        tdirUrl.nodeID = node.nodeID;
        tdirUrl.name = node.name;
        tdirUrl.statu = (TdirSvrStatu) node.status;
        if ((tdirUrl.statu == TdirSvrStatu.FINE) || (tdirUrl.statu == TdirSvrStatu.HEAVY))
        {
            tdirUrl.flag = (SvrFlag) node.svrFlag;
        }
        else
        {
            tdirUrl.flag = SvrFlag.NoFlag;
        }
        tdirUrl.mask = node.staticInfo.bitmapMask;
        tdirUrl.addrs = new List<IPAddrInfo>();
        tdirUrl.attr = node.staticInfo.appAttr;
        if (node.userRoleInfo != null)
        {
            tdirUrl.roleCount = (uint) node.userRoleInfo.Count;
            uint lastLoginTime = 0;
            for (int j = 0; j < node.userRoleInfo.Count; j++)
            {
                TdirUserRoleInfo info3 = node.userRoleInfo[j];
                if (info3.lastLoginTime > lastLoginTime)
                {
                    TdirUserRoleInfo info4 = node.userRoleInfo[j];
                    lastLoginTime = info4.lastLoginTime;
                }
            }
            tdirUrl.lastLoginTime = lastLoginTime;
        }
        else
        {
            tdirUrl.roleCount = 0;
            tdirUrl.lastLoginTime = 0;
        }
        char[] separator = new char[] { ';' };
        string[] strArray = connectUrl.Split(separator);
        if (strArray == null)
        {
            return false;
        }
        for (int i = 0; i < strArray.Length; i++)
        {
            char[] chArray2 = new char[] { ':' };
            string[] strArray2 = strArray[i].Split(chArray2);
            for (int k = 1; k < strArray2.Length; k++)
            {
                if (strArray2[k].IndexOf('-') >= 0)
                {
                    int num5;
                    int num6;
                    char[] chArray3 = new char[] { '-' };
                    string[] strArray3 = strArray2[k].Split(chArray3);
                    if (((strArray3.Length == 2) && int.TryParse(strArray3[0], out num5)) && (int.TryParse(strArray3[1], out num6) && (num5 <= num6)))
                    {
                        int num7 = (num6 - num5) + 1;
                        for (int m = 0; m < num7; m++)
                        {
                            int num9 = num5 + m;
                            IPAddrInfo item = new IPAddrInfo {
                                ip = strArray2[0],
                                port = num9.ToString()
                            };
                            tdirUrl.addrs.Add(item);
                        }
                    }
                }
                else
                {
                    IPAddrInfo info2 = new IPAddrInfo {
                        ip = strArray2[0],
                        port = strArray2[k]
                    };
                    tdirUrl.addrs.Add(info2);
                }
            }
        }
        object param = null;
        if (this.ParseNodeAppAttr(tdirUrl.attr, TdirNodeAttrType.LogicWorldID, ref param))
        {
            tdirUrl.logicWorldID = (int) param;
        }
        else
        {
            tdirUrl.logicWorldID = 0;
        }
        object obj3 = null;
        if (this.ParseNodeAppAttr(tdirUrl.attr, TdirNodeAttrType.UseNetAcc, ref obj3))
        {
            tdirUrl.useNetAcc = (bool) obj3;
        }
        else
        {
            tdirUrl.useNetAcc = false;
        }
        return true;
    }

    private bool CreateSvrUrl(TdirUrl srcUrl, ref TdirUrl tdirUrl)
    {
        if (!this.CheckTdirUrlValid(srcUrl))
        {
            tdirUrl.nodeID = 0;
            return false;
        }
        tdirUrl.nodeID = srcUrl.nodeID;
        tdirUrl.name = srcUrl.name;
        tdirUrl.statu = srcUrl.statu;
        tdirUrl.flag = srcUrl.flag;
        tdirUrl.mask = srcUrl.mask;
        tdirUrl.roleCount = srcUrl.roleCount;
        tdirUrl.attr = srcUrl.attr;
        tdirUrl.addrs = new List<IPAddrInfo>(srcUrl.addrs);
        tdirUrl.logicWorldID = srcUrl.logicWorldID;
        return true;
    }

    public void Dispose()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.TDir_QuitGame, new CUIEventManager.OnUIEventHandler(this.OnQuitGame));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.TDir_TryAgain, new CUIEventManager.OnUIEventHandler(this.OnTryAgain));
    }

    public void EnterGame(TdirUrl tdirUrl)
    {
        this.mSelectedTdir = tdirUrl;
        this.SetConfigAtClickTdir(this.mSelectedTdir);
    }

    public TdirUrl GetDefaultTdirUrl()
    {
        if (this.CheckTdirUrlValid(this.mLastLoginUrl))
        {
            return this.mLastLoginUrl;
        }
        if (this.RecommondUrlList.tdirUrls != null)
        {
            for (int i = 0; i < this.RecommondUrlList.tdirUrls.Count; i++)
            {
                if (this.CheckTdirUrlValid(this.RecommondUrlList.tdirUrls[i]))
                {
                    return this.RecommondUrlList.tdirUrls[i];
                }
            }
        }
        if (this.mSvrListGroup != null)
        {
            for (int j = 0; j < this.mSvrListGroup.Count; j++)
            {
                TdirSvrGroup group6;
                TdirSvrGroup group8;
                TdirSvrGroup group5 = this.mSvrListGroup[j];
                if (group5.tdirUrls == null)
                {
                    continue;
                }
                int num3 = 0;
                goto Label_00FF;
            Label_00B9:
                group6 = this.mSvrListGroup[j];
                if (this.CheckTdirUrlValid(group6.tdirUrls[num3]))
                {
                    TdirSvrGroup group7 = this.mSvrListGroup[j];
                    return group7.tdirUrls[num3];
                }
                num3++;
            Label_00FF:
                group8 = this.mSvrListGroup[j];
                if (num3 < group8.tdirUrls.Count)
                {
                    goto Label_00B9;
                }
            }
        }
        return new TdirUrl();
    }

    public string GetDianXingIP()
    {
        return this.GetIpByIspCode(1);
    }

    public string GetIpByIspCode(int code)
    {
        object param = 0;
        if (this.ParseNodeAppAttr(this.SelectedTdir.attr, TdirNodeAttrType.ISPChoose, ref param))
        {
            Dictionary<string, int> dictionary = (Dictionary<string, int>) param;
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, int> pair in dictionary)
                {
                    if (pair.Value == code)
                    {
                        return pair.Key;
                    }
                }
            }
        }
        return null;
    }

    private int GetIPPosByNodeID(int nodeID, int pos)
    {
        return (nodeID & (((int) 0xff) << (pos * 8)));
    }

    public int GetISP()
    {
        if (this.mTdir == null)
        {
            return 0;
        }
        TreeCommonData treeCommonData = this.mTdir.GetTreeCommonData();
        if (treeCommonData.ispCode == 0)
        {
            return 0;
        }
        if (treeCommonData.ispCode == 1)
        {
            return 1;
        }
        if (treeCommonData.ispCode == 2)
        {
            return 2;
        }
        if (treeCommonData.ispCode == 5)
        {
            return 3;
        }
        return 4;
    }

    private bool GetLastLoginNode(int nodeID, ref TdirUrl url)
    {
        if (((((this.mSvrListGroup == null) || (nodeID == 0)) || ((this.mPlatform == ApolloPlatform.QQ) && (this.GetIPPosByNodeID(nodeID, 0) != QQNodeID))) || ((this.mPlatform == ApolloPlatform.Wechat) && (this.GetIPPosByNodeID(nodeID, 0) != WeixinNodeID))) || ((this.mPlatform == ApolloPlatform.Guest) && (this.GetIPPosByNodeID(nodeID, 0) != GuestNodeID)))
        {
            url.nodeID = 0;
            return false;
        }
        for (int i = 0; i < this.mSvrListGroup.Count; i++)
        {
            TdirSvrGroup group = this.mSvrListGroup[i];
            List<TdirUrl> tdirUrls = group.tdirUrls;
            for (int j = 0; j < tdirUrls.Count; j++)
            {
                TdirUrl url2 = tdirUrls[j];
                if (url2.nodeID == nodeID)
                {
                    return this.CreateSvrUrl(tdirUrls[j], ref url);
                }
            }
        }
        return false;
    }

    public string GetLianTongIP()
    {
        return this.GetIpByIspCode(2);
    }

    private int GetNodeIDByPos(int first = 0, int second = 0, int third = 0, int forth = 0)
    {
        if (((first == 0) && (second == 0)) && ((third == 0) && (forth == 0)))
        {
            return 0;
        }
        return (((first | (second << 8)) | (third << 0x10)) | (forth << 0x18));
    }

    public bool GetTreeNodeAttr(TdirTreeNode node, TdirNodeAttrType attrType, ref object param)
    {
        return this.ParseNodeAppAttr(node.staticInfo.appAttr, attrType, ref param);
    }

    public string GetYiDongIP()
    {
        return this.GetIpByIspCode(3);
    }

    protected override void Init()
    {
        this.mTdir = new ApolloTdir();
        if (PlayerPrefs.HasKey("LoginFailTimesKey"))
        {
            this.FailTimes = PlayerPrefs.GetInt("LoginFailTimesKey");
        }
        this.apolloTimeout = PlayerPrefs.GetInt(TdirNodeAttrType.tdirTimeOut.ToString(), this.apolloTimeout);
        this.syncTimeOut = PlayerPrefs.GetInt(TdirNodeAttrType.tdirSyncTimeOut.ToString(), this.syncTimeOut);
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.TDir_QuitGame, new CUIEventManager.OnUIEventHandler(this.OnQuitGame));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.TDir_TryAgain, new CUIEventManager.OnUIEventHandler(this.OnTryAgain));
    }

    private void InitSvrList()
    {
        this.m_connectIndex = -1;
        if (this.mPlatform == ApolloPlatform.QQ)
        {
            this.mCurrentNodeIDFirst = QQNodeID;
        }
        else if (this.mPlatform == ApolloPlatform.Wechat)
        {
            this.mCurrentNodeIDFirst = WeixinNodeID;
        }
        else if (this.mPlatform == ApolloPlatform.Guest)
        {
            this.mCurrentNodeIDFirst = GuestNodeID;
        }
        this.ResetLastLoginUrl();
        this.ResetSelectedUrl();
        this.mOwnRoleList = new TdirSvrGroup();
        this.mOwnRoleList.name = Singleton<CTextManager>.instance.GetText("Tdir_My_Svr");
        this.mOwnRoleList.tdirUrls = new List<TdirUrl>();
        this.mRecommondUrlList = new TdirSvrGroup();
        this.mRecommondUrlList.name = Singleton<CTextManager>.instance.GetText("Tdir_Rcmd_Svr");
        this.mRecommondUrlList.tdirUrls = new List<TdirUrl>();
        this.mPrivateSvrList = new TdirSvrGroup();
        this.mPrivateSvrList.name = "PrivateSvrList";
        this.mPrivateSvrList.nodeID = this.GetNodeIDByPos(this.mCurrentNodeIDFirst, 1, 0, 0);
        this.mPrivateSvrList.tdirUrls = new List<TdirUrl>();
        this.mTestSvrList = new TdirSvrGroup();
        this.mTestSvrList.name = "TestSvrList";
        this.mTestSvrList.nodeID = this.GetNodeIDByPos(this.mCurrentNodeIDFirst, 2, 0, 0);
        this.mTestSvrList.tdirUrls = new List<TdirUrl>();
        this.mSvrListGroup = new List<TdirSvrGroup>();
        this.mSvrListGroup.Add(this.mOwnRoleList);
        this.mSvrListGroup.Add(this.mRecommondUrlList);
        if (this.mTdirEnv == TdirEnv.Test)
        {
            this.mSvrListGroup.Add(this.mPrivateSvrList);
            this.mSvrListGroup.Add(this.mTestSvrList);
        }
        this.ParseNodeInfo();
        this.mOwnRoleList.tdirUrls.Sort(new Comparison<TdirUrl>(this.SortTdirUrl));
        if (((this.mLastLoginUrl.nodeID > 0) && (this.LastLoginUrl.name != null)) && (this.LastLoginUrl.name.Length != 0))
        {
            this.mSelectedTdir = this.mLastLoginUrl;
        }
        else if (this.mRecommondUrlList.tdirUrls.Count > 0)
        {
            this.mSelectedTdir = this.mRecommondUrlList.tdirUrls[0];
        }
        else if (this.mSvrListGroup.Count > 0)
        {
            TdirSvrGroup group5 = this.mSvrListGroup[this.mSvrListGroup.Count - 1];
            if (group5.tdirUrls.Count > 0)
            {
                TdirSvrGroup group6 = this.mSvrListGroup[this.mSvrListGroup.Count - 1];
                int count = group6.tdirUrls.Count;
                TdirSvrGroup group7 = this.mSvrListGroup[this.mSvrListGroup.Count - 1];
                this.mSelectedTdir = group7.tdirUrls[count - 1];
            }
        }
        if (this.SvrListLoaded != null)
        {
            this.SvrListLoaded();
        }
    }

    private void InitTdir(ApolloPlatform platform)
    {
        this.mPlatform = platform;
        this.InitSvrList();
    }

    private void OnQuitGame(CUIEvent uiEvent)
    {
        SGameApplication.Quit();
    }

    private void OnTryAgain(CUIEvent uiEvent)
    {
        this.TdirAsync(this.m_info, null, null, true);
    }

    public bool ParseNodeAppAttr(string attr, TdirNodeAttrType attrType, ref object param)
    {
        if (!string.IsNullOrEmpty(attr))
        {
            attr = attr.ToLower();
            char[] separator = new char[] { ';' };
            string[] strArray = attr.Split(separator);
            if (strArray == null)
            {
                return false;
            }
            string str = attrType.ToString().ToLower();
            for (int i = 0; i < strArray.Length; i++)
            {
                char[] chArray4;
                Dictionary<string, int> dictionary;
                string[] strArray3;
                int num2;
                IPAddress address;
                char[] chArray2 = new char[] { ':' };
                string[] strArray2 = strArray[i].Split(chArray2);
                if ((((strArray2 != null) && (strArray2.Length == 2)) && (!string.IsNullOrEmpty(strArray2[0]) && !string.IsNullOrEmpty(strArray2[1]))) && (strArray2[0].ToLower() == str))
                {
                    switch (attrType)
                    {
                        case TdirNodeAttrType.recommond:
                        case TdirNodeAttrType.enableTSS:
                        case TdirNodeAttrType.backTime:
                        case TdirNodeAttrType.msdkLogMem:
                        case TdirNodeAttrType.tdirTimeOut:
                        case TdirNodeAttrType.tdirSyncTimeOut:
                        case TdirNodeAttrType.waitHurtActionDone:
                        case TdirNodeAttrType.unloadValidCnt:
                        case TdirNodeAttrType.checkdevice:
                        case TdirNodeAttrType.authorParam:
                        case TdirNodeAttrType.autoReplaceActorParam:
                        case TdirNodeAttrType.socketRecvBuffer:
                        case TdirNodeAttrType.reconnectMaxCnt:
                        case TdirNodeAttrType.LogicWorldID:
                        case TdirNodeAttrType.UseDeviceIDChooseSvr:
                        case TdirNodeAttrType.IOSX:
                        case TdirNodeAttrType.UseTongcai:
                        case TdirNodeAttrType.ShowTCBtn:
                        {
                            int num4 = 0;
                            if (!int.TryParse(strArray2[1], out num4))
                            {
                                break;
                            }
                            param = num4;
                            return true;
                        }
                        case TdirNodeAttrType.ISPChoose:
                        {
                            dictionary = new Dictionary<string, int>();
                            char[] chArray3 = new char[] { '*' };
                            strArray3 = strArray2[1].Split(chArray3);
                            num2 = 0;
                            goto Label_0196;
                        }
                        case TdirNodeAttrType.versionDown:
                        case TdirNodeAttrType.versionUp:
                        case TdirNodeAttrType.versionOnlyExcept:
                            IPAddress address2;
                            if (!IPAddress.TryParse(strArray2[1], out address2))
                            {
                                break;
                            }
                            param = BitConverter.ToInt32(address2.GetAddressBytes(), 0);
                            return true;

                        case TdirNodeAttrType.UseNetAcc:
                        {
                            int num5 = 0;
                            if (!int.TryParse(strArray2[1], out num5))
                            {
                                break;
                            }
                            if (num5 == 0)
                            {
                                goto Label_0219;
                            }
                            param = true;
                            goto Label_0221;
                        }
                    }
                }
                continue;
            Label_012C:
                chArray4 = new char[] { '$' };
                string[] strArray4 = strArray3[num2].Split(chArray4);
                DebugHelper.Assert(strArray4.Length == 2, "后台大爷把用于ISP解析的字符串配错了");
                int result = 0;
                int.TryParse(strArray4[0], out result);
                if (!IPAddress.TryParse(strArray4[1], out address))
                {
                    DebugHelper.Assert(false, "后台大爷把用于ISP解析的字符串配错了,ip解析不了");
                }
                dictionary.Add(strArray4[1], result);
                num2++;
            Label_0196:
                if (num2 < strArray3.Length)
                {
                    goto Label_012C;
                }
                param = dictionary;
                return true;
            Label_0219:
                param = false;
            Label_0221:
                return true;
            }
        }
        return false;
    }

    private void ParseNodeInfo()
    {
        if (this.mTreeNodes != null)
        {
            object param = new object();
            for (int i = 0; i < this.mTreeNodes.Count; i++)
            {
                TdirTreeNode node = this.mTreeNodes[i];
                if (this.GetIPPosByNodeID(node.nodeID, 0) != this.mCurrentNodeIDFirst)
                {
                    continue;
                }
                TdirUrl tdirUrl = new TdirUrl();
                if (!this.CreateSvrUrl(this.mTreeNodes[i], ref tdirUrl))
                {
                    continue;
                }
                if (tdirUrl.roleCount != 0)
                {
                    this.mOwnRoleList.tdirUrls.Add(tdirUrl);
                }
                if (tdirUrl.flag != SvrFlag.Recommend)
                {
                    TdirTreeNode node2 = this.mTreeNodes[i];
                    if (!this.ParseNodeAppAttr(node2.staticInfo.appAttr, TdirNodeAttrType.recommond, ref param) || (((int) param) == 0))
                    {
                        goto Label_00D9;
                    }
                }
                this.mRecommondUrlList.tdirUrls.Add(tdirUrl);
            Label_00D9:
                if ((tdirUrl.roleCount > 0) && (this.mLastLoginUrl.lastLoginTime < tdirUrl.lastLoginTime))
                {
                    this.mLastLoginUrl = tdirUrl;
                }
                TdirTreeNode node3 = this.mTreeNodes[i];
                if (node3.parentID != 0)
                {
                    TdirTreeNode node4 = this.mTreeNodes[i];
                    if (node4.nodeType == 0)
                    {
                        TdirSvrGroup group = new TdirSvrGroup();
                        if (this.CreateSvrGroup(this.mTreeNodes[i], ref group))
                        {
                            this.mSvrListGroup.Add(group);
                        }
                    }
                    else
                    {
                        bool flag = false;
                        for (int j = 0; j < this.mSvrListGroup.Count; j++)
                        {
                            TdirTreeNode node5 = this.mTreeNodes[i];
                            TdirSvrGroup group3 = this.mSvrListGroup[j];
                            if (node5.parentID == group3.nodeID)
                            {
                                TdirSvrGroup group4 = this.mSvrListGroup[j];
                                group4.tdirUrls.Add(tdirUrl);
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            for (int k = 0; k < this.mTreeNodes.Count; k++)
                            {
                                TdirTreeNode node6 = this.mTreeNodes[i];
                                TdirTreeNode node7 = this.mTreeNodes[k];
                                if (node6.parentID == node7.nodeID)
                                {
                                    TdirSvrGroup group2 = new TdirSvrGroup();
                                    if (this.CreateSvrGroup(this.mTreeNodes[k], ref group2))
                                    {
                                        group2.tdirUrls.Add(tdirUrl);
                                        this.mSvrListGroup.Add(group2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator QueryTdirAsync(ApolloAccountInfo info, Action successCallBack, Action failCallBack, bool reasync)
    {
        return new <QueryTdirAsync>c__Iterator23 { info = info, failCallBack = failCallBack, reasync = reasync, successCallBack = successCallBack, <$>info = info, <$>failCallBack = failCallBack, <$>reasync = reasync, <$>successCallBack = successCallBack, <>f__this = this };
    }

    private void ResetLastLoginUrl()
    {
        this.mLastLoginUrl.logicWorldID = 0;
        this.mLastLoginUrl.nodeID = 0;
        this.mLastLoginUrl.lastLoginTime = 0;
        this.mLastLoginUrl.name = null;
        this.mLastLoginUrl.flag = SvrFlag.NoFlag;
        this.mLastLoginUrl.attr = null;
        this.mLastLoginUrl.statu = TdirSvrStatu.UNAVAILABLE;
        this.mLastLoginUrl.addrs = null;
    }

    private void ResetSelectedUrl()
    {
        this.mSelectedTdir.logicWorldID = 0;
        this.mSelectedTdir.nodeID = 0;
        this.mSelectedTdir.lastLoginTime = 0;
        this.mSelectedTdir.name = null;
        this.mSelectedTdir.flag = SvrFlag.NoFlag;
        this.mSelectedTdir.attr = null;
        this.mSelectedTdir.statu = TdirSvrStatu.UNAVAILABLE;
        this.mSelectedTdir.addrs = null;
    }

    private void SetConfigAtClickTdir(TdirUrl tdirUrl)
    {
        if (tdirUrl.nodeID != 0)
        {
            object param = null;
            if (!this.ParseNodeAppAttr(tdirUrl.attr, TdirNodeAttrType.reconnectMaxCnt, ref param) || (((int) param) < 0))
            {
            }
        }
    }

    private void SetGlobalConfig()
    {
        if ((this.mTreeNodes != null) && (this.mTreeNodes.Count != 0))
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < this.mTreeNodes.Count; i++)
            {
                object param = new object();
                if (!flag && this.GetTreeNodeAttr(this.mTreeNodes[i], TdirNodeAttrType.enableTSS, ref param))
                {
                    PlayerPrefs.SetInt("EnableTSS", (int) param);
                    flag = true;
                }
                if (!flag2 && this.GetTreeNodeAttr(this.mTreeNodes[i], TdirNodeAttrType.tdirTimeOut, ref param))
                {
                    PlayerPrefs.SetInt(TdirNodeAttrType.tdirTimeOut.ToString(), (int) param);
                    flag2 = true;
                    this.apolloTimeout = (int) param;
                }
                if (this.GetTreeNodeAttr(this.mTreeNodes[i], TdirNodeAttrType.tdirSyncTimeOut, ref param))
                {
                    PlayerPrefs.SetInt(TdirNodeAttrType.tdirSyncTimeOut.ToString(), (int) param);
                    flag3 = true;
                    this.syncTimeOut = (int) param;
                }
                if ((flag && flag2) && flag3)
                {
                    return;
                }
            }
        }
    }

    public void SetIpAndPort()
    {
        this.m_iplist = TdirConfig.GetTdirIPList();
        this.m_portlist = TdirConfig.GetTdirPortList();
    }

    public bool ShowBuyTongCaiBtn()
    {
        object param = null;
        if (this.ParseNodeAppAttr(this.SelectedTdir.attr, TdirNodeAttrType.ShowTCBtn, ref param))
        {
            int num = (int) param;
            if (num == 1)
            {
                return true;
            }
        }
        return false;
    }

    private int SortTdirUrl(TdirUrl url1, TdirUrl url2)
    {
        int nodeID = url1.nodeID;
        int num2 = url2.nodeID;
        if (nodeID > num2)
        {
            return 1;
        }
        if (nodeID == num2)
        {
            return 0;
        }
        return -1;
    }

    public void StartGetHostAddress(TdirUrl tdirurl)
    {
        IPAddress address;
        bool flag = false;
        IPAddrInfo info = tdirurl.addrs[0];
        if (IPAddress.TryParse(info.ip, out address))
        {
            long num = BitConverter.ToInt32(address.GetAddressBytes(), 0);
            if (num > 0L)
            {
                flag = true;
            }
        }
        ApolloConfig.ISPType = 0;
        if (!flag)
        {
            if (this.getAddressResult == null)
            {
                IPAddrInfo info2 = tdirurl.addrs[0];
                this.getAddressResult = Dns.BeginGetHostAddresses(info2.ip, null, null);
            }
            if (this.getAddressResult == null)
            {
                object param = 0;
                if (this.ParseNodeAppAttr(tdirurl.attr, TdirNodeAttrType.ISPChoose, ref param))
                {
                    Dictionary<string, int> dictionary = (Dictionary<string, int>) param;
                    if (dictionary != null)
                    {
                        foreach (KeyValuePair<string, int> pair in dictionary)
                        {
                            if (pair.Value == this.GetISP())
                            {
                                IPAddrInfo info3 = tdirurl.addrs[0];
                                ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", pair.Key, info3.port);
                                ApolloConfig.loginOnlyIpOrUrl = pair.Key;
                                IPAddrInfo info4 = tdirurl.addrs[0];
                                ApolloConfig.loginOnlyVPort = ushort.Parse(info4.port);
                                ApolloConfig.ISPType = this.GetISP();
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            IPAddrInfo info5 = tdirurl.addrs[0];
            IPAddrInfo info6 = tdirurl.addrs[0];
            ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", info5.ip, info6.port);
            IPAddrInfo info7 = tdirurl.addrs[0];
            ApolloConfig.loginOnlyIpOrUrl = info7.ip;
            IPAddrInfo info8 = tdirurl.addrs[0];
            ApolloConfig.loginOnlyVPort = ushort.Parse(info8.port);
            ApolloConfig.ISPType = 1;
        }
    }

    public void TdirAsync(ApolloAccountInfo info, Action successCallBack, Action failCallBack, bool reasync)
    {
        this.m_info = info;
        this.mSyncTime = 0;
        this.SetIpAndPort();
        this.m_GetTdirTime = Time.time;
        base.StartCoroutine(this.QueryTdirAsync(info, successCallBack, failCallBack, reasync));
    }

    private void Update()
    {
    }

    public AsyncStatu AsyncFinished
    {
        get
        {
            return this.mAsyncFinished;
        }
    }

    private int FailTimes
    {
        get
        {
            return this.mFailTimes;
        }
        set
        {
            PlayerPrefs.SetInt("LoginFailTimesKey", value);
            this.mFailTimes = value;
        }
    }

    private static int GuestNodeID
    {
        get
        {
            if (SNodeType != RootNodeType.Normal)
            {
                if (SNodeType == RootNodeType.TestFlight)
                {
                    return 10;
                }
                if (SNodeType == RootNodeType.TestSpecial)
                {
                    return 13;
                }
            }
            return (!mIsCheckVersion ? 4 : 7);
        }
    }

    private static int InternalNodeID
    {
        get
        {
            return (!mIsCheckVersion ? 1 : 1);
        }
    }

    public static bool IsCheckVersion
    {
        get
        {
            return mIsCheckVersion;
        }
    }

    public TdirUrl LastLoginUrl
    {
        get
        {
            return this.mLastLoginUrl;
        }
    }

    public TdirSvrGroup OwnRoleList
    {
        get
        {
            return this.mOwnRoleList;
        }
    }

    private static int QQNodeID
    {
        get
        {
            if (SNodeType != RootNodeType.Normal)
            {
                if (SNodeType == RootNodeType.TestFlight)
                {
                    return 8;
                }
                if (SNodeType == RootNodeType.TestSpecial)
                {
                    return 11;
                }
            }
            return (!mIsCheckVersion ? 2 : 5);
        }
    }

    public TdirSvrGroup RecommondUrlList
    {
        get
        {
            return this.mRecommondUrlList;
        }
    }

    public TdirUrl SelectedTdir
    {
        get
        {
            return this.mSelectedTdir;
        }
    }

    public List<TdirSvrGroup> SvrUrlList
    {
        get
        {
            return this.mSvrListGroup;
        }
    }

    private static int WeixinNodeID
    {
        get
        {
            if (SNodeType != RootNodeType.Normal)
            {
                if (SNodeType == RootNodeType.TestFlight)
                {
                    return 9;
                }
                if (SNodeType == RootNodeType.TestSpecial)
                {
                    return 12;
                }
            }
            return (!mIsCheckVersion ? 3 : 6);
        }
    }

    [CompilerGenerated]
    private sealed class <QueryTdirAsync>c__Iterator23 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Action <$>failCallBack;
        internal ApolloAccountInfo <$>info;
        internal bool <$>reasync;
        internal Action <$>successCallBack;
        internal TdirMgr <>f__this;
        internal int <errorcode>__5;
        internal int <errorcode>__7;
        internal List<KeyValuePair<string, string>> <events>__4;
        internal List<KeyValuePair<string, string>> <events>__6;
        internal TdirResult <getTreeResult>__3;
        internal TdirResult <innerResult>__2;
        internal TdirResult <queryResult>__0;
        internal float <time>__1;
        internal Action failCallBack;
        internal ApolloAccountInfo info;
        internal bool reasync;
        internal Action successCallBack;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (this.<>f__this.mAsyncFinished != AsyncStatu.IsAsyncing)
                    {
                        this.<>f__this.mAsyncFinished = AsyncStatu.AsyncFail;
                        if (this.info == null)
                        {
                            if (this.failCallBack != null)
                            {
                                this.failCallBack();
                            }
                        }
                        else
                        {
                            if (!this.reasync && this.<>f__this.mOpenIDTreeNodeDic.ContainsKey(this.info.OpenId + this.info.Platform))
                            {
                                this.<>f__this.mTreeNodes = this.<>f__this.mOpenIDTreeNodeDic[this.info.OpenId + this.info.Platform];
                                this.<>f__this.mAsyncFinished = AsyncStatu.AsyncSuccess;
                                goto Label_03BF;
                            }
                            this.<queryResult>__0 = this.<>f__this.mTdir.Query(TdirConfig.GetTdirAppId(), this.<>f__this.m_iplist, this.<>f__this.m_portlist, string.Empty, 0, this.info.OpenId, false);
                            this.<>f__this.mTdir.SetSvrTimeout(this.<>f__this.apolloTimeout);
                            if (this.<queryResult>__0 != TdirResult.TdirNoError)
                            {
                                if (this.failCallBack != null)
                                {
                                    this.failCallBack();
                                }
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_WeakNetwork"), enUIEventID.TDir_TryAgain, enUIEventID.TDir_QuitGame, Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_Retry"), Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_Exit"), false);
                            }
                            else
                            {
                                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                                this.<time>__1 = 0f;
                                this.<>f__this.mSyncTime++;
                                break;
                            }
                        }
                    }
                    goto Label_0773;

                case 1:
                    break;

                default:
                    goto Label_0773;
            }
            while ((this.<>f__this.mTdir.Status() != TdirResult.RecvDone) && ((this.<>f__this.syncTimeOut == 0) || (this.<time>__1 < this.<>f__this.syncTimeOut)))
            {
                this.<time>__1 += Time.deltaTime;
                this.<innerResult>__2 = this.<>f__this.mTdir.Recv(this.<>f__this.recvTimeout);
                this.<>f__this.m_GetinnerResult = this.<innerResult>__2;
                this.<>f__this.mAsyncFinished = AsyncStatu.IsAsyncing;
                if (this.<innerResult>__2 == TdirResult.AllIpConnectFail)
                {
                    this.<>f__this.mAsyncFinished = AsyncStatu.AsyncFail;
                    break;
                }
                this.$current = new WaitForSeconds(0.01f);
                this.$PC = 1;
                return true;
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (this.<>f__this.mTdir.Status() == TdirResult.RecvDone)
            {
                this.<getTreeResult>__3 = this.<>f__this.mTdir.GetTreeNodes(ref this.<>f__this.mTreeNodes);
                if (this.<getTreeResult>__3 == TdirResult.TdirNoError)
                {
                    this.<>f__this.mAsyncFinished = AsyncStatu.AsyncSuccess;
                    if (this.<>f__this.mOpenIDTreeNodeDic.ContainsKey(this.info.OpenId + this.info.Platform))
                    {
                        this.<>f__this.mOpenIDTreeNodeDic[this.info.OpenId + this.info.Platform] = this.<>f__this.mTreeNodes;
                    }
                    else
                    {
                        this.<>f__this.mOpenIDTreeNodeDic.Add(this.info.OpenId + this.info.Platform, this.<>f__this.mTreeNodes);
                    }
                }
            }
        Label_03BF:
            if (this.<>f__this.mAsyncFinished != AsyncStatu.AsyncSuccess)
            {
                this.<>f__this.mAsyncFinished = AsyncStatu.AsyncFail;
                if (this.<>f__this.mSyncTime < 3)
                {
                    this.<>f__this.m_GetTdirTime = Time.time;
                    MonoSingleton<CTongCaiSys>.instance.isCanUseTongCai = false;
                    this.<>f__this.SetIpAndPort();
                    this.<>f__this.StartCoroutine(this.<>f__this.QueryTdirAsync(this.info, this.successCallBack, this.failCallBack, false));
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_WeakNetwork"), enUIEventID.TDir_TryAgain, enUIEventID.TDir_QuitGame, Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_Retry"), Singleton<CTextManager>.GetInstance().GetText("ChooseSvr_Exit"), false);
                }
                this.<events>__4 = new List<KeyValuePair<string, string>>();
                this.<events>__4.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
                this.<events>__4.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
                this.<events>__4.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
                this.<events>__4.Add(new KeyValuePair<string, string>("openid", "NULL"));
                float num2 = Time.time - this.<>f__this.m_GetTdirTime;
                this.<events>__4.Add(new KeyValuePair<string, string>("totaltime", num2.ToString()));
                this.<errorcode>__5 = (int) this.<>f__this.m_GetinnerResult;
                this.<events>__4.Add(new KeyValuePair<string, string>("errorcode", this.<errorcode>__5.ToString()));
                this.<events>__4.Add(new KeyValuePair<string, string>("error_msg", "null"));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_SelectServer", this.<events>__4, true);
            }
            else
            {
                TdirMgr.mIsCheckVersion = false;
                this.<>f__this.SetGlobalConfig();
                try
                {
                    if (!System.IO.File.Exists(Application.persistentDataPath + "/ServerListCfg.xml"))
                    {
                        this.<>f__this.CheckAuditVersion();
                    }
                }
                catch (Exception)
                {
                    this.<>f__this.CheckAuditVersion();
                }
                this.<>f__this.InitTdir(this.info.Platform);
                this.<>f__this.mSyncTime = 0;
                if (this.successCallBack != null)
                {
                    this.successCallBack();
                }
                if (!TdirMgr.IsCheckVersion)
                {
                }
                this.<events>__6 = new List<KeyValuePair<string, string>>();
                this.<events>__6.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
                this.<events>__6.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
                this.<events>__6.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
                this.<events>__6.Add(new KeyValuePair<string, string>("openid", "NULL"));
                float num3 = Time.time - this.<>f__this.m_GetTdirTime;
                this.<events>__6.Add(new KeyValuePair<string, string>("totaltime", num3.ToString()));
                this.<errorcode>__7 = (int) this.<>f__this.m_GetinnerResult;
                this.<events>__6.Add(new KeyValuePair<string, string>("errorcode", this.<errorcode>__7.ToString()));
                this.<events>__6.Add(new KeyValuePair<string, string>("error_msg", "null"));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_SelectServer", this.<events>__6, true);
                Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime = Time.time;
            }
            this.$PC = -1;
        Label_0773:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    public delegate void TdirManagerEvent();
}

