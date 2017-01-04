namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public interface IStarCondition
    {
        void Dispose();
        bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker);
        void Initialize(ResDT_ConditionInfo InConditionInfo);
        void OnActorDeath(ref GameDeadEventParam prm);
        void OnCampScoreUpdated(ref SCampScoreUpdateParam prm);
        void Start();

        ResDT_ConditionInfo configInfo { get; }

        string description { get; }

        int extraType { get; }

        int[] keys { get; }

        string rawDescription { get; }

        StarEvaluationStatus status { get; }

        int type { get; }

        int[] values { get; }
    }
}

