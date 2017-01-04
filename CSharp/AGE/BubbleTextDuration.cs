namespace AGE
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    [EventCategory("MMGame/Drama")]
    public class BubbleTextDuration : DurationEvent
    {
        private List<PoolObjHandle<ActorRoot>> actorRootList;
        public bool bPlayer1;
        public bool bPlayer2;
        public bool bPlayer3;
        public bool bTeammate1;
        public bool bTeammate2;
        public bool bTeammate3;
        public int BubbleTextId;
        public int Offset_x;
        public int Offset_y;
        public COM_PLAYERCAMP PlayerCamp;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        private void AddActorRootList(COM_PLAYERCAMP inPlayerCamp)
        {
            List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(inPlayerCamp);
            if (!this.bPlayer3 && (allCampPlayers.Count >= 3))
            {
                allCampPlayers.RemoveAt(2);
            }
            if (!this.bPlayer2 && (allCampPlayers.Count >= 2))
            {
                allCampPlayers.RemoveAt(1);
            }
            if (!this.bPlayer1 && (allCampPlayers.Count >= 1))
            {
                allCampPlayers.RemoveAt(0);
            }
            List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = enumerator.Current.GetAllHeroes();
                List<PoolObjHandle<ActorRoot>> collection = new List<PoolObjHandle<ActorRoot>>(allHeroes.Count);
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    collection.Add(allHeroes[i]);
                }
                if (!this.bTeammate3 && (collection.Count >= 3))
                {
                    collection.RemoveAt(2);
                }
                if (!this.bTeammate2 && (collection.Count >= 2))
                {
                    collection.RemoveAt(1);
                }
                if (!this.bTeammate1 && (collection.Count >= 1))
                {
                    collection.RemoveAt(0);
                }
                this.actorRootList.AddRange(collection);
            }
        }

        public override BaseEvent Clone()
        {
            BubbleTextDuration duration = ClassObjPool<BubbleTextDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BubbleTextDuration duration = src as BubbleTextDuration;
            this.srcId = duration.srcId;
            this.targetId = duration.targetId;
            this.bPlayer1 = duration.bPlayer1;
            this.bPlayer2 = duration.bPlayer2;
            this.bPlayer3 = duration.bPlayer3;
            this.bTeammate1 = duration.bTeammate1;
            this.bTeammate2 = duration.bTeammate2;
            this.bTeammate3 = duration.bTeammate3;
            this.PlayerCamp = duration.PlayerCamp;
            this.BubbleTextId = duration.BubbleTextId;
            this.Offset_x = duration.Offset_x;
            this.Offset_y = duration.Offset_y;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            if (this.BubbleTextId > 0)
            {
                this.actorRootList = new List<PoolObjHandle<ActorRoot>>();
                PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
                PoolObjHandle<ActorRoot> item = _action.GetActorHandle(this.targetId);
                if (actorHandle != 0)
                {
                    this.actorRootList.Add(actorHandle);
                }
                if (item != 0)
                {
                    this.actorRootList.Add(item);
                }
                if (this.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
                    this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
                }
                else
                {
                    this.AddActorRootList(this.PlayerCamp);
                }
                List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.actorRootList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.SetHudText(Utility.GetBubbleText((uint) this.BubbleTextId), enumerator.Current);
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actorRootList != null)
            {
                List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.actorRootList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.SetHudText(string.Empty, enumerator.Current);
                }
                this.actorRootList.Clear();
                this.actorRootList = null;
            }
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.srcId = 0;
            this.targetId = 0;
            this.bPlayer1 = false;
            this.bPlayer2 = false;
            this.bPlayer3 = false;
            this.bTeammate1 = false;
            this.bTeammate2 = false;
            this.bTeammate3 = false;
            this.PlayerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
            this.BubbleTextId = 0;
            this.Offset_x = 0;
            this.Offset_y = 0;
        }

        private void SetHudText(string inText, PoolObjHandle<ActorRoot> inActor)
        {
            if ((inActor != 0) && (inActor.handle.HudControl != null))
            {
                inActor.handle.HudControl.SetTextHud(inText, this.Offset_x, this.Offset_y, false);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

