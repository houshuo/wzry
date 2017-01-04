namespace Assets.Scripts.GameSystem
{
    using System;

    public interface IMallSort<T>
    {
        T GetCurSortType();
        string GetSortTypeName(T sortType);
        bool IsDesc();
        void SetSortType(T sortType);
    }
}

