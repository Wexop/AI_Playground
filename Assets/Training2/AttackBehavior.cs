using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    
    public AgentFight agentFight;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agentFight.AddReward(3f);
            agentFight.time = agentFight.initialTime;
        }
    }
}
