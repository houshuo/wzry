namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class TriggerActionTextBubble : TriggerActionBase
    {
        private ListView<ActorRootInfo> m_actorTimerMap;

        public TriggerActionTextBubble(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
            this.m_actorTimerMap = new ListView<ActorRootInfo>();
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private void Clear()
        {
            ListView<ActorRootInfo>.Enumerator enumerator = this.m_actorTimerMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ActorRootInfo current = enumerator.Current;
                PoolObjHandle<ActorRoot> actor = current.Actor;
                ActorRootInfo info2 = enumerator.Current;
                int id = info2.Id;
                this.SetHudText(string.Empty, actor);
                Singleton<CTimerManager>.GetInstance().RemoveTimer(id);
            }
            this.m_actorTimerMap.Clear();
        }

        public override void Destroy()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private int FindActor(PoolObjHandle<ActorRoot> InActor)
        {
            for (int i = 0; i < this.m_actorTimerMap.Count; i++)
            {
                ActorRootInfo info = this.m_actorTimerMap[i];
                if (info.Actor == InActor)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool HasActor(PoolObjHandle<ActorRoot> InActor)
        {
            return (this.FindActor(InActor) != -1);
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            PoolObjHandle<ActorRoot> src = prm.src;
            int index = this.FindActor(src);
            if (index != -1)
            {
                this.SetHudText(string.Empty, src);
                ActorRootInfo info = this.m_actorTimerMap[index];
                Singleton<CTimerManager>.GetInstance().RemoveTimer(info.Id);
                this.m_actorTimerMap.RemoveAt(index);
            }
        }

        private void OnTimeUp(int timersequence)
        {
            PoolObjHandle<ActorRoot> inActor = new PoolObjHandle<ActorRoot>();
            int index = -1;
            for (int i = 0; i < this.m_actorTimerMap.Count; i++)
            {
                ActorRootInfo info = this.m_actorTimerMap[i];
                if (info.Id == timersequence)
                {
                    index = i;
                    inActor = info.Actor;
                    break;
                }
            }
            if ((inActor != 0) && (index != -1))
            {
                this.SetHudText(string.Empty, inActor);
                this.m_actorTimerMap.RemoveAt(index);
            }
            Singleton<CTimerManager>.GetInstance().RemoveTimer(timersequence);
        }

        private void SetHudText(string inText, PoolObjHandle<ActorRoot> inActor)
        {
            if ((inActor != 0) && (inActor.handle.HudControl != null))
            {
                inActor.handle.HudControl.SetTextHud(inText, base.Offset_x, base.Offset_y, false);
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
            if (base.bSrc && (src != 0))
            {
                list.Add(src);
            }
            if (base.bAtker && (atker != 0))
            {
                list.Add(atker);
            }
            if ((base.RefObjList != null) && (base.RefObjList.Length > 0))
            {
                foreach (GameObject obj2 in base.RefObjList)
                {
                    if (obj2 != null)
                    {
                        SpawnPoint componentInChildren = obj2.GetComponentInChildren<SpawnPoint>();
                        if (componentInChildren != null)
                        {
                            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = componentInChildren.GetSpawnedList().GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                PoolObjHandle<ActorRoot> current = enumerator.Current;
                                if (current != 0)
                                {
                                    list.Add(current);
                                }
                            }
                        }
                        else
                        {
                            PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(obj2);
                            if (actorRoot != 0)
                            {
                                list.Add(actorRoot);
                            }
                        }
                    }
                }
            }
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator2 = list.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                if ((enumerator2.Current != 0) && !this.HasActor(enumerator2.Current))
                {
                    this.SetHudText(Utility.GetBubbleText((uint) base.EnterUniqueId), enumerator2.Current);
                    if (base.TotalTime > 0)
                    {
                        int num2 = Singleton<CTimerManager>.GetInstance().AddTimer(base.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
                        ActorRootInfo item = new ActorRootInfo {
                            Actor = enumerator2.Current,
                            Id = num2
                        };
                        this.m_actorTimerMap.Add(item);
                    }
                }
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                this.Clear();
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ActorRootInfo
        {
            public PoolObjHandle<ActorRoot> Actor;
            public int Id;
        }
    }
}

