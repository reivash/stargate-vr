using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StickController : MonoBehaviour
{
    public int stickDamage = 4;


    private void Awake()
    {
        this.enabled = false;
        XRGrabInteractable xRGrabInteractable = GetComponent<XRGrabInteractable>();
        xRGrabInteractable.selectEntered.AddListener(OnGrabbed);
        xRGrabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs arg)
    {
        this.enabled = true;
    }

    private void OnReleased(SelectExitEventArgs arg)
    {
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.name == "ChaserCapsule")
        {
            ChaserAI script = GameObject.Find("ChaserCapsule").GetComponent<ChaserAI>();
            script.TakeDamage(stickDamage);
        }
    }
}
