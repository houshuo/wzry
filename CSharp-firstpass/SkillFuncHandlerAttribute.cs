using System;

public class SkillFuncHandlerAttribute : Attribute, IIdentifierAttribute<int>
{
    private int[] AddFuncTypeList;
    private int SkillFuncType;

    public SkillFuncHandlerAttribute(int inSkillFuncType, params int[] inAddFuncTypeList)
    {
        this.SkillFuncType = inSkillFuncType;
        this.AddFuncTypeList = inAddFuncTypeList;
    }

    public int[] AdditionalIdList
    {
        get
        {
            return this.AddFuncTypeList;
        }
    }

    public int ID
    {
        get
        {
            return this.SkillFuncType;
        }
    }
}

