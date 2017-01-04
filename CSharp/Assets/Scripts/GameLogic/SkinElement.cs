namespace Assets.Scripts.GameLogic
{
    using System;

    [Serializable]
    public class SkinElement
    {
        public AdvanceSkinElement[] AdvanceSkin = new AdvanceSkinElement[0];
        public string[] ArtSkinLobbyShowLOD = new string[2];
        public string[] ArtSkinPrefabLOD = new string[3];
    }
}

