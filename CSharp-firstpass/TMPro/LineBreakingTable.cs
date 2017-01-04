namespace TMPro
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class LineBreakingTable
    {
        public Dictionary<int, char> followingCharacters = new Dictionary<int, char>();
        public Dictionary<int, char> leadingCharacters = new Dictionary<int, char>();
    }
}

