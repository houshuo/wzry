namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Action : PooledClassObject
    {
        public string actionName = string.Empty;
        private List<PoolObjHandle<ActorRoot>> actorHandles = new List<PoolObjHandle<ActorRoot>>();
        private bool conditionChanged = true;
        private Dictionary<Track, bool> conditions = new Dictionary<Track, bool>();
        public int deltaTime;
        public bool enabled = true;
        public ListLinqView<GameObject> gameObjects = new ListLinqView<GameObject>();
        public int length = 0x1388;
        public bool loop;
        public static VFactor MinPlaySpeed = new VFactor(1L, 30L);
        public string name = string.Empty;
        public bool nextDestroy;
        public AGE.Action parentAction;
        public VFactor playSpeed = VFactor.one;
        private VFactor playSpeedOnPause = VFactor.zero;
        public int refGameObjectsCount = -1;
        public RefParamOperator refParams = new RefParamOperator();
        public RefParamOperator refParamsSrc;
        public bool started_;
        public Dictionary<string, int> templateObjectIds = new Dictionary<string, int>();
        private DictionaryView<uint, ListView<GameObject>> tempObjsAffectedByPlaySpeed = new DictionaryView<uint, ListView<GameObject>>();
        private int time;
        private ListView<Track> tracks = new ListView<Track>();
        public bool unstoppable;

        public event ActionStopDelegate onActionStop;

        public Action()
        {
            base.bChkReset = false;
        }

        public void AddGameObject(GameObject go)
        {
            this.gameObjects.Add(go);
            PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(go);
            this.actorHandles.Add(actorRoot);
        }

        public void AddTemplateObject(string str, int id)
        {
            this.templateObjectIds.Add(str, id);
        }

        public void AddTempObject(PlaySpeedAffectedType type, GameObject obj)
        {
            if (this.tempObjsAffectedByPlaySpeed == null)
            {
                this.tempObjsAffectedByPlaySpeed = new DictionaryView<uint, ListView<GameObject>>();
            }
            ListView<GameObject> view = null;
            if (!this.tempObjsAffectedByPlaySpeed.TryGetValue((uint) type, out view))
            {
                view = new ListView<GameObject>(8);
                this.tempObjsAffectedByPlaySpeed.Add((uint) type, view);
            }
            for (int i = 0; i < view.Count; i++)
            {
                GameObject obj2 = view[i];
                if (obj2 == obj)
                {
                    return;
                }
            }
            view.Add(obj);
            float single = this.playSpeed.single;
            if (type == PlaySpeedAffectedType.ePSAT_Anim)
            {
                Animation[] componentsInChildren = obj.GetComponentsInChildren<Animation>();
                if (componentsInChildren != null)
                {
                    for (int j = 0; j < componentsInChildren.Length; j++)
                    {
                        Animation animation = componentsInChildren[j];
                        if (animation.playAutomatically && (animation.clip != null))
                        {
                            AnimationState state = animation[animation.clip.name];
                            if (state != null)
                            {
                                state.speed = single;
                            }
                        }
                    }
                }
                Animator[] animatorArray = obj.GetComponentsInChildren<Animator>();
                if (animatorArray != null)
                {
                    for (int k = 0; k < animatorArray.Length; k++)
                    {
                        Animator animator = animatorArray[k];
                        animator.speed = single;
                    }
                }
            }
            else
            {
                ParticleSystem[] systemArray = obj.GetComponentsInChildren<ParticleSystem>();
                if (systemArray != null)
                {
                    for (int m = 0; m < systemArray.Length; m++)
                    {
                        ParticleSystem system = systemArray[m];
                        system.playbackSpeed = single;
                    }
                }
            }
        }

        public Track AddTrack(Track _track)
        {
            _track.action = this;
            _track.trackIndex = this.tracks.Count;
            this.tracks.Add(_track);
            this.conditions.Add(_track, false);
            return _track;
        }

        public Track AddTrack(System.Type _eventType)
        {
            Track item = new Track(this, _eventType) {
                trackIndex = this.tracks.Count
            };
            this.tracks.Add(item);
            this.conditions.Add(item, false);
            return item;
        }

        public void ClearGameObject(GameObject _gameObject)
        {
            for (int i = 0; i < this.gameObjects.Count; i++)
            {
                if (this.gameObjects[i] == _gameObject)
                {
                    this.SetGameObject(i, null);
                }
            }
        }

        public void CopyRefParams(AGE.Action resource)
        {
            this.refParamsSrc = resource.refParams;
            this.refParams.ClearParams();
            DictionaryView<string, SRefParam>.Enumerator enumerator = this.refParamsSrc.refParamList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, SRefParam> current = enumerator.Current;
                KeyValuePair<string, SRefParam> pair2 = enumerator.Current;
                this.refParams.refParamList.Add(current.Key, pair2.Value.Clone());
            }
        }

        public void ExpandGameObject(int maxIdx)
        {
            while (maxIdx >= this.gameObjects.Count)
            {
                this.AddGameObject(null);
            }
        }

        public void ForceStart()
        {
            this.time = 0;
            if (this.tracks != null)
            {
                int count = this.tracks.Count;
                for (int i = 0; i < count; i++)
                {
                    Track track = this.tracks[i];
                    if ((!track.execOnForceStopped && !track.execOnActionCompleted) && (track.waitForConditions == null))
                    {
                        track.Start(this);
                    }
                }
            }
        }

        public void ForceUpdate(int _time)
        {
            this.deltaTime = _time - this.time;
            this.time = _time;
            if (this.time > this.length)
            {
                if (!this.loop)
                {
                    this.time = this.length;
                    this.Process(this.time);
                    if (this.parentAction == null)
                    {
                        this.Stop(false);
                    }
                    return;
                }
                this.time -= this.length;
                int count = this.tracks.Count;
                for (int i = 0; i < count; i++)
                {
                    Track track = this.tracks[i];
                    if (track.waitForConditions == null)
                    {
                        track.DoLoop();
                    }
                }
            }
            this.Process(this.time);
        }

        public PoolObjHandle<ActorRoot> GetActorHandle(int _index)
        {
            if ((_index >= 0) && (_index < this.actorHandles.Count))
            {
                return this.actorHandles[_index];
            }
            return new PoolObjHandle<ActorRoot>();
        }

        public Dictionary<string, bool> GetAssociatedResources()
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            int count = this.tracks.Count;
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, bool> associatedResources = this.tracks[i].GetAssociatedResources();
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

        public void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID = 0)
        {
            if (results == null)
            {
                results = new Dictionary<object, AssetRefType>();
            }
            for (int i = 0; i < this.tracks.Count; i++)
            {
                Track track = this.tracks[i];
                if (track != null)
                {
                    track.GetAssociatedResources(results, markID);
                }
            }
        }

        public bool GetCondition(Track _track)
        {
            return this.conditions[_track];
        }

        public int GetConditionCount()
        {
            return this.conditions.Count;
        }

        public GameObject GetGameObject(int _index)
        {
            if ((_index >= 0) && (_index < this.gameObjects.Count))
            {
                return this.gameObjects[_index];
            }
            return null;
        }

        public ListLinqView<GameObject> GetGameObjectList()
        {
            return this.gameObjects;
        }

        public Track GetTrack(int _index)
        {
            if ((_index >= 0) && (_index < this.tracks.Count))
            {
                return this.tracks[_index];
            }
            return null;
        }

        public void GetTracks(System.Type evtType, ref ArrayList resLst)
        {
            if (resLst == null)
            {
                resLst = new ArrayList();
            }
            int count = this.tracks.Count;
            for (int i = 0; i < count; i++)
            {
                Track track = this.tracks[i];
                if ((track != null) && (track.EventType == evtType))
                {
                    resLst.Add(track);
                }
            }
        }

        public void InheritRefParams(AGE.Action resource)
        {
            DictionaryView<string, SRefParam>.Enumerator enumerator = resource.refParams.refParamList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, SRefParam> current = enumerator.Current;
                string key = current.Key;
                KeyValuePair<string, SRefParam> pair2 = enumerator.Current;
                SRefParam param = pair2.Value;
                KeyValuePair<string, SRefParam> pair3 = enumerator.Current;
                KeyValuePair<string, SRefParam> pair4 = enumerator.Current;
                this.refParams.SetOrAddRefParam(pair3.Key, pair4.Value);
            }
        }

        public void LoadAction(AGE.Action _actionResource, params GameObject[] _gameObjects)
        {
            this.length = _actionResource.length;
            this.loop = _actionResource.loop;
            this.time = 0;
            this.actionName = _actionResource.actionName;
            this.templateObjectIds = _actionResource.templateObjectIds;
            int count = _actionResource.tracks.Count;
            for (int i = 0; i < count; i++)
            {
                this.AddTrack(_actionResource.tracks[i].Clone());
            }
            foreach (GameObject obj2 in _gameObjects)
            {
                this.AddGameObject(obj2);
            }
            this.CopyRefParams(_actionResource);
        }

        public override void OnRelease()
        {
            this.playSpeedOnPause = VFactor.zero;
            this.deltaTime = 0;
            this.started_ = false;
            this.nextDestroy = false;
            this.enabled = true;
            this.name = string.Empty;
            this.length = 0x1388;
            this.loop = false;
            this.playSpeed = VFactor.one;
            this.unstoppable = false;
            this.actionName = string.Empty;
            this.refGameObjectsCount = -1;
            this.time = 0;
            this.parentAction = null;
            this.actorHandles.Clear();
            this.gameObjects.Clear();
            this.tracks.Clear();
            this.conditions.Clear();
            this.conditionChanged = true;
            this.refParams.Reset();
            this.refParamsSrc = null;
            this.templateObjectIds.Clear();
            this.tempObjsAffectedByPlaySpeed.Clear();
            this.onActionStop = (ActionStopDelegate) Delegate.RemoveAll(this.onActionStop, this.onActionStop);
        }

        public override void OnUse()
        {
            this.refParams.owner = this;
            DebugHelper.Assert((this.actorHandles.Count == 0) && (this.gameObjects.Count == 0), "age gameObjects is not null");
        }

        public void Pause()
        {
            if (this.enabled)
            {
                this.enabled = false;
                this.playSpeedOnPause = this.playSpeed;
                this.SetPlaySpeed(VFactor.zero);
            }
        }

        public void Play()
        {
            if (!this.enabled)
            {
                this.enabled = true;
                this.playSpeed = this.playSpeedOnPause;
                this.SetPlaySpeed(this.playSpeedOnPause);
            }
        }

        public void Process(int _time)
        {
            if (this.tracks != null)
            {
                int count = this.tracks.Count;
                for (int i = 0; i < count; i++)
                {
                    Track track = this.tracks[i];
                    if (track.waitForConditions == null)
                    {
                        if (track.started)
                        {
                            track.Process(_time);
                        }
                    }
                    else
                    {
                        if ((this.conditionChanged && !track.started) && ((track.startCount == 0) && track.CheckConditions(this)))
                        {
                            track.Start(this);
                            if (!this.loop)
                            {
                                int eventEndTime = track.GetEventEndTime();
                                if (this.length < eventEndTime)
                                {
                                    this.length = eventEndTime;
                                }
                            }
                        }
                        if (track.started)
                        {
                            track.Process(track.curTime + this.deltaTime);
                            if (track.curTime > track.GetEventEndTime())
                            {
                                track.Stop(this);
                            }
                        }
                    }
                }
            }
            this.conditionChanged = false;
        }

        public void RemoveTempObject(PlaySpeedAffectedType type, GameObject obj)
        {
            if (this.tempObjsAffectedByPlaySpeed != null)
            {
                ListView<GameObject> view = null;
                if (this.tempObjsAffectedByPlaySpeed.TryGetValue((uint) type, out view))
                {
                    view.Remove(obj);
                }
            }
        }

        public void ResetLength(int inLengthMs, bool bPlaySpeed)
        {
            if ((this.length > 0) && (inLengthMs != 0))
            {
                bool flag = true;
                bool flag2 = true;
                if (inLengthMs < 0)
                {
                    inLengthMs = 0x3fffffff;
                    flag = true;
                    flag2 = false;
                }
                VFactor factor = new VFactor((long) inLengthMs, (long) this.length);
                if (bPlaySpeed)
                {
                    this.SetPlaySpeed(factor.Inverse);
                }
                else
                {
                    this.length *= factor;
                    int count = this.tracks.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Track track = this.tracks[i];
                        if (track != null)
                        {
                            int num3 = track.trackEvents.Count;
                            for (int j = 0; j < num3; j++)
                            {
                                BaseEvent event2 = track.trackEvents[j];
                                if (event2 != null)
                                {
                                    if (event2.GetType().IsSubclassOf(typeof(DurationEvent)))
                                    {
                                        DurationEvent event3 = event2 as DurationEvent;
                                        if (flag2 && event3.bScaleStart)
                                        {
                                            event3.time *= factor;
                                        }
                                        if (event3.time > track.Length)
                                        {
                                            event3.time = track.Length;
                                        }
                                        if (flag && event3.bScaleLength)
                                        {
                                            event3.length *= factor;
                                        }
                                        if (event3.End > track.Length)
                                        {
                                            event3.End = track.Length;
                                        }
                                    }
                                    else
                                    {
                                        if (flag2 && event2.bScaleStart)
                                        {
                                            event2.time *= factor;
                                        }
                                        if (event2.time > track.Length)
                                        {
                                            event2.time = track.Length;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetCondition(Track _track, bool _status)
        {
            bool flag = this.conditions[_track];
            if (flag != _status)
            {
                this.conditionChanged = true;
                this.conditions[_track] = _status;
            }
        }

        public void SetGameObject(int _index, GameObject go)
        {
            this.actorHandles[_index] = ActorHelper.GetActorRoot(go);
            this.gameObjects[_index] = go;
        }

        public void SetPlaySpeed(VFactor _speed)
        {
            this.playSpeed = _speed;
            if (_speed.IsZero)
            {
                this.ForceUpdate(this.time);
                this.enabled = false;
            }
            else
            {
                if (this.playSpeed < MinPlaySpeed)
                {
                    this.playSpeed = MinPlaySpeed;
                }
                this.enabled = true;
            }
            this.UpdateTempObjectSpeed();
        }

        private void Start()
        {
            if (!ActionManager.Instance.frameMode)
            {
                this.ForceStart();
            }
        }

        public void Stop(bool bForce)
        {
            if (bForce)
            {
                if (this.tracks != null)
                {
                    int count = this.tracks.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Track track = this.tracks[i];
                        if (track.execOnForceStopped || track.execOnActionCompleted)
                        {
                            track.Start(this);
                            int deltaTime = this.deltaTime;
                            this.deltaTime = this.length;
                            track.Process(this.deltaTime);
                            track.Stop(this);
                            this.deltaTime = deltaTime;
                        }
                        else if (track.started)
                        {
                            track.Stop(this);
                        }
                    }
                }
                if (this.tempObjsAffectedByPlaySpeed != null)
                {
                    this.tempObjsAffectedByPlaySpeed.Clear();
                }
                if (this.onActionStop != null)
                {
                    PoolObjHandle<AGE.Action> action = new PoolObjHandle<AGE.Action>(this);
                    this.onActionStop(ref action);
                }
                if (this.tracks != null)
                {
                    for (int j = 0; j < this.tracks.Count; j++)
                    {
                        this.tracks[j].Release();
                    }
                    this.tracks.Clear();
                }
                if (ActionManager.Instance != null)
                {
                    ActionManager.Instance.RemoveAction(this);
                }
            }
            else if (!this.nextDestroy)
            {
                this.nextDestroy = true;
                ActionManager.Instance.DeferReleaseAction(this);
            }
        }

        private void Update()
        {
            if ((!ActionManager.Instance.frameMode && this.enabled) && !this.playSpeed.IsZero)
            {
                int num = ActionUtility.SecToMs(Time.deltaTime * this.playSpeed.single) + this.time;
                this.ForceUpdate(num);
            }
        }

        public void UpdateLogic(int nDelta)
        {
            if ((ActionManager.Instance.frameMode && this.enabled) && !this.playSpeed.IsZero)
            {
                if (!this.started_)
                {
                    this.ForceStart();
                    this.started_ = true;
                }
                int num = 0;
                if (this.playSpeed.nom == this.playSpeed.den)
                {
                    num = nDelta;
                }
                else
                {
                    num = (int) ((nDelta * this.playSpeed.nom) / this.playSpeed.den);
                }
                num += this.time;
                this.ForceUpdate(num);
            }
        }

        public void UpdateTempObjectForPreview(float _oldProgress, float _newProgress)
        {
        }

        private void UpdateTempObjectSpeed()
        {
            if (this.tempObjsAffectedByPlaySpeed != null)
            {
                float single = this.playSpeed.single;
                DictionaryView<uint, ListView<GameObject>>.Enumerator enumerator = this.tempObjsAffectedByPlaySpeed.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, ListView<GameObject>> current = enumerator.Current;
                    PlaySpeedAffectedType key = current.Key;
                    KeyValuePair<uint, ListView<GameObject>> pair2 = enumerator.Current;
                    ListView<GameObject> view = pair2.Value;
                    int count = view.Count;
                    for (int i = 0; i < count; i++)
                    {
                        GameObject obj2 = view[i];
                        switch (key)
                        {
                            case PlaySpeedAffectedType.ePSAT_Anim:
                            {
                                Animation[] componentsInChildren = obj2.GetComponentsInChildren<Animation>();
                                if (componentsInChildren != null)
                                {
                                    for (int j = 0; j < componentsInChildren.Length; j++)
                                    {
                                        Animation animation = componentsInChildren[j];
                                        if (animation.playAutomatically && (animation.clip != null))
                                        {
                                            AnimationState state = animation[animation.clip.name];
                                            if (state != null)
                                            {
                                                state.speed = single;
                                            }
                                        }
                                    }
                                }
                                Animator[] animatorArray = obj2.GetComponentsInChildren<Animator>();
                                if (animatorArray != null)
                                {
                                    for (int k = 0; k < animatorArray.Length; k++)
                                    {
                                        Animator animator = animatorArray[k];
                                        animator.speed = single;
                                    }
                                }
                                break;
                            }
                            case PlaySpeedAffectedType.ePSAT_Fx:
                            {
                                ParticleSystem[] systemArray = obj2.GetComponentsInChildren<ParticleSystem>();
                                if (systemArray != null)
                                {
                                    for (int m = 0; m < systemArray.Length; m++)
                                    {
                                        ParticleSystem system = systemArray[m];
                                        system.playbackSpeed = single;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public int CurrentTime
        {
            get
            {
                return this.time;
            }
        }

        public float CurrentTimeSec
        {
            get
            {
                return (this.time * 0.001f);
            }
        }

        public float LengthSec
        {
            get
            {
                return (this.length * 0.001f);
            }
        }

        public Dictionary<string, int> TemplateObjectIds
        {
            get
            {
                return this.templateObjectIds;
            }
        }

        public enum PlaySpeedAffectedType
        {
            ePSAT_Anim = 1,
            ePSAT_Fx = 2
        }
    }
}

