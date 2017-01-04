using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

public class NewbieGuideDataManager : Singleton<NewbieGuideDataManager>
{
    private ListView<NewbieGuideMainLineConf> mCacheMainLineSourceList;
    private ListView<NewbieGuideMainLineConf> mCacheMainLineTargetList;
    private ListView<NewbieWeakGuideMainLineConf> mCacheWeakSourceList;
    private ListView<NewbieWeakGuideMainLineConf> mCacheWeakTargetList;
    private NewbieGuideMainLineConf[] mMainLineCacheArr;
    private DictionaryView<uint, ListView<NewbieGuideScriptConf>> mScriptCacheDic;
    private NewbieGuideSpecialTipConf[] mSpecialTipCacheArr;
    private NewbieWeakGuideMainLineConf[] mWeakMainLineCacheArr;
    private DictionaryView<uint, ListView<NewbieGuideWeakConf>> mWeakScriptCacheDic;

    public NewbieGuideDataManager()
    {
        int length = 0;
        this.mMainLineCacheArr = new NewbieGuideMainLineConf[GameDataMgr.newbieMainLineDatabin.Count()];
        GameDataMgr.newbieMainLineDatabin.CopyTo(ref this.mMainLineCacheArr);
        this.SortMainLineList(this.mMainLineCacheArr);
        this.mScriptCacheDic = new DictionaryView<uint, ListView<NewbieGuideScriptConf>>();
        NewbieGuideScriptConf[] array = new NewbieGuideScriptConf[GameDataMgr.newbieScriptDatabin.RawDatas.Length];
        GameDataMgr.newbieScriptDatabin.RawDatas.CopyTo(array, 0);
        length = array.Length;
        for (int i = 0; i < length; i++)
        {
            ListView<NewbieGuideScriptConf> view;
            NewbieGuideScriptConf item = array[i];
            if (!this.mScriptCacheDic.TryGetValue(item.wMainLineID, out view))
            {
                view = new ListView<NewbieGuideScriptConf>();
                this.mScriptCacheDic.Add(item.wMainLineID, view);
            }
            view.Add(item);
        }
        this.mSpecialTipCacheArr = new NewbieGuideSpecialTipConf[GameDataMgr.newbieSpecialTipDatabin.count];
        GameDataMgr.newbieSpecialTipDatabin.CopyTo(ref this.mSpecialTipCacheArr);
        this.mWeakMainLineCacheArr = new NewbieWeakGuideMainLineConf[GameDataMgr.newbieWeakMainLineDataBin.count];
        GameDataMgr.newbieWeakMainLineDataBin.CopyTo(ref this.mWeakMainLineCacheArr);
        this.SortWeakMainLineList(this.mWeakMainLineCacheArr);
        NewbieGuideWeakConf[] inArrayRef = new NewbieGuideWeakConf[GameDataMgr.newbieWeakDatabin.count];
        GameDataMgr.newbieWeakDatabin.CopyTo(ref inArrayRef);
        this.mWeakScriptCacheDic = new DictionaryView<uint, ListView<NewbieGuideWeakConf>>();
        length = inArrayRef.Length;
        for (int j = 0; j < length; j++)
        {
            ListView<NewbieGuideWeakConf> view2;
            NewbieGuideWeakConf conf2 = inArrayRef[j];
            if (!this.mWeakScriptCacheDic.TryGetValue(conf2.dwWeakLineID, out view2))
            {
                view2 = new ListView<NewbieGuideWeakConf>();
                this.mWeakScriptCacheDic.Add(conf2.dwWeakLineID, view2);
            }
            view2.Add(conf2);
        }
        this.mCacheMainLineSourceList = new ListView<NewbieGuideMainLineConf>();
        this.mCacheMainLineTargetList = new ListView<NewbieGuideMainLineConf>();
        this.mCacheWeakSourceList = new ListView<NewbieWeakGuideMainLineConf>();
        this.mCacheWeakTargetList = new ListView<NewbieWeakGuideMainLineConf>();
    }

