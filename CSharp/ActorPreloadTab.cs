using Assets.Scripts.GameLogic.DataCenter;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ActorPreloadTab
{
    public List<AssetLoadBase> ageActions;
    public List<AssetLoadBase> behaviorXml;
    public int MarkID;
    public List<AssetLoadBase> mesPrefabs;
    public AssetLoadBase modelPrefab;
    public List<AssetLoadBase> parPrefabs;
    public List<AssetLoadBase> soundBanks;
    public float spawnCnt;
    public List<AssetLoadBase> spritePrefabs;
    public ActorMeta theActor;

    public void AddAction(string path)
    {
        AssetLoadBase item = new AssetLoadBase {
            assetPath = path
        };
        this.ageActions.Add(item);
    }

    public void AddMesh(string path)
    {
        AssetLoadBase item = new AssetLoadBase {
            assetPath = path
        };
        this.mesPrefabs.Add(item);
    }

    public void AddParticle(string path)
    {
        AssetLoadBase item = new AssetLoadBase {
            assetPath = path
        };
        this.parPrefabs.Add(item);
    }

    public void AddSprite(string path)
    {
        AssetLoadBase item = new AssetLoadBase {
            assetPath = path
        };
        this.spritePrefabs.Add(item);
    }

    public bool IsExistsSprite(string path)
    {
        <IsExistsSprite>c__AnonStorey66 storey = new <IsExistsSprite>c__AnonStorey66 {
            path = path
        };
        return this.spritePrefabs.Exists(new Predicate<AssetLoadBase>(storey.<>m__43));
    }

    [CompilerGenerated]
    private sealed class <IsExistsSprite>c__AnonStorey66
    {
        internal string path;

        internal bool <>m__43(AssetLoadBase info)
        {
            return (info.assetPath == this.path);
        }
    }
}

