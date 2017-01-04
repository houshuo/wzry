using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class SpawnCountdown : MonoBehaviour
{
    private SpawnGroup m_sg;
    public string m_tipsID;

    private void OnDestroy()
    {
        if (this.m_sg == Singleton<BattleLogic>.GetInstance().m_dragonSpawn)
        {
            Singleton<BattleLogic>.GetInstance().m_dragonSpawn = null;
            Singleton<BattleLogic>.GetInstance().m_countDownTips = null;
        }
    }

    private void Start()
    {
        this.m_sg = base.GetComponent<SpawnGroup>();
        if (this.m_sg != null)
        {
            Singleton<BattleLogic>.GetInstance().m_dragonSpawn = this.m_sg;
            Singleton<BattleLogic>.GetInstance().m_countDownTips = Singleton<CTextManager>.GetInstance().GetText(this.m_tipsID);
        }
    }
}

