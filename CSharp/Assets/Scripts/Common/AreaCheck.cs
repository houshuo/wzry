namespace Assets.Scripts.Common
{
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class AreaCheck
    {
        private ActorFilterDelegate _actorAreaFunc;
        private ActorProcess _actorProcess;
        private DictionaryView<uint, AroundRecord> _aroundRecords;
        private uint _checkFreq = 5;
        private List<PoolObjHandle<ActorRoot>> _waitCheckRef;

        public AreaCheck(ActorFilterDelegate actorAreaFunc, ActorProcess actorProcess, List<PoolObjHandle<ActorRoot>> checkList)
        {
            this._actorAreaFunc = actorAreaFunc;
            this._actorProcess = actorProcess;
            this._waitCheckRef = checkList;
            this._aroundRecords = new DictionaryView<uint, AroundRecord>();
        }

        public int CountActors(ActorFilterDelegate actorFilter)
        {
            int num = 0;
            DictionaryView<uint, AroundRecord>.Enumerator enumerator = this._aroundRecords.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (actorFilter != null)
                {
                    KeyValuePair<uint, AroundRecord> current = enumerator.Current;
                    if (!actorFilter(ref current.Value.actor))
                    {
                        continue;
                    }
                }
                num++;
            }
            return num;
        }

        public bool HasActorIn(PoolObjHandle<ActorRoot> actor)
        {
            return this._aroundRecords.ContainsKey(actor.handle.ObjID);
        }

        public void UpdateLogic(uint checkPos)
        {
            uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
            if ((curFrameNum % this._checkFreq) == (checkPos % this._checkFreq))
            {
                int count = this._waitCheckRef.Count;
                for (int i = 0; i < count; i++)
                {
                    PoolObjHandle<ActorRoot> actor = this._waitCheckRef[i];
                    if ((this._actorAreaFunc == null) || this._actorAreaFunc(ref actor))
                    {
                        if (this._aroundRecords.ContainsKey(actor.handle.ObjID))
                        {
                            AroundRecord record = this._aroundRecords[actor.handle.ObjID];
                            record.frame = curFrameNum;
                            this._actorProcess(actor, ActorAction.Hover);
                        }
                        else
                        {
                            this._aroundRecords.Add(actor.handle.ObjID, new AroundRecord(actor, (ulong) curFrameNum));
                            this._actorProcess(actor, ActorAction.Enter);
                        }
                    }
                }
                if (this._aroundRecords.Count > 0)
                {
                    uint key = 0;
                    AroundRecord record2 = null;
                    ulong frame = curFrameNum;
                    DictionaryView<uint, AroundRecord>.Enumerator enumerator = this._aroundRecords.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, AroundRecord> current = enumerator.Current;
                        AroundRecord record3 = current.Value;
                        if (record3.frame < frame)
                        {
                            frame = record3.frame;
                            KeyValuePair<uint, AroundRecord> pair2 = enumerator.Current;
                            key = pair2.Key;
                            record2 = record3;
                        }
                    }
                    if (record2 != null)
                    {
                        this._aroundRecords.Remove(key);
                        this._actorProcess(record2.actor, ActorAction.Leave);
                    }
                }
            }
        }

        public enum ActorAction
        {
            Enter,
            Hover,
            Leave
        }

        public delegate void ActorProcess(PoolObjHandle<ActorRoot> actor, AreaCheck.ActorAction action);

        private class AroundRecord
        {
            public PoolObjHandle<ActorRoot> actor;
            public ulong frame;

            public AroundRecord(PoolObjHandle<ActorRoot> _actor, ulong _frame)
            {
                this.actor = _actor;
                this.frame = _frame;
            }
        }
    }
}

