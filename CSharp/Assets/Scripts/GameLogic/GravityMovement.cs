namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class GravityMovement
    {
        private AccelerateMotionControler gravityControler = new AccelerateMotionControler();
        public const int MAX_MOTION_COUNT = 3;
        private ListView<SpecialMotionControler> motionControlers = new ListView<SpecialMotionControler>();
        private PlayerMovement Movement;

        public GravityMovement(PlayerMovement _movement)
        {
            this.Movement = _movement;
            this.gravityControler.InitMotionControler(0, -98);
        }

        public void AddMotionControler(SpecialMotionControler _controler)
        {
            if (this.motionControlers.Count <= 3)
            {
                this.motionControlers.Add(_controler);
            }
        }

        public int GetMotionControlerCount()
        {
            return this.motionControlers.Count;
        }

        public void GravityMoveLerp(int _deltaTime, bool bReset)
        {
            int motionLerpDistance = 0;
            Vector3 zero = Vector3.zero;
            SpecialMotionControler controler = null;
            if (this.Movement != null)
            {
                VInt num2;
                if (this.motionControlers.Count != 0)
                {
                    this.Movement.isLerpFlying = true;
                    zero = this.Movement.actor.gameObject.transform.position;
                    PathfindingUtility.GetGroundY((VInt3) zero, out num2);
                    for (int i = 0; i < this.motionControlers.Count; i++)
                    {
                        controler = this.motionControlers[i];
                        motionLerpDistance += controler.GetMotionLerpDistance(_deltaTime);
                    }
                    zero.y += ((float) motionLerpDistance) / 1000f;
                    if (((float) num2) > zero.y)
                    {
                        zero.y = (float) num2;
                        this.Movement.actor.gameObject.transform.position = zero;
                    }
                    else
                    {
                        this.Movement.actor.gameObject.transform.position = zero;
                    }
                }
                else if (this.Movement.isLerpFlying)
                {
                    zero = this.Movement.actor.gameObject.transform.position;
                    PathfindingUtility.GetGroundY((VInt3) zero, out num2);
                    if (((float) num2) >= zero.y)
                    {
                        zero.y = (float) num2;
                        this.Movement.actor.gameObject.transform.position = zero;
                        this.Movement.isLerpFlying = false;
                        this.gravityControler.ResetLerpTime();
                    }
                    else
                    {
                        motionLerpDistance = this.gravityControler.GetMotionLerpDistance(_deltaTime);
                        zero.y += ((float) motionLerpDistance) / 1000f;
                        if (((float) num2) > zero.y)
                        {
                            zero.y = (float) num2;
                            this.Movement.actor.gameObject.transform.position = zero;
                            this.Movement.isLerpFlying = false;
                            this.gravityControler.ResetLerpTime();
                        }
                        else
                        {
                            this.Movement.actor.gameObject.transform.position = zero;
                        }
                    }
                }
            }
        }

        public void Init()
        {
            this.Movement.isFlying = false;
            this.Movement.isLerpFlying = false;
            this.motionControlers.Clear();
        }

        public void Move(int _deltaTime)
        {
            int motionDeltaDistance = 0;
            if (this.Movement.isFlying)
            {
                VInt3 location;
                if (this.motionControlers.Count != 0)
                {
                    for (int i = 0; i < this.motionControlers.Count; i++)
                    {
                        SpecialMotionControler controler = this.motionControlers[i];
                        motionDeltaDistance += controler.GetMotionDeltaDistance(_deltaTime);
                    }
                    location = this.Movement.actor.location;
                    location.y += motionDeltaDistance;
                    if (this.Movement.actor.groundY.i > location.y)
                    {
                        location.y = this.Movement.actor.groundY.i;
                        this.Movement.actor.location = location;
                    }
                    else
                    {
                        this.Movement.actor.location = location;
                    }
                }
                else
                {
                    location = this.Movement.actor.location;
                    if (this.Movement.actor.groundY.i == this.Movement.actor.location.y)
                    {
                        this.Movement.isFlying = false;
                        this.gravityControler.ResetTime();
                    }
                    else if (this.Movement.actor.groundY.i > this.Movement.actor.location.y)
                    {
                        location.y = this.Movement.actor.groundY.i;
                        this.Movement.actor.location = location;
                        this.Movement.isFlying = false;
                        this.gravityControler.ResetTime();
                    }
                    else
                    {
                        motionDeltaDistance = this.gravityControler.GetMotionDeltaDistance(_deltaTime);
                        location.y += motionDeltaDistance;
                        if (this.Movement.actor.groundY.i > location.y)
                        {
                            location.y = this.Movement.actor.groundY.i;
                            this.Movement.actor.location = location;
                            this.Movement.isFlying = false;
                            this.gravityControler.ResetTime();
                        }
                        else
                        {
                            this.Movement.actor.location = location;
                        }
                    }
                }
            }
        }

        public void RemoveMotionControler(SpecialMotionControler _controler)
        {
            this.motionControlers.Remove(_controler);
        }

        public void Reset()
        {
            this.Init();
            this.gravityControler.Reset();
        }
    }
}

