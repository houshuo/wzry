namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Track : PooledClassObject
    {
        public AGE.Action action;
        private List<DurationEvent> activeEvents;
        public Color color;
        public int curTime;
        public bool enabled;
        private System.Type eventType;
        public bool execOnActionCompleted;
        public bool execOnForceStopped;
        private bool isCondition;
        private bool isDurationEvent;
        private int preExcuteTime;
        public uint startCount;
        public bool started;
        private bool supportEditMode;
        public List<BaseEvent> trackEvents;
        public int trackIndex;
        public string trackName;
        public Dictionary<int, bool> waitForConditions;

        public Track()
        {
            this.trackEvents = new List<BaseEvent>();
            this.activeEvents = new List<DurationEvent>();
            this.trackIndex = -1;
            this.color = Color.red;
            this.trackName = string.Empty;
            base.bChkReset = false;
        }

        public Track(AGE.Action _action, System.Type _eventType)
        {
            this.trackEvents = new List<BaseEvent>();
            this.activeEvents = new List<DurationEvent>();
            this.trackIndex = -1;
            this.color = Color.red;
            this.trackName = string.Empty;
            this.CopyData(_action, _eventType);
        }

        public BaseEvent AddEvent(int _time, int _length)
        {
            BaseEvent item = (BaseEvent) Activator.CreateInstance(this.eventType);
            item.time = _time;
            if (this.isDurationEvent)
            {
                (item as DurationEvent).length = _length;
            }
            int count = 0;
            if (this.LocateInsertPos(_time, out count))
            {
                if (count > this.trackEvents.Count)
                {
                    count = this.trackEvents.Count;
                }
                this.trackEvents.Insert(count, item);
            }
            else
            {
                this.trackEvents.Add(item);
            }
            item.track = this;
            return item;
        }

        public bool CheckConditions(AGE.Action _action)
        {
            if (this.waitForConditions != null)
            {
                Dictionary<int, bool>.Enumerator enumerator = this.waitForConditions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, bool> current = enumerator.Current;
                    int key = current.Key;
                    if ((key >= 0) && (key < _action.GetConditionCount()))
                    {
                        KeyValuePair<int, bool> pair2 = enumerator.Current;
                        if (_action.GetCondition(_action.GetTrack(key)) != pair2.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected bool CheckSkip(int _curTime, int _checkTime)
        {
            if (!this.Loop)
            {
                return ((_checkTime < _curTime) && (_checkTime >= this.preExcuteTime));
            }
            int num = _curTime;
            int preExcuteTime = this.preExcuteTime;
            int length = this.action.length;
            if (_checkTime < 0)
            {
                _checkTime += length;
            }
            else if (_checkTime >= length)
            {
                _checkTime -= length;
            }
            if (preExcuteTime >= 0)
            {
                return ((_checkTime < num) && (_checkTime >= preExcuteTime));
            }
            if (((_checkTime >= num) || (_checkTime < 0)) && ((_checkTime > length) || (_checkTime < (preExcuteTime + length))))
            {
                return false;
            }
            return true;
        }

        public Track Clone()
        {
            Track track = ClassObjPool<Track>.Get();
            track.CopyData(this);
            int count = this.trackEvents.Count;
            for (int i = 0; i < count; i++)
            {
                BaseEvent item = this.trackEvents[i].Clone();
                item.track = track;
                track.trackEvents.Add(item);
            }
            track.waitForConditions = this.waitForConditions;
            track.enabled = this.enabled;
            track.color = this.color;
            track.trackName = this.trackName;
            track.execOnActionCompleted = this.execOnActionCompleted;
            track.execOnForceStopped = this.execOnForceStopped;
            return track;
        }

        public void CopyData(Track src)
        {
            this.action = src.action;
            this.eventType = src.eventType;
            this.isDurationEvent = src.isDurationEvent;
            this.isCondition = src.isCondition;
            this.supportEditMode = src.supportEditMode;
            this.curTime = 0;
            this.preExcuteTime = 0;
        }

        public void CopyData(AGE.Action _action, System.Type _eventType)
        {
            this.action = _action;
            this.eventType = _eventType;
            if (this.eventType.IsSubclassOf(typeof(DurationEvent)))
            {
                this.isDurationEvent = true;
            }
            if (this.eventType.IsSubclassOf(typeof(TickCondition)) || this.eventType.IsSubclassOf(typeof(DurationCondition)))
            {
                this.isCondition = true;
            }
            this.supportEditMode = ((BaseEvent) Activator.CreateInstance(this.eventType)).SupportEditMode();
            this.curTime = 0;
            this.preExcuteTime = 0;
        }

        public void DoLoop()
        {
        }

        public Dictionary<string, bool> GetAssociatedResources()
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            foreach (BaseEvent event2 in this.trackEvents)
            {
                Dictionary<string, bool> associatedResources = event2.GetAssociatedResources();
                if (associatedResources != null)
                {
                    foreach (string str in associatedResources.Keys)
                    {
                        if (dictionary.ContainsKey(str))
                        {
                            Dictionary<string, bool> dictionary3;
                            string str2;
                            bool flag = dictionary3[str2];
                            (dictionary3 = dictionary)[str2 = str] = flag | associatedResources[str];
                        }
                        else
                        {
                            dictionary.Add(str, associatedResources[str]);
                        }
                    }
                }
            }
            return dictionary;
        }

        public void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID)
        {
            for (int i = 0; i < this.trackEvents.Count; i++)
            {
                BaseEvent event2 = this.trackEvents[i];
                if (event2 != null)
                {
                    event2.GetAssociatedResources(results, markID);
                }
            }
        }

        public BaseEvent GetEvent(int index)
        {
            if ((index >= 0) && (index < this.trackEvents.Count))
            {
                return this.trackEvents[index];
            }
            return null;
        }

        public int GetEventEndTime()
        {
            if (this.trackEvents.Count == 0)
            {
                return 0;
            }
            if (this.isDurationEvent)
            {
                return ((this.trackEvents[this.trackEvents.Count - 1] as DurationEvent).End + 0x21);
            }
            return ((this.trackEvents[this.trackEvents.Count - 1] as TickEvent).time + 0x21);
        }

        public int GetEventsCount()
        {
            return this.trackEvents.Count;
        }

        public int GetIndexOfEvent(BaseEvent _curEvent)
        {
            return this.trackEvents.LastIndexOf(_curEvent);
        }

        public BaseEvent GetOffsetEvent(BaseEvent _curEvent, int _offset)
        {
            int num = this.trackEvents.LastIndexOf(_curEvent);
            if (this.Loop)
            {
                int num2 = (num + _offset) % this.trackEvents.Count;
                if (num2 < 0)
                {
                    num2 += this.trackEvents.Count;
                }
                return this.trackEvents[num2];
            }
            int num3 = num + _offset;
            if ((num3 >= 0) && (num3 < this.trackEvents.Count))
            {
                return this.trackEvents[num3];
            }
            return null;
        }

        public bool LocateEvent(int _curTime, out int _result)
        {
            _result = 0;
            int length = this.Length;
            int count = this.trackEvents.Count;
            if (count == 0)
            {
                return false;
            }
            if (this.Loop)
            {
                while (_curTime < 0)
                {
                    _curTime += length;
                }
                while (_curTime >= length)
                {
                    _curTime -= length;
                }
            }
            else if (_curTime < 0)
            {
                _curTime = 0;
            }
            else if (_curTime > length)
            {
                _curTime = length;
            }
            int num3 = -1;
            int num4 = 0;
            int num5 = this.trackEvents.Count - 1;
            while (num4 != num5)
            {
                int num6 = ((num4 + num5) / 2) + 1;
                if (_curTime < this.trackEvents[num6].time)
                {
                    num5 = num6 - 1;
                }
                else
                {
                    num4 = num6;
                }
            }
            int time = this.trackEvents[0].time;
            int num8 = this.trackEvents[count - 1].time;
            if ((num4 == 0) && (_curTime < time))
            {
                num3 = -1;
            }
            else
            {
                num3 = num4;
            }
            if (num3 < 0)
            {
                if (this.Loop)
                {
                    int num9 = time;
                    int num10 = length - num8;
                    _result = (count - 1) + ((_curTime + num10) / (num9 + num10));
                }
                else
                {
                    _result = -1 + (_curTime / time);
                }
            }
            else if (num3 == (count - 1))
            {
                if (this.Loop)
                {
                    int num11 = time;
                    int num12 = length - num8;
                    _result = (count - 1) + ((_curTime - num8) / (num11 + num12));
                }
                else
                {
                    _result = (count - 1) + ((_curTime - num8) / (length - num8));
                }
            }
            else
            {
                int num13 = this.trackEvents[num3].time;
                int num14 = this.trackEvents[num3 + 1].time;
                _result = num3 + ((_curTime - num13) / (num14 - num13));
            }
            return true;
        }

        private bool LocateInsertPos(int _curTime, out int _result)
        {
            _result = 0;
            int length = this.Length;
            int count = this.trackEvents.Count;
            if (count == 0)
            {
                return false;
            }
            if (_curTime < 0)
            {
                _curTime = 0;
            }
            else if (_curTime > length)
            {
                _curTime = length;
            }
            int num3 = -1;
            int num4 = 0;
            int num5 = this.trackEvents.Count - 1;
            while (num4 != num5)
            {
                int num6 = ((num4 + num5) / 2) + 1;
                if (_curTime < this.trackEvents[num6].time)
                {
                    num5 = num6 - 1;
                }
                else
                {
                    num4 = num6;
                }
            }
            int time = this.trackEvents[0].time;
            if ((num4 == 0) && (_curTime < time))
            {
                num3 = -1;
            }
            else
            {
                num3 = num4;
            }
            if (num3 < 0)
            {
                _result = 0;
            }
            else if (num3 == (count - 1))
            {
                _result = count;
            }
            else
            {
                _result = num3;
            }
            return true;
        }

        public override void OnRelease()
        {
            int count = this.trackEvents.Count;
            for (int i = 0; i < count; i++)
            {
                this.trackEvents[i].Release();
            }
            this.trackEvents.Clear();
            this.activeEvents.Clear();
            this.waitForConditions = null;
            this.action = null;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.waitForConditions = null;
            this.curTime = 0;
            this.preExcuteTime = 0;
            this.trackIndex = -1;
            this.trackName = string.Empty;
            this.color = Color.red;
            this.execOnActionCompleted = false;
            this.execOnForceStopped = false;
            this.eventType = null;
            this.isDurationEvent = false;
            this.isCondition = false;
            this.trackEvents.Clear();
            this.action = null;
            this.started = false;
            this.enabled = false;
            this.startCount = 0;
            this.supportEditMode = false;
            this.activeEvents.Clear();
        }

        public void Process(int _curTime)
        {
            this.preExcuteTime = this.curTime;
            this.curTime = _curTime;
            int num = 0;
            if (this.LocateEvent(_curTime, out num) && (num >= 0))
            {
                int length = this.Length;
                if ((_curTime >= length) && !this.Loop)
                {
                    num = this.trackEvents.Count - 1;
                }
                if (this.Loop)
                {
                    int num3 = ((num - 1) + this.trackEvents.Count) % this.trackEvents.Count;
                    int num4 = ((num + 1) + this.trackEvents.Count) % this.trackEvents.Count;
                    if (this.isDurationEvent)
                    {
                        int num5 = num3;
                        for (int i = 0; i < this.trackEvents.Count; i++)
                        {
                            DurationEvent item = this.trackEvents[num5] as DurationEvent;
                            if (this.CheckSkip(_curTime, item.Start) && item.CheckConditions(this.action))
                            {
                                if (this.activeEvents.Count == 0)
                                {
                                    item.Enter(this.action, this);
                                }
                                else
                                {
                                    DurationEvent event3 = this.activeEvents[0];
                                    if ((event3.Start < item.Start) && (event3.End < length))
                                    {
                                        int num7 = event3.End - item.Start;
                                        item.EnterBlend(this.action, this, event3, num7);
                                    }
                                    else if ((event3.Start < item.Start) && (event3.End >= length))
                                    {
                                        int num8 = event3.End - item.Start;
                                        item.EnterBlend(this.action, this, event3, num8);
                                    }
                                    else
                                    {
                                        int num9 = (event3.End - length) - item.Start;
                                        item.EnterBlend(this.action, this, event3, num9);
                                    }
                                }
                                this.activeEvents.Add(item);
                            }
                            if (this.CheckSkip(_curTime, item.End) && this.activeEvents.Contains(item))
                            {
                                item.Leave(this.action, this);
                                this.activeEvents.Remove(item);
                            }
                            num5 = (num5 + 1) % this.trackEvents.Count;
                        }
                    }
                    else
                    {
                        int num10 = num3;
                        for (int j = 0; j < this.trackEvents.Count; j++)
                        {
                            TickEvent event4 = this.trackEvents[num10] as TickEvent;
                            if (this.CheckSkip(_curTime, event4.time) && event4.CheckConditions(this.action))
                            {
                                event4.Process(this.action, this);
                            }
                            num10 = (num10 + 1) % this.trackEvents.Count;
                        }
                        if (num != num4)
                        {
                            TickEvent event5 = this.trackEvents[num] as TickEvent;
                            TickEvent event6 = this.trackEvents[num4] as TickEvent;
                            if (event6.time > event5.time)
                            {
                                float num12 = ((float) (_curTime - event5.time)) / ((float) (event6.time - event5.time));
                                event6.ProcessBlend(this.action, this, event5, num12);
                            }
                            else if (event6.time < event5.time)
                            {
                                if (_curTime >= event5.time)
                                {
                                    float num13 = ((float) (_curTime - event5.time)) / ((float) ((event6.time + length) - event5.time));
                                    event6.ProcessBlend(this.action, this, event5, num13);
                                }
                                else
                                {
                                    float num14 = ((float) ((_curTime + length) - event5.time)) / ((float) ((event6.time + length) - event5.time));
                                    event6.ProcessBlend(this.action, this, event5, num14);
                                }
                            }
                        }
                        else
                        {
                            TickEvent event7 = this.trackEvents[num] as TickEvent;
                            if (_curTime > event7.time)
                            {
                                int num15 = _curTime - event7.time;
                                event7.PostProcess(this.action, this, num15);
                            }
                            else if (_curTime < event7.time)
                            {
                                if (_curTime >= event7.time)
                                {
                                    int num16 = _curTime - event7.time;
                                    event7.PostProcess(this.action, this, num16);
                                }
                                else
                                {
                                    int num17 = (_curTime + length) - event7.time;
                                    event7.PostProcess(this.action, this, num17);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num18 = num - 1;
                    if (num18 < 0)
                    {
                        num18 = 0;
                    }
                    int num19 = num + 1;
                    if (num19 >= this.trackEvents.Count)
                    {
                        num19 = num;
                    }
                    if (this.isDurationEvent)
                    {
                        for (int k = num18; k < this.trackEvents.Count; k++)
                        {
                            DurationEvent event8 = this.trackEvents[k] as DurationEvent;
                            if (this.CheckSkip(_curTime, event8.Start) && event8.CheckConditions(this.action))
                            {
                                if (this.activeEvents.Count == 0)
                                {
                                    event8.Enter(this.action, this);
                                }
                                else
                                {
                                    DurationEvent event9 = this.activeEvents[0];
                                    int num21 = event9.End - event8.Start;
                                    event8.EnterBlend(this.action, this, event9, num21);
                                }
                                this.activeEvents.Add(event8);
                            }
                            if (this.CheckSkip(_curTime, event8.End) && this.activeEvents.Contains(event8))
                            {
                                if (this.activeEvents.Count > 1)
                                {
                                    DurationEvent event10 = this.activeEvents[1];
                                    int num22 = event8.End - event10.Start;
                                    event8.LeaveBlend(this.action, this, event10, num22);
                                }
                                else
                                {
                                    event8.Leave(this.action, this);
                                }
                                this.activeEvents.Remove(event8);
                            }
                        }
                    }
                    else
                    {
                        for (int m = num18; m < this.trackEvents.Count; m++)
                        {
                            TickEvent event11 = this.trackEvents[m] as TickEvent;
                            if (this.CheckSkip(_curTime, event11.time) && event11.CheckConditions(this.action))
                            {
                                event11.Process(this.action, this);
                            }
                        }
                        if (num != num19)
                        {
                            TickEvent event12 = this.trackEvents[num] as TickEvent;
                            TickEvent event13 = this.trackEvents[num19] as TickEvent;
                            float num24 = ((float) (_curTime - event12.time)) / ((float) (event13.time - event12.time));
                            event13.ProcessBlend(this.action, this, event12, num24);
                        }
                        else
                        {
                            TickEvent event14 = this.trackEvents[num] as TickEvent;
                            int num25 = _curTime - event14.time;
                            event14.PostProcess(this.action, this, num25);
                        }
                    }
                }
                if (this.activeEvents.Count == 1)
                {
                    DurationEvent event15 = this.activeEvents[0];
                    int num26 = 0;
                    if (_curTime >= event15.Start)
                    {
                        num26 = _curTime - event15.Start;
                    }
                    else
                    {
                        num26 = (_curTime + length) - event15.Start;
                    }
                    event15.Process(this.action, this, num26);
                }
                else if (this.activeEvents.Count == 2)
                {
                    DurationEvent event16 = this.activeEvents[0];
                    DurationEvent event17 = this.activeEvents[1];
                    if ((event16.Start < event17.Start) && (event16.End < length))
                    {
                        int num27 = _curTime - event17.Start;
                        int num28 = _curTime - event16.Start;
                        float num29 = ((float) (_curTime - event17.Start)) / ((float) (event16.End - event17.Start));
                        event17.ProcessBlend(this.action, this, num27, event16, num28, num29);
                    }
                    else if ((event16.Start < event17.Start) && (event16.End >= length))
                    {
                        if (_curTime >= event17.Start)
                        {
                            int num30 = _curTime - event17.Start;
                            int num31 = _curTime - event16.Start;
                            float num32 = ((float) (_curTime - event17.Start)) / ((float) (event16.End - event17.Start));
                            event17.ProcessBlend(this.action, this, num30, event16, num31, num32);
                        }
                        else
                        {
                            int num33 = (_curTime + length) - event17.Start;
                            int num34 = (_curTime + length) - event16.Start;
                            float num35 = ((float) ((_curTime + length) - event17.Start)) / ((float) (event16.End - event17.Start));
                            event17.ProcessBlend(this.action, this, num33, event16, num34, num35);
                        }
                    }
                    else
                    {
                        int num36 = _curTime - event17.Start;
                        int num37 = (_curTime + length) - event16.Start;
                        float num38 = ((float) (_curTime - event17.Start)) / ((float) ((event16.End - length) - event17.Start));
                        event17.ProcessBlend(this.action, this, num36, event16, num37, num38);
                    }
                }
            }
        }

        public void Start(AGE.Action _action)
        {
            if (this.enabled)
            {
                if (!this.isCondition)
                {
                    _action.SetCondition(this, true);
                }
                this.curTime = 0;
                this.preExcuteTime = 0;
                this.started = true;
                this.startCount++;
            }
        }

        public void Stop(AGE.Action _action)
        {
            if (this.started)
            {
                for (int i = 0; i < this.activeEvents.Count; i++)
                {
                    this.activeEvents[i].Leave(this.action, this);
                }
                this.activeEvents.Clear();
                if (!this.isCondition)
                {
                    _action.SetCondition(this, false);
                }
                this.started = false;
            }
        }

        public bool SupportEditMode()
        {
            return this.supportEditMode;
        }

        public System.Type EventType
        {
            get
            {
                return this.eventType;
            }
        }

        public bool IsDurationEvent
        {
            get
            {
                return this.isDurationEvent;
            }
        }

        public int Length
        {
            get
            {
                return this.action.length;
            }
        }

        public bool Loop
        {
            get
            {
                return this.action.loop;
            }
        }
    }
}

