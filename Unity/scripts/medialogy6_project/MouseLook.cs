using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public List<float> listvaluesX;
    public List<float> listvaluesY;

    float xRotation = 0f;
    //StreamWriter writer;
    // Start is called before the first frame update
    void Start()
    {
        listvaluesX = new List<float>();
        listvaluesY = new List<float>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Invoke("OneMinMarkDelay", 60f);
        //writer = new StreamWriter("Assets/Resources/text.txt", true);
        //writer.Write("mouseX:");
        //writer.Close();
    }

    string output = "MouseX: ";
    float time = 0;
    public int outputLimit = 0;
    public bool oneMinMark = false;
    public bool doOnce = false;
    public bool Qactivated = false;
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        if (oneMinMark && outputLimit <= 1800 && !Qactivated)
        {
            time += 1 * Time.deltaTime;
            if (time >= 0.1f)
            {
                outputLimit += 1;
                output += " " + mouseX;
                //writer = new StreamWriter("Assets/Resources/text.txt", true);
                //writer.Write(" " + mouseX);
                //writer.Close();
                time = 0;
                doOnce = true;
            }
        } else if(doOnce && !Qactivated)
        {
            //writer = new StreamWriter("Assets/Resources/text.txt", true);
            //writer.WriteLine(" Done");
            //writer.Close();

            WriteString(output);
            doOnce = false;
        }



        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void WriteString(string text)
    {
        string path = "test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(text);
        writer.Close();
    }

    private void OneMinMarkDelay()
    {
        oneMinMark = true;
    }
}
