using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomGenerator : MonoBehaviour {

    /** Generates the room for this particular room type at the logical dungeon X and dungeon Z indexes */
    public abstract void GenerateRoom(int dungeonX, int dungeonZ);
}
