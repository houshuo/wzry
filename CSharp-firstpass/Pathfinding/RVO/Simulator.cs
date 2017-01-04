namespace Pathfinding.RVO
{
    using Pathfinding.RVO.Sampled;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    public class Simulator
    {
        public List<Agent> agents;
        public SamplingAlgorithm algorithm;
        private WorkerContext coroutineWorkerContext;
        public int DeltaTimeMS;
        private float desiredDeltaTime = 0.05f;
        private bool doCleanObstacles;
        private bool doubleBuffering = true;
        private bool doUpdateObstacles;
        public static long frameNum;
        private bool interpolation = true;
        private float lastStep;
        private float lastStepInterpolationReference;
        public List<ObstacleVertex> obstacles;
        private bool oversampling;
        private RVOQuadtree quadtree = new RVOQuadtree();
        public VFactor qualityCutoff;
        public float stepScale;
        private VFactor wallThickness;
        private Worker[] workers;

        public Simulator(int workers, bool doubleBuffering)
        {
            VFactor factor = new VFactor {
                nom = 1L,
                den = 20L
            };
            this.qualityCutoff = factor;
            this.stepScale = 1.5f;
            this.lastStep = -99999f;
            this.lastStepInterpolationReference = -9999f;
            this.wallThickness = VFactor.one;
            this.coroutineWorkerContext = new WorkerContext();
            this.workers = new Worker[workers];
            this.doubleBuffering = doubleBuffering;
            for (int i = 0; i < workers; i++)
            {
                this.workers[i] = new Worker(this);
            }
            this.agents = new List<Agent>();
            this.obstacles = new List<ObstacleVertex>();
        }

        public Agent AddAgent(Agent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException("Agent must not be null");
            }
            if ((agent.simulator != null) && (agent.simulator == this))
            {
                throw new ArgumentException("The agent is already in the simulation");
            }
            if (agent.simulator != null)
            {
                throw new ArgumentException("The agent is already added to another simulation");
            }
            agent.simulator = this;
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            this.agents.Add(agent);
            return agent;
        }

        public Agent AddAgent(VInt3 position)
        {
            Agent item = new Agent(position);
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            this.agents.Add(item);
            item.simulator = this;
            return item;
        }

        public ObstacleVertex AddObstacle(ObstacleVertex v)
        {
            if (v == null)
            {
                throw new ArgumentNullException("Obstacle must not be null");
            }
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            this.obstacles.Add(v);
            this.UpdateObstacles();
            return v;
        }

        public ObstacleVertex AddObstacle(VInt3[] vertices, int height)
        {
            return this.AddObstacle(vertices, height, Matrix4x4.identity, RVOLayer.DefaultObstacle);
        }

        public ObstacleVertex AddObstacle(VInt3 a, VInt3 b, VInt height)
        {
            ObstacleVertex item = new ObstacleVertex();
            ObstacleVertex vertex2 = new ObstacleVertex();
            item.prev = vertex2;
            vertex2.prev = item;
            item.next = vertex2;
            vertex2.next = item;
            item.position = a;
            vertex2.position = b;
            item.height = height;
            vertex2.height = height;
            vertex2.ignore = true;
            VInt2 num = new VInt2(b.x - a.x, b.z - a.z);
            num.Normalize();
            item.dir = num;
            vertex2.dir = -num;
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            this.obstacles.Add(item);
            this.UpdateObstacles();
            return item;
        }

        public ObstacleVertex AddObstacle(VInt3[] vertices, int height, Matrix4x4 matrix, RVOLayer layer = 2)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException("Vertices must not be null");
            }
            if (vertices.Length < 2)
            {
                throw new ArgumentException("Less than 2 vertices in an obstacle");
            }
            ObstacleVertex item = null;
            ObstacleVertex vertex2 = null;
            bool flag = matrix == Matrix4x4.identity;
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int j = 0; j < this.workers.Length; j++)
                {
                    this.workers[j].WaitOne();
                }
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                ObstacleVertex vertex3 = new ObstacleVertex();
                if (item == null)
                {
                    item = vertex3;
                }
                else
                {
                    vertex2.next = vertex3;
                }
                vertex3.prev = vertex2;
                vertex3.layer = layer;
                vertex3.position = !flag ? ((VInt3) matrix.MultiplyPoint3x4((Vector3) vertices[i])) : vertices[i];
                vertex3.height = height;
                vertex2 = vertex3;
            }
            vertex2.next = item;
            item.prev = vertex2;
            ObstacleVertex next = item;
            do
            {
                VInt3 num3 = next.next.position - next.position;
                next.dir = num3.xz.normalized;
                next = next.next;
            }
            while (next != item);
            this.obstacles.Add(item);
            this.UpdateObstacles();
            return item;
        }

        private void BuildQuadtree()
        {
            this.quadtree.Clear();
            if (this.agents.Count > 0)
            {
                VRect r = VRect.MinMaxRect(this.agents[0].position.x, this.agents[0].position.y, this.agents[0].position.x, this.agents[0].position.y);
                for (int i = 1; i < this.agents.Count; i++)
                {
                    VInt3 position = this.agents[i].position;
                    int left = Mathf.Min(r.xMin, position.x);
                    int top = Mathf.Min(r.yMin, position.z);
                    int right = Mathf.Max(r.xMax, position.x);
                    r = VRect.MinMaxRect(left, top, right, Mathf.Max(r.yMax, position.z));
                }
                this.quadtree.SetBounds(r);
                for (int j = 0; j < this.agents.Count; j++)
                {
                    this.quadtree.Insert(this.agents[j]);
                }
            }
        }

        private void CalculateAgentNeighbours()
        {
            for (int i = 0; i < this.agents.Count; i++)
            {
                Agent agent = this.agents[i];
                agent.neighbours.Clear();
                agent.neighbourDists.Clear();
            }
            for (int j = 0; j < this.agents.Count; j++)
            {
                Agent agent2 = this.agents[j];
                long num3 = agent2.neighbourDist.i * agent2.neighbourDist.i;
                for (int k = j + 1; k < this.agents.Count; k++)
                {
                    Agent agent3 = this.agents[k];
                    if (!agent2.locked || !agent3.locked)
                    {
                        long num5 = agent3.neighbourDist.i * agent3.neighbourDist.i;
                        long distSq = agent2.position.XZSqrMagnitude(ref agent3.position);
                        if ((distSq < num3) && !agent2.locked)
                        {
                            agent2.InsertNeighbour(agent3, distSq);
                        }
                        if ((distSq < num5) && !agent3.locked)
                        {
                            agent3.InsertNeighbour(agent2, distSq);
                        }
                    }
                }
            }
        }

        private void CleanObstacles()
        {
            for (int i = 0; i < this.obstacles.Count; i++)
            {
                ObstacleVertex vertex = this.obstacles[i];
                ObstacleVertex next = vertex;
                do
                {
                    while (next.next.split)
                    {
                        next.next = next.next.next;
                        next.next.prev = next;
                    }
                    next = next.next;
                }
                while (next != vertex);
            }
        }

        public void ClearAgents()
        {
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int j = 0; j < this.workers.Length; j++)
                {
                    this.workers[j].WaitOne();
                }
            }
            for (int i = 0; i < this.agents.Count; i++)
            {
                this.agents[i].simulator = null;
            }
            this.agents.Clear();
        }

        ~Simulator()
        {
            this.OnDestroy();
        }

        public List<Agent> GetAgents()
        {
            return this.agents;
        }

        public List<ObstacleVertex> GetObstacles()
        {
            return this.obstacles;
        }

        public void OnDestroy()
        {
            if (this.workers != null)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].Terminate();
                }
            }
        }

        public void RemoveAgent(IAgent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException("Agent must not be null");
            }
            Agent item = agent as Agent;
            if (item == null)
            {
                throw new ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());
            }
            if (item.simulator != this)
            {
                throw new ArgumentException("The agent is not added to this simulation");
            }
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            item.simulator = null;
            if (!this.agents.Remove(item))
            {
                throw new ArgumentException("Critical Bug! This should not happen. Please report this.");
            }
        }

        public void RemoveObstacle(ObstacleVertex v)
        {
            if (v == null)
            {
                throw new ArgumentNullException("Vertex must not be null");
            }
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            this.obstacles.Remove(v);
            this.UpdateObstacles();
        }

        public void SafeRemoveAgent(IAgent agent)
        {
            if (agent != null)
            {
                Agent item = agent as Agent;
                if ((item != null) && (item.simulator == this))
                {
                    if (this.Multithreading && this.doubleBuffering)
                    {
                        for (int i = 0; i < this.workers.Length; i++)
                        {
                            this.workers[i].WaitOne();
                        }
                    }
                    item.simulator = null;
                    this.agents.Remove(item);
                }
            }
        }

        private void ScheduleCleanObstacles()
        {
            this.doCleanObstacles = true;
        }

        public void UpdateLogic(int nDelta)
        {
            this.DeltaTimeMS = nDelta;
            if (this.doCleanObstacles)
            {
                this.CleanObstacles();
                this.doCleanObstacles = false;
                this.doUpdateObstacles = true;
            }
            if (this.doUpdateObstacles)
            {
                this.doUpdateObstacles = false;
            }
            for (int i = 0; i < this.agents.Count; i++)
            {
                Agent agent = this.agents[i];
                agent.Update();
                agent.BufferSwitch();
            }
            this.CalculateAgentNeighbours();
            for (int j = 0; j < this.agents.Count; j++)
            {
                this.agents[j].computeNewVelocity();
            }
            for (int k = 0; k < this.agents.Count; k++)
            {
                this.agents[k].Interpolate();
            }
        }

        public void UpdateObstacle(ObstacleVertex obstacle, VInt3[] vertices, Matrix4x4 matrix)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException("Vertices must not be null");
            }
            if (obstacle == null)
            {
                throw new ArgumentNullException("Obstacle must not be null");
            }
            if (vertices.Length < 2)
            {
                throw new ArgumentException("Less than 2 vertices in an obstacle");
            }
            if (obstacle.split)
            {
                throw new ArgumentException("Obstacle is not a start vertex. You should only pass those ObstacleVertices got from AddObstacle method calls");
            }
            if (this.Multithreading && this.doubleBuffering)
            {
                for (int i = 0; i < this.workers.Length; i++)
                {
                    this.workers[i].WaitOne();
                }
            }
            int index = 0;
            ObstacleVertex next = obstacle;
            do
            {
                while (next.next.split)
                {
                    next.next = next.next.next;
                    next.next.prev = next;
                }
                if (index >= vertices.Length)
                {
                    Debug.DrawLine((Vector3) next.prev.position, (Vector3) next.position, Color.red);
                    throw new ArgumentException("Obstacle has more vertices than supplied for updating (" + vertices.Length + " supplied)");
                }
                next.position = (VInt3) matrix.MultiplyPoint3x4((Vector3) vertices[index]);
                index++;
                next = next.next;
            }
            while (next != obstacle);
            next = obstacle;
            do
            {
                VInt3 num3 = next.next.position - next.position;
                next.dir = num3.xz.normalized;
                next = next.next;
            }
            while (next != obstacle);
            this.ScheduleCleanObstacles();
            this.UpdateObstacles();
        }

        public void UpdateObstacles()
        {
            this.doUpdateObstacles = true;
        }

        public bool Interpolation
        {
            get
            {
                return this.interpolation;
            }
            set
            {
                this.interpolation = value;
            }
        }

        public bool Multithreading
        {
            get
            {
                return ((this.workers != null) && (this.workers.Length > 0));
            }
        }

        public bool Oversampling
        {
            get
            {
                return this.oversampling;
            }
            set
            {
                this.oversampling = value;
            }
        }

        public RVOQuadtree Quadtree
        {
            get
            {
                return this.quadtree;
            }
        }

        public VFactor WallThickness
        {
            get
            {
                return this.wallThickness;
            }
            set
            {
                this.wallThickness = value;
            }
        }

        public enum SamplingAlgorithm
        {
            AdaptiveSampling,
            GradientDecent
        }

        private class Worker
        {
            private Simulator.WorkerContext context = new Simulator.WorkerContext();
            public int end;
            public AutoResetEvent runFlag = new AutoResetEvent(false);
            public Simulator simulator;
            public int start;
            public int task;
            private bool terminate;
            [NonSerialized]
            public Thread thread;
            public ManualResetEvent waitFlag = new ManualResetEvent(true);

            public Worker(Simulator sim)
            {
                this.simulator = sim;
                this.thread = new Thread(new ThreadStart(this.Run));
                this.thread.IsBackground = true;
                this.thread.Name = "RVO Simulator Thread";
                this.thread.Start();
            }

            public void Execute(int task)
            {
                this.task = task;
                this.waitFlag.Reset();
                this.runFlag.Set();
            }

            public void Run()
            {
                this.runFlag.WaitOne();
                while (!this.terminate)
                {
                    try
                    {
                        List<Agent> agents = this.simulator.GetAgents();
                        if (this.task == 0)
                        {
                            for (int i = this.start; i < this.end; i++)
                            {
                                agents[i].CalculateNeighbours();
                                agents[i].CalculateVelocity(this.context);
                            }
                        }
                        else if (this.task == 1)
                        {
                            for (int j = this.start; j < this.end; j++)
                            {
                                agents[j].Update();
                                agents[j].BufferSwitch();
                            }
                        }
                        else
                        {
                            if (this.task != 2)
                            {
                                Debug.LogError("Invalid Task Number: " + this.task);
                                throw new Exception("Invalid Task Number: " + this.task);
                            }
                            this.simulator.BuildQuadtree();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception);
                    }
                    this.waitFlag.Set();
                    this.runFlag.WaitOne();
                }
            }

            public void Terminate()
            {
                this.terminate = true;
            }

            public void WaitOne()
            {
                this.waitFlag.WaitOne();
            }
        }

        internal class WorkerContext
        {
            public VInt2[] bestPos = new VInt2[3];
            public VFactor[] bestScores = new VFactor[4];
            public long[] bestSizes = new long[3];
            public const int KeepCount = 3;
            public VInt2[] samplePos = new VInt2[50];
            public long[] sampleSize = new long[50];
            public Agent.VO[] vos = new Agent.VO[20];
        }
    }
}

