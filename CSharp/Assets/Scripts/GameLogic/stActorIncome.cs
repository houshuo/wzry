namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stActorIncome
    {
        public ActorRoot m_actorRoot;
        public enIncomeType m_incomeType;
        public uint m_incomeValue;
    }
}

