using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coordinates;

public class RoomDecorator : MonoBehaviour {
    public GameObject crates;
    public GameObject rock;

    public void DecorateRoom(int dungeonX, int dungeonZ) {
        float x = GetTransformValueFromDungeonIndex(dungeonX);
        float z = GetTransformValueFromDungeonIndex(dungeonZ);

        // Randomize props.
        if (Random.value > .5) {
            Instantiate(crates, new Vector3(x, 0, z), Quaternion.AngleAxis(90 * (int)(Random.value * 4), Vector3.up));
        } else if (Random.value > 0) {
            Instantiate(rock, new Vector3(x, 0, z), Quaternion.AngleAxis(90 * (int)(Random.value * 4), Vector3.up));
        }

        // TODO: Randomize lights.
    }
}
