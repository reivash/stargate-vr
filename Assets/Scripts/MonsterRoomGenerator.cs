using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coordinates;

public class MonsterRoomGenerator : RoomGenerator {
    
    public GameObject monsterPrefab;

    public override void GenerateRoom(int dungeonX, int dungeonZ) {
        float x = GetTransformValueFromDungeonIndex(dungeonX);
        float z = GetTransformValueFromDungeonIndex(dungeonZ);
        Instantiate(monsterPrefab, new Vector3(x, 1, z), Quaternion.identity);
    }
}
