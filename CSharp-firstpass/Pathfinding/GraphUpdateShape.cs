namespace Pathfinding
{
    using System;
    using UnityEngine;

    public class GraphUpdateShape
    {
        private bool _convex;
        private Vector3[] _convexPoints;
        private Vector3[] _points;

        private void CalculateConvexHull()
        {
            if (this.points == null)
            {
                this._convexPoints = null;
            }
            else
            {
                this._convexPoints = Polygon.ConvexHull(this.points);
                for (int i = 0; i < this._convexPoints.Length; i++)
                {
                    Debug.DrawLine(this._convexPoints[i], this._convexPoints[(i + 1) % this._convexPoints.Length], Color.green);
                }
            }
        }

        public bool Contains(GraphNode node)
        {
            Vector3 position = (Vector3) node.position;
            if (this.convex)
            {
                if (this._convexPoints == null)
                {
                    return false;
                }
                int index = 0;
                int num2 = this._convexPoints.Length - 1;
                while (index < this._convexPoints.Length)
                {
                    if (Polygon.Left(this._convexPoints[index], this._convexPoints[num2], position))
                    {
                        return false;
                    }
                    num2 = index;
                    index++;
                }
                return true;
            }
            if (this._points == null)
            {
                return false;
            }
            return Polygon.ContainsPoint(this._points, position);
        }

        public bool Contains(Vector3 point)
        {
            if (this.convex)
            {
                if (this._convexPoints == null)
                {
                    return false;
                }
                int index = 0;
                int num2 = this._convexPoints.Length - 1;
                while (index < this._convexPoints.Length)
                {
                    if (Polygon.Left(this._convexPoints[index], this._convexPoints[num2], point))
                    {
                        return false;
                    }
                    num2 = index;
                    index++;
                }
                return true;
            }
            if (this._points == null)
            {
                return false;
            }
            return Polygon.ContainsPoint(this._points, point);
        }

        public Bounds GetBounds()
        {
            if ((this.points == null) || (this.points.Length == 0))
            {
                return new Bounds();
            }
            Vector3 lhs = this.points[0];
            Vector3 vector2 = this.points[0];
            for (int i = 0; i < this.points.Length; i++)
            {
                lhs = Vector3.Min(lhs, this.points[i]);
                vector2 = Vector3.Max(vector2, this.points[i]);
            }
            return new Bounds((Vector3) ((lhs + vector2) * 0.5f), vector2 - lhs);
        }

        public bool convex
        {
            get
            {
                return this._convex;
            }
            set
            {
                if ((this._convex != value) && value)
                {
                    this._convex = value;
                    this.CalculateConvexHull();
                }
                else
                {
                    this._convex = value;
                }
            }
        }

        public Vector3[] points
        {
            get
            {
                return this._points;
            }
            set
            {
                this._points = value;
                if (this.convex)
                {
                    this.CalculateConvexHull();
                }
            }
        }
    }
}

