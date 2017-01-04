using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), AddComponentMenu("Wwise/AkEnvironment")]
public class AkEnvironment : MonoBehaviour
{
    public bool excludeOthers;
    public bool isDefault;
    [SerializeField]
    private int m_auxBusID;
    public static int MAX_NB_ENVIRONMENTS = 4;
    public int priority;
    public static AkEnvironment_CompareByPriority s_compareByPriority = new AkEnvironment_CompareByPriority();
    public static AkEnvironment_CompareBySelectionAlgorithm s_compareBySelectionAlgorithm = new AkEnvironment_CompareBySelectionAlgorithm();

    public uint GetAuxBusID()
    {
        return (uint) this.m_auxBusID;
    }

    public float GetAuxSendValueForPosition(Vector3 in_position)
    {
        return 1f;
    }

    public void SetAuxBusID(int in_auxBusID)
    {
        this.m_auxBusID = in_auxBusID;
    }

    public class AkEnvironment_CompareByPriority : IComparer<AkEnvironment>
    {
        public int Compare(AkEnvironment a, AkEnvironment b)
        {
            int num = a.priority.CompareTo(b.priority);
            if ((num == 0) && (a != b))
            {
                return 1;
            }
            return num;
        }
    }

    public class AkEnvironment_CompareBySelectionAlgorithm : IComparer<AkEnvironment>
    {
        public int Compare(AkEnvironment a, AkEnvironment b)
        {
            if (a.isDefault)
            {
                if (b.isDefault)
                {
                    return this.compareByPriority(a, b);
                }
                return 1;
            }
            if (b.isDefault)
            {
                return -1;
            }
            if (a.excludeOthers)
            {
                if (b.excludeOthers)
                {
                    return this.compareByPriority(a, b);
                }
                return -1;
            }
            if (b.excludeOthers)
            {
                return 1;
            }
            return this.compareByPriority(a, b);
        }

        private int compareByPriority(AkEnvironment a, AkEnvironment b)
        {
            int num = a.priority.CompareTo(b.priority);
            if ((num == 0) && (a != b))
            {
                return 1;
            }
            return num;
        }
    }
}

