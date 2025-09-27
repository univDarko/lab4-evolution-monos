using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoMonkey : MonoBehaviour
{
    public float velocity = 5;
    public float area = 2;
    public float bananas = 0;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Banana"))
        {
            bananas++;
            Destroy(other.gameObject);
        }
    }
}
