namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ApolloTdir : ApolloObject, IApolloTdir
    {
        private bool isServiceTableEnable = false;
        private int m_appID;
        private string[] m_ipList;
        private string m_lastSuccessIP;
        private int m_lastSuccessPort;
        private string m_openID;
        private int[] m_portList;
        private TreeCommonData m_treeCommomData;
        private List<TdirTreeNode> m_treenodes;
        private DictionaryView<int, List<TdirUserRoleInfo>> m_treeRoleInfoDic = new DictionaryView<int, List<TdirUserRoleInfo>>();
        private TdirResult mErrorCode = TdirResult.TdirNoError;
        private string mErrorString = "no error";
        private bool mLog = false;
        private TdirServiceTable mServiceTable;

        public TdirResult DisableLog()
        {
            this.mLog = false;
            return tcls_disable_log(base.ObjectId);
        }

        private void Dump()
        {
            Console.WriteLine("AppID:" + this.m_appID);
            Console.WriteLine("ip: " + string.Join("|", this.m_ipList));
            Console.WriteLine("port: " + string.Join("|", this.ToStringArray(this.m_portList)));
            Console.WriteLine("lastSuccessIP: " + this.m_lastSuccessIP);
            Console.WriteLine("lastSuccessPort: " + this.m_lastSuccessPort);
            Console.WriteLine("openID: " + this.m_openID);
        }

        public TdirResult EnableLog()
        {
            this.mLog = true;
            return tcls_enable_log(base.ObjectId);
        }

        public TdirResult GetErrorCode()
        {
            if (this.mErrorCode == TdirResult.TdirNoError)
            {
                return tcls_get_errCode(base.ObjectId);
            }
            return this.mErrorCode;
        }

        public string GetErrorString()
        {
            if ("no error" == this.mErrorString)
            {
                return Marshal.PtrToStringAnsi(tcls_get_errString(base.ObjectId));
            }
            return this.mErrorString;
        }

        public TdirResult GetServiceTable(ref TdirServiceTable table)
        {
            if (tcls_status(base.ObjectId) != TdirResult.RecvDone)
            {
                return TdirResult.NeedRecvDoneStatus;
            }
            if (this.isServiceTableEnable)
            {
                table = this.mServiceTable;
                return TdirResult.TdirNoError;
            }
            return TdirResult.NoServiceTable;
        }

        public TreeCommonData GetTreeCommonData()
        {
            return this.m_treeCommomData;
        }

        public TdirResult GetTreeNodes(ref List<TdirTreeNode> nodeList)
        {
            if (tcls_status(base.ObjectId) == TdirResult.RecvDone)
            {
                nodeList = this.m_treenodes;
                return TdirResult.TdirNoError;
            }
            return TdirResult.NeedRecvDoneStatus;
        }

        private void Log(string msg)
        {
            this.TdirLog(msg);
        }

        public TdirResult Query(int appID, string[] ipList, int[] portList, string lastSuccessIP, int lastSuccessPort, string openID, bool isOnlyTACC)
        {
            if ((ipList == null) || (ipList.Length == 0))
            {
                this.mErrorCode = TdirResult.ParamError;
                this.mErrorString = "the input IP list is null or empty";
                this.TdirLog("the input IP list is null or empty");
                return TdirResult.ParamError;
            }
            if ((portList == null) || (portList.Length == 0))
            {
                this.mErrorCode = TdirResult.ParamError;
                this.mErrorString = "the input port list is null or empty";
                this.TdirLog("the input port list is null or empty");
                return TdirResult.ParamError;
            }
            this.mErrorCode = TdirResult.TdirNoError;
            this.mErrorString = "no error";
            this.isServiceTableEnable = false;
            this.m_appID = appID;
            this.m_ipList = ipList;
            this.m_portList = portList;
            this.m_lastSuccessIP = lastSuccessIP;
            this.m_lastSuccessPort = lastSuccessPort;
            this.m_openID = openID;
            this.m_treenodes = new List<TdirTreeNode>();
            this.m_treeRoleInfoDic.Clear();
            string str = string.Join("|", ipList);
            string str2 = string.Join("|", this.ToStringArray(portList));
            this.Dump();
            return tcls_init(base.ObjectId, appID, str, str2, lastSuccessIP, Convert.ToString(lastSuccessPort), openID, isOnlyTACC);
        }

        public TdirResult Recv(int timeout)
        {
            if (this.Status() == TdirResult.WaitForQuery)
            {
                return tcls_query(base.ObjectId, timeout);
            }
            if (this.Status() == TdirResult.WaitForRecv)
            {
                return tcls_recv(base.ObjectId, timeout);
            }
            return this.Status();
        }

        private void RecvNode(IntPtr ptr)
        {
            SubTdirTreeNode node = (SubTdirTreeNode) Marshal.PtrToStructure(ptr, typeof(SubTdirTreeNode));
            TdirTreeNode item = new TdirTreeNode {
                userRoleInfo = new List<TdirUserRoleInfo>(),
                nodeID = node.nodeID,
                parentID = node.parentID,
                flag = node.flag,
                name = node.name,
                status = node.status,
                nodeType = node.nodeType,
                svrFlag = node.svrFlag,
                staticInfo = node.staticInfo,
                dynamicInfo = node.dynamicInfo
            };
            if (this.m_treeRoleInfoDic.ContainsKey(item.nodeID))
            {
                item.userRoleInfo = this.m_treeRoleInfoDic[item.nodeID];
                string introduced2 = Convert.ToString(item.nodeID);
                this.TdirLog(string.Format("the size of node ID[{0}]'s user role info is [{1}]", introduced2, item.userRoleInfo.Count));
            }
            else
            {
                this.TdirLog(string.Format("node ID[{0}]'s user role info is empty", Convert.ToString(item.nodeID)));
            }
            this.m_treenodes.Add(item);
        }

        private void RecvRoleInfo(byte[] data)
        {
            CTdirUserRoleInfo info = new CTdirUserRoleInfo {
                appBuff = new byte[0x100]
            };
            info.Decode(data);
            TdirUserRoleInfo item = new TdirUserRoleInfo {
                zoneID = info.zoneID,
                roleID = info.roleID,
                lastLoginTime = info.lastLoginTime,
                roleName = info.roleName,
                roleLevel = info.roleLevel,
                appLen = info.appLen
            };
            if (0 < info.appLen)
            {
                item.appBuff = new byte[item.appLen];
                for (int i = 0; i < info.appLen; i++)
                {
                    item.appBuff[i] = info.appBuff[i];
                }
            }
            if (this.m_treeRoleInfoDic.ContainsKey(item.zoneID))
            {
                this.m_treeRoleInfoDic[item.zoneID].Add(item);
            }
            else
            {
                List<TdirUserRoleInfo> list = new List<TdirUserRoleInfo> {
                    item
                };
                this.m_treeRoleInfoDic.Add(item.zoneID, list);
            }
        }

        private void RecvServiceTable(byte[] data)
        {
            CTdirServiceTable table = new CTdirServiceTable {
                appBuff = new byte[0x200]
            };
            table.Decode(data);
            this.mServiceTable.updateTime = table.updateTime;
            this.mServiceTable.bitMap = table.bitMap;
            this.mServiceTable.userAttr = table.userAttr;
            this.mServiceTable.zoneID = table.zoneID;
            this.mServiceTable.appLen = table.appLen;
            if (0 < table.appLen)
            {
                this.mServiceTable.appBuff = new byte[table.appLen];
                for (int i = 0; i < table.appLen; i++)
                {
                    this.mServiceTable.appBuff[i] = table.appBuff[i];
                }
            }
            this.isServiceTableEnable = true;
        }

        private void RecvTreeCommomData(IntPtr ptr)
        {
            this.m_treeCommomData = (TreeCommonData) Marshal.PtrToStructure(ptr, typeof(TreeCommonData));
        }

        public TdirResult SetSvrTimeout(int timeout)
        {
            return tcls_set_svr_timeout(base.ObjectId, timeout);
        }

        public TdirResult Status()
        {
            return tcls_status(base.ObjectId);
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_disable_log(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_enable_log(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_get_errCode(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr tcls_get_errString(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_init(ulong objId, int appID, [MarshalAs(UnmanagedType.LPStr)] string ipList, [MarshalAs(UnmanagedType.LPStr)] string portList, [MarshalAs(UnmanagedType.LPStr)] string lastSuccessIP, [MarshalAs(UnmanagedType.LPStr)] string lastSuccessPort, [MarshalAs(UnmanagedType.LPStr)] string openID, bool isOnlyTACC);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_query(ulong objId, int timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_recv(ulong objId, int timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_set_svr_timeout(ulong objId, int timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern TdirResult tcls_status(ulong objId);
        public void TdirLog(string msg)
        {
            if (this.mLog)
            {
                Debug.Log(msg);
            }
        }

        private string[] ToStringArray(int[] portList)
        {
            string[] strArray = new string[portList.Length];
            for (int i = 0; i < portList.Length; i++)
            {
                strArray[i] = portList[i].ToString();
            }
            return strArray;
        }
    }
}

