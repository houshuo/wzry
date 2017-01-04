namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    [EventCategory("MMGame/Drama")]
    public class TeamMemberBuffTick : TickEvent
    {
        public bool bPlayer1;
        public bool bPlayer2;
        public bool bPlayer3;
        public bool bPlayer4;
        public bool bPlayer5;
        public bool bSkipDead = true;
        public bool bTeammate1;
        public bool bTeammate2;
        public bool bTeammate3;
        [AssetReference(AssetRefType.SkillCombine)]
        public int BuffID;
        public COM_PLAYERCAMP PlayerCamp;

        private void AddActorRootList(COM_PLAYERCAMP inPlayerCamp, ref List<PoolObjHandle<ActorRoot>> actorRootList)
        {
            List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(inPlayerCamp);
            if (!this.bPlayer5 && (allCampPlayers.Count >= 5))
            {
                allCampPlayers.RemoveAt(4);
            }
            if (!this.bPlayer4 && (allCampPlayers.Count >= 4))
            {
                allCampPlayers.RemoveAt(3);
            }
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
                List<PoolObjHandle<ActorRoot>> list2 = new List<PoolObjHandle<ActorRoot>>(allHeroes.Count);
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    list2.Add(allHeroes[i]);
                }
                if (!this.bTeammate3 && (list2.Count >= 3))
                {
                    list2.RemoveAt(2);
                }
                if (!this.bTeammate2 && (list2.Count >= 2))
                {
                    list2.RemoveAt(1);
                }
                if (!this.bTeammate1 && (list2.Count >= 1))
                {
                    list2.RemoveAt(0);
                }
                if (!this.bSkipDead)
                {
                    actorRootList.AddRange<PoolObjHandle<ActorRoot>>(allHeroes);
                }
                else
                {
                    int count = allHeroes.Count;
                    for (int j = 0; j < count; j++)
                    {
                        PoolObjHandle<ActorRoot> item = allHeroes[j];
                        if ((item != 0) && !item.handle.ActorControl.IsDeadState)
                        {
                            actorRootList.Add(item);
                        }
                    }
                }
            }
        }

        public override BaseEvent Clone()
        {
            TeamMemberBuffTick tick = ClassObjPool<TeamMemberBuffTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TeamMemberBuffTick tick = src as TeamMemberBuffTick;
            this.bPlayer1 = tick.bPlayer1;
            this.bPlayer2 = tick.bPlayer2;
            this.bPlayer3 = tick.bPlayer3;
            this.bPlayer4 = tick.bPlayer4;
            this.bPlayer5 = tick.bPlayer5;
            this.bTeammate1 = tick.bTeammate1;
            this.bTeammate2 = tick.bTeammate2;
            this.bTeammate3 = tick.bTeammate3;
            this.PlayerCamp = tick.PlayerCamp;
            this.BuffID = tick.BuffID;
            this.bSkipDead = tick.bSkipDead;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.bPlayer1 = false;
            this.bPlayer2 = false;
            this.bPlayer3 = false;
            this.bPlayer4 = false;
            this.bPlayer5 = false;
            this.bTeammate1 = false;
            this.bTeammate2 = false;
            this.bTeammate3 = false;
            this.PlayerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
            this.BuffID = 0;
            this.bSkipDead = true;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            if (this.BuffID > 0)
            {
                List<PoolObjHandle<ActorRoot>> actorRootList = new List<PoolObjHandle<ActorRoot>>();
                if (this.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ref actorRootList);
                    this.AddActorRootList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ref actorRootList);
                }
                else
                {
                    this.AddActorRootList(this.PlayerCamp, ref actorRootList);
                }
                List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = actorRootList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    new BufConsumer(this.BuffID, enumerator.Current, enumerator.Current).Use();
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

