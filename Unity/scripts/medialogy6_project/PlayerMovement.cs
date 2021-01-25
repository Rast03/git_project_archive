using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 8f;
    public float runningSpeed = 20f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public Transform WaterCheckW;
    public Transform WaterCheckS;
    public Transform WaterCheckA;
    public Transform WaterCheckD;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3f;
    public LayerMask groundMask;
    public LayerMask waterLayer;

    public Vector3 lastPosition;
    public Vector3 resetPosition;
    public float time = 0;

    Vector3 velocity;
    bool isGrounded;
    public bool isCloseToWater;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log("teleport");
            GetComponent<CharacterController>().enabled = false;
            this.transform.position = resetPosition;
            GetComponent<CharacterController>().enabled = true;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isCloseToWater = Physics.CheckSphere(transform.position, 0.5f, waterLayer);
        //isCloseToWater = Physics.CheckSphere(WaterCheckS.position, 2, waterLayer);
        //isCloseToWater = Physics.CheckSphere(WaterCheckA.position, 2, waterLayer);
        //isCloseToWater = Physics.CheckSphere(WaterCheckD.position, 2, waterLayer);

        if (isCloseToWater)
        {
            Debug.Log("teleport");
            GetComponent<CharacterController>().enabled = false;
            this.transform.position = lastPosition;
            GetComponent<CharacterController>().enabled = true;
        }
            

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(move * runningSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime);
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        time += 1 * Time.deltaTime;
        if (time >= 6)
        {
            time = 0;
            lastPosition = transform.position;
        }
    }
}
