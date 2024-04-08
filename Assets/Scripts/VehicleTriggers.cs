using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleTriggers : MonoBehaviour
{
    public Transform spawnPointTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Killbox")
        {
            transform.position = spawnPointTransform.position;
            transform.rotation = spawnPointTransform.rotation;
        }
        else if (other.name == "End Point")
        {
            Debug.Log("You win!");
        }
    }
}
