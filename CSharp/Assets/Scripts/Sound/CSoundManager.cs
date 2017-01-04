namespace Assets.Scripts.Sound
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CSoundManager : Singleton<CSoundManager>
    {
        private bool m_isPrepared;
        private List<string>[] m_loadedBanks;
        private GameObject m_soundRoot;

        public override void Init()
        {
            this.m_soundRoot = new GameObject("CSoundManager");
            GameObject obj2 = GameObject.Find("BootObj");
            if (obj2 != null)
            {
                this.m_soundRoot.transform.parent = obj2.transform;
            }
            this.m_loadedBanks = new List<string>[Enum.GetNames(typeof(BankType)).Length];
            for (int i = 0; i < this.m_loadedBanks.Length; i++)
            {
                this.m_loadedBanks[i] = new List<string>();
            }
            this.m_isPrepared = false;
        }

        public void LoadBank(string bankName, BankType bankType)
        {
            if (this.m_isPrepared && !this.m_loadedBanks[(int) bankType].Contains(bankName))
            {
                if (AkInitializer.s_loadBankFromMemory)
                {
                    string soundBankPathInResources = AkInitializer.GetSoundBankPathInResources(bankName);
                    CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource(soundBankPathInResources, typeof(TextAsset), enResourceType.Sound, false, false).m_content as CBinaryObject;
                    if (content != null)
                    {
                        AkBankManager.LoadBank(bankName, content.m_data);
                    }
                    Singleton<CResourceManager>.GetInstance().RemoveCachedResource(soundBankPathInResources);
                }
                else
                {
                    AkBankManager.LoadBank(bankName);
                }
                this.m_loadedBanks[(int) bankType].Add(bankName);
            }
        }

        public void LoadHeroSoundBank(string bankName)
        {
            int index = 1;
            if (!this.m_loadedBanks[index].Contains(bankName))
            {
                if (this.m_loadedBanks[index].Count >= 10)
                {
                    this.UnLoadBank(this.m_loadedBanks[index][0], BankType.Lobby);
                }
                this.LoadBank(bankName, BankType.Lobby);
            }
            else
            {
                this.m_loadedBanks[index].Remove(bankName);
                this.m_loadedBanks[index].Add(bankName);
            }
        }

        public void LoadSkinSoundBank(uint heroId, uint skinId, GameObject obj, bool bLobby)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
            if ((heroSkin == null) && (skinId != 0))
            {
                heroSkin = CSkinInfo.GetHeroSkin(heroId, 0);
            }
            if (heroSkin != null)
            {
                if (!string.IsNullOrEmpty(heroSkin.szSkinSoundResPack))
                {
                    if (bLobby)
                    {
                        this.LoadHeroSoundBank(heroSkin.szSkinSoundResPack + "_Show");
                    }
                    else
                    {
                        this.LoadBank(heroSkin.szSkinSoundResPack + "_SFX", BankType.Battle);
                        this.LoadBank(heroSkin.szSkinSoundResPack + "_VO", BankType.Battle);
                    }
                }
                if (!string.IsNullOrEmpty(heroSkin.szSoundSwitchEvent))
                {
                    this.PostEvent(heroSkin.szSoundSwitchEvent, obj);
                }
            }
            else
            {
                object[] inParameters = new object[] { heroId, skinId };
                DebugHelper.Assert(false, "Default sound resource can not find heroId = {0}skinId ={1}", inParameters);
            }
        }

        public uint PlayBattleSound(string eventName, PoolObjHandle<ActorRoot> actor, GameObject srcGameObject = null)
        {
            if ((GameSettings.EnableSound && !MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover) && ((actor == 0) || actor.handle.Visible))
            {
                return this.PostEvent(eventName, srcGameObject);
            }
            return 0;
        }

        public uint PlayBattleSound2D(string eventName)
        {
            if (GameSettings.EnableSound && !MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                return this.PostEvent(eventName, null);
            }
            return 0;
        }

        public uint PlayHeroActSound(string soundName)
        {
            if (!GameSettings.EnableSound || MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                return 0;
            }
            if (string.IsNullOrEmpty(soundName))
            {
                return 0;
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle.ActorControl != null)) && !hostPlayer.Captain.handle.ActorControl.IsDeadState)
            {
                return this.PostEvent(soundName, hostPlayer.Captain.handle.gameObject);
            }
            return this.PostEvent(soundName, null);
        }

        public uint PostEvent(string eventName, GameObject srcGameObject = null)
        {
            if (this.m_isPrepared)
            {
                if (srcGameObject == null)
                {
                    if (Camera.main != null)
                    {
                        srcGameObject = Camera.main.gameObject;
                    }
                    else
                    {
                        srcGameObject = this.m_soundRoot;
                    }
                }
                if (srcGameObject != null)
                {
                    return AkSoundEngine.PostEvent(eventName, srcGameObject);
                }
            }
            return 0;
        }

        public void Prepare()
        {
            if (!this.m_isPrepared)
            {
                this.m_soundRoot.AddComponent<AkTerminator>();
                this.m_soundRoot.AddComponent<AkInitializer>();
                this.m_isPrepared = true;
            }
        }

        public override void UnInit()
        {
        }

        public void UnLoadBank(string bankName, BankType bankType)
        {
            if (this.m_isPrepared && this.m_loadedBanks[(int) bankType].Remove(bankName))
            {
                AkBankManager.UnloadBank(bankName);
            }
        }

        public void UnloadBanks(BankType bankType)
        {
            if (this.m_isPrepared)
            {
                List<string> list = this.m_loadedBanks[(int) bankType];
                for (int i = 0; i < list.Count; i++)
                {
                    AkBankManager.UnloadBank(list[i]);
                }
                list.Clear();
            }
        }

        public enum BankType
        {
            Global,
            Lobby,
            Battle,
            LevelMusic
        }
    }
}

