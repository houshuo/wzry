using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BeaconHelper : Singleton<BeaconHelper>
{
    public Beacon_BuyDianInfo m_curBuyDianInfo = new Beacon_BuyDianInfo();
    public Beacon_BuyPropInfo m_curBuyPropInfo = new Beacon_BuyPropInfo();
    private static float m_Time;

    public void Event_ApplicationPause(bool pause)
    {
        List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
            new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
            new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
            new KeyValuePair<string, string>("openid", "NULL"),
            new KeyValuePair<string, string>("GameType", Singleton<GameBuilder>.GetInstance().m_kGameType.ToString()),
            new KeyValuePair<string, string>("MapID", Singleton<GameBuilder>.GetInstance().m_iMapId.ToString()),
            new KeyValuePair<string, string>("Status", pause.ToString())
        };
        if (pause)
        {
            m_Time = Time.time;
            events.Add(new KeyValuePair<string, string>("PauseTime", string.Empty));
        }
        else
        {
            float num = Time.time - m_Time;
            events.Add(new KeyValuePair<string, string>("PauseTime", num.ToString()));
            m_Time = 0f;
        }
        string str = string.Empty;
        string str2 = string.Empty;
        if (Singleton<BattleLogic>.instance.isRuning && (Singleton<BattleLogic>.GetInstance().battleStat != null))
        {
            DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
            if (campStat != null)
            {
                if ((campStat.ContainsKey(1) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1))
                {
                    str = campStat[1].campScore.ToString();
                }
                if (campStat.ContainsKey(2))
                {
                    str2 = campStat[2].campScore.ToString();
                }
            }
        }
        events.Add(new KeyValuePair<string, string>("MyScore", str));
        events.Add(new KeyValuePair<string, string>("EnemyScore", str2));
        events.Add(new KeyValuePair<string, string>("RoomID", string.Empty));
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_ApplicationPause", events, true);
    }

    public void Event_CommonReport(string eventName)
    {
        List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("IS_IOS", "0"),
            new KeyValuePair<string, string>("LoginPlatForm", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
            new KeyValuePair<string, string>("worldid", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString())
        };
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent(eventName, events, true);
    }

    public void EventBase(ref List<KeyValuePair<string, string>> events)
    {
        events.Add(new KeyValuePair<string, string>("IS_IOS", "0"));
        events.Add(new KeyValuePair<string, string>("LoginPlatForm", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
        events.Add(new KeyValuePair<string, string>("worldid", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
    }

    public void EventPhotoReport(string status, float totalTime, string errorCode)
    {
        List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
            new KeyValuePair<string, string>("status", status),
            new KeyValuePair<string, string>("totaltime", totalTime.ToString()),
            new KeyValuePair<string, string>("errorCode", errorCode)
        };
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_GetPhoto", events, true);
    }

    public override void Init()
    {
        base.Init();
        this.m_curBuyDianInfo.clear();
        this.m_curBuyPropInfo.clear();
    }

    public void ReportBuyDianEvent()
    {
        try
        {
            if (!string.IsNullOrEmpty(this.m_curBuyDianInfo.buy_dia_channel))
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", Singleton<ApolloHelper>.GetInstance().GetOpenID()),
                    new KeyValuePair<string, string>("buy_dia_channel", this.m_curBuyDianInfo.buy_dia_channel),
                    new KeyValuePair<string, string>("buy_dia_id", this.m_curBuyDianInfo.buy_dia_id),
                    new KeyValuePair<string, string>("pay_type_result", this.m_curBuyDianInfo.pay_type_result),
                    new KeyValuePair<string, string>("callback_result", this.m_curBuyDianInfo.callback_result),
                    new KeyValuePair<string, string>("apollo_stage", this.m_curBuyDianInfo.apollo_stage),
                    new KeyValuePair<string, string>("apollo_result", this.m_curBuyDianInfo.apollo_result),
                    new KeyValuePair<string, string>("buy_quantity", this.m_curBuyDianInfo.buy_quantity),
                    new KeyValuePair<string, string>("callback_time", this.m_curBuyDianInfo.call_back_time.ToString())
                };
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Buydia", events, true);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
        this.m_curBuyDianInfo.clear();
    }

    public void ReportBuyPropEvent(string buy_prop_id_result)
    {
        try
        {
            if (!string.IsNullOrEmpty(this.m_curBuyPropInfo.buy_prop_channel))
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", "NULL"),
                    new KeyValuePair<string, string>("buy_prop_channel", this.m_curBuyPropInfo.buy_prop_channel),
                    new KeyValuePair<string, string>("buy_prop_id", this.m_curBuyPropInfo.buy_prop_id),
                    new KeyValuePair<string, string>("buy_quantity", this.m_curBuyPropInfo.buy_quantity),
                    new KeyValuePair<string, string>("buy_prop_way", this.m_curBuyPropInfo.buy_prop_way),
                    new KeyValuePair<string, string>("buy_prop_id_result", buy_prop_id_result)
                };
                float num = Time.time - this.m_curBuyPropInfo.buy_prop_id_time;
                events.Add(new KeyValuePair<string, string>("buy_prop_id_time", num.ToString()));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Buyprop", events, true);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
        this.m_curBuyPropInfo.clear();
    }

    public void ReportOpenBuyDianEvent(DateTime curTime)
    {
        try
        {
            if (!string.IsNullOrEmpty(this.m_curBuyDianInfo.buy_dia_channel))
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", "NULL"),
                    new KeyValuePair<string, string>("openpage_channel", this.m_curBuyDianInfo.buy_dia_channel),
                    new KeyValuePair<string, string>("buy_dia_id", this.m_curBuyDianInfo.buy_dia_id),
                    new KeyValuePair<string, string>("openpage_time", curTime.ToString())
                };
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Openpage", events, true);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Beacon_BuyDianInfo
    {
        public string buy_dia_channel;
        public string buy_dia_id;
        public string pay_type_result;
        public string callback_result;
        public string apollo_stage;
        public string apollo_result;
        public string buy_quantity;
        public float call_back_time;
        public void clear()
        {
            this.buy_dia_channel = string.Empty;
            this.buy_dia_id = string.Empty;
            this.pay_type_result = string.Empty;
            this.apollo_stage = string.Empty;
            this.callback_result = string.Empty;
            this.apollo_result = string.Empty;
            this.buy_quantity = string.Empty;
            this.call_back_time = 0f;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Beacon_BuyPropInfo
    {
        public string buy_prop_channel;
        public string buy_prop_id;
        public string buy_quantity;
        public string buy_prop_way;
        public string buy_prop_id_result;
        public float buy_prop_id_time;
        public void clear()
        {
            this.buy_prop_channel = string.Empty;
            this.buy_prop_id = string.Empty;
            this.buy_quantity = string.Empty;
            this.buy_prop_way = string.Empty;
            this.buy_prop_id_result = string.Empty;
            this.buy_prop_id_time = 0f;
        }
    }
}

