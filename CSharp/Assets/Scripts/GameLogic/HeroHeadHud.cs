namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    public class HeroHeadHud : MonoBehaviour
    {
        public PlayerHead[] heroHeads;
        public Vector3 pickedScale = new Vector3(1.15f, 1.15f, 1.15f);

        public void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_PickHeroHead, new CUIEventManager.OnUIEventHandler(this.onClickHead));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
        }

        public void Init()
        {
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetHostPlayer().GetAllHeroes().GetEnumerator();
            int index = -1;
            while (enumerator.MoveNext() && (++index < this.heroHeads.Length))
            {
                this.heroHeads[index].gameObject.CustomSetActive(true);
                this.heroHeads[index].Init(this, enumerator.Current);
                this.heroHeads[index].SetPicked(0 == index);
            }
            while (++index < this.heroHeads.Length)
            {
                this.heroHeads[index].gameObject.CustomSetActive(false);
            }
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_PickHeroHead, new CUIEventManager.OnUIEventHandler(this.onClickHead));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
        }

        public void onActorDead(ref GameDeadEventParam prm)
        {
            if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp == prm.src.handle.TheActorMeta.ActorCamp))
            {
                int index = -1;
                while (++index < this.heroHeads.Length)
                {
                    PlayerHead head = this.heroHeads[index];
                    if ((head.MyHero != 0) && (head.MyHero == prm.src))
                    {
                        head.OnMyHeroDead();
                        break;
                    }
                }
                if (((index < this.heroHeads.Length) && !Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode()) && ActorHelper.IsCaptainActor(ref prm.src))
                {
                    int num2 = -1;
                    while (++num2 < this.heroHeads.Length)
                    {
                        if ((num2 != index) && ((this.heroHeads[num2].MyHero != 0) && !this.heroHeads[num2].MyHero.handle.ActorControl.IsDeadState))
                        {
                            this.pickHeroHead(this.heroHeads[num2]);
                            break;
                        }
                    }
                }
            }
        }

        public void OnActorRevive(ref DefaultGameEventParam prm)
        {
            if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp == prm.src.handle.TheActorMeta.ActorCamp))
            {
                for (int i = 0; i < this.heroHeads.Length; i++)
                {
                    PlayerHead head = this.heroHeads[i];
                    if ((head.MyHero != 0) && (head.MyHero == prm.src))
                    {
                        head.OnMyHeroRevive();
                        break;
                    }
                }
            }
        }

        private void onClickHead(CUIEvent evt)
        {
            PlayerHead component = evt.m_srcWidget.GetComponent<PlayerHead>();
            if (component.state == PlayerHead.HeadState.ReviveReady)
            {
                if (component.MyHero.handle.ActorControl.CanRevive)
                {
                    component.MyHero.handle.ActorControl.Revive(false);
                }
            }
            else
            {
                this.pickHeroHead(component);
            }
        }

        public void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int curVal, int maxVal)
        {
            if (hero != 0)
            {
                for (int i = 0; i < this.heroHeads.Length; i++)
                {
                    PlayerHead head = this.heroHeads[i];
                    if ((head.MyHero != 0) && (head.MyHero == hero))
                    {
                        head.OnHeroHpChange(curVal, maxVal);
                        break;
                    }
                }
            }
        }

        public void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            if (hero != 0)
            {
                for (int i = 0; i < this.heroHeads.Length; i++)
                {
                    PlayerHead head = this.heroHeads[i];
                    if ((head.MyHero != 0) && (head.MyHero == hero))
                    {
                        head.OnHeroSoulLvlChange(level);
                        break;
                    }
                }
            }
        }

        public void pickHeroHead(PlayerHead ph)
        {
            if ((((ph.MyHero.handle != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle != null)) && (ph.MyHero != Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
            {
                for (int i = 0; i < this.heroHeads.Length; i++)
                {
                    PlayerHead head = this.heroHeads[i];
                    if ((null == head) || (head.MyHero == 0))
                    {
                        break;
                    }
                    head.SetPicked(ph == head);
                }
                FrameCommand<SwitchCaptainCommand> command = FrameCommandFactory.CreateFrameCommand<SwitchCaptainCommand>();
                command.cmdData.ObjectID = ph.MyHero.handle.ObjID;
                command.Send();
            }
        }
    }
}

