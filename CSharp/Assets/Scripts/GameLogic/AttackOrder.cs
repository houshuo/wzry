namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;

    public class AttackOrder
    {
        private DictionaryView<int, List<PoolObjHandle<ActorRoot>>> _orderDepends;
        private Dictionary<int, PoolObjHandle<ActorRoot>> _orderOwner;

        public void FightOver()
        {
            this._orderDepends = null;
            this._orderOwner = null;
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        public void FightStart()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            this._orderDepends = new DictionaryView<int, List<PoolObjHandle<ActorRoot>>>();
            this._orderOwner = new Dictionary<int, PoolObjHandle<ActorRoot>>();
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            for (int i = 0; i < gameActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = gameActors[i];
                int battleOrder = handle.handle.ObjLinker.BattleOrder;
                if ((battleOrder != 0) && !this._orderOwner.ContainsKey(battleOrder))
                {
                    this._orderOwner.Add(battleOrder, handle);
                }
                int[][] numArray = new int[][] { handle.handle.ObjLinker.BattleOrderDepend };
                if (numArray != null)
                {
                    for (int j = 0; j < numArray.Length; j++)
                    {
                        int[] numArray2 = numArray[j];
                        if (numArray2 != null)
                        {
                            for (int k = 0; k < numArray2.Length; k++)
                            {
                                int key = numArray2[k];
                                if (key != 0)
                                {
                                    handle.handle.AttackOrderReady = false;
                                    List<PoolObjHandle<ActorRoot>> list2 = null;
                                    if (this._orderDepends.ContainsKey(key))
                                    {
                                        list2 = this._orderDepends[key];
                                    }
                                    else
                                    {
                                        list2 = new List<PoolObjHandle<ActorRoot>>();
                                        this._orderDepends.Add(key, list2);
                                    }
                                    list2.Add(handle);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            int battleOrder = prm.src.handle.ObjLinker.BattleOrder;
            if ((battleOrder != 0) && this._orderDepends.ContainsKey(battleOrder))
            {
                List<PoolObjHandle<ActorRoot>> list = this._orderDepends[battleOrder];
                for (int i = 0; i < list.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = list[i];
                    if (handle != 0)
                    {
                        int[][] numArray = new int[][] { handle.handle.ObjLinker.BattleOrderDepend };
                        if (numArray == null)
                        {
                            handle.handle.AttackOrderReady = true;
                        }
                        else
                        {
                            for (int j = 0; j < numArray.Length; j++)
                            {
                                int[] numArray2 = numArray[j];
                                if (numArray2 != null)
                                {
                                    bool flag = false;
                                    for (int k = 0; k < numArray2.Length; k++)
                                    {
                                        int key = numArray2[k];
                                        if ((key != 0) && this._orderOwner.ContainsKey(key))
                                        {
                                            PoolObjHandle<ActorRoot> handle2 = this._orderOwner[key];
                                            if (handle2.handle.ActorControl.IsDeadState)
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (flag || (numArray2.Length == 0))
                                    {
                                        handle.handle.AttackOrderReady = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

