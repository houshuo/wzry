namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    public class DatabinTableMulti<T, K> where T: class, tsf4g_csharp_interface, new()
    {
        private const int _headsize = 0x88;
        private bool bLoaded;
        private string keyName;
        private MultiValueHashDictionary<long, object> mapItems;
        private string name;

        public DatabinTableMulti(string name, string key)
        {
            this.mapItems = new MultiValueHashDictionary<long, object>();
            this.bLoaded = false;
            this.name = name;
            this.keyName = key;
            Singleton<ResourceLoader>.GetInstance().LoadDatabin(name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
        }

        public int Count()
        {
            this.Reload();
            return this.mapItems.Count;
        }

        public T GetDataByIdSingle(int id)
        {
            return DatabinTableMulti<T, K>.GetSingleValue(this.GetDataByIndex(id));
        }

        public HashSet<object> GetDataByIndex(int id)
        {
            this.Reload();
            if (this.bLoaded)
            {
                DictionaryView<long, HashSet<object>>.Enumerator enumerator = this.mapItems.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    if (i == id)
                    {
                        KeyValuePair<long, HashSet<object>> current = enumerator.Current;
                        return current.Value;
                    }
                }
            }
            return null;
        }

        public HashSet<object> GetDataByKey(int key)
        {
            this.Reload();
            if (this.bLoaded)
            {
                return this.mapItems.GetValues((long) key, true);
            }
            return null;
        }

        public HashSet<object> GetDataByKey(uint key)
        {
            this.Reload();
            if (this.bLoaded)
            {
                return this.mapItems.GetValues((long) key, true);
            }
            return null;
        }

        public T GetDataByKeySingle(uint key)
        {
            return DatabinTableMulti<T, K>.GetSingleValue(this.GetDataByKey(key));
        }

        private long GetDataKey(T data)
        {
            FieldInfo field = data.GetType().GetField(this.keyName);
            object[] inParameters = new object[] { this.keyName, this.name };
            DebugHelper.Assert(field != null, "Failed Get Databin key feild {0}, Databin:{1}", inParameters);
            object obj2 = field.GetValue(data);
            try
            {
                return Convert.ToInt64(obj2);
            }
            catch (Exception exception)
            {
                object[] objArray2 = new object[] { obj2, exception.Message };
                DebugHelper.Assert(false, "Exception in Databin Get Key, {0}, {1}", objArray2);
            }
            return 0L;
        }

        private static T GetSingleValue(HashSet<object> data)
        {
            if (data != null)
            {
                HashSet<object>.Enumerator enumerator = data.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            return null;
        }

        public bool isLoaded()
        {
            return this.bLoaded;
        }

        public void LoadTdrBin(byte[] rawData)
        {
            if (rawData.Length > 0x88)
            {
                TdrReadBuf srcBuf = new TdrReadBuf(ref rawData, rawData.Length);
                TResHeadAll all = new TResHeadAll();
                all.load(ref srcBuf);
                int iCount = all.mHead.iCount;
                for (int i = 0; i < iCount; i++)
                {
                    T data = Activator.CreateInstance<T>();
                    data.load(ref srcBuf, 0);
                    long dataKey = this.GetDataKey(data);
                    this.mapItems.Add(dataKey, data);
                }
            }
            else
            {
                Debug.Log("RecordTable<T>.LoadTdrBin:read record error! file length is zero. ");
            }
        }

        private void onRecordLoaded(ref byte[] rawData)
        {
            this.LoadTdrBin(rawData);
            this.bLoaded = true;
        }

        private void Reload()
        {
            if (!this.isLoaded())
            {
                Singleton<ResourceLoader>.GetInstance().LoadDatabin(this.Name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
            }
        }

        public void Unload()
        {
            this.mapItems.Clear();
            this.bLoaded = false;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public object[] RawDatas
        {
            get
            {
                this.Reload();
                return this.mapItems.GetAllValueArray();
            }
        }
    }
}

