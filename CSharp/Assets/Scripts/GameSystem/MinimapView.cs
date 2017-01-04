namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MinimapView
    {
        private DictionaryView<CTailsman, GameObject> m_mapPointer = new DictionaryView<CTailsman, GameObject>();
        private CUIContainerScript m_mapPointerContainer;
        private ListView<SpawnGroupCounter> m_spawnGroupCounter = new ListView<SpawnGroupCounter>();
        private CUIContainerScript m_spawnGroupCounterContainer;

        private void AddCharmIcon(CTailsman inCharm)
        {
            if ((inCharm != null) && !this.m_mapPointer.ContainsKey(inCharm))
            {
                this.m_mapPointerContainer = this.GetMapPointerContainer();
                if (this.m_mapPointerContainer != null)
                {
                    int element = this.m_mapPointerContainer.GetElement();
                    if (element >= 0)
                    {
                        RectTransform mapPointerRectTransform = null;
                        GameObject obj2 = this.m_mapPointerContainer.GetElement(element);
                        if (obj2 != null)
                        {
                            mapPointerRectTransform = obj2.transform as RectTransform;
                            obj2.GetComponent<Image>().SetSprite(string.Format("{0}{1}", "UGUI/Sprite/Battle/", "Img_Map_Base_Green"), Singleton<CBattleSystem>.instance.FormScript, true, false, false);
                        }
                        if (mapPointerRectTransform != null)
                        {
                            mapPointerRectTransform.SetAsFirstSibling();
                        }
                        Vector3 zero = Vector3.zero;
                        if (inCharm.Presentation != null)
                        {
                            zero = inCharm.Presentation.transform.position;
                        }
                        SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                        if ((curLvelContext != null) && curLvelContext.IsMobaMode())
                        {
                            this.UpdateUIMap(mapPointerRectTransform, zero, (float) curLvelContext.m_mapWidth, (float) curLvelContext.m_mapHeight);
                        }
                        this.m_mapPointer.Add(inCharm, obj2);
                    }
                }
            }
        }

        private void AddSpawnGroupCounter(int inCountdown, int inAlertPreroll, Vector3 inInitPos, SpawnerWrapper.ESpawnObjectType inObjType)
        {
            this.m_spawnGroupCounterContainer = this.GetSpawnGroupTextContainer();
            if (this.m_spawnGroupCounterContainer != null)
            {
                int element = this.m_spawnGroupCounterContainer.GetElement();
                if (element >= 0)
                {
                    SpawnGroupCounter counter;
                    counter = new SpawnGroupCounter {
                        CountdownTime = inCountdown,
                        timer = counter.CountdownTime,
                        AlertTime = counter.timer - inAlertPreroll,
                        bAlertPreroll = inAlertPreroll > 0,
                        bDidAlert = false
                    };
                    RectTransform mapPointerRectTransform = null;
                    GameObject p = this.m_spawnGroupCounterContainer.GetElement(element);
                    if (p != null)
                    {
                        mapPointerRectTransform = p.transform as RectTransform;
                        counter.TextObj = p;
                        counter.timerText = Utility.FindChild(p, "TimerText").GetComponent<Text>();
                    }
                    if (mapPointerRectTransform != null)
                    {
                        mapPointerRectTransform.SetAsFirstSibling();
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if ((curLvelContext != null) && curLvelContext.IsMobaMode())
                    {
                        this.UpdateUIMap(mapPointerRectTransform, inInitPos, (float) curLvelContext.m_mapWidth, (float) curLvelContext.m_mapHeight);
                    }
                    this.m_spawnGroupCounter.Add(counter);
                }
            }
        }

        public void Clear()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, new RefAction<SCommonSpawnEventParam>(this.OnSpawnGroupStartCount));
            Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, new RefAction<STailsmanEventParam>(this.OnTailsmanSpawn));
            Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.OnTailsmanUsed));
            this.m_mapPointerContainer = null;
            this.m_spawnGroupCounterContainer = null;
        }

        private void Draw()
        {
            ListView<SpawnGroupCounter>.Enumerator enumerator = this.m_spawnGroupCounter.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SpawnGroupCounter current = enumerator.Current;
                if ((current != null) && (current.timerText != null))
                {
                    int num = current.timer / 0x3e8;
                    int num2 = num / 60;
                    int num3 = num - (num2 * 60);
                    current.timerText.text = string.Format("{0:D2}:{1:D2}", num2, num3);
                }
            }
        }

        private CUIContainerScript GetMapPointerContainer()
        {
            return null;
        }

        private CUIContainerScript GetSpawnGroupTextContainer()
        {
            return null;
        }

        public void Init(GameObject dragonInfo, SpawnGroup dragonSpawnGroup)
        {
            Singleton<GameEventSys>.instance.AddEventHandler<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, new RefAction<SCommonSpawnEventParam>(this.OnSpawnGroupStartCount));
            Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, new RefAction<STailsmanEventParam>(this.OnTailsmanSpawn));
            Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.OnTailsmanUsed));
        }

        private void OnSpawnGroupStartCount(ref SCommonSpawnEventParam param)
        {
            this.AddSpawnGroupCounter(param.LeftTime, param.AlertPreroll, (Vector3) param.SpawnPos, param.SpawnObjType);
        }

        private void OnTailsmanSpawn(ref STailsmanEventParam param)
        {
            this.AddCharmIcon(param.tailsman.handle);
        }

        private void OnTailsmanUsed(ref STailsmanEventParam param)
        {
            this.RemoveCharmIcon(param.tailsman.handle);
        }

        public void RemoveCharmIcon(CTailsman inCharm)
        {
            if ((inCharm != null) && this.m_mapPointer.ContainsKey(inCharm))
            {
                GameObject elementObject = this.m_mapPointer[inCharm];
                this.m_mapPointer.Remove(inCharm);
                if ((elementObject != null) && (this.m_mapPointerContainer != null))
                {
                    this.m_mapPointerContainer.RecycleElement(elementObject);
                }
            }
        }

        public void UpdateLogic(int inDelta)
        {
            if (this.m_spawnGroupCounter.Count > 0)
            {
                for (int i = this.m_spawnGroupCounter.Count - 1; i >= 0; i--)
                {
                    SpawnGroupCounter counter = this.m_spawnGroupCounter[i];
                    counter.timer -= inDelta;
                    if ((counter.bAlertPreroll && !counter.bDidAlert) && (counter.timer <= counter.AlertTime))
                    {
                        counter.bDidAlert = true;
                    }
                    if (counter.timer <= 0)
                    {
                        if ((this.m_spawnGroupCounterContainer != null) && (counter.TextObj != null))
                        {
                            this.m_spawnGroupCounterContainer.RecycleElement(counter.TextObj);
                        }
                        counter.TextObj = null;
                        counter.timerText = null;
                        this.m_spawnGroupCounter.RemoveAt(i);
                    }
                }
            }
            this.Draw();
        }

        private void UpdateUIMap(RectTransform mapPointerRectTransform, Vector3 actorPosition, float mapWidth, float mapHeight)
        {
            if (((mapPointerRectTransform != null) && (mapWidth != 0f)) && (mapHeight != 0f))
            {
                float x = actorPosition.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x;
                float y = actorPosition.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y;
                mapPointerRectTransform.anchoredPosition = new Vector2(x, y);
            }
        }

        private class SpawnGroupCounter
        {
            public int AlertTime;
            public bool bAlertPreroll;
            public bool bDidAlert;
            public int CountdownTime;
            public GameObject TextObj;
            public int timer;
            public Text timerText;
        }
    }
}

