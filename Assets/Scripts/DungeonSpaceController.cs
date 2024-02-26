using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpace : MonoBehaviour {
    
    public GameObject dungeonSpacePrefab;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            print("Player entered the Dungeon");
            ExpandDungeon();
            this.enabled = false;
        }
    }
    void ExpandDungeon() {
        Instantiate(dungeonSpacePrefab, new Vector3(10, 10, 10), Quaternion.identity);
    }
}
