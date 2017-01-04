namespace Pathfinding.Voxels
{
    using Pathfinding;
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Voxelize
    {
        public const ushort BorderReg = 0x8000;
        public int borderSize;
        public readonly float cellHeight = 0.1f;
        public readonly Vector3 cellScale;
        public readonly Vector3 cellScaleDivision;
        public readonly float cellSize = 0.2f;
        public const int ContourRegMask = 0xffff;
        public VoxelContourSet countourSet;
        public string debugString = string.Empty;
        public int depth;
        private static readonly int[] emptyArr = new int[0];
        public Bounds forcedBounds;
        public List<ExtraMesh> inputExtraMeshes;
        protected int[] inputTriangles;
        protected Vector3[] inputVertices;
        private static List<int[]> intArrCache = new List<int[]>();
        public float maxEdgeLength = 20f;
        public const int MaxLayers = 0xffff;
        public const int MaxRegions = 500;
        public float maxSlope = 30f;
        public int minRegionSize = 100;
        public const uint NotConnected = 0x3f;
        public const int RC_AREA_BORDER = 0x20000;
        public const int RC_BORDER_VERTEX = 0x10000;
        public const int RC_CONTOUR_TESS_AREA_EDGES = 2;
        public const int RC_CONTOUR_TESS_WALL_EDGES = 1;
        public RecastGraph.RelevantGraphSurfaceMode relevantGraphSurfaceMode;
        public const int UnwalkableArea = 0;
        public const int VERTEX_BUCKET_COUNT = 0x1000;
        public VoxelArea voxelArea;
        public Vector3 voxelOffset;
        public readonly int voxelWalkableClimb;
        public readonly uint voxelWalkableHeight;
        public int width;

        public Voxelize(float ch, float cs, float wc, float wh, float ms)
        {
            this.cellSize = cs;
            this.cellHeight = ch;
            float num = wh;
            float num2 = wc;
            this.maxSlope = ms;
            this.cellScale = new Vector3(this.cellSize, this.cellHeight, this.cellSize);
            this.cellScaleDivision = new Vector3(1f / this.cellSize, 1f / this.cellHeight, 1f / this.cellSize);
            this.voxelWalkableHeight = (uint) (num / this.cellHeight);
            this.voxelWalkableClimb = Mathf.RoundToInt(num2 / this.cellHeight);
        }

        public static int Area2(int a, int b, int c, int[] verts)
        {
            return (((verts[b] - verts[a]) * (verts[c + 2] - verts[a + 2])) - ((verts[c] - verts[a]) * (verts[b + 2] - verts[a + 2])));
        }

        private static bool Between(int a, int b, int c, int[] verts)
        {
            if (!Collinear(a, b, c, verts))
            {
                return false;
            }
            if (verts[a] != verts[b])
            {
                return (((verts[a] <= verts[c]) && (verts[c] <= verts[b])) || ((verts[a] >= verts[c]) && (verts[c] >= verts[b])));
            }
            return (((verts[a + 2] <= verts[c + 2]) && (verts[c + 2] <= verts[b + 2])) || ((verts[a + 2] >= verts[c + 2]) && (verts[c + 2] >= verts[b + 2])));
        }

        public ushort[] BoxBlur(ushort[] src, ushort[] dst)
        {
            ushort num = 20;
            int num2 = this.voxelArea.width * this.voxelArea.depth;
            for (int i = num2 - this.voxelArea.width; i >= 0; i -= this.voxelArea.width)
            {
                for (int j = this.voxelArea.width - 1; j >= 0; j--)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[j + i];
                    int index = (int) cell.index;
                    int num6 = (int) (cell.index + cell.count);
                    while (index < num6)
                    {
                        CompactVoxelSpan span = this.voxelArea.compactSpans[index];
                        ushort num7 = src[index];
                        if (num7 < num)
                        {
                            dst[index] = num7;
                        }
                        else
                        {
                            int num8 = num7;
                            for (int k = 0; k < 4; k++)
                            {
                                if (span.GetConnection(k) != 0x3fL)
                                {
                                    int num10 = j + this.voxelArea.DirectionX[k];
                                    int num11 = i + this.voxelArea.DirectionZ[k];
                                    int num12 = ((int) this.voxelArea.compactCells[num10 + num11].index) + span.GetConnection(k);
                                    num8 += src[num12];
                                    CompactVoxelSpan span2 = this.voxelArea.compactSpans[num12];
                                    int dir = (k + 1) & 3;
                                    if (span2.GetConnection(dir) != 0x3fL)
                                    {
                                        int num14 = num10 + this.voxelArea.DirectionX[dir];
                                        int num15 = num11 + this.voxelArea.DirectionZ[dir];
                                        int num16 = ((int) this.voxelArea.compactCells[num14 + num15].index) + span2.GetConnection(dir);
                                        num8 += src[num16];
                                    }
                                    else
                                    {
                                        num8 += num7;
                                    }
                                }
                                else
                                {
                                    num8 += num7 * 2;
                                }
                            }
                            dst[index] = (ushort) (((float) (num8 + 5)) / 9f);
                        }
                        index++;
                    }
                }
            }
            return dst;
        }

        public void BuildCompactField()
        {
            int spanCount = this.voxelArea.GetSpanCount();
            this.voxelArea.compactSpanCount = spanCount;
            if ((this.voxelArea.compactSpans == null) || (this.voxelArea.compactSpans.Length < spanCount))
            {
                this.voxelArea.compactSpans = new CompactVoxelSpan[spanCount];
                this.voxelArea.areaTypes = new int[spanCount];
            }
            uint index = 0;
            int width = this.voxelArea.width;
            int depth = this.voxelArea.depth;
            int num5 = width * depth;
            if (this.voxelWalkableHeight >= 0xffff)
            {
                Debug.LogWarning("Too high walkable height to guarantee correctness. Increase voxel height or lower walkable height.");
            }
            LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
            int num6 = 0;
            for (int i = 0; num6 < num5; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int next = j + num6;
                    if (linkedSpans[next].bottom == uint.MaxValue)
                    {
                        this.voxelArea.compactCells[j + num6] = new CompactVoxelCell(0, 0);
                    }
                    else
                    {
                        uint num10 = index;
                        uint c = 0;
                        while (next != -1)
                        {
                            if (linkedSpans[next].area != 0)
                            {
                                int top = (int) linkedSpans[next].top;
                                int num13 = linkedSpans[next].next;
                                int num14 = (num13 == -1) ? 0x10000 : ((int) linkedSpans[num13].bottom);
                                this.voxelArea.compactSpans[index] = new CompactVoxelSpan((top <= 0xffff) ? ((ushort) top) : ((ushort) 0xffff), ((num14 - top) <= 0xffff) ? ((uint) (num14 - top)) : 0xffff);
                                this.voxelArea.areaTypes[index] = linkedSpans[next].area;
                                index++;
                                c++;
                            }
                            next = linkedSpans[next].next;
                        }
                        this.voxelArea.compactCells[j + num6] = new CompactVoxelCell(num10, c);
                    }
                }
                num6 += width;
            }
        }

        public void BuildContours(float maxError, int maxEdgeLength, VoxelContourSet cset, int buildFlags)
        {
            int width = this.voxelArea.width;
            int depth = this.voxelArea.depth;
            int num3 = width * depth;
            List<VoxelContour> list = new List<VoxelContour>(Mathf.Max(8, 8));
            ushort[] tmpUShortArr = this.voxelArea.tmpUShortArr;
            if (tmpUShortArr.Length < this.voxelArea.compactSpanCount)
            {
                tmpUShortArr = this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount];
            }
            for (int i = 0; i < num3; i += this.voxelArea.width)
            {
                for (int m = 0; m < this.voxelArea.width; m++)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[m + i];
                    int index = (int) cell.index;
                    int num8 = (int) (cell.index + cell.count);
                    while (index < num8)
                    {
                        ushort num9 = 0;
                        CompactVoxelSpan span = this.voxelArea.compactSpans[index];
                        if ((span.reg == 0) || ((span.reg & 0x8000) == 0x8000))
                        {
                            tmpUShortArr[index] = 0;
                        }
                        else
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                int reg = 0;
                                if (span.GetConnection(n) != 0x3fL)
                                {
                                    int num12 = m + this.voxelArea.DirectionX[n];
                                    int num13 = i + this.voxelArea.DirectionZ[n];
                                    int num14 = ((int) this.voxelArea.compactCells[num12 + num13].index) + span.GetConnection(n);
                                    reg = this.voxelArea.compactSpans[num14].reg;
                                }
                                if (reg == span.reg)
                                {
                                    num9 = (ushort) (num9 | ((ushort) (((int) 1) << n)));
                                }
                            }
                            tmpUShortArr[index] = (ushort) (num9 ^ 15);
                        }
                        index++;
                    }
                }
            }
            List<int> verts = ListPool<int>.Claim(0x100);
            List<int> simplified = ListPool<int>.Claim(0x40);
            for (int j = 0; j < num3; j += this.voxelArea.width)
            {
                for (int num16 = 0; num16 < this.voxelArea.width; num16++)
                {
                    CompactVoxelCell cell2 = this.voxelArea.compactCells[num16 + j];
                    int num17 = (int) cell2.index;
                    int num18 = (int) (cell2.index + cell2.count);
                    while (num17 < num18)
                    {
                        if ((tmpUShortArr[num17] == 0) || (tmpUShortArr[num17] == 15))
                        {
                            tmpUShortArr[num17] = 0;
                        }
                        else
                        {
                            int num19 = this.voxelArea.compactSpans[num17].reg;
                            if ((num19 != 0) && ((num19 & 0x8000) != 0x8000))
                            {
                                int num20 = this.voxelArea.areaTypes[num17];
                                verts.Clear();
                                simplified.Clear();
                                this.WalkContour(num16, j, num17, tmpUShortArr, verts);
                                this.SimplifyContour(verts, simplified, maxError, maxEdgeLength, buildFlags);
                                this.RemoveDegenerateSegments(simplified);
                                VoxelContour item = new VoxelContour {
                                    verts = ClaimIntArr(simplified.Count, false)
                                };
                                for (int num21 = 0; num21 < simplified.Count; num21++)
                                {
                                    item.verts[num21] = simplified[num21];
                                }
                                item.nverts = simplified.Count / 4;
                                item.reg = num19;
                                item.area = num20;
                                list.Add(item);
                            }
                        }
                        num17++;
                    }
                }
            }
            ListPool<int>.Release(verts);
            ListPool<int>.Release(simplified);
            for (int k = 0; k < list.Count; k++)
            {
                VoxelContour cb = list[k];
                if (this.CalcAreaOfPolygon2D(cb.verts, cb.nverts) < 0)
                {
                    int num23 = -1;
                    for (int num24 = 0; num24 < list.Count; num24++)
                    {
                        if (k != num24)
                        {
                            VoxelContour contour4 = list[num24];
                            if (contour4.nverts > 0)
                            {
                                VoxelContour contour5 = list[num24];
                                if (contour5.reg == cb.reg)
                                {
                                    VoxelContour contour6 = list[num24];
                                    VoxelContour contour7 = list[num24];
                                    if (this.CalcAreaOfPolygon2D(contour6.verts, contour7.nverts) > 0)
                                    {
                                        num23 = num24;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (num23 == -1)
                    {
                        Debug.LogError("rcBuildContours: Could not find merge target for bad contour " + k + ".");
                    }
                    else
                    {
                        Debug.LogWarning("Fixing contour");
                        VoxelContour ca = list[num23];
                        int ia = 0;
                        int ib = 0;
                        this.GetClosestIndices(ca.verts, ca.nverts, cb.verts, cb.nverts, ref ia, ref ib);
                        if ((ia == -1) || (ib == -1))
                        {
                            Debug.LogWarning(string.Concat(new object[] { "rcBuildContours: Failed to find merge points for ", k, " and ", num23, "." }));
                        }
                        else if (!MergeContours(ref ca, ref cb, ia, ib))
                        {
                            Debug.LogWarning(string.Concat(new object[] { "rcBuildContours: Failed to merge contours ", k, " and ", num23, "." }));
                        }
                        else
                        {
                            list[num23] = ca;
                            list[k] = cb;
                        }
                    }
                }
            }
            cset.conts = list;
        }

        public void BuildDistanceField()
        {
            ushort[] tmpUShortArr = this.voxelArea.tmpUShortArr;
            if ((tmpUShortArr == null) || (tmpUShortArr.Length < this.voxelArea.compactSpanCount))
            {
                tmpUShortArr = this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount];
            }
            Memory.MemSet<ushort>(tmpUShortArr, 0xffff, 2);
            this.voxelArea.maxDistance = this.CalculateDistanceField(tmpUShortArr);
            ushort[] dist = this.voxelArea.dist;
            if ((dist == null) || (dist.Length < this.voxelArea.compactSpanCount))
            {
                dist = new ushort[this.voxelArea.compactSpanCount];
            }
            dist = this.BoxBlur(tmpUShortArr, dist);
            this.voxelArea.dist = dist;
        }

        public void BuildPolyMesh(VoxelContourSet cset, int nvp, out VoxelMesh mesh)
        {
            nvp = 3;
            int num = 0;
            int num2 = 0;
            int a = 0;
            for (int i = 0; i < cset.conts.Count; i++)
            {
                VoxelContour contour2 = cset.conts[i];
                if (contour2.nverts >= 3)
                {
                    VoxelContour contour3 = cset.conts[i];
                    num += contour3.nverts;
                    VoxelContour contour4 = cset.conts[i];
                    num2 += contour4.nverts - 2;
                    VoxelContour contour5 = cset.conts[i];
                    a = AstarMath.Max(a, contour5.nverts);
                }
            }
            if (num >= 0xfffe)
            {
                Debug.LogWarning("To many vertices for unity to render - Unity might screw up rendering, but hopefully the navmesh will work ok");
            }
            VInt3[] numArray = new VInt3[num];
            int[] array = new int[num2 * nvp];
            Memory.MemSet<int>(array, 0xff, 4);
            int[] indices = new int[a];
            int[] tris = new int[a * 3];
            int index = 0;
            int num6 = 0;
            for (int j = 0; j < cset.conts.Count; j++)
            {
                VoxelContour contour = cset.conts[j];
                if (contour.nverts >= 3)
                {
                    for (int m = 0; m < contour.nverts; m++)
                    {
                        indices[m] = m;
                        contour.verts[(m * 4) + 2] /= this.voxelArea.width;
                    }
                    int num9 = this.Triangulate(contour.nverts, contour.verts, ref indices, ref tris);
                    int num10 = index;
                    for (int n = 0; n < (num9 * 3); n++)
                    {
                        array[num6] = tris[n] + num10;
                        num6++;
                    }
                    for (int num12 = 0; num12 < contour.nverts; num12++)
                    {
                        numArray[index] = new VInt3(contour.verts[num12 * 4], contour.verts[(num12 * 4) + 1], contour.verts[(num12 * 4) + 2]);
                        index++;
                    }
                }
            }
            mesh = new VoxelMesh();
            VInt3[] numArray5 = new VInt3[index];
            for (int k = 0; k < index; k++)
            {
                numArray5[k] = numArray[k];
            }
            int[] dst = new int[num6];
            Buffer.BlockCopy(array, 0, dst, 0, num6 * 4);
            mesh.verts = numArray5;
            mesh.tris = dst;
        }

        public void BuildRegions()
        {
            int width = this.voxelArea.width;
            int depth = this.voxelArea.depth;
            int num3 = width * depth;
            int maxIterations = 8;
            int compactSpanCount = this.voxelArea.compactSpanCount;
            List<int> stack = ListPool<int>.Claim(0x400);
            ushort[] srcReg = new ushort[compactSpanCount];
            ushort[] srcDist = new ushort[compactSpanCount];
            ushort[] dstReg = new ushort[compactSpanCount];
            ushort[] dstDist = new ushort[compactSpanCount];
            ushort r = 2;
            this.MarkRectWithRegion(0, this.borderSize, 0, depth, (ushort) (r | 0x8000), srcReg);
            r = (ushort) (r + 1);
            this.MarkRectWithRegion(width - this.borderSize, width, 0, depth, (ushort) (r | 0x8000), srcReg);
            r = (ushort) (r + 1);
            this.MarkRectWithRegion(0, width, 0, this.borderSize, (ushort) (r | 0x8000), srcReg);
            r = (ushort) (r + 1);
            this.MarkRectWithRegion(0, width, depth - this.borderSize, depth, (ushort) (r | 0x8000), srcReg);
            r = (ushort) (r + 1);
            uint level = (uint) ((this.voxelArea.maxDistance + 1) & -2);
            for (int i = 0; level > 0; i++)
            {
                level = (level < 2) ? 0 : (level - 2);
                if (this.ExpandRegions(maxIterations, level, srcReg, srcDist, dstReg, dstDist, stack) != srcReg)
                {
                    ushort[] numArray5 = srcReg;
                    srcReg = dstReg;
                    dstReg = numArray5;
                    numArray5 = srcDist;
                    srcDist = dstDist;
                    dstDist = numArray5;
                }
                int z = 0;
                for (int k = 0; z < num3; k++)
                {
                    for (int m = 0; m < this.voxelArea.width; m++)
                    {
                        CompactVoxelCell cell = this.voxelArea.compactCells[z + m];
                        int index = (int) cell.index;
                        int num13 = (int) (cell.index + cell.count);
                        while (index < num13)
                        {
                            if ((((this.voxelArea.dist[index] >= level) && (srcReg[index] == 0)) && (this.voxelArea.areaTypes[index] != 0)) && this.FloodRegion(m, z, index, level, r, srcReg, srcDist, stack))
                            {
                                r = (ushort) (r + 1);
                            }
                            index++;
                        }
                    }
                    z += width;
                }
            }
            if (this.ExpandRegions(maxIterations * 8, 0, srcReg, srcDist, dstReg, dstDist, stack) != srcReg)
            {
                ushort[] numArray6 = srcReg;
                srcReg = dstReg;
                dstReg = numArray6;
                numArray6 = srcDist;
                srcDist = dstDist;
                dstDist = numArray6;
            }
            this.voxelArea.maxRegions = r;
            this.FilterSmallRegions(srcReg, this.minRegionSize, this.voxelArea.maxRegions);
            for (int j = 0; j < this.voxelArea.compactSpanCount; j++)
            {
                this.voxelArea.compactSpans[j].reg = srcReg[j];
            }
            ListPool<int>.Release(stack);
        }

        public void BuildVoxelConnections()
        {
            int num = this.voxelArea.width * this.voxelArea.depth;
            CompactVoxelSpan[] compactSpans = this.voxelArea.compactSpans;
            CompactVoxelCell[] compactCells = this.voxelArea.compactCells;
            int num2 = 0;
            for (int i = 0; num2 < num; i++)
            {
                for (int j = 0; j < this.voxelArea.width; j++)
                {
                    CompactVoxelCell cell = compactCells[j + num2];
                    int index = (int) cell.index;
                    int num6 = (int) (cell.index + cell.count);
                    while (index < num6)
                    {
                        CompactVoxelSpan span = compactSpans[index];
                        compactSpans[index].con = uint.MaxValue;
                        for (int k = 0; k < 4; k++)
                        {
                            int num8 = j + this.voxelArea.DirectionX[k];
                            int num9 = num2 + this.voxelArea.DirectionZ[k];
                            if ((((num8 >= 0) && (num9 >= 0)) && (num9 < num)) && (num8 < this.voxelArea.width))
                            {
                                CompactVoxelCell cell2 = compactCells[num8 + num9];
                                int num10 = (int) cell2.index;
                                int num11 = (int) (cell2.index + cell2.count);
                                while (num10 < num11)
                                {
                                    CompactVoxelSpan span2 = compactSpans[num10];
                                    int num12 = Math.Max(span.y, span2.y);
                                    if (((AstarMath.Min((int) (span.y + span.h), (int) (span2.y + span2.h)) - num12) >= this.voxelWalkableHeight) && (Math.Abs((int) (span2.y - span.y)) <= this.voxelWalkableClimb))
                                    {
                                        uint num14 = ((uint) num10) - cell2.index;
                                        if (num14 > 0xffff)
                                        {
                                            Debug.LogError("Too many layers");
                                        }
                                        else
                                        {
                                            compactSpans[index].SetConnection(k, num14);
                                            break;
                                        }
                                    }
                                    num10++;
                                }
                            }
                        }
                        index++;
                    }
                }
                num2 += this.voxelArea.width;
            }
        }

        public int CalcAreaOfPolygon2D(int[] verts, int nverts)
        {
            int num = 0;
            int num2 = 0;
            for (int i = nverts - 1; num2 < nverts; i = num2++)
            {
                int index = num2 * 4;
                int num5 = i * 4;
                num += (verts[index] * (verts[num5 + 2] / this.voxelArea.width)) - (verts[num5] * (verts[index + 2] / this.voxelArea.width));
            }
            return ((num + 1) / 2);
        }

        public ushort CalculateDistanceField(ushort[] src)
        {
            int num = this.voxelArea.width * this.voxelArea.depth;
            for (int i = 0; i < num; i += this.voxelArea.width)
            {
                for (int n = 0; n < this.voxelArea.width; n++)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[n + i];
                    int index = (int) cell.index;
                    int num5 = (int) (cell.index + cell.count);
                    while (index < num5)
                    {
                        CompactVoxelSpan span = this.voxelArea.compactSpans[index];
                        int num6 = 0;
                        for (int num7 = 0; num7 < 4; num7++)
                        {
                            if (span.GetConnection(num7) == 0x3fL)
                            {
                                break;
                            }
                            num6++;
                        }
                        if (num6 != 4)
                        {
                            src[index] = 0;
                        }
                        index++;
                    }
                }
            }
            for (int j = 0; j < num; j += this.voxelArea.width)
            {
                for (int num9 = 0; num9 < this.voxelArea.width; num9++)
                {
                    CompactVoxelCell cell2 = this.voxelArea.compactCells[num9 + j];
                    int num10 = (int) cell2.index;
                    int num11 = (int) (cell2.index + cell2.count);
                    while (num10 < num11)
                    {
                        CompactVoxelSpan span2 = this.voxelArea.compactSpans[num10];
                        if (span2.GetConnection(0) != 0x3fL)
                        {
                            int num12 = num9 + this.voxelArea.DirectionX[0];
                            int num13 = j + this.voxelArea.DirectionZ[0];
                            int num14 = ((int) this.voxelArea.compactCells[num12 + num13].index) + span2.GetConnection(0);
                            if ((src[num14] + 2) < src[num10])
                            {
                                src[num10] = (ushort) (src[num14] + 2);
                            }
                            CompactVoxelSpan span3 = this.voxelArea.compactSpans[num14];
                            if (span3.GetConnection(3) != 0x3fL)
                            {
                                int num15 = num12 + this.voxelArea.DirectionX[3];
                                int num16 = num13 + this.voxelArea.DirectionZ[3];
                                int num17 = ((int) this.voxelArea.compactCells[num15 + num16].index) + span3.GetConnection(3);
                                if ((src[num17] + 3) < src[num10])
                                {
                                    src[num10] = (ushort) (src[num17] + 3);
                                }
                            }
                        }
                        if (span2.GetConnection(3) != 0x3fL)
                        {
                            int num18 = num9 + this.voxelArea.DirectionX[3];
                            int num19 = j + this.voxelArea.DirectionZ[3];
                            int num20 = ((int) this.voxelArea.compactCells[num18 + num19].index) + span2.GetConnection(3);
                            if ((src[num20] + 2) < src[num10])
                            {
                                src[num10] = (ushort) (src[num20] + 2);
                            }
                            CompactVoxelSpan span4 = this.voxelArea.compactSpans[num20];
                            if (span4.GetConnection(2) != 0x3fL)
                            {
                                int num21 = num18 + this.voxelArea.DirectionX[2];
                                int num22 = num19 + this.voxelArea.DirectionZ[2];
                                int num23 = ((int) this.voxelArea.compactCells[num21 + num22].index) + span4.GetConnection(2);
                                if ((src[num23] + 3) < src[num10])
                                {
                                    src[num10] = (ushort) (src[num23] + 3);
                                }
                            }
                        }
                        num10++;
                    }
                }
            }
            for (int k = num - this.voxelArea.width; k >= 0; k -= this.voxelArea.width)
            {
                for (int num25 = this.voxelArea.width - 1; num25 >= 0; num25--)
                {
                    CompactVoxelCell cell3 = this.voxelArea.compactCells[num25 + k];
                    int num26 = (int) cell3.index;
                    int num27 = (int) (cell3.index + cell3.count);
                    while (num26 < num27)
                    {
                        CompactVoxelSpan span5 = this.voxelArea.compactSpans[num26];
                        if (span5.GetConnection(2) != 0x3fL)
                        {
                            int num28 = num25 + this.voxelArea.DirectionX[2];
                            int num29 = k + this.voxelArea.DirectionZ[2];
                            int num30 = ((int) this.voxelArea.compactCells[num28 + num29].index) + span5.GetConnection(2);
                            if ((src[num30] + 2) < src[num26])
                            {
                                src[num26] = (ushort) (src[num30] + 2);
                            }
                            CompactVoxelSpan span6 = this.voxelArea.compactSpans[num30];
                            if (span6.GetConnection(1) != 0x3fL)
                            {
                                int num31 = num28 + this.voxelArea.DirectionX[1];
                                int num32 = num29 + this.voxelArea.DirectionZ[1];
                                int num33 = ((int) this.voxelArea.compactCells[num31 + num32].index) + span6.GetConnection(1);
                                if ((src[num33] + 3) < src[num26])
                                {
                                    src[num26] = (ushort) (src[num33] + 3);
                                }
                            }
                        }
                        if (span5.GetConnection(1) != 0x3fL)
                        {
                            int num34 = num25 + this.voxelArea.DirectionX[1];
                            int num35 = k + this.voxelArea.DirectionZ[1];
                            int num36 = ((int) this.voxelArea.compactCells[num34 + num35].index) + span5.GetConnection(1);
                            if ((src[num36] + 2) < src[num26])
                            {
                                src[num26] = (ushort) (src[num36] + 2);
                            }
                            CompactVoxelSpan span7 = this.voxelArea.compactSpans[num36];
                            if (span7.GetConnection(0) != 0x3fL)
                            {
                                int num37 = num34 + this.voxelArea.DirectionX[0];
                                int num38 = num35 + this.voxelArea.DirectionZ[0];
                                int num39 = ((int) this.voxelArea.compactCells[num37 + num38].index) + span7.GetConnection(0);
                                if ((src[num39] + 3) < src[num26])
                                {
                                    src[num26] = (ushort) (src[num39] + 3);
                                }
                            }
                        }
                        num26++;
                    }
                }
            }
            ushort num40 = 0;
            for (int m = 0; m < this.voxelArea.compactSpanCount; m++)
            {
                num40 = Math.Max(src[m], num40);
            }
            return num40;
        }

        private static int[] ClaimIntArr(int minCapacity, bool zero)
        {
            for (int i = 0; i < intArrCache.Count; i++)
            {
                if (intArrCache[i].Length >= minCapacity)
                {
                    int[] array = intArrCache[i];
                    intArrCache.RemoveAt(i);
                    if (zero)
                    {
                        Memory.MemSet<int>(array, 0, 4);
                    }
                    return array;
                }
            }
            return new int[minCapacity];
        }

        public void CollectMeshes()
        {
            CollectMeshes(this.inputExtraMeshes, this.forcedBounds, out this.inputVertices, out this.inputTriangles);
        }

        public static void CollectMeshes(List<ExtraMesh> extraMeshes, Bounds bounds, out Vector3[] verts, out int[] tris)
        {
            verts = null;
            tris = null;
        }

        public static bool Collinear(int a, int b, int c, int[] verts)
        {
            return (Area2(a, b, c, verts) == 0);
        }

        public Vector3 CompactSpanToVector(int x, int z, int i)
        {
            return (this.voxelOffset + new Vector3(x * this.cellSize, this.voxelArea.compactSpans[i].y * this.cellHeight, z * this.cellSize));
        }

        public Vector3 ConvertPos(int x, int y, int z)
        {
            return (Vector3.Scale(new Vector3(x + 0.5f, (float) y, (((float) z) / ((float) this.voxelArea.width)) + 0.5f), this.cellScale) + this.voxelOffset);
        }

        public Vector3 ConvertPosCorrZ(int x, int y, int z)
        {
            return (Vector3.Scale(new Vector3((float) x, (float) y, (float) z), this.cellScale) + this.voxelOffset);
        }

        public Vector3 ConvertPosition(int x, int z, int i)
        {
            CompactVoxelSpan span = this.voxelArea.compactSpans[i];
            return (new Vector3(x * this.cellSize, span.y * this.cellHeight, (((float) z) / ((float) this.voxelArea.width)) * this.cellSize) + this.voxelOffset);
        }

        public Vector3 ConvertPosWithoutOffset(int x, int y, int z)
        {
            return (Vector3.Scale(new Vector3((float) x, (float) y, ((float) z) / ((float) this.voxelArea.width)), this.cellScale) + this.voxelOffset);
        }

        public static bool Diagonal(int i, int j, int n, int[] verts, int[] indices)
        {
            return (InCone(i, j, n, verts, indices) && Diagonalie(i, j, n, verts, indices));
        }

        private static bool Diagonalie(int i, int j, int n, int[] verts, int[] indices)
        {
            int a = (indices[i] & 0xfffffff) * 4;
            int num2 = (indices[j] & 0xfffffff) * 4;
            for (int k = 0; k < n; k++)
            {
                int index = Next(k, n);
                if (((k != i) && (index != i)) && ((k != j) && (index != j)))
                {
                    int b = (indices[k] & 0xfffffff) * 4;
                    int num6 = (indices[index] & 0xfffffff) * 4;
                    if ((((!Vequal(a, b, verts) && !Vequal(num2, b, verts)) && !Vequal(a, num6, verts)) && !Vequal(num2, num6, verts)) && Intersect(a, num2, b, num6, verts))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void DrawLine(int a, int b, int[] indices, int[] verts, Color col)
        {
            int index = (indices[a] & 0xfffffff) * 4;
            int num2 = (indices[b] & 0xfffffff) * 4;
            Debug.DrawLine(this.ConvertPosCorrZ(verts[index], verts[index + 1], verts[index + 2]), this.ConvertPosCorrZ(verts[num2], verts[num2 + 1], verts[num2 + 2]), col);
        }

        [Obsolete("This function is not complete and should not be used")]
        public void ErodeVoxels(int radius)
        {
            if (radius > 0xff)
            {
                Debug.LogError("Max Erode Radius is 255");
                radius = 0xff;
            }
            int num = this.voxelArea.width * this.voxelArea.depth;
            int[] numArray = new int[this.voxelArea.compactSpanCount];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = 0xff;
            }
            for (int j = 0; j < num; j += this.voxelArea.width)
            {
                for (int k = 0; k < this.voxelArea.width; k++)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[k + j];
                    int index = (int) cell.index;
                    int num6 = (int) (cell.index + cell.count);
                    while (index < num6)
                    {
                        if (this.voxelArea.areaTypes[index] != 0)
                        {
                            CompactVoxelSpan span = this.voxelArea.compactSpans[index];
                            int num7 = 0;
                            for (int m = 0; m < 4; m++)
                            {
                                if (span.GetConnection(m) != 0x3fL)
                                {
                                    num7++;
                                }
                            }
                            if (num7 != 4)
                            {
                                numArray[index] = 0;
                            }
                        }
                        index++;
                    }
                }
            }
        }

        public void ErodeWalkableArea(int radius)
        {
            ushort[] tmpUShortArr = this.voxelArea.tmpUShortArr;
            if ((tmpUShortArr == null) || (tmpUShortArr.Length < this.voxelArea.compactSpanCount))
            {
                tmpUShortArr = this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount];
            }
            Memory.MemSet<ushort>(tmpUShortArr, 0xffff, 2);
            this.CalculateDistanceField(tmpUShortArr);
            for (int i = 0; i < tmpUShortArr.Length; i++)
            {
                if (tmpUShortArr[i] < (radius * 2))
                {
                    this.voxelArea.areaTypes[i] = 0;
                }
            }
        }

        public ushort[] ExpandRegions(int maxIterations, uint level, ushort[] srcReg, ushort[] srcDist, ushort[] dstReg, ushort[] dstDist, List<int> stack)
        {
            int num11;
            int width = this.voxelArea.width;
            int depth = this.voxelArea.depth;
            int num3 = width * depth;
            stack.Clear();
            int item = 0;
            for (int i = 0; item < num3; i++)
            {
                for (int k = 0; k < this.voxelArea.width; k++)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[item + k];
                    int index = (int) cell.index;
                    int num8 = (int) (cell.index + cell.count);
                    while (index < num8)
                    {
                        if (((this.voxelArea.dist[index] >= level) && (srcReg[index] == 0)) && (this.voxelArea.areaTypes[index] != 0))
                        {
                            stack.Add(k);
                            stack.Add(item);
                            stack.Add(index);
                        }
                        index++;
                    }
                }
                item += width;
            }
            int num9 = 0;
            int count = stack.Count;
            if (count <= 0)
            {
                return srcReg;
            }
        Label_0105:
            num11 = 0;
            Buffer.BlockCopy(srcReg, 0, dstReg, 0, srcReg.Length * 2);
            Buffer.BlockCopy(srcDist, 0, dstDist, 0, dstDist.Length * 2);
            for (int j = 0; j < count; j += 3)
            {
                if (j >= count)
                {
                    break;
                }
                int num13 = stack[j];
                int num14 = stack[j + 1];
                int num15 = stack[j + 2];
                if (num15 < 0)
                {
                    num11++;
                }
                else
                {
                    ushort num16 = srcReg[num15];
                    ushort num17 = 0xffff;
                    CompactVoxelSpan span = this.voxelArea.compactSpans[num15];
                    int num18 = this.voxelArea.areaTypes[num15];
                    for (int m = 0; m < 4; m++)
                    {
                        if (span.GetConnection(m) != 0x3fL)
                        {
                            int num20 = num13 + this.voxelArea.DirectionX[m];
                            int num21 = num14 + this.voxelArea.DirectionZ[m];
                            int num22 = ((int) this.voxelArea.compactCells[num20 + num21].index) + span.GetConnection(m);
                            if ((num18 == this.voxelArea.areaTypes[num22]) && (((srcReg[num22] > 0) && ((srcReg[num22] & 0x8000) == 0)) && ((srcDist[num22] + 2) < num17)))
                            {
                                num16 = srcReg[num22];
                                num17 = (ushort) (srcDist[num22] + 2);
                            }
                        }
                    }
                    if (num16 != 0)
                    {
                        stack[j + 2] = -1;
                        dstReg[num15] = num16;
                        dstDist[num15] = num17;
                    }
                    else
                    {
                        num11++;
                    }
                }
            }
            ushort[] numArray = srcReg;
            srcReg = dstReg;
            dstReg = numArray;
            numArray = srcDist;
            srcDist = dstDist;
            dstDist = numArray;
            if ((num11 * 3) < count)
            {
                if (level <= 0)
                {
                    goto Label_0105;
                }
                num9++;
                if (num9 < maxIterations)
                {
                    goto Label_0105;
                }
            }
            return srcReg;
        }

        public void FilterLedges(uint voxelWalkableHeight, int voxelWalkableClimb, float cs, float ch, Vector3 min)
        {
            int num = this.voxelArea.width * this.voxelArea.depth;
            LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
            int[] directionX = this.voxelArea.DirectionX;
            int[] directionZ = this.voxelArea.DirectionZ;
            int width = this.voxelArea.width;
            int num3 = 0;
            for (int i = 0; num3 < num; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (linkedSpans[j + num3].bottom != uint.MaxValue)
                    {
                        for (int k = j + num3; k != -1; k = linkedSpans[k].next)
                        {
                            if (linkedSpans[k].area == 0)
                            {
                                continue;
                            }
                            int top = (int) linkedSpans[k].top;
                            int num8 = (linkedSpans[k].next == -1) ? 0x10000 : ((int) linkedSpans[linkedSpans[k].next].bottom);
                            int num9 = 0x10000;
                            int num10 = (int) linkedSpans[k].top;
                            int num11 = num10;
                            for (int m = 0; m < 4; m++)
                            {
                                int num13 = j + directionX[m];
                                int num14 = num3 + directionZ[m];
                                if (((num13 < 0) || (num14 < 0)) || ((num14 >= num) || (num13 >= width)))
                                {
                                    linkedSpans[k].area = 0;
                                    break;
                                }
                                int index = num13 + num14;
                                int num16 = -voxelWalkableClimb;
                                int num17 = (linkedSpans[index].bottom == uint.MaxValue) ? 0x10000 : ((int) linkedSpans[index].bottom);
                                if ((Math.Min(num8, num17) - Math.Max(top, num16)) > voxelWalkableHeight)
                                {
                                    num9 = Math.Min(num9, num16 - top);
                                }
                                if (linkedSpans[index].bottom != uint.MaxValue)
                                {
                                    for (int n = index; n != -1; n = linkedSpans[n].next)
                                    {
                                        num16 = (int) linkedSpans[n].top;
                                        num17 = (linkedSpans[n].next == -1) ? 0x10000 : ((int) linkedSpans[linkedSpans[n].next].bottom);
                                        if ((Math.Min(num8, num17) - Math.Max(top, num16)) > voxelWalkableHeight)
                                        {
                                            num9 = AstarMath.Min(num9, num16 - top);
                                            if (Math.Abs((int) (num16 - top)) <= voxelWalkableClimb)
                                            {
                                                if (num16 < num10)
                                                {
                                                    num10 = num16;
                                                }
                                                if (num16 > num11)
                                                {
                                                    num11 = num16;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if ((num9 < -voxelWalkableClimb) || ((num11 - num10) > voxelWalkableClimb))
                            {
                                linkedSpans[k].area = 0;
                            }
                        }
                    }
                }
                num3 += width;
            }
        }

        public void FilterLowHeightSpans(uint voxelWalkableHeight, float cs, float ch, Vector3 min)
        {
            int num = this.voxelArea.width * this.voxelArea.depth;
            LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
            int num2 = 0;
            for (int i = 0; num2 < num; i++)
            {
                for (int j = 0; j < this.voxelArea.width; j++)
                {
                    for (int k = num2 + j; (k != -1) && (linkedSpans[k].bottom != uint.MaxValue); k = linkedSpans[k].next)
                    {
                        uint top = linkedSpans[k].top;
                        uint num7 = (linkedSpans[k].next == -1) ? 0x10000 : linkedSpans[linkedSpans[k].next].bottom;
                        if ((num7 - top) < voxelWalkableHeight)
                        {
                            linkedSpans[k].area = 0;
                        }
                    }
                }
                num2 += this.voxelArea.width;
            }
        }

        public void FilterSmallRegions(ushort[] reg, int minRegionSize, int maxRegions)
        {
            RelevantGraphSurface root = RelevantGraphSurface.Root;
            bool flag = (root != null) && (this.relevantGraphSurfaceMode != RecastGraph.RelevantGraphSurfaceMode.DoNotRequire);
            if (flag || (minRegionSize > 0))
            {
                int[] array = new int[maxRegions];
                ushort[] tmpUShortArr = this.voxelArea.tmpUShortArr;
                if ((tmpUShortArr == null) || (tmpUShortArr.Length < maxRegions))
                {
                    tmpUShortArr = this.voxelArea.tmpUShortArr = new ushort[maxRegions];
                }
                Memory.MemSet<int>(array, -1, 4);
                Memory.MemSet<ushort>(tmpUShortArr, 0, maxRegions, 2);
                int length = array.Length;
                int num2 = this.voxelArea.width * this.voxelArea.depth;
                int num5 = 2 | ((this.relevantGraphSurfaceMode != RecastGraph.RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile) ? 0 : 1);
                if (flag)
                {
                    while (root != null)
                    {
                        int num6;
                        int num7;
                        this.VectorToIndex(root.Position, out num6, out num7);
                        if (((num6 < 0) || (num7 < 0)) || ((num6 >= this.voxelArea.width) || (num7 >= this.voxelArea.depth)))
                        {
                            root = root.Next;
                        }
                        else
                        {
                            int num8 = (int) ((root.Position.y - this.voxelOffset.y) / this.cellHeight);
                            int num9 = (int) (root.maxRange / this.cellHeight);
                            CompactVoxelCell cell = this.voxelArea.compactCells[num6 + (num7 * this.voxelArea.width)];
                            for (int n = (int) cell.index; n < (cell.index + cell.count); n++)
                            {
                                CompactVoxelSpan span = this.voxelArea.compactSpans[n];
                                if ((Math.Abs((int) (span.y - num8)) <= num9) && (reg[n] != 0))
                                {
                                    tmpUShortArr[union_find_find(array, reg[n] & -32769)] = (ushort) (tmpUShortArr[union_find_find(array, reg[n] & -32769)] | 2);
                                }
                            }
                            root = root.Next;
                        }
                    }
                }
                int num11 = 0;
                for (int i = 0; num11 < num2; i++)
                {
                    for (int num13 = 0; num13 < this.voxelArea.width; num13++)
                    {
                        CompactVoxelCell cell2 = this.voxelArea.compactCells[num13 + num11];
                        for (int num14 = (int) cell2.index; num14 < (cell2.index + cell2.count); num14++)
                        {
                            CompactVoxelSpan span2 = this.voxelArea.compactSpans[num14];
                            int x = reg[num14];
                            if ((x & -32769) != 0)
                            {
                                if (x >= length)
                                {
                                    tmpUShortArr[union_find_find(array, x & -32769)] = (ushort) (tmpUShortArr[union_find_find(array, x & -32769)] | 1);
                                }
                                else
                                {
                                    int index = union_find_find(array, x);
                                    array[index]--;
                                    for (int num17 = 0; num17 < 4; num17++)
                                    {
                                        if (span2.GetConnection(num17) != 0x3fL)
                                        {
                                            int num18 = num13 + this.voxelArea.DirectionX[num17];
                                            int num19 = num11 + this.voxelArea.DirectionZ[num17];
                                            int num20 = ((int) this.voxelArea.compactCells[num18 + num19].index) + span2.GetConnection(num17);
                                            int b = reg[num20];
                                            if ((x != b) && ((b & -32769) != 0))
                                            {
                                                if ((b & 0x8000) != 0)
                                                {
                                                    tmpUShortArr[index] = (ushort) (tmpUShortArr[index] | 1);
                                                }
                                                else
                                                {
                                                    union_find_union(array, index, b);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    num11 += this.voxelArea.width;
                }
                for (int j = 0; j < array.Length; j++)
                {
                    tmpUShortArr[union_find_find(array, j)] = (ushort) (tmpUShortArr[union_find_find(array, j)] | tmpUShortArr[j]);
                }
                for (int k = 0; k < array.Length; k++)
                {
                    int num24 = union_find_find(array, k);
                    if ((tmpUShortArr[num24] & 1) != 0)
                    {
                        array[num24] = -minRegionSize - 2;
                    }
                    if (flag && ((tmpUShortArr[num24] & num5) == 0))
                    {
                        array[num24] = -1;
                    }
                }
                for (int m = 0; m < this.voxelArea.compactSpanCount; m++)
                {
                    int num26 = reg[m];
                    if ((num26 < length) && (array[union_find_find(array, num26)] >= (-minRegionSize - 1)))
                    {
                        reg[m] = 0;
                    }
                }
            }
        }

        private void FloodOnes(List<VInt3> st1, ushort[] regs, uint level, ushort reg)
        {
            for (int i = 0; i < st1.Count; i++)
            {
                VInt3 num10 = st1[i];
                int x = num10.x;
                VInt3 num11 = st1[i];
                int y = num11.y;
                VInt3 num12 = st1[i];
                int z = num12.z;
                regs[y] = reg;
                CompactVoxelSpan span = this.voxelArea.compactSpans[y];
                int num5 = this.voxelArea.areaTypes[y];
                for (int j = 0; j < 4; j++)
                {
                    if (span.GetConnection(j) != 0x3fL)
                    {
                        int num7 = x + this.voxelArea.DirectionX[j];
                        int num8 = z + this.voxelArea.DirectionZ[j];
                        int index = ((int) this.voxelArea.compactCells[num7 + num8].index) + span.GetConnection(j);
                        if ((num5 == this.voxelArea.areaTypes[index]) && (regs[index] == 1))
                        {
                            regs[index] = reg;
                            st1.Add(new VInt3(num7, index, num8));
                        }
                    }
                }
            }
        }

        public bool FloodRegion(int x, int z, int i, uint level, ushort r, ushort[] srcReg, ushort[] srcDist, List<int> stack)
        {
            int num = this.voxelArea.areaTypes[i];
            stack.Clear();
            stack.Add(x);
            stack.Add(z);
            stack.Add(i);
            srcReg[i] = r;
            srcDist[i] = 0;
            int num2 = (level < 2) ? 0 : (((int) level) - 2);
            int num3 = 0;
            while (stack.Count > 0)
            {
                int index = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                int num5 = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                int num6 = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                CompactVoxelSpan span = this.voxelArea.compactSpans[index];
                ushort num7 = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (span.GetConnection(j) != 0x3fL)
                    {
                        int num9 = num6 + this.voxelArea.DirectionX[j];
                        int num10 = num5 + this.voxelArea.DirectionZ[j];
                        int num11 = ((int) this.voxelArea.compactCells[num9 + num10].index) + span.GetConnection(j);
                        if (this.voxelArea.areaTypes[num11] == num)
                        {
                            ushort num12 = srcReg[num11];
                            if ((num12 & 0x8000) != 0x8000)
                            {
                                if ((num12 != 0) && (num12 != r))
                                {
                                    num7 = num12;
                                }
                                CompactVoxelSpan span2 = this.voxelArea.compactSpans[num11];
                                int dir = (j + 1) & 3;
                                if (span2.GetConnection(dir) != 0x3fL)
                                {
                                    int num14 = num9 + this.voxelArea.DirectionX[dir];
                                    int num15 = num10 + this.voxelArea.DirectionZ[dir];
                                    int num16 = ((int) this.voxelArea.compactCells[num14 + num15].index) + span2.GetConnection(dir);
                                    if (this.voxelArea.areaTypes[num16] == num)
                                    {
                                        num12 = srcReg[num16];
                                        if ((num12 != 0) && (num12 != r))
                                        {
                                            num7 = num12;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (num7 != 0)
                {
                    srcReg[index] = 0;
                }
                else
                {
                    num3++;
                    for (int k = 0; k < 4; k++)
                    {
                        if (span.GetConnection(k) != 0x3fL)
                        {
                            int item = num6 + this.voxelArea.DirectionX[k];
                            int num19 = num5 + this.voxelArea.DirectionZ[k];
                            int num20 = ((int) this.voxelArea.compactCells[item + num19].index) + span.GetConnection(k);
                            if ((this.voxelArea.areaTypes[num20] == num) && ((this.voxelArea.dist[num20] >= num2) && (srcReg[num20] == 0)))
                            {
                                srcReg[num20] = r;
                                srcDist[num20] = 0;
                                stack.Add(item);
                                stack.Add(num19);
                                stack.Add(num20);
                            }
                        }
                    }
                }
            }
            return (num3 > 0);
        }

        private void GetClosestIndices(int[] vertsa, int nvertsa, int[] vertsb, int nvertsb, ref int ia, ref int ib)
        {
            int num = 0xfffffff;
            ia = -1;
            ib = -1;
            for (int i = 0; i < nvertsa; i++)
            {
                int num3 = (i + 1) % nvertsa;
                int num4 = ((i + nvertsa) - 1) % nvertsa;
                int b = i * 4;
                int num6 = num3 * 4;
                int a = num4 * 4;
                for (int j = 0; j < nvertsb; j++)
                {
                    int c = j * 4;
                    if (Ileft(a, b, c, vertsa, vertsa, vertsb) && Ileft(b, num6, c, vertsa, vertsa, vertsb))
                    {
                        int num10 = vertsb[c] - vertsa[b];
                        int num11 = (vertsb[c + 2] / this.voxelArea.width) - (vertsa[b + 2] / this.voxelArea.width);
                        int num12 = (num10 * num10) + (num11 * num11);
                        if (num12 < num)
                        {
                            ia = i;
                            ib = j;
                            num = num12;
                        }
                    }
                }
            }
        }

        public int GetCornerHeight(int x, int z, int i, int dir, ref bool isBorderVertex)
        {
            CompactVoxelSpan span = this.voxelArea.compactSpans[i];
            int y = span.y;
            int num2 = (dir + 1) & 3;
            uint[] numArray = new uint[4];
            numArray[0] = (uint) (this.voxelArea.compactSpans[i].reg | (this.voxelArea.areaTypes[i] << 0x10));
            if (span.GetConnection(dir) != 0x3fL)
            {
                int num3 = x + this.voxelArea.DirectionX[dir];
                int num4 = z + this.voxelArea.DirectionZ[dir];
                int index = ((int) this.voxelArea.compactCells[num3 + num4].index) + span.GetConnection(dir);
                CompactVoxelSpan span2 = this.voxelArea.compactSpans[index];
                y = AstarMath.Max(y, span2.y);
                numArray[1] = (uint) (span2.reg | (this.voxelArea.areaTypes[index] << 0x10));
                if (span2.GetConnection(num2) != 0x3fL)
                {
                    int num6 = num3 + this.voxelArea.DirectionX[num2];
                    int num7 = num4 + this.voxelArea.DirectionZ[num2];
                    int num8 = ((int) this.voxelArea.compactCells[num6 + num7].index) + span2.GetConnection(num2);
                    CompactVoxelSpan span3 = this.voxelArea.compactSpans[num8];
                    y = AstarMath.Max(y, span3.y);
                    numArray[2] = (uint) (span3.reg | (this.voxelArea.areaTypes[num8] << 0x10));
                }
            }
            if (span.GetConnection(num2) != 0x3fL)
            {
                int num9 = x + this.voxelArea.DirectionX[num2];
                int num10 = z + this.voxelArea.DirectionZ[num2];
                int num11 = ((int) this.voxelArea.compactCells[num9 + num10].index) + span.GetConnection(num2);
                CompactVoxelSpan span4 = this.voxelArea.compactSpans[num11];
                y = AstarMath.Max(y, span4.y);
                numArray[3] = (uint) (span4.reg | (this.voxelArea.areaTypes[num11] << 0x10));
                if (span4.GetConnection(dir) != 0x3fL)
                {
                    int num12 = num9 + this.voxelArea.DirectionX[dir];
                    int num13 = num10 + this.voxelArea.DirectionZ[dir];
                    int num14 = ((int) this.voxelArea.compactCells[num12 + num13].index) + span4.GetConnection(dir);
                    CompactVoxelSpan span5 = this.voxelArea.compactSpans[num14];
                    y = AstarMath.Max(y, span5.y);
                    numArray[2] = (uint) (span5.reg | (this.voxelArea.areaTypes[num14] << 0x10));
                }
            }
            for (int j = 0; j < 4; j++)
            {
                int num16 = j;
                int num17 = (j + 1) & 3;
                int num18 = (j + 2) & 3;
                int num19 = (j + 3) & 3;
                bool flag = (((numArray[num16] & numArray[num17]) & 0x8000) != 0) && (numArray[num16] == numArray[num17]);
                bool flag2 = ((numArray[num18] | numArray[num19]) & 0x8000) == 0;
                bool flag3 = (numArray[num18] >> 0x10) == (numArray[num19] >> 0x10);
                bool flag4 = (((numArray[num16] != 0) && (numArray[num17] != 0)) && (numArray[num18] != 0)) && (numArray[num19] != 0);
                if ((flag && flag2) && (flag3 && flag4))
                {
                    isBorderVertex = true;
                    return y;
                }
            }
            return y;
        }

        public static bool Ileft(int a, int b, int c, int[] va, int[] vb, int[] vc)
        {
            return ((((vb[b] - va[a]) * (vc[c + 2] - va[a + 2])) - ((vc[c] - va[a]) * (vb[b + 2] - va[a + 2]))) <= 0);
        }

        public static bool InCone(int i, int j, int n, int[] verts, int[] indices)
        {
            int b = (indices[i] & 0xfffffff) * 4;
            int num2 = (indices[j] & 0xfffffff) * 4;
            int c = (indices[Next(i, n)] & 0xfffffff) * 4;
            int a = (indices[Prev(i, n)] & 0xfffffff) * 4;
            if (LeftOn(a, b, c, verts))
            {
                return (Left(b, num2, a, verts) && Left(num2, b, c, verts));
            }
            return !(LeftOn(b, num2, c, verts) && LeftOn(num2, b, a, verts));
        }

        public void Init()
        {
            if (((this.voxelArea == null) || (this.voxelArea.width != this.width)) || (this.voxelArea.depth != this.depth))
            {
                this.voxelArea = new VoxelArea(this.width, this.depth);
            }
            else
            {
                this.voxelArea.Reset();
            }
        }

        private static bool Intersect(int a, int b, int c, int d, int[] verts)
        {
            if (!IntersectProp(a, b, c, d, verts) && ((!Between(a, b, c, verts) && !Between(a, b, d, verts)) && (!Between(c, d, a, verts) && !Between(c, d, b, verts))))
            {
                return false;
            }
            return true;
        }

        public static bool IntersectProp(int a, int b, int c, int d, int[] verts)
        {
            if ((Collinear(a, b, c, verts) || Collinear(a, b, d, verts)) || (Collinear(c, d, a, verts) || Collinear(c, d, b, verts)))
            {
                return false;
            }
            return (Xorb(Left(a, b, c, verts), Left(a, b, d, verts)) && Xorb(Left(c, d, a, verts), Left(c, d, b, verts)));
        }

        public static bool Left(int a, int b, int c, int[] verts)
        {
            return (Area2(a, b, c, verts) < 0);
        }

        public static bool LeftOn(int a, int b, int c, int[] verts)
        {
            return (Area2(a, b, c, verts) <= 0);
        }

        public void MarkRectWithRegion(int minx, int maxx, int minz, int maxz, ushort region, ushort[] srcReg)
        {
            int num = maxz * this.voxelArea.width;
            for (int i = minz * this.voxelArea.width; i < num; i += this.voxelArea.width)
            {
                for (int j = minx; j < maxx; j++)
                {
                    CompactVoxelCell cell = this.voxelArea.compactCells[i + j];
                    int index = (int) cell.index;
                    int num5 = (int) (cell.index + cell.count);
                    while (index < num5)
                    {
                        if (this.voxelArea.areaTypes[index] != 0)
                        {
                            srcReg[index] = region;
                        }
                        index++;
                    }
                }
            }
        }

        public static bool MergeContours(ref VoxelContour ca, ref VoxelContour cb, int ia, int ib)
        {
            int num = (ca.nverts + cb.nverts) + 2;
            int[] numArray = ClaimIntArr(num * 4, false);
            int num2 = 0;
            for (int i = 0; i <= ca.nverts; i++)
            {
                int index = num2 * 4;
                int num5 = ((ia + i) % ca.nverts) * 4;
                numArray[index] = ca.verts[num5];
                numArray[index + 1] = ca.verts[num5 + 1];
                numArray[index + 2] = ca.verts[num5 + 2];
                numArray[index + 3] = ca.verts[num5 + 3];
                num2++;
            }
            for (int j = 0; j <= cb.nverts; j++)
            {
                int num7 = num2 * 4;
                int num8 = ((ib + j) % cb.nverts) * 4;
                numArray[num7] = cb.verts[num8];
                numArray[num7 + 1] = cb.verts[num8 + 1];
                numArray[num7 + 2] = cb.verts[num8 + 2];
                numArray[num7 + 3] = cb.verts[num8 + 3];
                num2++;
            }
            ReleaseIntArr(ca.verts);
            ReleaseIntArr(cb.verts);
            ca.verts = numArray;
            ca.nverts = num2;
            cb.verts = emptyArr;
            cb.nverts = 0;
            return true;
        }

        public static int Next(int i, int n)
        {
            return (((i + 1) >= n) ? 0 : (i + 1));
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(5f, 5f, 200f, (float) Screen.height), this.debugString);
        }

        public static int Prev(int i, int n)
        {
            return (((i - 1) < 0) ? (n - 1) : (i - 1));
        }

        private static void ReleaseContours(VoxelContourSet cset)
        {
            for (int i = 0; i < cset.conts.Count; i++)
            {
                VoxelContour contour = cset.conts[i];
                ReleaseIntArr(contour.verts);
                ReleaseIntArr(contour.rverts);
            }
            cset.conts = null;
        }

        private static void ReleaseIntArr(int[] arr)
        {
            if (arr != null)
            {
                intArrCache.Add(arr);
            }
        }

        public void RemoveDegenerateSegments(List<int> simplified)
        {
            for (int i = 0; i < (simplified.Count / 4); i++)
            {
                int num2 = i + 1;
                if (num2 >= (simplified.Count / 4))
                {
                    num2 = 0;
                }
                if ((simplified[i * 4] == simplified[num2 * 4]) && (simplified[(i * 4) + 2] == simplified[(num2 * 4) + 2]))
                {
                    simplified.RemoveRange(i, 4);
                }
            }
        }

        public void SimplifyContour(List<int> verts, List<int> simplified, float maxError, int maxEdgeLenght, int buildFlags)
        {
            bool flag = false;
            for (int i = 0; i < verts.Count; i += 4)
            {
                if ((verts[i + 3] & 0xffff) != 0)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                int item = 0;
                int num3 = verts.Count / 4;
                while (item < num3)
                {
                    int num4 = (item + 1) % num3;
                    bool flag2 = (verts[(item * 4) + 3] & 0xffff) != (verts[(num4 * 4) + 3] & 0xffff);
                    bool flag3 = (verts[(item * 4) + 3] & 0x20000) != (verts[(num4 * 4) + 3] & 0x20000);
                    if (flag2 || flag3)
                    {
                        simplified.Add(verts[item * 4]);
                        simplified.Add(verts[(item * 4) + 1]);
                        simplified.Add(verts[(item * 4) + 2]);
                        simplified.Add(item);
                    }
                    item++;
                }
            }
            if (simplified.Count == 0)
            {
                int num5 = verts[0];
                int num6 = verts[1];
                int num7 = verts[2];
                int num8 = 0;
                int num9 = verts[0];
                int num10 = verts[1];
                int num11 = verts[2];
                int num12 = 0;
                for (int k = 0; k < verts.Count; k += 4)
                {
                    int num14 = verts[k];
                    int num15 = verts[k + 1];
                    int num16 = verts[k + 2];
                    if ((num14 < num5) || ((num14 == num5) && (num16 < num7)))
                    {
                        num5 = num14;
                        num6 = num15;
                        num7 = num16;
                        num8 = k / 4;
                    }
                    if ((num14 > num9) || ((num14 == num9) && (num16 > num11)))
                    {
                        num9 = num14;
                        num10 = num15;
                        num11 = num16;
                        num12 = k / 4;
                    }
                }
                simplified.Add(num5);
                simplified.Add(num6);
                simplified.Add(num7);
                simplified.Add(num8);
                simplified.Add(num9);
                simplified.Add(num10);
                simplified.Add(num11);
                simplified.Add(num12);
            }
            int num17 = verts.Count / 4;
            maxError *= maxError;
            int num18 = 0;
            while (num18 < (simplified.Count / 4))
            {
                int num28;
                int num29;
                int num30;
                int num19 = (num18 + 1) % (simplified.Count / 4);
                int px = simplified[num18 * 4];
                int num21 = simplified[(num18 * 4) + 2];
                int num22 = simplified[(num18 * 4) + 3];
                int qx = simplified[num19 * 4];
                int num24 = simplified[(num19 * 4) + 2];
                int num25 = simplified[(num19 * 4) + 3];
                float num26 = 0f;
                int num27 = -1;
                if ((qx > px) || ((qx == px) && (num24 > num21)))
                {
                    num29 = 1;
                    num28 = (num22 + num29) % num17;
                    num30 = num25;
                }
                else
                {
                    num29 = num17 - 1;
                    num28 = (num25 + num29) % num17;
                    num30 = num22;
                }
                if (((verts[(num28 * 4) + 3] & 0xffff) == 0) || ((verts[(num28 * 4) + 3] & 0x20000) == 0x20000))
                {
                    while (num28 != num30)
                    {
                        float num31 = AstarMath.DistancePointSegment(verts[num28 * 4], verts[(num28 * 4) + 2] / this.voxelArea.width, px, num21 / this.voxelArea.width, qx, num24 / this.voxelArea.width);
                        if (num31 > num26)
                        {
                            num26 = num31;
                            num27 = num28;
                        }
                        num28 = (num28 + num29) % num17;
                    }
                }
                if ((num27 != -1) && (num26 > maxError))
                {
                    simplified.Add(0);
                    simplified.Add(0);
                    simplified.Add(0);
                    simplified.Add(0);
                    int num32 = simplified.Count / 4;
                    for (int m = num32 - 1; m > num18; m--)
                    {
                        simplified[m * 4] = simplified[(m - 1) * 4];
                        simplified[(m * 4) + 1] = simplified[((m - 1) * 4) + 1];
                        simplified[(m * 4) + 2] = simplified[((m - 1) * 4) + 2];
                        simplified[(m * 4) + 3] = simplified[((m - 1) * 4) + 3];
                    }
                    simplified[(num18 + 1) * 4] = verts[num27 * 4];
                    simplified[((num18 + 1) * 4) + 1] = verts[(num27 * 4) + 1];
                    simplified[((num18 + 1) * 4) + 2] = verts[(num27 * 4) + 2];
                    simplified[((num18 + 1) * 4) + 3] = num27;
                }
                else
                {
                    num18++;
                }
            }
            float num34 = this.maxEdgeLength / this.cellSize;
            if ((num34 > 0f) && ((buildFlags & 3) != 0))
            {
                int num35 = 0;
                while (num35 < (simplified.Count / 4))
                {
                    if ((simplified.Count / 4) > 200)
                    {
                        break;
                    }
                    int num36 = (num35 + 1) % (simplified.Count / 4);
                    int num37 = simplified[num35 * 4];
                    int num38 = simplified[(num35 * 4) + 2];
                    int num39 = simplified[(num35 * 4) + 3];
                    int num40 = simplified[num36 * 4];
                    int num41 = simplified[(num36 * 4) + 2];
                    int num42 = simplified[(num36 * 4) + 3];
                    int num43 = -1;
                    int num44 = (num39 + 1) % num17;
                    bool flag4 = false;
                    if (((buildFlags & 1) == 1) && ((verts[(num44 * 4) + 3] & 0xffff) == 0))
                    {
                        flag4 = true;
                    }
                    if (((buildFlags & 2) == 1) && ((verts[(num44 * 4) + 3] & 0x20000) == 1))
                    {
                        flag4 = true;
                    }
                    if (flag4)
                    {
                        int num45 = num40 - num37;
                        int num46 = (num41 / this.voxelArea.width) - (num38 / this.voxelArea.width);
                        if (((num45 * num45) + (num46 * num46)) > (num34 * num34))
                        {
                            if ((num40 > num37) || ((num40 == num37) && (num41 > num38)))
                            {
                                int num47 = (num42 >= num39) ? (num42 - num39) : ((num42 + num17) - num39);
                                num43 = (num39 + (num47 / 2)) % num17;
                            }
                            else
                            {
                                int num48 = (num42 >= num39) ? (num42 - num39) : ((num42 + num17) - num39);
                                num43 = (num39 + ((num48 + 1) / 2)) % num17;
                            }
                        }
                    }
                    if (num43 != -1)
                    {
                        simplified.AddRange(new int[4]);
                        int num49 = simplified.Count / 4;
                        for (int n = num49 - 1; n > num35; n--)
                        {
                            simplified[n * 4] = simplified[(n - 1) * 4];
                            simplified[(n * 4) + 1] = simplified[((n - 1) * 4) + 1];
                            simplified[(n * 4) + 2] = simplified[((n - 1) * 4) + 2];
                            simplified[(n * 4) + 3] = simplified[((n - 1) * 4) + 3];
                        }
                        simplified[(num35 + 1) * 4] = verts[num43 * 4];
                        simplified[((num35 + 1) * 4) + 1] = verts[(num43 * 4) + 1];
                        simplified[((num35 + 1) * 4) + 2] = verts[(num43 * 4) + 2];
                        simplified[((num35 + 1) * 4) + 3] = num43;
                    }
                    else
                    {
                        num35++;
                    }
                }
            }
            for (int j = 0; j < (simplified.Count / 4); j++)
            {
                int num52 = (simplified[(j * 4) + 3] + 1) % num17;
                int num53 = simplified[(j * 4) + 3];
                simplified[(j * 4) + 3] = (verts[(num52 * 4) + 3] & 0xffff) | (verts[(num53 * 4) + 3] & 0x10000);
            }
        }

        public int Triangulate(int n, int[] verts, ref int[] indices, ref int[] tris)
        {
            int num = 0;
            int[] numArray = tris;
            int index = 0;
            int num3 = n;
            for (int i = 0; i < n; i++)
            {
                int num5 = Next(i, n);
                int j = Next(num5, n);
                if (Diagonal(i, j, n, verts, indices))
                {
                    indices[num5] |= 0x40000000;
                }
            }
            while (n > 3)
            {
                int num7 = -1;
                int num8 = -1;
                for (int k = 0; k < n; k++)
                {
                    int num10 = Next(k, n);
                    if ((indices[num10] & 0x40000000) != 0)
                    {
                        int num11 = (indices[k] & 0xfffffff) * 4;
                        int num12 = (indices[Next(num10, n)] & 0xfffffff) * 4;
                        int num13 = verts[num12] - verts[num11];
                        int num14 = verts[num12 + 2] - verts[num11 + 2];
                        int num15 = (num13 * num13) + (num14 * num14);
                        if ((num7 < 0) || (num15 < num7))
                        {
                            num7 = num15;
                            num8 = k;
                        }
                    }
                }
                if (num8 == -1)
                {
                    Debug.LogError("This should not happen");
                    for (int num16 = 0; num16 < num3; num16++)
                    {
                        this.DrawLine(Prev(num16, num3), num16, indices, verts, Color.red);
                    }
                    return -num;
                }
                int num17 = num8;
                int num18 = Next(num17, n);
                int num19 = Next(num18, n);
                numArray[index] = indices[num17] & 0xfffffff;
                index++;
                numArray[index] = indices[num18] & 0xfffffff;
                index++;
                numArray[index] = indices[num19] & 0xfffffff;
                index++;
                num++;
                n--;
                for (int m = num18; m < n; m++)
                {
                    indices[m] = indices[m + 1];
                }
                if (num18 >= n)
                {
                    num18 = 0;
                }
                num17 = Prev(num18, n);
                if (Diagonal(Prev(num17, n), num18, n, verts, indices))
                {
                    indices[num17] |= 0x40000000;
                }
                else
                {
                    indices[num17] &= 0xfffffff;
                }
                if (Diagonal(num17, Next(num18, n), n, verts, indices))
                {
                    indices[num18] |= 0x40000000;
                }
                else
                {
                    indices[num18] &= 0xfffffff;
                }
            }
            numArray[index] = indices[0] & 0xfffffff;
            index++;
            numArray[index] = indices[1] & 0xfffffff;
            index++;
            numArray[index] = indices[2] & 0xfffffff;
            index++;
            num++;
            return num;
        }

        private static int union_find_find(int[] arr, int x)
        {
            int num;
            if (arr[x] < 0)
            {
                return x;
            }
            arr[x] = num = union_find_find(arr, arr[x]);
            return num;
        }

        private static void union_find_union(int[] arr, int a, int b)
        {
            a = union_find_find(arr, a);
            b = union_find_find(arr, b);
            if (a != b)
            {
                if (arr[a] > arr[b])
                {
                    int num = a;
                    a = b;
                    b = num;
                }
                arr[a] += arr[b];
                arr[b] = a;
            }
        }

        public void VectorToIndex(Vector3 p, out int x, out int z)
        {
            p -= this.voxelOffset;
            x = Mathf.RoundToInt(p.x / this.cellSize);
            z = Mathf.RoundToInt(p.z / this.cellSize);
        }

        private static bool Vequal(int a, int b, int[] verts)
        {
            return ((verts[a] == verts[b]) && (verts[a + 2] == verts[b + 2]));
        }

        public void VoxelizeInput()
        {
            Vector3 min = this.forcedBounds.min;
            this.voxelOffset = min;
            float x = 1f / this.cellSize;
            float y = 1f / this.cellHeight;
            float num3 = Mathf.Cos(Mathf.Atan(Mathf.Tan(this.maxSlope * 0.01745329f) * (y * this.cellSize)));
            float[] a = new float[9];
            float[] vOut = new float[0x15];
            float[] numArray3 = new float[0x15];
            float[] numArray4 = new float[0x15];
            float[] numArray5 = new float[0x15];
            if (this.inputExtraMeshes == null)
            {
                throw new NullReferenceException("inputExtraMeshes not set");
            }
            int num4 = 0;
            for (int i = 0; i < this.inputExtraMeshes.Count; i++)
            {
                ExtraMesh mesh2 = this.inputExtraMeshes[i];
                if (mesh2.bounds.Intersects(this.forcedBounds))
                {
                    ExtraMesh mesh3 = this.inputExtraMeshes[i];
                    num4 = Math.Max(mesh3.vertices.Length, num4);
                }
            }
            Vector3[] vectorArray = new Vector3[num4];
            Matrix4x4 matrixx = Matrix4x4.Scale(new Vector3(x, y, x)) * Matrix4x4.TRS(-min, Quaternion.identity, Vector3.one);
            for (int j = 0; j < this.inputExtraMeshes.Count; j++)
            {
                ExtraMesh mesh = this.inputExtraMeshes[j];
                bool flag = MMGame_Math.isMirror(mesh.matrix);
                if (flag)
                {
                    Debug.Log(string.Format("GameObject {0} is mirrored!", mesh.name));
                }
                if (mesh.bounds.Intersects(this.forcedBounds))
                {
                    Matrix4x4 matrix = mesh.matrix;
                    matrix = matrixx * matrix;
                    Vector3[] vertices = mesh.vertices;
                    int[] triangles = mesh.triangles;
                    int length = triangles.Length;
                    for (int k = 0; k < vertices.Length; k++)
                    {
                        vectorArray[k] = matrix.MultiplyPoint3x4(vertices[k]);
                    }
                    int area = mesh.area;
                    for (int m = 0; m < length; m += 3)
                    {
                        Vector3 vector2;
                        Vector3 vector3;
                        Vector3 vector4;
                        if (flag)
                        {
                            vector2 = vectorArray[triangles[m]];
                            vector3 = vectorArray[triangles[m + 2]];
                            vector4 = vectorArray[triangles[m + 1]];
                        }
                        else
                        {
                            vector2 = vectorArray[triangles[m]];
                            vector3 = vectorArray[triangles[m + 1]];
                            vector4 = vectorArray[triangles[m + 2]];
                        }
                        int num11 = (int) Utility.Min(vector2.x, vector3.x, vector4.x);
                        int num12 = (int) Utility.Min(vector2.z, vector3.z, vector4.z);
                        int num13 = (int) Math.Ceiling((double) Utility.Max(vector2.x, vector3.x, vector4.x));
                        int num14 = (int) Math.Ceiling((double) Utility.Max(vector2.z, vector3.z, vector4.z));
                        num11 = Mathf.Clamp(num11, 0, this.voxelArea.width - 1);
                        num13 = Mathf.Clamp(num13, 0, this.voxelArea.width - 1);
                        num12 = Mathf.Clamp(num12, 0, this.voxelArea.depth - 1);
                        num14 = Mathf.Clamp(num14, 0, this.voxelArea.depth - 1);
                        if ((((num11 < this.voxelArea.width) && (num12 < this.voxelArea.depth)) && (num13 > 0)) && (num14 > 0))
                        {
                            int num15;
                            if (Vector3.Dot(Vector3.Cross(vector3 - vector2, vector4 - vector2).normalized, Vector3.up) < num3)
                            {
                                num15 = 0;
                            }
                            else
                            {
                                num15 = 1 + area;
                            }
                            Utility.CopyVector(a, 0, vector2);
                            Utility.CopyVector(a, 3, vector3);
                            Utility.CopyVector(a, 6, vector4);
                            for (int n = num11; n <= num13; n++)
                            {
                                int num18 = Utility.ClipPolygon(a, 3, vOut, 1f, -n + 0.5f, 0);
                                if (num18 >= 3)
                                {
                                    num18 = Utility.ClipPolygon(vOut, num18, numArray3, -1f, n + 0.5f, 0);
                                    if (num18 >= 3)
                                    {
                                        float num19 = numArray3[2];
                                        float num20 = numArray3[2];
                                        for (int num21 = 1; num21 < num18; num21++)
                                        {
                                            float num22 = numArray3[(num21 * 3) + 2];
                                            num19 = Math.Min(num19, num22);
                                            num20 = Math.Max(num20, num22);
                                        }
                                        int num23 = AstarMath.Clamp((int) Math.Round((double) num19), 0, this.voxelArea.depth - 1);
                                        int num24 = AstarMath.Clamp((int) Math.Round((double) num20), 0, this.voxelArea.depth - 1);
                                        for (int num25 = num23; num25 <= num24; num25++)
                                        {
                                            int num26 = Utility.ClipPolygon(numArray3, num18, numArray4, 1f, -num25 + 0.5f, 2);
                                            if (num26 >= 3)
                                            {
                                                num26 = Utility.ClipPolygonY(numArray4, num26, numArray5, -1f, num25 + 0.5f, 2);
                                                if (num26 >= 3)
                                                {
                                                    float num27 = numArray5[1];
                                                    float num28 = numArray5[1];
                                                    for (int num29 = 1; num29 < num26; num29++)
                                                    {
                                                        float num30 = numArray5[(num29 * 3) + 1];
                                                        num27 = Math.Min(num27, num30);
                                                        num28 = Math.Max(num28, num30);
                                                    }
                                                    int num31 = (int) Math.Ceiling((double) num28);
                                                    if (num31 >= 0)
                                                    {
                                                        int num32 = (int) (num27 + 1f);
                                                        this.voxelArea.AddLinkedSpan((num25 * this.voxelArea.width) + n, (num32 < 0) ? 0 : ((uint) num32), (uint) num31, num15, this.voxelWalkableClimb);
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
            }
        }

        public void WalkContour(int x, int z, int i, ushort[] flags, List<int> verts)
        {
            int dir = 0;
            while ((flags[i] & ((ushort) (((int) 1) << dir))) == 0)
            {
                dir++;
            }
            int num2 = dir;
            int num3 = i;
            int num4 = this.voxelArea.areaTypes[i];
            int num5 = 0;
            while (num5++ < 0x9c40)
            {
                if ((flags[i] & ((ushort) (((int) 1) << dir))) != 0)
                {
                    bool isBorderVertex = false;
                    bool flag2 = false;
                    int item = x;
                    int num7 = this.GetCornerHeight(x, z, i, dir, ref isBorderVertex);
                    int num8 = z;
                    switch (dir)
                    {
                        case 0:
                            num8 += this.voxelArea.width;
                            break;

                        case 1:
                            item++;
                            num8 += this.voxelArea.width;
                            break;

                        case 2:
                            item++;
                            break;
                    }
                    int reg = 0;
                    CompactVoxelSpan span = this.voxelArea.compactSpans[i];
                    if (span.GetConnection(dir) != 0x3fL)
                    {
                        int num10 = x + this.voxelArea.DirectionX[dir];
                        int num11 = z + this.voxelArea.DirectionZ[dir];
                        int index = ((int) this.voxelArea.compactCells[num10 + num11].index) + span.GetConnection(dir);
                        reg = this.voxelArea.compactSpans[index].reg;
                        if (num4 != this.voxelArea.areaTypes[index])
                        {
                            flag2 = true;
                        }
                    }
                    if (isBorderVertex)
                    {
                        reg |= 0x10000;
                    }
                    if (flag2)
                    {
                        reg |= 0x20000;
                    }
                    verts.Add(item);
                    verts.Add(num7);
                    verts.Add(num8);
                    verts.Add(reg);
                    flags[i] = (ushort) (flags[i] & ~(((int) 1) << dir));
                    dir = (dir + 1) & 3;
                }
                else
                {
                    int num13 = -1;
                    int num14 = x + this.voxelArea.DirectionX[dir];
                    int num15 = z + this.voxelArea.DirectionZ[dir];
                    CompactVoxelSpan span2 = this.voxelArea.compactSpans[i];
                    if (span2.GetConnection(dir) != 0x3fL)
                    {
                        CompactVoxelCell cell = this.voxelArea.compactCells[num14 + num15];
                        num13 = ((int) cell.index) + span2.GetConnection(dir);
                    }
                    if (num13 == -1)
                    {
                        Debug.LogError("This should not happen");
                        return;
                    }
                    x = num14;
                    z = num15;
                    i = num13;
                    dir = (dir + 3) & 3;
                }
                if ((num3 == i) && (num2 == dir))
                {
                    break;
                }
            }
        }

        public static bool Xorb(bool x, bool y)
        {
            return (!x ^ !y);
        }
    }
}

