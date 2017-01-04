using Assets.Scripts.Framework;
using Pathfinding;
using Pathfinding.Serialization;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MTileHandlerHelper : MonoBehaviour
{
    public const int CampCount = 3;
    private List<Bounds> forcedReloadBounds = new List<Bounds>();
    private SGameTileHandler[] handlers;
    public static MTileHandlerHelper Instance;
    private ListView<RecastGraph> recastGraphs = new ListView<RecastGraph>();

    private void Awake()
    {
        Instance = this;
    }

    private void CreateHandlers(ListView<NavmeshCut> cuts)
    {
        if (this.handlers == null)
        {
            AstarPath active = AstarPath.active;
            if (((active != null) && (active.astarData != null)) && (active.astarData.recastGraph != null))
            {
                bool flag = false;
                for (int i = 0; i < cuts.Count; i++)
                {
                    if (cuts[i].campIndex != -1)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    if (active.astarDataArray == null)
                    {
                        active.astarDataArray = new AstarData[3];
                        for (int k = 0; k < 3; k++)
                        {
                            AstarData data;
                            data = new AstarData {
                                DataGroupIndex = k + 1,
                                recastGraph = active.astarData.recastGraph.Clone(data),
                                userConnections = new UserConnection[0]
                            };
                            data.graphs = new NavGraph[] { data.recastGraph };
                            active.astarDataArray[k] = data;
                        }
                    }
                    this.handlers = new SGameTileHandler[3];
                    for (int j = 0; j < 3; j++)
                    {
                        AstarData data2 = active.astarDataArray[j];
                        this.handlers[j] = new SGameTileHandler(data2.recastGraph);
                        this.handlers[j].CreateTileTypesFromGraph();
                    }
                }
                else
                {
                    this.handlers = new SGameTileHandler[] { new SGameTileHandler(active.astarData.recastGraph) };
                    this.handlers[0].CreateTileTypesFromGraph();
                }
            }
        }
    }

    public void DiscardPending()
    {
        ListView<NavmeshCut> all = NavmeshCut.GetAll();
        for (int i = 0; i < all.Count; i++)
        {
            if (all[i].RequiresUpdate())
            {
                all[i].NotifyUpdated();
            }
        }
    }

    private void HandleOnDestroyCallback(NavmeshCut obj)
    {
        this.forcedReloadBounds.Add(obj.LastBounds);
    }

    private void OnDestroy()
    {
        Instance = null;
        this.handlers = null;
    }

    private void OnDisable()
    {
        NavmeshCut.OnDestroyCallback -= new Action<NavmeshCut>(this.HandleOnDestroyCallback);
    }

    private void OnEnable()
    {
        NavmeshCut.OnDestroyCallback += new Action<NavmeshCut>(this.HandleOnDestroyCallback);
    }

    private void Rebuild2()
    {
        ListView<NavmeshCut> all = NavmeshCut.GetAll();
        ListView<NavmeshCut> navmeshCuts = new ListView<NavmeshCut>();
        this.CreateHandlers(all);
        if (this.handlers != null)
        {
            int num = AstarPath.active.astarData.graphs.Length + 1;
            for (int i = 0; i < all.Count; i++)
            {
                all[i].Check();
            }
            for (int j = 0; j < this.handlers.Length; j++)
            {
                navmeshCuts.Clear();
                for (int m = 0; m < all.Count; m++)
                {
                    NavmeshCut item = all[m];
                    if ((item.campIndex != j) && item.enabled)
                    {
                        navmeshCuts.Add(item);
                    }
                }
                this.handlers[j].ReloadTiles(navmeshCuts);
                this.handlers[j].graph.astarData.RasterizeGraphNodes();
            }
            for (int k = 0; k < all.Count; k++)
            {
                if (all[k].RequiresUpdate())
                {
                    all[k].NotifyUpdated();
                }
            }
            this.forcedReloadBounds.Clear();
        }
    }

    private void SaveNavData(int campIndex)
    {
        if (((AstarPath.active != null) && (AstarPath.active.astarDataArray != null)) && ((campIndex >= 0) && (campIndex < AstarPath.active.astarDataArray.Length)))
        {
            string str;
            GameReplayModule instance = Singleton<GameReplayModule>.GetInstance();
            string streamPath = instance.streamPath;
            if (instance.isReplay)
            {
                streamPath = string.Format("{0}/{1}_sgame_debug.txt", DebugHelper.logRootPath, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                int index = streamPath.IndexOf("_sgame_debug");
                if (index != -1)
                {
                    streamPath = streamPath.Substring(0, index);
                }
            }
            if (streamPath == null)
            {
                string path = GameReplayModule.ReplayFolder + "/NavData";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                str = path + "/NAV";
            }
            else
            {
                int length = streamPath.LastIndexOf('.');
                if (length != -1)
                {
                    str = streamPath.Substring(0, length);
                }
                else
                {
                    str = streamPath;
                }
                ulong curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
                if (!Singleton<BattleLogic>.instance.isFighting)
                {
                    curFrameNum = 0L;
                }
                str = str + "_" + curFrameNum;
            }
            str = str + "_camp" + campIndex;
            string str4 = str;
            int num4 = 1;
            while (File.Exists(str4 + ".nav"))
            {
                str4 = str;
                str4 = (str4 + " (") + ((string) num4++) + ")";
            }
            this.SaveNavData(str4 + ".nav", AstarPath.active.astarDataArray[campIndex]);
        }
    }

    private void SaveNavData(string file, AstarData astarData)
    {
        if (astarData != null)
        {
            byte[] array = astarData.SerializeGraphsExtra(new SerializeSettings());
            FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write);
            stream.Write(array, 0, array.Length);
            stream.Close();
        }
    }

    private bool ShouldRebuildNav()
    {
        ListView<NavmeshCut> all = NavmeshCut.GetAll();
        if (this.forcedReloadBounds.Count != 0)
        {
            return true;
        }
        for (int i = 0; i < all.Count; i++)
        {
            if (all[i].RequiresUpdate())
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateLogic()
    {
        if (this.ShouldRebuildNav())
        {
            this.Rebuild2();
        }
    }
}

