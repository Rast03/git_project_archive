using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mirrorController : MonoBehaviour
{
    public int corner;
    public GameObject initialHit;
    public Color lightColor = Color.white;


    //[HideInInspector]
    public string sourceDirection;
    //[HideInInspector]
    public bool interaction;
    //[HideInInspector]
    public GameObject lastHit;
    public string type;

    public int numberOfInteractions;
    public bool reflection;
    public bool newPosition;
    private LineRenderer lineRenderer;
    private int previousCorner;
    public string previousSourceDirection;
    public string direction;
    private Vector3 reflectionDirection;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        interaction = false;
        reflection = false;
        previousCorner = 0;
        numberOfInteractions = 0;
        sourceDirection = "none";
        previousSourceDirection = "none";
        direction = "none";
        lineRenderer.startColor = lightColor;
        lineRenderer.endColor = lightColor;
        newPosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (newPosition)
        {
            updatePOsition();
            newPosition = false;
        }


        if ((corner != previousCorner) || (sourceDirection != previousSourceDirection))
        {

            reflection = sourceCornerCheck();

            if (lastHit != null && lastHit.tag.Contains("Reflective"))
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
            previousSourceDirection = sourceDirection;
            previousCorner = corner;
        }

        if (interaction && reflection)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + reflectionDirection / 100, reflectionDirection);
            if (!lineRenderer.enabled)
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
                switch (corner)
                {
                    case 1:
                        lineRenderer.SetPosition(0, transform.position);
                        break;
                    case 2:
                        lineRenderer.SetPosition(0, transform.position);
                        break;
                    case 3:
                        lineRenderer.SetPosition(0, transform.position + reflectionDirection/3);
                        break;
                    case 4:
                        lineRenderer.SetPosition(0, transform.position + reflectionDirection/3);
                        break;
                }
                if (hit)
                    lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, transform.position.z));
                else
                    lineRenderer.SetPosition(1, reflectionDirection * 2000);
            }
        }
        else
        {   //reset
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
            //sourceDirection = "none";
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
        }


    }

    public void updatePOsition()
    {
        switch (corner)
        {
            case 1:
                if (type == "pushable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_move1");
                if (type == "rotatable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot1");
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.1f, 0.14f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.42f, 0.58f);
                break;
            case 2:
                if (type == "pushable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_move2");
                if (type == "rotatable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot2");
                GetComponent<BoxCollider2D>().offset = new Vector2(0.12f, 0.14f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.35f, 0.58f);
                break;
            case 3:
                if (type == "pushable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_move3");
                if (type == "rotatable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot3");
                GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.0f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.62f, 0.85f);
                break;
            case 4:
                if (type == "pushable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_move4");
                if (type == "rotatable")
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot4");
                GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.0f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.62f, 0.85f);
                break;
        }
    }

    private string colorToString()
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
            if (lastHit.GetComponent<mirrorController>().numberOfInteractions < 2 && !lastHit.GetComponent<mirrorController>().reflection) 
            {       
                lastHit.GetComponent<mirrorController>().sourceDirection = direction; 
                lastHit.GetComponent<mirrorController>().updateLightColor(lightColor); 
                lastHit.GetComponent<mirrorController>().interaction = true;
            }
            if (lastHit.GetComponent<mirrorController>().numberOfInteractions < 2)
                lastHit.GetComponent<mirrorController>().numberOfInteractions++;
        }
    }

    public void updateLightColor(Color color)
    {
        lightColor = color;
        lineRenderer.startColor = lightColor;
        lineRenderer.endColor = lightColor;
    }

    private bool sourceCornerCheck()
    {
        switch (sourceDirection)
        {
            case "up":
                if (corner == 1)        //top left corner
                {
                    reflectionDirection = new Vector3(1, 0, 0);     //right reflection
                    direction = "right";
                    return true;
                }
                    
                else if (corner == 2)       //top right corner
                {
                    reflectionDirection = new Vector3(-1, 0, 0);     //left reflection
                    direction = "left";
                    return true;
                }
                break;
            case "down":
                if (corner == 3)        //bottom right corner
                {
                    reflectionDirection = new Vector3(-1, 0, 0);     //left reflection
                    direction = "left";
                    return true;
                }
                    
                else if (corner == 4)       //bottom left corner
                {
                    reflectionDirection = new Vector3(1, 0, 0);     //right reflection
                    direction = "right";
                    return true;
                }
                break;
            case "right":
                if (corner == 2)        //top right corner
                {
                    reflectionDirection = new Vector3(0, -1, 0);     //down reflection
                    direction = "down";
                    return true;
                }
                    
                else if (corner == 3)       //bottom right corner
                {
                    reflectionDirection = new Vector3(0, 1, 0);     //up reflection
                    direction = "up";
                    return true;
                }
                break;
            case "left":
                if (corner == 1)        //top left corner
                {
                    reflectionDirection = new Vector3(0, -1, 0);     //down reflection
                    direction = "down";
                    return true;
                }
                    
                else if (corner == 4)       //bottom left corner
                {
                    reflectionDirection = new Vector3(0, 1, 0);     //up reflection
                    direction = "up";
                    return true;
                }
                break;   
        }
        return false;
    }
}
