using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovementScript : MonoBehaviour
{
    public Vector3 target = new Vector3(0, 0, 0);
    public float speed = 1;
    public float factor = 1;
    public Vector3 size = Vector3.one;

    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    bool toggle = false;

    private void Update()
    {
        if (transform.position != target && !toggle)
        {
            speed -= speed * Time.deltaTime * factor;
            // Moves the object to target position
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
            if (transform.position == target)
            {
                toggle = true;
                transform.localScale = size;
                posOffset = transform.position;
            } 
        } else if(toggle)
        {
            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;
        }

    }
}
