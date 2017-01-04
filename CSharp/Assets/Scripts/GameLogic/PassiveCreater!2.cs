namespace Assets.Scripts.GameLogic
{
    using System;

    public class PassiveCreater<CreateType, CreateTypeAttribute> : Singleton<PassiveCreater<CreateType, CreateTypeAttribute>> where CreateTypeAttribute: PassivetAttribute
    {
        private DictionaryView<int, System.Type> eventTypeSet;

        public PassiveCreater()
        {
            this.eventTypeSet = new DictionaryView<int, System.Type>();
        }

        public CreateType Create(int _type)
        {
            System.Type type;
            if (this.eventTypeSet.TryGetValue(_type, out type))
            {
                return (CreateType) Activator.CreateInstance(type);
            }
            return default(CreateType);
        }

        public override void Init()
        {
            ClassEnumerator enumerator = new ClassEnumerator(typeof(CreateTypeAttribute), typeof(CreateType), typeof(CreateTypeAttribute).Assembly, true, false, false);
            foreach (System.Type type in enumerator.results)
            {
                Attribute customAttribute = Attribute.GetCustomAttribute(type, typeof(CreateTypeAttribute));
                this.eventTypeSet.Add((customAttribute as CreateTypeAttribute).attributeType, type);
            }
        }
    }
}

