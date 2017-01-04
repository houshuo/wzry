namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Common;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GamePlayerCenter : Singleton<GamePlayerCenter>
    {
        private Player _hostPlayer;
        private readonly SortedDictionary<uint, Player> _players = new SortedDictionary<uint, Player>();
        public uint HostPlayerId;
        public const uint MaxPlayerNum = 10;

        public Player AddPlayer(uint playerId, COM_PLAYERCAMP camp, int campPos = 0, uint level = 1, bool isComputer = false, string name = null, int headIconId = 0, int logicWrold = 0, ulong uid = 0, uint vipLv = 0, string openId = null, uint gradeOfRank = 0, uint classOfRank = 0, int honorId = 0, int honorLevel = 0)
        {
            Player player = null;
            if (playerId == 0)
            {
                DebugHelper.Assert(false, "Try to create player by Id 0");
            }
            else if (this._players.ContainsKey(playerId))
            {
                object[] inParameters = new object[] { playerId };
                DebugHelper.Assert(false, "Try to create player which is already existed, ID is {0}", inParameters);
                player = this.GetPlayer(playerId);
            }
            else
            {
                Player player2 = new Player {
                    PlayerId = playerId,
                    LogicWrold = logicWrold,
                    PlayerUId = uid,
                    PlayerCamp = camp,
                    CampPos = campPos,
                    Level = (int) level,
                    HeadIconId = headIconId,
                    Computer = isComputer,
                    Name = CUIUtility.RemoveEmoji(name),
                    isGM = false,
                    VipLv = vipLv,
                    OpenId = openId,
                    GradeOfRank = gradeOfRank,
                    ClassOfRank = classOfRank,
                    HonorId = honorId,
                    HonorLevel = honorLevel
                };
                this._players.Add(playerId, player2);
                player = player2;
            }
            DebugHelper.Assert(this._players.Count <= 10L, "超出Player最大数量");
            return player;
        }

        public void ClearAllPlayers()
        {
            SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, Player> current = enumerator.Current;
                current.Value.ClearHeroes();
            }
            this._players.Clear();
            this._hostPlayer = null;
            this.HostPlayerId = 0;
            Debug.LogWarning("-----------------Venom---------PlayerCenter is CLEAR!");
        }

        public void ConnectActorRootAndPlayer(ref PoolObjHandle<ActorRoot> hero)
        {
            if (hero == 0)
            {
                DebugHelper.Assert(false, "Failed Connect Actor Root And Player, hero is null");
            }
            else
            {
                Player player = this.GetPlayer(hero.handle.TheActorMeta.PlayerId);
                if (player == null)
                {
                    object[] inParameters = new object[] { hero.handle.TheActorMeta.PlayerId };
                    DebugHelper.Assert(false, "Failed Find palyer {0}, failed connect actor.", inParameters);
                }
                else
                {
                    player.ConnectHeroActorRoot(ref hero);
                }
            }
        }

        public List<Player> GetAllCampPlayers(COM_PLAYERCAMP camp)
        {
            List<Player> list = new List<Player>();
            SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, Player> current = enumerator.Current;
                Player item = current.Value;
                if (item.PlayerCamp == camp)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public List<Player> GetAllPlayers()
        {
            List<Player> list = new List<Player>();
            SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, Player> current = enumerator.Current;
                Player item = current.Value;
                list.Add(item);
            }
            return list;
        }

        public Player GetHostPlayer()
        {
            if ((this._hostPlayer == null) || (this._hostPlayer.PlayerId != this.HostPlayerId))
            {
                this._hostPlayer = this.GetPlayer(this.HostPlayerId);
                DebugHelper.Assert(this._hostPlayer != null);
            }
            return this._hostPlayer;
        }

        public Player GetPlayer(uint playerId)
        {
            Player player = null;
            this._players.TryGetValue(playerId, out player);
            return player;
        }

        public Player GetPlayerByUid(ulong uid)
        {
            SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, Player> current = enumerator.Current;
                if (current.Value.PlayerUId == uid)
                {
                    KeyValuePair<uint, Player> pair2 = enumerator.Current;
                    return pair2.Value;
                }
            }
            return null;
        }

        public int GetPlayerCampPosIndex(uint playerId)
        {
            int num = 0;
            Player player = this.GetPlayer(playerId);
            if (player != null)
            {
                SortedDictionary<uint, Player>.Enumerator enumerator = this._players.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Player> current = enumerator.Current;
                    if (current.Value.PlayerId == playerId)
                    {
                        return num;
                    }
                    KeyValuePair<uint, Player> pair2 = enumerator.Current;
                    if (pair2.Value.PlayerCamp == player.PlayerCamp)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public override void Init()
        {
        }

        public bool IsAtSameCamp(uint player1Id, uint player2Id)
        {
            Player player = this.GetPlayer(player1Id);
            Player player2 = this.GetPlayer(player2Id);
            return (((player != null) && (player2 != null)) && (player.PlayerCamp == player2.PlayerCamp));
        }

        public void SetHostPlayer(uint playerId)
        {
            if (!this._players.ContainsKey(playerId))
            {
                object[] inParameters = new object[] { playerId };
                DebugHelper.Assert(false, "try to set hostplayer which is not exists in player lists. id={0}", inParameters);
            }
            else
            {
                object[] objArray2 = new object[] { playerId };
                DebugHelper.CustomLog("SetHostPlayer id = {0}", objArray2);
                this.HostPlayerId = playerId;
                this._players.TryGetValue(playerId, out this._hostPlayer);
            }
        }

        public COM_PLAYERCAMP hostPlayerCamp
        {
            get
            {
                Player hostPlayer = this.GetHostPlayer();
                return ((hostPlayer == null) ? COM_PLAYERCAMP.COM_PLAYERCAMP_MID : hostPlayer.PlayerCamp);
            }
        }

        public bool isHostPlayerCaptainDead
        {
            get
            {
                Player hostPlayer = this.GetHostPlayer();
                return ((((hostPlayer == null) || (hostPlayer.Captain == 0)) || (hostPlayer.Captain.handle.ActorControl == null)) || hostPlayer.Captain.handle.ActorControl.IsDeadState);
            }
        }
    }
}

