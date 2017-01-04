using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkSoundEngine
{
    public const int AK_BANK_PLATFORM_DATA_ALIGNMENT = 0x10;
    public const int AK_BUFFER_ALIGNMENT = 0x10;
    public const int AK_COMM_DEFAULT_DISCOVERY_PORT = 0x5dd8;
    public const uint AK_DEFAULT_BANK_IO_PRIORITY = 50;
    public const double AK_DEFAULT_BANK_THROUGHPUT = 1048.576;
    public const int AK_DEFAULT_POOL_ID = -1;
    public const uint AK_DEFAULT_PRIORITY = 50;
    public const uint AK_DEFAULT_SWITCH_STATE = 0;
    public const uint AK_FALLBACK_ARGUMENTVALUE_ID = 0;
    public const int AK_IDX_SETUP_0_LFE = 0;
    public const int AK_IDX_SETUP_1_CENTER = 0;
    public const int AK_IDX_SETUP_1_LFE = 1;
    public const int AK_IDX_SETUP_2_LEFT = 0;
    public const int AK_IDX_SETUP_2_LFE = 2;
    public const int AK_IDX_SETUP_2_RIGHT = 1;
    public const int AK_IDX_SETUP_3_CENTER = 2;
    public const int AK_IDX_SETUP_3_LEFT = 0;
    public const int AK_IDX_SETUP_3_LFE = 3;
    public const int AK_IDX_SETUP_3_RIGHT = 1;
    public const int AK_IDX_SETUP_4_FRONTLEFT = 0;
    public const int AK_IDX_SETUP_4_FRONTRIGHT = 1;
    public const int AK_IDX_SETUP_4_LFE = 4;
    public const int AK_IDX_SETUP_4_REARLEFT = 2;
    public const int AK_IDX_SETUP_4_REARRIGHT = 3;
    public const int AK_IDX_SETUP_5_CENTER = 2;
    public const int AK_IDX_SETUP_5_FRONTLEFT = 0;
    public const int AK_IDX_SETUP_5_FRONTRIGHT = 1;
    public const int AK_IDX_SETUP_5_LFE = 5;
    public const int AK_IDX_SETUP_5_REARLEFT = 3;
    public const int AK_IDX_SETUP_5_REARRIGHT = 4;
    public const int AK_IDX_SETUP_CENTER = 2;
    public const int AK_IDX_SETUP_FRONT_LEFT = 0;
    public const int AK_IDX_SETUP_FRONT_RIGHT = 1;
    public const int AK_IDX_SETUP_NOCENTER_BACK_LEFT = 2;
    public const int AK_IDX_SETUP_NOCENTER_BACK_RIGHT = 3;
    public const int AK_IDX_SETUP_NOCENTER_SIDE_LEFT = 4;
    public const int AK_IDX_SETUP_NOCENTER_SIDE_RIGHT = 5;
    public const int AK_IDX_SETUP_WITHCENTER_BACK_LEFT = 3;
    public const int AK_IDX_SETUP_WITHCENTER_BACK_RIGHT = 4;
    public const int AK_IDX_SETUP_WITHCENTER_SIDE_LEFT = 5;
    public const int AK_IDX_SETUP_WITHCENTER_SIDE_RIGHT = 6;
    public const uint AK_INVALID_BANK_ID = 0;
    public const uint AK_INVALID_DEVICE_ID = uint.MaxValue;
    public const uint AK_INVALID_ENV_ID = 0;
    public const uint AK_INVALID_FILE_ID = uint.MaxValue;
    public const uint AK_INVALID_GAME_OBJECT = uint.MaxValue;
    public const uint AK_INVALID_LISTENER_INDEX = uint.MaxValue;
    public const uint AK_INVALID_PLAYING_ID = 0;
    public const uint AK_INVALID_PLUGINID = uint.MaxValue;
    public const uint AK_INVALID_POOL_ID = uint.MaxValue;
    public const uint AK_INVALID_RTPC_ID = 0;
    public const uint AK_INVALID_UNIQUE_ID = 0;
    public const uint AK_LISTENERS_MASK_ALL = uint.MaxValue;
    public const int AK_MAX_AUX_PER_OBJ = 4;
    public const int AK_MAX_AUX_SUPPORTED = 8;
    public const int AK_MAX_BITS_METERING_FLAGS = 5;
    public const int AK_MAX_LANGUAGE_NAME_SIZE = 0x20;
    public const int AK_MAX_PATH = 260;
    public const uint AK_MAX_PRIORITY = 100;
    public const int AK_MIDI_CC_ALL_CONTROLLERS_OFF = 0x79;
    public const int AK_MIDI_CC_ALL_NOTES_OFF = 0x7b;
    public const int AK_MIDI_CC_ALL_SOUND_OFF = 120;
    public const int AK_MIDI_CC_BALANCE_COARSE = 8;
    public const int AK_MIDI_CC_BALANCE_FINE = 40;
    public const int AK_MIDI_CC_BANK_SELECT_COARSE = 0;
    public const int AK_MIDI_CC_BANK_SELECT_FINE = 0x20;
    public const int AK_MIDI_CC_BREATH_CTRL_COARSE = 2;
    public const int AK_MIDI_CC_BREATH_CTRL_FINE = 0x22;
    public const int AK_MIDI_CC_CELESTE_LEVEL = 0x5e;
    public const int AK_MIDI_CC_CHORUS_LEVEL = 0x5d;
    public const int AK_MIDI_CC_CTRL_14_COARSE = 14;
    public const int AK_MIDI_CC_CTRL_14_FINE = 0x2e;
    public const int AK_MIDI_CC_CTRL_15_COARSE = 15;
    public const int AK_MIDI_CC_CTRL_15_FINE = 0x2f;
    public const int AK_MIDI_CC_CTRL_20_COARSE = 20;
    public const int AK_MIDI_CC_CTRL_20_FINE = 0x34;
    public const int AK_MIDI_CC_CTRL_21_COARSE = 0x15;
    public const int AK_MIDI_CC_CTRL_21_FINE = 0x35;
    public const int AK_MIDI_CC_CTRL_22_COARSE = 0x16;
    public const int AK_MIDI_CC_CTRL_22_FINE = 0x36;
    public const int AK_MIDI_CC_CTRL_23_COARSE = 0x17;
    public const int AK_MIDI_CC_CTRL_23_FINE = 0x37;
    public const int AK_MIDI_CC_CTRL_24_COARSE = 0x18;
    public const int AK_MIDI_CC_CTRL_24_FINE = 0x38;
    public const int AK_MIDI_CC_CTRL_25_COARSE = 0x19;
    public const int AK_MIDI_CC_CTRL_25_FINE = 0x39;
    public const int AK_MIDI_CC_CTRL_26_COARSE = 0x1a;
    public const int AK_MIDI_CC_CTRL_26_FINE = 0x3a;
    public const int AK_MIDI_CC_CTRL_27_COARSE = 0x1b;
    public const int AK_MIDI_CC_CTRL_27_FINE = 0x3b;
    public const int AK_MIDI_CC_CTRL_28_COARSE = 0x1c;
    public const int AK_MIDI_CC_CTRL_28_FINE = 60;
    public const int AK_MIDI_CC_CTRL_29_COARSE = 0x1d;
    public const int AK_MIDI_CC_CTRL_29_FINE = 0x3d;
    public const int AK_MIDI_CC_CTRL_3_COARSE = 3;
    public const int AK_MIDI_CC_CTRL_3_FINE = 0x23;
    public const int AK_MIDI_CC_CTRL_30_COARSE = 30;
    public const int AK_MIDI_CC_CTRL_30_FINE = 0x3e;
    public const int AK_MIDI_CC_CTRL_31_COARSE = 0x1f;
    public const int AK_MIDI_CC_CTRL_31_FINE = 0x3f;
    public const int AK_MIDI_CC_CTRL_9_COARSE = 9;
    public const int AK_MIDI_CC_CTRL_9_FINE = 0x29;
    public const int AK_MIDI_CC_DATA_BUTTON_M1 = 0x61;
    public const int AK_MIDI_CC_DATA_BUTTON_P1 = 0x60;
    public const int AK_MIDI_CC_DATA_ENTRY_COARSE = 6;
    public const int AK_MIDI_CC_DATA_ENTRY_FINE = 0x26;
    public const int AK_MIDI_CC_EFFECT_CTRL_1_COARSE = 12;
    public const int AK_MIDI_CC_EFFECT_CTRL_1_FINE = 0x2c;
    public const int AK_MIDI_CC_EFFECT_CTRL_2_COARSE = 13;
    public const int AK_MIDI_CC_EFFECT_CTRL_2_FINE = 0x2d;
    public const int AK_MIDI_CC_EXPRESSION_COARSE = 11;
    public const int AK_MIDI_CC_EXPRESSION_FINE = 0x2b;
    public const int AK_MIDI_CC_FOOT_PEDAL_COARSE = 4;
    public const int AK_MIDI_CC_FOOT_PEDAL_FINE = 0x24;
    public const int AK_MIDI_CC_GEN_SLIDER_1 = 0x10;
    public const int AK_MIDI_CC_GEN_SLIDER_2 = 0x11;
    public const int AK_MIDI_CC_GEN_SLIDER_3 = 0x12;
    public const int AK_MIDI_CC_GEN_SLIDER_4 = 0x13;
    public const int AK_MIDI_CC_GENERAL_BUTTON_1 = 80;
    public const int AK_MIDI_CC_GENERAL_BUTTON_2 = 0x51;
    public const int AK_MIDI_CC_GENERAL_BUTTON_3 = 0x52;
    public const int AK_MIDI_CC_GENERAL_BUTTON_4 = 0x53;
    public const int AK_MIDI_CC_HOLD_PEDAL = 0x40;
    public const int AK_MIDI_CC_HOLD_PEDAL_2 = 0x45;
    public const int AK_MIDI_CC_LEGATO_PEDAL = 0x44;
    public const int AK_MIDI_CC_LOCAL_KEYBOARD = 0x7a;
    public const int AK_MIDI_CC_MOD_WHEEL_COARSE = 1;
    public const int AK_MIDI_CC_MOD_WHEEL_FINE = 0x21;
    public const int AK_MIDI_CC_NON_REGISTER_COARSE = 0x62;
    public const int AK_MIDI_CC_NON_REGISTER_FINE = 0x63;
    public const int AK_MIDI_CC_OMNI_MODE_OFF = 0x7c;
    public const int AK_MIDI_CC_OMNI_MODE_ON = 0x7d;
    public const int AK_MIDI_CC_OMNI_MONOPHONIC_ON = 0x7e;
    public const int AK_MIDI_CC_OMNI_POLYPHONIC_ON = 0x7f;
    public const int AK_MIDI_CC_PAN_POSITION_COARSE = 10;
    public const int AK_MIDI_CC_PAN_POSITION_FINE = 0x2a;
    public const int AK_MIDI_CC_PHASER_LEVEL = 0x5f;
    public const int AK_MIDI_CC_PORTAMENTO_COARSE = 5;
    public const int AK_MIDI_CC_PORTAMENTO_FINE = 0x25;
    public const int AK_MIDI_CC_PORTAMENTO_ON_OFF = 0x41;
    public const int AK_MIDI_CC_REVERB_LEVEL = 0x5b;
    public const int AK_MIDI_CC_SOFT_PEDAL = 0x43;
    public const int AK_MIDI_CC_SOUND_ATTACK_TIME = 0x49;
    public const int AK_MIDI_CC_SOUND_BRIGHTNESS = 0x4a;
    public const int AK_MIDI_CC_SOUND_CTRL_10 = 0x4f;
    public const int AK_MIDI_CC_SOUND_CTRL_6 = 0x4b;
    public const int AK_MIDI_CC_SOUND_CTRL_7 = 0x4c;
    public const int AK_MIDI_CC_SOUND_CTRL_8 = 0x4d;
    public const int AK_MIDI_CC_SOUND_CTRL_9 = 0x4e;
    public const int AK_MIDI_CC_SOUND_RELEASE_TIME = 0x48;
    public const int AK_MIDI_CC_SOUND_TIMBRE = 0x47;
    public const int AK_MIDI_CC_SOUND_VARIATION = 70;
    public const int AK_MIDI_CC_SUSTENUTO_PEDAL = 0x42;
    public const int AK_MIDI_CC_TREMOLO_LEVEL = 0x5c;
    public const int AK_MIDI_CC_VOLUME_COARSE = 7;
    public const int AK_MIDI_CC_VOLUME_FINE = 0x27;
    public const int AK_MIDI_EVENT_TYPE_CHANNEL_AFTERTOUCH = 0xd0;
    public const int AK_MIDI_EVENT_TYPE_CONTROLLER = 0xb0;
    public const int AK_MIDI_EVENT_TYPE_ESCAPE = 0xf7;
    public const int AK_MIDI_EVENT_TYPE_INVALID = 0;
    public const int AK_MIDI_EVENT_TYPE_META = 0xff;
    public const int AK_MIDI_EVENT_TYPE_NOTE_AFTERTOUCH = 160;
    public const int AK_MIDI_EVENT_TYPE_NOTE_OFF = 0x80;
    public const int AK_MIDI_EVENT_TYPE_NOTE_ON = 0x90;
    public const int AK_MIDI_EVENT_TYPE_PITCH_BEND = 0xe0;
    public const int AK_MIDI_EVENT_TYPE_PROGRAM_CHANGE = 0xc0;
    public const int AK_MIDI_EVENT_TYPE_SYSEX = 240;
    public const uint AK_MIN_PRIORITY = 0;
    public const int AK_NUM_LISTENERS = 8;
    public const int AK_OS_STRUCT_ALIGN = 4;
    public const int AK_SIMD_ALIGNMENT = 0x10;
    public const int AK_SPEAKER_BACK_CENTER = 0x100;
    public const int AK_SPEAKER_BACK_LEFT = 0x10;
    public const int AK_SPEAKER_BACK_RIGHT = 0x20;
    public const int AK_SPEAKER_FRONT_CENTER = 4;
    public const int AK_SPEAKER_FRONT_LEFT = 1;
    public const int AK_SPEAKER_FRONT_RIGHT = 2;
    public const int AK_SPEAKER_HEIGHT_BACK_CENTER = 0x10000;
    public const int AK_SPEAKER_HEIGHT_BACK_LEFT = 0x8000;
    public const int AK_SPEAKER_HEIGHT_BACK_RIGHT = 0x20000;
    public const int AK_SPEAKER_HEIGHT_FRONT_CENTER = 0x2000;
    public const int AK_SPEAKER_HEIGHT_FRONT_LEFT = 0x1000;
    public const int AK_SPEAKER_HEIGHT_FRONT_RIGHT = 0x4000;
    public const int AK_SPEAKER_LOW_FREQUENCY = 8;
    public const int AK_SPEAKER_SETUP_0_1 = 8;
    public const int AK_SPEAKER_SETUP_0POINT1 = 8;
    public const int AK_SPEAKER_SETUP_1_0 = 1;
    public const int AK_SPEAKER_SETUP_1_0_CENTER = 4;
    public const int AK_SPEAKER_SETUP_1_1 = 9;
    public const int AK_SPEAKER_SETUP_1_1_CENTER = 12;
    public const int AK_SPEAKER_SETUP_1POINT1 = 12;
    public const int AK_SPEAKER_SETUP_2_0 = 3;
    public const int AK_SPEAKER_SETUP_2_1 = 11;
    public const int AK_SPEAKER_SETUP_2POINT1 = 11;
    public const int AK_SPEAKER_SETUP_3_0 = 7;
    public const int AK_SPEAKER_SETUP_3_1 = 15;
    public const int AK_SPEAKER_SETUP_3POINT1 = 15;
    public const int AK_SPEAKER_SETUP_3STEREO = 7;
    public const int AK_SPEAKER_SETUP_4 = 0x603;
    public const int AK_SPEAKER_SETUP_4_0 = 0x603;
    public const int AK_SPEAKER_SETUP_4_1 = 0x60b;
    public const int AK_SPEAKER_SETUP_4POINT1 = 0x60b;
    public const int AK_SPEAKER_SETUP_5 = 0x607;
    public const int AK_SPEAKER_SETUP_5_0 = 0x607;
    public const int AK_SPEAKER_SETUP_5_1 = 0x60f;
    public const int AK_SPEAKER_SETUP_5POINT1 = 0x60f;
    public const int AK_SPEAKER_SETUP_6 = 0x633;
    public const int AK_SPEAKER_SETUP_6_0 = 0x633;
    public const int AK_SPEAKER_SETUP_6_1 = 0x63b;
    public const int AK_SPEAKER_SETUP_6POINT1 = 0x63b;
    public const int AK_SPEAKER_SETUP_7 = 0x637;
    public const int AK_SPEAKER_SETUP_7_0 = 0x637;
    public const int AK_SPEAKER_SETUP_7_1 = 0x63f;
    public const int AK_SPEAKER_SETUP_7POINT1 = 0x63f;
    public const int AK_SPEAKER_SETUP_ALL_SPEAKERS = 0x3ff3f;
    public const int AK_SPEAKER_SETUP_AURO_10 = 0x2de07;
    public const int AK_SPEAKER_SETUP_AURO_10POINT1 = 0x2de0f;
    public const int AK_SPEAKER_SETUP_AURO_11 = 0x2fe07;
    public const int AK_SPEAKER_SETUP_AURO_11_740 = 0x2d637;
    public const int AK_SPEAKER_SETUP_AURO_11POINT1 = 0x2fe0f;
    public const int AK_SPEAKER_SETUP_AURO_11POINT1_740 = 0x2d63f;
    public const int AK_SPEAKER_SETUP_AURO_13_751 = 0x2fe37;
    public const int AK_SPEAKER_SETUP_AURO_13POINT1_751 = 0x2fe3f;
    public const int AK_SPEAKER_SETUP_AURO_222 = 0x5603;
    public const int AK_SPEAKER_SETUP_AURO_8 = 0x2d603;
    public const int AK_SPEAKER_SETUP_AURO_9 = 0x2d607;
    public const int AK_SPEAKER_SETUP_AURO_9POINT1 = 0x2d60f;
    public const int AK_SPEAKER_SETUP_DOLBY_5_0_2 = 0x5607;
    public const int AK_SPEAKER_SETUP_DOLBY_5_1_2 = 0x560f;
    public const int AK_SPEAKER_SETUP_DOLBY_7_0_2 = 0x5637;
    public const int AK_SPEAKER_SETUP_DOLBY_7_1_2 = 0x563f;
    public const int AK_SPEAKER_SETUP_DPL2 = 0x603;
    public const int AK_SPEAKER_SETUP_FRONT = 7;
    public const int AK_SPEAKER_SETUP_HEIGHT_4 = 0x2d000;
    public const int AK_SPEAKER_SETUP_HEIGHT_5 = 0x2f000;
    public const int AK_SPEAKER_SETUP_HEIGHT_ALL = 0x3f000;
    public const int AK_SPEAKER_SETUP_MONO = 4;
    public const int AK_SPEAKER_SETUP_STEREO = 3;
    public const int AK_SPEAKER_SETUP_SURROUND = 0x103;
    public const int AK_SPEAKER_SIDE_LEFT = 0x200;
    public const int AK_SPEAKER_SIDE_RIGHT = 0x400;
    public const int AK_SPEAKER_TOP = 0x800;
    public const int AK_STANDARD_MAX_NUM_CHANNELS = 6;
    public const int AK_VOICE_MAX_NUM_CHANNELS = 6;
    public const int AK_WAVE_FORMAT_AAC = 0xaac0;
    public const int AK_WAVE_FORMAT_AT9 = 0xfffc;
    public const int AK_WAVE_FORMAT_VAG = 0xfffb;
    public const int AK_WAVE_FORMAT_VORBIS = 0xffff;
    public const int AKCODECID_AAC = 10;
    public const int AKCODECID_ADPCM = 2;
    public const int AKCODECID_ANALYSISFILE = 15;
    public const int AKCODECID_ATRAC9 = 12;
    public const int AKCODECID_BANK = 0;
    public const int AKCODECID_EXTERNAL_SOURCE = 8;
    public const int AKCODECID_FILE_PACKAGE = 11;
    public const int AKCODECID_MIDI = 0x10;
    public const int AKCODECID_PCM = 1;
    public const int AKCODECID_PCMEX = 7;
    public const int AKCODECID_PROFILERCAPTURE = 14;
    public const int AKCODECID_VAG = 13;
    public const int AKCODECID_VORBIS = 4;
    public const int AKCODECID_WIIADPCM = 5;
    public const int AKCODECID_XMA = 3;
    public const int AKCODECID_XWMA = 9;
    public const uint AKCOMPANYID_AUDIOKINETIC = 0;
    public const int AKCOMPANYID_AUDIOKINETIC_EXTERNAL = 1;
    public const int AKCOMPANYID_AUROTECHNOLOGIES = 0x107;
    public const int AKCOMPANYID_CRANKCASEAUDIO = 0x105;
    public const int AKCOMPANYID_DOLBY = 0x108;
    public const int AKCOMPANYID_GENAUDIO = 260;
    public const int AKCOMPANYID_IOSONO = 0x106;
    public const int AKCOMPANYID_IZOTOPE = 0x103;
    public const int AKCOMPANYID_MCDSP = 0x100;
    public const int AKCOMPANYID_PHONETICARTS = 0x102;
    public const int AKCOMPANYID_WAVEARTS = 0x101;
    public const int AKCURVEINTERPOLATION_NUM_STORAGE_BIT = 5;
    public const uint AKMOTIONDEVICEID_RUMBLE = 0x196;
    public const int NULL = 0;
    public const int PANNER_NUM_STORAGE_BITS = 2;
    public const int POSSOURCE_NUM_STORAGE_BITS = 2;

    public static AKRESULT AddBasePath(string in_pszBasePath)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AddBasePath(in_pszBasePath);
    }

    public static AKRESULT AddOutputCaptureMarker(string in_MarkerText)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AddOutputCaptureMarker(in_MarkerText);
    }

    public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_1(in_iPlayerID, in_iCompanyID, in_iDeviceID);
    }

    public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID, IntPtr in_pDevice)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_0(in_iPlayerID, in_iCompanyID, in_iDeviceID, in_pDevice);
    }

    public static AKRESULT AddSecondaryOutput(uint in_iOutputID, AkAudioOutputType in_iDeviceType, uint in_uListenerMask)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AddSecondaryOutput(in_iOutputID, (int) in_iDeviceType, in_uListenerMask);
    }

    public static void AK_SPEAKER_SETUP_CONVERT_TO_SUPPORTED(ref uint io_uChannelMask)
    {
        AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_CONVERT_TO_SUPPORTED(ref io_uChannelMask);
    }

    public static void AK_SPEAKER_SETUP_FIX_LEFT_TO_CENTER(ref uint io_uChannelMask)
    {
        AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_FIX_LEFT_TO_CENTER(ref io_uChannelMask);
    }

    public static void AK_SPEAKER_SETUP_FIX_REAR_TO_SIDE(ref uint io_uChannelMask)
    {
        AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_FIX_REAR_TO_SIDE(ref io_uChannelMask);
    }

    public static uint BackToSideChannels(uint in_uChannelMask)
    {
        return AkSoundEnginePINVOKE.CSharp_BackToSideChannels(in_uChannelMask);
    }

    public static void CancelBankCallbackCookie(object in_pCookie)
    {
        foreach (int num in AkCallbackManager.RemoveBankCallback(in_pCookie))
        {
            AkSoundEnginePINVOKE.CSharp_CancelBankCallbackCookie((IntPtr) num);
        }
    }

    public static void CancelEventCallback(uint in_playingID)
    {
        AkCallbackManager.RemoveEventCallback(in_playingID);
        AkSoundEnginePINVOKE.CSharp_CancelEventCallback(in_playingID);
    }

    public static void CancelEventCallbackCookie(object in_pCookie)
    {
        foreach (int num in AkCallbackManager.RemoveEventCallbackCookie(in_pCookie))
        {
            AkSoundEnginePINVOKE.CSharp_CancelEventCallbackCookie((IntPtr) num);
        }
    }

    public static uint ChannelIndexToDisplayIndex(AkChannelOrdering in_eOrdering, uint in_uChannelMask, uint in_uChannelIdx)
    {
        return AkSoundEnginePINVOKE.CSharp_ChannelIndexToDisplayIndex((int) in_eOrdering, in_uChannelMask, in_uChannelIdx);
    }

    public static uint ChannelMaskFromNumChannels(uint in_uNumChannels)
    {
        return AkSoundEnginePINVOKE.CSharp_ChannelMaskFromNumChannels(in_uNumChannels);
    }

    public static uint ChannelMaskToNumChannels(uint in_uChannelMask)
    {
        return AkSoundEnginePINVOKE.CSharp_ChannelMaskToNumChannels(in_uChannelMask);
    }

    public static AKRESULT ClearBanks()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ClearBanks();
    }

    public static AKRESULT ClearPreparedEvents()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ClearPreparedEvents();
    }

    public static AKRESULT DynamicSequenceBreak(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceBreak(in_playingID);
    }

    public static AKRESULT DynamicSequenceClose(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceClose(in_playingID);
    }

    public static Playlist DynamicSequenceLockPlaylist(uint in_playingID)
    {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_DynamicSequenceLockPlaylist(in_playingID);
        return (!(cPtr == IntPtr.Zero) ? new Playlist(cPtr, false) : null);
    }

    public static uint DynamicSequenceOpen(GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_3(instanceID);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_2(instanceID, in_uFlags);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_1(instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, DynamicSequenceType in_eDynamicSequenceType)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_0(instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), (int) in_eDynamicSequenceType);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static AKRESULT DynamicSequencePause(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_2(in_playingID);
    }

    public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_1(in_playingID, in_uTransitionDuration);
    }

    public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_0(in_playingID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT DynamicSequencePlay(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_2(in_playingID);
    }

    public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_1(in_playingID, in_uTransitionDuration);
    }

    public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_0(in_playingID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT DynamicSequenceResume(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_2(in_playingID);
    }

    public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_1(in_playingID, in_uTransitionDuration);
    }

    public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_0(in_playingID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT DynamicSequenceStop(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_2(in_playingID);
    }

    public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_1(in_playingID, in_uTransitionDuration);
    }

    public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_0(in_playingID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT DynamicSequenceUnlockPlaylist(uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_DynamicSequenceUnlockPlaylist(in_playingID);
    }

    public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_9(in_pszEventName, (int) in_ActionType);
    }

    public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_4(in_eventID, (int) in_ActionType);
    }

    public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_8(in_pszEventName, (int) in_ActionType, instanceID);
    }

    public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_3(in_eventID, (int) in_ActionType, instanceID);
    }

    public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_7(in_pszEventName, (int) in_ActionType, instanceID, in_uTransitionDuration);
    }

    public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_2(in_eventID, (int) in_ActionType, instanceID, in_uTransitionDuration);
    }

    public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_6(in_pszEventName, (int) in_ActionType, instanceID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_1(in_eventID, (int) in_ActionType, instanceID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_5(in_pszEventName, (int) in_ActionType, instanceID, in_uTransitionDuration, (int) in_eFadeCurve, in_PlayingID);
    }

    public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_0(in_eventID, (int) in_ActionType, instanceID, in_uTransitionDuration, (int) in_eFadeCurve, in_PlayingID);
    }

    public static AKRESULT GetActiveListeners(GameObject in_GameObjectID, out uint out_ruListenerMask)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetActiveListeners(instanceID, out out_ruListenerMask);
    }

    public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out int out_iValue)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_0(in_ObjectID, in_uPropID, out out_iValue);
    }

    public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out float out_fValue)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_1(in_ObjectID, in_uPropID, out out_fValue);
    }

    public static void GetDefaultDeviceSettings(AkDeviceSettings out_settings)
    {
        AkSoundEnginePINVOKE.CSharp_GetDefaultDeviceSettings(AkDeviceSettings.getCPtr(out_settings));
    }

    public static void GetDefaultInitSettings(AkInitSettings out_settings)
    {
        AkSoundEnginePINVOKE.CSharp_GetDefaultInitSettings(AkInitSettings.getCPtr(out_settings));
    }

    public static void GetDefaultMusicSettings(AkMusicSettings out_settings)
    {
        AkSoundEnginePINVOKE.CSharp_GetDefaultMusicSettings(AkMusicSettings.getCPtr(out_settings));
    }

    public static void GetDefaultPlatformInitSettings(AkPlatformInitSettings out_settings)
    {
        AkSoundEnginePINVOKE.CSharp_GetDefaultPlatformInitSettings(AkPlatformInitSettings.getCPtr(out_settings));
    }

    public static void GetDefaultStreamSettings(AkStreamMgrSettings out_settings)
    {
        AkSoundEnginePINVOKE.CSharp_GetDefaultStreamSettings(AkStreamMgrSettings.getCPtr(out_settings));
    }

    public static uint GetEventIDFromPlayingID(uint in_playingID)
    {
        return AkSoundEnginePINVOKE.CSharp_GetEventIDFromPlayingID(in_playingID);
    }

    public static AKRESULT GetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray out_paAuxSendValues, ref uint io_ruNumSendValues)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetGameObjectAuxSendValues(instanceID, out_paAuxSendValues.m_Buffer, ref io_ruNumSendValues);
    }

    public static AKRESULT GetGameObjectDryLevelValue(GameObject in_gameObjectID, out float out_rfControlValue)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetGameObjectDryLevelValue(instanceID, out out_rfControlValue);
    }

    public static uint GetGameObjectFromPlayingID(uint in_playingID)
    {
        return AkSoundEnginePINVOKE.CSharp_GetGameObjectFromPlayingID(in_playingID);
    }

    public static uint GetIDFromString(string in_pszString)
    {
        return AkSoundEnginePINVOKE.CSharp_GetIDFromString(in_pszString);
    }

    public static bool GetIsGameObjectActive(GameObject in_GameObjId)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjId != null)
        {
            instanceID = (uint) in_GameObjId.GetInstanceID();
            if (in_GameObjId.activeInHierarchy)
            {
                if (in_GameObjId.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjId.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjId);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return AkSoundEnginePINVOKE.CSharp_GetIsGameObjectActive(instanceID);
    }

    public static AKRESULT GetListenerPosition(uint in_uIndex, AkListenerPosition out_rPosition)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetListenerPosition(in_uIndex, AkListenerPosition.getCPtr(out_rPosition));
    }

    public static uint GetMajorMinorVersion()
    {
        return AkSoundEnginePINVOKE.CSharp_GetMajorMinorVersion();
    }

    public static float GetMaxRadius(GameObject in_GameObjId)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjId != null)
        {
            instanceID = (uint) in_GameObjId.GetInstanceID();
            if (in_GameObjId.activeInHierarchy)
            {
                if (in_GameObjId.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjId.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjId);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return AkSoundEnginePINVOKE.CSharp_GetMaxRadius(instanceID);
    }

    public static uint GetNumNonZeroBits(uint in_uWord)
    {
        return AkSoundEnginePINVOKE.CSharp_GetNumNonZeroBits(in_uWord);
    }

    public static AKRESULT GetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, out float out_rfObstructionLevel, out float out_rfOcclusionLevel)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_ObjectID != null)
        {
            instanceID = (uint) in_ObjectID.GetInstanceID();
            if (in_ObjectID.activeInHierarchy)
            {
                if (in_ObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_ObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_ObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetObjectObstructionAndOcclusion(instanceID, in_uListener, out out_rfObstructionLevel, out out_rfOcclusionLevel);
    }

    public static AKRESULT GetPanningRule(out int out_ePanningRule)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_2(out out_ePanningRule);
    }

    public static AKRESULT GetPanningRule(out int out_ePanningRule, AkAudioOutputType in_eSinkType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_1(out out_ePanningRule, (int) in_eSinkType);
    }

    public static AKRESULT GetPanningRule(out int out_ePanningRule, AkAudioOutputType in_eSinkType, uint in_iOutputID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_0(out out_ePanningRule, (int) in_eSinkType, in_iOutputID);
    }

    public static AKRESULT GetPlayingIDsFromGameObject(GameObject in_GameObjId, ref uint io_ruNumIDs, uint[] out_aPlayingIDs)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjId != null)
        {
            instanceID = (uint) in_GameObjId.GetInstanceID();
            if (in_GameObjId.activeInHierarchy)
            {
                if (in_GameObjId.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjId.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjId);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPlayingIDsFromGameObject(instanceID, ref io_ruNumIDs, out_aPlayingIDs);
    }

    public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_1(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo));
    }

    public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo, bool in_bExtrapolate)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_0(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo), in_bExtrapolate);
    }

    public static AKRESULT GetPosition(GameObject in_GameObjectID, AkSoundPosition out_rPosition)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPosition(instanceID, AkSoundPosition.getCPtr(out_rPosition));
    }

    public static AKRESULT GetPositioningInfo(uint in_ObjectID, AkPositioningInfo out_rPositioningInfo)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetPositioningInfo(in_ObjectID, AkPositioningInfo.getCPtr(out_rPositioningInfo));
    }

    public static AKRESULT GetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_1(in_pszRtpcName, instanceID, out out_rValue, ref io_rValueType);
    }

    public static AKRESULT GetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_0(in_rtpcID, instanceID, out out_rValue, ref io_rValueType);
    }

    public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_1(in_PlayingID, out out_puPosition);
    }

    public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition, bool in_bExtrapolate)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_0(in_PlayingID, out out_puPosition, in_bExtrapolate);
    }

    public static AKRESULT GetSourceStreamBuffering(uint in_PlayingID, out int out_buffering, out int out_bIsBuffering)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSourceStreamBuffering(in_PlayingID, out out_buffering, out out_bIsBuffering);
    }

    public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_2(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle);
    }

    public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle, AkAudioOutputType in_eSinkType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_1(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle, (int) in_eSinkType);
    }

    public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle, AkAudioOutputType in_eSinkType, uint in_iOutputID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_0(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle, (int) in_eSinkType, in_iOutputID);
    }

    public static AkChannelConfig GetSpeakerConfiguration()
    {
        return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_2(), true);
    }

    public static AkChannelConfig GetSpeakerConfiguration(AkAudioOutputType in_eSinkType)
    {
        return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_1((int) in_eSinkType), true);
    }

    public static AkChannelConfig GetSpeakerConfiguration(AkAudioOutputType in_eSinkType, uint in_iOutputID)
    {
        return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_0((int) in_eSinkType, in_iOutputID), true);
    }

    public static AKRESULT GetState(string in_pstrStateGroupName, out uint out_rState)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetState__SWIG_1(in_pstrStateGroupName, out out_rState);
    }

    public static AKRESULT GetState(uint in_stateGroup, out uint out_rState)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetState__SWIG_0(in_stateGroup, out out_rState);
    }

    public static uint GetSubminorBuildVersion()
    {
        return AkSoundEnginePINVOKE.CSharp_GetSubminorBuildVersion();
    }

    public static AKRESULT GetSwitch(string in_pstrSwitchGroupName, GameObject in_GameObj, out uint out_rSwitchState)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObj != null)
        {
            instanceID = (uint) in_GameObj.GetInstanceID();
            if (in_GameObj.activeInHierarchy)
            {
                if (in_GameObj.GetComponent<AkGameObj>() == null)
                {
                    in_GameObj.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObj);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_1(in_pstrSwitchGroupName, instanceID, out out_rSwitchState);
    }

    public static AKRESULT GetSwitch(uint in_switchGroup, GameObject in_gameObjectID, out uint out_rSwitchState)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_0(in_switchGroup, instanceID, out out_rSwitchState);
    }

    public static int GetTimeStamp()
    {
        return AkSoundEnginePINVOKE.CSharp_GetTimeStamp();
    }

    public static bool HasSideAndRearChannels(uint in_uChannelMask)
    {
        return AkSoundEnginePINVOKE.CSharp_HasSideAndRearChannels(in_uChannelMask);
    }

    public static bool HasStrictlyOnePairOfSurroundChannels(uint in_uChannelMask)
    {
        return AkSoundEnginePINVOKE.CSharp_HasStrictlyOnePairOfSurroundChannels(in_uChannelMask);
    }

    public static bool HasSurroundChannels(uint in_uChannelMask)
    {
        return AkSoundEnginePINVOKE.CSharp_HasSurroundChannels(in_uChannelMask);
    }

    public static AKRESULT Init(AkMemSettings in_pMemSettings, AkStreamMgrSettings in_pStmSettings, AkDeviceSettings in_pDefaultDeviceSettings, AkInitSettings in_pSettings, AkPlatformInitSettings in_pPlatformSettings, AkMusicSettings in_pMusicSettings)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Init(AkMemSettings.getCPtr(in_pMemSettings), AkStreamMgrSettings.getCPtr(in_pStmSettings), AkDeviceSettings.getCPtr(in_pDefaultDeviceSettings), AkInitSettings.getCPtr(in_pSettings), AkPlatformInitSettings.getCPtr(in_pPlatformSettings), AkMusicSettings.getCPtr(in_pMusicSettings));
    }

    public static bool IsInitialized()
    {
        return AkSoundEnginePINVOKE.CSharp_IsInitialized();
    }

    public static AKRESULT LoadBank(uint in_bankID, int in_memPoolId)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_1(in_bankID, in_memPoolId);
    }

    public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, out uint out_bankID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_2(in_pInMemoryBankPtr, in_uInMemoryBankSize, out out_bankID);
    }

    public static AKRESULT LoadBank(string in_pszString, int in_memPoolId, out uint out_bankID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_0(in_pszString, in_memPoolId, out out_bankID);
    }

    public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, int in_uPoolForBankMedia, out uint out_bankID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_3(in_pInMemoryBankPtr, in_uInMemoryBankSize, in_uPoolForBankMedia, out out_bankID);
    }

    public static AKRESULT LoadBank(uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_5(in_bankID, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_memPoolId);
    }

    public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, out uint out_bankID)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_6(in_pInMemoryBankPtr, in_uInMemoryBankSize, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), out out_bankID);
    }

    public static AKRESULT LoadBank(string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId, out uint out_bankID)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_4(in_pszString, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_memPoolId, out out_bankID);
    }

    public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_uPoolForBankMedia, out uint out_bankID)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_7(in_pInMemoryBankPtr, in_uInMemoryBankSize, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_uPoolForBankMedia, out out_bankID);
    }

    public static AKRESULT LoadFilePackage(string in_pszFilePackageName, out uint out_uPackageID, int in_memPoolID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_LoadFilePackage(in_pszFilePackageName, out out_uPackageID, in_memPoolID);
    }

    public static void MuteBackgroundMusic(bool in_bMute)
    {
        AkSoundEnginePINVOKE.CSharp_MuteBackgroundMusic(in_bMute);
    }

    public static AKRESULT PostCode(ErrorCode in_eError, ErrorLevel in_eErrorLevel)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PostCode((int) in_eError, (int) in_eErrorLevel);
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_11(in_pszEventName, instanceID);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_5(in_eventID, instanceID);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_10(in_pszEventName, instanceID, in_uFlags);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_4(in_eventID, instanceID, in_uFlags);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_9(in_pszEventName, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_3(in_eventID, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_8(in_pszEventName, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_2(in_eventID, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_7(in_pszEventName, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_1(in_eventID, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_6(in_pszEventName, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
        uint num2 = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_0(in_eventID, instanceID, in_uFlags, (in_uFlags == 0) ? IntPtr.Zero : ((IntPtr) 1), (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
        AkCallbackManager.SetLastAddedPlayingID(num2);
        return num2;
    }

    public static AKRESULT PostString(string in_pszError, ErrorLevel in_eErrorLevel)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PostString(in_pszError, (int) in_eErrorLevel);
    }

    public static AKRESULT PostTrigger(string in_pszTrigger, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_1(in_pszTrigger, instanceID);
    }

    public static AKRESULT PostTrigger(uint in_triggerID, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_0(in_triggerID, instanceID);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_1((int) in_PreparationType, in_pszString);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_3((int) in_PreparationType, in_bankID);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkBankContent in_uFlags)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_0((int) in_PreparationType, in_pszString, (int) in_uFlags);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkBankContent in_uFlags)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_2((int) in_PreparationType, in_bankID, (int) in_uFlags);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_5((int) in_PreparationType, in_pszString, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_7((int) in_PreparationType, in_bankID, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_4((int) in_PreparationType, in_pszString, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), (int) in_uFlags);
    }

    public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_6((int) in_PreparationType, in_bankID, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()), (int) in_uFlags);
    }

    public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent)
    {
        AKRESULT akresult2;
        int num = 0;
        foreach (string str in in_ppszString)
        {
            num += str.Length + 1;
        }
        int num3 = 2;
        IntPtr ptr = Marshal.AllocHGlobal((int) (num * num3));
        Marshal.WriteInt16(ptr, (short) in_ppszString.Length);
        IntPtr destination = (IntPtr) (ptr.ToInt64() + num3);
        foreach (string str2 in in_ppszString)
        {
            Marshal.Copy(str2.ToCharArray(), 0, destination, str2.Length);
            destination = (IntPtr) (destination.ToInt64() + (num3 * str2.Length));
            Marshal.WriteInt16(destination, (short) 0);
            destination = (IntPtr) (destination.ToInt64() + num3);
        }
        try
        {
            akresult2 = (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_0((int) in_PreparationType, ptr, in_uNumEvent);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return akresult2;
    }

    public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_1((int) in_PreparationType, in_pEventID, in_uNumEvent);
    }

    public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        AKRESULT akresult2;
        int num = 0;
        foreach (string str in in_ppszString)
        {
            num += str.Length + 1;
        }
        int num3 = 2;
        IntPtr ptr = Marshal.AllocHGlobal((int) (num * num3));
        Marshal.WriteInt16(ptr, (short) in_ppszString.Length);
        IntPtr destination = (IntPtr) (ptr.ToInt64() + num3);
        foreach (string str2 in in_ppszString)
        {
            Marshal.Copy(str2.ToCharArray(), 0, destination, str2.Length);
            destination = (IntPtr) (destination.ToInt64() + (num3 * str2.Length));
            Marshal.WriteInt16(destination, (short) 0);
            destination = (IntPtr) (destination.ToInt64() + num3);
        }
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        try
        {
            akresult2 = (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_2((int) in_PreparationType, ptr, in_uNumEvent, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return akresult2;
    }

    public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_3((int) in_PreparationType, in_pEventID, in_uNumEvent, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs)
    {
        AKRESULT akresult2;
        int num = 0;
        foreach (string str in in_ppszGameSyncName)
        {
            num += str.Length + 1;
        }
        int num3 = 2;
        IntPtr ptr = Marshal.AllocHGlobal((int) (num * num3));
        Marshal.WriteInt16(ptr, (short) in_ppszGameSyncName.Length);
        IntPtr destination = (IntPtr) (ptr.ToInt64() + num3);
        foreach (string str2 in in_ppszGameSyncName)
        {
            Marshal.Copy(str2.ToCharArray(), 0, destination, str2.Length);
            destination = (IntPtr) (destination.ToInt64() + (num3 * str2.Length));
            Marshal.WriteInt16(destination, (short) 0);
            destination = (IntPtr) (destination.ToInt64() + num3);
        }
        try
        {
            akresult2 = (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_0((int) in_PreparationType, (int) in_eGameSyncType, in_pszGroupName, ptr, in_uNumGameSyncs);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return akresult2;
    }

    public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_1((int) in_PreparationType, (int) in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs);
    }

    public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        AKRESULT akresult2;
        int num = 0;
        foreach (string str in in_ppszGameSyncName)
        {
            num += str.Length + 1;
        }
        int num3 = 2;
        IntPtr ptr = Marshal.AllocHGlobal((int) (num * num3));
        Marshal.WriteInt16(ptr, (short) in_ppszGameSyncName.Length);
        IntPtr destination = (IntPtr) (ptr.ToInt64() + num3);
        foreach (string str2 in in_ppszGameSyncName)
        {
            Marshal.Copy(str2.ToCharArray(), 0, destination, str2.Length);
            destination = (IntPtr) (destination.ToInt64() + (num3 * str2.Length));
            Marshal.WriteInt16(destination, (short) 0);
            destination = (IntPtr) (destination.ToInt64() + num3);
        }
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        try
        {
            akresult2 = (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_2((int) in_PreparationType, (int) in_eGameSyncType, in_pszGroupName, ptr, in_uNumGameSyncs, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return akresult2;
    }

    public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_3((int) in_PreparationType, (int) in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT QueryAudioObjectIDs(string in_pszEventName, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_1(in_pszEventName, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
    }

    public static AKRESULT QueryAudioObjectIDs(uint in_eventID, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_0(in_eventID, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
    }

    public static AKRESULT RegisterGameObj(GameObject in_gameObjectID)
    {
        return RegisterGameObjInternal(in_gameObjectID.GetInstanceID());
    }

    public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, string in_pszObjName)
    {
        return RegisterGameObjInternal_WithName(in_gameObjectID.GetInstanceID(), in_gameObjectID.name);
    }

    public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, uint in_uListenerMask)
    {
        return RegisterGameObjInternal_WithMask(in_gameObjectID.GetInstanceID(), in_uListenerMask);
    }

    public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, string in_pszObjName, uint in_uListenerMask)
    {
        return RegisterGameObjInternal_WithName_WithMask(in_gameObjectID.GetInstanceID(), in_gameObjectID.name, in_uListenerMask);
    }

    public static AKRESULT RegisterGameObjInternal(int in_GameObj)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal(in_GameObj);
    }

    public static AKRESULT RegisterGameObjInternal_WithMask(int in_GameObj, uint in_ulListenerMask)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithMask(in_GameObj, in_ulListenerMask);
    }

    public static AKRESULT RegisterGameObjInternal_WithName(int in_GameObj, string in_pszObjName)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithName(in_GameObj, in_pszObjName);
    }

    public static AKRESULT RegisterGameObjInternal_WithName_WithMask(int in_GameObj, string in_pszObjName, uint in_ulListenerMask)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithName_WithMask(in_GameObj, in_pszObjName, in_ulListenerMask);
    }

    public static void RemovePlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
    {
        AkSoundEnginePINVOKE.CSharp_RemovePlayerMotionDevice(in_iPlayerID, in_iCompanyID, in_iDeviceID);
    }

    public static AKRESULT RemoveSecondaryOutput(uint in_iOutputID, AkAudioOutputType in_iDeviceType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RemoveSecondaryOutput(in_iOutputID, (int) in_iDeviceType);
    }

    public static AKRESULT RenderAudio()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_RenderAudio();
    }

    public static AKRESULT ResetRTPCValue(string in_pszRtpcName)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_9(in_pszRtpcName);
    }

    public static AKRESULT ResetRTPCValue(uint in_rtpcID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_4(in_rtpcID);
    }

    public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_8(in_pszRtpcName, instanceID);
    }

    public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_3(in_rtpcID, instanceID);
    }

    public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_7(in_pszRtpcName, instanceID, in_uValueChangeDuration);
    }

    public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_2(in_rtpcID, instanceID, in_uValueChangeDuration);
    }

    public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_6(in_pszRtpcName, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_1(in_rtpcID, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_5(in_pszRtpcName, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_0(in_rtpcID, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments)
    {
        return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_1(in_eventID, in_aArgumentValues, in_uNumArguments);
    }

    public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments, uint in_idSequence)
    {
        return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_0(in_eventID, in_aArgumentValues, in_uNumArguments, in_idSequence);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_5(in_pszEventName, instanceID, in_iPosition);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_11(in_pszEventName, instanceID, in_fPercent);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_2(in_eventID, instanceID, in_iPosition);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_8(in_eventID, instanceID, in_fPercent);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_4(in_pszEventName, instanceID, in_iPosition, in_bSeekToNearestMarker);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_10(in_pszEventName, instanceID, in_fPercent, in_bSeekToNearestMarker);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_1(in_eventID, instanceID, in_iPosition, in_bSeekToNearestMarker);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_7(in_eventID, instanceID, in_fPercent, in_bSeekToNearestMarker);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_3(in_pszEventName, instanceID, in_iPosition, in_bSeekToNearestMarker, in_PlayingID);
    }

    public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_9(in_pszEventName, instanceID, in_fPercent, in_bSeekToNearestMarker, in_PlayingID);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_0(in_eventID, instanceID, in_iPosition, in_bSeekToNearestMarker, in_PlayingID);
    }

    public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker, uint in_PlayingID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_6(in_eventID, instanceID, in_fPercent, in_bSeekToNearestMarker, in_PlayingID);
    }

    public static AKRESULT SetActiveListeners(GameObject in_GameObjectID, uint in_uListenerMask)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetActiveListeners(instanceID, in_uListenerMask);
    }

    public static AKRESULT SetActorMixerEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetActorMixerEffect(in_audioNodeID, in_uFXIndex, in_shareSetID);
    }

    public static AKRESULT SetAttenuationScalingFactor(GameObject in_GameObjectID, float in_fAttenuationScalingFactor)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetAttenuationScalingFactor(instanceID, in_fAttenuationScalingFactor);
    }

    public static AKRESULT SetBankLoadIOSettings(float in_fThroughput, sbyte in_priority)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetBankLoadIOSettings(in_fThroughput, in_priority);
    }

    public static AKRESULT SetBasePath(string in_pszBasePath)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetBasePath(in_pszBasePath);
    }

    public static AKRESULT SetBusEffect(string in_pszBusName, uint in_uFXIndex, uint in_shareSetID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_1(in_pszBusName, in_uFXIndex, in_shareSetID);
    }

    public static AKRESULT SetBusEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_0(in_audioNodeID, in_uFXIndex, in_shareSetID);
    }

    public static AKRESULT SetCurrentLanguage(string in_pszAudioSrcPath)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetCurrentLanguage(in_pszAudioSrcPath);
    }

    public static AKRESULT SetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray in_aAuxSendValues, uint in_uNumSendValues)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetGameObjectAuxSendValues(instanceID, in_aAuxSendValues.m_Buffer, in_uNumSendValues);
    }

    public static AKRESULT SetGameObjectOutputBusVolume(GameObject in_gameObjectID, float in_fControlValue)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetGameObjectOutputBusVolume(instanceID, in_fControlValue);
    }

    public static AKRESULT SetListenerPipeline(uint in_uIndex, bool in_bAudio, bool in_bMotion)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetListenerPipeline(in_uIndex, in_bAudio, in_bMotion);
    }

    public static AKRESULT SetListenerPosition(float FrontX, float FrontY, float FrontZ, float TopX, float TopY, float TopZ, float PosX, float PosY, float PosZ, uint in_ulListenerIndex)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetListenerPosition(FrontX, FrontY, FrontZ, TopX, TopY, TopZ, PosX, PosY, PosZ, in_ulListenerIndex);
    }

    public static AKRESULT SetListenerScalingFactor(uint in_uListenerIndex, float in_fListenerScalingFactor)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetListenerScalingFactor(in_uListenerIndex, in_fListenerScalingFactor);
    }

    public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized, AkChannelConfig in_channelConfig)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_1(in_uIndex, in_bSpatialized, AkChannelConfig.getCPtr(in_channelConfig));
    }

    public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized, AkChannelConfig in_channelConfig, float[] in_pVolumeOffsets)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_0(in_uIndex, in_bSpatialized, AkChannelConfig.getCPtr(in_channelConfig), in_pVolumeOffsets);
    }

    public static AKRESULT SetMaxNumVoicesLimit(ushort in_maxNumberVoices)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMaxNumVoicesLimit(in_maxNumberVoices);
    }

    public static AKRESULT SetMedia(AkSourceSettings in_pSourceSettings, uint in_uNumSourceSettings)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMedia(AkSourceSettings.getCPtr(in_pSourceSettings), in_uNumSourceSettings);
    }

    public static AKRESULT SetMixer(string in_pszBusName, uint in_shareSetID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMixer__SWIG_1(in_pszBusName, in_shareSetID);
    }

    public static AKRESULT SetMixer(uint in_audioNodeID, uint in_shareSetID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMixer__SWIG_0(in_audioNodeID, in_shareSetID);
    }

    public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_1(instanceID, in_pPositions.m_Buffer, in_NumPositions);
    }

    public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions, MultiPositionType in_eMultiPositionType)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_0(instanceID, in_pPositions.m_Buffer, in_NumPositions, (int) in_eMultiPositionType);
    }

    public static AKRESULT SetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, float in_fObstructionLevel, float in_fOcclusionLevel)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_ObjectID != null)
        {
            instanceID = (uint) in_ObjectID.GetInstanceID();
            if (in_ObjectID.activeInHierarchy)
            {
                if (in_ObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_ObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_ObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetObjectObstructionAndOcclusion(instanceID, in_uListener, in_fObstructionLevel, in_fOcclusionLevel);
    }

    public static AKRESULT SetObjectPosition(GameObject in_GameObjectID, float PosX, float PosY, float PosZ, float OrientationX, float OrientationY, float OrientationZ)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_GameObjectID != null)
        {
            instanceID = (uint) in_GameObjectID.GetInstanceID();
            if (in_GameObjectID.activeInHierarchy)
            {
                if (in_GameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_GameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_GameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetObjectPosition(instanceID, PosX, PosY, PosZ, OrientationX, OrientationY, OrientationZ);
    }

    public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_2((int) in_ePanningRule);
    }

    public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule, AkAudioOutputType in_eSinkType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_1((int) in_ePanningRule, (int) in_eSinkType);
    }

    public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule, AkAudioOutputType in_eSinkType, uint in_iOutputID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_0((int) in_ePanningRule, (int) in_eSinkType, in_iOutputID);
    }

    public static void SetPlayerListener(byte in_iPlayerID, byte in_iListener)
    {
        AkSoundEnginePINVOKE.CSharp_SetPlayerListener(in_iPlayerID, in_iListener);
    }

    public static void SetPlayerVolume(byte in_iPlayerID, float in_fVolume)
    {
        AkSoundEnginePINVOKE.CSharp_SetPlayerVolume(in_iPlayerID, in_fVolume);
    }

    public static void SetRandomSeed(uint in_uSeed)
    {
        AkSoundEnginePINVOKE.CSharp_SetRandomSeed(in_uSeed);
    }

    public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_9(in_pszRtpcName, in_value);
    }

    public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_4(in_rtpcID, in_value);
    }

    public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_8(in_pszRtpcName, in_value, instanceID);
    }

    public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_3(in_rtpcID, in_value, instanceID);
    }

    public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_7(in_pszRtpcName, in_value, instanceID, in_uValueChangeDuration);
    }

    public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_2(in_rtpcID, in_value, instanceID, in_uValueChangeDuration);
    }

    public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_6(in_pszRtpcName, in_value, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_1(in_rtpcID, in_value, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_5(in_pszRtpcName, in_value, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_0(in_rtpcID, in_value, instanceID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_7(in_pszRtpcName, in_value, in_playingID);
    }

    public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_3(in_rtpcID, in_value, in_playingID);
    }

    public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_6(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration);
    }

    public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_2(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration);
    }

    public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_5(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_1(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_4(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_0(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration, (int) in_eFadeCurve, in_bBypassInternalValueInterpolation);
    }

    public static AKRESULT SetSecondaryOutputVolume(uint in_iOutputID, AkAudioOutputType in_iDeviceType, float in_fVolume)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSecondaryOutputVolume(in_iOutputID, (int) in_iDeviceType, in_fVolume);
    }

    public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_2(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle);
    }

    public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle, AkAudioOutputType in_eSinkType)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_1(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle, (int) in_eSinkType);
    }

    public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle, AkAudioOutputType in_eSinkType, uint in_iOutputID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_0(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle, (int) in_eSinkType, in_iOutputID);
    }

    public static AKRESULT SetState(string in_pszStateGroup, string in_pszState)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetState__SWIG_1(in_pszStateGroup, in_pszState);
    }

    public static AKRESULT SetState(uint in_stateGroup, uint in_state)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetState__SWIG_0(in_stateGroup, in_state);
    }

    public static AKRESULT SetSwitch(string in_pszSwitchGroup, string in_pszSwitchState, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_1(in_pszSwitchGroup, in_pszSwitchState, instanceID);
    }

    public static AKRESULT SetSwitch(uint in_switchGroup, uint in_switchState, GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_0(in_switchGroup, in_switchState, instanceID);
    }

    public static AKRESULT SetVolumeThreshold(float in_fVolumeThresholdDB)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_SetVolumeThreshold(in_fVolumeThresholdDB);
    }

    public static AKRESULT StartOutputCapture(string in_CaptureFileName)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_StartOutputCapture(in_CaptureFileName);
    }

    public static AKRESULT StartProfilerCapture(string in_CaptureFileName)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_StartProfilerCapture(in_CaptureFileName);
    }

    public static void StopAll()
    {
        AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_1();
    }

    public static void StopAll(GameObject in_gameObjectID)
    {
        AkAutoObject obj2 = null;
        uint instanceID;
        if (in_gameObjectID != null)
        {
            instanceID = (uint) in_gameObjectID.GetInstanceID();
            if (in_gameObjectID.activeInHierarchy)
            {
                if (in_gameObjectID.GetComponent<AkGameObj>() == null)
                {
                    in_gameObjectID.AddComponent<AkGameObj>();
                }
            }
            else
            {
                obj2 = new AkAutoObject(in_gameObjectID);
                instanceID = (uint) obj2.m_id;
            }
        }
        else
        {
            instanceID = uint.MaxValue;
        }
        AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_0(instanceID);
    }

    public static AKRESULT StopOutputCapture()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_StopOutputCapture();
    }

    public static void StopPlayingID(uint in_playingID)
    {
        AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_2(in_playingID);
    }

    public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration)
    {
        AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_1(in_playingID, in_uTransitionDuration);
    }

    public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
    {
        AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_0(in_playingID, in_uTransitionDuration, (int) in_eFadeCurve);
    }

    public static AKRESULT StopProfilerCapture()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_StopProfilerCapture();
    }

    public static AKRESULT Suspend()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Suspend__SWIG_1();
    }

    public static AKRESULT Suspend(bool in_bRenderAnyway)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Suspend__SWIG_0(in_bRenderAnyway);
    }

    public static void Term()
    {
        AkSoundEnginePINVOKE.CSharp_Term();
    }

    public static AKRESULT UnloadAllFilePackages()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadAllFilePackages();
    }

    public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_1(in_pszString, in_pInMemoryBankPtr);
    }

    public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_3(in_bankID, in_pInMemoryBankPtr);
    }

    public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr, out int out_pMemPoolId)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_0(in_pszString, in_pInMemoryBankPtr, out out_pMemPoolId);
    }

    public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr, out int out_pMemPoolId)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_2(in_bankID, in_pInMemoryBankPtr, out out_pMemPoolId);
    }

    public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_4(in_pszString, in_pInMemoryBankPtr, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
    {
        in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_5(in_bankID, in_pInMemoryBankPtr, IntPtr.Zero, (in_pCookie == null) ? IntPtr.Zero : ((IntPtr) in_pCookie.GetHashCode()));
    }

    public static AKRESULT UnloadFilePackage(uint in_uPackageID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnloadFilePackage(in_uPackageID);
    }

    public static AKRESULT UnregisterAllGameObj()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnregisterAllGameObj();
    }

    public static AKRESULT UnregisterGameObj(GameObject in_gameObjectID)
    {
        return UnregisterGameObjInternal(in_gameObjectID.GetInstanceID());
    }

    public static AKRESULT UnregisterGameObjInternal(int in_GameObj)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnregisterGameObjInternal(in_GameObj);
    }

    public static AKRESULT UnsetMedia(AkSourceSettings in_pSourceSettings, uint in_uNumSourceSettings)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_UnsetMedia(AkSourceSettings.getCPtr(in_pSourceSettings), in_uNumSourceSettings);
    }

    public static AKRESULT WakeupFromSuspend()
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_WakeupFromSuspend();
    }

    public static ushort AK_FLOAT
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_FLOAT_get();
        }
    }

    public static ushort AK_INT
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INT_get();
        }
    }

    public static byte AK_INTERLEAVED
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INTERLEAVED_get();
        }
    }

    public static uint AK_INVALID_AUX_ID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INVALID_AUX_ID_get();
        }
    }

    public static uint AK_INVALID_CHANNELMASK
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INVALID_CHANNELMASK_get();
        }
    }

    public static byte AK_INVALID_MIDI_CHANNEL
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INVALID_MIDI_CHANNEL_get();
        }
    }

    public static byte AK_INVALID_MIDI_NOTE
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INVALID_MIDI_NOTE_get();
        }
    }

    public static uint AK_INVALID_OUTPUT_DEVICE_ID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_INVALID_OUTPUT_DEVICE_ID_get();
        }
    }

    public static uint AK_LE_NATIVE_BITSPERSAMPLE
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_BITSPERSAMPLE_get();
        }
    }

    public static uint AK_LE_NATIVE_INTERLEAVE
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_INTERLEAVE_get();
        }
    }

    public static uint AK_LE_NATIVE_SAMPLETYPE
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_SAMPLETYPE_get();
        }
    }

    public static byte AK_NONINTERLEAVED
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_NONINTERLEAVED_get();
        }
    }

    public static uint AK_SOUNDBANK_VERSION
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AK_SOUNDBANK_VERSION_get();
        }
    }
}

