using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodTileCollider : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            meshRenderer.material.color = Color.green;
        }
    }
}
