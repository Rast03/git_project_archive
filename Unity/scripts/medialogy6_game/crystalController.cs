using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crystalController : MonoBehaviour
{
    public GameObject initialHit;
    public GameObject lastHit;
    public Color lightColor;
    public string KRYSTAL_TYPE;
    public string direction;
    public bool active;

    private LineRenderer lineRenderer;
    private Vector3 lightDirection; void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        active = false;
        switch (KRYSTAL_TYPE)
        {
            case "red":
                lightColor = Color.red;
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Krystal_Red");
                break;
            case "green":
                lightColor = Color.green;
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Krystal_Green");
                break;
            case "blue":
                lightColor = Color.blue;
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Krystal_Blue");
                break;
        }
        lineRenderer.startColor = lightColor;
        lineRenderer.endColor = lightColor;

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + lightDirection / 100, lightDirection);
            lineRenderer.enabled = true;
            if (hit.transform.position != lastHit.transform.position)
            {
                lineRenderer.SetPosition(0, transform.position);
                if (hit.transform.tag.Contains("Reflective"))
                {
                    if (hit.transform.GetComponent<mirrorController>().numberOfInteractions < 2 && !hit.transform.GetComponent<mirrorController>().reflection)
                    {
                        hit.transform.GetComponent<mirrorController>().sourceDirection = direction;
                        hit.transform.GetComponent<mirrorController>().updateLightColor(lightColor);
                        hit.transform.GetComponent<mirrorController>().interaction = true;
                    }
                    if (hit.transform.GetComponent<mirrorController>().numberOfInteractions < 2)
                        hit.transform.GetComponent<mirrorController>().numberOfInteractions++;
                    //Invoke("delayNextMirror", 0.1f);
                }
                else if (hit.transform.tag.Contains("Prism"))
                {
                    hit.transform.GetComponent<prismController>().changeSourceDir(direction, colorToString());
                    //hit.transform.GetComponent<prismController>().sourceColor = colorToString();
                }
                else if (hit.transform.tag.Contains("Crystal"))
                {
                    if (hit.transform.GetComponent<crystalController>().lightColor == lightColor || lightColor == Color.white)
                    {
                        hit.transform.GetComponent<crystalController>().updateBeam(direction);
                        hit.transform.GetComponent<crystalController>().active = true;
                    }
                    //hit.transform.GetComponent<prismController>().sourceColor = colorToString();
                }
                else if (hit.transform.tag.Contains("Goal"))
                {
                    hit.transform.GetComponent<goalController>().sourceColor = colorToString();
                    Invoke("delayAchievedGoal", 0.2f);
                }


                if (lastHit.tag.Contains("Prism"))
                {
                    if (lastHit.GetComponent<prismController>().sourceDirection == direction)
                        lastHit.GetComponent<prismController>().sourceDirection = "none";
                }
                if (lastHit.tag.Contains("Crystal"))
                {
                    lastHit.GetComponent<crystalController>().active = false;
                }
                if (lastHit.tag.Contains("Reflective"))
                {
                    if (lastHit.GetComponent<mirrorController>().numberOfInteractions < 2)
                    {
                        lastHit.GetComponent<mirrorController>().interaction = false;
                        lastHit.GetComponent<mirrorController>().sourceDirection = "none";

                    }
                    if (lastHit.GetComponent<mirrorController>().numberOfInteractions > 0)
                    {
                        lastHit.GetComponent<mirrorController>().numberOfInteractions--;
                    }
                }
                lastHit = hit.transform.gameObject;
            }
            if (true) //TODO performance improvement
            {
                lineRenderer.SetPosition(0, transform.position);
                if (hit)
                    lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, transform.position.z));
                else
                    lineRenderer.SetPosition(1, lightDirection * 2000);
            }
        }
        else
        {   //reset
            lineRenderer.enabled = false;
            if (lastHit.tag.Contains("Reflective"))
            {
                if (lastHit.GetComponent<mirrorController>().numberOfInteractions < 2)
                {
                    lastHit.GetComponent<mirrorController>().interaction = false;
                    lastHit.GetComponent<mirrorController>().sourceDirection = "none";

                }
                if (lastHit.GetComponent<mirrorController>().numberOfInteractions > 0)
                {
                    lastHit.GetComponent<mirrorController>().numberOfInteractions--;
                }

            }
            if (lastHit.tag.Contains("Prism"))
            {
                if (lastHit.GetComponent<prismController>().sourceDirection == direction)
                    lastHit.GetComponent<prismController>().sourceDirection = "none";
            }
            if (lastHit.tag.Contains("Crystal"))
            {
                lastHit.GetComponent<crystalController>().active = false;
            }
            lastHit = initialHit;
        }

    }

    public void updateBeam(string sourceDir)
    {
        direction = sourceDir;

        switch (direction) 
        {
            case "up":
                lightDirection = new Vector3(0, 1, 0);
                break;
            case "down":
                lightDirection = new Vector3(0, -1, 0);
                break;
            case "right":
                lightDirection = new Vector3(1, 0, 0);
                break;
            case "left":
                lightDirection = new Vector3(-1, 0, 0);
                break;
        }
    }

    public string colorToString()
    {
        string tmpColor = "white";
        if (lightColor == Color.red)
        {
            tmpColor = "red";
        }
        else if (lightColor == Color.green)
        {
            tmpColor = "green";
        }
        else if (lightColor == Color.blue)
        {
            tmpColor = "blue";
        }
        return tmpColor;
    }

    private void delayAchievedGoal()
    {
        if (lastHit.tag.Contains("Goal"))
            lastHit.GetComponent<goalController>().goalAchieved();
    }
    private void delayNextMirror()
    {
        if (lastHit.tag.Contains("Reflective"))
        {
            if (lastHit.GetComponent<mirrorController>().numberOfInteractions < 2)
            {
                lastHit.GetComponent<mirrorController>().sourceDirection = direction;
                lastHit.GetComponent<mirrorController>().updateLightColor(lightColor);
                lastHit.GetComponent<mirrorController>().interaction = true;
            }
        }
    }

}
