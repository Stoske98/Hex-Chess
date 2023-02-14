using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardGameObject : MonoBehaviour
{
    public Action<MoveTowardGameObject,Vector3> OnArriveDestination;
    private Vector3 desired_position;
    private float speed;
    private float time;
    private float currentTime;

    public void Initialization(Vector3 _desired_position, float _speed,float delay = 0)
    {
        desired_position = _desired_position;
        speed = _speed;
        currentTime = Time.time;
        time = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= currentTime + time)
        {
            transform.position += (desired_position - transform.position).normalized * Time.deltaTime * speed;
            transform.LookAt(desired_position);
            if((desired_position - transform.position).magnitude < 0.5f)
            {
                OnArriveDestination?.Invoke(this, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
