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
        InstantiateConeLight(x, z);
    }

    private void InstantiateConeLight(float x, float z) {
        GameObject lightGameObject = new GameObject("WeaponRoomConeLight");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.type = LightType.Spot;
        lightGameObject.transform.position = new Vector3(x, 3, z);
        lightGameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        lightComp.range = 5;
        lightComp.intensity = 15;
    }
}
