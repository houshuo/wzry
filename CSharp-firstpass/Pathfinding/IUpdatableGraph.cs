namespace Pathfinding
{
    using System;

    public interface IUpdatableGraph
    {
        GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o);
        void UpdateArea(GraphUpdateObject o);
        void UpdateAreaInit(GraphUpdateObject o);
    }
}

