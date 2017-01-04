namespace AGE
{
    using System;

    public class RefParamObject
    {
        public bool dirty;
        public object value;

        public RefParamObject(object v)
        {
            this.value = v;
            this.dirty = false;
        }
    }
}

