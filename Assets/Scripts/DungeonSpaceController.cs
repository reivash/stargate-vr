using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DungeonSpace : MonoBehaviour
{

    public GameObject dungeonSpacePrefab;

    public static int DUNGEON_SIZE = 1024;
    public static int HALF_DUNGEON_SIZE = DUNGEON_SIZE >> 1;
    public static int DUNGEON_ROOM_SIZE = 5;
    public static bool[,] matrix = new bool[DUNGEON_SIZE, DUNGEON_SIZE];
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            int x = GetDungeonIndexFromTransformValue(transform.position.x);
            int y = GetDungeonIndexFromTransformValue(transform.position.y);
            print("$Player entered the Dungeon room ({x}, {y})");
            ExpandDungeon();
            this.enabled = false;
        }
    }
    void ExpandDungeon()
    {
        // TODO: Check 8 neighbours for existance.
        // TODO: Prevent out of boundary
        int x = GetDungeonIndexFromTransformValue(transform.position.x),
            y = GetDungeonIndexFromTransformValue(transform.position.y);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, y - 1);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, y);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, y + 1);
        MaybeInitializeNeighbouringDungeonRoom(x, y - 1);
        MaybeInitializeNeighbouringDungeonRoom(x, y + 1);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, y - 1);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, y);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, y + 1);
    }
    void MaybeInitializeNeighbouringDungeonRoom(int x, int y)
    {
  
        if (DungeonRoomNeighboursNotYetInitialized(x, y))
        {
            matrix[x, y] = true;
            Instantiate(dungeonSpacePrefab, 
                        new Vector3(GetTransformValueFromDungeonIndex(x), 
                        0, 
                        GetTransformValueFromDungeonIndex(y)), Quaternion.identity);
        }
    }
    int GetDungeonIndexFromTransformValue(float transform_value)
    {
        return (int)transform_value / DUNGEON_ROOM_SIZE + HALF_DUNGEON_SIZE;
    }

    float GetTransformValueFromDungeonIndex(int dungeon_index)
    {
        return (dungeon_index - HALF_DUNGEON_SIZE) * DUNGEON_ROOM_SIZE;
    }

    bool DungeonRoomNeighboursNotYetInitialized(int x, int y)
    {
        if (x < 0 || y < 0 || x >= DUNGEON_SIZE || y >= DUNGEON_SIZE)
        {
            print($"Dungeon limit reached ({x}, {y})");
            return false;
        }

        return !matrix[x, y];
    }
}
