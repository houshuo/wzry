using Pathfinding;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GraphNodeRasterizer
{
    public List<object>[] cells;
    public int cellSize;
    private Color[] debugColors = new Color[] { new Color(0f, 1f, 0f, 0.5f), new Color(1f, 0f, 0f, 0.5f), new Color(0f, 0f, 1f, 0.5f), new Color(1f, 1f, 0f, 0.5f) };
    public int DebugNodeID = -1;
    private Edge[] edges = new Edge[3];
    public int numCellsX;
    public int numCellsY;
    public VInt2 origin;
    public int SizeX;
    public int SizeY;
    private float[] tempVar = new float[4];

    private void AddEdge(ref Edge e0, ref Edge e1, object data)
    {
        if ((e0.y0 != e0.y1) && (e1.y0 != e1.y1))
        {
            float num = (e0.y1 - e0.y0) * 0.001f;
            float num2 = (e1.y1 - e1.y0) * 0.001f;
            float num3 = (e0.x1 - e0.x0) * 0.001f;
            float num4 = (e1.x1 - e1.x0) * 0.001f;
            float num5 = ((float) (e1.y0 - e0.y0)) / num;
            float num6 = ((float) this.cellSize) / num;
            float num7 = 0f;
            float num8 = ((float) this.cellSize) / num2;
            int num15 = e1.y0;
            int cellSize = this.cellSize;
            int num17 = (num15 - this.origin.y) % this.cellSize;
            if (num17 != 0)
            {
                cellSize = this.cellSize - num17;
            }
            while (num15 <= e1.y1)
            {
                this.tempVar[0] = (num3 * num5) + e0.x0;
                this.tempVar[1] = (num4 * num7) + e1.x0;
                float num9 = 1f;
                if ((num15 + cellSize) <= e1.y1)
                {
                    if (cellSize != this.cellSize)
                    {
                        num9 = ((float) cellSize) / ((float) this.cellSize);
                    }
                }
                else
                {
                    num9 = ((float) (e1.y1 - num15)) / ((float) this.cellSize);
                }
                num5 += num6 * num9;
                num7 += num8 * num9;
                this.tempVar[2] = (num3 * num5) + e0.x0;
                this.tempVar[3] = (num4 * num7) + e1.x0;
                int num10 = Mathf.FloorToInt(Mathf.Min(this.tempVar));
                int num11 = Mathf.CeilToInt(Mathf.Max(this.tempVar));
                int y = (num15 - this.origin.y) / this.cellSize;
                int num12 = (num10 - this.origin.x) / this.cellSize;
                int num13 = (num11 - this.origin.x) / this.cellSize;
                this.AddLine(num12, num13, y, data);
                num15 += cellSize;
                cellSize = this.cellSize;
            }
        }
    }

    private void AddLine(int x0, int x1, int y, object data)
    {
        int num = y * this.numCellsX;
        for (int i = x0; i <= x1; i++)
        {
            int index = num + i;
            if ((index < 0) || (index >= this.cells.Length))
            {
                DebugHelper.Assert((index >= 0) && (index < this.cells.Length), "index of rasterizer cells is out of range !");
            }
            List<object> list = this.cells[index];
            if (list == null)
            {
                list = new List<object>();
                this.cells[index] = list;
                list.Add(data);
            }
            else if (!list.Contains(data))
            {
                list.Add(data);
            }
        }
    }

    public void AddTriangle(ref VInt2 v0, ref VInt2 v1, ref VInt2 v2, object data)
    {
        this.InitEdge(ref v0, ref v1, 0);
        this.InitEdge(ref v1, ref v2, 1);
        this.InitEdge(ref v2, ref v0, 2);
        int longEdge = this.GetLongEdge();
        int index = (longEdge + 1) % 3;
        int num3 = (longEdge + 2) % 3;
        this.AddEdge(ref this.edges[longEdge], ref this.edges[index], data);
        this.AddEdge(ref this.edges[longEdge], ref this.edges[num3], data);
    }

    private void drawAll()
    {
        if (this.cells != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            float x = this.cellSize * 0.001f;
            Vector3 size = new Vector3(x, 1f, x);
            for (int i = 0; i < this.numCellsY; i++)
            {
                for (int j = 0; j < this.numCellsX; j++)
                {
                    int index = (i * this.numCellsX) + j;
                    if (this.cells[index] != null)
                    {
                        float num5 = (((j * this.cellSize) + this.origin.x) + (this.cellSize / 2)) * 0.001f;
                        float z = (((i * this.cellSize) + this.origin.y) + (this.cellSize / 2)) * 0.001f;
                        Gizmos.color = this.debugColors[((i % 2) * 2) + (j % 2)];
                        Gizmos.DrawCube(new Vector3(num5, 0f, z), size);
                    }
                }
            }
        }
    }

    public void DrawGizmos()
    {
    }

    private void drawSelected()
    {
        if ((this.cells != null) && (this.DebugNodeID != -1))
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            float x = this.cellSize * 0.001f;
            Vector3 size = new Vector3(x, 1f, x);
            for (int i = 0; i < this.numCellsY; i++)
            {
                for (int j = 0; j < this.numCellsX; j++)
                {
                    int index = (i * this.numCellsX) + j;
                    if ((this.cells[index] != null) && (this.cells[index].Find(o => (o as TriangleMeshNode).NodeIndex == this.DebugNodeID) != null))
                    {
                        float num5 = (((j * this.cellSize) + this.origin.x) + (this.cellSize / 2)) * 0.001f;
                        float z = (((i * this.cellSize) + this.origin.y) + (this.cellSize / 2)) * 0.001f;
                        Gizmos.color = this.debugColors[((i % 2) * 2) + (j % 2)];
                        Gizmos.DrawCube(new Vector3(num5, 0f, z), size);
                    }
                }
            }
        }
    }

    public void GetCellPosClamped(out int x, out int y, VInt3 pos)
    {
        x = (pos.x - this.origin.x) / this.cellSize;
        y = (pos.z - this.origin.y) / this.cellSize;
        x = Mathf.Clamp(x, 0, this.numCellsX - 1);
        y = Mathf.Clamp(y, 0, this.numCellsY - 1);
    }

    public List<object> GetLocated(VInt3 pos)
    {
        int num = pos.x - this.origin.x;
        int num2 = pos.z - this.origin.y;
        num /= this.cellSize;
        num2 /= this.cellSize;
        if (((num < 0) || (num2 < 0)) || ((num >= this.numCellsX) || (num2 >= this.numCellsY)))
        {
            return null;
        }
        return this.cells[(num2 * this.numCellsX) + num];
    }

    private int GetLongEdge()
    {
        int num = this.edges[0].y1 - this.edges[0].y0;
        int num2 = this.edges[1].y1 - this.edges[1].y0;
        int num3 = this.edges[2].y1 - this.edges[2].y0;
        if (num > num2)
        {
            return ((num <= num3) ? 2 : 0);
        }
        return ((num2 <= num3) ? 2 : 1);
    }

    public List<object> GetObjs(int x, int y)
    {
        int index = (y * this.numCellsX) + x;
        return this.cells[index];
    }

    public void Init(VInt2 pos, int sizeX, int sizeY, int inCellSize)
    {
        this.origin = pos;
        this.SizeX = sizeX;
        this.SizeY = sizeY;
        this.cellSize = inCellSize;
        this.numCellsX = (sizeX / this.cellSize) + 1;
        this.numCellsY = (sizeY / this.cellSize) + 1;
        this.cells = new List<object>[this.numCellsX * this.numCellsY];
    }

    private void InitEdge(ref VInt2 v0, ref VInt2 v1, int index)
    {
        Edge edge = new Edge();
        if (v0.y < v1.y)
        {
            edge.x0 = v0.x;
            edge.y0 = v0.y;
            edge.x1 = v1.x;
            edge.y1 = v1.y;
        }
        else
        {
            edge.x0 = v1.x;
            edge.y0 = v1.y;
            edge.x1 = v0.x;
            edge.y1 = v0.y;
        }
        this.edges[index] = edge;
    }

    public void InitPot(VInt2 pos, int sizeX, int sizeY, int inCellSize)
    {
        this.origin = pos;
        this.SizeX = sizeX;
        this.SizeY = sizeY;
        this.numCellsX = sizeX / inCellSize;
        int num = 2;
        while (this.numCellsX > num)
        {
            num *= 2;
        }
        this.numCellsX = num;
        this.numCellsY = sizeY / inCellSize;
        int num2 = 2;
        while (this.numCellsY > num2)
        {
            num2 *= 2;
        }
        this.numCellsY = num2;
        this.cellSize = sizeX / (this.numCellsX - 1);
        this.cells = new List<object>[this.numCellsX * this.numCellsY];
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Edge
    {
        public int x0;
        public int y0;
        public int x1;
        public int y1;
    }
}

