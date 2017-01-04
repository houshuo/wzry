namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class RefParamOperator
    {
        public AGE.Action owner;
        public DictionaryView<string, ListView<RefData>> refDataList;
        public DictionaryView<string, SRefParam> refParamList = new DictionaryView<string, SRefParam>();

        public RefData AddRefData(string name, FieldInfo field, object data)
        {
            ListView<RefData> view;
            if (this.refDataList == null)
            {
                this.refDataList = new DictionaryView<string, ListView<RefData>>();
            }
            if (!this.refDataList.TryGetValue(name, out view))
            {
                view = new ListView<RefData>();
                this.refDataList.Add(name, view);
            }
            RefData item = new RefData(field, data);
            view.Add(item);
            return item;
        }

        public void AddRefParam(string name, bool value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.Bool;
                param.union._bool = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, int value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.Int;
                param.union._int = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, object value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.obj = value;
                param.type = SRefParam.ValType.Object;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, uint value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.UInt;
                param.union._uint = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, Vector3 value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.Vector3;
                param.union._vec3 = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, PoolObjHandle<ActorRoot> value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.handle = value;
                param.type = SRefParam.ValType.ActorRoot;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, float value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.Float;
                param.union._float = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, Quaternion value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.Quaternion;
                param.union._quat = value;
                this.refParamList.Add(name, param);
            }
        }

        public void AddRefParam(string name, VInt3 value)
        {
            if (!this.refParamList.ContainsKey(name))
            {
                SRefParam param = SObjPool<SRefParam>.New();
                param.type = SRefParam.ValType.VInt3;
                param.union._vint3 = value;
                this.refParamList.Add(name, param);
            }
        }

        public void ClearParams()
        {
            DictionaryView<string, SRefParam>.Enumerator enumerator = this.refParamList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, SRefParam> current = enumerator.Current;
                current.Value.Destroy();
            }
            this.refParamList.Clear();
        }

        public bool GetRefParam(string name, ref bool value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Bool))
            {
                value = param.union._bool;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref int value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Int))
            {
                value = param.union._int;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref object value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Object))
            {
                value = param.obj;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref float value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Float))
            {
                value = param.union._float;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref uint value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.UInt))
            {
                value = param.union._uint;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref Quaternion value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Quaternion))
            {
                value = param.union._quat;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref Vector3 value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.Vector3))
            {
                value = param.union._vec3;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref PoolObjHandle<ActorRoot> value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.ActorRoot))
            {
                value = param.handle;
                return true;
            }
            return false;
        }

        public bool GetRefParam(string name, ref VInt3 value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param) && (param.type == SRefParam.ValType.VInt3))
            {
                value = param.union._vint3;
                return true;
            }
            return false;
        }

        public T GetRefParamObject<T>(string name) where T: class
        {
            object obj2 = null;
            if (this.GetRefParam(name, ref obj2))
            {
                return (obj2 as T);
            }
            return null;
        }

        public ListView<RefData> GetSrcRefDataList(string name)
        {
            ListView<RefData> view = null;
            if (((this.owner != null) && (this.owner.refParamsSrc != null)) && (this.owner.refParamsSrc.refDataList != null))
            {
                this.owner.refParamsSrc.refDataList.TryGetValue(name, out view);
            }
            return view;
        }

        public void Reset()
        {
            this.owner = null;
            this.ClearParams();
        }

        public void SetOrAddRefParam(string name, SRefParam param)
        {
            SRefParam param2 = null;
            if (this.refParamList.TryGetValue(name, out param2))
            {
                if (param2.type == param.type)
                {
                    if (param2.type < SRefParam.ValType.Object)
                    {
                        param2.union._quat = param.union._quat;
                    }
                    else
                    {
                        param2.obj = param.obj;
                        if (param2.type == SRefParam.ValType.ActorRoot)
                        {
                            param2.union._uint = param.union._uint;
                        }
                    }
                }
            }
            else
            {
                this.refParamList.Add(name, param.Clone());
            }
        }

        public void SetRefData(object value, ListView<RefData> srcRefDataList)
        {
            for (int i = 0; i < srcRefDataList.Count; i++)
            {
                RefData data = srcRefDataList[i];
                if (data.eventIdx > -1)
                {
                    BaseEvent event2 = this.owner.GetTrack(data.trackIndex).trackEvents[data.eventIdx];
                    data.fieldInfo.SetValue(event2, value);
                }
                else
                {
                    Track track = this.owner.GetTrack(data.trackIndex);
                    data.fieldInfo.SetValue(track, value);
                }
            }
        }

        public void SetRefParam(string name, bool value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._bool = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, int value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._int = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, object value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.obj = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, float value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._float = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, uint value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._uint = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, Quaternion value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._quat = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, Vector3 value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._vec3 = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, PoolObjHandle<ActorRoot> value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.handle = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }

        public void SetRefParam(string name, VInt3 value)
        {
            SRefParam param = null;
            if (this.refParamList.TryGetValue(name, out param))
            {
                param.union._vint3 = value;
                ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
                if (srcRefDataList != null)
                {
                    this.SetRefData(value, srcRefDataList);
                }
            }
        }
    }
}

