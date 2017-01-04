namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BufferMarkRule
    {
        private BuffHolderComponent buffHolder;
        private DictionaryView<int, BufferMark> buffMarkSet = new DictionaryView<int, BufferMark>();

        public void AddBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID, uint _markType)
        {
            if (this.CheckDependMark(_markID))
            {
                BufferMark mark;
                if (this.buffMarkSet.TryGetValue(_markID, out mark))
                {
                    mark.AutoTrigger(_originator);
                }
                else
                {
                    mark = new BufferMark(_markID);
                    if (mark.cfgData != null)
                    {
                        mark.Init(this.buffHolder, _originator, _markType);
                        this.buffMarkSet.Add(_markID, mark);
                    }
                }
            }
        }

        public bool CheckDependMark(int _markID)
        {
            BufferMark mark;
            ResSkillMarkCfgInfo dataByKey = GameDataMgr.skillMarkDatabin.GetDataByKey((long) _markID);
            if (dataByKey == null)
            {
                return false;
            }
            return ((dataByKey.iDependCfgID == 0) || ((this.buffMarkSet.TryGetValue(dataByKey.iDependCfgID, out mark) && (mark != null)) && (mark.GetCurLayer() > 0)));
        }

        public void Clear()
        {
            DictionaryView<int, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, BufferMark> current = enumerator.Current;
                current.Value.SetCurLayer(0);
            }
            this.buffMarkSet.Clear();
        }

        public void ClearBufferMark(int _typeMask)
        {
            DictionaryView<int, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, BufferMark> current = enumerator.Current;
                BufferMark mark = current.Value;
                int markType = (int) mark.GetMarkType();
                if ((_typeMask & (((int) 1) << markType)) > 0)
                {
                    mark.SetCurLayer(0);
                }
            }
        }

        public void Init(BuffHolderComponent _buffHolder)
        {
            this.buffHolder = _buffHolder;
            this.Clear();
        }

        public void RemoveBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
        {
            BufferMark mark;
            if (this.buffMarkSet.TryGetValue(_markID, out mark))
            {
                mark.DecLayer(1);
            }
        }

        public void TriggerBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
        {
            BufferMark mark;
            if (this.buffMarkSet.TryGetValue(_markID, out mark))
            {
                mark.UpperTrigger(_originator);
            }
        }

        public void UpdateLogic(int nDelta)
        {
            DictionaryView<int, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, BufferMark> current = enumerator.Current;
                current.Value.UpdateLogic(nDelta);
            }
        }
    }
}

