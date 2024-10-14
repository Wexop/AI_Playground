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
            
            AgentFight playerFight = other.gameObject.GetComponent<AgentFight>();
            
            agentFight.killStreak++;
            agentFight.AddReward(3f + ((float)agentFight.killStreak / 2) + ((float) playerFight.killStreak / 2));
            agentFight.time = agentFight.initialTime;
            
        }
    }
}
