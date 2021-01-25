using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public GameObject FinalText;
    private void OnTriggerEnter(Collider other)
    {
        CinematicBars.Show_Static(1300f, 4f);
        FinalText.SetActive(true);
        gameObject.SetActive(false);
    }
}
