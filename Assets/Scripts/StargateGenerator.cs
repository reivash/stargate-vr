using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coordinates;
using static Constants;
public class StargateGenerator : MonoBehaviour {

    public GameObject starPrefab;

    public void SpawnStargate(int dungeonX, int dungeonZ) {
        float x = GetTransformValueFromDungeonIndex(dungeonX);
        float z = GetTransformValueFromDungeonIndex(dungeonZ);
        Instantiate(starPrefab, new Vector3(x, DUNGEON_FLOOR_Y + 1, z), Quaternion.LookRotation(Vector3.up));
    }
}
