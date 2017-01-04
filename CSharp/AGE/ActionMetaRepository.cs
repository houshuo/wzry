namespace AGE
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ActionMetaRepository : Singleton<ActionMetaRepository>
    {
        public DictionaryView<System.Type, List<AssetReferenceMeta>> Repositories = new DictionaryView<System.Type, List<AssetReferenceMeta>>();

        public List<AssetReferenceMeta> GetAssociatedResourcesMeta(System.Type InType)
        {
            if ((InType != typeof(BaseEvent)) && !InType.IsSubclassOf(typeof(BaseEvent)))
            {
                return null;
            }
            List<AssetReferenceMeta> list = null;
            if (!this.Repositories.TryGetValue(InType, out list))
            {
                list = new List<AssetReferenceMeta>();
                this.Repositories.Add(InType, list);
                for (System.Type type = InType; (type == typeof(BaseEvent)) || type.IsSubclassOf(typeof(BaseEvent)); type = type.BaseType)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    if (fields != null)
                    {
                        for (int i = 0; i < fields.Length; i++)
                        {
                            FieldInfo element = fields[i];
                            AssetReference customAttribute = Attribute.GetCustomAttribute(element, typeof(AssetReference)) as AssetReference;
                            if (customAttribute != null)
                            {
                                AssetReferenceMeta item = new AssetReferenceMeta {
                                    MetaFieldInfo = element,
                                    Reference = customAttribute
                                };
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}

