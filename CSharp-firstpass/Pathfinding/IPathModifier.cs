namespace Pathfinding
{
    using System;

    public interface IPathModifier
    {
        void Apply(Path p, ModifierData source);
        void ApplyOriginal(Path p);
        void PreProcess(Path p);

        ModifierData input { get; }

        ModifierData output { get; }

        int Priority { get; set; }
    }
}

