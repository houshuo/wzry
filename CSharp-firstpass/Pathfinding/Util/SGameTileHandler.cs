namespace Pathfinding.Util
{
    using Pathfinding;
    using Pathfinding.ClipperLib;
    using Pathfinding.Poly2Tri;
    using Pathfinding.Voxels;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SGameTileHandler
    {
        private RecastGraph _graph;
        private int[] activeTileOffsets;
        private int[] activeTileRotations;
        private TileType[] activeTileTypes;
        private int[] cached_int_array = new int[0x20];
        private Dictionary<VInt2, int> cached_Int2_int_dict = new Dictionary<VInt2, int>();
        private Dictionary<VInt3, int> cached_Int3_int_dict = new Dictionary<VInt3, int>();
        private Pathfinding.ClipperLib.Clipper clipper;
        private const int CUT_ALL = 0;
        private const int CUT_BREAK = 2;
        private const int CUT_DUAL = 1;
        private bool isBatching;
        private bool[] reloadedInBatch;
        private ListView<TileType> tileTypes = new ListView<TileType>();

        public SGameTileHandler(RecastGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("'graph' cannot be null");
            }
            if (graph.GetTiles() == null)
            {
                throw new ArgumentException("graph has no tiles. Please scan the graph before creating a TileHandler");
            }
            this.activeTileTypes = new TileType[graph.tileXCount * graph.tileZCount];
            this.activeTileRotations = new int[this.activeTileTypes.Length];
            this.activeTileOffsets = new int[this.activeTileTypes.Length];
            this.reloadedInBatch = new bool[this.activeTileTypes.Length];
            this._graph = graph;
        }

        public void ClearTile(int x, int z)
        {
            <ClearTile>c__AnonStorey3A storeya = new <ClearTile>c__AnonStorey3A {
                x = x,
                z = z,
                <>f__this = this
            };
            if ((AstarPath.active != null) && (((storeya.x >= 0) && (storeya.z >= 0)) && ((storeya.x < this.graph.tileXCount) && (storeya.z < this.graph.tileZCount))))
            {
                AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(new Func<bool, bool>(storeya.<>m__2D)));
            }
        }

        public void CreateTileTypesFromGraph()
        {
            RecastGraph.NavmeshTile[] tiles = this.graph.GetTiles();
            if ((tiles == null) || (tiles.Length != (this.graph.tileXCount * this.graph.tileZCount)))
            {
                throw new InvalidOperationException("Graph tiles are invalid (either null or number of tiles is not equal to width*depth of the graph");
            }
            for (int i = 0; i < this.graph.tileZCount; i++)
            {
                for (int j = 0; j < this.graph.tileXCount; j++)
                {
                    RecastGraph.NavmeshTile tile = tiles[j + (i * this.graph.tileXCount)];
                    VInt3 min = (VInt3) this.graph.GetTileBounds(j, i, 1, 1).min;
                    VInt3 tileSize = (VInt3) (new VInt3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize));
                    min += new VInt3((tileSize.x * tile.w) / 2, 0, (tileSize.z * tile.d) / 2);
                    min = -min;
                    TileType item = new TileType(tile.verts, tile.tris, tileSize, min, tile.w, tile.d);
                    this.tileTypes.Add(item);
                    int index = j + (i * this.graph.tileXCount);
                    this.activeTileTypes[index] = item;
                    this.activeTileRotations[index] = 0;
                    this.activeTileOffsets[index] = 0;
                }
            }
        }

        private void CutPoly(ListView<NavmeshCut> InNavmeshCuts, VInt3[] verts, int[] tris, ref VInt3[] outVertsArr, ref int[] outTrisArr, out int outVCount, out int outTCount, VInt3[] extraShape, VInt3 cuttingOffset, Bounds realBounds, CutMode mode = 3, int perturbate = 0)
        {
            ListView<NavmeshCut> view;
            if ((verts.Length == 0) || (tris.Length == 0))
            {
                outVCount = 0;
                outTCount = 0;
                outTrisArr = new int[0];
                outVertsArr = new VInt3[0];
                return;
            }
            List<Pathfinding.ClipperLib.IntPoint> pg = null;
            if ((extraShape == null) && ((mode & CutMode.CutExtra) != ((CutMode) 0)))
            {
                throw new Exception("extraShape is null and the CutMode specifies that it should be used. Cannot use null shape.");
            }
            if ((mode & CutMode.CutExtra) != ((CutMode) 0))
            {
                pg = new List<Pathfinding.ClipperLib.IntPoint>(extraShape.Length);
                for (int num = 0; num < extraShape.Length; num++)
                {
                    pg.Add(new Pathfinding.ClipperLib.IntPoint((long) (extraShape[num].x + cuttingOffset.x), (long) (extraShape[num].z + cuttingOffset.z)));
                }
            }
            List<Pathfinding.ClipperLib.IntPoint> list2 = new List<Pathfinding.ClipperLib.IntPoint>(5);
            Dictionary<TriangulationPoint, int> dictionary = new Dictionary<TriangulationPoint, int>();
            List<PolygonPoint> list3 = new List<PolygonPoint>();
            IntRect b = new IntRect(verts[0].x, verts[0].z, verts[0].x, verts[0].z);
            for (int i = 0; i < verts.Length; i++)
            {
                b = b.ExpandToContain(verts[i].x, verts[i].z);
            }
            List<VInt3> list = ListPool<VInt3>.Claim(verts.Length * 2);
            List<int> list5 = ListPool<int>.Claim(tris.Length);
            Pathfinding.ClipperLib.PolyTree polytree = new Pathfinding.ClipperLib.PolyTree();
            List<List<Pathfinding.ClipperLib.IntPoint>> solution = new List<List<Pathfinding.ClipperLib.IntPoint>>();
            Stack<Polygon> stack = new Stack<Polygon>();
            if (this.clipper == null)
            {
                this.clipper = new Pathfinding.ClipperLib.Clipper(0);
            }
            this.clipper.ReverseSolution = true;
            this.clipper.StrictlySimple = true;
            if (mode == CutMode.CutExtra)
            {
                view = new ListView<NavmeshCut>();
            }
            else
            {
                view = new ListView<NavmeshCut>(InNavmeshCuts);
            }
            List<int> list7 = ListPool<int>.Claim();
            List<IntRect> list8 = ListPool<IntRect>.Claim();
            List<VInt2> list9 = ListPool<VInt2>.Claim();
            List<List<Pathfinding.ClipperLib.IntPoint>> buffer = new List<List<Pathfinding.ClipperLib.IntPoint>>();
            List<bool> list11 = ListPool<bool>.Claim();
            List<bool> list12 = ListPool<bool>.Claim();
            if (perturbate > 10)
            {
                Debug.LogError("Too many perturbations aborting : " + mode);
                Debug.Break();
                outVCount = verts.Length;
                outTCount = tris.Length;
                outTrisArr = tris;
                outVertsArr = verts;
                return;
            }
            System.Random random = null;
            if (perturbate > 0)
            {
                random = new System.Random();
            }
            for (int j = 0; j < view.Count; j++)
            {
                Bounds bounds = view[j].GetBounds();
                VInt3 num4 = ((VInt3) bounds.min) + cuttingOffset;
                VInt3 num5 = ((VInt3) bounds.max) + cuttingOffset;
                IntRect a = new IntRect(num4.x, num4.z, num5.x, num5.z);
                if (IntRect.Intersects(a, b))
                {
                    VInt2 num6 = new VInt2(0, 0);
                    if (perturbate > 0)
                    {
                        num6.x = ((random.Next() % 6) * perturbate) - (3 * perturbate);
                        if (num6.x >= 0)
                        {
                            num6.x++;
                        }
                        num6.y = ((random.Next() % 6) * perturbate) - (3 * perturbate);
                        if (num6.y >= 0)
                        {
                            num6.y++;
                        }
                    }
                    int count = buffer.Count;
                    view[j].GetContour(buffer);
                    for (int num8 = count; num8 < buffer.Count; num8++)
                    {
                        List<Pathfinding.ClipperLib.IntPoint> list13 = buffer[num8];
                        if (list13.Count == 0)
                        {
                            Debug.LogError("Zero Length Contour");
                            IntRect item = new IntRect();
                            list8.Add(item);
                            list9.Add(new VInt2(0, 0));
                        }
                        else
                        {
                            Pathfinding.ClipperLib.IntPoint point3 = list13[0];
                            Pathfinding.ClipperLib.IntPoint point4 = list13[0];
                            Pathfinding.ClipperLib.IntPoint point5 = list13[0];
                            Pathfinding.ClipperLib.IntPoint point6 = list13[0];
                            IntRect rect3 = new IntRect(((int) point3.X) + cuttingOffset.x, ((int) point4.Y) + cuttingOffset.y, ((int) point5.X) + cuttingOffset.x, ((int) point6.Y) + cuttingOffset.y);
                            for (int num9 = 0; num9 < list13.Count; num9++)
                            {
                                Pathfinding.ClipperLib.IntPoint point = list13[num9];
                                point.X += cuttingOffset.x;
                                point.Y += cuttingOffset.z;
                                if (perturbate > 0)
                                {
                                    point.X += num6.x;
                                    point.Y += num6.y;
                                }
                                list13[num9] = point;
                                rect3 = rect3.ExpandToContain((int) point.X, (int) point.Y);
                            }
                            list9.Add(new VInt2(num4.y, num5.y));
                            list8.Add(rect3);
                            list11.Add(view[j].isDual);
                            list12.Add(view[j].cutsAddedGeom);
                        }
                    }
                }
            }
            ListView<NavmeshAdd> view2 = new ListView<NavmeshAdd>();
            VInt3[] vbuffer = verts;
            int[] tbuffer = tris;
            int num10 = -1;
            int index = -3;
            VInt3[] vIn = null;
            VInt3[] vOut = null;
            VInt3 zero = VInt3.zero;
            if (view2.Count > 0)
            {
                vIn = new VInt3[7];
                vOut = new VInt3[7];
                zero = (VInt3) realBounds.extents;
            }
        Label_0546:
            index += 3;
            while (index >= tbuffer.Length)
            {
                num10++;
                index = 0;
                if (num10 >= view2.Count)
                {
                    vbuffer = null;
                    break;
                }
                if (vbuffer == verts)
                {
                    vbuffer = null;
                }
                view2[num10].GetMesh(cuttingOffset, ref vbuffer, out tbuffer);
            }
            if (vbuffer != null)
            {
                VInt3 num13 = vbuffer[tbuffer[index]];
                VInt3 num14 = vbuffer[tbuffer[index + 1]];
                VInt3 num15 = vbuffer[tbuffer[index + 2]];
                IntRect rect4 = new IntRect(num13.x, num13.z, num13.x, num13.z);
                rect4 = rect4.ExpandToContain(num14.x, num14.z).ExpandToContain(num15.x, num15.z);
                int num16 = Math.Min(num13.y, Math.Min(num14.y, num15.y));
                int num17 = Math.Max(num13.y, Math.Max(num14.y, num15.y));
                list7.Clear();
                bool flag = false;
                for (int num18 = 0; num18 < buffer.Count; num18++)
                {
                    VInt2 num54 = list9[num18];
                    int x = num54.x;
                    VInt2 num55 = list9[num18];
                    int y = num55.y;
                    if (((IntRect.Intersects(rect4, list8[num18]) && (y >= num16)) && (x <= num17)) && (list12[num18] || (num10 == -1)))
                    {
                        VInt3 num21 = num13;
                        num21.y = x;
                        VInt3 num22 = num13;
                        num22.y = y;
                        list7.Add(num18);
                        flag |= list11[num18];
                    }
                }
                if (((list7.Count == 0) && ((mode & CutMode.CutExtra) == ((CutMode) 0))) && (((mode & CutMode.CutAll) != ((CutMode) 0)) && (num10 == -1)))
                {
                    list5.Add(list.Count);
                    list5.Add(list.Count + 1);
                    list5.Add(list.Count + 2);
                    list.Add(num13);
                    list.Add(num14);
                    list.Add(num15);
                }
                else
                {
                    list2.Clear();
                    if (num10 == -1)
                    {
                        list2.Add(new Pathfinding.ClipperLib.IntPoint((long) num13.x, (long) num13.z));
                        list2.Add(new Pathfinding.ClipperLib.IntPoint((long) num14.x, (long) num14.z));
                        list2.Add(new Pathfinding.ClipperLib.IntPoint((long) num15.x, (long) num15.z));
                    }
                    else
                    {
                        vIn[0] = num13;
                        vIn[1] = num14;
                        vIn[2] = num15;
                        int num23 = Utility.ClipPolygon(vIn, 3, vOut, 1, 0, 0);
                        if (num23 == 0)
                        {
                            goto Label_0546;
                        }
                        num23 = Utility.ClipPolygon(vOut, num23, vIn, -1, 2 * zero.x, 0);
                        if (num23 == 0)
                        {
                            goto Label_0546;
                        }
                        num23 = Utility.ClipPolygon(vIn, num23, vOut, 1, 0, 2);
                        if (num23 == 0)
                        {
                            goto Label_0546;
                        }
                        num23 = Utility.ClipPolygon(vOut, num23, vIn, -1, 2 * zero.z, 2);
                        if (num23 == 0)
                        {
                            goto Label_0546;
                        }
                        for (int num24 = 0; num24 < num23; num24++)
                        {
                            list2.Add(new Pathfinding.ClipperLib.IntPoint((long) vIn[num24].x, (long) vIn[num24].z));
                        }
                    }
                    dictionary.Clear();
                    VInt3 num25 = num14 - num13;
                    VInt3 num26 = num15 - num13;
                    VInt3 num27 = num25;
                    VInt3 num28 = num26;
                    num27.y = 0;
                    num28.y = 0;
                    for (int num29 = 0; num29 < 0x10; num29++)
                    {
                        if (((((int) mode) >> num29) & 1) != 0)
                        {
                            if ((((int) 1) << num29) == 1)
                            {
                                this.clipper.Clear();
                                this.clipper.AddPolygon(list2, Pathfinding.ClipperLib.PolyType.ptSubject);
                                for (int num30 = 0; num30 < list7.Count; num30++)
                                {
                                    this.clipper.AddPolygon(buffer[list7[num30]], Pathfinding.ClipperLib.PolyType.ptClip);
                                }
                                polytree.Clear();
                                this.clipper.Execute(Pathfinding.ClipperLib.ClipType.ctDifference, polytree, Pathfinding.ClipperLib.PolyFillType.pftEvenOdd, Pathfinding.ClipperLib.PolyFillType.pftNonZero);
                            }
                            else if ((((int) 1) << num29) == 2)
                            {
                                if (!flag)
                                {
                                    continue;
                                }
                                this.clipper.Clear();
                                this.clipper.AddPolygon(list2, Pathfinding.ClipperLib.PolyType.ptSubject);
                                for (int num31 = 0; num31 < list7.Count; num31++)
                                {
                                    if (list11[list7[num31]])
                                    {
                                        this.clipper.AddPolygon(buffer[list7[num31]], Pathfinding.ClipperLib.PolyType.ptClip);
                                    }
                                }
                                solution.Clear();
                                this.clipper.Execute(Pathfinding.ClipperLib.ClipType.ctIntersection, solution, Pathfinding.ClipperLib.PolyFillType.pftEvenOdd, Pathfinding.ClipperLib.PolyFillType.pftNonZero);
                                this.clipper.Clear();
                                for (int num32 = 0; num32 < solution.Count; num32++)
                                {
                                    this.clipper.AddPolygon(solution[num32], !Pathfinding.ClipperLib.Clipper.Orientation(solution[num32]) ? Pathfinding.ClipperLib.PolyType.ptSubject : Pathfinding.ClipperLib.PolyType.ptClip);
                                }
                                for (int num33 = 0; num33 < list7.Count; num33++)
                                {
                                    if (!list11[list7[num33]])
                                    {
                                        this.clipper.AddPolygon(buffer[list7[num33]], Pathfinding.ClipperLib.PolyType.ptClip);
                                    }
                                }
                                polytree.Clear();
                                this.clipper.Execute(Pathfinding.ClipperLib.ClipType.ctDifference, polytree, Pathfinding.ClipperLib.PolyFillType.pftEvenOdd, Pathfinding.ClipperLib.PolyFillType.pftNonZero);
                            }
                            else if ((((int) 1) << num29) == 4)
                            {
                                this.clipper.Clear();
                                this.clipper.AddPolygon(list2, Pathfinding.ClipperLib.PolyType.ptSubject);
                                this.clipper.AddPolygon(pg, Pathfinding.ClipperLib.PolyType.ptClip);
                                polytree.Clear();
                                this.clipper.Execute(Pathfinding.ClipperLib.ClipType.ctIntersection, polytree, Pathfinding.ClipperLib.PolyFillType.pftEvenOdd, Pathfinding.ClipperLib.PolyFillType.pftNonZero);
                            }
                            for (int num34 = 0; num34 < polytree.ChildCount; num34++)
                            {
                                Pathfinding.ClipperLib.PolyNode node = polytree.Childs[num34];
                                List<Pathfinding.ClipperLib.IntPoint> contour = node.Contour;
                                List<Pathfinding.ClipperLib.PolyNode> childs = node.Childs;
                                if (((childs.Count == 0) && (contour.Count == 3)) && (num10 == -1))
                                {
                                    for (int num35 = 0; num35 < contour.Count; num35++)
                                    {
                                        Pathfinding.ClipperLib.IntPoint point7 = contour[num35];
                                        Pathfinding.ClipperLib.IntPoint point8 = contour[num35];
                                        VInt3 num36 = new VInt3((int) point7.X, 0, (int) point8.Y);
                                        double num37 = ((num14.z - num15.z) * (num13.x - num15.x)) + ((num15.x - num14.x) * (num13.z - num15.z));
                                        if (num37 == 0.0)
                                        {
                                            Debug.LogWarning("Degenerate triangle");
                                        }
                                        else
                                        {
                                            double num38 = (((num14.z - num15.z) * (num36.x - num15.x)) + ((num15.x - num14.x) * (num36.z - num15.z))) / num37;
                                            double num39 = (((num15.z - num13.z) * (num36.x - num15.x)) + ((num13.x - num15.x) * (num36.z - num15.z))) / num37;
                                            num36.y = (int) Math.Round((double) (((num38 * num13.y) + (num39 * num14.y)) + (((1.0 - num38) - num39) * num15.y)));
                                            list5.Add(list.Count);
                                            list.Add(num36);
                                        }
                                    }
                                }
                                else
                                {
                                    Polygon t = null;
                                    int num40 = -1;
                                    for (List<Pathfinding.ClipperLib.IntPoint> list16 = contour; list16 != null; list16 = (num40 >= childs.Count) ? null : childs[num40].Contour)
                                    {
                                        list3.Clear();
                                        for (int num41 = 0; num41 < list16.Count; num41++)
                                        {
                                            Pathfinding.ClipperLib.IntPoint point9 = list16[num41];
                                            Pathfinding.ClipperLib.IntPoint point10 = list16[num41];
                                            PolygonPoint point2 = new PolygonPoint((double) point9.X, (double) point10.Y);
                                            list3.Add(point2);
                                            Pathfinding.ClipperLib.IntPoint point11 = list16[num41];
                                            Pathfinding.ClipperLib.IntPoint point12 = list16[num41];
                                            VInt3 num42 = new VInt3((int) point11.X, 0, (int) point12.Y);
                                            double num43 = ((num14.z - num15.z) * (num13.x - num15.x)) + ((num15.x - num14.x) * (num13.z - num15.z));
                                            if (num43 == 0.0)
                                            {
                                                Debug.LogWarning("Degenerate triangle");
                                            }
                                            else
                                            {
                                                double num44 = (((num14.z - num15.z) * (num42.x - num15.x)) + ((num15.x - num14.x) * (num42.z - num15.z))) / num43;
                                                double num45 = (((num15.z - num13.z) * (num42.x - num15.x)) + ((num13.x - num15.x) * (num42.z - num15.z))) / num43;
                                                num42.y = (int) Math.Round((double) (((num44 * num13.y) + (num45 * num14.y)) + (((1.0 - num44) - num45) * num15.y)));
                                                dictionary[point2] = list.Count;
                                                list.Add(num42);
                                            }
                                        }
                                        Polygon polygon2 = null;
                                        if (stack.Count > 0)
                                        {
                                            polygon2 = stack.Pop();
                                            polygon2.AddPoints(list3);
                                        }
                                        else
                                        {
                                            polygon2 = new Polygon(list3);
                                        }
                                        if (t == null)
                                        {
                                            t = polygon2;
                                        }
                                        else
                                        {
                                            t.AddHole(polygon2);
                                        }
                                        num40++;
                                    }
                                    try
                                    {
                                        P2T.Triangulate(t);
                                    }
                                    catch (PointOnEdgeException)
                                    {
                                        Debug.LogWarning(string.Concat(new object[] { "PointOnEdgeException, perturbating vertices slightly ( at ", num29, " in ", mode, ")" }));
                                        this.CutPoly(InNavmeshCuts, verts, tris, ref outVertsArr, ref outTrisArr, out outVCount, out outTCount, extraShape, cuttingOffset, realBounds, mode, perturbate + 1);
                                        return;
                                    }
                                    for (int num46 = 0; num46 < t.get_Triangles().Count; num46++)
                                    {
                                        DelaunayTriangle triangle = t.get_Triangles()[num46];
                                        list5.Add(dictionary[triangle.Points._0]);
                                        list5.Add(dictionary[triangle.Points._1]);
                                        list5.Add(dictionary[triangle.Points._2]);
                                    }
                                    if (t.get_Holes() != null)
                                    {
                                        for (int num47 = 0; num47 < t.get_Holes().Count; num47++)
                                        {
                                            t.get_Holes()[num47].get_Points().Clear();
                                            t.get_Holes()[num47].ClearTriangles();
                                            if (t.get_Holes()[num47].get_Holes() != null)
                                            {
                                                t.get_Holes()[num47].get_Holes().Clear();
                                            }
                                            stack.Push(t.get_Holes()[num47]);
                                        }
                                    }
                                    t.ClearTriangles();
                                    if (t.get_Holes() != null)
                                    {
                                        t.get_Holes().Clear();
                                    }
                                    t.get_Points().Clear();
                                    stack.Push(t);
                                }
                            }
                        }
                    }
                }
                goto Label_0546;
            }
            Dictionary<VInt3, int> dictionary2 = this.cached_Int3_int_dict;
            dictionary2.Clear();
            if (this.cached_int_array.Length < list.Count)
            {
                this.cached_int_array = new int[Math.Max(this.cached_int_array.Length * 2, list.Count)];
            }
            int[] numArray5 = this.cached_int_array;
            int num48 = 0;
            for (int k = 0; k < list.Count; k++)
            {
                int num50;
                if (!dictionary2.TryGetValue(list[k], out num50))
                {
                    dictionary2.Add(list[k], num48);
                    numArray5[k] = num48;
                    list[num48] = list[k];
                    num48++;
                }
                else
                {
                    numArray5[k] = num50;
                }
            }
            outTCount = list5.Count;
            if ((outTrisArr == null) || (outTrisArr.Length < outTCount))
            {
                outTrisArr = new int[outTCount];
            }
            for (int m = 0; m < outTCount; m++)
            {
                outTrisArr[m] = numArray5[list5[m]];
            }
            outVCount = num48;
            if ((outVertsArr == null) || (outVertsArr.Length < outVCount))
            {
                outVertsArr = new VInt3[outVCount];
            }
            for (int n = 0; n < outVCount; n++)
            {
                outVertsArr[n] = list[n];
            }
            for (int num53 = 0; num53 < view.Count; num53++)
            {
                view[num53].UsedForCut();
            }
            ListPool<VInt3>.Release(list);
            ListPool<int>.Release(list5);
            ListPool<int>.Release(list7);
            ListPool<VInt2>.Release(list9);
            ListPool<bool>.Release(list11);
            ListPool<bool>.Release(list12);
            ListPool<IntRect>.Release(list8);
        }

        private void DelaunayRefinement(VInt3[] verts, int[] tris, ref int vCount, ref int tCount, bool delaunay, bool colinear, VInt3 worldOffset)
        {
            if ((tCount % 3) != 0)
            {
                throw new Exception("Triangle array length must be a multiple of 3");
            }
            Dictionary<VInt2, int> dictionary = this.cached_Int2_int_dict;
            dictionary.Clear();
            for (int i = 0; i < tCount; i += 3)
            {
                if (!Polygon.IsClockwise(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]))
                {
                    int num2 = tris[i];
                    tris[i] = tris[i + 2];
                    tris[i + 2] = num2;
                }
                dictionary[new VInt2(tris[i], tris[i + 1])] = i + 2;
                dictionary[new VInt2(tris[i + 1], tris[i + 2])] = i;
                dictionary[new VInt2(tris[i + 2], tris[i])] = i + 1;
            }
            int num3 = 9;
            for (int j = 0; j < tCount; j += 3)
            {
                for (int k = 0; k < 3; k++)
                {
                    int num6;
                    if (dictionary.TryGetValue(new VInt2(tris[j + ((k + 1) % 3)], tris[j + (k % 3)]), out num6))
                    {
                        VInt3 a = verts[tris[j + ((k + 2) % 3)]];
                        VInt3 b = verts[tris[j + ((k + 1) % 3)]];
                        VInt3 num9 = verts[tris[j + ((k + 3) % 3)]];
                        VInt3 c = verts[tris[num6]];
                        a.y = 0;
                        b.y = 0;
                        num9.y = 0;
                        c.y = 0;
                        bool flag = false;
                        if (!Polygon.Left(a, num9, c) || Polygon.LeftNotColinear(a, b, c))
                        {
                            if (!colinear)
                            {
                                continue;
                            }
                            flag = true;
                        }
                        if ((colinear && (AstarMath.DistancePointSegment(a, c, b) < num3)) && (!dictionary.ContainsKey(new VInt2(tris[j + ((k + 2) % 3)], tris[j + ((k + 1) % 3)])) && !dictionary.ContainsKey(new VInt2(tris[j + ((k + 1) % 3)], tris[num6]))))
                        {
                            tCount -= 3;
                            int index = (num6 / 3) * 3;
                            tris[j + ((k + 1) % 3)] = tris[num6];
                            if (index != tCount)
                            {
                                tris[index] = tris[tCount];
                                tris[index + 1] = tris[tCount + 1];
                                tris[index + 2] = tris[tCount + 2];
                                dictionary[new VInt2(tris[index], tris[index + 1])] = index + 2;
                                dictionary[new VInt2(tris[index + 1], tris[index + 2])] = index;
                                dictionary[new VInt2(tris[index + 2], tris[index])] = index + 1;
                                tris[tCount] = 0;
                                tris[tCount + 1] = 0;
                                tris[tCount + 2] = 0;
                            }
                            else
                            {
                                tCount += 3;
                            }
                            dictionary[new VInt2(tris[j], tris[j + 1])] = j + 2;
                            dictionary[new VInt2(tris[j + 1], tris[j + 2])] = j;
                            dictionary[new VInt2(tris[j + 2], tris[j])] = j + 1;
                        }
                        else if (delaunay && !flag)
                        {
                            float num12 = VInt3.Angle(b - a, num9 - a);
                            if (VInt3.Angle(b - c, num9 - c) > (6.283185f - (2f * num12)))
                            {
                                tris[j + ((k + 1) % 3)] = tris[num6];
                                int num14 = (num6 / 3) * 3;
                                int num15 = num6 - num14;
                                tris[num14 + (((num15 - 1) + 3) % 3)] = tris[j + ((k + 2) % 3)];
                                dictionary[new VInt2(tris[j], tris[j + 1])] = j + 2;
                                dictionary[new VInt2(tris[j + 1], tris[j + 2])] = j;
                                dictionary[new VInt2(tris[j + 2], tris[j])] = j + 1;
                                dictionary[new VInt2(tris[num14], tris[num14 + 1])] = num14 + 2;
                                dictionary[new VInt2(tris[num14 + 1], tris[num14 + 2])] = num14;
                                dictionary[new VInt2(tris[num14 + 2], tris[num14])] = num14 + 1;
                            }
                        }
                    }
                }
            }
        }

        public void EndBatchLoad()
        {
            if (this.isBatching)
            {
                for (int i = 0; i < this.reloadedInBatch.Length; i++)
                {
                    this.reloadedInBatch[i] = false;
                }
                this.isBatching = false;
                this.graph.EndBatchTileUpdate();
            }
        }

        public int GetActiveRotation(VInt2 p)
        {
            return this.activeTileRotations[p.x + (p.y * this._graph.tileXCount)];
        }

        public TileType GetTileType(int index)
        {
            return this.tileTypes[index];
        }

        public int GetTileTypeCount()
        {
            return this.tileTypes.Count;
        }

        private VInt3 IntPoint2Int3(Pathfinding.ClipperLib.IntPoint p)
        {
            return new VInt3((int) p.X, 0, (int) p.Y);
        }

        public void LoadTile(ListView<NavmeshCut> navmeshCuts, TileType tile, int x, int z, int rotation, int yoffset)
        {
            if (tile == null)
            {
                throw new ArgumentNullException("tile");
            }
            int index = x + (z * this.graph.tileXCount);
            rotation = rotation % 4;
            if (((!this.isBatching || !this.reloadedInBatch[index]) || ((this.activeTileOffsets[index] != yoffset) || (this.activeTileRotations[index] != rotation))) || (this.activeTileTypes[index] != tile))
            {
                if (this.isBatching)
                {
                    this.reloadedInBatch[index] = true;
                }
                this.activeTileOffsets[index] = yoffset;
                this.activeTileRotations[index] = rotation;
                this.activeTileTypes[index] = tile;
                if (((this.activeTileOffsets[index] == yoffset) && (this.activeTileRotations[index] == rotation)) && (this.activeTileTypes[index] == tile))
                {
                    VInt3[] numArray;
                    int[] numArray2;
                    int num3;
                    int num4;
                    GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
                    tile.Load(out numArray, out numArray2, rotation, yoffset);
                    Bounds realBounds = this.graph.GetTileBounds(x, z, tile.Width, tile.Depth);
                    VInt3 min = (VInt3) realBounds.min;
                    min = -min;
                    VInt3[] outVertsArr = null;
                    int[] outTrisArr = null;
                    this.CutPoly(navmeshCuts, numArray, numArray2, ref outVertsArr, ref outTrisArr, out num3, out num4, null, min, realBounds, CutMode.CutDual | CutMode.CutAll, 0);
                    this.DelaunayRefinement(outVertsArr, outTrisArr, ref num3, ref num4, true, false, -min);
                    if (num4 != outTrisArr.Length)
                    {
                        outTrisArr = ShrinkArray<int>(outTrisArr, num4);
                    }
                    if (num3 != outVertsArr.Length)
                    {
                        outVertsArr = ShrinkArray<VInt3>(outVertsArr, num3);
                    }
                    int w = ((rotation % 2) != 0) ? tile.Depth : tile.Width;
                    int d = ((rotation % 2) != 0) ? tile.Width : tile.Depth;
                    this.graph.ReplaceTile(x, z, w, d, outVertsArr, outTrisArr, false);
                    GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
                }
            }
        }

        private Vector3 Point2D2V3(TriangulationPoint p)
        {
            return (Vector3) (new Vector3((float) p.X, 0f, (float) p.Y) * 0.001f);
        }

        public TileType RegisterTileType(Mesh source, VInt3 centerOffset, int width = 1, int depth = 1)
        {
            TileType item = new TileType(source, (VInt3) (new VInt3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize)), centerOffset, width, depth);
            this.tileTypes.Add(item);
            return item;
        }

        public void ReloadTile(ListView<NavmeshCut> navmeshCuts, int x, int z)
        {
            if (((x >= 0) && (z >= 0)) && ((x < this.graph.tileXCount) && (z < this.graph.tileZCount)))
            {
                int index = x + (z * this.graph.tileXCount);
                if (this.activeTileTypes[index] != null)
                {
                    this.graph.StartBatchTileUpdate();
                    this.LoadTile(navmeshCuts, this.activeTileTypes[index], x, z, this.activeTileRotations[index], this.activeTileOffsets[index]);
                    this.graph.EndBatchTileUpdate();
                }
            }
        }

        public void ReloadTiles(ListView<NavmeshCut> navmeshCuts)
        {
            this.graph.StartBatchTileUpdate();
            for (int i = 0; i < this.graph.tileXCount; i++)
            {
                for (int j = 0; j < this.graph.tileZCount; j++)
                {
                    int index = i + (j * this.graph.tileXCount);
                    if (this.activeTileTypes[index] != null)
                    {
                        this.LoadTile(navmeshCuts, this.activeTileTypes[index], i, j, this.activeTileRotations[index], this.activeTileOffsets[index]);
                    }
                }
            }
            this.graph.EndBatchTileUpdate();
        }

        protected static T[] ShrinkArray<T>(T[] arr, int newLength)
        {
            newLength = Math.Min(newLength, arr.Length);
            T[] localArray = new T[newLength];
            if ((newLength % 4) == 0)
            {
                for (int j = 0; j < newLength; j += 4)
                {
                    localArray[j] = arr[j];
                    localArray[j + 1] = arr[j + 1];
                    localArray[j + 2] = arr[j + 2];
                    localArray[j + 3] = arr[j + 3];
                }
                return localArray;
            }
            if ((newLength % 3) == 0)
            {
                for (int k = 0; k < newLength; k += 3)
                {
                    localArray[k] = arr[k];
                    localArray[k + 1] = arr[k + 1];
                    localArray[k + 2] = arr[k + 2];
                }
                return localArray;
            }
            if ((newLength % 2) == 0)
            {
                for (int m = 0; m < newLength; m += 2)
                {
                    localArray[m] = arr[m];
                    localArray[m + 1] = arr[m + 1];
                }
                return localArray;
            }
            for (int i = 0; i < newLength; i++)
            {
                localArray[i] = arr[i];
            }
            return localArray;
        }

        public bool StartBatchLoad()
        {
            if (this.isBatching)
            {
                return false;
            }
            this.isBatching = true;
            this.graph.StartBatchTileUpdate();
            return true;
        }

        public RecastGraph graph
        {
            get
            {
                return this._graph;
            }
        }

        [CompilerGenerated]
        private sealed class <ClearTile>c__AnonStorey3A
        {
            internal SGameTileHandler <>f__this;
            internal int x;
            internal int z;

            internal bool <>m__2D(bool force)
            {
                this.<>f__this.graph.ReplaceTile(this.x, this.z, new VInt3[0], new int[0], false);
                this.<>f__this.activeTileTypes[this.x + (this.z * this.<>f__this.graph.tileXCount)] = null;
                GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
                AstarPath.active.QueueWorkItemFloodFill();
                return true;
            }
        }

        public enum CutMode
        {
            CutAll = 1,
            CutDual = 2,
            CutExtra = 4
        }

        public class TileType
        {
            private int depth;
            private int lastRotation;
            private int lastYOffset;
            private VInt3 offset;
            private static readonly int[] Rotations = new int[] { 1, 0, 0, 1, 0, 1, -1, 0, -1, 0, 0, -1, 0, -1, 1, 0 };
            private int[] tris;
            private VInt3[] verts;
            private int width;

            public TileType(Mesh source, VInt3 tileSize, VInt3 centerOffset, int width = 1, int depth = 1)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                Vector3[] vertices = source.vertices;
                this.tris = source.triangles;
                this.verts = new VInt3[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    this.verts[i] = ((VInt3) vertices[i]) + centerOffset;
                }
                this.offset = (VInt3) (tileSize / 2f);
                this.offset.x *= width;
                this.offset.z *= depth;
                this.offset.y = 0;
                for (int j = 0; j < vertices.Length; j++)
                {
                    this.verts[j] += this.offset;
                }
                this.lastRotation = 0;
                this.lastYOffset = 0;
                this.width = width;
                this.depth = depth;
            }

            public TileType(VInt3[] sourceVerts, int[] sourceTris, VInt3 tileSize, VInt3 centerOffset, int width = 1, int depth = 1)
            {
                if (sourceVerts == null)
                {
                    throw new ArgumentNullException("sourceVerts");
                }
                if (sourceTris == null)
                {
                    throw new ArgumentNullException("sourceTris");
                }
                this.tris = new int[sourceTris.Length];
                for (int i = 0; i < this.tris.Length; i++)
                {
                    this.tris[i] = sourceTris[i];
                }
                this.verts = new VInt3[sourceVerts.Length];
                for (int j = 0; j < sourceVerts.Length; j++)
                {
                    this.verts[j] = sourceVerts[j] + centerOffset;
                }
                this.offset = (VInt3) (tileSize / 2f);
                this.offset.x *= width;
                this.offset.z *= depth;
                this.offset.y = 0;
                for (int k = 0; k < sourceVerts.Length; k++)
                {
                    this.verts[k] += this.offset;
                }
                this.lastRotation = 0;
                this.lastYOffset = 0;
                this.width = width;
                this.depth = depth;
            }

            public void Load(out VInt3[] verts, out int[] tris, int rotation, int yoffset)
            {
                rotation = ((rotation % 4) + 4) % 4;
                int num = rotation;
                rotation = ((rotation - (this.lastRotation % 4)) + 4) % 4;
                this.lastRotation = num;
                verts = this.verts;
                int num2 = yoffset - this.lastYOffset;
                this.lastYOffset = yoffset;
                if ((rotation != 0) || (num2 != 0))
                {
                    for (int i = 0; i < verts.Length; i++)
                    {
                        VInt3 num4 = verts[i] - this.offset;
                        VInt3 num5 = num4;
                        num5.y += num2;
                        num5.x = (num4.x * Rotations[rotation * 4]) + (num4.z * Rotations[(rotation * 4) + 1]);
                        num5.z = (num4.x * Rotations[(rotation * 4) + 2]) + (num4.z * Rotations[(rotation * 4) + 3]);
                        verts[i] = num5 + this.offset;
                    }
                }
                tris = this.tris;
            }

            public int Depth
            {
                get
                {
                    return this.depth;
                }
            }

            public int Width
            {
                get
                {
                    return this.width;
                }
            }
        }
    }
}

