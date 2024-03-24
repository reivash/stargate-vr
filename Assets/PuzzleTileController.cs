using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodTileCollider : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public enum TILE_TYPE { GOOD, BAD, START, END };
    public TILE_TYPE type;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
                meshRenderer.material.color = Color.green;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (type == TILE_TYPE.BAD)
        {
            meshRenderer.material.color = Color.clear;
        }

    }
}
