using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class DungeonSpace : MonoBehaviour
{

    public GameObject dungeonSpacePrefab;

    public GameObject goodTilePrefab;
    // public GameObject badTilePrefab;
    // public GameObject startTilePrefab;
    // public GameObject endTilePrefab;

    public static int DUNGEON_SIZE = 1024;
    public static int HALF_DUNGEON_SIZE = DUNGEON_SIZE >> 1;
    public static int DUNGEON_ROOM_SIZE = 5;
    public static bool[,] matrix = new bool[DUNGEON_SIZE, DUNGEON_SIZE];
    public static float DUNGEON_FLOOR_Y = -0.5f;

    // The amount of space between the room wall and the tiles.
    public static float TILES_PUZZLE_PADDING = 2;

    enum TILE_TYPE { UNASSIGNED, PATH_START, PATH_END, PATH, TRAP };

    public static int TILES_SIDE = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            int x = GetDungeonIndexFromTransformValue(transform.position.x);
            int z = GetDungeonIndexFromTransformValue(transform.position.z);
            print($"Player entered the Dungeon room ({x}, {z})");
            ExpandDungeon();
            this.enabled = false;
        }
    }
    void ExpandDungeon()
    {
        int x = GetDungeonIndexFromTransformValue(transform.position.x),
            z = GetDungeonIndexFromTransformValue(transform.position.z);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, z - 1);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, z);
        MaybeInitializeNeighbouringDungeonRoom(x - 1, z + 1);
        MaybeInitializeNeighbouringDungeonRoom(x, z - 1);
        MaybeInitializeNeighbouringDungeonRoom(x, z + 1);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, z - 1);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, z);
        MaybeInitializeNeighbouringDungeonRoom(x + 1, z + 1);
    }
    void MaybeInitializeNeighbouringDungeonRoom(int x, int z)
    {
  
        if (DungeonRoomNeighboursNotYetInitialized(x, z))
        {
            matrix[x, z] = true;
            Instantiate(dungeonSpacePrefab, 
                        new Vector3(GetTransformValueFromDungeonIndex(x), 
                        0, 
                        GetTransformValueFromDungeonIndex(z)), Quaternion.identity);
            CreateFloorPuzzleDungeon(x, DUNGEON_FLOOR_Y, z);
        }
    }

    private void CreateFloorPuzzleDungeon(int dungeonX, float dungeonFloorY, int dungeonZ) {
        // Part 1: Generate maze with one possible path. Mark the rest as traps.
        TILE_TYPE[,] tiles = new TILE_TYPE[TILES_SIDE, TILES_SIDE];

        // Initialize area.
        for (int i = 0; i < TILES_SIDE; i++)
        {
            for (int j = 0; j < TILES_SIDE; j++) {
                tiles[i, j] = TILE_TYPE.UNASSIGNED;
            }
        }
        tiles[0, TILES_SIDE / 2] = TILE_TYPE.PATH_START;
        tiles[TILES_SIDE - 1, TILES_SIDE / 2] = TILE_TYPE.PATH_END;

        Stack<(int x, int z)> path = new Stack<(int x, int z)>();
        int x = 0, z = TILES_SIDE / 2;

        List<(int x, int z)> moveOptions = new List<(int x, int z)>();
        while (true) {
            // If final cell, break.
            if (tiles[x, z] == TILE_TYPE.PATH_END) {
                print("Final tile reached");
                break;
            }

            // Compute move options at current position.
            if (0 < x)
                if (IsValidTileForPath(tiles[x - 1, z]))
                    moveOptions.Add((x - 1, z));
            if (0 < z)
                if (IsValidTileForPath(tiles[x, z - 1]))
                    moveOptions.Add((x, z - 1));
            if (x < (TILES_SIDE - 1))
                if (IsValidTileForPath(tiles[x + 1, z ]))
                    moveOptions.Add((x + 1, z));
            if (z < (TILES_SIDE - 1))
                if (IsValidTileForPath(tiles[x, z + 1]))
                    moveOptions.Add((x, z + 1));

            // If no more move options, check cell as TRAP. Go back.
            if (moveOptions.Count == 0)
            {
                tiles[x, z] = TILE_TYPE.TRAP;

                if (path.Count == 0)
                {
                    print("Something went wrong.");
                    break;
                }

                (x, z) = path.Pop();
            } else { 
                // If move options, check cell as path and mark as path.
                if (tiles[x, z] == TILE_TYPE.UNASSIGNED)
                    tiles[x, z] = TILE_TYPE.PATH;
                path.Push((x, z));
                int randomNumber = Random.Range(0, moveOptions.Count);
                (x, z) = moveOptions[randomNumber];
                moveOptions.Clear();
            }
        }

        
        // Mark every unassigned as trap.
        for (int i = 0; i < TILES_SIDE; i++)
        {
            for (int j = 0; j < TILES_SIDE; j++)
            {
                if (tiles[i, j] == TILE_TYPE.UNASSIGNED)
                    tiles[i, j] = TILE_TYPE.TRAP;
            }
        }

        // Debug: print
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < TILES_SIDE; i++)
        {
            for (int j = 0; j < TILES_SIDE; j++)
            {
                switch (tiles[i, j])
                {
                    case TILE_TYPE.TRAP:
                        stringBuilder.Append("X");
                        break;
                    case TILE_TYPE.PATH:
                        stringBuilder.Append("O");
                        break;
                    case TILE_TYPE.PATH_START:
                        stringBuilder.Append("S");
                        break;
                    case TILE_TYPE.PATH_END:
                        stringBuilder.Append("E");
                        break;
                    default:
                        stringBuilder.Append("?");
                        break;
                }
            }
            stringBuilder.Append("\n");
        }
        print(stringBuilder.ToString());

        // Part 2: Generate GameObjects

        float TILES_PUZZLE_SIDE_SIZE = DUNGEON_ROOM_SIZE - TILES_PUZZLE_PADDING;
        float TILES_SIDE_SIZE = TILES_PUZZLE_SIDE_SIZE / TILES_SIDE;
        float HALF_TILES_SIDE_SIZE = TILES_SIDE_SIZE / 2;

        float topLeftTileCenterX = GetTransformValueFromDungeonIndex(dungeonX) 
                                    - (TILES_PUZZLE_SIDE_SIZE / 2)
                                    + HALF_TILES_SIDE_SIZE;
        float topLeftTileCenterZ = GetTransformValueFromDungeonIndex(dungeonZ) 
                                    - (TILES_PUZZLE_SIDE_SIZE / 2)
                                    + HALF_TILES_SIDE_SIZE;

        for (int tileIndexX = 0; tileIndexX < TILES_SIDE; tileIndexX++) {
            for (int tileIndexZ = 0; tileIndexZ < TILES_SIDE; tileIndexZ++) {
                print($"Spawning tile at: {topLeftTileCenterX + tileIndexX} {topLeftTileCenterZ + tileIndexZ}");
                Instantiate(goodTilePrefab,
                            new Vector3(
                                topLeftTileCenterX + (tileIndexX * TILES_SIDE_SIZE),
                                dungeonFloorY, 
                                topLeftTileCenterZ + (tileIndexZ * TILES_SIDE_SIZE)),
                                Quaternion.identity);
            }
        }

        // Go through all tiles to generate from that center


        // Prototype: Spawn a cube in each area of the space, being 0, 0 of the cube the center.
        // new Vector3(GetTransformValueFromDungeonIndex(x), 
        //                 0, 
        //                 GetTransformValueFromDungeonIndex(z));

    }

    bool IsValidTileForPath(TILE_TYPE tileType) {
        return tileType == TILE_TYPE.UNASSIGNED || tileType == TILE_TYPE.PATH_END;
    }

    int GetDungeonIndexFromTransformValue(float transform_value)
    {
        return (int)transform_value / DUNGEON_ROOM_SIZE + HALF_DUNGEON_SIZE;
    }

    float GetTransformValueFromDungeonIndex(int dungeon_index)
    {
        return (dungeon_index - HALF_DUNGEON_SIZE) * DUNGEON_ROOM_SIZE;
    }

    bool DungeonRoomNeighboursNotYetInitialized(int x, int z)
    {
        if (x < 0 || z < 0 || x >= DUNGEON_SIZE || z >= DUNGEON_SIZE)
        {
            print($"Dungeon limit reached ({x}, {z})");
            return false;
        }

        return !matrix[x, z];
    }
}
