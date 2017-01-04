namespace AGE
{
    using System;
    using System.Reflection;

    public class RefData
    {
        public object dataObject;
        public int eventIdx = -1;
        public FieldInfo fieldInfo;
        public int trackIndex = -1;

        public RefData(FieldInfo field, object obj)
        {
            this.fieldInfo = field;
            this.dataObject = obj;
            if (this.dataObject is Track)
            {
                Track dataObject = this.dataObject as Track;
                this.trackIndex = dataObject.trackIndex;
                this.eventIdx = -1;
            }
            else
            {
                BaseEvent event2 = this.dataObject as BaseEvent;
                this.trackIndex = event2.track.trackIndex;
                this.eventIdx = event2.track.GetIndexOfEvent(event2);
            }
        }
    }
}

