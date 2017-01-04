namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class TriggerActionAge : TriggerActionBase
    {
        private ListView<AGE.Action> m_duraActsPrivate;

        public TriggerActionAge(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
            this.m_duraActsPrivate = new ListView<AGE.Action>();
        }

        private void OnActionStopedPrivate(ref PoolObjHandle<AGE.Action> action)
        {
            if (action != 0)
            {
                action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStopedPrivate);
                this.m_duraActsPrivate.Remove(action.handle);
            }
        }

        private ListView<AGE.Action> PlayAgeActionPrivate(AreaEventTrigger.EActTiming inTiming, GameObject inSrc, GameObject inAtker)
        {
            return PlayAgeActionShared(inTiming, base.TimingActionsInter, new ActionStopDelegate(this.OnActionStopedPrivate), this.m_duraActsPrivate, inSrc, inAtker);
        }

        private static ListView<AGE.Action> PlayAgeActionShared(AreaEventTrigger.EActTiming inTiming, AreaEventTrigger.STimingAction[] inTimingActs, ActionStopDelegate inCallback, ListView<AGE.Action> outDuraActs, GameObject inSrc, GameObject inAtker)
        {
            ListView<AGE.Action> view = new ListView<AGE.Action>();
            foreach (AreaEventTrigger.STimingAction action in inTimingActs)
            {
                if (action.Timing == inTiming)
                {
                    ActionStopDelegate delegate2 = null;
                    if (inTiming == AreaEventTrigger.EActTiming.EnterDura)
                    {
                        delegate2 = inCallback;
                    }
                    AGE.Action item = PlayAgeActionShared(action.ActionName, action.HelperName, inSrc, inAtker, action.HelperIndex, inCallback);
                    if (item != null)
                    {
                        view.Add(item);
                        if (delegate2 != null)
                        {
                            outDuraActs.Add(item);
                        }
                    }
                }
            }
            return view;
        }

        private static AGE.Action PlayAgeActionShared(string inActionName, string inHelperName, GameObject inSrc, GameObject inAtker, int inHelperIndex = -1, ActionStopDelegate inCallback = null)
        {
            return DialogueProcessor.PlayAgeAction(inActionName, inHelperName, inSrc, inAtker, inCallback, inHelperIndex);
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            GameObject inSrc = (src == 0) ? null : src.handle.gameObject;
            GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
            if (inAtker == null)
            {
                inAtker = (atker == 0) ? null : atker.handle.gameObject;
            }
            this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Enter, inSrc, inAtker);
            ListView<AGE.Action> view = this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.EnterDura, inSrc, inAtker);
            RefParamOperator @operator = new RefParamOperator();
            @operator.AddRefParam("TriggerActionAgeEnterDura", view);
            return @operator;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            GameObject inSrc = (src == 0) ? null : src.handle.gameObject;
            GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
            this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Leave, inSrc, inAtker);
            AreaEventTrigger trigger = inTrigger as AreaEventTrigger;
            if (trigger != null)
            {
                AreaEventTrigger.STriggerContext context = trigger._inActors[src.handle.ObjID];
                RefParamOperator @operator = context.refParams[this];
                if (@operator != null)
                {
                    ListView<AGE.Action> refParamObject = @operator.GetRefParamObject<ListView<AGE.Action>>("TriggerActionAgeEnterDura");
                    if (refParamObject != null)
                    {
                        ListView<AGE.Action>.Enumerator enumerator = refParamObject.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.Stop(false);
                        }
                    }
                }
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            GameObject inSrc = (src == 0) ? null : src.handle.gameObject;
            GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
            this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Update, inSrc, inAtker);
        }
    }
}

