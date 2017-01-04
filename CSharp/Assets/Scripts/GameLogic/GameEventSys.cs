namespace Assets.Scripts.GameLogic
{
    using System;

    public class GameEventSys : Singleton<GameEventSys>
    {
        private Delegate[] ActionTable = new Delegate[0x33];
        private ListView<IPostEventWrapper> postEventList = new ListView<IPostEventWrapper>();

        public void AddEventHandler<ParamType>(GameEventDef evt, RefAction<ParamType> handler)
        {
            if (this.CheckValidAdd(evt, handler))
            {
                this.ActionTable[(int) evt] = (RefAction<ParamType>) Delegate.Combine((RefAction<ParamType>) this.ActionTable[(int) evt], handler);
            }
        }

        public void AddEventHandler(GameEventDef evt, Action handler)
        {
            if (this.CheckValidAdd(evt, handler))
            {
                this.ActionTable[(int) evt] = (Action) Delegate.Combine((Action) this.ActionTable[(int) evt], handler);
            }
        }

        private bool CheckValidAdd(GameEventDef evt, Delegate handler)
        {
            Delegate delegate2 = this.ActionTable[(int) evt];
            if ((delegate2 != null) && (delegate2.GetType() != handler.GetType()))
            {
                return false;
            }
            return true;
        }

        private bool CheckValidRmv(GameEventDef evt, Delegate handler)
        {
            Delegate delegate2 = this.ActionTable[(int) evt];
            if (delegate2 != null)
            {
                if (delegate2.GetType() != handler.GetType())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public void PostEvent<ParamType>(GameEventDef evt, ref ParamType prm)
        {
            RefAction<ParamType> action = this.ActionTable[(int) evt] as RefAction<ParamType>;
            if (action != null)
            {
                PostEventWrapper<ParamType> item = new PostEventWrapper<ParamType>(action, prm, 1);
                this.postEventList.Add(item);
            }
        }

        public void RmvEventHandler(GameEventDef evt, Action handler)
        {
            if (this.CheckValidRmv(evt, handler))
            {
                this.ActionTable[(int) evt] = (Action) Delegate.Remove((Action) this.ActionTable[(int) evt], handler);
            }
        }

        public void RmvEventHandler<ParamType>(GameEventDef evt, RefAction<ParamType> handler)
        {
            if (this.CheckValidRmv(evt, handler))
            {
                this.ActionTable[(int) evt] = (RefAction<ParamType>) Delegate.Remove((RefAction<ParamType>) this.ActionTable[(int) evt], handler);
            }
        }

        public void SendEvent(GameEventDef evt)
        {
            Action action = this.ActionTable[(int) evt] as Action;
            if (action != null)
            {
                action();
            }
        }

        public void SendEvent<ParamType>(GameEventDef evt, ref ParamType prm)
        {
            RefAction<ParamType> action = this.ActionTable[(int) evt] as RefAction<ParamType>;
            if (action != null)
            {
                action(ref prm);
            }
        }

        public void UpdateEvent()
        {
            if (this.postEventList.Count > 0)
            {
                int index = 0;
                while (index < this.postEventList.Count)
                {
                    uint curFrameNum = Singleton<FrameSynchr>.instance.CurFrameNum;
                    IPostEventWrapper wrapper = this.postEventList[index];
                    if (wrapper.GetFrameNum() >= curFrameNum)
                    {
                        wrapper.ExecCommand();
                        this.postEventList.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
    }
}

