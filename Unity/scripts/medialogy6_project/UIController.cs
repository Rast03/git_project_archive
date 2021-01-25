using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject triggerDrone;
    public GameObject virtualCamera;
    public GameObject mainCamera;

    public GameObject WASDtext;
    public GameObject ControllsPrompt;
    public GameObject Controlls;


    bool turnOffPrompt = true;

    // Start is called before the first frame update
    void Start()
    {
        CinematicBars.Show_Static(200f, 2f);
    }

    bool doOnce = true;
    // Update is called once per frame
    void Update()
    {
        if (triggerDrone.transform.position == triggerDrone.GetComponent<DroneMovementScript>().target && doOnce)
        {    
            CinematicBars.Hide_Static(2f);
            virtualCamera.SetActive(false);
            Invoke("delayedInput", 2f);
            doOnce = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (turnOffPrompt)
            {
                ControllsPrompt.SetActive(false);
                turnOffPrompt = false;
            }
            Controlls.SetActive(!Controlls.activeSelf);
        }

    }

    private void delayedInput()
    {
        mainCamera.GetComponent<MouseLook>().enabled = true;
        Invoke("delayedControls", 2f);
    }



    private void delayedControls()
    {
        ControllsPrompt.SetActive(true);
    }    

}
