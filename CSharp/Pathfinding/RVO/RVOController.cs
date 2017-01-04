namespace Pathfinding.RVO
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Pathfinding;
    using Pathfinding.RVO.Sampled;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Local Avoidance/RVO Controller")]
    public class RVOController : MonoBehaviour, IPooledMonoBehaviour
    {
        private PoolObjHandle<ActorRoot> actor;
        private VInt adjustedY = 0;
        [Tooltip("How far in the time to look for collisions with other agents")]
        public int agentTimeHorizon = 0x7d0;
        [Tooltip("Center of the agent relative to the pivot point of this game object")]
        public VInt3 center = new VInt3(0, 0x3e8, 0);
        [HideInInspector]
        public bool checkNavNode;
        [AstarEnumFlag]
        public RVOLayer collidesWith = -1;
        public bool debug;
        private VInt3 desiredVelocity;
        public bool enableRotation;
        private static readonly Color GizmoColor = new Color(0.9411765f, 0.8352941f, 0.1176471f);
        [Tooltip("Height of the agent. In world units")]
        public VInt height = 0x7d0;
        private VInt3 lastPosition;
        public RVOLayer layer = RVOLayer.DefaultAgent;
        [Tooltip("A locked unit cannot move. Other units will still avoid it. But avoidance quailty is not the best")]
        public bool locked;
        [Tooltip("Automatically set #locked to true when desired velocity is approximately zero")]
        public bool lockWhenNotMoving;
        [Tooltip("Layer mask for the ground. The RVOController will raycast down to check for the ground to figure out where to place the agent")]
        public LayerMask mask = -1;
        [Tooltip("Max number of other agents to take into account.\nA smaller value can reduce CPU load, a higher value can lead to better local avoidance quality.")]
        public int maxNeighbours = 6;
        [Tooltip("Max speed of the agent. In world units/second")]
        public VInt maxSpeed = 0x2710;
        [Tooltip("Maximum distance to other agents to take them into account for collisions.\nDecreasing this value can lead to better performance, increasing it can lead to better quality of the simulation")]
        public VInt neighbourDist = 0x7d0;
        [HideInInspector]
        public int obstacleTimeHorizon = 0x7d0;
        [Tooltip("Radius of the agent")]
        public VInt radius = 400;
        public float rotationSpeed = 30f;
        private Simulator simulator;
        [HideInInspector]
        public float wallAvoidFalloff = 1f;
        [HideInInspector]
        public float wallAvoidForce = 1f;

        public void DoUpdate(float dt)
        {
            if (this.rvoAgent != null)
            {
                ActorRoot handle = this.actor.handle;
                if (this.lastPosition != handle.location)
                {
                    this.Teleport(handle.location);
                }
                if (this.lockWhenNotMoving)
                {
                    this.locked = this.desiredVelocity == VInt3.zero;
                }
                this.UpdateAgentProperties();
                VInt3 interpolatedPosition = this.rvoAgent.InterpolatedPosition;
                this.rvoAgent.SetYPosition(this.adjustedY);
                this.rvoAgent.DesiredVelocity = this.desiredVelocity;
                VInt3 num2 = interpolatedPosition - this.center;
                num2.y += this.height.i >> 1;
                if (this.checkNavNode)
                {
                    VInt num3;
                    VInt3 delta = num2 - handle.location;
                    VInt3 num5 = PathfindingUtility.Move((ActorRoot) this.actor, delta, out num3, out handle.hasReachedNavEdge, null);
                    VInt3 pos = handle.location + num5;
                    handle.location = pos;
                    handle.groundY = num3;
                    this.rvoAgent.Teleport(pos);
                    this.adjustedY = pos.y;
                }
                else
                {
                    handle.location = num2;
                }
                this.lastPosition = handle.location;
                if (this.enableRotation && (this.velocity != VInt3.zero))
                {
                    Vector3 velocity = (Vector3) this.velocity;
                    Transform transform = base.transform;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), (dt * this.rotationSpeed) * Mathf.Min((float) this.velocity.magnitude, 0.2f));
                }
            }
        }

        public void EnsureActorAndSimulator()
        {
            if (this.actor == 0)
            {
                this.actor = ActorHelper.GetActorRoot(base.gameObject);
            }
            if (this.simulator == null)
            {
                RVOSimulator instance = RVOSimulator.GetInstance();
                if (instance == null)
                {
                    return;
                }
                this.simulator = instance.GetSimulator();
            }
            if (this.simulator != null)
            {
                VInt3 location;
                if (this.actor != 0)
                {
                    location = this.actor.handle.location;
                }
                else
                {
                    location = (VInt3) base.transform.position;
                }
                if (this.rvoAgent != null)
                {
                    if (!this.simulator.GetAgents().Contains(this.rvoAgent))
                    {
                        this.simulator.AddAgent(this.rvoAgent);
                    }
                }
                else
                {
                    this.rvoAgent = this.simulator.AddAgent(location);
                }
                if (this.rvoAgent != null)
                {
                    this.rvoAgent.owner = base.gameObject;
                }
                this.UpdateAgentProperties();
                this.rvoAgent.Teleport(location);
                this.adjustedY = this.rvoAgent.Position.y;
            }
        }

        public List<GameObject> GetNeighbours(bool colliding)
        {
            if (!base.enabled || (this.rvoAgent == null))
            {
                return null;
            }
            Agent rvoAgent = this.rvoAgent;
            if (rvoAgent.neighbours.Count == 0)
            {
                return null;
            }
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < rvoAgent.neighbours.Count; i++)
            {
                Agent agent2 = rvoAgent.neighbours[i];
                if ((agent2 != rvoAgent) && (agent2.owner != null))
                {
                    if (colliding)
                    {
                        int num2 = Math.Min((int) (rvoAgent.position.y + rvoAgent.height.i), (int) (agent2.position.y + agent2.height.i));
                        int num3 = Math.Max(rvoAgent.position.y, agent2.position.y);
                        if ((num2 - num3) >= 0)
                        {
                            VInt3 num4 = agent2.position - rvoAgent.position;
                            num4.y = 0;
                            long num5 = rvoAgent.radius.i + agent2.radius.i;
                            num5 *= num5;
                            if (num4.sqrMagnitudeLong < num5)
                            {
                                list.Add(agent2.owner as GameObject);
                            }
                        }
                    }
                    else
                    {
                        list.Add(agent2.owner as GameObject);
                    }
                }
            }
            return list;
        }

        public void Move(VInt3 vel)
        {
            this.desiredVelocity = vel;
        }

        public void OnCreate()
        {
        }

        public void OnDestroy()
        {
            if (this.rvoAgent != null)
            {
                this.rvoAgent.owner = null;
                this.rvoAgent = null;
            }
        }

        public void OnDisable()
        {
            if (this.simulator != null)
            {
                this.simulator.SafeRemoveAgent(this.rvoAgent);
            }
        }

        public void OnDrawGizmos()
        {
            float height = (float) this.height;
            float radius = (float) this.radius;
            Vector3 center = (Vector3) this.center;
            Gizmos.color = GizmoColor;
            Gizmos.DrawWireSphere((Vector3) (((base.transform.position + center) - ((Vector3.up * height) * 0.5f)) + ((Vector3.up * radius) * 0.5f)), radius);
            Gizmos.DrawLine((base.transform.position + center) - ((Vector3) ((Vector3.up * height) * 0.5f)), (base.transform.position + center) + ((Vector3) ((Vector3.up * height) * 0.5f)));
            Gizmos.DrawWireSphere((Vector3) (((base.transform.position + center) + ((Vector3.up * height) * 0.5f)) - ((Vector3.up * radius) * 0.5f)), radius);
        }

        public void OnEnable()
        {
            this.EnsureActorAndSimulator();
            if (this.rvoAgent != null)
            {
                this.rvoAgent.desiredVelocity = VInt3.zero;
                this.rvoAgent.DesiredVelocity = VInt3.zero;
                this.rvoAgent.newVelocity = VInt3.zero;
            }
        }

        public void OnGet()
        {
            this.radius = 400;
            this.maxSpeed = 0x2710;
            this.height = 0x7d0;
            this.locked = false;
            this.lockWhenNotMoving = false;
            this.agentTimeHorizon = 0x7d0;
            this.obstacleTimeHorizon = 0x7d0;
            this.neighbourDist = 0x7d0;
            this.maxNeighbours = 6;
            this.mask = -1;
            this.layer = RVOLayer.DefaultAgent;
            this.collidesWith = -1;
            this.wallAvoidForce = 1f;
            this.wallAvoidFalloff = 1f;
            this.center = new VInt3(0, 0x3e8, 0);
            this.enableRotation = false;
            this.rotationSpeed = 30f;
            this.simulator = null;
            this.adjustedY = 0;
            this.actor.Release();
            this.desiredVelocity = VInt3.zero;
            this.checkNavNode = false;
            this.lastPosition = VInt3.zero;
        }

        public void OnRecycle()
        {
            if (this.rvoAgent != null)
            {
                this.rvoAgent.owner = null;
                if (this.simulator != null)
                {
                    this.simulator.SafeRemoveAgent(this.rvoAgent);
                }
            }
        }

        public void Teleport(VInt3 pos)
        {
            this.actor.handle.location = pos;
            this.lastPosition = pos;
            this.rvoAgent.Teleport(pos);
            this.adjustedY = pos.y;
        }

        public void Update()
        {
            if (!RVOSimulator.IsFrameMode)
            {
                this.DoUpdate(Time.deltaTime);
            }
        }

        protected void UpdateAgentProperties()
        {
            this.rvoAgent.Radius = this.radius;
            this.rvoAgent.MaxSpeed = this.maxSpeed;
            this.rvoAgent.Height = this.height;
            this.rvoAgent.AgentTimeHorizon = this.agentTimeHorizon;
            this.rvoAgent.ObstacleTimeHorizon = this.obstacleTimeHorizon;
            this.rvoAgent.Locked = this.locked;
            this.rvoAgent.MaxNeighbours = this.maxNeighbours;
            this.rvoAgent.DebugDraw = this.debug;
            this.rvoAgent.NeighbourDist = this.neighbourDist;
            this.rvoAgent.Layer = this.layer;
            this.rvoAgent.CollidesWith = this.collidesWith;
        }

        public void UpdateLogic(int dt)
        {
            if (RVOSimulator.IsFrameMode)
            {
                this.DoUpdate(dt * 0.001f);
            }
        }

        public VInt3 position
        {
            get
            {
                return this.rvoAgent.InterpolatedPosition;
            }
        }

        public Agent rvoAgent { get; private set; }

        public VInt3 velocity
        {
            get
            {
                return this.rvoAgent.Velocity;
            }
        }
    }
}

