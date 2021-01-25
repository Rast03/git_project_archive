using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurSolutionCam : MonoBehaviour
{
    //public GameObject targetImage;

    void Start()
    {
        //targetImage.GetComponent<TakeScreenshot>().takeScreen(GetComponent<Camera>());
        Invoke("delayedStart", 0.1f);
    }

    private void delayedStart()
    {
        gameObject.SetActive(false);
    }
}
