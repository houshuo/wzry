namespace Assets.Scripts.Framework
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameSettings
    {
        private static CastType _castType = CastType.LunPanCast;
        private static bool _EnableHDMode;
        private static bool _EnableKingTimeMode;
        private static bool _EnableMusic = true;
        private static bool _enableOutline;
        private static bool _EnableRecorderMode;
        private static bool _EnableReplay;
        private static bool _EnableReplayKit;
        private static bool _EnableReplayKitAutoMode;
        private static bool _EnableSound = true;
        private static float _huaDongSensitivity;
        private static CommonAttactType _normalAttackType;
        private static SelectEnemyType _selectType = SelectEnemyType.SelectLowHp;
        private static SkillCancleType _skillCancleTyoe;
        private static float _yaoGanSensitivity;
        private static CameraHeightType cameraHeight = CameraHeightType.Medium;
        private static CameraMoveType cameraMoveType;
        public static int DefaultScreenHeight;
        public static int DefaultScreenWidth;
        public static SGameRenderQuality DeviceLevel = SGameRenderQuality.Low;
        private const string GameSettingCameraHuaDongSensitivity = "GameSettingCameraHuaDongSensitivity";
        private const string GameSettingCameraMoveType = "GameSettingCameraMoveType";
        private const string GameSettingCameraYaoGanSensitivity = "GameSettingCameraYaoGanSensitivity";
        private const string GameSettingCastType = "GameSettings_CastType";
        private const string GameSettingCommonAttackType = "GameSetting_CommonAttackType";
        private const string GameSettingEnableHDMode = "GameSettingEnableHDMode";
        private const string GameSettingEnableKingTimeMode = "GameSettingEnableKingTime";
        private const string GameSettingEnableRecorderMode = "GameSettingEnableRecorderMode";
        private const string GameSettingEnableReplay = "GameSetting_EnableReplay";
        private const string GameSettingEnableReplayKit = "GameSettingEnableReplayKit";
        private const string GameSettingEnableReplayKitAutoMode = "GameSettingEnableReplayKitAutoMode";
        private const string GameSettingEnableVibrate = "GameSettingEnableVibrate";
        private const string GameSettingLunPanSensitivity = "GameSettings_LunPanCastSensitivity";
        private const string GameSettingMusicEffectLevel = "GameSettingMusicEffectLevel";
        private const string GameSettingSelecEnemyType = "GameSettings_SelectEnemyType";
        private const string GameSettingSkillCancleType = "GameSettingSkillCancleType";
        private const string GameSettingSoundEffectLevel = "GameSettingSoundEffectLevel";
        private const string GameSettingVoiceEffectLevel = "GameSettingVoiceEffectLevel";
        private const float LunPanMaxAngularSpd = 2f;
        private const float LunPanMaxSpd = 0.02f;
        private const float LunPanMinAngularSpd = 0.2f;
        private const float LunPanMinSpd = 0.2f;
        private static int m_clickEnableInBattleInputChat = 1;
        private static bool m_dynamicParticleLOD = true;
        private static bool m_EnableVibrate = true;
        private static bool m_EnableVoice;
        private static int m_fpsShowType;
        private static int m_joystickMoveType;
        private static int m_joystickShowType;
        private static float m_MusicEffectLevel = 100f;
        private static float m_SoundEffectLevel = 100f;
        private static float m_VoiceEffectLevel = 100f;
        private const float MaxAll = 10f;
        public const int maxScreenHeight = 720;
        public const int maxScreenWidth = 0x500;
        public static SGameRenderQuality MaxShadowQuality;
        public static SGameRenderQuality ParticleQuality;
        public static SGameRenderQuality RenderQuality;
        private static float s_lunpanSensitivity = 1f;
        public const string str_cameraHeight = "cameraHeight";
        public const string str_enableMusic = "sgameSettings_muteMusic";
        public const string str_enableSound = "sgameSettings_muteSound";
        public const string str_enableVoice = "sgameSettings_EnableVoice";
        public const string str_fpsShowType = "str_fpsShowType";
        public const string str_inBatInputShowType = "str_inBatInputShowType";
        public const string str_joystickMoveType = "joystickMoveType";
        public const string str_joystickShowType = "joystickShowType";
        public const string str_outlineFilter = "sgameSettings_outline";
        public const string str_particleQuality = "sgameSettings_ParticleQuality";
        public const string str_renderQuality = "sgameSettings_RenderQuality";

        public static void ApplyActorShadowSettings(List<PoolObjHandle<ActorRoot>> actors)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = actors[i];
                ActorRoot root = handle.handle;
                if (((root != null) && (root.ShadowEffect != null)) && (root.gameObject != null))
                {
                    root.ShadowEffect.ApplyShadowSettings();
                }
            }
        }

        public static void ApplySettings_Music()
        {
            if (_EnableMusic)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UnMute_Music", null);
            }
            else
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("Mute_Music", null);
            }
        }

        public static void ApplySettings_Sound()
        {
            if (_EnableSound)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UnMute_SFX", null);
            }
            else
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("Mute_SFX", null);
            }
        }

        public static void ApplyShadowSettings()
        {
            if (Singleton<GameObjMgr>.HasInstance())
            {
                ApplyActorShadowSettings(Singleton<GameObjMgr>.instance.GameActors);
            }
        }

        public static void DecideDynamicParticleLOD()
        {
            if ((DeviceCheckSys.GetAvailMemoryMegaBytes() > 300) || (DeviceCheckSys.GetTotalMemoryMegaBytes() > 0x44c))
            {
                m_dynamicParticleLOD = true;
            }
            else
            {
                m_dynamicParticleLOD = false;
            }
        }

        public static void FightStart()
        {
            SendPlayerAttackTargetMode();
            SendPlayerCommonAttackMode();
        }

        public static float GetCurCameraSensitivity()
        {
            if (cameraMoveType == CameraMoveType.JoyStick)
            {
                return ((YaoGanSensitivity - 20000f) / 30000f);
            }
            if (cameraMoveType == CameraMoveType.Slide)
            {
                return ((HuaDongSensitivity - 40f) / 160f);
            }
            return -1f;
        }

        public static void GetLunPanSensitivity(out float spd, out float angularSpd)
        {
            if (LunPanSensitivity >= 1f)
            {
                spd = angularSpd = 10f;
            }
            else
            {
                spd = 0.2f + ((1f - LunPanSensitivity) * -0.18f);
                angularSpd = 0.2f + ((1f - LunPanSensitivity) * 1.8f);
            }
        }

        public static void Init()
        {
            DeviceLevel = SGameRenderQuality.Low;
            DeviceLevel = DetectRenderQuality.check_Android();
            if (PlayerPrefs.HasKey("sgameSettings_RenderQuality"))
            {
                RenderQuality = (SGameRenderQuality) Mathf.Clamp(PlayerPrefs.GetInt("sgameSettings_RenderQuality", 0), 0, 2);
            }
            else
            {
                RenderQuality = DeviceLevel;
            }
            if (PlayerPrefs.HasKey("sgameSettings_ParticleQuality"))
            {
                ParticleQuality = (SGameRenderQuality) Mathf.Clamp(PlayerPrefs.GetInt("sgameSettings_ParticleQuality", 0), 0, 2);
            }
            else
            {
                ParticleQuality = RenderQuality;
            }
            EnableSound = PlayerPrefs.GetInt("sgameSettings_muteSound", 1) == 1;
            EnableMusic = PlayerPrefs.GetInt("sgameSettings_muteMusic", 1) == 1;
            if (PlayerPrefs.HasKey("sgameSettings_EnableVoice"))
            {
                EnableVoice = PlayerPrefs.GetInt("sgameSettings_EnableVoice", 1) == 1;
            }
            else
            {
                EnableVoice = false;
            }
            EnableVibrate = PlayerPrefs.GetInt("GameSettingEnableVibrate", 1) == 1;
            EnableReplayKit = PlayerPrefs.GetInt("GameSettingEnableReplayKit", 0) == 1;
            EnableReplayKitAutoMode = PlayerPrefs.GetInt("GameSettingEnableReplayKitAutoMode", 0) == 1;
            EnableKingTimeMode = PlayerPrefs.GetInt("GameSettingEnableKingTime", 0) == 1;
            if (EnableKingTimeMode)
            {
                EnableRecorderMode = false;
            }
            else
            {
                EnableRecorderMode = PlayerPrefs.GetInt("GameSettingEnableRecorderMode", 0) == 1;
            }
            EnableOutline = PlayerPrefs.GetInt("sgameSettings_outline", 0) != 0;
            TheCastType = (CastType) PlayerPrefs.GetInt("GameSettings_CastType", 1);
            TheCommonAttackType = (CommonAttactType) PlayerPrefs.GetInt("GameSetting_CommonAttackType", 0);
            TheSelectType = (SelectEnemyType) PlayerPrefs.GetInt("GameSettings_SelectEnemyType", 1);
            s_lunpanSensitivity = !PlayerPrefs.HasKey("GameSettings_LunPanCastSensitivity") ? 1f : PlayerPrefs.GetFloat("GameSettings_LunPanCastSensitivity", 1f);
            TheSkillCancleType = (SkillCancleType) PlayerPrefs.GetInt("GameSettingSkillCancleType", 0);
            TheCameraMoveType = (CameraMoveType) PlayerPrefs.GetInt("GameSettingCameraMoveType", 0);
            YaoGanSensitivity = PlayerPrefs.GetFloat("GameSettingCameraYaoGanSensitivity", 25000f);
            HuaDongSensitivity = PlayerPrefs.GetFloat("GameSettingCameraHuaDongSensitivity", 100f);
            MusicEffectLevel = !PlayerPrefs.HasKey("GameSettingMusicEffectLevel") ? 100f : PlayerPrefs.GetFloat("GameSettingMusicEffectLevel", 100f);
            SoundEffectLevel = !PlayerPrefs.HasKey("GameSettingSoundEffectLevel") ? 100f : PlayerPrefs.GetFloat("GameSettingSoundEffectLevel", 100f);
            VoiceEffectLevel = !PlayerPrefs.HasKey("GameSettingVoiceEffectLevel") ? 100f : PlayerPrefs.GetFloat("GameSettingVoiceEffectLevel", 100f);
            if (DeviceLevel == SGameRenderQuality.Low)
            {
                cameraHeight = CameraHeightType.Low;
            }
            else
            {
                cameraHeight = CameraHeightType.Medium;
            }
            if (PlayerPrefs.HasKey("cameraHeight"))
            {
                CameraHeight = PlayerPrefs.GetInt("cameraHeight", 1);
            }
            JoyStickMoveType = PlayerPrefs.GetInt("joystickMoveType", 1);
            JoyStickShowType = PlayerPrefs.GetInt("joystickShowType", 0);
            FpsShowType = PlayerPrefs.GetInt("str_fpsShowType", 0);
            m_clickEnableInBattleInputChat = PlayerPrefs.GetInt("str_inBatInputShowType", 1);
        }

        private static void InitResolution()
        {
            if ((DefaultScreenWidth == 0) || (DefaultScreenHeight == 0))
            {
                int width = Screen.width;
                int height = Screen.height;
                DefaultScreenWidth = Mathf.Max(width, height);
                DefaultScreenHeight = Mathf.Min(width, height);
            }
        }

        public static void RefreshResolution()
        {
            InitResolution();
            if (PlayerPrefs.HasKey("GameSettingEnableHDMode"))
            {
                _EnableHDMode = PlayerPrefs.GetInt("GameSettingEnableHDMode", 0) > 0;
            }
            else
            {
                _EnableHDMode = !ShouldReduceResolution();
            }
            SetHDMode(_EnableHDMode);
        }

        public static void Save()
        {
            PlayerPrefs.SetInt("sgameSettings_muteSound", !EnableSound ? 0 : 1);
            PlayerPrefs.SetInt("sgameSettings_muteMusic", !EnableMusic ? 0 : 1);
            PlayerPrefs.SetInt("sgameSettings_RenderQuality", (int) RenderQuality);
            PlayerPrefs.SetInt("sgameSettings_ParticleQuality", (int) ParticleQuality);
            PlayerPrefs.SetInt("sgameSettings_outline", !EnableOutline ? 0 : 1);
            PlayerPrefs.SetInt("sgameSettings_EnableVoice", !EnableVoice ? 0 : 1);
            PlayerPrefs.SetInt("GameSettingEnableVibrate", !EnableVibrate ? 0 : 1);
            PlayerPrefs.SetInt("GameSettingEnableReplayKit", !EnableReplayKit ? 0 : 1);
            PlayerPrefs.SetInt("GameSettingEnableReplayKitAutoMode", !EnableReplayKitAutoMode ? 0 : 1);
            PlayerPrefs.SetInt("GameSettingEnableKingTime", !EnableKingTimeMode ? 0 : 1);
            PlayerPrefs.SetInt("GameSettingEnableRecorderMode", !EnableRecorderMode ? 0 : 1);
            PlayerPrefs.SetInt("GameSettings_CastType", (int) TheCastType);
            PlayerPrefs.SetInt("GameSetting_CommonAttackType", (int) TheCommonAttackType);
            PlayerPrefs.SetInt("GameSettings_SelectEnemyType", (int) TheSelectType);
            PlayerPrefs.SetFloat("GameSettings_LunPanCastSensitivity", LunPanSensitivity);
            PlayerPrefs.SetInt("cameraHeight", (int) cameraHeight);
            PlayerPrefs.SetInt("joystickMoveType", m_joystickMoveType);
            PlayerPrefs.SetInt("joystickShowType", m_joystickShowType);
            PlayerPrefs.SetInt("str_fpsShowType", m_fpsShowType);
            PlayerPrefs.SetInt("str_inBatInputShowType", m_clickEnableInBattleInputChat);
            PlayerPrefs.SetFloat("GameSettingMusicEffectLevel", MusicEffectLevel);
            PlayerPrefs.SetFloat("GameSettingSoundEffectLevel", SoundEffectLevel);
            PlayerPrefs.SetFloat("GameSettingVoiceEffectLevel", VoiceEffectLevel);
            PlayerPrefs.SetInt("GameSettingSkillCancleType", (int) TheSkillCancleType);
            PlayerPrefs.SetInt("GameSettingCameraMoveType", (int) TheCameraMoveType);
            PlayerPrefs.SetFloat("GameSettingCameraHuaDongSensitivity", HuaDongSensitivity);
            PlayerPrefs.SetFloat("GameSettingCameraYaoGanSensitivity", YaoGanSensitivity);
            bool flag = PlayerPrefs.GetInt("GameSettingEnableHDMode", 0) == 1;
            if (flag != _EnableHDMode)
            {
                SetHDMode(_EnableHDMode);
            }
            PlayerPrefs.SetInt("GameSettingEnableHDMode", !_EnableHDMode ? 0 : 1);
            PlayerPrefs.Save();
        }

        private static void SendPlayerAttackTargetMode()
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                FrameCommand<PlayAttackTargetModeCommand> command = FrameCommandFactory.CreateFrameCommand<PlayAttackTargetModeCommand>();
                command.cmdData.AttackTargetMode = (sbyte) _selectType;
                command.Send();
            }
        }

        private static void SendPlayerCommonAttackMode()
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                FrameCommand<PlayCommonAttackModeCommand> command = FrameCommandFactory.CreateFrameCommand<PlayCommonAttackModeCommand>();
                command.cmdData.CommonAttackMode = (byte) _normalAttackType;
                command.Send();
            }
        }

        public static void SetCurCameraSensitivity(float value)
        {
            if (cameraMoveType == CameraMoveType.JoyStick)
            {
                YaoGanSensitivity = 20000f + (30000f * Mathf.Clamp(value, 0f, 1f));
            }
            else if (cameraMoveType == CameraMoveType.Slide)
            {
                HuaDongSensitivity = 40f + (160f * Mathf.Clamp(value, 0f, 1f));
            }
        }

        public static void SetHDMode(bool enable)
        {
            InitResolution();
            int defaultScreenWidth = DefaultScreenWidth;
            int defaultScreenHeight = DefaultScreenHeight;
            if (!enable)
            {
                defaultScreenWidth = 0x500;
                defaultScreenHeight = (defaultScreenWidth * DefaultScreenHeight) / DefaultScreenWidth;
            }
            if ((defaultScreenWidth != Screen.width) || (defaultScreenHeight != Screen.height))
            {
                Screen.SetResolution(defaultScreenWidth, defaultScreenHeight, true);
            }
        }

        public static bool ShouldReduceResolution()
        {
            int num = (DefaultScreenWidth <= DefaultScreenHeight) ? DefaultScreenHeight : DefaultScreenWidth;
            int num2 = (DefaultScreenWidth <= DefaultScreenHeight) ? DefaultScreenWidth : DefaultScreenHeight;
            return ((num > 0x500) || (num2 > 720));
        }

        public static bool SupportHDMode()
        {
            int num = (DefaultScreenWidth <= DefaultScreenHeight) ? DefaultScreenHeight : DefaultScreenWidth;
            int num2 = (DefaultScreenWidth <= DefaultScreenHeight) ? DefaultScreenWidth : DefaultScreenHeight;
            return ((num >= 0x500) || (num2 >= 720));
        }

        public static bool supportOutline()
        {
            int num = (Screen.width <= Screen.height) ? Screen.height : Screen.width;
            int num2 = (Screen.width <= Screen.height) ? Screen.width : Screen.height;
            return (((num >= 960) && (num2 >= 540)) && (DeviceLevel != SGameRenderQuality.Low));
        }

        public static bool AllowOutlineFilter
        {
            get
            {
                if (!EnableOutline)
                {
                    return false;
                }
                return supportOutline();
            }
        }

        public static bool AllowRadialBlur
        {
            get
            {
                return (DeviceLevel != SGameRenderQuality.Low);
            }
        }

        public static int CameraHeight
        {
            get
            {
                return (int) cameraHeight;
            }
            set
            {
                cameraHeight = (CameraHeightType) Mathf.Clamp(value, 0, 1);
                Singleton<GameEventSys>.instance.SendEvent(GameEventDef.Event_CameraHeightChange);
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
                if ((theMinimapSys != null) && (theMinimapSys.m_miniMapCameraFrame != null))
                {
                    theMinimapSys.m_miniMapCameraFrame.SetFrameSize(cameraHeight);
                }
            }
        }

        public static float CameraHeightRateValue
        {
            get
            {
                if ((cameraHeight != CameraHeightType.Low) && (cameraHeight == CameraHeightType.Medium))
                {
                    return 1.2f;
                }
                return 1f;
            }
        }

        public static bool DynamicParticleLOD
        {
            get
            {
                return m_dynamicParticleLOD;
            }
        }

        public static bool EnableHDMode
        {
            get
            {
                return _EnableHDMode;
            }
            set
            {
                if (_EnableHDMode != value)
                {
                    _EnableHDMode = value;
                }
            }
        }

        public static bool EnableKingTimeMode
        {
            get
            {
                return _EnableKingTimeMode;
            }
            set
            {
                _EnableKingTimeMode = value;
            }
        }

        public static bool EnableMusic
        {
            get
            {
                return _EnableMusic;
            }
            set
            {
                _EnableMusic = value;
                ApplySettings_Music();
            }
        }

        public static bool EnableOutline
        {
            get
            {
                return _enableOutline;
            }
            set
            {
                if (_enableOutline != value)
                {
                    if ((Singleton<GameStateCtrl>.HasInstance() && Singleton<GameStateCtrl>.GetInstance().isBattleState) && supportOutline())
                    {
                        if (value)
                        {
                            OutlineFilter.EnableOutlineFilter();
                        }
                        else
                        {
                            OutlineFilter.DisableOutlineFilter();
                        }
                    }
                    _enableOutline = value;
                }
            }
        }

        public static bool EnableRecorderMode
        {
            get
            {
                return _EnableRecorderMode;
            }
            set
            {
                _EnableRecorderMode = value;
            }
        }

        public static bool enableReplay
        {
            get
            {
                return _EnableReplay;
            }
            set
            {
                if (_EnableReplay != value)
                {
                    _EnableReplay = value;
                }
            }
        }

        public static bool EnableReplayKit
        {
            get
            {
                return _EnableReplayKit;
            }
            set
            {
                _EnableReplayKit = value;
            }
        }

        public static bool EnableReplayKitAutoMode
        {
            get
            {
                return _EnableReplayKitAutoMode;
            }
            set
            {
                _EnableReplayKitAutoMode = value;
            }
        }

        public static bool EnableSound
        {
            get
            {
                return _EnableSound;
            }
            set
            {
                _EnableSound = value;
                ApplySettings_Sound();
            }
        }

        public static bool EnableVibrate
        {
            get
            {
                return m_EnableVibrate;
            }
            set
            {
                m_EnableVibrate = value;
            }
        }

        public static bool EnableVoice
        {
            get
            {
                return m_EnableVoice;
            }
            set
            {
                m_EnableVoice = value;
                MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting = m_EnableVoice;
            }
        }

        public static int FpsShowType
        {
            get
            {
                return m_fpsShowType;
            }
            set
            {
                m_fpsShowType = value;
                if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.SetFpsShowType(m_fpsShowType);
                }
            }
        }

        public static float HuaDongSensitivity
        {
            get
            {
                return _huaDongSensitivity;
            }
            set
            {
                if (value != _huaDongSensitivity)
                {
                    _huaDongSensitivity = value;
                    MonoSingleton<GlobalConfig>.GetInstance().PanelCameraMoveSpeed = _huaDongSensitivity;
                }
            }
        }

        public static int InBattleInputChatEnable
        {
            get
            {
                return m_clickEnableInBattleInputChat;
            }
            set
            {
                m_clickEnableInBattleInputChat = value;
                if (Singleton<InBattleMsgMgr>.instance.m_InputChat != null)
                {
                    Singleton<InBattleMsgMgr>.instance.m_InputChat.SetInputChatEnable(m_clickEnableInBattleInputChat);
                }
            }
        }

        public static bool IsHighQuality
        {
            get
            {
                return (RenderQuality == SGameRenderQuality.High);
            }
        }

        public static int JoyStickMoveType
        {
            get
            {
                return 1;
            }
            set
            {
                m_joystickMoveType = 1;
                if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.SetJoyStickMoveType(m_joystickMoveType);
                }
            }
        }

        public static int JoyStickShowType
        {
            get
            {
                return m_joystickShowType;
            }
            set
            {
                m_joystickShowType = value;
                if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.SetJoyStickShowType(m_joystickShowType);
                }
            }
        }

        public static float LunPanSensitivity
        {
            get
            {
                return s_lunpanSensitivity;
            }
            set
            {
                s_lunpanSensitivity = value;
                if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.ResetHostPlayerSkillIndicatorSensitivity();
                }
            }
        }

        public static int ModelLOD
        {
            get
            {
                return (int) RenderQuality;
            }
            set
            {
                RenderQuality = (SGameRenderQuality) Mathf.Clamp(value, 0, 2);
            }
        }

        public static float MusicEffectLevel
        {
            get
            {
                return m_MusicEffectLevel;
            }
            set
            {
                m_MusicEffectLevel = value;
                AkSoundEngine.SetRTPCValue("Set_Volume_Music", m_MusicEffectLevel);
            }
        }

        public static int ParticleLOD
        {
            get
            {
                return (int) ParticleQuality;
            }
            set
            {
                ParticleQuality = (SGameRenderQuality) Mathf.Clamp(value, 0, 2);
            }
        }

        public static SGameRenderQuality ShadowQuality
        {
            get
            {
                return (SGameRenderQuality) Mathf.Max((int) MaxShadowQuality, ModelLOD);
            }
            set
            {
                SGameRenderQuality shadowQuality = ShadowQuality;
                MaxShadowQuality = value;
                if (shadowQuality != MaxShadowQuality)
                {
                    ApplyShadowSettings();
                }
            }
        }

        public static float SoundEffectLevel
        {
            get
            {
                return m_SoundEffectLevel;
            }
            set
            {
                m_SoundEffectLevel = value;
                AkSoundEngine.SetRTPCValue("Set_Volume_SFX", m_SoundEffectLevel);
            }
        }

        public static CameraMoveType TheCameraMoveType
        {
            get
            {
                return cameraMoveType;
            }
            set
            {
                cameraMoveType = value;
                if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.SetCameraMoveMode(cameraMoveType);
                }
            }
        }

        public static CastType TheCastType
        {
            get
            {
                return _castType;
            }
            set
            {
                _castType = value;
                if (Singleton<GameInput>.instance != null)
                {
                    Singleton<GameInput>.instance.SetSmartUse(_castType != CastType.LunPanCast);
                }
            }
        }

        public static CommonAttactType TheCommonAttackType
        {
            get
            {
                return _normalAttackType;
            }
            set
            {
                _normalAttackType = value;
                SendPlayerCommonAttackMode();
            }
        }

        public static SelectEnemyType TheSelectType
        {
            get
            {
                return _selectType;
            }
            set
            {
                _selectType = value;
                SendPlayerAttackTargetMode();
            }
        }

        public static SkillCancleType TheSkillCancleType
        {
            get
            {
                return _skillCancleTyoe;
            }
            set
            {
                _skillCancleTyoe = value;
            }
        }

        public static float VoiceEffectLevel
        {
            get
            {
                return m_VoiceEffectLevel;
            }
            set
            {
                m_VoiceEffectLevel = value;
                MonoSingleton<VoiceSys>.GetInstance().VoiceLevel = m_VoiceEffectLevel;
            }
        }

        public static float YaoGanSensitivity
        {
            get
            {
                return _yaoGanSensitivity;
            }
            set
            {
                if (value != _yaoGanSensitivity)
                {
                    _yaoGanSensitivity = value;
                    MonoSingleton<GlobalConfig>.GetInstance().CameraMoveSpeed = _yaoGanSensitivity;
                }
            }
        }
    }
}

