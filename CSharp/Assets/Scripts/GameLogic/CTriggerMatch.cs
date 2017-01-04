namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [Serializable]
    public class CTriggerMatch
    {
        [SerializeField]
        public TriggerActionWrapper[] ActionList = new TriggerActionWrapper[0];
        public EGlobalTriggerAct ActType;
        public STriggerCondition Condition;
        [FriendlyName("冷却时间,0为无冷却")]
        public int CooldownTime;
        [FriendlyName("延迟触发时间,0为无延迟")]
        public int DelayTime;
        public EGlobalGameEvent EventType;
        public GameObject[] Listeners = new GameObject[0];
        [NonSerialized, HideInInspector]
        public int m_cooldownTimer;
        [NonSerialized, HideInInspector]
        public int m_triggeredCounter;
        public GameObject Originator;
        [FriendlyName("触发次数,0为无限")]
        public int TriggerCountMax;

        public void IntoCoolingDown()
        {
            if (this.CooldownTime > 0)
            {
                this.m_cooldownTimer = this.CooldownTime;
            }
        }

        public bool bCoolingDown
        {
            get
            {
                return ((this.CooldownTime > 0) && (this.m_cooldownTimer > 0));
            }
        }
    }
}

