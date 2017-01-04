namespace Assets.Scripts.GameLogic.DataCenter
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class ActorDataCenter : Singleton<ActorDataCenter>
    {
        private readonly DictionaryView<uint, IGameActorDataProvider> _providers = new DictionaryView<uint, IGameActorDataProvider>();
        private ActorServerDataProvider _serverDataProvider;

        public void AddHeroesServerData(uint playerId, COMDT_CHOICEHERO[] serverHeroInfos)
        {
            for (int i = 0; i < serverHeroInfos.Length; i++)
            {
                this.AddHeroServerInfo(playerId, serverHeroInfos[i]);
            }
        }

        public void AddHeroServerInfo(uint playerId, COMDT_CHOICEHERO serverHeroInfo)
        {
            this._serverDataProvider.AddHeroServerInfo(playerId, serverHeroInfo);
        }

        public void ClearHeroServerData()
        {
            this._serverDataProvider.ClearHeroServerInfo();
        }

        public IGameActorDataProvider GetActorDataProvider(GameActorDataProviderType providerType)
        {
            IGameActorDataProvider provider = null;
            this._providers.TryGetValue((uint) providerType, out provider);
            return provider;
        }

        public override void Init()
        {
            base.Init();
            this._serverDataProvider = new ActorServerDataProvider();
            this._providers.Add(1, new ActorStaticLobbyDataProvider());
            this._providers.Add(2, new ActorStaticBattleDataProvider());
            this._providers.Add(3, this._serverDataProvider);
            DictionaryView<uint, IGameActorDataProvider>.Enumerator enumerator = this._providers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, IGameActorDataProvider> current = enumerator.Current;
                ActorDataProviderBase base2 = current.Value as ActorDataProviderBase;
                if (base2 != null)
                {
                    base2.Init();
                }
            }
        }
    }
}

