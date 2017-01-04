using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;

[CheatCommandEntry("英雄")]
internal class CheatCommandHeroEntry
{
    [CheatCommandEntryMethod("技能/设置技能等级", true, false)]
    public static string SetHeroSkillLevel(SkillSlotType Slot, byte Level)
    {
        if (Singleton<GameStateCtrl>.instance.isBattleState)
        {
            FrameCommand<SetSkillLevelInBattleCommand> command = FrameCommandFactory.CreateFrameCommand<SetSkillLevelInBattleCommand>();
            command.cmdData.SkillSlot = (byte) Slot;
            command.cmdData.SkillLevel = Level;
            command.Send();
            return "已发出指令";
        }
        return "仅限局内";
    }
}

