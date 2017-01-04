namespace Assets.Scripts.GameSystem
{
    using System;

    public class CSignalTipsElement
    {
        public EType type;

        public CSignalTipsElement(EType type)
        {
            this.type = type;
        }

        public enum EType
        {
            None,
            Signal,
            InBattleMsg
        }
    }
}

