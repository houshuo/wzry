namespace behaviac
{
    using System;

    public interface ISerializableNode
    {
        ISerializableNode newChild(CSerializationID chidlId);
        void setAttr(CSerializationID attrId, string value);
        void setAttr<VariableType>(CSerializationID attrId, VariableType value);
    }
}

