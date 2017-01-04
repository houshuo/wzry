namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TargetDistanceFarFilter
    {
        private ulong minDisSqr;
        public TargetDistanceFarFilter(ulong _minDisSqr)
        {
            this.minDisSqr = _minDisSqr;
        }

        public ActorRoot Searcher(List<ActorRoot>.Enumerator _etr, ActorRoot _mainActor)
        {
            ActorRoot root = null;
            while (_etr.MoveNext())
            {
                ActorRoot current = _etr.Current;
                VInt3 num2 = current.location - _mainActor.location;
                ulong num = (ulong) num2.sqrMagnitudeLong2D;
                if (num > this.minDisSqr)
                {
                    root = current;
                    this.minDisSqr = num;
                }
            }
            return root;
        }
    }
}

