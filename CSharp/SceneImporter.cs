using System;
using UnityEngine;

public class SceneImporter : MonoBehaviour
{
    public LevelResAsset level;
    private static GameSerializer s_serializer = new GameSerializer();

    public void Import()
    {
        s_serializer.Load(this.level);
    }
}

