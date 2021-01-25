using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximityZoneScript : MonoBehaviour
{
    public GameObject button;
    public Color proximityColor;
    private void OnTriggerEnter(Collider other)
    {
        button.GetComponent<Image>().color = proximityColor;
        gameObject.SetActive(false);
    }
}
