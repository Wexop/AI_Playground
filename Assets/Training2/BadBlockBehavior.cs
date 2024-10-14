using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadBlockBehavior : MonoBehaviour
{
    private float timer;
    private float initTimer = 3;

    private Vector3 nextPos;
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
        nextPos = new Vector3(Random.Range(-10.0f, 10.0f), transform.localPosition.y, Random.Range(-10.0f, 10.0f));
        transform.localPosition = new Vector3(Random.Range(-12.0f, 12.0f), transform.localPosition.y, Random.Range(-12.0f, 12.0f));
        timer = 0f;
    }
    void Update()
    {
        timer += Time.deltaTime;

        float lerpProgress = timer / initTimer;
        
  
        
        transform.localPosition = Vector3.Lerp(initialPos, nextPos, lerpProgress );

        if (timer >= initTimer)
        {
            timer = 0;
            initialPos = transform.localPosition;
            nextPos = new Vector3(Random.Range(-12.0f, 12.0f), transform.localPosition.y, Random.Range(-12.0f, 12.0f));
        }
    }
}
