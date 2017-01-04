namespace Pathfinding.RVO
{
    using Pathfinding.RVO.Sampled;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Local Avoidance/RVO Simulator")]
    public class RVOSimulator : MonoBehaviour
    {
        [Tooltip("Desired FPS for rvo simulation. It is usually not necessary to run a crowd simulation at a very high fps.\nUsually 10-30 fps is enough, but can be increased for better quality.\nThe rvo simulation will never run at a higher fps than the game")]
        public int desiredSimulationFPS = 20;
        [Tooltip("[unitless][0...infinity] How much an agent will try to reach the desired velocity. A higher value will yield a more aggressive behaviour")]
        public float desiredVelocityWeight = 0.1f;
        [Tooltip("Calculate local avoidance in between frames.\nThis can increase jitter in the agents' movement so use it only if you really need the performance boost. It will also reduce the responsiveness of the agents to the commands you send to them.")]
        public bool doubleBuffering;
        [Tooltip("[0...infinity] Higher values will raise the penalty for agent-agent intersection")]
        public int incompressibility = 30;
        [Tooltip("Interpolate positions between simulation timesteps")]
        public bool interpolation = true;
        public static bool IsFrameMode = true;
        [Tooltip("Run multiple simulation steps per step. Much slower, but may lead to slightly higher quality local avoidance.")]
        public bool oversampling;
        [NonSerialized, HideInInspector, Tooltip("[GradientDecent][unitless][0...1] A higher value will result in lower quality local avoidance but faster calculations.")]
        public VFactor QualityCutoff = new VFactor(1L, 20L);
        private Simulator.SamplingAlgorithm samplingAlgorithm;
        private Simulator simulator;
        [Tooltip("[GradientDecent][unitless][0...2] How large steps to take when searching for a minimum to the penalty function. Larger values will make it faster, but less accurate, too low values (near 0) can also give large inaccuracies. Values around 0.5-1.5 work the best.")]
        public float stepScale = 1.5f;
        [NonSerialized, HideInInspector, Tooltip("Thickness of RVO obstacle walls.\nIf obstacles are passing through obstacles, try a larger value, if they are getting stuck near small obstacles, try reducing it")]
        public VFactor WallThickness = VFactor.one;
        [Tooltip("Number of RVO worker threads. If set to None, no multithreading will be used.")]
        public ThreadCount workerThreads = ThreadCount.Two;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Instance.enabled = false;
                Instance = this;
            }
            if (this.desiredSimulationFPS < 1)
            {
                this.desiredSimulationFPS = 1;
            }
            if (this.simulator == null)
            {
                int workers = AstarPath.CalculateThreadCount(this.workerThreads);
                this.simulator = new Simulator(workers, this.doubleBuffering);
                this.simulator.Interpolation = this.interpolation;
            }
        }

        public static RVOSimulator GetInstance()
        {
            if (Instance == null)
            {
                RVOSimulator simulator = UnityEngine.Object.FindObjectOfType<RVOSimulator>();
                if (simulator != null)
                {
                    simulator.Awake();
                }
            }
            return Instance;
        }

        public Simulator GetSimulator()
        {
            if (this.simulator == null)
            {
                this.Awake();
            }
            return this.simulator;
        }

        private void OnDestroy()
        {
            this.simulator.OnDestroy();
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void UpdateLogic(int nDelta)
        {
            if (IsFrameMode && base.enabled)
            {
                if (this.desiredSimulationFPS < 1)
                {
                    this.desiredSimulationFPS = 1;
                }
                Agent.DesiredVelocityWeight = this.desiredVelocityWeight;
                Agent.GlobalIncompressibility = this.incompressibility;
                Simulator simulator = this.GetSimulator();
                simulator.Interpolation = false;
                simulator.stepScale = this.stepScale;
                simulator.qualityCutoff = this.QualityCutoff;
                simulator.algorithm = this.samplingAlgorithm;
                simulator.Oversampling = this.oversampling;
                simulator.WallThickness = this.WallThickness;
                simulator.UpdateLogic(nDelta);
            }
        }

        public static RVOSimulator Instance
        {
            [CompilerGenerated]
            get
            {
                return <Instance>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <Instance>k__BackingField = value;
            }
        }
    }
}

