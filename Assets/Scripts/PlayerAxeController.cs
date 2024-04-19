using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerAxeController : MonoBehaviour {
    public int stickDamage = 4;

    private AudioSource audioSource;
    private AudioClip unsheatheAudioClip;
    private AudioClip weaponHitAudioClip;


    private void Awake() {
        enabled = false;
        audioSource = GetComponent<AudioSource>();
        unsheatheAudioClip= AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/unsheathe-sword.mp3");
        weaponHitAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/stick-hit.mp3");
        XRGrabInteractable xRGrabInteractable = GetComponent<XRGrabInteractable>();
        xRGrabInteractable.selectEntered.AddListener(OnGrabbed);
        xRGrabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs arg) {
        enabled = true;
        audioSource.PlayOneShot(unsheatheAudioClip);
    }

    private void OnReleased(SelectExitEventArgs arg) {
        enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.name.Contains("Orc")) {
            audioSource.PlayOneShot(weaponHitAudioClip);
            OrcAI script = other.gameObject.GetComponent<OrcAI>();
            Vector3 collisionDirection = transform.position - other.transform.position;
            collisionDirection.Normalize();
            collisionDirection.y = 0;
            script.TakeDamage(stickDamage, collisionDirection);
        }
    }
}
