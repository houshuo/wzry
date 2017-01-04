namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class BufferMark : BaseSkill
    {
        public ResSkillMarkCfgInfo cfgData;
        private int curLayer;
        private int curTime;
        private int immuneTime;
        private int lastMaxTime;
        private BufferMarkParticle markParticle;
        private uint markType;
        private PoolObjHandle<ActorRoot> originActor;
        private PoolObjHandle<ActorRoot> sourceActor;
        private int triggerCDTime;

        public BufferMark(int _markID)
        {
            base.SkillID = _markID;
            this.cfgData = GameDataMgr.skillMarkDatabin.GetDataByKey((long) _markID);
            this.markParticle = new BufferMarkParticle();
        }

        public void AddLayer(int _addLayer)
        {
            int num = this.curLayer + _addLayer;
            num = (num <= this.cfgData.iMaxLayer) ? num : this.cfgData.iMaxLayer;
            this.SetCurLayer(num);
            this.curTime = 0;
        }

        public void AutoTrigger(PoolObjHandle<ActorRoot> _originator)
        {
            if (((this.curLayer + 1) >= this.cfgData.iMaxLayer) && (this.cfgData.bAutoTrigger == 1))
            {
                if (this.triggerCDTime == 0)
                {
                    this.AddLayer(1);
                    this.triggerCDTime = this.cfgData.iCDTime;
                    this.Trigger(_originator);
                }
                else
                {
                    this.curTime = 0;
                }
            }
            else
            {
                this.AddLayer(1);
            }
        }

        private bool CheckTargetType(uint typeMask)
        {
            if (typeMask == 0)
            {
                return true;
            }
            if (this.sourceActor != 0)
            {
                int actorType = (int) this.sourceActor.handle.TheActorMeta.ActorType;
                if ((typeMask & (((int) 1) << actorType)) > 0L)
                {
                    return true;
                }
            }
            return false;
        }

        public void DecLayer(int _decLayer)
        {
            int num = this.curLayer - _decLayer;
            num = (num <= 0) ? 0 : num;
            this.SetCurLayer(num);
        }

        public int GetCurLayer()
        {
            return this.curLayer;
        }

        public uint GetMarkType()
        {
            return this.markType;
        }

        public void Init(BuffHolderComponent _buffHolder, PoolObjHandle<ActorRoot> _originator, uint _markType)
        {
            this.sourceActor = _buffHolder.actorPtr;
            this.originActor = _originator;
            if (this.cfgData != null)
            {
                base.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szActionName);
                base.bAgeImmeExcute = this.cfgData.bAgeImmeExcute == 1;
            }
            this.markParticle.Init(this.cfgData);
            this.SetCurLayer(1);
            this.immuneTime = 0;
            this.triggerCDTime = 0;
            this.curTime = 0;
            this.markType = _markType;
            this.lastMaxTime = (this.cfgData.iLastMaxTime != 0) ? this.cfgData.iLastMaxTime : 0x7fffffff;
        }

        public override void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
        {
            action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStoped);
            if ((base.curAction != 0) && (action == base.curAction))
            {
                this.curAction.Release();
            }
        }

        public void SetCurLayer(int _layer)
        {
            this.curLayer = _layer;
            if ((this.cfgData != null) && this.CheckTargetType(this.cfgData.dwEffectMask))
            {
                this.markParticle.PlayParticle(ref this.originActor, ref this.sourceActor, this.curLayer);
            }
        }

        public void Trigger(PoolObjHandle<ActorRoot> _originator)
        {
            if (this.immuneTime == 0)
            {
                if ((this.cfgData != null) && (this.cfgData.bLayerEffect == 1))
                {
                    base.skillContext.MarkCount = this.curLayer;
                }
                this.DecLayer(this.cfgData.iCostLayer);
                this.immuneTime = this.cfgData.iImmuneTime;
                this.TriggerAction(_originator);
            }
        }

        private void TriggerAction(PoolObjHandle<ActorRoot> _originator)
        {
            SkillUseParam param = new SkillUseParam();
            param.SetOriginator(_originator);
            param.TargetActor = this.sourceActor;
            this.Use(_originator, ref param);
        }

        public void UpdateLogic(int nDelta)
        {
            if (this.immuneTime > 0)
            {
                this.immuneTime -= nDelta;
                this.immuneTime = (this.immuneTime <= 0) ? 0 : this.immuneTime;
            }
            if (this.triggerCDTime > 0)
            {
                this.triggerCDTime -= nDelta;
                this.triggerCDTime = (this.triggerCDTime <= 0) ? 0 : this.triggerCDTime;
            }
            this.curTime += nDelta;
            if (this.curTime >= this.lastMaxTime)
            {
                this.SetCurLayer(0);
            }
        }

        public void UpperTrigger(PoolObjHandle<ActorRoot> _originator)
        {
            if ((this.curLayer >= this.cfgData.iTriggerLayer) && (this.cfgData.iTriggerLayer > 0))
            {
                this.Trigger(_originator);
            }
        }
    }
}

