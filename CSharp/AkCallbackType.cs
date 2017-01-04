using System;

public enum AkCallbackType
{
    AK_AudioInterruption = 0x22000000,
    AK_AudioSourceChange = 0x23000000,
    AK_Bank = 0x40000000,
    AK_CallbackBits = 0xfffff,
    AK_Duration = 8,
    AK_EnableGetMusicPlayPosition = 0x200000,
    AK_EnableGetSourcePlayPosition = 0x100000,
    AK_EnableGetSourceStreamBuffering = 0x400000,
    AK_EndOfDynamicSequenceItem = 2,
    AK_EndOfEvent = 1,
    AK_Marker = 4,
    AK_MidiEvent = 0x10000,
    AK_Monitoring = 0x20000000,
    AK_MusicPlaylistSelect = 0x40,
    AK_MusicPlayStarted = 0x80,
    AK_MusicSyncAll = 0x7f00,
    AK_MusicSyncBar = 0x200,
    AK_MusicSyncBeat = 0x100,
    AK_MusicSyncEntry = 0x400,
    AK_MusicSyncExit = 0x800,
    AK_MusicSyncGrid = 0x1000,
    AK_MusicSyncPoint = 0x4000,
    AK_MusicSyncUserCue = 0x2000,
    AK_SpeakerVolumeMatrix = 0x10,
    AK_Starvation = 0x20
}

