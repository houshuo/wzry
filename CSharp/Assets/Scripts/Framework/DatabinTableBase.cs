namespace Assets.Scripts.Framework
{
    using ResData;
    using System;
    using System.Collections.Generic;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    public class DatabinTableBase
    {
        protected const int _headsize = 0x88;
        protected bool bAllowUnLoad = true;
        protected bool bLoaded;
        protected string DataName;
        protected bool isDoubleKey;
        protected string KeyName;
        protected string KeyName1;
        protected string KeyName2;
        protected Dictionary<long, object> mapItems = new Dictionary<long, object>();
        protected System.Type ValueType;

        public DatabinTableBase(System.Type InValueType)
        {
            this.ValueType = InValueType;
        }

        public int Count()
        {
            this.Reload();
            return this.mapItems.Count;
        }

        protected object GetDataByKeyInner(long key)
        {
            object obj2;
            if (this.bLoaded && this.mapItems.TryGetValue(key, out obj2))
            {
                return obj2;
            }
            return null;
        }

        protected long GetDataKey(object data, System.Type InValueType)
        {
            System.Type type = data.GetType();
            object[] inParameters = new object[] { this.Name };
            DebugHelper.Assert(type == InValueType, "Invalid Config for Databin:{0}", inParameters);
            if (this.isDoubleKey)
            {
                object obj2 = type.GetField(this.KeyName1).GetValue(data);
                object[] objArray2 = new object[] { this.KeyName1, this.Name };
                DebugHelper.Assert(obj2 != null, "Can't Find Key {0} in DataBin:{1}", objArray2);
                object obj3 = type.GetField(this.KeyName2).GetValue(data);
                object[] objArray3 = new object[] { this.KeyName2, this.Name };
                DebugHelper.Assert(obj3 != null, "Can't Find Key {0} in DataBin:{1}", objArray3);
                try
                {
                    if ((obj2 != null) && (obj3 != null))
                    {
                        ulong num = Convert.ToUInt64(obj2) << 0x20;
                        int num2 = Convert.ToInt32(obj3);
                        return (((long) num) + num2);
                    }
                    return 0L;
                }
                catch (Exception exception)
                {
                    object[] objArray4 = new object[] { obj2, obj3, exception.Message };
                    DebugHelper.Assert(false, "Exception in Databin Get Key1, {0}, Key2{1},{2}", objArray4);
                    return 0L;
                }
            }
            object obj4 = type.GetField(this.KeyName).GetValue(data);
            object[] objArray5 = new object[] { this.KeyName, this.Name };
            DebugHelper.Assert(obj4 != null, "Can't Find Key {0} in DataBin:{1}", objArray5);
            try
            {
                return ((obj4 == null) ? 0L : Convert.ToInt64(obj4));
            }
            catch (Exception exception2)
            {
                object[] objArray6 = new object[] { obj4, exception2.Message };
                DebugHelper.Assert(false, "Exception in Databin Get Key, {0}, {1}", objArray6);
                return 0L;
            }
        }

        public Dictionary<long, object>.Enumerator GetEnumerator()
        {
            return this.mapItems.GetEnumerator();
        }

        public void LoadTdrBin(byte[] rawData, System.Type InValueType)
        {
            if (rawData.Length > 0x88)
            {
                TdrReadBuf srcBuf = new TdrReadBuf(ref rawData, rawData.Length);
                TResHeadAll all = new TResHeadAll();
                all.load(ref srcBuf);
                int iCount = all.mHead.iCount;
                DebugHelper.Assert(iCount < 0x186a0, "有这么恐怖吗，超过10w条配置数据。。。。");
                for (int i = 0; i < iCount; i++)
                {
                    tsf4g_csharp_interface data = Activator.CreateInstance(InValueType) as tsf4g_csharp_interface;
                    object[] inParameters = new object[] { InValueType.Name };
                    DebugHelper.Assert(data != null, "Failed Create Object, Type:{0}", inParameters);
                    data.load(ref srcBuf, 0);
                    long dataKey = this.GetDataKey(data, InValueType);
                    try
                    {
                        this.mapItems.Add(dataKey, data);
                    }
                    catch (ArgumentException exception)
                    {
                        DebugHelper.Assert(false, exception.Message);
                        object[] objArray2 = new object[] { dataKey, this.Name, InValueType.Name };
                        DebugHelper.Assert(false, "RecordTable<{2}>.LoadTdrBin: Key Repeat: {0}, DataBinName:{1}", objArray2);
                    }
                }
            }
            else
            {
                Debug.Log("RecordTable<T>.LoadTdrBin:read record error! file length is zero. ");
            }
        }

        protected void onRecordLoaded(ref byte[] rawData)
        {
            this.LoadTdrBin(rawData, this.ValueType);
            this.bLoaded = true;
        }

        protected void Reload()
        {
            if (!this.isLoaded)
            {
                Singleton<ResourceLoader>.GetInstance().LoadDatabin(this.Name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
            }
        }

        public void Unload()
        {
            if (this.bAllowUnLoad)
            {
                this.bLoaded = false;
                this.mapItems.Clear();
            }
        }

        public int count
        {
            get
            {
                return this.Count();
            }
        }

        public bool isAllowUnLoad
        {
            set
            {
                this.bAllowUnLoad = value;
            }
        }

        public bool isLoaded
        {
            get
            {
                return this.bLoaded;
            }
        }

        public string Name
        {
            get
            {
                return this.DataName;
            }
        }
    }
}

