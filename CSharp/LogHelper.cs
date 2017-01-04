using Pathfinding.RVO;
using Pathfinding.RVO.Sampled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class LogHelper
{
    private static byte[] m_fowBaseData;
    private static byte[] m_fowPermanentLitData;
    private static byte[] m_fowVisibleData;
    private static int[] m_surfCellData;

    private static string ByteArrayToHexString(byte[] data)
    {
        StringBuilder builder = new StringBuilder();
        int length = data.Length;
        for (int i = 0; i < length; i++)
        {
            builder.Append(Convert.ToString(data[i]));
        }
        return builder.ToString();
    }

    private static string IntArrayToHexString(int[] data)
    {
        StringBuilder builder = new StringBuilder();
        int length = data.Length;
        for (int i = 0; i < length; i++)
        {
            builder.Append(Convert.ToString(data[i]));
        }
        return builder.ToString();
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
    public static void LogActor(List<PoolObjHandle<ActorRoot>> GameActors)
    {
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void LogRvo()
    {
        SLogObj loger = DebugHelper.GetLoger(SLogCategory.Rvo);
        if ((loger != null) && Singleton<FrameSynchr>.instance.bActive)
        {
            List<Agent> agents = RVOSimulator.Instance.GetSimulator().agents;
            loger.Log(string.Format("FrameNum: {0}", Singleton<FrameSynchr>.GetInstance().CurFrameNum));
            for (int i = 0; i < agents.Count; i++)
            {
                Agent agent = agents[i];
                GameObject owner = agent.owner as GameObject;
                StringBuilder builder = new StringBuilder();
                builder.Append((owner == null) ? "<null>" : owner.name);
                builder.Append("   ");
                builder.Append(" smoothPos:");
                builder.Append(agent.InterpolatedPosition.ToString());
                builder.Append(" prevSmoothPos:");
                builder.Append(agent.prevSmoothPos.ToString());
                builder.Append(" position:");
                builder.Append(agent.position.ToString());
                builder.Append(" desiredVelocity:");
                builder.Append(agent.desiredVelocity.ToString());
                builder.Append(" Velocity:");
                builder.Append(agent.Velocity.ToString());
                builder.Append(" velocity:");
                builder.Append(agent.velocity.ToString());
                builder.Append(" newVelocity:");
                builder.Append(agent.newVelocity.ToString());
                if (agent.neighbours.Count > 0)
                {
                    builder.Append(" neighbours:");
                    for (int j = 0; j < agent.neighbours.Count; j++)
                    {
                        Agent agent2 = agent.neighbours[j];
                        builder.Append((agent2.owner as GameObject).name);
                        builder.Append("=");
                        builder.Append(agent.neighbourDists[j]);
                        builder.Append(";");
                    }
                }
                loger.Log(builder.ToString());
            }
            loger.Log("\n\n");
        }
    }

    private static string UIntArrayToHexString(uint[] data)
    {
        StringBuilder builder = new StringBuilder();
        int length = data.Length;
        for (int i = 0; i < length; i++)
        {
            builder.Append(Convert.ToString(data[i]));
        }
        return builder.ToString();
    }
}

