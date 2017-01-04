using System;
using System.Collections.Generic;

public class CheatCommandsRepository : Singleton<CheatCommandsRepository>
{
    private CheatCommandGroup GeneralRepositories = new CheatCommandGroup();
    private DictionaryView<string, CheatCommandGroup> Repositories = new DictionaryView<string, CheatCommandGroup>();

    public string ExecuteCommand(string InCommand, string[] InArgs)
    {
        if (this.HasCommand(InCommand))
        {
            return this.GeneralRepositories.Commands[InCommand.ToLower()].StartProcess(InArgs);
        }
        return "Command not found";
    }

    public ListView<ICheatCommand> FilterByString(string InPrefix)
    {
        DebugHelper.Assert(InPrefix != null);
        ListView<ICheatCommand> view = new ListView<ICheatCommand>(0x10);
        DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.GeneralRepositories.Commands.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, ICheatCommand> current = enumerator.Current;
            ICheatCommand item = current.Value;
            if (item.command.baseName.StartsWith(InPrefix, StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(InPrefix))
            {
                view.Add(item);
            }
        }
        return view;
    }

    public ICheatCommand FindCommand(string InCommand)
    {
        ICheatCommand command = null;
        this.GeneralRepositories.Commands.TryGetValue(InCommand.ToLower(), out command);
        return command;
    }

    public bool HasCommand(string InCommand)
    {
        return this.GeneralRepositories.Commands.ContainsKey(InCommand.ToLower());
    }

    public void RegisterCommand(ICheatCommand InCommand)
    {
        DebugHelper.Assert((InCommand != null) && !this.HasCommand(InCommand.command.baseName));
        this.GeneralRepositories.Commands[InCommand.command.baseName.ToLower()] = InCommand;
        string[] groupHierarchies = InCommand.command.groupHierarchies;
        DebugHelper.Assert(groupHierarchies != null);
        string key = groupHierarchies[0];
        CheatCommandGroup group = null;
        if (!this.Repositories.TryGetValue(key, out group))
        {
            group = new CheatCommandGroup();
            this.Repositories[key] = group;
        }
        group.AddCommand(InCommand, 1);
    }

    public CheatCommandGroup generalRepositories
    {
        get
        {
            return this.GeneralRepositories;
        }
    }

    public DictionaryView<string, CheatCommandGroup> repositories
    {
        get
        {
            return this.Repositories;
        }
    }
}

