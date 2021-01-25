using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushingScript : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 movePoint;

    // Start is called before the first frame update
    void Start()
    {
        movePoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint, speed * Time.deltaTime);
    }
}