    public List<uint> GetMainLineIDList()
    {
        List<uint> list = new List<uint>();
        int length = this.mMainLineCacheArr.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideMainLineConf conf = this.mMainLineCacheArr[i];
            list.Add(conf.dwID);
        }
        return list;
    }

    public NewbieGuideMainLineConf GetNewbieGuideMainLineConf(uint id)
    {
        int length = this.mMainLineCacheArr.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideMainLineConf conf = this.mMainLineCacheArr[i];
            if (conf.dwID == id)
            {
                return conf;
            }
        }
        return null;
    }

    public ListView<NewbieGuideMainLineConf> GetNewbieGuideMainLineConfListBySkipType(NewbieGuideSkipConditionType type)
    {
        this.mCacheMainLineSourceList.Clear();
        this.mCacheMainLineSourceList.AddRange(this.mMainLineCacheArr);
        ListView<NewbieGuideMainLineConf> view = new ListView<NewbieGuideMainLineConf>();
        int count = this.mCacheMainLineSourceList.Count;
        for (int i = 0; i < count; i++)
        {
            NewbieGuideMainLineConf item = this.mCacheMainLineSourceList[i];
            for (int j = 0; j < item.astSkipCondition.Length; j++)
            {
                if (((NewbieGuideSkipConditionType) item.astSkipCondition[j].wType) == type)
                {
                    view.Add(item);
                    break;
                }
            }
        }
        return view;
    }

    public ListView<NewbieGuideMainLineConf> GetNewbieGuideMainLineConfListByTriggerTimeType(NewbieGuideTriggerTimeType type, uint[] param)
    {
        this.mCacheMainLineSourceList.Clear();
        this.mCacheMainLineSourceList.AddRange(this.mMainLineCacheArr);
        this.mCacheMainLineTargetList.Clear();
        int count = this.mCacheMainLineSourceList.Count;
        for (int i = 0; i < count; i++)
        {
            NewbieGuideMainLineConf data = this.mCacheMainLineSourceList[i];
            if (this.IsContainsTriggerTimeType(data, type, param))
            {
                this.mCacheMainLineTargetList.Add(data);
            }
        }
        return this.mCacheMainLineTargetList;
    }

    public ListView<NewbieWeakGuideMainLineConf> GetNewBieGuideWeakMainLineConfListByTiggerTimeType(NewbieGuideTriggerTimeType type, uint[] param)
    {
        this.mCacheWeakSourceList.Clear();
        this.mCacheWeakSourceList.AddRange(this.mWeakMainLineCacheArr);
        this.mCacheWeakTargetList.Clear();
        int count = this.mCacheWeakSourceList.Count;
        for (int i = 0; i < count; i++)
        {
            NewbieWeakGuideMainLineConf conf = this.mCacheWeakSourceList[i];
            if (this.IsContainsTriggerTimeType(conf, type, param))
            {
                this.mCacheWeakTargetList.Add(conf);
            }
        }
        return this.mCacheWeakTargetList;
    }

    public ListView<NewbieWeakGuideMainLineConf> GetNewbieGuideWeakMianLineConfListBySkipType(NewbieGuideSkipConditionType type)
    {
        this.mCacheWeakSourceList.Clear();
        this.mCacheWeakSourceList.AddRange(this.mWeakMainLineCacheArr);
        ListView<NewbieWeakGuideMainLineConf> view = new ListView<NewbieWeakGuideMainLineConf>();
        int count = this.mCacheWeakSourceList.Count;
        for (int i = 0; i < count; i++)
        {
            NewbieWeakGuideMainLineConf item = this.mCacheWeakSourceList[i];
            for (int j = 0; j < item.astSkipCondition.Length; j++)
            {
                if (((NewbieGuideSkipConditionType) item.astSkipCondition[j].wType) == type)
                {
                    view.Add(item);
                    break;
                }
            }
        }
        return view;
    }

    public NewbieWeakGuideMainLineConf GetNewbieWeakGuideMainLineConf(uint id)
    {
        int length = this.mWeakMainLineCacheArr.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieWeakGuideMainLineConf conf = this.mWeakMainLineCacheArr[i];
            if (conf.dwID == id)
            {
                return conf;
            }
        }
        return null;
    }

    public ListView<NewbieGuideScriptConf> GetScriptList(uint mainLineID)
    {
        ListView<NewbieGuideScriptConf> view;
        if (this.mScriptCacheDic.TryGetValue(mainLineID, out view))
        {
            return view;
        }
        return null;
    }

    public NewbieGuideSpecialTipConf GetSpecialTipConfig(uint id)
    {
        int length = this.mSpecialTipCacheArr.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideSpecialTipConf conf = this.mSpecialTipCacheArr[i];
            if (conf.dwID == id)
            {
                return conf;
            }
        }
        return null;
    }

    public List<uint> GetWeakMianLineIDList()
    {
        List<uint> list = new List<uint>();
        int length = this.mWeakMainLineCacheArr.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieWeakGuideMainLineConf conf = this.mWeakMainLineCacheArr[i];
            list.Add(conf.dwID);
        }
        return list;
    }

    public ListView<NewbieGuideWeakConf> GetWeakScriptList(uint mainLineID)
    {
        ListView<NewbieGuideWeakConf> view = new ListView<NewbieGuideWeakConf>();
        if (this.mWeakScriptCacheDic.TryGetValue(mainLineID, out view))
        {
            return view;
        }
        return null;
    }

    public bool IsContainsSkipConditionType(NewbieWeakGuideMainLineConf conf, NewbieGuideSkipConditionType type, uint[] param)
    {
        int length = conf.astSkipCondition.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideSkipConditionItem item = conf.astSkipCondition[i];
            if ((((NewbieGuideSkipConditionType) item.wType) == type) && NewbieGuideCheckSkipConditionUtil.CheckSkipCondition(item, param))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsContainsTriggerTimeType(NewbieGuideMainLineConf data, NewbieGuideTriggerTimeType type, uint[] param)
    {
        int length = data.astTriggerTime.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideTriggerTimeItem time = data.astTriggerTime[i];
            if ((type == ((NewbieGuideTriggerTimeType) time.wType)) && NewbieGuideCheckTriggerTimeUtil.CheckTriggerTime(time, param))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsContainsTriggerTimeType(NewbieWeakGuideMainLineConf conf, NewbieGuideTriggerTimeType type, uint[] param)
    {
        int length = conf.astTriggerTime.Length;
        for (int i = 0; i < length; i++)
        {
            NewbieGuideTriggerTimeItem time = conf.astTriggerTime[i];
            if ((type == ((NewbieGuideTriggerTimeType) time.wType)) && NewbieGuideCheckTriggerTimeUtil.CheckTriggerTime(time, param))
            {
                return true;
            }
        }
        return false;
    }

    private bool SortMainLineConf(NewbieGuideMainLineConf confA, NewbieGuideMainLineConf confB)
    {
        if (confA.dwPriority > confB.dwPriority)
        {
            return false;
        }
        if ((confA.dwPriority >= confB.dwPriority) && (confA.dwID < confB.dwID))
        {
            return false;
        }
        return true;
    }

    private void SortMainLineList(NewbieGuideMainLineConf[] list)
    {
        for (int i = 1; i < list.Length; i++)
        {
            NewbieGuideMainLineConf confB = list[i];
            int index = i;
            while ((index > 0) && this.SortMainLineConf(list[index - 1], confB))
            {
                list[index] = list[index - 1];
                index--;
            }
            list[index] = confB;
        }
    }

    private void SortWeakMainLineList(NewbieWeakGuideMainLineConf[] list)
    {
        for (int i = 1; i < list.Length; i++)
        {
            NewbieWeakGuideMainLineConf confB = list[i];
            int index = i;
            while ((index > 0) && this.SortWeakMianLineConf(list[index - 1], confB))
            {
                list[index] = list[index - 1];
                index--;
            }
            list[index] = confB;
        }
    }

    private bool SortWeakMianLineConf(NewbieWeakGuideMainLineConf confA, NewbieWeakGuideMainLineConf confB)
    {
        if (confA.dwPriority > confB.dwPriority)
        {
            return false;
        }
        if ((confA.dwPriority >= confB.dwPriority) && (confA.dwID < confB.dwID))
        {
            return false;
        }
        return true;
    }
}

