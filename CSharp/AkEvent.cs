using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Wwise/AkEvent")]
public class AkEvent : AkUnityEventHandler
{
    public AkActionOnEventType actionOnEventType;
    public AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;
    public bool enableActionOnEvent;
    public int eventID;
    public AkEventCallbackData m_callbackData;
    public GameObject soundEmitterObject;
    public float transitionDuration;

    private void Callback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        for (int i = 0; i < this.m_callbackData.callbackFunc.Count; i++)
        {
            if (((in_type & this.m_callbackData.callbackFlags[i]) != ((AkCallbackType) 0)) && (this.m_callbackData.callbackGameObj[i] != null))
            {
                AkEventCallbackMsg msg = new AkEventCallbackMsg {
                    type = in_type,
                    sender = base.gameObject,
                    info = in_info
                };
                this.m_callbackData.callbackGameObj[i].SendMessage(this.m_callbackData.callbackFunc[i], msg);
            }
        }
    }

    public override void HandleEvent(GameObject in_gameObject)
    {
        GameObject obj2 = (!base.useOtherObject || (in_gameObject == null)) ? base.gameObject : in_gameObject;
        this.soundEmitterObject = obj2;
        if (this.enableActionOnEvent)
        {
            AkSoundEngine.ExecuteActionOnEvent((uint) this.eventID, this.actionOnEventType, obj2, ((int) this.transitionDuration) * 0x3e8, this.curveInterpolation);
        }
        else if (this.m_callbackData != null)
        {
            AkSoundEngine.PostEvent((uint) this.eventID, obj2, (uint) this.m_callbackData.uFlags, new AkCallbackManager.EventCallback(this.Callback), null, 0, null, 0);
        }
        else
        {
            AkSoundEngine.PostEvent((uint) this.eventID, obj2);
        }
    }

    public void Stop(int _transitionDuration, AkCurveInterpolation _curveInterpolation = 4)
    {
        AkSoundEngine.ExecuteActionOnEvent((uint) this.eventID, AkActionOnEventType.AkActionOnEventType_Stop, this.soundEmitterObject, _transitionDuration, _curveInterpolation);
    }
}

