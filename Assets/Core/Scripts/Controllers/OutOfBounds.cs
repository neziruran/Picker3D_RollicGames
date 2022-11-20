using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BallTag"))
        {
            Debug.Log(" Destroyed out of bounds ball");
            Destroy(collision.gameObject);
        }

    }
}
