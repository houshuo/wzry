namespace Pathfinding
{
    using Pathfinding.Serialization;
    using Pathfinding.Serialization.JsonFx;
    using Pathfinding.Util;
    using Pathfinding.Voxels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, JsonOptIn]
    public class RecastGraph : NavGraph, IUpdatableGraph, IRaycastableGraph, INavmesh, INavmeshHolder
    {
        private BBTree _bbTree;
        private VInt3[] _vertices;
        private bool batchTileUpdate;
        private List<int> batchUpdatedTiles = new List<int>();
        public const int BorderVertexMask = 1;
        public const int BorderVertexOffset = 0x1f;
        private readonly int[] BoxColliderTris = new int[] { 
            0, 1, 2, 0, 2, 3, 6, 5, 4, 7, 6, 4, 0, 5, 1, 0, 
            4, 5, 1, 6, 2, 1, 5, 6, 2, 7, 3, 2, 6, 7, 3, 4, 
            0, 3, 7, 4
         };
        private readonly Vector3[] BoxColliderVerts = new Vector3[] { new Vector3(-1f, -1f, -1f), new Vector3(1f, -1f, -1f), new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f), new Vector3(-1f, 1f, -1f), new Vector3(1f, 1f, -1f), new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f) };
        private Dictionary<VInt2, int> cachedInt2_int_dict = new Dictionary<VInt2, int>();
        private Dictionary<VInt3, int> cachedInt3_int_dict = new Dictionary<VInt3, int>();
        private List<CapsuleCache> capsuleCache = new List<CapsuleCache>();
        [JsonMember]
        public float cellHeight = 0.4f;
        [JsonMember]
        public float cellSize = 0.5f;
        [JsonMember]
        public float characterRadius = 0.5f;
        [JsonMember]
        public float colliderRasterizeDetail = 10f;
        [JsonMember]
        public float contourMaxError = 2f;
        private AstarData dataOverride;
        public bool dynamic = true;
        [JsonMember]
        public int editorTileSize = 0x80;
        [JsonMember]
        public Vector3 forcedBoundsCenter;
        [JsonMember]
        public Vector3 forcedBoundsSize = new Vector3(100f, 40f, 100f);
        private Voxelize globalVox;
        [JsonMember]
        public LayerMask mask = -1;
        [JsonMember]
        public float maxEdgeLength = 20f;
        [JsonMember]
        public float maxSlope = 30f;
        [JsonMember]
        public float minRegionSize = 3f;
        [JsonMember]
        public bool nearestSearchOnlyXZ;
        [JsonMember]
        public bool rasterizeColliders;
        [JsonMember]
        public bool rasterizeMeshes = true;
        [JsonMember]
        public bool rasterizeTerrain = true;
        [JsonMember]
        public bool rasterizeTrees = true;
        [JsonMember]
        public RelevantGraphSurfaceMode relevantGraphSurfaceMode;
        public bool scanEmptyGraph;
        [JsonMember]
        public bool showMeshOutline = true;
        [JsonMember]
        public bool showNodeConnections;
        [JsonMember]
        public List<string> tagMask = new List<string>();
        [JsonMember]
        public int terrainSampleSize = 3;
        public const int TileIndexMask = 0x7ffff;
        public const int TileIndexOffset = 12;
        private NavmeshTile[] tiles;
        [JsonMember]
        public int tileSizeX = 0x80;
        [JsonMember]
        public int tileSizeZ = 0x80;
        public int tileXCount;
        public int tileZCount;
        [JsonMember]
        public bool useTiles;
        public const int VertexIndexMask = 0xfff;
        [JsonMember]
        public float walkableClimb = 0.5f;
        [JsonMember]
        public float walkableHeight = 2f;

        protected void BuildTileMesh(Voxelize vox, int x, int z)
        {
            VoxelMesh mesh;
            float num = this.tileSizeX * this.cellSize;
            float num2 = this.tileSizeZ * this.cellSize;
            int radius = Mathf.CeilToInt(this.characterRadius / this.cellSize);
            Vector3 min = this.forcedBounds.min;
            Vector3 max = this.forcedBounds.max;
            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(x * num, 0f, z * num2) + min, new Vector3(((x + 1) * num) + min.x, max.y, ((z + 1) * num2) + min.z));
            vox.borderSize = radius + 3;
            bounds.Expand((Vector3) ((new Vector3((float) vox.borderSize, 0f, (float) vox.borderSize) * this.cellSize) * 2f));
            vox.forcedBounds = bounds;
            vox.width = this.tileSizeX + (vox.borderSize * 2);
            vox.depth = this.tileSizeZ + (vox.borderSize * 2);
            if (!this.useTiles && (this.relevantGraphSurfaceMode == RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile))
            {
                vox.relevantGraphSurfaceMode = RelevantGraphSurfaceMode.RequireForAll;
            }
            else
            {
                vox.relevantGraphSurfaceMode = this.relevantGraphSurfaceMode;
            }
            vox.minRegionSize = Mathf.RoundToInt(this.minRegionSize / (this.cellSize * this.cellSize));
            vox.Init();
            vox.CollectMeshes();
            vox.VoxelizeInput();
            vox.FilterLedges(vox.voxelWalkableHeight, vox.voxelWalkableClimb, vox.cellSize, vox.cellHeight, vox.forcedBounds.min);
            vox.FilterLowHeightSpans(vox.voxelWalkableHeight, vox.cellSize, vox.cellHeight, vox.forcedBounds.min);
            vox.BuildCompactField();
            vox.BuildVoxelConnections();
            vox.ErodeWalkableArea(radius);
            vox.BuildDistanceField();
            vox.BuildRegions();
            VoxelContourSet cset = new VoxelContourSet();
            vox.BuildContours(this.contourMaxError, 1, cset, 1);
            vox.BuildPolyMesh(cset, 3, out mesh);
            for (int i = 0; i < mesh.verts.Length; i++)
            {
                mesh.verts[i] = ((mesh.verts[i] * 0x3e8) * vox.cellScale) + ((VInt3) vox.voxelOffset);
            }
            NavmeshTile tile = this.CreateTile(vox, mesh, x, z);
            this.tiles[tile.x + (tile.z * this.tileXCount)] = tile;
        }

        public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
        {
            if (o.updatePhysics)
            {
                return GraphUpdateThreading.SeparateAndUnityInit;
            }
            return GraphUpdateThreading.SeparateThread;
        }

        public RecastGraph Clone(AstarData owner)
        {
            RecastGraph graph = new RecastGraph {
                dataOverride = owner
            };
            int length = owner.graphs.Length;
            TriangleMeshNode.SetNavmeshHolder(owner.DataGroupIndex, length, graph);
            this.Duplicate(graph);
            return graph;
        }

        public Vector3 ClosestPointOnNode(TriangleMeshNode node, Vector3 pos)
        {
            return Polygon.ClosestPointOnTriangle((Vector3) this.GetVertex(node.v0), (Vector3) this.GetVertex(node.v1), (Vector3) this.GetVertex(node.v2), pos);
        }

        private void CollectColliderMeshes(Bounds bounds, List<ExtraMesh> extraMeshes)
        {
            foreach (Collider collider in UnityEngine.Object.FindObjectsOfType(typeof(Collider)) as Collider[])
            {
                if (((((((int) 1) << collider.gameObject.layer) & this.mask) != 0) && collider.enabled) && (!collider.isTrigger && collider.bounds.Intersects(bounds)))
                {
                    ExtraMesh item = this.RasterizeCollider(collider);
                    if (item.vertices != null)
                    {
                        extraMeshes.Add(item);
                    }
                }
            }
            this.capsuleCache.Clear();
        }

        private bool CollectMeshes(out List<ExtraMesh> extraMeshes, Bounds bounds)
        {
            extraMeshes = new List<ExtraMesh>();
            if (this.rasterizeMeshes)
            {
                GetSceneMeshes(bounds, this.tagMask, this.mask, extraMeshes);
            }
            this.GetRecastMeshObjs(bounds, extraMeshes);
            if (this.rasterizeTerrain)
            {
                this.CollectTerrainMeshes(bounds, this.rasterizeTrees, extraMeshes);
            }
            if (this.rasterizeColliders)
            {
                this.CollectColliderMeshes(bounds, extraMeshes);
            }
            if (extraMeshes.Count == 0)
            {
                Debug.LogWarning("No MeshFilters where found contained in the layers specified by the 'mask' variables");
                return false;
            }
            return true;
        }

        private void CollectTerrainMeshes(Bounds bounds, bool rasterizeTrees, List<ExtraMesh> extraMeshes)
        {
            Terrain[] terrainArray = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
            if (terrainArray.Length > 0)
            {
                for (int i = 0; i < terrainArray.Length; i++)
                {
                    TerrainData terrainData = terrainArray[i].terrainData;
                    if (terrainData != null)
                    {
                        Vector3 position = terrainArray[i].GetPosition();
                        Vector3 center = position + ((Vector3) (terrainData.size * 0.5f));
                        Bounds b = new Bounds(center, terrainData.size);
                        if (b.Intersects(bounds))
                        {
                            float[,] numArray = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
                            this.terrainSampleSize = (this.terrainSampleSize >= 1) ? this.terrainSampleSize : 1;
                            int heightmapWidth = terrainData.heightmapWidth;
                            int heightmapHeight = terrainData.heightmapHeight;
                            int num4 = (((terrainData.heightmapWidth + this.terrainSampleSize) - 1) / this.terrainSampleSize) + 1;
                            int num5 = (((terrainData.heightmapHeight + this.terrainSampleSize) - 1) / this.terrainSampleSize) + 1;
                            Vector3[] v = new Vector3[num4 * num5];
                            Vector3 heightmapScale = terrainData.heightmapScale;
                            float y = terrainData.size.y;
                            int num7 = 0;
                            for (int j = 0; j < num5; j++)
                            {
                                int num9 = 0;
                                for (int m = 0; m < num4; m++)
                                {
                                    int num11 = Math.Min(num9, heightmapWidth - 1);
                                    int num12 = Math.Min(num7, heightmapHeight - 1);
                                    v[(j * num4) + m] = new Vector3(num12 * heightmapScale.z, numArray[num11, num12] * y, num11 * heightmapScale.x) + position;
                                    num9 += this.terrainSampleSize;
                                }
                                num7 += this.terrainSampleSize;
                            }
                            int[] t = new int[(((num4 - 1) * (num5 - 1)) * 2) * 3];
                            int index = 0;
                            for (int k = 0; k < (num5 - 1); k++)
                            {
                                for (int n = 0; n < (num4 - 1); n++)
                                {
                                    t[index] = (k * num4) + n;
                                    t[index + 1] = ((k * num4) + n) + 1;
                                    t[index + 2] = (((k + 1) * num4) + n) + 1;
                                    index += 3;
                                    t[index] = (k * num4) + n;
                                    t[index + 1] = (((k + 1) * num4) + n) + 1;
                                    t[index + 2] = ((k + 1) * num4) + n;
                                    index += 3;
                                }
                            }
                            extraMeshes.Add(new ExtraMesh(v, t, b));
                            if (rasterizeTrees)
                            {
                                this.CollectTreeMeshes(terrainArray[i], extraMeshes);
                            }
                        }
                    }
                }
            }
        }

        private void CollectTreeMeshes(Terrain terrain, List<ExtraMesh> extraMeshes)
        {
            TerrainData terrainData = terrain.terrainData;
            for (int i = 0; i < terrainData.treeInstances.Length; i++)
            {
                TreeInstance instance = terrainData.treeInstances[i];
                TreePrototype prototype = terrainData.treePrototypes[instance.prototypeIndex];
                if (prototype.prefab.GetComponent<Collider>() == null)
                {
                    Bounds b = new Bounds(terrain.transform.position + Vector3.Scale(instance.position, terrainData.size), new Vector3(instance.widthScale, instance.heightScale, instance.widthScale));
                    Matrix4x4 matrix = Matrix4x4.TRS(terrain.transform.position + Vector3.Scale(instance.position, terrainData.size), Quaternion.identity, (Vector3) (new Vector3(instance.widthScale, instance.heightScale, instance.widthScale) * 0.5f));
                    ExtraMesh item = new ExtraMesh(this.BoxColliderVerts, this.BoxColliderTris, b, matrix);
                    extraMeshes.Add(item);
                }
                else
                {
                    Vector3 pos = terrain.transform.position + Vector3.Scale(instance.position, terrainData.size);
                    Vector3 s = new Vector3(instance.widthScale, instance.heightScale, instance.widthScale);
                    ExtraMesh mesh2 = this.RasterizeCollider(prototype.prefab.GetComponent<Collider>(), Matrix4x4.TRS(pos, Quaternion.identity, s));
                    if (mesh2.vertices != null)
                    {
                        mesh2.RecalculateBounds();
                        extraMeshes.Add(mesh2);
                    }
                }
            }
        }

        private void ConnectTiles(NavmeshTile tile1, NavmeshTile tile2)
        {
            if ((tile1 != null) && (tile2 != null))
            {
                int num5;
                int num6;
                int num7;
                int num8;
                float num9;
                if (tile1.nodes == null)
                {
                    throw new ArgumentException("tile1 does not contain any nodes");
                }
                if (tile2.nodes == null)
                {
                    throw new ArgumentException("tile2 does not contain any nodes");
                }
                int num = Mathf.Clamp(tile2.x, tile1.x, (tile1.x + tile1.w) - 1);
                int num2 = Mathf.Clamp(tile1.x, tile2.x, (tile2.x + tile2.w) - 1);
                int num3 = Mathf.Clamp(tile2.z, tile1.z, (tile1.z + tile1.d) - 1);
                int num4 = Mathf.Clamp(tile1.z, tile2.z, (tile2.z + tile2.d) - 1);
                if (num == num2)
                {
                    num5 = 2;
                    num6 = 0;
                    num7 = num3;
                    num8 = num4;
                    num9 = this.tileSizeZ * this.cellSize;
                }
                else
                {
                    if (num3 != num4)
                    {
                        throw new ArgumentException("Tiles are not adjacent (neither x or z coordinates match)");
                    }
                    num5 = 0;
                    num6 = 2;
                    num7 = num;
                    num8 = num2;
                    num9 = this.tileSizeX * this.cellSize;
                }
                if (Math.Abs((int) (num7 - num8)) != 1)
                {
                    Debug.Log(string.Concat(new object[] { 
                        tile1.x, " ", tile1.z, " ", tile1.w, " ", tile1.d, "\n", tile2.x, " ", tile2.z, " ", tile2.w, " ", tile2.d, "\n", 
                        num, " ", num3, " ", num2, " ", num4
                     }));
                    object[] objArray2 = new object[] { "Tiles are not adjacent (tile coordinates must differ by exactly 1. Got '", num7, "' and '", num8, "')" };
                    throw new ArgumentException(string.Concat(objArray2));
                }
                int num10 = (int) Math.Round((double) (((Math.Max(num7, num8) * num9) + this.forcedBounds.min[num5]) * 1000f));
                TriangleMeshNode[] nodes = tile1.nodes;
                TriangleMeshNode[] nodeArray2 = tile2.nodes;
                for (int i = 0; i < nodes.Length; i++)
                {
                    TriangleMeshNode node = nodes[i];
                    int vertexCount = node.GetVertexCount();
                    for (int j = 0; j < vertexCount; j++)
                    {
                        VInt3 vertex = node.GetVertex(j);
                        VInt3 num15 = node.GetVertex((j + 1) % vertexCount);
                        if ((Math.Abs((int) (vertex[num5] - num10)) < 2) && (Math.Abs((int) (num15[num5] - num10)) < 2))
                        {
                            int num16 = Math.Min(vertex[num6], num15[num6]);
                            int num17 = Math.Max(vertex[num6], num15[num6]);
                            if (num16 != num17)
                            {
                                for (int k = 0; k < nodeArray2.Length; k++)
                                {
                                    TriangleMeshNode node2 = nodeArray2[k];
                                    int num19 = node2.GetVertexCount();
                                    for (int m = 0; m < num19; m++)
                                    {
                                        VInt3 num21 = node2.GetVertex(m);
                                        VInt3 num22 = node2.GetVertex((m + 1) % vertexCount);
                                        if ((Math.Abs((int) (num21[num5] - num10)) < 2) && (Math.Abs((int) (num22[num5] - num10)) < 2))
                                        {
                                            int num23 = Math.Min(num21[num6], num22[num6]);
                                            int num24 = Math.Max(num21[num6], num22[num6]);
                                            if (((num23 != num24) && ((num17 > num23) && (num16 < num24))) && ((((vertex == num21) && (num15 == num22)) || ((vertex == num22) && (num15 == num21))) || (Polygon.DistanceSegmentSegment3D((Vector3) vertex, (Vector3) num15, (Vector3) num21, (Vector3) num22) < (this.walkableClimb * this.walkableClimb))))
                                            {
                                                VInt3 num26 = node.position - node2.position;
                                                uint costMagnitude = (uint) num26.costMagnitude;
                                                node.AddConnection(node2, costMagnitude);
                                                node2.AddConnection(node, costMagnitude);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ConnectTileWithNeighbours(NavmeshTile tile)
        {
            if (tile.x > 0)
            {
                int num = tile.x - 1;
                for (int i = tile.z; i < (tile.z + tile.d); i++)
                {
                    this.ConnectTiles(this.tiles[num + (i * this.tileXCount)], tile);
                }
            }
            if ((tile.x + tile.w) < this.tileXCount)
            {
                int num3 = tile.x + tile.w;
                for (int j = tile.z; j < (tile.z + tile.d); j++)
                {
                    this.ConnectTiles(this.tiles[num3 + (j * this.tileXCount)], tile);
                }
            }
            if (tile.z > 0)
            {
                int num5 = tile.z - 1;
                for (int k = tile.x; k < (tile.x + tile.w); k++)
                {
                    this.ConnectTiles(this.tiles[k + (num5 * this.tileXCount)], tile);
                }
            }
            if ((tile.z + tile.d) < this.tileZCount)
            {
                int num7 = tile.z + tile.d;
                for (int m = tile.x; m < (tile.x + tile.w); m++)
                {
                    this.ConnectTiles(this.tiles[m + (num7 * this.tileXCount)], tile);
                }
            }
        }

        public bool ContainsPoint(TriangleMeshNode node, Vector3 pos)
        {
            return ((Polygon.IsClockwise((Vector3) this.GetVertex(node.v0), (Vector3) this.GetVertex(node.v1), pos) && Polygon.IsClockwise((Vector3) this.GetVertex(node.v1), (Vector3) this.GetVertex(node.v2), pos)) && Polygon.IsClockwise((Vector3) this.GetVertex(node.v2), (Vector3) this.GetVertex(node.v0), pos));
        }

        private void CreateNodeConnections(TriangleMeshNode[] nodes)
        {
            ListLinqView<MeshNode> view = new ListLinqView<MeshNode>();
            List<uint> list = ListPool<uint>.Claim();
            Dictionary<VInt2, int> dictionary = this.cachedInt2_int_dict;
            dictionary.Clear();
            for (int i = 0; i < nodes.Length; i++)
            {
                TriangleMeshNode node = nodes[i];
                int vertexCount = node.GetVertexCount();
                for (int k = 0; k < vertexCount; k++)
                {
                    try
                    {
                        dictionary.Add(new VInt2(node.GetVertexIndex(k), node.GetVertexIndex((k + 1) % vertexCount)), i);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            for (int j = 0; j < nodes.Length; j++)
            {
                TriangleMeshNode node2 = nodes[j];
                view.Clear();
                list.Clear();
                int num5 = node2.GetVertexCount();
                for (int m = 0; m < num5; m++)
                {
                    int num9;
                    int vertexIndex = node2.GetVertexIndex(m);
                    int x = node2.GetVertexIndex((m + 1) % num5);
                    if (dictionary.TryGetValue(new VInt2(x, vertexIndex), out num9))
                    {
                        TriangleMeshNode item = nodes[num9];
                        int num10 = item.GetVertexCount();
                        for (int n = 0; n < num10; n++)
                        {
                            if ((item.GetVertexIndex(n) == x) && (item.GetVertexIndex((n + 1) % num10) == vertexIndex))
                            {
                                VInt3 num13 = node2.position - item.position;
                                uint costMagnitude = (uint) num13.costMagnitude;
                                view.Add(item);
                                list.Add(costMagnitude);
                                break;
                            }
                        }
                    }
                }
                node2.connections = view.ToArray();
                node2.connectionCosts = list.ToArray();
            }
            ListPool<uint>.Release(list);
        }

        public override void CreateNodes(int number)
        {
            throw new NotSupportedException();
        }

        private NavmeshTile CreateTile(Voxelize vox, VoxelMesh mesh, int x, int z)
        {
            NavmeshTile tile;
            if (mesh.tris == null)
            {
                throw new ArgumentNullException("The mesh must be valid. tris is null.");
            }
            if (mesh.verts == null)
            {
                throw new ArgumentNullException("The mesh must be valid. verts is null.");
            }
            tile = new NavmeshTile {
                x = x,
                z = z,
                w = 1,
                d = 1,
                tris = mesh.tris,
                verts = mesh.verts,
                bbTree = new BBTree(tile)
            };
            if ((tile.tris.Length % 3) != 0)
            {
                throw new ArgumentException("Indices array's length must be a multiple of 3 (mesh.tris)");
            }
            if (tile.verts.Length >= 0xfff)
            {
                throw new ArgumentException("Too many vertices per tile (more than " + 0xfff + ").\nTry enabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector");
            }
            Dictionary<VInt3, int> dictionary = this.cachedInt3_int_dict;
            dictionary.Clear();
            int[] numArray = new int[tile.verts.Length];
            int num = 0;
            for (int i = 0; i < tile.verts.Length; i++)
            {
                try
                {
                    dictionary.Add(tile.verts[i], num);
                    numArray[i] = num;
                    tile.verts[num] = tile.verts[i];
                    num++;
                }
                catch
                {
                    numArray[i] = dictionary[tile.verts[i]];
                }
            }
            for (int j = 0; j < tile.tris.Length; j++)
            {
                tile.tris[j] = numArray[tile.tris[j]];
            }
            VInt3[] numArray2 = new VInt3[num];
            for (int k = 0; k < num; k++)
            {
                numArray2[k] = tile.verts[k];
            }
            tile.verts = numArray2;
            TriangleMeshNode[] nodeArray = new TriangleMeshNode[tile.tris.Length / 3];
            tile.nodes = nodeArray;
            int length = this.astarData.graphs.Length;
            TriangleMeshNode.SetNavmeshHolder(0, length, tile);
            int num6 = x + (z * this.tileXCount);
            num6 = num6 << 12;
            for (int m = 0; m < nodeArray.Length; m++)
            {
                TriangleMeshNode node = new TriangleMeshNode(base.active);
                nodeArray[m] = node;
                node.GraphIndex = (uint) length;
                node.v0 = tile.tris[m * 3] | num6;
                node.v1 = tile.tris[(m * 3) + 1] | num6;
                node.v2 = tile.tris[(m * 3) + 2] | num6;
                if (!Polygon.IsClockwise(node.GetVertex(0), node.GetVertex(1), node.GetVertex(2)))
                {
                    int num8 = node.v0;
                    node.v0 = node.v2;
                    node.v2 = num8;
                }
                node.Walkable = true;
                node.Penalty = base.initialPenalty;
                node.UpdatePositionFromVertices();
                tile.bbTree.Insert(node);
            }
            this.CreateNodeConnections(tile.nodes);
            TriangleMeshNode.SetNavmeshHolder(0, length, null);
            return tile;
        }

        public override void DeserializeExtraInfo(GraphSerializationContext ctx)
        {
            BinaryReader reader = ctx.reader;
            this.tileXCount = reader.ReadInt32();
            if (this.tileXCount >= 0)
            {
                this.tileZCount = reader.ReadInt32();
                this.tiles = new NavmeshTile[this.tileXCount * this.tileZCount];
                TriangleMeshNode.SetNavmeshHolder(0, ctx.graphIndex, this);
                for (int i = 0; i < this.tileZCount; i++)
                {
                    for (int j = 0; j < this.tileXCount; j++)
                    {
                        int index = j + (i * this.tileXCount);
                        int num4 = reader.ReadInt32();
                        if (num4 < 0)
                        {
                            throw new Exception("Invalid tile coordinates (x < 0)");
                        }
                        int num5 = reader.ReadInt32();
                        if (num5 < 0)
                        {
                            throw new Exception("Invalid tile coordinates (z < 0)");
                        }
                        if ((num4 != j) || (num5 != i))
                        {
                            this.tiles[index] = this.tiles[(num5 * this.tileXCount) + num4];
                        }
                        else
                        {
                            NavmeshTile tile;
                            tile = new NavmeshTile {
                                x = num4,
                                z = num5,
                                w = reader.ReadInt32(),
                                d = reader.ReadInt32(),
                                bbTree = new BBTree(tile)
                            };
                            this.tiles[index] = tile;
                            int num6 = reader.ReadInt32();
                            if ((num6 % 3) != 0)
                            {
                                throw new Exception("Corrupt data. Triangle indices count must be divisable by 3. Got " + num6);
                            }
                            tile.tris = new int[num6];
                            for (int k = 0; k < tile.tris.Length; k++)
                            {
                                tile.tris[k] = reader.ReadInt32();
                            }
                            tile.verts = new VInt3[reader.ReadInt32()];
                            for (int m = 0; m < tile.verts.Length; m++)
                            {
                                tile.verts[m] = new VInt3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            }
                            int num9 = reader.ReadInt32();
                            tile.nodes = new TriangleMeshNode[num9];
                            index = index << 12;
                            for (int n = 0; n < tile.nodes.Length; n++)
                            {
                                TriangleMeshNode node = new TriangleMeshNode(base.active);
                                tile.nodes[n] = node;
                                node.GraphIndex = (uint) ctx.graphIndex;
                                node.DeserializeNode(ctx);
                                node.v0 = tile.tris[n * 3] | index;
                                node.v1 = tile.tris[(n * 3) + 1] | index;
                                node.v2 = tile.tris[(n * 3) + 2] | index;
                                node.UpdatePositionFromVertices();
                                tile.bbTree.Insert(node);
                            }
                        }
                    }
                }
            }
        }

        public override void DeserializeSettings(GraphSerializationContext ctx)
        {
            base.DeserializeSettings(ctx);
            this.characterRadius = ctx.reader.ReadSingle();
            this.contourMaxError = ctx.reader.ReadSingle();
            this.cellSize = ctx.reader.ReadSingle();
            this.cellHeight = ctx.reader.ReadSingle();
            this.walkableHeight = ctx.reader.ReadSingle();
            this.maxSlope = ctx.reader.ReadSingle();
            this.maxEdgeLength = ctx.reader.ReadSingle();
            this.editorTileSize = ctx.reader.ReadInt32();
            this.tileSizeX = ctx.reader.ReadInt32();
            this.nearestSearchOnlyXZ = ctx.reader.ReadBoolean();
            this.useTiles = ctx.reader.ReadBoolean();
            this.relevantGraphSurfaceMode = (RelevantGraphSurfaceMode) ctx.reader.ReadInt32();
            this.rasterizeColliders = ctx.reader.ReadBoolean();
            this.rasterizeMeshes = ctx.reader.ReadBoolean();
            this.rasterizeTerrain = ctx.reader.ReadBoolean();
            this.rasterizeTrees = ctx.reader.ReadBoolean();
            this.colliderRasterizeDetail = ctx.reader.ReadSingle();
            this.forcedBoundsCenter = ctx.DeserializeVector3();
            this.forcedBoundsSize = ctx.DeserializeVector3();
            this.mask = ctx.reader.ReadInt32();
            if (!this.useTiles)
            {
                this.tileSizeX = (int) ((this.forcedBoundsSize.x / this.cellSize) + 0.5f);
                this.tileSizeZ = (int) ((this.forcedBoundsSize.z / this.cellSize) + 0.5f);
            }
            else
            {
                this.tileSizeX = this.editorTileSize;
                this.tileSizeZ = this.editorTileSize;
            }
            int capacity = ctx.reader.ReadInt32();
            this.tagMask = new List<string>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this.tagMask.Add(ctx.reader.ReadString());
            }
            this.showMeshOutline = ctx.reader.ReadBoolean();
            this.showNodeConnections = ctx.reader.ReadBoolean();
            this.terrainSampleSize = ctx.reader.ReadInt32();
        }

        protected override void Duplicate(NavGraph graph)
        {
            base.Duplicate(graph);
            RecastGraph owner = (RecastGraph) graph;
            owner.dynamic = this.dynamic;
            owner.characterRadius = this.characterRadius;
            owner.contourMaxError = this.contourMaxError;
            owner.cellSize = this.cellSize;
            owner.cellHeight = this.cellHeight;
            owner.walkableHeight = this.walkableHeight;
            owner.walkableClimb = this.walkableClimb;
            owner.maxSlope = this.maxSlope;
            owner.maxEdgeLength = this.maxEdgeLength;
            owner.minRegionSize = this.minRegionSize;
            owner.editorTileSize = this.editorTileSize;
            owner.tileSizeX = this.tileSizeX;
            owner.tileSizeZ = this.tileSizeZ;
            owner.nearestSearchOnlyXZ = this.nearestSearchOnlyXZ;
            owner.useTiles = this.useTiles;
            owner.scanEmptyGraph = this.scanEmptyGraph;
            owner.relevantGraphSurfaceMode = this.relevantGraphSurfaceMode;
            owner.rasterizeColliders = this.rasterizeColliders;
            owner.rasterizeMeshes = this.rasterizeMeshes;
            owner.rasterizeTerrain = this.rasterizeTerrain;
            owner.rasterizeTrees = this.rasterizeTrees;
            owner.colliderRasterizeDetail = this.colliderRasterizeDetail;
            owner.forcedBoundsCenter = this.forcedBoundsCenter;
            owner.forcedBoundsSize = this.forcedBoundsSize;
            owner.mask = this.mask;
            owner.tagMask = new List<string>(this.tagMask);
            owner.showMeshOutline = this.showMeshOutline;
            owner.showNodeConnections = this.showNodeConnections;
            owner.terrainSampleSize = this.terrainSampleSize;
            if (this._vertices != null)
            {
                owner._vertices = new VInt3[this._vertices.Length];
                for (int j = 0; j < this._vertices.Length; j++)
                {
                    owner._vertices[j] = this._vertices[j];
                }
            }
            owner.tileXCount = this.tileXCount;
            owner.tileZCount = this.tileZCount;
            owner.tiles = new NavmeshTile[this.tiles.Length];
            for (int i = 0; i < this.tiles.Length; i++)
            {
                owner.tiles[i] = this.tiles[i].Clone(owner);
                owner.tiles[i].PostClone();
            }
            owner.batchTileUpdate = this.batchTileUpdate;
            owner.batchUpdatedTiles = new List<int>();
        }

        public void EndBatchTileUpdate()
        {
            if (!this.batchTileUpdate)
            {
                throw new InvalidOperationException("Calling EndBatchLoad when batching not enabled");
            }
            this.batchTileUpdate = false;
            int tileXCount = this.tileXCount;
            int tileZCount = this.tileZCount;
            for (int i = 0; i < tileZCount; i++)
            {
                for (int m = 0; m < tileXCount; m++)
                {
                    this.tiles[m + (i * this.tileXCount)].flag = false;
                }
            }
            for (int j = 0; j < this.batchUpdatedTiles.Count; j++)
            {
                this.tiles[this.batchUpdatedTiles[j]].flag = true;
            }
            for (int k = 0; k < tileZCount; k++)
            {
                for (int n = 0; n < tileXCount; n++)
                {
                    if (((n < (tileXCount - 1)) && (this.tiles[n + (k * this.tileXCount)].flag || this.tiles[(n + 1) + (k * this.tileXCount)].flag)) && (this.tiles[n + (k * this.tileXCount)] != this.tiles[(n + 1) + (k * this.tileXCount)]))
                    {
                        this.ConnectTiles(this.tiles[n + (k * this.tileXCount)], this.tiles[(n + 1) + (k * this.tileXCount)]);
                    }
                    if (((k < (tileZCount - 1)) && (this.tiles[n + (k * this.tileXCount)].flag || this.tiles[n + ((k + 1) * this.tileXCount)].flag)) && (this.tiles[n + (k * this.tileXCount)] != this.tiles[n + ((k + 1) * this.tileXCount)]))
                    {
                        this.ConnectTiles(this.tiles[n + (k * this.tileXCount)], this.tiles[n + ((k + 1) * this.tileXCount)]);
                    }
                }
            }
            this.batchUpdatedTiles.Clear();
        }

        public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
        {
            return this.GetNearestForce(position, null);
        }

        public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
        {
            if (this.tiles == null)
            {
                return new NNInfo();
            }
            Vector3 vector = position - this.forcedBounds.min;
            int num = Mathf.FloorToInt(vector.x / (this.cellSize * this.tileSizeX));
            int num2 = Mathf.FloorToInt(vector.z / (this.cellSize * this.tileSizeZ));
            num = Mathf.Clamp(num, 0, this.tileXCount - 1);
            num2 = Mathf.Clamp(num2, 0, this.tileZCount - 1);
            int num3 = Math.Max(this.tileXCount, this.tileZCount);
            NNInfo previous = new NNInfo();
            float positiveInfinity = float.PositiveInfinity;
            bool flag = this.nearestSearchOnlyXZ || ((constraint != null) && constraint.distanceXZ);
            for (int i = 0; i < num3; i++)
            {
                if (!flag && (positiveInfinity < (((i - 1) * this.cellSize) * Math.Max(this.tileSizeX, this.tileSizeZ))))
                {
                    break;
                }
                int num6 = Math.Min((i + num2) + 1, this.tileZCount);
                for (int j = Math.Max(-i + num2, 0); j < num6; j++)
                {
                    int num8 = Math.Abs((int) (i - Math.Abs((int) (j - num2))));
                    if ((-num8 + num) >= 0)
                    {
                        int num9 = -num8 + num;
                        NavmeshTile tile = this.tiles[num9 + (j * this.tileXCount)];
                        if (tile != null)
                        {
                            if (flag)
                            {
                                previous = tile.bbTree.QueryClosestXZ(position, constraint, ref positiveInfinity, previous);
                                if (positiveInfinity < float.PositiveInfinity)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                previous = tile.bbTree.QueryClosest(position, constraint, ref positiveInfinity, previous);
                            }
                        }
                    }
                    if ((num8 != 0) && ((num8 + num) < this.tileXCount))
                    {
                        int num10 = num8 + num;
                        NavmeshTile tile2 = this.tiles[num10 + (j * this.tileXCount)];
                        if (tile2 != null)
                        {
                            if (flag)
                            {
                                previous = tile2.bbTree.QueryClosestXZ(position, constraint, ref positiveInfinity, previous);
                                if (positiveInfinity >= float.PositiveInfinity)
                                {
                                    continue;
                                }
                                break;
                            }
                            previous = tile2.bbTree.QueryClosest(position, constraint, ref positiveInfinity, previous);
                        }
                    }
                }
            }
            previous.node = previous.constrainedNode;
            previous.constrainedNode = null;
            previous.clampedPosition = previous.constClampedPosition;
            return previous;
        }

        public override void GetNodes(GraphNodeDelegateCancelable del)
        {
            if (this.tiles != null)
            {
                for (int i = 0; i < this.tiles.Length; i++)
                {
                    if ((this.tiles[i] != null) && ((this.tiles[i].x + (this.tiles[i].z * this.tileXCount)) == i))
                    {
                        TriangleMeshNode[] nodes = this.tiles[i].nodes;
                        if (nodes != null)
                        {
                            for (int j = 0; (j < nodes.Length) && del(nodes[j]); j++)
                            {
                            }
                        }
                    }
                }
            }
        }

        public void GetRecastMeshObjs(Bounds bounds, List<ExtraMesh> buffer)
        {
            List<RecastMeshObj> list = ListPool<RecastMeshObj>.Claim();
            RecastMeshObj.GetAllInBounds(list, bounds);
            DictionaryObjectView<Mesh, Vector3[]> view = new DictionaryObjectView<Mesh, Vector3[]>();
            DictionaryObjectView<Mesh, int[]> view2 = new DictionaryObjectView<Mesh, int[]>();
            for (int i = 0; i < list.Count; i++)
            {
                MeshFilter meshFilter = list[i].GetMeshFilter();
                Renderer renderer = (meshFilter == null) ? null : meshFilter.GetComponent<Renderer>();
                if ((meshFilter != null) && (renderer != null))
                {
                    Mesh sharedMesh = meshFilter.sharedMesh;
                    ExtraMesh item = new ExtraMesh {
                        name = renderer.name,
                        matrix = renderer.localToWorldMatrix,
                        original = meshFilter,
                        area = list[i].area
                    };
                    if (view.ContainsKey(sharedMesh))
                    {
                        item.vertices = view[sharedMesh];
                        item.triangles = view2[sharedMesh];
                    }
                    else
                    {
                        item.vertices = sharedMesh.vertices;
                        item.triangles = sharedMesh.triangles;
                        view[sharedMesh] = item.vertices;
                        view2[sharedMesh] = item.triangles;
                    }
                    item.bounds = renderer.bounds;
                    buffer.Add(item);
                }
                else
                {
                    Collider col = list[i].GetCollider();
                    if (col == null)
                    {
                        Debug.LogError("RecastMeshObject (" + list[i].gameObject.name + ") didn't have a collider or MeshFilter attached");
                    }
                    else
                    {
                        ExtraMesh mesh3 = this.RasterizeCollider(col);
                        mesh3.area = list[i].area;
                        if (mesh3.vertices != null)
                        {
                            buffer.Add(mesh3);
                        }
                    }
                }
            }
            this.capsuleCache.Clear();
            ListPool<RecastMeshObj>.Release(list);
        }

        public static string GetRecastPath()
        {
            return (Application.dataPath + "/Recast/recast");
        }

        private static void GetSceneMeshes(Bounds bounds, List<string> tagMask, LayerMask layerMask, List<ExtraMesh> meshes)
        {
            if (((tagMask != null) && (tagMask.Count > 0)) || (layerMask != 0))
            {
                MeshFilter[] filterArray = UnityEngine.Object.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
                ListView<MeshFilter> view = new ListView<MeshFilter>(filterArray.Length / 3);
                for (int i = 0; i < filterArray.Length; i++)
                {
                    MeshFilter item = filterArray[i];
                    Renderer component = item.GetComponent<Renderer>();
                    if (((((component != null) && (item.sharedMesh != null)) && component.enabled) && ((((((int) 1) << item.gameObject.layer) & layerMask) != 0) || tagMask.Contains(item.tag))) && (item.GetComponent<RecastMeshObj>() == null))
                    {
                        view.Add(item);
                    }
                }
                DictionaryObjectView<Mesh, Vector3[]> view2 = new DictionaryObjectView<Mesh, Vector3[]>();
                DictionaryObjectView<Mesh, int[]> view3 = new DictionaryObjectView<Mesh, int[]>();
                bool flag = false;
                for (int j = 0; j < view.Count; j++)
                {
                    MeshFilter filter2 = view[j];
                    Renderer renderer2 = filter2.GetComponent<Renderer>();
                    if (renderer2.isPartOfStaticBatch)
                    {
                        flag = true;
                    }
                    else if (renderer2.bounds.Intersects(bounds))
                    {
                        Mesh sharedMesh = filter2.sharedMesh;
                        ExtraMesh mesh2 = new ExtraMesh {
                            name = renderer2.name,
                            matrix = renderer2.localToWorldMatrix,
                            original = filter2
                        };
                        if (view2.ContainsKey(sharedMesh))
                        {
                            mesh2.vertices = view2[sharedMesh];
                            mesh2.triangles = view3[sharedMesh];
                        }
                        else
                        {
                            mesh2.vertices = sharedMesh.vertices;
                            mesh2.triangles = sharedMesh.triangles;
                            view2[sharedMesh] = mesh2.vertices;
                            view3[sharedMesh] = mesh2.triangles;
                        }
                        mesh2.bounds = renderer2.bounds;
                        meshes.Add(mesh2);
                    }
                    if (flag)
                    {
                        Debug.LogWarning("Some meshes were statically batched. These meshes can not be used for navmesh calculation due to technical constraints.\nDuring runtime scripts cannot access the data of meshes which have been statically batched.\nOne way to solve this problem is to use cached startup (Save & Load tab in the inspector) to only calculate the graph when the game is not playing.");
                    }
                }
            }
        }

        public Bounds GetTileBounds(IntRect rect)
        {
            return this.GetTileBounds(rect.xmin, rect.ymin, rect.Width, rect.Height);
        }

        public Bounds GetTileBounds(int x, int z, int width = 1, int depth = 1)
        {
            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3((x * this.tileSizeX) * this.cellSize, 0f, (z * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min, new Vector3(((x + width) * this.tileSizeX) * this.cellSize, this.forcedBounds.size.y, ((z + depth) * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min);
            return bounds;
        }

        public VInt2 GetTileCoordinates(Vector3 p)
        {
            p -= this.forcedBounds.min;
            p.x /= this.cellSize * this.tileSizeX;
            p.z /= this.cellSize * this.tileSizeZ;
            return new VInt2((int) p.x, (int) p.z);
        }

        public void GetTileCoordinates(int tileIndex, out int x, out int z)
        {
            z = tileIndex / this.tileXCount;
            x = tileIndex - (z * this.tileXCount);
        }

        public int GetTileIndex(int index)
        {
            return ((index >> 12) & 0x7ffff);
        }

        public NavmeshTile[] GetTiles()
        {
            return this.tiles;
        }

        public IntRect GetTouchingTiles(Bounds b)
        {
            b.center -= this.forcedBounds.min;
            IntRect a = new IntRect(Mathf.FloorToInt(b.min.x / (this.tileSizeX * this.cellSize)), Mathf.FloorToInt(b.min.z / (this.tileSizeZ * this.cellSize)), Mathf.FloorToInt(b.max.x / (this.tileSizeX * this.cellSize)), Mathf.FloorToInt(b.max.z / (this.tileSizeZ * this.cellSize)));
            return IntRect.Intersection(a, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
        }

        public IntRect GetTouchingTilesRound(Bounds b)
        {
            b.center -= this.forcedBounds.min;
            IntRect a = new IntRect(Mathf.RoundToInt(b.min.x / (this.tileSizeX * this.cellSize)), Mathf.RoundToInt(b.min.z / (this.tileSizeZ * this.cellSize)), Mathf.RoundToInt(b.max.x / (this.tileSizeX * this.cellSize)) - 1, Mathf.RoundToInt(b.max.z / (this.tileSizeZ * this.cellSize)) - 1);
            return IntRect.Intersection(a, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
        }

        public VInt3 GetVertex(int index)
        {
            int num = (index >> 12) & 0x7ffff;
            return this.tiles[num].GetVertex(index);
        }

        public int GetVertexArrayIndex(int index)
        {
            return (index & 0xfff);
        }

        public bool Linecast(VInt3 origin, VInt3 end)
        {
            return this.Linecast(origin, end, base.GetNearest(origin, NNConstraint.None).node);
        }

        public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint)
        {
            GraphHitInfo info;
            return NavMeshGraph.Linecast(this, origin, end, hint, out info, null);
        }

        public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint, out GraphHitInfo hit)
        {
            return NavMeshGraph.Linecast(this, origin, end, hint, out hit, null);
        }

        public bool Linecast(VInt3 tmp_origin, VInt3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
        {
            return NavMeshGraph.Linecast(this, tmp_origin, tmp_end, hint, out hit, trace);
        }

        private static NavmeshTile NewEmptyTile(int x, int z)
        {
            NavmeshTile tile;
            return new NavmeshTile { x = x, z = z, w = 1, d = 1, verts = new VInt3[0], tris = new int[0], nodes = new TriangleMeshNode[0], bbTree = new BBTree(tile) };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            TriangleMeshNode.SetNavmeshHolder(this.astarData.DataGroupIndex, this.astarData.GetGraphIndex(this), null);
        }

        public override void OnDrawGizmos(bool drawNodes)
        {
            <OnDrawGizmos>c__AnonStorey33 storey = new <OnDrawGizmos>c__AnonStorey33 {
                <>f__this = this
            };
            if (drawNodes)
            {
                if (this.bbTree != null)
                {
                    this.bbTree.OnDrawGizmos();
                }
                Gizmos.DrawWireCube(this.forcedBounds.center, this.forcedBounds.size);
                storey.debugData = AstarPath.active.debugPathData;
                GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(storey.<>m__27);
                this.GetNodes(del);
            }
        }

        public GraphNode PointOnNavmesh(Vector3 position, NNConstraint constraint)
        {
            if (this.tiles != null)
            {
                Vector3 vector = position - this.forcedBounds.min;
                int num = Mathf.FloorToInt(vector.x / (this.cellSize * this.tileSizeX));
                int num2 = Mathf.FloorToInt(vector.z / (this.cellSize * this.tileSizeZ));
                if (((num < 0) || (num2 < 0)) || ((num >= this.tileXCount) || (num2 >= this.tileZCount)))
                {
                    return null;
                }
                NavmeshTile tile = this.tiles[num + (num2 * this.tileXCount)];
                if (tile != null)
                {
                    return tile.bbTree.QueryInside(position, constraint);
                }
            }
            return null;
        }

        private ExtraMesh RasterizeCollider(Collider col)
        {
            return this.RasterizeCollider(col, col.transform.localToWorldMatrix);
        }

        private ExtraMesh RasterizeCollider(Collider col, Matrix4x4 localToWorldMatrix)
        {
            if (col is BoxCollider)
            {
                BoxCollider collider = col as BoxCollider;
                Matrix4x4 matrix = Matrix4x4.TRS(collider.center, Quaternion.identity, (Vector3) (collider.size * 0.5f));
                matrix = localToWorldMatrix * matrix;
                return new ExtraMesh(this.BoxColliderVerts, this.BoxColliderTris, collider.bounds, matrix);
            }
            if ((col is SphereCollider) || (col is CapsuleCollider))
            {
                Vector3[] verts;
                SphereCollider collider2 = col as SphereCollider;
                CapsuleCollider collider3 = col as CapsuleCollider;
                float num = (collider2 == null) ? collider3.radius : collider2.radius;
                float b = (collider2 == null) ? (((collider3.height * 0.5f) / num) - 1f) : 0f;
                Matrix4x4 matrixx2 = Matrix4x4.TRS((collider2 == null) ? collider3.center : collider2.center, Quaternion.identity, (Vector3) (Vector3.one * num));
                matrixx2 = localToWorldMatrix * matrixx2;
                int num3 = Mathf.Max(4, Mathf.RoundToInt(this.colliderRasterizeDetail * Mathf.Sqrt(matrixx2.MultiplyVector(Vector3.one).magnitude)));
                if (num3 > 100)
                {
                    Debug.LogWarning("Very large detail for some collider meshes. Consider decreasing Collider Rasterize Detail (RecastGraph)");
                }
                int num4 = num3;
                CapsuleCache item = null;
                for (int i = 0; i < this.capsuleCache.Count; i++)
                {
                    CapsuleCache cache2 = this.capsuleCache[i];
                    if ((cache2.rows == num3) && Mathf.Approximately(cache2.height, b))
                    {
                        item = cache2;
                    }
                }
                if (item == null)
                {
                    verts = new Vector3[(num3 * num4) + 2];
                    List<int> list = new List<int>();
                    verts[verts.Length - 1] = Vector3.up;
                    for (int j = 0; j < num3; j++)
                    {
                        for (int num7 = 0; num7 < num4; num7++)
                        {
                            verts[num7 + (j * num4)] = new Vector3(Mathf.Cos(((num7 * 3.141593f) * 2f) / ((float) num4)) * Mathf.Sin((j * 3.141593f) / ((float) (num3 - 1))), Mathf.Cos((j * 3.141593f) / ((float) (num3 - 1))) + ((j >= (num3 / 2)) ? -b : b), Mathf.Sin(((num7 * 3.141593f) * 2f) / ((float) num4)) * Mathf.Sin((j * 3.141593f) / ((float) (num3 - 1))));
                        }
                    }
                    verts[verts.Length - 2] = Vector3.down;
                    int num8 = 0;
                    for (int k = num4 - 1; num8 < num4; k = num8++)
                    {
                        list.Add(verts.Length - 1);
                        list.Add((0 * num4) + k);
                        list.Add((0 * num4) + num8);
                    }
                    for (int m = 1; m < num3; m++)
                    {
                        int num11 = 0;
                        for (int num12 = num4 - 1; num11 < num4; num12 = num11++)
                        {
                            list.Add((m * num4) + num11);
                            list.Add((m * num4) + num12);
                            list.Add(((m - 1) * num4) + num11);
                            list.Add(((m - 1) * num4) + num12);
                            list.Add(((m - 1) * num4) + num11);
                            list.Add((m * num4) + num12);
                        }
                    }
                    int num13 = 0;
                    for (int n = num4 - 1; num13 < num4; n = num13++)
                    {
                        list.Add(verts.Length - 2);
                        list.Add(((num3 - 1) * num4) + n);
                        list.Add(((num3 - 1) * num4) + num13);
                    }
                    item = new CapsuleCache {
                        rows = num3,
                        height = b,
                        verts = verts,
                        tris = list.ToArray()
                    };
                    this.capsuleCache.Add(item);
                }
                verts = item.verts;
                int[] tris = item.tris;
                return new ExtraMesh(verts, tris, col.bounds, matrixx2);
            }
            if (col is MeshCollider)
            {
                MeshCollider collider4 = col as MeshCollider;
                if (collider4.sharedMesh != null)
                {
                    return new ExtraMesh(collider4.sharedMesh.vertices, collider4.sharedMesh.triangles, collider4.bounds, localToWorldMatrix);
                }
            }
            return new ExtraMesh();
        }

        public void RemoveConnectionsFromTile(NavmeshTile tile)
        {
            if (tile.x > 0)
            {
                int num = tile.x - 1;
                for (int i = tile.z; i < (tile.z + tile.d); i++)
                {
                    this.RemoveConnectionsFromTo(this.tiles[num + (i * this.tileXCount)], tile);
                }
            }
            if ((tile.x + tile.w) < this.tileXCount)
            {
                int num3 = tile.x + tile.w;
                for (int j = tile.z; j < (tile.z + tile.d); j++)
                {
                    this.RemoveConnectionsFromTo(this.tiles[num3 + (j * this.tileXCount)], tile);
                }
            }
            if (tile.z > 0)
            {
                int num5 = tile.z - 1;
                for (int k = tile.x; k < (tile.x + tile.w); k++)
                {
                    this.RemoveConnectionsFromTo(this.tiles[k + (num5 * this.tileXCount)], tile);
                }
            }
            if ((tile.z + tile.d) < this.tileZCount)
            {
                int num7 = tile.z + tile.d;
                for (int m = tile.x; m < (tile.x + tile.w); m++)
                {
                    this.RemoveConnectionsFromTo(this.tiles[m + (num7 * this.tileXCount)], tile);
                }
            }
        }

        public void RemoveConnectionsFromTo(NavmeshTile a, NavmeshTile b)
        {
            if (((a != null) && (b != null)) && (a != b))
            {
                int num = b.x + (b.z * this.tileXCount);
                for (int i = 0; i < a.nodes.Length; i++)
                {
                    TriangleMeshNode node = a.nodes[i];
                    if (node.connections != null)
                    {
                        for (int j = 0; j < node.connections.Length; j++)
                        {
                            TriangleMeshNode node2 = node.connections[j] as TriangleMeshNode;
                            if (node2 != null)
                            {
                                int num4 = (node2.GetVertexIndex(0) >> 12) & 0x7ffff;
                                if (num4 == num)
                                {
                                    node.RemoveConnection(node.connections[j]);
                                    j--;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ReplaceTile(int x, int z, VInt3[] verts, int[] tris, bool worldSpace)
        {
            this.ReplaceTile(x, z, 1, 1, verts, tris, worldSpace);
        }

        public void ReplaceTile(int x, int z, int w, int d, VInt3[] verts, int[] tris, bool worldSpace)
        {
            NavmeshTile tile3;
            if ((((x + w) > this.tileXCount) || ((z + d) > this.tileZCount)) || ((x < 0) || (z < 0)))
            {
                object[] objArray1 = new object[] { "Tile is placed at an out of bounds position or extends out of the graph bounds (", x, ", ", z, " [", w, ", ", d, "] ", this.tileXCount, " ", this.tileZCount, ")" };
                throw new ArgumentException(string.Concat(objArray1));
            }
            if ((w < 1) || (d < 1))
            {
                object[] objArray2 = new object[] { "width and depth must be greater or equal to 1. Was ", w, ", ", d };
                throw new ArgumentException(string.Concat(objArray2));
            }
            for (int i = z; i < (z + d); i++)
            {
                for (int n = x; n < (x + w); n++)
                {
                    NavmeshTile tile = this.tiles[n + (i * this.tileXCount)];
                    if (tile != null)
                    {
                        this.RemoveConnectionsFromTile(tile);
                        for (int num3 = 0; num3 < tile.nodes.Length; num3++)
                        {
                            tile.nodes[num3].Destroy();
                        }
                        for (int num4 = tile.z; num4 < (tile.z + tile.d); num4++)
                        {
                            for (int num5 = tile.x; num5 < (tile.x + tile.w); num5++)
                            {
                                NavmeshTile tile2 = this.tiles[num5 + (num4 * this.tileXCount)];
                                if ((tile2 == null) || (tile2 != tile))
                                {
                                    throw new Exception("This should not happen");
                                }
                                if (((num4 < z) || (num4 >= (z + d))) || ((num5 < x) || (num5 >= (x + w))))
                                {
                                    this.tiles[num5 + (num4 * this.tileXCount)] = NewEmptyTile(num5, num4);
                                    if (this.batchTileUpdate)
                                    {
                                        this.batchUpdatedTiles.Add(num5 + (num4 * this.tileXCount));
                                    }
                                }
                                else
                                {
                                    this.tiles[num5 + (num4 * this.tileXCount)] = null;
                                }
                            }
                        }
                    }
                }
            }
            tile3 = new NavmeshTile {
                x = x,
                z = z,
                w = w,
                d = d,
                tris = tris,
                verts = verts,
                bbTree = new BBTree(tile3)
            };
            if ((tile3.tris.Length % 3) != 0)
            {
                throw new ArgumentException("Triangle array's length must be a multiple of 3 (tris)");
            }
            if (tile3.verts.Length > 0xffff)
            {
                throw new ArgumentException("Too many vertices per tile (more than 65535)");
            }
            if (!worldSpace)
            {
                if (!Mathf.Approximately(((x * this.tileSizeX) * this.cellSize) * 1000f, (float) Math.Round((double) (((x * this.tileSizeX) * this.cellSize) * 1000f))))
                {
                    Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");
                }
                if (!Mathf.Approximately(((z * this.tileSizeZ) * this.cellSize) * 1000f, (float) Math.Round((double) (((z * this.tileSizeZ) * this.cellSize) * 1000f))))
                {
                    Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");
                }
                VInt3 num6 = (VInt3) (new Vector3((x * this.tileSizeX) * this.cellSize, 0f, (z * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min);
                for (int num7 = 0; num7 < verts.Length; num7++)
                {
                    verts[num7] += num6;
                }
            }
            TriangleMeshNode[] nodeArray = new TriangleMeshNode[tile3.tris.Length / 3];
            tile3.nodes = nodeArray;
            int length = this.astarData.graphs.Length;
            int dataGroupIndex = this.astarData.DataGroupIndex;
            TriangleMeshNode.SetNavmeshHolder(dataGroupIndex, length, tile3);
            int num10 = x + (z * this.tileXCount);
            num10 = num10 << 12;
            for (int j = 0; j < nodeArray.Length; j++)
            {
                TriangleMeshNode node = new TriangleMeshNode(base.active);
                nodeArray[j] = node;
                node.GraphIndex = (uint) length;
                node.DataGroupIndex = dataGroupIndex;
                node.v0 = tile3.tris[j * 3] | num10;
                node.v1 = tile3.tris[(j * 3) + 1] | num10;
                node.v2 = tile3.tris[(j * 3) + 2] | num10;
                if (!Polygon.IsClockwise(node.GetVertex(0), node.GetVertex(1), node.GetVertex(2)))
                {
                    int num12 = node.v0;
                    node.v0 = node.v2;
                    node.v2 = num12;
                }
                node.Walkable = true;
                node.Penalty = base.initialPenalty;
                node.UpdatePositionFromVertices();
                tile3.bbTree.Insert(node);
            }
            this.CreateNodeConnections(tile3.nodes);
            for (int k = z; k < (z + d); k++)
            {
                for (int num14 = x; num14 < (x + w); num14++)
                {
                    this.tiles[num14 + (k * this.tileXCount)] = tile3;
                }
            }
            if (this.batchTileUpdate)
            {
                this.batchUpdatedTiles.Add(x + (z * this.tileXCount));
            }
            else
            {
                this.ConnectTileWithNeighbours(tile3);
            }
            TriangleMeshNode.SetNavmeshHolder(dataGroupIndex, length, null);
            length = this.astarData.GetGraphIndex(this);
            for (int m = 0; m < nodeArray.Length; m++)
            {
                nodeArray[m].GraphIndex = (uint) length;
            }
        }

        protected void ScanAllTiles(OnScanStatus statusCallback)
        {
            <ScanAllTiles>c__AnonStorey32 storey = new <ScanAllTiles>c__AnonStorey32();
            int num = (int) ((this.forcedBounds.size.x / this.cellSize) + 0.5f);
            int num2 = (int) ((this.forcedBounds.size.z / this.cellSize) + 0.5f);
            if (!this.useTiles)
            {
                this.tileSizeX = num;
                this.tileSizeZ = num2;
            }
            else
            {
                this.tileSizeX = this.editorTileSize;
                this.tileSizeZ = this.editorTileSize;
            }
            int num3 = ((num + this.tileSizeX) - 1) / this.tileSizeX;
            int num4 = ((num2 + this.tileSizeZ) - 1) / this.tileSizeZ;
            this.tileXCount = num3;
            this.tileZCount = num4;
            if ((this.tileXCount * this.tileZCount) > 0x80000)
            {
                object[] objArray1 = new object[] { "Too many tiles (", this.tileXCount * this.tileZCount, ") maximum is ", 0x80000, "\nTry disabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* inspector." };
                throw new Exception(string.Concat(objArray1));
            }
            this.tiles = new NavmeshTile[this.tileXCount * this.tileZCount];
            if (this.scanEmptyGraph)
            {
                for (int i = 0; i < num4; i++)
                {
                    for (int j = 0; j < num3; j++)
                    {
                        this.tiles[(i * this.tileXCount) + j] = NewEmptyTile(j, i);
                    }
                }
            }
            else
            {
                List<ExtraMesh> list;
                Console.WriteLine("Collecting Meshes");
                this.CollectMeshes(out list, this.forcedBounds);
                Voxelize vox = new Voxelize(this.cellHeight, this.cellSize, this.walkableClimb, this.walkableHeight, this.maxSlope) {
                    inputExtraMeshes = list,
                    maxEdgeLength = this.maxEdgeLength
                };
                int num7 = -1;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int k = 0; k < num4; k++)
                {
                    for (int n = 0; n < num3; n++)
                    {
                        int num10 = (k * this.tileXCount) + n;
                        Console.WriteLine(string.Concat(new object[] { "Generating Tile #", num10, " of ", num4 * num3 }));
                        if (((((num10 * 10) / this.tiles.Length) > num7) || (stopwatch.ElapsedMilliseconds > 0x7d0L)) && (statusCallback != null))
                        {
                            num7 = (num10 * 10) / this.tiles.Length;
                            stopwatch.Reset();
                            stopwatch.Start();
                            object[] objArray3 = new object[] { "Building Tile ", num10, "/", this.tiles.Length };
                            statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.9f, ((float) num10) / ((float) this.tiles.Length)), string.Concat(objArray3)));
                        }
                        this.BuildTileMesh(vox, n, k);
                    }
                }
                Console.WriteLine("Assigning Graph Indices");
                if (statusCallback != null)
                {
                    statusCallback(new Progress(0.9f, "Connecting tiles"));
                }
                storey.graphIndex = (uint) this.astarData.GetGraphIndex(this);
                GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(storey.<>m__26);
                this.GetNodes(del);
                for (int m = 0; m < num4; m++)
                {
                    for (int num12 = 0; num12 < num3; num12++)
                    {
                        Console.WriteLine(string.Concat(new object[] { "Connecing Tile #", (m * this.tileXCount) + num12, " of ", num4 * num3 }));
                        if (num12 < (num3 - 1))
                        {
                            this.ConnectTiles(this.tiles[num12 + (m * this.tileXCount)], this.tiles[(num12 + 1) + (m * this.tileXCount)]);
                        }
                        if (m < (num4 - 1))
                        {
                            this.ConnectTiles(this.tiles[num12 + (m * this.tileXCount)], this.tiles[num12 + ((m + 1) * this.tileXCount)]);
                        }
                    }
                }
            }
        }

        protected void ScanCRecast()
        {
            Debug.LogError("The C++ version of recast can only be used in osx editor or osx standalone mode, I'm sure it cannot be used in the webplayer, but other platforms are not tested yet\nIf you are in the Unity Editor, try switching Platform to OSX Standalone just when scanning, scanned graphs can be cached to enable them to be used in a webplayer.");
        }

        public override void ScanInternal(OnScanStatus statusCallback)
        {
            TriangleMeshNode.SetNavmeshHolder(0, this.astarData.GetGraphIndex(this), this);
            this.ScanTiledNavmesh(statusCallback);
        }

        protected void ScanTiledNavmesh(OnScanStatus statusCallback)
        {
            this.ScanAllTiles(statusCallback);
        }

        public override void SerializeExtraInfo(GraphSerializationContext ctx)
        {
            BinaryWriter writer = ctx.writer;
            if (this.tiles == null)
            {
                writer.Write(-1);
            }
            else
            {
                writer.Write(this.tileXCount);
                writer.Write(this.tileZCount);
                for (int i = 0; i < this.tileZCount; i++)
                {
                    for (int j = 0; j < this.tileXCount; j++)
                    {
                        NavmeshTile tile = this.tiles[j + (i * this.tileXCount)];
                        if (tile == null)
                        {
                            throw new Exception("NULL Tile");
                        }
                        writer.Write(tile.x);
                        writer.Write(tile.z);
                        if ((tile.x == j) && (tile.z == i))
                        {
                            writer.Write(tile.w);
                            writer.Write(tile.d);
                            writer.Write(tile.tris.Length);
                            for (int k = 0; k < tile.tris.Length; k++)
                            {
                                writer.Write(tile.tris[k]);
                            }
                            writer.Write(tile.verts.Length);
                            for (int m = 0; m < tile.verts.Length; m++)
                            {
                                writer.Write(tile.verts[m].x);
                                writer.Write(tile.verts[m].y);
                                writer.Write(tile.verts[m].z);
                            }
                            writer.Write(tile.nodes.Length);
                            for (int n = 0; n < tile.nodes.Length; n++)
                            {
                                tile.nodes[n].SerializeNode(ctx);
                            }
                        }
                    }
                }
            }
        }

        public override void SerializeSettings(GraphSerializationContext ctx)
        {
            base.SerializeSettings(ctx);
            ctx.writer.Write(this.characterRadius);
            ctx.writer.Write(this.contourMaxError);
            ctx.writer.Write(this.cellSize);
            ctx.writer.Write(this.cellHeight);
            ctx.writer.Write(this.walkableHeight);
            ctx.writer.Write(this.maxSlope);
            ctx.writer.Write(this.maxEdgeLength);
            ctx.writer.Write(this.editorTileSize);
            ctx.writer.Write(this.tileSizeX);
            ctx.writer.Write(this.nearestSearchOnlyXZ);
            ctx.writer.Write(this.useTiles);
            ctx.writer.Write((int) this.relevantGraphSurfaceMode);
            ctx.writer.Write(this.rasterizeColliders);
            ctx.writer.Write(this.rasterizeMeshes);
            ctx.writer.Write(this.rasterizeTerrain);
            ctx.writer.Write(this.rasterizeTrees);
            ctx.writer.Write(this.colliderRasterizeDetail);
            ctx.SerializeVector3(this.forcedBoundsCenter);
            ctx.SerializeVector3(this.forcedBoundsSize);
            ctx.writer.Write((int) this.mask);
            ctx.writer.Write(this.tagMask.Count);
            for (int i = 0; i < this.tagMask.Count; i++)
            {
                ctx.writer.Write(this.tagMask[i]);
            }
            ctx.writer.Write(this.showMeshOutline);
            ctx.writer.Write(this.showNodeConnections);
            ctx.writer.Write(this.terrainSampleSize);
        }

        public void SnapForceBoundsToScene()
        {
            List<ExtraMesh> meshes = new List<ExtraMesh>();
            GetSceneMeshes(this.forcedBounds, this.tagMask, this.mask, meshes);
            if (meshes.Count != 0)
            {
                Bounds bounds = new Bounds();
                ExtraMesh mesh = meshes[0];
                bounds = mesh.bounds;
                for (int i = 1; i < meshes.Count; i++)
                {
                    ExtraMesh mesh2 = meshes[i];
                    bounds.Encapsulate(mesh2.bounds);
                }
                this.forcedBoundsCenter = bounds.center;
                this.forcedBoundsSize = bounds.size;
            }
        }

        public void StartBatchTileUpdate()
        {
            if (this.batchTileUpdate)
            {
                throw new InvalidOperationException("Calling StartBatchLoad when batching is already enabled");
            }
            this.batchTileUpdate = true;
        }

        public void UpdateArea(GraphUpdateObject guo)
        {
            Bounds bounds = guo.bounds;
            bounds.center -= this.forcedBounds.min;
            int xmin = Mathf.FloorToInt(bounds.min.x / (this.tileSizeX * this.cellSize));
            int ymin = Mathf.FloorToInt(bounds.min.z / (this.tileSizeZ * this.cellSize));
            int xmax = Mathf.FloorToInt(bounds.max.x / (this.tileSizeX * this.cellSize));
            IntRect a = new IntRect(xmin, ymin, xmax, Mathf.FloorToInt(bounds.max.z / (this.tileSizeZ * this.cellSize)));
            a = IntRect.Intersection(a, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
            if (!guo.updatePhysics)
            {
                for (int i = a.ymin; i <= a.ymax; i++)
                {
                    for (int k = a.xmin; k <= a.xmax; k++)
                    {
                        NavmeshTile tile = this.tiles[(i * this.tileXCount) + k];
                        tile.flag = true;
                    }
                }
                for (int j = a.ymin; j <= a.ymax; j++)
                {
                    for (int m = a.xmin; m <= a.xmax; m++)
                    {
                        NavmeshTile graph = this.tiles[(j * this.tileXCount) + m];
                        if (graph.flag)
                        {
                            graph.flag = false;
                            NavMeshGraph.UpdateArea(guo, graph);
                        }
                    }
                }
            }
            else
            {
                if (!this.dynamic)
                {
                    throw new Exception("Recast graph must be marked as dynamic to enable graph updates with updatePhysics = true");
                }
                Voxelize globalVox = this.globalVox;
                if (globalVox == null)
                {
                    throw new InvalidOperationException("No Voxelizer object. UpdateAreaInit should have been called before this function.");
                }
                for (int n = a.xmin; n <= a.xmax; n++)
                {
                    for (int num6 = a.ymin; num6 <= a.ymax; num6++)
                    {
                        this.RemoveConnectionsFromTile(this.tiles[n + (num6 * this.tileXCount)]);
                    }
                }
                for (int num7 = a.xmin; num7 <= a.xmax; num7++)
                {
                    for (int num8 = a.ymin; num8 <= a.ymax; num8++)
                    {
                        this.BuildTileMesh(globalVox, num7, num8);
                    }
                }
                uint graphIndex = (uint) this.astarData.GetGraphIndex(this);
                for (int num10 = a.xmin; num10 <= a.xmax; num10++)
                {
                    for (int num11 = a.ymin; num11 <= a.ymax; num11++)
                    {
                        NavmeshTile tile3 = this.tiles[num10 + (num11 * this.tileXCount)];
                        GraphNode[] nodes = tile3.nodes;
                        for (int num12 = 0; num12 < nodes.Length; num12++)
                        {
                            nodes[num12].GraphIndex = graphIndex;
                        }
                    }
                }
                a = IntRect.Intersection(a.Expand(1), new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
                for (int num13 = a.xmin; num13 <= a.xmax; num13++)
                {
                    for (int num14 = a.ymin; num14 <= a.ymax; num14++)
                    {
                        if ((num13 < (this.tileXCount - 1)) && a.Contains(num13 + 1, num14))
                        {
                            this.ConnectTiles(this.tiles[num13 + (num14 * this.tileXCount)], this.tiles[(num13 + 1) + (num14 * this.tileXCount)]);
                        }
                        if ((num14 < (this.tileZCount - 1)) && a.Contains(num13, num14 + 1))
                        {
                            this.ConnectTiles(this.tiles[num13 + (num14 * this.tileXCount)], this.tiles[num13 + ((num14 + 1) * this.tileXCount)]);
                        }
                    }
                }
            }
        }

        public void UpdateAreaInit(GraphUpdateObject o)
        {
            if (o.updatePhysics)
            {
                List<ExtraMesh> list;
                if (!this.dynamic)
                {
                    throw new Exception("Recast graph must be marked as dynamic to enable graph updates");
                }
                RelevantGraphSurface.UpdateAllPositions();
                IntRect touchingTiles = this.GetTouchingTiles(o.bounds);
                Bounds tileBounds = this.GetTileBounds(touchingTiles);
                int num2 = Mathf.CeilToInt(this.characterRadius / this.cellSize) + 3;
                tileBounds.Expand((Vector3) ((new Vector3((float) num2, 0f, (float) num2) * this.cellSize) * 2f));
                this.CollectMeshes(out list, tileBounds);
                Voxelize globalVox = this.globalVox;
                if (globalVox == null)
                {
                    globalVox = new Voxelize(this.cellHeight, this.cellSize, this.walkableClimb, this.walkableHeight, this.maxSlope) {
                        maxEdgeLength = this.maxEdgeLength
                    };
                    if (this.dynamic)
                    {
                        this.globalVox = globalVox;
                    }
                }
                globalVox.inputExtraMeshes = list;
            }
        }

        public AstarData astarData
        {
            get
            {
                if (this.dataOverride == null)
                {
                }
                return AstarPath.active.astarData;
            }
        }

        public BBTree bbTree
        {
            get
            {
                return this._bbTree;
            }
            set
            {
                this._bbTree = value;
            }
        }

        public Bounds forcedBounds
        {
            get
            {
                return new Bounds(this.forcedBoundsCenter, this.forcedBoundsSize);
            }
        }

        public VInt3[] vertices
        {
            get
            {
                return this._vertices;
            }
            set
            {
                this._vertices = value;
            }
        }

        [CompilerGenerated]
        private sealed class <OnDrawGizmos>c__AnonStorey33
        {
            internal RecastGraph <>f__this;
            internal PathHandler debugData;

            internal bool <>m__27(GraphNode _node)
            {
                TriangleMeshNode node = _node as TriangleMeshNode;
                if (AstarPath.active.showSearchTree && (this.debugData != null))
                {
                    bool flag = NavGraph.InSearchTree(node, AstarPath.active.debugPath);
                    if ((flag && this.<>f__this.showNodeConnections) && (this.debugData.GetPathNode(node).parent != null))
                    {
                        Gizmos.color = this.<>f__this.NodeColor(node, this.debugData);
                        Gizmos.DrawLine((Vector3) node.position, (Vector3) this.debugData.GetPathNode(node).parent.node.position);
                    }
                    if (this.<>f__this.showMeshOutline)
                    {
                        Gizmos.color = this.<>f__this.NodeColor(node, this.debugData);
                        if (!flag)
                        {
                            Gizmos.color *= new Color(1f, 1f, 1f, 0.1f);
                        }
                        Gizmos.DrawLine((Vector3) node.GetVertex(0), (Vector3) node.GetVertex(1));
                        Gizmos.DrawLine((Vector3) node.GetVertex(1), (Vector3) node.GetVertex(2));
                        Gizmos.DrawLine((Vector3) node.GetVertex(2), (Vector3) node.GetVertex(0));
                    }
                }
                else
                {
                    if (this.<>f__this.showNodeConnections)
                    {
                        Gizmos.color = this.<>f__this.NodeColor(node, null);
                        for (int i = 0; i < node.connections.Length; i++)
                        {
                            Gizmos.DrawLine((Vector3) node.position, Vector3.Lerp((Vector3) node.connections[i].position, (Vector3) node.position, 0.4f));
                        }
                    }
                    if (this.<>f__this.showMeshOutline)
                    {
                        Gizmos.color = this.<>f__this.NodeColor(node, this.debugData);
                        Gizmos.DrawLine((Vector3) node.GetVertex(0), (Vector3) node.GetVertex(1));
                        Gizmos.DrawLine((Vector3) node.GetVertex(1), (Vector3) node.GetVertex(2));
                        Gizmos.DrawLine((Vector3) node.GetVertex(2), (Vector3) node.GetVertex(0));
                    }
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <ScanAllTiles>c__AnonStorey32
        {
            internal uint graphIndex;

            internal bool <>m__26(GraphNode n)
            {
                n.GraphIndex = this.graphIndex;
                return true;
            }
        }

        private class CapsuleCache
        {
            public float height;
            public int rows;
            public int[] tris;
            public Vector3[] verts;
        }

        public class NavmeshTile : INavmesh, INavmeshHolder
        {
            public BBTree bbTree;
            public int d;
            public bool flag;
            public TriangleMeshNode[] nodes;
            public int[] tris;
            public VInt3[] verts;
            public int w;
            public int x;
            public int z;

            public RecastGraph.NavmeshTile Clone(RecastGraph owner)
            {
                RecastGraph.NavmeshTile tile = new RecastGraph.NavmeshTile {
                    tris = new int[this.tris.Length]
                };
                Array.Copy(this.tris, tile.tris, this.tris.Length);
                tile.verts = new VInt3[this.verts.Length];
                Array.Copy(this.verts, tile.verts, this.verts.Length);
                tile.x = this.x;
                tile.z = this.z;
                tile.w = this.w;
                tile.d = this.d;
                tile.flag = this.flag;
                Dictionary<int, TriangleMeshNode> dictionary = new Dictionary<int, TriangleMeshNode>();
                AstarData astarData = owner.astarData;
                uint length = (uint) astarData.graphs.Length;
                int dataGroupIndex = astarData.DataGroupIndex;
                tile.nodes = new TriangleMeshNode[this.nodes.Length];
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    TriangleMeshNode node2 = this.nodes[i].Clone();
                    node2.GraphIndex = length;
                    node2.DataGroupIndex = dataGroupIndex;
                    tile.nodes[i] = node2;
                    dictionary.Add(node2.NodeIndex, tile.nodes[i]);
                }
                for (int j = 0; j < this.nodes.Length; j++)
                {
                    TriangleMeshNode node3 = this.nodes[j];
                    TriangleMeshNode node4 = tile.nodes[j];
                    if (node3.connections != null)
                    {
                        node4.connections = new GraphNode[node3.connections.Length];
                        for (int k = 0; k < node3.connections.Length; k++)
                        {
                            TriangleMeshNode node5;
                            int nodeIndex = node3.connections[k].NodeIndex;
                            dictionary.TryGetValue(nodeIndex, out node5);
                            node4.connections[k] = node5;
                        }
                    }
                }
                dictionary.Clear();
                return tile;
            }

            public void GetNodes(GraphNodeDelegateCancelable del)
            {
                if (this.nodes != null)
                {
                    for (int i = 0; (i < this.nodes.Length) && del(this.nodes[i]); i++)
                    {
                    }
                }
            }

            public void GetTileCoordinates(int tileIndex, out int x, out int z)
            {
                x = this.x;
                z = this.z;
            }

            public VInt3 GetVertex(int index)
            {
                int num = index & 0xfff;
                return this.verts[num];
            }

            public int GetVertexArrayIndex(int index)
            {
                return (index & 0xfff);
            }

            public void PostClone()
            {
                this.bbTree = new BBTree(this);
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    this.bbTree.Insert(this.nodes[i]);
                }
            }
        }

        public enum RelevantGraphSurfaceMode
        {
            DoNotRequire,
            OnlyForCompletelyInsideTile,
            RequireForAll
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SceneMesh
        {
            public Mesh mesh;
            public Matrix4x4 matrix;
            public Bounds bounds;
        }
    }
}

