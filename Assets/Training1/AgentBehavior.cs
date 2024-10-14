using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentBehavior : Agent
{
    public List<TargetBehavior> targets;
    public List<Transform> walls;

    private float time = 10f;

    private void Update()
    {
        time -= Time.deltaTime;

        if (time < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
        
    }

    public override void OnEpisodeBegin()
    {

        time = 10f;
        
        transform.localPosition = new Vector3(0, 1.74f, 0);
        
        targets.ForEach(target =>
        {
            target.transform.localPosition = transform.localPosition + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        });
        
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(time);
        
        walls.ForEach(wall =>
        {
            sensor.AddObservation(wall.localPosition);
        });
        
        targets.ForEach(target =>
        {
            sensor.AddObservation(target.transform.localPosition);
            sensor.AddObservation(target.value);
        });
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveSpeed = 6f;
        
        transform.localPosition += new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.deltaTime;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            
            TargetBehavior target = other.gameObject.GetComponent<TargetBehavior>();

            if (target != null)
            {
                AddReward(target.value);
                EndEpisode();
            }
            

        }
        
        if (other.CompareTag("wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
