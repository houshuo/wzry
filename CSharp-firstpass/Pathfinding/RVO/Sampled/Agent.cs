namespace Pathfinding.RVO.Sampled
{
    using Pathfinding;
    using Pathfinding.RVO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Agent : IAgent
    {
        private static long[] adpSamp_Steps1_Pos_Den;
        private static long[] adpSamp_Steps1_Pos_Nom1;
        private static long[] adpSamp_Steps1_Pos_Nom2;
        private static int[] adpSamp_Steps1_Size_Nom;
        private static long[] adpSamp_Steps2_Den;
        private static VFactor adpSamp_Steps2_InnerScale;
        private static long[] adpSamp_Steps2_Nom_fw;
        private static long[] adpSamp_Steps2_Nom_rw;
        private static VFactor[] adpSamp_Steps3_Cos;
        private static VFactor[] adpSamp_Steps3_Sin;
        public VInt agentTimeHorizon;
        private RVOLayer collidesWith;
        public VInt3 desiredVelocity;
        public static float DesiredVelocityScale;
        public static float DesiredVelocityWeight;
        public static int GlobalIncompressibility;
        public bool hasCollided;
        public VInt height;
        private RVOLayer layer;
        public bool locked;
        public int maxNeighbours;
        public VInt maxSpeed;
        public VInt neighbourDist;
        public List<long> neighbourDists = new List<long>(8);
        public List<Agent> neighbours = new List<Agent>(8);
        public VInt3 newVelocity;
        public Agent next;
        private List<float> obstacleDists = new List<float>();
        private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();
        private List<ObstacleVertex> obstaclesBuffered = new List<ObstacleVertex>();
        public VInt obstacleTimeHorizon;
        private static List<VLine> orcaLines;
        [NonSerialized]
        public object owner;
        public VInt3 position;
        public VInt3 prevSmoothPos;
        private static List<VLine> projLines;
        public VInt radius;
        public Simulator simulator;
        private VInt3 smoothPos;
        public VInt3 velocity;
        public static VInt WallWeight;
        public static Stopwatch watch1;
        public static Stopwatch watch2;
        public VInt weight;

        static Agent()
        {
            VFactor factor;
            VFactor factor2;
            orcaLines = new List<VLine>();
            projLines = new List<VLine>();
            watch1 = new Stopwatch();
            watch2 = new Stopwatch();
            adpSamp_Steps1_Size_Nom = null;
            adpSamp_Steps1_Pos_Nom1 = null;
            adpSamp_Steps1_Pos_Nom2 = null;
            adpSamp_Steps1_Pos_Den = null;
            adpSamp_Steps2_Nom_rw = null;
            adpSamp_Steps2_Nom_fw = null;
            adpSamp_Steps2_Den = null;
            adpSamp_Steps2_InnerScale = new VFactor(3L, 5L);
            adpSamp_Steps3_Sin = null;
            adpSamp_Steps3_Cos = null;
            DesiredVelocityWeight = 0.02f;
            DesiredVelocityScale = 0.1f;
            GlobalIncompressibility = 30;
            WallWeight = 0x1388;
            adpSamp_Steps1_Pos_Nom1 = new long[8];
            adpSamp_Steps1_Pos_Nom2 = new long[8];
            adpSamp_Steps1_Pos_Den = new long[8];
            adpSamp_Steps1_Size_Nom = new int[8];
            for (int i = 0; i < 8; i++)
            {
                IntMath.sincos(out factor, out factor2, (long) (i * 0xf570), 0x13880L);
                factor2.nom += factor2.den;
                adpSamp_Steps1_Pos_Nom1[i] = factor.nom * factor2.den;
                adpSamp_Steps1_Pos_Nom2[i] = factor.den * factor2.nom;
                adpSamp_Steps1_Pos_Den[i] = factor.den * factor2.den;
                adpSamp_Steps1_Size_Nom[i] = 8 - Mathf.Abs((int) (i - 4));
            }
            VFactor inverse = adpSamp_Steps2_InnerScale.Inverse;
            adpSamp_Steps2_Nom_rw = new long[6];
            adpSamp_Steps2_Nom_fw = new long[6];
            adpSamp_Steps2_Den = new long[6];
            for (int j = 0; j < 6; j++)
            {
                IntMath.sincos(out factor, out factor2, (long) (((2 * j) + 1) * 0x7ab8), 0xea60L);
                factor += inverse;
                adpSamp_Steps2_Den[j] = (factor2.den * factor.den) * adpSamp_Steps2_InnerScale.den;
                adpSamp_Steps2_Nom_rw[j] = factor2.nom * factor.den;
                adpSamp_Steps2_Nom_fw[j] = factor.nom * factor2.den;
            }
            adpSamp_Steps3_Sin = new VFactor[6];
            adpSamp_Steps3_Cos = new VFactor[6];
            for (int k = 0; k < 6; k++)
            {
                IntMath.sincos(out factor, out factor2, (long) (((2 * k) + 1) * 0x7ab8), 0xea60L);
                factor.den *= 5L;
                factor2.den *= 5L;
                adpSamp_Steps3_Sin[k] = factor;
                adpSamp_Steps3_Cos[k] = factor2;
            }
        }

        public Agent(VInt3 pos)
        {
            this.MaxSpeed = 0x7d0;
            this.NeighbourDist = 15;
            this.AgentTimeHorizon = 0x7d0;
            this.ObstacleTimeHorizon = 0x7d0;
            this.Height = 0x1388;
            this.Radius = 0x1388;
            this.MaxNeighbours = 10;
            this.Locked = false;
            this.position = pos;
            this.Position = this.position;
            this.prevSmoothPos = this.position;
            this.smoothPos = this.position;
            this.Layer = RVOLayer.DefaultAgent;
            this.CollidesWith = -1;
        }

        public void BufferSwitch()
        {
            this.radius = this.Radius;
            this.height = this.Height;
            this.maxSpeed = this.MaxSpeed;
            this.neighbourDist = this.NeighbourDist;
            this.agentTimeHorizon = this.AgentTimeHorizon;
            this.obstacleTimeHorizon = this.ObstacleTimeHorizon;
            this.maxNeighbours = this.MaxNeighbours;
            this.desiredVelocity = this.DesiredVelocity;
            this.locked = this.Locked;
            this.collidesWith = this.CollidesWith;
            this.layer = this.Layer;
            this.Velocity = this.velocity;
            List<ObstacleVertex> obstaclesBuffered = this.obstaclesBuffered;
            this.obstaclesBuffered = this.obstacles;
            this.obstacles = obstaclesBuffered;
        }

        private static void CalcSamplePos_Step1(VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale, VInt2 rw, VInt2 fw)
        {
            VFactor factor = new VFactor {
                den = 0x10L
            };
            for (int i = 0; i < 8; i++)
            {
                factor.nom = adpSamp_Steps1_Size_Nom[i] * sampleScale;
                samplePos[samplePosCount].x = (int) IntMath.Divide((long) ((rw.x * adpSamp_Steps1_Pos_Nom1[i]) + (fw.x * adpSamp_Steps1_Pos_Nom2[i])), adpSamp_Steps1_Pos_Den[i]);
                samplePos[samplePosCount].y = (int) IntMath.Divide((long) ((rw.y * adpSamp_Steps1_Pos_Nom1[i]) + (fw.y * adpSamp_Steps1_Pos_Nom2[i])), adpSamp_Steps1_Pos_Den[i]);
                sampleSize[samplePosCount] = factor.roundInt;
                samplePosCount++;
            }
        }

        private static void CalcSamplePos_Step2(VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale, ref VInt2 fw, ref VInt2 rw)
        {
            int num = IntMath.Divide((int) (sampleScale * 3), 10);
            for (int i = 0; i < 6; i++)
            {
                samplePos[samplePosCount].x = (int) IntMath.Divide((long) (((rw.x * adpSamp_Steps2_Nom_rw[i]) + (fw.x * adpSamp_Steps2_Nom_fw[i])) * adpSamp_Steps2_InnerScale.nom), adpSamp_Steps2_Den[i]);
                samplePos[samplePosCount].y = (int) IntMath.Divide((long) (((rw.y * adpSamp_Steps2_Nom_rw[i]) + (fw.y * adpSamp_Steps2_Nom_fw[i])) * adpSamp_Steps2_InnerScale.nom), adpSamp_Steps2_Den[i]);
                sampleSize[samplePosCount] = num;
                samplePosCount++;
            }
        }

        private static void CalcSamplePos_Step3(ref VInt2 optimalVelocity, VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale)
        {
            int num = IntMath.Divide((int) (sampleScale * 2), 5);
            for (int i = 0; i < 6; i++)
            {
                samplePos[samplePosCount].x = optimalVelocity.x + (sampleScale * adpSamp_Steps3_Cos[i]);
                samplePos[samplePosCount].y = optimalVelocity.y + (sampleScale * adpSamp_Steps3_Sin[i]);
                sampleSize[samplePosCount] = num;
                samplePosCount++;
            }
        }

        private void CalcSamplePosScore(Simulator.WorkerContext context, ref VInt2 position2D, VO[] vos, int voCount, ref VInt2 optimalVelocity, ref VInt2 result, VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, ref VInt2 desired2D)
        {
            VInt2[] bestPos = context.bestPos;
            long[] bestSizes = context.bestSizes;
            VFactor[] bestScores = context.bestScores;
            for (int i = 0; i < 3; i++)
            {
                bestScores[i] = new VFactor(0x7fffffffL, 1L);
            }
            bestScores[3] = new VFactor(-2147483648L, 1L);
            VInt2 num3 = optimalVelocity;
            VFactor factor = new VFactor(0x7fffffffL, 1L);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < samplePosCount; k++)
                {
                    VFactor zero = VFactor.zero;
                    for (int n = 0; n < voCount; n++)
                    {
                        VFactor factor3 = vos[n].ScalarSample(samplePos[k]);
                        zero = (zero <= factor3) ? factor3 : zero;
                    }
                    VInt2 num14 = samplePos[k] - desired2D;
                    int magnitude = num14.magnitude;
                    VFactor factor4 = zero + new VFactor((long) magnitude, 5L);
                    zero += new VFactor((long) magnitude, 0x3e8L);
                    if (this.DebugDraw)
                    {
                        DrawCross((Vector2) (position2D + samplePos[k]), Rainbow(Mathf.Log(zero.single + 1f) * 5f), sampleSize[k] * 0.5f);
                    }
                    if (factor4 < bestScores[0])
                    {
                        for (int num8 = 0; num8 < 3; num8++)
                        {
                            if (factor4 >= bestScores[num8 + 1])
                            {
                                for (int num9 = 0; num9 < num8; num9++)
                                {
                                    bestScores[num9] = bestScores[num9 + 1];
                                    bestSizes[num9] = bestSizes[num9 + 1];
                                    bestPos[num9] = bestPos[num9 + 1];
                                }
                                bestScores[num8] = factor4;
                                bestSizes[num8] = sampleSize[k];
                                bestPos[num8] = samplePos[k];
                                break;
                            }
                        }
                    }
                    if (zero < factor)
                    {
                        num3 = samplePos[k];
                        factor = zero;
                        if (zero.IsZero)
                        {
                            j = 100;
                            break;
                        }
                    }
                }
                samplePosCount = 0;
                for (int m = 0; m < 3; m++)
                {
                    VInt2 num11 = bestPos[m];
                    long num12 = bestSizes[m];
                    bestScores[m] = new VFactor(0x7fffffffL, 1L);
                    int num13 = (int) IntMath.Divide((long) (num12 * 3L), (long) 10L);
                    samplePos[samplePosCount].x = num11.x + num13;
                    samplePos[samplePosCount].y = num11.y + num13;
                    samplePos[samplePosCount + 1].x = num11.x - num13;
                    samplePos[samplePosCount + 1].y = num11.y + num13;
                    samplePos[samplePosCount + 2].x = num11.x - num13;
                    samplePos[samplePosCount + 2].y = num11.y - num13;
                    samplePos[samplePosCount + 3].x = num11.x + num13;
                    samplePos[samplePosCount + 3].y = num11.y - num13;
                    num12 = IntMath.Divide((long) ((num12 * num12) * 3L), (long) 0x1388L);
                    sampleSize[samplePosCount] = num12;
                    sampleSize[samplePosCount + 1] = num12;
                    sampleSize[samplePosCount + 2] = num12;
                    sampleSize[samplePosCount + 3] = num12;
                    samplePosCount += 4;
                }
            }
            result = num3;
        }

        public void CalculateNeighbours()
        {
            this.neighbours.Clear();
            this.neighbourDists.Clear();
            if (!this.locked)
            {
                if (this.MaxNeighbours > 0)
                {
                    this.simulator.Quadtree.Query(this.position.xz, (long) this.neighbourDist.i, this);
                }
                this.obstacles.Clear();
                this.obstacleDists.Clear();
            }
        }

        internal void CalculateVelocity(Simulator.WorkerContext context)
        {
            if (this.locked)
            {
                this.newVelocity = VInt3.zero;
            }
            else
            {
                if (context.vos.Length < (this.neighbours.Count + this.simulator.obstacles.Count))
                {
                    context.vos = new VO[Mathf.Max((int) (context.vos.Length * 2), (int) (this.neighbours.Count + this.simulator.obstacles.Count))];
                }
                VInt2 num = new VInt2(this.position.x, this.position.z);
                VO[] vos = context.vos;
                int voCount = 0;
                VInt2 optimalVelocity = new VInt2(this.velocity.x, this.velocity.z);
                VFactor inverseAgentTimeHorizon = new VFactor(0x3e8L, (long) this.agentTimeHorizon.i);
                VFactor one = VFactor.one;
                if (this.simulator.algorithm != Simulator.SamplingAlgorithm.GradientDecent)
                {
                    one.nom = WallWeight.i;
                    one.den = 0x3e8L;
                }
                VFactor factor3 = one;
                factor3.nom = factor3.nom << 1;
                VFactor factor4 = one;
                factor4.den *= 20L;
                this.CalcVelocity_Neighbours(ref num, vos, ref voCount, ref optimalVelocity, ref inverseAgentTimeHorizon);
                VInt2 zero = VInt2.zero;
                if (voCount > 0)
                {
                    if (this.simulator.algorithm == Simulator.SamplingAlgorithm.GradientDecent)
                    {
                        throw new NotImplementedException("GradientDecent");
                    }
                    VInt2[] samplePos = context.samplePos;
                    long[] sampleSize = context.sampleSize;
                    int index = 0;
                    VInt2 xz = this.desiredVelocity.xz;
                    int sampleScale = Mathf.Max(this.radius.i, Mathf.Max(xz.magnitude, this.Velocity.magnitude));
                    samplePos[index] = xz;
                    sampleSize[index] = (sampleScale * 3) / 10;
                    index++;
                    samplePos[index] = optimalVelocity;
                    sampleSize[index] = (sampleScale * 3) / 10;
                    index++;
                    VInt2 fw = new VInt2(optimalVelocity.x >> 1, optimalVelocity.y >> 1);
                    VInt2 rw = new VInt2(fw.y, -fw.x);
                    CalcSamplePos_Step1(samplePos, sampleSize, ref index, sampleScale, rw, fw);
                    CalcSamplePos_Step2(samplePos, sampleSize, ref index, sampleScale, ref fw, ref rw);
                    CalcSamplePos_Step3(ref optimalVelocity, samplePos, sampleSize, ref index, sampleScale);
                    samplePos[index] = IntMath.Divide(optimalVelocity, 2L);
                    sampleSize[index] = IntMath.Divide((int) (sampleScale * 2), 5);
                    index++;
                    this.CalcSamplePosScore(context, ref num, vos, voCount, ref optimalVelocity, ref zero, samplePos, sampleSize, ref index, ref xz);
                }
                else
                {
                    zero = this.desiredVelocity.xz;
                }
                if (this.DebugDraw)
                {
                    DrawCross((Vector2) (zero + num), 1f);
                }
                zero = VInt2.ClampMagnitude(zero, this.maxSpeed.i);
                this.newVelocity = new VInt3(zero.x, 0, zero.y);
            }
        }

        private void CalcVelocity_Neighbours(ref VInt2 position2D, VO[] vos, ref int voCount, ref VInt2 optimalVelocity, ref VFactor inverseAgentTimeHorizon)
        {
            for (int i = 0; i < this.neighbours.Count; i++)
            {
                Agent agent = this.neighbours[i];
                if (agent != this)
                {
                    int num2 = Math.Min((int) (this.position.y + this.height.i), (int) (agent.position.y + agent.height.i));
                    int num3 = Math.Max(this.position.y, agent.position.y);
                    if ((num2 - num3) >= 0)
                    {
                        VInt2 num8;
                        VInt2 xz = agent.Velocity.xz;
                        VInt num5 = this.radius + agent.radius;
                        VInt2 center = agent.position.xz - position2D;
                        VInt2 sideChooser = optimalVelocity - xz;
                        if (agent.locked)
                        {
                            num8 = xz;
                        }
                        else
                        {
                            num8 = optimalVelocity + xz;
                            num8.x = num8.x >> 1;
                            num8.y = num8.y >> 1;
                        }
                        vos[voCount] = new VO(center, num8, num5.i, sideChooser, inverseAgentTimeHorizon, VFactor.one);
                        voCount++;
                        if (this.DebugDraw)
                        {
                            VInt2 num9 = IntMath.Divide(center, inverseAgentTimeHorizon.nom, inverseAgentTimeHorizon.den);
                            VInt num10 = (int) IntMath.Divide((long) (num5.i * inverseAgentTimeHorizon.nom), inverseAgentTimeHorizon.den);
                            DrawVO((Vector2) ((position2D + num9) + num8), (float) num10, (Vector2) (position2D + num8));
                        }
                    }
                }
            }
        }

        private void CalcVelocity_Obstacles(ref VInt2 position2D, VO[] vos, ref int voCount, ref VFactor wallThickness, ref VFactor wallWeight, ref VFactor wallWeight2, ref VFactor wallWeight3)
        {
            for (int i = 0; i < this.simulator.obstacles.Count; i++)
            {
                ObstacleVertex vertex = this.simulator.obstacles[i];
                ObstacleVertex next = vertex;
                do
                {
                    if ((next.ignore || (this.position.y > (next.position.y + next.height.i))) || (((this.position.y + this.height.i) < next.position.y) || ((next.layer & this.collidesWith) == 0)))
                    {
                        next = next.next;
                    }
                    else
                    {
                        long num3 = VO.Det(next.position.xz, next.dir, position2D);
                        long num4 = VInt2.DotLong(next.dir, position2D - next.position.xz);
                        bool flag = (wallWeight3 >= num4) || ((wallWeight3 + num4) >= (next.position.xz - next.next.position.xz).magnitude);
                        if (Math.Abs(num3) < (this.neighbourDist.i * 0x3e8))
                        {
                            if (((num3 <= 0L) && !flag) && ((-wallThickness * 0x3e8L) < num3))
                            {
                                vos[voCount] = new VO(position2D, next.position.xz - position2D, next.dir, wallWeight2);
                                voCount++;
                            }
                            else if (num3 > 0L)
                            {
                                VInt2 num5 = next.position.xz - position2D;
                                VInt2 num6 = next.next.position.xz - position2D;
                                VInt2 normalized = num5.normalized;
                                VInt2 num8 = num6.normalized;
                                vos[voCount] = new VO(position2D, num5, num6, normalized, num8, wallWeight);
                                voCount++;
                            }
                        }
                        next = next.next;
                    }
                }
                while (next != vertex);
            }
        }

        public void computeNewVelocity()
        {
            orcaLines.Clear();
            this.hasCollided = false;
            int numObstLines = 0;
            VFactor factor = new VFactor(0x3e8L, (long) this.AgentTimeHorizon);
            VInt2 zero = VInt2.zero;
            VLine item = new VLine();
            VInt2 a = new VInt2();
            for (int i = 0; i < this.neighbours.Count; i++)
            {
                Agent agent = this.neighbours[i];
                VInt3 num21 = agent.position - this.position;
                VInt2 xz = num21.xz;
                VInt3 num22 = this.velocity - agent.velocity;
                VInt2 num6 = num22.xz;
                long b = VInt2.DotLong(ref xz, ref xz);
                long num8 = this.radius.i + agent.radius.i;
                long num9 = num8 * num8;
                if (b > num9)
                {
                    if (!this.hasCollided && ((b * 2L) <= (num9 * 3L)))
                    {
                        this.hasCollided = true;
                    }
                    VInt2 num10 = num6 - (xz * factor);
                    long sqrMagnitudeLong = num10.sqrMagnitudeLong;
                    long num12 = VInt2.DotLong(ref num10, ref xz);
                    if ((num12 < 0L) && ((num12 * num12) > (num9 * sqrMagnitudeLong)))
                    {
                        VInt2 num13 = num10;
                        num13.Normalize();
                        item.direction.x = num13.y;
                        item.direction.y = -num13.x;
                        long num14 = IntMath.Divide((long) (factor.nom * num8), factor.den) - IntMath.Sqrt(sqrMagnitudeLong);
                        a.x = (int) IntMath.Divide((long) (num13.x * num14), (long) 0x3e8L);
                        a.y = (int) IntMath.Divide((long) (num13.y * num14), (long) 0x3e8L);
                    }
                    else
                    {
                        long num15 = IntMath.SqrtLong(b - num9);
                        if (VInt2.DetLong(ref xz, ref num10) > 0L)
                        {
                            item.direction.x = (int) IntMath.Divide((long) (((xz.x * num15) - (xz.y * num8)) * 0x3e8L), b);
                            item.direction.y = (int) IntMath.Divide((long) (((xz.y * num8) - (xz.y * num15)) * 0x3e8L), b);
                        }
                        else
                        {
                            item.direction.x = -((int) IntMath.Divide((long) (((xz.x * num15) + (xz.y * num8)) * 0x3e8L), b));
                            item.direction.y = -((int) IntMath.Divide((long) (((-xz.x * num8) + (xz.y * num15)) * 0x3e8L), b));
                        }
                        item.direction.Normalize();
                        long num16 = VInt2.DotLong(ref num6, ref item.direction);
                        a.x = ((int) IntMath.Divide((long) (item.direction.x * num16), (long) 0xf4240L)) - num6.x;
                        a.y = ((int) IntMath.Divide((long) (item.direction.y * num16), (long) 0xf4240L)) - num6.y;
                    }
                }
                else
                {
                    this.hasCollided = true;
                    VFactor factor2 = new VFactor(0x3e8L, 30L);
                    VInt2 num17 = num6 - (xz * factor2);
                    VInt2 normalized = num17.normalized;
                    item.direction = new VInt2(normalized.y, -normalized.x);
                    long num19 = IntMath.Divide((long) (num8 * factor2.nom), factor2.den) - num17.magnitude;
                    a.x = (int) IntMath.Divide((long) (normalized.x * num19), (long) 0x3e8L);
                    a.y = (int) IntMath.Divide((long) (normalized.y * num19), (long) 0x3e8L);
                }
                item.point = this.velocity.xz + IntMath.Divide(a, 2L);
                orcaLines.Add(item);
            }
            int beginLine = linearProgram2(orcaLines, this.maxSpeed.i, this.desiredVelocity.xz, false, ref zero);
            if (beginLine < orcaLines.Count)
            {
                linearProgram3(orcaLines, numObstLines, beginLine, this.maxSpeed.i, ref zero);
            }
            this.newVelocity = this.FixVelocity(zero);
            orcaLines.Clear();
        }

        private static void DrawCircle(Vector2 _p, float radius, Color col)
        {
            DrawCircle(_p, radius, 0f, 6.283185f, col);
        }

        private static void DrawCircle(Vector2 _p, float radius, float a0, float a1, Color col)
        {
            Vector3 vector = To3D(_p);
            while (a0 > a1)
            {
                a0 -= 6.283185f;
            }
            Vector3 vector2 = new Vector3(Mathf.Cos(a0) * radius, 0f, Mathf.Sin(a0) * radius);
            for (int i = 0; i <= 40f; i++)
            {
                Vector3 vector3 = new Vector3(Mathf.Cos(Mathf.Lerp(a0, a1, ((float) i) / 40f)) * radius, 0f, Mathf.Sin(Mathf.Lerp(a0, a1, ((float) i) / 40f)) * radius);
                Debug.DrawLine(vector + vector2, vector + vector3, col);
                vector2 = vector3;
            }
        }

        private static void DrawCross(Vector2 p, float size = 1)
        {
            DrawCross(p, Color.white, size);
        }

        private static void DrawCross(Vector2 p, Color col, float size = 1)
        {
            size *= 0.5f;
            Debug.DrawLine(new Vector3(p.x, 0f, p.y) - ((Vector3) (Vector3.right * size)), new Vector3(p.x, 0f, p.y) + ((Vector3) (Vector3.right * size)), col);
            Debug.DrawLine(new Vector3(p.x, 0f, p.y) - ((Vector3) (Vector3.forward * size)), new Vector3(p.x, 0f, p.y) + ((Vector3) (Vector3.forward * size)), col);
        }

        private static void DrawVO(Vector2 circleCenter, float radius, Vector2 origin)
        {
            Vector2 vector5 = origin - circleCenter;
            Vector2 vector6 = origin - circleCenter;
            float num = Mathf.Atan2(vector5.y, vector6.x);
            Vector2 vector7 = origin - circleCenter;
            float f = radius / vector7.magnitude;
            float num3 = (f > 1f) ? 0f : Mathf.Abs(Mathf.Acos(f));
            DrawCircle(circleCenter, radius, num - num3, num + num3, Color.black);
            Vector2 p = (Vector2) (new Vector2(Mathf.Cos(num - num3), Mathf.Sin(num - num3)) * radius);
            Vector2 vector2 = (Vector2) (new Vector2(Mathf.Cos(num + num3), Mathf.Sin(num + num3)) * radius);
            Vector2 vector3 = -new Vector2(-p.y, p.x);
            Vector2 vector4 = new Vector2(-vector2.y, vector2.x);
            p += circleCenter;
            vector2 += circleCenter;
            Debug.DrawRay(To3D(p), (Vector3) (To3D(vector3).normalized * 100f), Color.black);
            Debug.DrawRay(To3D(vector2), (Vector3) (To3D(vector4).normalized * 100f), Color.black);
        }

        public VInt3 FixVelocity(VInt2 result)
        {
            int sqrMagnitude = this.desiredVelocity.xz.sqrMagnitude;
            int num2 = 0;
            if (sqrMagnitude == 0)
            {
                num2 = this.maxSpeed.i / 2;
            }
            else
            {
                num2 = (IntMath.Sqrt((long) sqrMagnitude) * 3) / 2;
            }
            VInt2 num3 = result;
            long sqrMagnitudeLong = result.sqrMagnitudeLong;
            if (sqrMagnitudeLong > (num2 * num2))
            {
                num3 = IntMath.Divide(result, (long) num2, (long) IntMath.Sqrt(sqrMagnitudeLong));
            }
            return new VInt3(num3.x, 0, num3.y);
        }

        private void insertAgentNeighbor(Agent agent, ref float rangeSq)
        {
        }

        public long InsertAgentNeighbour(Agent agent, long rangeSq)
        {
            if (this != agent)
            {
                if ((agent.layer & this.collidesWith) == 0)
                {
                    return rangeSq;
                }
                long item = agent.position.XZSqrMagnitude(ref this.position);
                if (item >= rangeSq)
                {
                    return rangeSq;
                }
                if (this.neighbours.Count < this.maxNeighbours)
                {
                    this.neighbours.Add(agent);
                    this.neighbourDists.Add(item);
                }
                int num2 = this.neighbours.Count - 1;
                if (item < this.neighbourDists[num2])
                {
                    while ((num2 != 0) && (item < this.neighbourDists[num2 - 1]))
                    {
                        this.neighbours[num2] = this.neighbours[num2 - 1];
                        this.neighbourDists[num2] = this.neighbourDists[num2 - 1];
                        num2--;
                    }
                    this.neighbours[num2] = agent;
                    this.neighbourDists[num2] = item;
                }
                if (this.neighbours.Count == this.maxNeighbours)
                {
                    rangeSq = this.neighbourDists[this.neighbourDists.Count - 1];
                }
            }
            return rangeSq;
        }

        public void InsertNeighbour(Agent agent, long distSq)
        {
            if (this.neighbours.Count > 0)
            {
                if (this.neighbours.Count == this.maxNeighbours)
                {
                    int num = this.neighbours.Count - 1;
                    if (this.neighbourDists[num] <= distSq)
                    {
                        return;
                    }
                    this.neighbours[num] = agent;
                    this.neighbourDists[num] = distSq;
                }
                else
                {
                    this.neighbours.Add(agent);
                    this.neighbourDists.Add(distSq);
                }
                int num2 = this.neighbours.Count - 1;
                while ((num2 != 0) && (distSq < this.neighbourDists[num2 - 1]))
                {
                    this.neighbours[num2] = this.neighbours[num2 - 1];
                    this.neighbourDists[num2] = this.neighbourDists[num2 - 1];
                    num2--;
                }
                this.neighbours[num2] = agent;
                this.neighbourDists[num2] = distSq;
            }
            else
            {
                this.neighbours.Add(agent);
                this.neighbourDists.Add(distSq);
            }
        }

        public void InsertObstacleNeighbour(ObstacleVertex ob1, long rangeSq)
        {
            ObstacleVertex next = ob1.next;
            long num = AstarMath.DistancePointSegmentStrict(ob1.position, next.position, this.Position);
            if (num < rangeSq)
            {
                this.obstacles.Add(ob1);
                this.obstacleDists.Add((float) num);
                int num2 = this.obstacles.Count - 1;
                while ((num2 != 0) && (num < this.obstacleDists[num2 - 1]))
                {
                    this.obstacles[num2] = this.obstacles[num2 - 1];
                    this.obstacleDists[num2] = this.obstacleDists[num2 - 1];
                    num2--;
                }
                this.obstacles[num2] = ob1;
                this.obstacleDists[num2] = num;
            }
        }

        public void Interpolate()
        {
            this.smoothPos = this.Position;
        }

        public void Interpolate(float t)
        {
            if (t == 1f)
            {
                this.smoothPos = this.Position;
            }
            else
            {
                this.smoothPos = this.prevSmoothPos + ((VInt3) ((this.Position - this.prevSmoothPos) * t));
            }
        }

        public static bool IntersectionFactor(VInt2 start1, VInt2 dir1, VInt2 start2, VInt2 dir2, out VFactor factor)
        {
            long x = dir2.x;
            long y = dir2.y;
            long num3 = (y * dir1.x) - (x * dir1.y);
            if (num3 == 0)
            {
                factor = VFactor.zero;
                return false;
            }
            long num4 = (x * (start1.y - start2.y)) - (y * (start1.x - start2.x));
            factor.nom = num4;
            factor.den = num3;
            return true;
        }

        private static bool linearProgram1(List<VLine> lines, int lineNo, int radius, VInt2 optVelocity, bool directionOpt, ref VInt2 result)
        {
            VLine line = lines[lineNo];
            long num = IntMath.Divide(VInt2.DotLong(ref line.point, ref line.direction), 0x3e8L);
            long a = ((num * num) + (radius * radius)) - line.point.sqrMagnitude;
            if (a < 0L)
            {
                return false;
            }
            long num3 = IntMath.SqrtLong(a);
            VFactor factor = new VFactor(-num - num3, 1L);
            VFactor factor2 = new VFactor(-num + num3, 1L);
            VFactor factor3 = new VFactor();
            for (int i = 0; i < lineNo; i++)
            {
                VLine line2 = lines[i];
                factor3.den = VInt2.DetLong(ref line.direction, ref line2.direction);
                factor3.nom = VInt2.DetLong(line2.direction, line.point - line2.point);
                if (factor3.den == 0)
                {
                    if (factor3.nom < 0L)
                    {
                        return false;
                    }
                }
                else
                {
                    if (factor3.den > 0L)
                    {
                        factor2 = (factor2 <= factor3) ? factor3 : factor2;
                    }
                    else
                    {
                        factor = (factor <= factor3) ? factor3 : factor;
                    }
                    if (factor > factor2)
                    {
                        return false;
                    }
                }
            }
            if (directionOpt)
            {
                if (VInt2.DotLong(ref optVelocity, ref line.direction) > 0L)
                {
                    result = line.point + IntMath.Divide(line.direction * factor2, 0x3e8L);
                }
                else
                {
                    result = line.point + IntMath.Divide(line.direction * factor, 0x3e8L);
                }
            }
            else
            {
                VFactor factor4 = new VFactor(VInt2.DotLong(line.direction, optVelocity - line.point), 0x3e8L);
                if (factor > factor4)
                {
                    result = line.point + IntMath.Divide(line.direction * factor, 0x3e8L);
                }
                else if (factor2 < factor4)
                {
                    result = line.point + IntMath.Divide(line.direction * factor2, 0x3e8L);
                }
                else
                {
                    result = line.point;
                    result.x += (int) IntMath.Divide((long) (line.direction.x * factor4.nom), (long) (factor4.den * 0x3e8L));
                    result.y += (int) IntMath.Divide((long) (line.direction.y * factor4.nom), (long) (factor4.den * 0x3e8L));
                }
            }
            return true;
        }

        private static int linearProgram2(List<VLine> lines, int radius, VInt2 optVelocity, bool directionOpt, ref VInt2 result)
        {
            if (directionOpt)
            {
                result = IntMath.Divide(optVelocity, (long) radius, 0x3e8L);
            }
            else if (optVelocity.sqrMagnitudeLong > (radius * radius))
            {
                result = IntMath.Divide(optVelocity.normalized, (long) radius, 0x3e8L);
            }
            else
            {
                result = optVelocity;
            }
            for (int i = 0; i < lines.Count; i++)
            {
                VLine line = lines[i];
                if (VInt2.DetLong(line.direction, line.point - result) > 0L)
                {
                    VInt2 num2 = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
                    {
                        result = num2;
                        return i;
                    }
                }
            }
            return lines.Count;
        }

        private static void linearProgram3(List<VLine> lines, int numObstLines, int beginLine, int radius, ref VInt2 result)
        {
            long num = 0L;
            for (int i = beginLine; i < lines.Count; i++)
            {
                VLine line = lines[i];
                if (VInt2.DetLong(line.direction, line.point - result) > num)
                {
                    projLines.Clear();
                    for (int j = 0; j < numObstLines; j++)
                    {
                        projLines.Add(lines[j]);
                    }
                    for (int k = numObstLines; k < i; k++)
                    {
                        VLine line2;
                        VLine line3 = lines[k];
                        long b = VInt2.DetLong(line.direction, line3.direction);
                        if (b == 0)
                        {
                            if (VInt2.DetLong(line.direction, line3.direction) > 0L)
                            {
                                continue;
                            }
                            line2.point = line.point + line3.point;
                            line2.point.x /= 2;
                            line2.point.y /= 2;
                        }
                        else
                        {
                            VInt2 num6 = IntMath.Divide(line.direction, VInt2.DetLong(line3.direction, line.point - line3.point), b);
                            line2.point = line.point + num6;
                        }
                        VInt2 num8 = line3.direction - line.direction;
                        line2.direction = num8.normalized;
                        projLines.Add(line2);
                    }
                    VInt2 num7 = result;
                    if (linearProgram2(projLines, radius, new VInt2(-line.direction.y, line.direction.x), true, ref result) < projLines.Count)
                    {
                        result = num7;
                    }
                    projLines.Clear();
                    num = VInt2.DetLong(line.direction, line.point - result);
                }
            }
        }

        private static Color Rainbow(float v)
        {
            Color color = new Color(v, 0f, 0f);
            if (color.r > 1f)
            {
                color.g = color.r - 1f;
                color.r = 1f;
            }
            if (color.g > 1f)
            {
                color.b = color.g - 1f;
                color.g = 1f;
            }
            return color;
        }

        public void SetYPosition(VInt yCoordinate)
        {
            this.Position = new VInt3(this.Position.x, yCoordinate.i, this.Position.z);
            this.smoothPos.y = yCoordinate.i;
            this.prevSmoothPos.y = yCoordinate.i;
        }

        public void Teleport(VInt3 pos)
        {
            this.Position = pos;
            this.smoothPos = pos;
            this.prevSmoothPos = pos;
        }

        private static Vector3 To3D(Vector2 p)
        {
            return new Vector3(p.x, 0f, p.y);
        }

        private VInt2 Trace(VO[] vos, int voCount, VInt2 p, VFactor cutoff, out float score)
        {
            score = 0f;
            float stepScale = this.simulator.stepScale;
            float positiveInfinity = float.PositiveInfinity;
            VInt2 num3 = p;
            for (int i = 0; i < 50; i++)
            {
                float num5 = 1f - (((float) i) / 50f);
                num5 *= stepScale;
                VInt2 zero = VInt2.zero;
                VFactor factor = VFactor.zero;
                for (int j = 0; j < voCount; j++)
                {
                    VFactor factor2;
                    VInt2 num8 = vos[j].Sample(p, out factor2);
                    zero += num8;
                    if (factor2 > factor)
                    {
                        factor = factor2;
                    }
                }
                VInt2 a = this.desiredVelocity.xz - p;
                VFactor factor3 = new VFactor((long) a.magnitude, 500L);
                zero += IntMath.Divide(a, 10L);
                factor = (factor <= factor3) ? factor3 : factor;
                score = factor.single;
                if (score < positiveInfinity)
                {
                    positiveInfinity = score;
                }
                num3 = p;
                if ((score <= cutoff.single) && (i > 10))
                {
                    break;
                }
                int sqrMagnitude = zero.sqrMagnitude;
                if (sqrMagnitude > 0)
                {
                    VFactor factor4 = factor;
                    factor4.den *= IntMath.Sqrt((long) sqrMagnitude);
                    zero *= factor4;
                }
                zero = (VInt2) (((Vector2) zero) * num5);
                VInt2 num11 = p + zero;
                if (this.DebugDraw)
                {
                    Debug.DrawLine(To3D((Vector2) num11) + ((Vector3) this.position), To3D((Vector2) p) + ((Vector3) this.position), Rainbow(0.1f / score) * new Color(1f, 1f, 1f, 0.2f));
                }
            }
            score = positiveInfinity;
            return num3;
        }

        public void Update()
        {
            this.velocity = this.newVelocity;
            this.prevSmoothPos = this.smoothPos;
            this.position = this.prevSmoothPos;
            this.position += IntMath.Divide(this.velocity, (long) this.simulator.DeltaTimeMS, 0x3e8L);
            this.Position = this.position;
        }

        public int AgentTimeHorizon { get; set; }

        public RVOLayer CollidesWith { get; set; }

        public bool DebugDraw { get; set; }

        public VInt3 DesiredVelocity { get; set; }

        public VInt Height { get; set; }

        public VInt3 InterpolatedPosition
        {
            get
            {
                return this.smoothPos;
            }
        }

        public RVOLayer Layer { get; set; }

        public bool Locked { get; set; }

        public int MaxNeighbours { get; set; }

        public VInt MaxSpeed { get; set; }

        public VInt NeighbourDist { get; set; }

        public List<ObstacleVertex> NeighbourObstacles
        {
            get
            {
                return null;
            }
        }

        public int ObstacleTimeHorizon { get; set; }

        public VInt3 Position { get; private set; }

        public VInt Radius { get; set; }

        public VInt3 Velocity { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        public struct VO
        {
            public VInt2 origin;
            public VInt2 center;
            private VInt2 line1;
            private VInt2 line2;
            private VInt2 dir1;
            private VInt2 dir2;
            private VInt2 cutoffLine;
            private VInt2 cutoffDir;
            private long sqrCutoffDistance;
            private bool leftSide;
            private bool colliding;
            private int radius;
            private VFactor weightFactor;
            public VO(VInt2 offset, VInt2 p0, VInt2 dir, VFactor weightFactor)
            {
                this.colliding = true;
                this.line1 = p0;
                this.dir1 = -dir;
                this.origin = VInt2.zero;
                this.center = VInt2.zero;
                this.line2 = VInt2.zero;
                this.dir2 = VInt2.zero;
                this.cutoffLine = VInt2.zero;
                this.cutoffDir = VInt2.zero;
                this.sqrCutoffDistance = 0L;
                this.leftSide = false;
                this.radius = 0;
                this.weightFactor.nom = weightFactor.nom;
                this.weightFactor.den = weightFactor.den << 1;
            }

            public VO(VInt2 offset, VInt2 p1, VInt2 p2, VInt2 tang1, VInt2 tang2, VFactor weightFactor)
            {
                this.weightFactor.nom = weightFactor.nom;
                this.weightFactor.den = weightFactor.den << 1;
                this.colliding = false;
                this.cutoffLine = p1;
                VInt2 num = p2 - p1;
                this.cutoffDir = num.normalized;
                this.line1 = p1;
                this.dir1 = tang1;
                this.line2 = p2;
                this.dir2 = tang2;
                this.dir2 = -this.dir2;
                this.cutoffDir = -this.cutoffDir;
                this.origin = VInt2.zero;
                this.center = VInt2.zero;
                this.sqrCutoffDistance = 0L;
                this.leftSide = false;
                this.radius = 0;
                weightFactor.nom = 5L;
                weightFactor.den = 1L;
            }

            public VO(VInt2 center, VInt2 offset, int radius, VInt2 sideChooser, VFactor inverseDt, VFactor weightFactor)
            {
                this.weightFactor.nom = weightFactor.nom;
                this.weightFactor.den = weightFactor.den << 1;
                this.origin = offset;
                weightFactor.nom = 1L;
                weightFactor.den = 2L;
                long sqrMagnitudeLong = center.sqrMagnitudeLong;
                int b = IntMath.Sqrt(sqrMagnitudeLong);
                VInt2 a = center;
                if (b > 0)
                {
                    a.x = IntMath.Divide((int) (a.x * 0x3e8), b);
                    a.y = IntMath.Divide((int) (a.y * 0x3e8), b);
                }
                long num5 = radius;
                num5 *= num5;
                if (sqrMagnitudeLong < num5)
                {
                    this.colliding = true;
                    this.leftSide = false;
                    this.line1 = IntMath.Divide(a, (long) (b - radius), 0x3e8L);
                    VInt2 num6 = new VInt2(this.line1.y, -this.line1.x);
                    this.dir1 = num6.normalized;
                    this.line1 += offset;
                    this.cutoffDir = VInt2.zero;
                    this.cutoffLine = VInt2.zero;
                    this.sqrCutoffDistance = 0L;
                    this.dir2 = VInt2.zero;
                    this.line2 = VInt2.zero;
                    this.center = VInt2.zero;
                    this.radius = 0;
                }
                else
                {
                    VFactor factor3;
                    VFactor factor4;
                    this.colliding = false;
                    center *= inverseDt;
                    radius *= inverseDt;
                    b = center.magnitude;
                    VInt2 num = center + offset;
                    this.sqrCutoffDistance = b - radius;
                    this.center = center;
                    this.cutoffLine = IntMath.Divide(a, this.sqrCutoffDistance, 0x3e8L);
                    VInt2 num7 = new VInt2(-this.cutoffLine.y, this.cutoffLine.x);
                    this.cutoffDir = num7.normalized;
                    this.cutoffLine += offset;
                    this.sqrCutoffDistance *= this.sqrCutoffDistance;
                    VFactor factor = IntMath.atan2(-center.y, -center.x);
                    VFactor factor2 = IntMath.acos((long) radius, (long) b);
                    this.radius = radius;
                    this.leftSide = Polygon.Left(VInt2.zero, center, sideChooser);
                    IntMath.sincos(out factor3, out factor4, factor + factor2);
                    this.line1 = new VInt2(radius * factor4, radius * factor3);
                    VInt2 num8 = new VInt2(this.line1.y, -this.line1.x);
                    this.dir1 = num8.normalized;
                    IntMath.sincos(out factor3, out factor4, factor - factor2);
                    this.line2 = new VInt2(radius * factor4, radius * factor3);
                    VInt2 num9 = new VInt2(this.line2.y, -this.line2.x);
                    this.dir2 = num9.normalized;
                    this.line1 += num;
                    this.line2 += num;
                }
            }

            public static bool Left(VInt2 a, VInt2 dir, VInt2 p)
            {
                return (((dir.x * (p.y - a.y)) - ((p.x - a.x) * dir.y)) <= 0);
            }

            public static long Det(VInt2 a, VInt2 dir, VInt2 p)
            {
                return (((p.x - a.x) * dir.y) - (dir.x * (p.y - a.y)));
            }

            public VInt2 Sample(VInt2 p, out VFactor weight)
            {
                if (this.colliding)
                {
                    long num = Det(this.line1, this.dir1, p);
                    if (num >= 0L)
                    {
                        weight.nom = this.weightFactor.nom * num;
                        weight.den = this.weightFactor.den * 0x3e8L;
                        return IntMath.Divide(new VInt2(-this.dir1.y, this.dir1.x), weight.nom * Agent.GlobalIncompressibility, weight.den);
                    }
                    weight = VFactor.zero;
                    return VInt2.zero;
                }
                long num2 = Det(this.cutoffLine, this.cutoffDir, p);
                if (num2 <= 0L)
                {
                    weight = VFactor.zero;
                    return VInt2.zero;
                }
                long num3 = Det(this.line1, this.dir1, p);
                long num4 = Det(this.line2, this.dir2, p);
                if ((num3 >= 0L) && (num4 >= 0L))
                {
                    if (this.leftSide)
                    {
                        if (num2 < (this.radius * 0x3e8))
                        {
                            weight.nom = this.weightFactor.nom * num2;
                            weight.den = this.weightFactor.den * 0x3e8L;
                            return (new VInt2(-this.cutoffDir.y, this.cutoffDir.x) * weight);
                        }
                        weight.nom = this.weightFactor.nom * num3;
                        weight.den = this.weightFactor.den * 0x3e8L;
                        return (new VInt2(-this.dir1.y, this.dir1.x) * weight);
                    }
                    if (num2 < (this.radius * 0x3e8))
                    {
                        weight.nom = this.weightFactor.nom * num2;
                        weight.den = this.weightFactor.den * 0x3e8L;
                        return (new VInt2(-this.cutoffDir.y, this.cutoffDir.x) * weight);
                    }
                    weight.nom = this.weightFactor.nom * num4;
                    weight.den = this.weightFactor.den * 0x3e8L;
                    return (new VInt2(-this.dir2.y, this.dir2.x) * weight);
                }
                weight = VFactor.zero;
                return VInt2.zero;
            }

            public VFactor ScalarSample(VInt2 p)
            {
                if (this.colliding)
                {
                    long num = Det(this.line1, this.dir1, p);
                    if (num >= 0L)
                    {
                        return new VFactor((num * Agent.GlobalIncompressibility) * this.weightFactor.nom, this.weightFactor.den * 0x3e8L);
                    }
                    return VFactor.zero;
                }
                long num2 = Det(this.cutoffLine, this.cutoffDir, p);
                if (num2 <= 0L)
                {
                    return VFactor.zero;
                }
                long num3 = Det(this.line1, this.dir1, p);
                long num4 = Det(this.line2, this.dir2, p);
                if ((num3 < 0L) || (num4 < 0L))
                {
                    return VFactor.zero;
                }
                if (this.leftSide)
                {
                    if (num2 < (this.radius * 0x3e8))
                    {
                        return new VFactor(num2 * this.weightFactor.nom, this.weightFactor.den * 0x3e8L);
                    }
                    return new VFactor(num3 * this.weightFactor.nom, this.weightFactor.den * 0x3e8L);
                }
                if (num2 < this.radius)
                {
                    return new VFactor(num2 * this.weightFactor.nom, this.weightFactor.den * 0x3e8L);
                }
                return new VFactor(num4 * this.weightFactor.nom, this.weightFactor.den * 0x3e8L);
            }
        }
    }
}

