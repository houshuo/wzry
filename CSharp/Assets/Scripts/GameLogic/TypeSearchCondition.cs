namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class TypeSearchCondition
    {
        public static bool Fit(ActorRoot _actor, ActorTypeDef _actorType)
        {
            return (_actor.TheActorMeta.ActorType == _actorType);
        }

        public static bool Fit(ActorRoot _actor, ActorTypeDef _actorType, bool isBoss)
        {
            return ((_actor.TheActorMeta.ActorType == _actorType) && (_actor.ActorControl.IsBossOrHeroAutoAI() == isBoss));
        }

        public static bool FitWithJungleMonsterNotInBattle(ActorRoot _actor, bool bWithMonsterNotInBattle)
        {
            if (_actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
            {
                return false;
            }
            if (!bWithMonsterNotInBattle)
            {
                MonsterWrapper actorControl = _actor.ActorControl as MonsterWrapper;
                if (actorControl != null)
                {
                    ResMonsterCfgInfo cfgInfo = actorControl.cfgInfo;
                    if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                    {
                        switch (actorControl.myBehavior)
                        {
                            case ObjBehaviMode.State_Idle:
                            case ObjBehaviMode.State_Dead:
                            case ObjBehaviMode.State_Null:
                                return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}

