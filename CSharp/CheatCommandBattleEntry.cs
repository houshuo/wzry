using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;

[CheatCommandEntry("战斗")]
internal class CheatCommandBattleEntry : BoolenFlagConverter
{
    [CheatCommandEntryMethod("加1000局内金币", true, false)]
    public static string AddGoldCoinInBattle()
    {
        FrameCommand<PlayerAddGoldCoinInBattleCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerAddGoldCoinInBattleCommand>();
        command.cmdData.m_addValue = 0xc350;
        command.Send();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("燃烧远征/刷新战力", true, false)]
    public static string BurnClearPower()
    {
        BurnExpeditionNetCore.Clear_ResetBurning_Power();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("燃烧远征/刷新重置", true, false)]
    public static string BurnReset()
    {
        BurnExpeditionNetCore.Clear_ResetBurning_Limit();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("自动战斗", true, false)]
    public static string ForceAutoAI()
    {
        FrameCommand<SwitchActorAutoAICommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
        command.cmdData.IsAutoAI = 1;
        command.Send();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("切换无敌状态", true, false)]
    public static string GodMode()
    {
        if (!LobbyMsgHandler.isHostGMAcnt)
        {
            return "没有gm权限";
        }
        Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
        if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ActorControl is HeroWrapper))
        {
            HeroWrapper actorControl = hostPlayer.Captain.handle.ActorControl as HeroWrapper;
            FrameCommand<SwitchActorSwitchGodMode> command = FrameCommandFactory.CreateFrameCommand<SwitchActorSwitchGodMode>();
            command.cmdData.IsGodMode = !actorControl.bGodMode ? ((sbyte) 1) : ((sbyte) 0);
            command.Send();
            return CheatCommandBase.Done;
        }
        return "无法获取到正确的角色信息";
    }

    [CheatCommandEntryMethod("切换一击必杀", true, false)]
    public static string SuperKiller()
    {
        if (!LobbyMsgHandler.isHostGMAcnt)
        {
            return "没有gm权限";
        }
        Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
        if ((hostPlayer != null) && (hostPlayer.Captain != 0))
        {
            FrameCommand<SwitchActorSuperKillerCommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorSuperKillerCommand>();
            command.cmdData.IsSuperKiller = !hostPlayer.Captain.handle.bOneKiller ? ((sbyte) 1) : ((sbyte) 0);
            command.Send();
            return CheatCommandBase.Done;
        }
        return "无法获取到正确的角色信息";
    }
}

