using System;

public class CheatCommandGroup
{
    public DictionaryView<string, CheatCommandGroup> ChildrenGroups = new DictionaryView<string, CheatCommandGroup>();
    public DictionaryView<string, ICheatCommand> Commands = new DictionaryView<string, ICheatCommand>();

    public void AddCommand(ICheatCommand InCommand, int HierarchiesIndex)
    {
        DebugHelper.Assert(InCommand != null);
        string[] groupHierarchies = InCommand.command.groupHierarchies;
        DebugHelper.Assert(groupHierarchies != null);
        if (HierarchiesIndex < groupHierarchies.Length)
        {
            CheatCommandGroup group = null;
            if (!this.ChildrenGroups.TryGetValue(groupHierarchies[HierarchiesIndex], out group))
            {
                group = new CheatCommandGroup();
                this.ChildrenGroups.Add(groupHierarchies[HierarchiesIndex], group);
            }
            DebugHelper.Assert(group != null);
            group.AddCommand(InCommand, HierarchiesIndex + 1);
        }
        else
        {
            this.Commands.Add(InCommand.command.baseName, InCommand);
        }
    }
}

