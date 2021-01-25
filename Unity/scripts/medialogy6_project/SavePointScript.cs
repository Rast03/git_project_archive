using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointScript : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        player.GetComponent<PlayerMovement>().resetPosition = transform.position;
        gameObject.SetActive(false);
    }
}
