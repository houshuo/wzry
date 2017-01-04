namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Reflection;

    [SkillFuncHandlerClass]
    public class SkillFuncDelegator : Singleton<SkillFuncDelegator>
    {
        private DealSkillFunc[] SkillFuncHandlers = new DealSkillFunc[0x4c];

        public bool DoSkillFunc(int inSkillFuncType, ref SSkillFuncContext inContext)
        {
            DealSkillFunc func = this.SkillFuncHandlers[inSkillFuncType];
            return ((func != null) && func(ref inContext));
        }

        public override void Init()
        {
            ClassEnumerator enumerator = new ClassEnumerator(typeof(SkillFuncHandlerClassAttribute), null, typeof(SkillFuncDelegator).Assembly, true, false, false);
            ListView<System.Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                MethodInfo[] methods = enumerator2.Current.GetMethods();
                for (int i = 0; (methods != null) && (i < methods.Length); i++)
                {
                    MethodInfo method = methods[i];
                    if (method.IsStatic)
                    {
                        object[] customAttributes = method.GetCustomAttributes(typeof(SkillFuncHandlerAttribute), true);
                        for (int j = 0; j < customAttributes.Length; j++)
                        {
                            SkillFuncHandlerAttribute attribute = customAttributes[j] as SkillFuncHandlerAttribute;
                            if (attribute != null)
                            {
                                this.RegisterHandler(attribute.ID, (DealSkillFunc) Delegate.CreateDelegate(typeof(DealSkillFunc), method));
                                if (attribute.AdditionalIdList != null)
                                {
                                    int length = attribute.AdditionalIdList.Length;
                                    for (int k = 0; k < length; k++)
                                    {
                                        this.RegisterHandler(attribute.AdditionalIdList[k], (DealSkillFunc) Delegate.CreateDelegate(typeof(DealSkillFunc), method));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RegisterHandler(int inSkillFuncType, DealSkillFunc handler)
        {
            if (this.SkillFuncHandlers[inSkillFuncType] != null)
            {
                DebugHelper.Assert(false, "重复注册技能效果处理函数，请检查");
            }
            else
            {
                this.SkillFuncHandlers[inSkillFuncType] = handler;
            }
        }
    }
}

