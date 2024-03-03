using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    public int stickDamage = 4;

    private void OnTriggerEnter(Collider other)
    {
        print("Entered trigger: " + other.name);
        if (other.name == "ChaserCapsule")
        {
            ChaserAI script = GameObject.Find("ChaserCapsule").GetComponent<ChaserAI>();
            script.TakeDamage(stickDamage);
        }
    }
}
