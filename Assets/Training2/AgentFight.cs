using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentFight : Agent
{
    public List<Transform> walls;
    public List<Transform> badBlocks;
    
    public Rigidbody rb;

    public AttackBehavior attackBehavior;
    
    public List<AgentFight> targets;

    public int killStreak;
    
    private RaycastHit hit = new RaycastHit();

    public float attackTime;
    private float attackCD = 2f;


    public float jumpTime;
    private float jumpCD = 3f;

    public float dashTime;
    public float inDashTime;
    private float dashCD = 2f;
    
    
    public float time;
    public float initialTime = 120f;


    private void Start()
    {
        attackBehavior.gameObject.SetActive(false);
    }

    private void Update()
    {
        Ray ray = new Ray(transform.localPosition, transform.forward);

        Physics.Raycast(ray, out hit);
        attackTime -= Time.deltaTime;
        dashTime -= Time.deltaTime;
        inDashTime -= Time.deltaTime;
        jumpTime -= Time.deltaTime;
        
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        
        time -= Time.deltaTime;

        if (time < 0)
        {
            AddReward(-3f);
            EndEpisode();
        }

        if (inDashTime > 0)
        {
            rb.AddForce(transform.forward * 5, ForceMode.VelocityChange);
        }
        
    }

    private IEnumerator Attack()
    {
        attackBehavior.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        
        attackBehavior.gameObject.SetActive(false);


    }


    public override void OnEpisodeBegin()
    {
        
        transform.localPosition = new Vector3(Random.Range(-10f, 10f), 1.74f, Random.Range(-10f, 10f));
        attackTime = attackCD;
        dashTime = dashCD;
        jumpTime = jumpCD;
        time = initialTime;

        killStreak = 0;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
        sensor.AddObservation(rb.velocity.y);
        
        sensor.AddObservation(killStreak);
        
        sensor.AddObservation(dashTime);
        sensor.AddObservation(dashCD);
        sensor.AddObservation(inDashTime);
        
        sensor.AddObservation(jumpTime);
        sensor.AddObservation(jumpCD);
        
        sensor.AddObservation(time);
        
        
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(attackBehavior.transform.localPosition);
        

        
        sensor.AddObservation(attackCD);
        sensor.AddObservation(attackTime);
        sensor.AddObservation(attackBehavior.enabled);


        sensor.AddObservation(hit.distance);
        sensor.AddObservation( hit.rigidbody && hit.rigidbody.CompareTag("Player"));
        

        
        walls.ForEach(wall =>
        {
            sensor.AddObservation(wall.localPosition);
        });
        
        badBlocks.ForEach(b =>
        {
            sensor.AddObservation(b.localPosition);
            sensor.AddObservation(Vector3.Distance(transform.localPosition, b.localPosition));
        });
        
        targets.ForEach(target =>
        {
            sensor.AddObservation(target.attackBehavior.enabled);
            sensor.AddObservation(target.transform.localPosition);
            sensor.AddObservation(target.transform.localRotation);
            sensor.AddObservation(target.attackTime);
            sensor.AddObservation(Vector3.Distance(transform.localPosition, target.transform.localPosition));
            sensor.AddObservation(target.jumpTime);
            sensor.AddObservation(target.dashTime);
            sensor.AddObservation(target.killStreak);
        });
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotateL = actions.ContinuousActions[2];
        float rotateR = actions.ContinuousActions[3];
        float attack = actions.ContinuousActions[4];
        float dash = actions.ContinuousActions[5];
        float jump = actions.ContinuousActions[6];
        float moveSpeed = 10f;
        float rotateSpeed = 10f;
        float jumpForce = 8f;
        
        rb.velocity = new Vector3(moveX * moveSpeed, rb.velocity.y, moveZ * moveSpeed) ;
        
        transform.eulerAngles += new Vector3(0f, rotateL - rotateR, 0f) * rotateSpeed;

        if (attack > 0 && attackTime < 0)
        {
            attackTime = attackCD;
            StartCoroutine(Attack());
        }

        if (dash > 0 && dashTime < 0)
        {
            dashTime = dashCD;
            inDashTime = 0.2f;
        }

        if (jump > 0 && jumpTime < 0)
        {
            jumpTime = jumpCD;
            rb.velocity = new Vector3(rb.velocity.x,jumpForce, rb.velocity.z);
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
        actions[2] = Input.GetKey(KeyCode.R) ? 1f : 0f;
        actions[3] = Input.GetKey(KeyCode.T) ? 1f : 0f;
        actions[4] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        actions[5] = Input.GetKey(KeyCode.E) ? 1f : 0f;
        actions[6] = Input.GetKey(KeyCode.F) ? 1f : 0f;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("wall") )
        {
            AddReward(-1f);
            EndEpisode();
        }
        if (other.CompareTag("attack") || other.CompareTag("obstacle"))
        {
            AddReward(-2f);
            EndEpisode();
        }
    }
}
