namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class AddGoldInBattle : TickEvent
    {
        public int iGoldToAdd;

        public override BaseEvent Clone()
        {
            AddGoldInBattle battle = ClassObjPool<AddGoldInBattle>.Get();
            battle.CopyData(this);
            return battle;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            AddGoldInBattle battle = src as AddGoldInBattle;
            this.iGoldToAdd = battle.iGoldToAdd;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ValueComponent != null))
            {
                hostPlayer.Captain.handle.ValueComponent.ChangeGoldCoinInBattle(this.iGoldToAdd, true, false, new Vector3(), false);
            }
            else if (hostPlayer == null)
            {
                DebugHelper.Assert(false, "invalid host player");
            }
            else if (hostPlayer.Captain == 0)
            {
                DebugHelper.Assert(false, "invalid host player captain");
            }
            else if (hostPlayer.Captain.handle.ValueComponent == null)
            {
                DebugHelper.Assert(false, "invalid host player captain->valuecomponent");
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

