namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class GuidePathIndicatorTick : TickEvent
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int atkerId = 1;
        public bool bPlay;
        [ObjectTemplate(new System.Type[] {  })]
        public int destId = 2;
        private PathIndicator MyPathIndicator;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId;
        public Vector3 TargetPos = new Vector3(0f, 0f, 0f);

        public override BaseEvent Clone()
        {
            GuidePathIndicatorTick tick = ClassObjPool<GuidePathIndicatorTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            GuidePathIndicatorTick tick = src as GuidePathIndicatorTick;
            this.srcId = tick.srcId;
            this.atkerId = tick.atkerId;
            this.destId = tick.destId;
            this.TargetPos = tick.TargetPos;
            this.bPlay = tick.bPlay;
            this.MyPathIndicator = tick.MyPathIndicator;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.MyPathIndicator = null;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            GameObject gameObject = _action.GetGameObject(this.srcId);
            GameObject inDest = _action.GetGameObject(this.destId);
            if ((gameObject == null) && (hostPlayer.Captain != 0))
            {
                gameObject = hostPlayer.Captain.handle.gameObject;
            }
            if (gameObject != null)
            {
                if (this.MyPathIndicator == null)
                {
                    this.MyPathIndicator = UnityEngine.Object.FindObjectOfType<PathIndicator>();
                }
                if (this.MyPathIndicator != null)
                {
                    if (this.bPlay)
                    {
                        this.MyPathIndicator.Play(gameObject, inDest, ref this.TargetPos);
                    }
                    else
                    {
                        this.MyPathIndicator.Stop();
                    }
                }
            }
            this.MyPathIndicator = null;
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

