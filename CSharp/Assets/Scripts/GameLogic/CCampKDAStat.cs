namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class CCampKDAStat
    {
        private uint[] m_campTotalDamage = new uint[2];
        private uint[] m_campTotalTakenDamage = new uint[2];
        private uint[] m_campTotalToHeroDamage = new uint[2];

        private void GetTeamInfoByPlayerKda(PlayerKDA kda)
        {
            if ((kda != null) && ((kda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) || (kda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)))
            {
                ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    HeroKDA current = enumerator.Current;
                    if (current != null)
                    {
                        this.m_campTotalDamage[((int) kda.PlayerCamp) - 1] += (uint) current.hurtToEnemy;
                        this.m_campTotalTakenDamage[((int) kda.PlayerCamp) - 1] += (uint) current.hurtTakenByEnemy;
                        this.m_campTotalToHeroDamage[((int) kda.PlayerCamp) - 1] += (uint) current.hurtToHero;
                    }
                }
            }
        }

        private void GetTeamKDA(DictionaryView<uint, PlayerKDA> playerKDAStat)
        {
            if (playerKDAStat != null)
            {
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA kda = current.Value;
                    this.GetTeamInfoByPlayerKda(kda);
                }
            }
        }

        public uint GetTeamTotalDamage(COM_PLAYERCAMP camp)
        {
            if ((camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_1) && (camp <= COM_PLAYERCAMP.COM_PLAYERCAMP_2))
            {
                return this.m_campTotalDamage[((int) camp) - 1];
            }
            return 0;
        }

        public uint GetTeamTotalTakenDamage(COM_PLAYERCAMP camp)
        {
            if ((camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_1) && (camp <= COM_PLAYERCAMP.COM_PLAYERCAMP_2))
            {
                return this.m_campTotalTakenDamage[((int) camp) - 1];
            }
            return 0;
        }

        public uint GetTeamTotalToHeroDamage(COM_PLAYERCAMP camp)
        {
            if ((camp >= COM_PLAYERCAMP.COM_PLAYERCAMP_1) && (camp <= COM_PLAYERCAMP.COM_PLAYERCAMP_2))
            {
                return this.m_campTotalToHeroDamage[((int) camp) - 1];
            }
            return 0;
        }

        public void Initialize(DictionaryView<uint, PlayerKDA> playerKDAStat)
        {
            for (int i = 1; i <= 2; i++)
            {
                this.m_campTotalDamage[i - 1] = 0;
                this.m_campTotalTakenDamage[i - 1] = 0;
                this.m_campTotalToHeroDamage[i - 1] = 0;
            }
            this.GetTeamKDA(playerKDAStat);
        }

        public uint camp1TotalDamage
        {
            get
            {
                return this.GetTeamTotalDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
        }

        public uint camp1TotalTakenDamage
        {
            get
            {
                return this.GetTeamTotalTakenDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
        }

        public uint camp1TotalToHeroDamage
        {
            get
            {
                return this.GetTeamTotalToHeroDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
        }

        public uint camp2TotalDamage
        {
            get
            {
                return this.GetTeamTotalDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            }
        }

        public uint camp2TotalTakenDamage
        {
            get
            {
                return this.GetTeamTotalTakenDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            }
        }

        public uint camp2TotalToHeroDamage
        {
            get
            {
                return this.GetTeamTotalToHeroDamage(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
        }
    }
}

