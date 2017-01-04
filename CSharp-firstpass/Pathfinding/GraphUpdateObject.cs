namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GraphUpdateObject
    {
        public int addPenalty;
        private List<uint> backupData;
        private List<VInt3> backupPositionData;
        public Bounds bounds;
        public List<GraphNode> changedNodes;
        public bool modifyTag;
        public bool modifyWalkability;
        public NNConstraint nnConstraint;
        public bool requiresFloodFill;
        public bool resetPenaltyOnPhysics;
        public int setTag;
        public bool setWalkability;
        public GraphUpdateShape shape;
        public bool trackChangedNodes;
        public bool updateErosion;
        public bool updatePhysics;

        public GraphUpdateObject()
        {
            this.requiresFloodFill = true;
            this.updatePhysics = true;
            this.resetPenaltyOnPhysics = true;
            this.updateErosion = true;
            this.nnConstraint = NNConstraint.None;
        }

        public GraphUpdateObject(Bounds b)
        {
            this.requiresFloodFill = true;
            this.updatePhysics = true;
            this.resetPenaltyOnPhysics = true;
            this.updateErosion = true;
            this.nnConstraint = NNConstraint.None;
            this.bounds = b;
        }

        public virtual void Apply(GraphNode node)
        {
            if ((this.shape == null) || this.shape.Contains(node))
            {
                node.Penalty += (uint) this.addPenalty;
                if (this.modifyWalkability)
                {
                    node.Walkable = this.setWalkability;
                }
                if (this.modifyTag)
                {
                    node.Tag = (uint) this.setTag;
                }
            }
        }

        public virtual void RevertFromBackup()
        {
            if (!this.trackChangedNodes)
            {
                throw new InvalidOperationException("Changed nodes have not been tracked, cannot revert from backup");
            }
            if (this.changedNodes != null)
            {
                int num = 0;
                for (int i = 0; i < this.changedNodes.Count; i++)
                {
                    this.changedNodes[i].Penalty = this.backupData[num];
                    num++;
                    this.changedNodes[i].Flags = this.backupData[num];
                    num++;
                    this.changedNodes[i].position = this.backupPositionData[i];
                }
                ListPool<GraphNode>.Release(this.changedNodes);
                ListPool<uint>.Release(this.backupData);
                ListPool<VInt3>.Release(this.backupPositionData);
            }
        }

        public virtual void WillUpdateNode(GraphNode node)
        {
            if (this.trackChangedNodes && (node != null))
            {
                if (this.changedNodes == null)
                {
                    this.changedNodes = ListPool<GraphNode>.Claim();
                    this.backupData = ListPool<uint>.Claim();
                    this.backupPositionData = ListPool<VInt3>.Claim();
                }
                this.changedNodes.Add(node);
                this.backupPositionData.Add(node.position);
                this.backupData.Add(node.Penalty);
                this.backupData.Add(node.Flags);
            }
        }
    }
}

