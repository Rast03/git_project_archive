using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class goalController : MonoBehaviour
{
    public Sprite achievedGoal;
    public string sourceColor;
    public string pyramidColor;
    public SpriteRenderer spriteRenderer;
    public GameObject levelCompletePanel;
    List<GameObject> pyramids = new List<GameObject>();

    void Start() {
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            var pattern = @"\b" + Regex.Escape("Pyramid") + @"\b";
            Regex rgx = new Regex(pattern);
            Match isMatch = rgx.Match(obj.name);

            if (isMatch.Success)
            {
                this.pyramids.Add(obj);
            }   
        }
    }
    
    public string getSourceColor() {
        return this.sourceColor;
    }

    public string getPyramidColor() {
        return this.pyramidColor;
    }

    public void goalAchieved()
    {   
        if(this.pyramidColor != this.sourceColor) 
        {
            return;
        }

        bool isCompleted = true;
        spriteRenderer.sprite = achievedGoal;
        foreach (GameObject p in pyramids)
        {   
            goalController controller = (goalController) p.GetComponent(typeof(goalController));
            if (controller.getPyramidColor() != controller.getSourceColor())
            {
                isCompleted = false;
                break;    
            }
        }

        if(isCompleted) 
        {
            Invoke("delayLevelCpl",0.7f);
        }        
    }

    void delayLevelCpl()
    {
        levelCompletePanel.SetActive(true);
    }

}
