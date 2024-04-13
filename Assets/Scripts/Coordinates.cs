using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Constants;

public class Coordinates
{

    public static int GetDungeonIndexFromTransformValue(float transform_value) {
        return (int)transform_value / DUNGEON_ROOM_SIZE + HALF_DUNGEON_SIZE;
    }

    public static float GetTransformValueFromDungeonIndex(int dungeon_index) {
        return (dungeon_index - HALF_DUNGEON_SIZE) * DUNGEON_ROOM_SIZE;
    }

}
