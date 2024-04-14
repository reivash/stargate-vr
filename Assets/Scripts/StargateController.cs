using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StargateController : MonoBehaviour {

    public GameObject youWinCameraOverlay;
    private AudioSource winningMusic;

    private void Awake() {
        winningMusic = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            print("You reached the Stargate!");
            GameObject gameOverOverlay = Instantiate(youWinCameraOverlay);
            Canvas canvas = gameOverOverlay.GetComponent<Canvas>();
            canvas.worldCamera = GameObject.FindFirstObjectByType<Camera>();
            winningMusic.Play();
            // TODO: Stop player movement. Maybe the world as well.
        }
    }
}
