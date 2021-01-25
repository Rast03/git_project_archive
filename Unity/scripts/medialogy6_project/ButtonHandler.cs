using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject camImage;
    public GameObject fsButton;
    public GameObject displayedCamButton;

    public GameObject positionText;
    public GameObject leftButton;
    public GameObject rightButton;

    public GameObject Camera;       //for changing the angle
    public GameObject Drone;       //for changing the angle

    public string directionLabel = "none";

    void Start()
    {
    }

    public void fullscreenImgON()
    {
        fsButton.SetActive(true);
        fsButton.GetComponent<ButtonHandler>().displayedCamButton = gameObject;
        FindObjectInChilds(fsButton, "FSCamImage").GetComponent<RawImage>().texture = camImage.GetComponent<RawImage>().texture;
        FindObjectInChilds(fsButton, "FSPositionText").GetComponent<Text>().text = positionText.GetComponent<Text>().text;
        FindObjectInChilds(fsButton, "FSLeftText").GetComponent<Text>().text = leftButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(fsButton, "FSRightText").GetComponent<Text>().text = rightButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(fsButton, "FSLeftButton").GetComponent<ButtonHandler>().directionLabel = leftButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(fsButton, "FSRightButton").GetComponent<ButtonHandler>().directionLabel = rightButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(fsButton, "FSLeftButton").GetComponent<ButtonHandler>().Camera = Camera;
        FindObjectInChilds(fsButton, "FSLeftButton").GetComponent<ButtonHandler>().Drone = Drone;
        FindObjectInChilds(fsButton, "FSRightButton").GetComponent<ButtonHandler>().Camera = Camera;
        FindObjectInChilds(fsButton, "FSRightButton").GetComponent<ButtonHandler>().Drone = Drone;
    }
    public void fullscreenImgOff()
    {
        FindObjectInChilds(displayedCamButton, "CamImage").GetComponent<RawImage>().texture = camImage.GetComponent<RawImage>().texture;
        FindObjectInChilds(displayedCamButton, "PositionText").GetComponent<Text>().text = positionText.GetComponent<Text>().text;
        FindObjectInChilds(displayedCamButton, "LeftText").GetComponent<Text>().text = leftButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(displayedCamButton, "RightText").GetComponent<Text>().text = rightButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(displayedCamButton, "LeftButton").GetComponent<ButtonHandler>().directionLabel = leftButton.GetComponentInChildren<Text>().text;
        FindObjectInChilds(displayedCamButton, "RightButton").GetComponent<ButtonHandler>().directionLabel = rightButton.GetComponentInChildren<Text>().text;
        fsButton.SetActive(false);
    }

    public void nextCamView()
    {
        switch (positionText.GetComponent<Text>().text)
        {
            case "-NORTH-":
                if (directionLabel == "W")
                {
                    changeDirection("-WEST-", "S", "N", 270f);
                }
                else if (directionLabel == "E")
                {
                    changeDirection("-EAST-", "N", "S", 90f);
                }
                break;
            case "-SOUTH-":
                if (directionLabel == "W")
                {
                    changeDirection("-WEST-", "S", "N", 270f);
                }
                else if (directionLabel == "E")
                {
                    changeDirection("-EAST-", "N", "S", 90f);
                }
                break;
            case "-WEST-":
                if (directionLabel == "N")
                {
                    changeDirection("-NORTH-", "W", "E", 0f);
                }
                else if (directionLabel == "S")
                {
                    changeDirection("-SOUTH-", "E", "W", 180f);
                }
                break;
            case "-EAST-":
                if (directionLabel == "N")
                {
                    changeDirection("-NORTH-", "W", "E", 0f);
                }
                else if (directionLabel == "S")
                {
                    changeDirection("-SOUTH-", "E", "W", 180f);
                }
                break;
            default:
                break;
        }
    }

    private void changeDirection(string pText, string lBText, string rBText, float angle)
    {
        positionText.GetComponent<Text>().text = pText;
        leftButton.GetComponentInChildren<Text>().text = lBText;
        rightButton.GetComponentInChildren<Text>().text = rBText;
        leftButton.GetComponent<ButtonHandler>().directionLabel = lBText;
        rightButton.GetComponent<ButtonHandler>().directionLabel = rBText;
        Camera.SetActive(true);
        Camera.transform.localRotation = Quaternion.Euler(0, angle, 0f);
        Drone.transform.localRotation = Quaternion.Euler(0, angle, 0f);
        //camImage.GetComponent<TakeScreenshot>().takeScreen(Camera.GetComponent<Camera>());
        Invoke("delayedStart", 0.1f);
    }

    private void delayedStart()
    {
        Camera.SetActive(false);
    }

    GameObject FindObjectInChilds(GameObject gameObject, string gameObjectName)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform item in children)
        {
            if (item.name == gameObjectName)
            {
                return item.gameObject;
            }
        }

        return null;
    }

}
