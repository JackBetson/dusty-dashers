using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public GameObject winMessage; // Assign the UI Text GameObject in the Inspector

    // Start is called before the first frame update
    private void Start()
    {
        if (winMessage != null)
            winMessage.SetActive(false); // Ensure the win message is hidden at start
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            if (winMessage != null)
                winMessage.SetActive(true); // Display the win message
        }
    }
}
