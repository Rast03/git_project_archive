using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prismController : MonoBehaviour
{
    public GameObject redLight;
    public GameObject greenLight;
    public GameObject blueLight;
    public GameObject initialHit;

    public string direction;
    public string sourceDirection;
    public string sourceColor;
    public bool newPosition;

    string redDir;
    string greenDir;
    string blueDir;

    Color redBeam = Color.red;
    Color greenBeam = Color.green;
    Color blueBeam = Color.blue;

    void Start()
    {
        redLight.GetComponent<prismBeamController>().initialHit = initialHit;
        greenLight.GetComponent<prismBeamController>().initialHit = initialHit;
        blueLight.GetComponent<prismBeamController>().initialHit = initialHit;
        redLight.GetComponent<prismBeamController>().lastHit = initialHit;
        greenLight.GetComponent<prismBeamController>().lastHit = initialHit;
        blueLight.GetComponent<prismBeamController>().lastHit = initialHit;
        newPosition = true;
    }

    void Update()
    {

        if (newPosition)
        {
            updatePosition();
            newPosition = false;
        }


        if (activationCheck())
        {
            if (sourceColor == "white" || sourceColor == "red")
            {
                redLight.GetComponent<prismBeamController>().updateBeam(redBeam, redDir);
                redLight.GetComponent<prismBeamController>().active = true;
                redLight.GetComponent<LineRenderer>().enabled = true;
            }
            if (sourceColor == "white" || sourceColor == "green")
            {
                greenLight.GetComponent<prismBeamController>().updateBeam(greenBeam, greenDir);
                greenLight.GetComponent<prismBeamController>().active = true;
                greenLight.GetComponent<LineRenderer>().enabled = true;
            }
            if (sourceColor == "white" || sourceColor == "blue")
            {
                blueLight.GetComponent<prismBeamController>().updateBeam(blueBeam, blueDir);
                blueLight.GetComponent<prismBeamController>().active = true;
                blueLight.GetComponent<LineRenderer>().enabled = true;
            }
        }
        else
        {
            redLight.GetComponent<prismBeamController>().active = false;
            greenLight.GetComponent<prismBeamController>().active = false;
            blueLight.GetComponent<prismBeamController>().active = false;
            redLight.GetComponent<LineRenderer>().enabled = false;
            greenLight.GetComponent<LineRenderer>().enabled = false;
            blueLight.GetComponent<LineRenderer>().enabled = false;
            //Invoke("delayTurnOffBeam", 0.1f);
        }
    }

    private void delayTurnOffBeam()
    {
        redLight.GetComponent<prismBeamController>().active = false;
        greenLight.GetComponent<prismBeamController>().active = false;
        blueLight.GetComponent<prismBeamController>().active = false;
        redLight.GetComponent<LineRenderer>().enabled = false;
        greenLight.GetComponent<LineRenderer>().enabled = false;
        blueLight.GetComponent<LineRenderer>().enabled = false;
    }

    public void updatePosition()
    {
        switch (direction)
        {
            case "up":
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.16f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.13f, 0.666f);
                redLight.transform.localPosition = new Vector3(0.25f, -0.12f, 0.0f);
                greenLight.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f);
                blueLight.transform.localPosition = new Vector3(-0.25f, -0.12f, 0.0f);
                break;
            case "down":
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.13f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.4f, 0.66f);
                redLight.transform.localPosition = new Vector3(-0.2f, 0.0f, 0.0f);
                greenLight.transform.localPosition = new Vector3(0.0f,0.45f,0.0f);
                blueLight.transform.localPosition = new Vector3(0.2f, 0.0f, 0.0f);
                break;
            case "right":
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.015f, 0.16f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 0.52f);
                redLight.transform.localPosition = new Vector3(0.0f, -0.15f, 0.0f);
                greenLight.transform.localPosition = new Vector3(-0.189f, -0.1f, 0.0f);
                blueLight.transform.localPosition = new Vector3(0.0f, 0.4f, 0.0f);
                break;
            case "left":
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.17f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(0.31f, 0.54f);
                redLight.transform.localPosition = new Vector3(0.0f, 0.4f, 0.0f);
                greenLight.transform.localPosition = new Vector3(0.19f, -0.05f, 0.0f);
                blueLight.transform.localPosition = new Vector3(0f, -0.2f, 0.0f);
                break;
        }
    }

    public void changeSourceDir(string newSourceDir, string newsourceColor)
    {
        if (!activationCheck())
        {
            sourceDirection = newSourceDir;
            sourceColor = newsourceColor;
        }
            
    }

    bool activationCheck()
    {
        if (direction.Equals("up") && sourceDirection.Equals("down"))
        {
            redDir = "right";
            greenDir = "down";
            blueDir = "left";
            return true;
        }
        else if (direction.Equals("down") && sourceDirection.Equals("up"))
        {
            redDir = "left";
            greenDir = "up";
            blueDir = "right";
            return true;
        }
        else if (direction.Equals("right") && sourceDirection.Equals("left"))
        {
            redDir = "down";
            greenDir = "left";
            blueDir = "up";
            return true;
        }
        else if (direction.Equals("left") && sourceDirection.Equals("right"))
        {
            redDir = "up";
            greenDir = "right";
            blueDir = "down";
            return true;
        }
        else
            return false;
    }

}
