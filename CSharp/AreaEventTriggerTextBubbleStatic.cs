using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class AreaEventTriggerTextBubbleStatic : AreaEventTrigger
{
    [FriendlyName("起效时间")]
    public float active_time;
    private int active_timer = -1;
    private PoolObjHandle<ActorRoot> actorRoot;
    [FriendlyName("剧情对话组ID")]
    public int DialogueGroupId;
    public int offset_x;
    public int offset_y;
    public GameObject target;
    private int timer = -1;
    [FriendlyName("持续时间")]
    public float total_time;

    private void Clear()
    {
        if (this.timer != -1)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
        }
        if (this.active_timer != -1)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.active_timer);
        }
        this.SetHudText(string.Empty);
        Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        this.actorRoot.Release();
        this.target = null;
    }

    protected override DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnter(ref PoolObjHandle<ActorRoot> inActor)
    {
        if (this.target != null)
        {
            this.actorRoot = ActorHelper.GetActorRoot(this.target);
            if ((this.actorRoot == 0) || this.actorRoot.handle.isRecycled)
            {
                return null;
            }
            if (this.total_time > 0f)
            {
                this.timer = Singleton<CTimerManager>.GetInstance().AddTimer((int) (this.total_time * 1000f), 1, new CTimer.OnTimeUpHandler(this.OnTimeUp));
                Singleton<CTimerManager>.instance.PauseTimer(this.timer);
            }
            if (this.active_time > 0f)
            {
                this.active_timer = Singleton<CTimerManager>.GetInstance().AddTimer((int) (this.active_time * 1000f), 1, new CTimer.OnTimeUpHandler(this.OnActiveTimeUp));
            }
            else if (((this.actorRoot != 0) && !this.actorRoot.handle.ActorControl.IsDeadState) && (this.actorRoot.handle.HudControl != null))
            {
                this.actorRoot.handle.HudControl.SetTextHud(Utility.GetBubbleText((uint) this.DialogueGroupId), this.offset_x, this.offset_y, false);
            }
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        }
        return null;
    }

    protected override void DoActorLeave(ref PoolObjHandle<ActorRoot> inActor)
    {
    }

    protected override void DoActorUpdate(ref PoolObjHandle<ActorRoot> inActor)
    {
    }

    private void OnActiveTimeUp(int timersequence)
    {
        if ((this.actorRoot != 0) && (this.actorRoot.handle.HudControl != null))
        {
            if (this.total_time > 0f)
            {
                Singleton<CTimerManager>.instance.ResumeTimer(this.timer);
            }
            this.actorRoot.handle.HudControl.SetTextHud(Utility.GetBubbleText((uint) this.DialogueGroupId), this.offset_x, this.offset_y, false);
        }
    }

    private void OnActorDeath(ref GameDeadEventParam prm)
    {
        if ((prm.src != 0) && (prm.src == this.actorRoot))
        {
            this.Clear();
        }
    }

    private void OnTimeUp(int timersequence)
    {
        this.Clear();
    }

    private void SetHudText(string txt)
    {
        if (this.actorRoot != 0)
        {
            this.actorRoot.handle.HudControl.SetTextHud(txt, this.offset_x, this.offset_y, false);
        }
    }
}

