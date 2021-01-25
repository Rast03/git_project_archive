using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurSolutionScript : MonoBehaviour
{
    public GameObject canvas;
    public GameObject mainCamera;
    public GameObject Qprompt;

    bool turnOffQPrompt = true;
    void Start()
    {
        Invoke("delayedQPrompt", 15f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (turnOffQPrompt)
            {
                Qprompt.SetActive(false);
                turnOffQPrompt = false;
            }
            canvas.SetActive(!canvas.activeSelf);
            Cursor.lockState = canvas.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = canvas.activeSelf ? true : false;
            mainCamera.GetComponent<MouseLook>().mouseSensitivity = canvas.activeSelf ? 5f : 100;
            mainCamera.GetComponent<MouseLook>().Qactivated = canvas.activeSelf ? true : false;
        }
    }

    private void delayedQPrompt()
    {
        Qprompt.SetActive(true);
    }
}
