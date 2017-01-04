namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TargetDistanceNearFilter
    {
        private ulong maxDisSqr;
        public TargetDistanceNearFilter(ulong _maxDisSqr)
        {
            this.maxDisSqr = _maxDisSqr;
        }

        public ActorRoot Searcher(List<ActorRoot>.Enumerator _etr, ActorRoot _mainActor)
        {
            ActorRoot root = null;
            while (_etr.MoveNext())
            {
                ActorRoot current = _etr.Current;
                VInt3 num2 = current.location - _mainActor.location;
                ulong num = (ulong) num2.sqrMagnitudeLong2D;
                if (num < this.maxDisSqr)
                {
                    root = current;
                    this.maxDisSqr = num;
                }
            }
            return root;
        }
    }
}

