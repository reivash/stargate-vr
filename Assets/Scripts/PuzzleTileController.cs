using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PuzzleTileController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public enum TILE_TYPE { GOOD, BAD, START, END };
    public TILE_TYPE type;
    private Color defaultColor;
    public GameObject gameOverCameraOverlay;
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultColor = meshRenderer.material.color;
        if (type == TILE_TYPE.START || type == TILE_TYPE.END) {
            meshRenderer.material.color = Color.yellow;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            if (type == TILE_TYPE.GOOD)
            {
                meshRenderer.material.color = Color.green;
            }
            else if (type == TILE_TYPE.BAD) {
                meshRenderer.material.color = Color.red;
                // Trigger Game Over in 200ms.
                Invoke(nameof(TriggerGameOver), .2f);
            }
        }
    }

    void TriggerGameOver() {
        GameObject gameOverOverlay = Instantiate(gameOverCameraOverlay);
        Canvas canvas = gameOverOverlay.GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindFirstObjectByType<Camera>();
        audioSource.Play();

        // TODO: Stop player movement.
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == TILE_TYPE.BAD)
        {
            //meshRenderer.material.color = defaultColor;
        }

    }
}
