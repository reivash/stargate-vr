using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coordinates;

public class WeaponRoomGenerator : RoomGenerator
{
    public GameObject weaponPrefab;

    public override void GenerateRoom(int dungeonX, int dungeonZ)  {
        float x = GetTransformValueFromDungeonIndex(dungeonX);
        float z = GetTransformValueFromDungeonIndex(dungeonZ);
        Instantiate(weaponPrefab, new Vector3(x, 1, z), Quaternion.identity);
    }
}
