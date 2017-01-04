using System;

public class AkMultiPosEvent
{
    public bool eventIsPlaying;
    public ListView<AkAmbient> list = new ListView<AkAmbient>();

    public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
    {
        this.eventIsPlaying = false;
    }
}

