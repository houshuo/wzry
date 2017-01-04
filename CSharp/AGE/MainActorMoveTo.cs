namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class MainActorMoveTo : TickEvent
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int atkerId = 1;
        [ObjectTemplate(new System.Type[] {  })]
        public int destId = 2;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId;
        public Vector3 TargetPos = new Vector3(0f, 0f, 0f);

        public override BaseEvent Clone()
        {
            MainActorMoveTo to = ClassObjPool<MainActorMoveTo>.Get();
            to.CopyData(this);
            return to;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MainActorMoveTo to = src as MainActorMoveTo;
            this.srcId = to.srcId;
            this.atkerId = to.atkerId;
            this.destId = to.destId;
            this.TargetPos = to.TargetPos;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            Vector3 targetPos = this.TargetPos;
            GameObject gameObject = _action.GetGameObject(this.destId);
            if (gameObject != null)
            {
                targetPos = gameObject.transform.position;
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
            if ((actorHandle == 0) || ((hostPlayer.Captain != 0) && (actorHandle == hostPlayer.Captain)))
            {
                if (hostPlayer.Captain.handle.ActorControl != null)
                {
                    FrameCommandFactory.CreateFrameCommand<StopMoveCommand>().Send();
                    FrameCommand<MoveToPosCommand> command2 = FrameCommandFactory.CreateFrameCommand<MoveToPosCommand>();
                    command2.cmdData.destPosition = (VInt3) targetPos;
                    command2.Send();
                }
            }
            else if (actorHandle != 0)
            {
                actorHandle.handle.ActorControl.RealMovePosition((VInt3) targetPos, 0);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

