namespace Assets.Scripts.GameLogic
{
    using System;

    public class CPlayerBehaviorStat
    {
        private static uint[] m_data = new uint[0x18];

        public static void Clear()
        {
            m_data = new uint[0x18];
        }

        public static uint GetData(BehaviorType type)
        {
            return m_data[(int) type];
        }

        public static void Plus(BehaviorType type)
        {
            m_data[(int) type]++;
        }

        private static void SetData(BehaviorType type, uint value)
        {
            m_data[(int) type] = value;
        }

        public enum BehaviorType
        {
            SortBYCoinBtnClick,
            Battle_TextChat_1,
            Battle_TextChat_2,
            Battle_TextChat_3,
            Battle_TextChat_4,
            Battle_TextChat_5,
            Battle_TextChat_6,
            Battle_TextChat_7,
            Battle_TextChat_8,
            Battle_TextChat_9,
            Battle_TextChat_10,
            Battle_TextChat_11,
            Battle_TextChat_12,
            Battle_TextChat_13,
            Battle_TextChat_14,
            Battle_TextChat_15,
            Battle_Voice_OpenSpeak,
            Battle_Voice_OpenMic,
            Battle_OpenBigMap,
            Battle_Signal_2,
            Battle_Signal_3,
            Battle_Signal_4,
            Battle_Signal_Textmsg,
            Battle_ButtonViewSkillInfo,
            Count
        }
    }
}

