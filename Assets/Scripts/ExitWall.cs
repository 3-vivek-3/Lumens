using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GameplayManager.instance.GameOver();
            Debug.Log("Hit exit wall");
        }
    }
}
