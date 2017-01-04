namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class Pandora : MonoBehaviour
    {
        public static GameObject goPanelParent;
        public static GameObject goPdrParent;
        public static int iMidasPayFlag = 9;
        private static Pandora instance;
        public static bool isPannelOpen;
        public static bool NotDoUpdate;
        private static GameObject pandora_gm;
        public static int panelDepth = 100;
        public static bool stopConnectAll;
        private static string strCurRole = string.Empty;

        public static void Close()
        {
            com.tencent.pandora.Logger.d("call close");
            try
            {
                isPannelOpen = false;
                LuaHelper.GetPanelManager().CloseAllPanel();
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("CloseError:" + exception.Message);
            }
        }

        public static void Do(string strJson)
        {
            Dictionary<string, object> dictionary = null;
            try
            {
                dictionary = Json.Deserialize(strJson) as Dictionary<string, object>;
                if ((dictionary.ContainsKey("type") && dictionary.ContainsKey("content")) && dictionary["type"].Equals("inMainSence"))
                {
                    if (dictionary["content"].Equals("0"))
                    {
                        com.tencent.pandora.Logger.d("停止下载");
                        NetProxcy.PandoraLibDownloadControl(false);
                        setNoUpdateFlag(true);
                    }
                    else
                    {
                        com.tencent.pandora.Logger.d("恢复下载");
                        NetProxcy.PandoraLibDownloadControl(true);
                        setNoUpdateFlag(false);
                    }
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("协议无法解析:" + strJson + "," + exception.Message);
                NotificationCenter.DefaultCenter().PostNotification(null, "PraseCmdFail", strJson);
                return;
            }
            object[] args = new object[] { strJson };
            Util.CallMethod("commonlib", "DoCmdFromGame", args);
        }

        public static Pandora GetInstance()
        {
            if (object.ReferenceEquals(instance, null))
            {
                GameObject obj2 = new GameObject("Pandora");
                if (GetPandoraParent() != null)
                {
                    obj2.transform.parent = GetPandoraParent().transform;
                }
                instance = obj2.AddComponent<Pandora>();
            }
            return instance;
        }

        public static GameObject GetPandoraParent()
        {
            return goPdrParent;
        }

        public static GameObject GetPanelParent()
        {
            if (goPanelParent == null)
            {
                return goPdrParent;
            }
            return goPanelParent;
        }

        public static void Init()
        {
            FileUtils.InitPath();
            InitGameManager();
        }

        private static void InitGameManager()
        {
            if (pandora_gm == null)
            {
                pandora_gm = new GameObject("luaObj");
                if (GetPandoraParent() != null)
                {
                    pandora_gm.transform.parent = GetInstance().gameObject.transform;
                }
                else
                {
                    com.tencent.pandora.Logger.d("is error!");
                }
                AppFacade.Instance.AddManager("LuaScriptMgr", new LuaScriptMgr());
                AppFacade.Instance.AddManager<ResourceManager>("ResourceManager");
                AppFacade.Instance.AddManager<PanelManager>("PanelManager");
            }
        }

        public static void LogOutAccount()
        {
            try
            {
                Configer.strCtrFlagTotalSwitch = string.Empty;
                stopConnectAll = false;
                NetProxcy.CloseSocket();
                if (Pdr.isInitUlua)
                {
                    object[] args = new object[] { string.Empty };
                    Util.CallMethod("commonlib", "ChangeRole", args);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message);
            }
        }

        public void midaspayCallBack(string midasJson)
        {
            com.tencent.pandora.Logger.d("midasCallBack " + midasJson);
            NetProxcy.listInfoSO.Enqueue(new NetProxcy.infoFromSo(midasJson, midasJson.Length, iMidasPayFlag));
        }

        private void OnApplicationQuit()
        {
            stopConnectAll = true;
            try
            {
                NetProxcy.CloseSocket();
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d(exception.Message);
            }
            com.tencent.pandora.Logger.d("Pandora OnApplicationQuit");
            instance = null;
            pandora_gm = null;
        }

        private void OnDestroy()
        {
            LuaScriptMgr.Instance = null;
            stopConnectAll = true;
            com.tencent.pandora.Logger.d("Pandora OnDestroy");
        }

        public static void setDepth(int d)
        {
            panelDepth = d;
        }

        public static void setNoUpdateFlag(bool notUpdate)
        {
            com.tencent.pandora.Logger.d("setNoUpdateFlag: " + notUpdate.ToString());
            NotDoUpdate = notUpdate;
        }

        public void SetPanelDepth(int depth)
        {
            panelDepth = depth;
        }

        public static void SetPanelParent(GameObject goParent)
        {
            goPanelParent = goParent;
        }

        public static void setParentGameObject(GameObject parent)
        {
            stopConnectAll = false;
            goPdrParent = parent;
        }

        public void SetUserInfo(Dictionary<string, string> dicPara)
        {
            stopConnectAll = false;
            FileUtils.InitPath();
            string str = string.Empty;
            if (dicPara.ContainsKey("sRoleId"))
            {
                str = dicPara["sRoleId"].ToString();
            }
            if (str.Equals(string.Empty))
            {
                com.tencent.pandora.Logger.d("role is empty");
            }
            CUserData.SetPara(dicPara);
            if (Configer.strCtrFlagTotalSwitch.Equals(string.Empty))
            {
                com.tencent.pandora.Logger.d("InitSocket");
                NetProxcy.GetInstance().InitSocket();
            }
            else
            {
                object[] args = new object[] { string.Empty };
                Util.CallMethod("commonlib", "ReSetUserInfo", args);
            }
        }

        public void SetUserInfo(string strJson)
        {
            try
            {
                stopConnectAll = false;
                Debug.Log("Pandora SetPara");
                Dictionary<string, object> dictionary = Json.Deserialize(strJson) as Dictionary<string, object>;
                Dictionary<string, string> dicPara = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> pair in dictionary)
                {
                    dicPara.Add(pair.Key, pair.Value as string);
                }
                string str = string.Empty;
                if (dictionary.ContainsKey("sRoleId"))
                {
                    str = dictionary["sRoleId"].ToString();
                }
                if (str.Equals(string.Empty))
                {
                    com.tencent.pandora.Logger.d("role is empty");
                }
                CUserData.SetPara(dicPara);
                if (Configer.strCtrFlagTotalSwitch.Equals(string.Empty))
                {
                    com.tencent.pandora.Logger.d("InitSocket");
                    NetProxcy.GetInstance().InitSocket();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void Start()
        {
            FileUtils.InitPath();
        }

        private void Update()
        {
            if (!stopConnectAll && !NotDoUpdate)
            {
                if ((NetProxcy.listInfoSO.Count > 0) && Pdr.isInitUlua)
                {
                    com.tencent.pandora.Logger.d("begin to send jsonactdata to lua");
                    NetProxcy.infoFromSo so = NetProxcy.listInfoSO.Dequeue() as NetProxcy.infoFromSo;
                    object[] args = new object[] { so.jsonFromSO, so.jsonFromSOLength, so.iFlagSO };
                    Util.CallMethod("commonlib", "CsharpToLuaCallBack", args);
                }
                if (NetProxcy.GetCallBackData(new NetProxcy.CallBack(NetProxcy.DoCallBack)) == 0)
                {
                }
            }
        }
    }
}

