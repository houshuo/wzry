namespace Assets.Scripts.UI
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stUIEventParams
    {
        public stSnsFriendEventParams snsFriendEventParams;
        public stItemGetInfoParams itemGetInfoParams;
        public stSkillTipParams skillTipParam;
        public stHeroSkinEventParams heroSkinParam;
        public CUseable iconUseable;
        public SkillSlotType m_skillSlotType;
        public enSelectGameType heroSelectGameType;
        public stSymbolEventParams symbolParam;
        public stSymbolTransformParams symbolTransParam;
        public stDianQuanBuyParam dianQuanBuyPar;
        public stOpenHeroFormParams openHeroFormPar;
        public stBattleEquipParams battleEquipPar;
        public stFriendHeroSkinParams friendHeroSkinPar;
        public uint heroId;
        public List<uint> heroIdList;
        public uint taskId;
        public uint weakGuideId;
        public int selectIndex;
        public int tag;
        public int tag2;
        public int tag3;
        public uint commonUInt32Param1;
        public ushort commonUInt16Param1;
        public ulong commonUInt64Param1;
        public ulong commonUInt64Param2;
        public string tagStr;
        public string tagStr1;
        public uint tagUInt;
        public int skillSlotId;
        public float sliderValue;
        public bool togleIsOn;
        public bool commonBool;
    }
}

