using System;
using System.Collections;
using System.Reflection;

public class CheatCommandRegister : Singleton<CheatCommandRegister>
{
    protected ListView<ICheatCommand> CommandRepositories = new ListView<ICheatCommand>();

    protected void OnFoundClass(string InID, Type InType)
    {
        CheatCommandAttribute attribute = InType.GetCustomAttributes(typeof(CheatCommandAttribute), false)[0] as CheatCommandAttribute;
        DebugHelper.Assert(attribute != null);
        ICheatCommand item = Activator.CreateInstance(InType) as ICheatCommand;
        DebugHelper.Assert(item != null);
        this.CommandRepositories.Add(item);
        Singleton<CheatCommandsRepository>.instance.RegisterCommand(item);
    }

    public void Register(Assembly InAssembly)
    {
        this.RegisterCommonCommands(InAssembly);
        this.RegisterMethodCommands(InAssembly);
    }

    protected void RegisterCommonCommands(Assembly InAssembly)
    {
        Type[] types = InAssembly.GetTypes();
        for (int i = 0; (types != null) && (i < types.Length); i++)
        {
            Type inType = types[i];
            object[] customAttributes = inType.GetCustomAttributes(typeof(CheatCommandAttribute), true);
            if (customAttributes != null)
            {
                for (int j = 0; j < customAttributes.Length; j++)
                {
                    CheatCommandAttribute attribute = customAttributes[j] as CheatCommandAttribute;
                    if (attribute != null)
                    {
                        this.OnFoundClass(attribute.ID, inType);
                    }
                }
            }
        }
    }

    protected void RegisterMethod(Type InEntryType, CheatCommandEntryAttribute InEntryAttr, MethodInfo InMethod, CheatCommandEntryMethodAttribute InMethodAttr)
    {
        CheatCommandMethod inCommand = new CheatCommandMethod(InMethod, InEntryAttr, InMethodAttr);
        Singleton<CheatCommandsRepository>.instance.RegisterCommand(inCommand);
    }

    protected void RegisterMethodCommands(Assembly InAssembly)
    {
        ClassEnumerator enumerator = new ClassEnumerator(typeof(CheatCommandEntryAttribute), null, InAssembly, true, false, false);
        ListView<Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Type current = enumerator2.Current;
            this.RegisterMethods(current);
        }
    }

    protected void RegisterMethods(Type InType)
    {
        CheatCommandEntryAttribute inEntryAttr = (CheatCommandEntryAttribute) InType.GetCustomAttributes(typeof(CheatCommandEntryAttribute), false)[0];
        DebugHelper.Assert(inEntryAttr != null);
        MethodInfo[] methods = InType.GetMethods();
        if (methods != null)
        {
            IEnumerator enumerator = methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodInfo current = (MethodInfo) enumerator.Current;
                if (current.IsStatic)
                {
                    object[] customAttributes = current.GetCustomAttributes(typeof(CheatCommandEntryMethodAttribute), false);
                    if (((customAttributes != null) && (customAttributes.Length > 0)) && this.ValidateMethodArguments(current))
                    {
                        CheatCommandEntryMethodAttribute inMethodAttr = (CheatCommandEntryMethodAttribute) customAttributes[0];
                        this.RegisterMethod(InType, inEntryAttr, current, inMethodAttr);
                    }
                }
            }
        }
    }

    protected bool ValidateMethodArguments(MethodInfo InMethod)
    {
        if (InMethod.ReturnType != typeof(string))
        {
            DebugHelper.Assert(false, "Method Command must return a string.");
            return false;
        }
        ParameterInfo[] parameters = InMethod.GetParameters();
        if ((parameters != null) && (parameters.Length > 0))
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info = parameters[i];
                if (info.IsOut)
                {
                    DebugHelper.Assert(false, string.Format("method command argument can't be out parameter. Method:{0}, Parameter:{1} {2}", InMethod.Name, info.ParameterType.Name, info.Name));
                    return false;
                }
                if (info.ParameterType.IsByRef)
                {
                    DebugHelper.Assert(false, string.Format("method command argument can't be ref parameter. Method:{0}, Parameter:{1} {2}", InMethod.Name, info.ParameterType.Name, info.Name));
                    return false;
                }
                IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(info.ParameterType);
                DebugHelper.Assert(description != null);
                if (!description.AcceptAsMethodParameter(info.ParameterType))
                {
                    DebugHelper.Assert(false, string.Format("unsupported argument type for method command. Method:{0}, {1}, {2}", InMethod.Name, info.ParameterType.Name, info.Name));
                    return false;
                }
            }
        }
        return true;
    }
}

