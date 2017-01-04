namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionBuff : TriggerActionBase
    {
        public TriggerActionBuff(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            int enterUniqueId = base.EnterUniqueId;
            GameObject[] refObjList = base.RefObjList;
            RefParamOperator @operator = new RefParamOperator();
            if (enterUniqueId > 0)
            {
                if (refObjList != null)
                {
                    int length = refObjList.Length;
                    for (int i = 0; i < length; i++)
                    {
                        GameObject go = refObjList[i];
                        if (go != null)
                        {
                            PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(go);
                            if (actorRoot != 0)
                            {
                                BufConsumer consumer = new BufConsumer(enterUniqueId, actorRoot, actorRoot);
                                if (consumer.Use())
                                {
                                    @operator = new RefParamOperator();
                                    string name = string.Format("TriggerActionBuffTar_{0}", i);
                                    @operator.AddRefParam(name, consumer.buffSkill);
                                }
                            }
                        }
                    }
                }
                if (base.bSrc && (src != 0))
                {
                    BufConsumer consumer2 = new BufConsumer(enterUniqueId, src, atker);
                    if (consumer2.Use())
                    {
                        @operator = new RefParamOperator();
                        @operator.AddRefParam("TriggerActionBuffSrc", consumer2.buffSkill);
                    }
                }
                if (base.bAtker && (atker != 0))
                {
                    BufConsumer consumer3 = new BufConsumer(enterUniqueId, atker, src);
                    if (consumer3.Use())
                    {
                        @operator = new RefParamOperator();
                        @operator.AddRefParam("TriggerActionBuffAtker", consumer3.buffSkill);
                    }
                }
            }
            return @operator;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            int leaveUniqueId = base.LeaveUniqueId;
            if (leaveUniqueId > 0)
            {
                new BufConsumer(leaveUniqueId, src, new PoolObjHandle<ActorRoot>(null)).Use();
            }
            int enterUniqueId = base.EnterUniqueId;
            if ((base.bStopWhenLeaving && (enterUniqueId > 0)) && (inTrigger != null))
            {
                AreaEventTrigger trigger = inTrigger as AreaEventTrigger;
                if (trigger != null)
                {
                    AreaEventTrigger.STriggerContext context = trigger._inActors[src.handle.ObjID];
                    RefParamOperator @operator = context.refParams[this];
                    if (@operator != null)
                    {
                        ListView<string> view = new ListView<string>();
                        GameObject[] refObjList = base.RefObjList;
                        if (refObjList != null)
                        {
                            int length = refObjList.Length;
                            for (int i = 0; i < length; i++)
                            {
                                view.Add(string.Format("TriggerActionBuffTar_{0}", i));
                            }
                        }
                        if (base.bSrc)
                        {
                            view.Add("TriggerActionBuffSrc");
                        }
                        if (base.bAtker)
                        {
                            view.Add("TriggerActionBuffAtker");
                        }
                        ListView<string>.Enumerator enumerator = view.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            if (!string.IsNullOrEmpty(current))
                            {
                                BuffFense refParamObject = @operator.GetRefParamObject<BuffFense>(current);
                                if (refParamObject != null)
                                {
                                    refParamObject.Stop();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            int updateUniqueId = base.UpdateUniqueId;
            if (updateUniqueId > 0)
            {
                new BufConsumer(updateUniqueId, src, atker).Use();
            }
        }
    }
}

