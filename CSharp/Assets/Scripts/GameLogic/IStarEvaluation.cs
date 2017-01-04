namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public interface IStarEvaluation
    {
        void Dispose();
        IStarCondition GetConditionAt(int Index);
        ListView<IStarCondition>.Enumerator GetEnumerator();
        void Initialize(ResEvaluateStarInfo InStarInfo);
        void OnActorDeath(ref GameDeadEventParam prm);
        void OnCampScoreUpdated(ref SCampScoreUpdateParam prm);
        void Start();

        ResEvaluateStarInfo configInfo { get; }

        string description { get; }

        int index { get; }

        bool isFailure { get; }

        bool isInProgressing { get; }

        bool isSuccess { get; }

        string rawDescription { get; }

        StarEvaluationStatus status { get; }
    }
}

