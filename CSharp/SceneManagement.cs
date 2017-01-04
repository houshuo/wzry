using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    private int invokeIndex;
    public List<Node> nodes = new List<Node>();
    public int TileCountX = 1;
    public int TileCountY = 1;
    public int TileIndexMax_X;
    public int TileIndexMax_Y;
    public VInt2 TileOrigin;
    public Tile[] tiles;
    public const int TileSize = 0x2710;

    private void AddToTile(Node node, ref Coordinate coord)
    {
        int num = (coord.Y * this.TileCountX) + coord.X;
        for (int i = 0; i < coord.NumY; i++)
        {
            for (int j = 0; j < coord.NumX; j++)
            {
                this.tiles[num + j].nodes.Add(node);
            }
            num += this.TileCountX;
        }
    }

    public void Clear()
    {
        if (this.tiles != null)
        {
            for (int i = 0; i < this.tiles.Length; i++)
            {
                this.tiles[i].nodes.Clear();
            }
        }
        this.nodes.Clear();
        this.invokeIndex = 0;
    }

    public void ForeachActors(Coordinate coord, Process proc)
    {
        if ((coord.X != -1) && (coord.Y != -1))
        {
            this.invokeIndex++;
            int num = (coord.Y * this.TileCountX) + coord.X;
            for (int i = 0; i < coord.NumY; i++)
            {
                for (int j = 0; j < coord.NumX; j++)
                {
                    List<Node> nodes = this.tiles[num + j].nodes;
                    int count = nodes.Count;
                    for (int k = 0; k < count; k++)
                    {
                        Node node = nodes[k];
                        if ((node.owner != 0) && (node.curInvokeIdx != this.invokeIndex))
                        {
                            node.curInvokeIdx = this.invokeIndex;
                            proc(ref node.owner);
                        }
                    }
                }
                num += this.TileCountX;
            }
        }
    }

    public void ForeachActorsBreak(Coordinate coord, Process_Bool proc)
    {
        if ((coord.X != -1) && (coord.Y != -1))
        {
            this.invokeIndex++;
            int num = (coord.Y * this.TileCountX) + coord.X;
            for (int i = 0; i < coord.NumY; i++)
            {
                for (int j = 0; j < coord.NumX; j++)
                {
                    List<Node> nodes = this.tiles[num + j].nodes;
                    int count = nodes.Count;
                    for (int k = 0; k < count; k++)
                    {
                        Node node = nodes[k];
                        if ((node.owner != 0) && (node.curInvokeIdx != this.invokeIndex))
                        {
                            node.curInvokeIdx = this.invokeIndex;
                            if (!proc(ref node.owner))
                            {
                                return;
                            }
                        }
                    }
                }
                num += this.TileCountX;
            }
        }
    }

    public void GetCoord(ref Coordinate coord, VCollisionShape shape)
    {
        VInt2 num;
        VInt2 num2;
        shape.GetAabb2D(out num, out num2);
        this.GetCoord(ref coord, num, num2);
    }

    public void GetCoord(ref Coordinate coord, VInt2 origin, VInt2 size)
    {
        origin -= this.TileOrigin;
        coord.X = Mathf.Clamp(origin.x / 0x2710, 0, this.TileIndexMax_X);
        coord.Y = Mathf.Clamp(origin.y / 0x2710, 0, this.TileIndexMax_Y);
        coord.NumX = Mathf.Min((int) ((((origin.x - (coord.X * 0x2710)) + size.x) / 0x2710) + 1), (int) (this.TileCountX - coord.X));
        coord.NumY = Mathf.Min((int) ((((origin.y - (coord.Y * 0x2710)) + size.y) / 0x2710) + 1), (int) (this.TileCountY - coord.Y));
    }

    public void GetCoord_Center(ref Coordinate coord, VInt2 origin, int radius)
    {
        origin.x -= radius + this.TileOrigin.x;
        origin.y -= radius + this.TileOrigin.y;
        int num = radius << 1;
        coord.X = Mathf.Clamp(origin.x / 0x2710, 0, this.TileIndexMax_X);
        coord.Y = Mathf.Clamp(origin.y / 0x2710, 0, this.TileIndexMax_Y);
        coord.NumX = Mathf.Min((int) ((((origin.x - (coord.X * 0x2710)) + num) / 0x2710) + 1), (int) (this.TileCountX - coord.X));
        coord.NumY = Mathf.Min((int) ((((origin.y - (coord.Y * 0x2710)) + num) / 0x2710) + 1), (int) (this.TileCountY - coord.Y));
    }

    public void InitScene()
    {
        AstarPath active = AstarPath.active;
        if (((active != null) && (active.astarData != null)) && (active.astarData.recastGraph != null))
        {
            Bounds forcedBounds = active.astarData.recastGraph.forcedBounds;
            VInt3 size = (VInt3) forcedBounds.size;
            this.TileCountX = (size.x / 0x2710) + (((size.x % 0x2710) <= 0) ? 0 : 1);
            this.TileCountY = (size.z / 0x2710) + (((size.z % 0x2710) <= 0) ? 0 : 1);
            this.TileIndexMax_X = this.TileCountX - 1;
            this.TileIndexMax_Y = this.TileCountY - 1;
            VInt3 min = (VInt3) forcedBounds.min;
            this.TileOrigin = min.xz;
            this.tiles = new Tile[this.TileCountX * this.TileCountY];
            for (int i = 0; i < this.tiles.Length; i++)
            {
                this.tiles[i] = new Tile();
            }
            this.nodes.Clear();
        }
    }

    private void RemoveFromTile(Node node, ref Coordinate coord)
    {
        if ((coord.X != -1) && (coord.Y != -1))
        {
            int num = (coord.Y * this.TileCountX) + coord.X;
            for (int i = 0; i < coord.NumY; i++)
            {
                for (int j = 0; j < coord.NumX; j++)
                {
                    this.tiles[num + j].nodes.Remove(node);
                }
                num += this.TileCountX;
            }
        }
    }

    public override void UnInit()
    {
        base.UnInit();
        this.Clear();
    }

    public void UpdateDirtyNodes()
    {
        int count = this.nodes.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                this.nodes[i].Update();
            }
            this.nodes.Clear();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Coordinate
    {
        public int X;
        public int Y;
        public int NumX;
        public int NumY;
        public bool Equals(ref SceneManagement.Coordinate r)
        {
            return ((((this.X == r.X) && (this.Y == r.Y)) && (this.NumX == r.NumX)) && (this.NumY == r.NumY));
        }
    }

    public class Node : PooledClassObject
    {
        public SceneManagement.Coordinate coord;
        public int curInvokeIdx;
        public PoolObjHandle<ActorRoot> owner;

        public Node()
        {
            SceneManagement.Coordinate coordinate = new SceneManagement.Coordinate {
                X = -1,
                Y = -1
            };
            this.coord = coordinate;
            this.curInvokeIdx = -1;
            base.bChkReset = false;
        }

        public void Attach()
        {
            if (((this.owner != 0) && (this.owner.handle.shape != null)) && ((this.owner.handle.TheActorMeta.ActorType != ActorTypeDef.Invalid) && !this.attached))
            {
                SceneManagement instance = Singleton<SceneManagement>.GetInstance();
                instance.GetCoord(ref this.coord, this.owner.handle.shape);
                instance.AddToTile(this, ref this.coord);
                this.dirty = false;
                this.attached = true;
            }
        }

        public void Detach()
        {
            if ((this.owner != 0) && this.attached)
            {
                SceneManagement instance = Singleton<SceneManagement>.GetInstance();
                instance.RemoveFromTile(this, ref this.coord);
                instance.nodes.Remove(this);
                this.attached = false;
                this.dirty = false;
            }
        }

        public void makeDirty()
        {
            if (!this.dirty && this.attached)
            {
                Singleton<SceneManagement>.GetInstance().nodes.Add(this);
                this.dirty = true;
            }
        }

        public override void OnRelease()
        {
            this.owner.Release();
            this.coord.X = this.coord.Y = -1;
            this.curInvokeIdx = -1;
        }

        public void Update()
        {
            if ((this.dirty && (this.owner != 0)) && (this.owner.handle.shape != null))
            {
                SceneManagement instance = Singleton<SceneManagement>.instance;
                SceneManagement.Coordinate coord = new SceneManagement.Coordinate();
                instance.GetCoord(ref coord, this.owner.handle.shape);
                if (!this.coord.Equals(ref coord))
                {
                    instance.RemoveFromTile(this, ref this.coord);
                    this.coord = coord;
                    instance.AddToTile(this, ref this.coord);
                }
                this.dirty = false;
            }
        }

        public bool attached { get; private set; }

        public bool dirty { get; private set; }
    }

    public delegate void Process(ref PoolObjHandle<ActorRoot> actor);

    public delegate bool Process_Bool(ref PoolObjHandle<ActorRoot> actor);

    public class Tile
    {
        public List<SceneManagement.Node> nodes = new List<SceneManagement.Node>(0x20);
    }
}

