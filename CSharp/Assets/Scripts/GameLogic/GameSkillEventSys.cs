namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    public class GameSkillEventSys : Singleton<GameSkillEventSys>
    {
        public Delegate[] skillEventTable = new Delegate[0x1a];

        public void AddEventHandler<ParamType>(GameSkillEventDef _event, GameSkillEvent<ParamType> _handler)
        {
            int index = (int) _event;
            this.skillEventTable[index] = (GameSkillEvent<ParamType>) Delegate.Combine((GameSkillEvent<ParamType>) this.skillEventTable[index], _handler);
        }

        public void RmvEventHandler<ParamType>(GameSkillEventDef _event, GameSkillEvent<ParamType> _handler)
        {
            int index = (int) _event;
            this.skillEventTable[index] = (GameSkillEvent<ParamType>) Delegate.Remove((GameSkillEvent<ParamType>) this.skillEventTable[index], _handler);
        }

        private void SendEvent<ParamType>(GameSkillEventDef _event, ref ParamType _param)
        {
            int index = (int) _event;
            GameSkillEvent<ParamType> event2 = this.skillEventTable[index] as GameSkillEvent<ParamType>;
            if (event2 != null)
            {
                event2(ref _param);
            }
        }

        public void SendEvent<ParamType>(GameSkillEventDef _event, PoolObjHandle<ActorRoot> _src, ref ParamType _param, GameSkillEventChannel _channel = 0)
        {
            if (_src != 0)
            {
                if (_channel == GameSkillEventChannel.Channel_HostCtrlActor)
                {
                    if (ActorHelper.IsHostCtrlActor(ref _src))
                    {
                        this.SendEvent<ParamType>(_event, ref _param);
                    }
                }
                else if (_channel == GameSkillEventChannel.Channel_HostActor)
                {
                    if (ActorHelper.IsHostActor(ref _src))
                    {
                        this.SendEvent<ParamType>(_event, ref _param);
                    }
                }
                else if (_channel == GameSkillEventChannel.Channel_AllActor)
                {
                    this.SendEvent<ParamType>(_event, ref _param);
                }
            }
        }
    }
}

