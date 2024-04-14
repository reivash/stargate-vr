using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coordinates;

public class RoomDecorator : MonoBehaviour {
    public GameObject crates;

    public void DecorateRoom(int dungeonX, int dungeonZ) {
        if (Random.value > .5) {
            float x = GetTransformValueFromDungeonIndex(dungeonX);
            float z = GetTransformValueFromDungeonIndex(dungeonZ);
            Instantiate(crates, new Vector3(x, 0, z), Quaternion.identity);
        }
    }
}
