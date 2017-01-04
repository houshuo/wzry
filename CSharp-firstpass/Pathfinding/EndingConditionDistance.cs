namespace Pathfinding
{
    using System;

    public class EndingConditionDistance : PathEndingCondition
    {
        public int maxGScore;

        public EndingConditionDistance(Path p, int maxGScore) : base(p)
        {
            this.maxGScore = 100;
            this.maxGScore = maxGScore;
        }

        public override bool TargetFound(PathNode node)
        {
            return (node.G >= this.maxGScore);
        }
    }
}

