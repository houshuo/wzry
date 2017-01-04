namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class ReviveRegion : FuncRegion
    {
        [FriendlyName("仅作为出生点")]
        public bool OnlyBirth;
        public GameObject[] SubRevivePlaces = new GameObject[0];
    }
}

