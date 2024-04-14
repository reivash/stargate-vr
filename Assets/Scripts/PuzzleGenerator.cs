using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using static Coordinates;
using static Constants;

// TODO: Rename to HiddenPath or something more descritive.
public class PuzzleGenerator : MonoBehaviour {
    
    public GameObject tilePrefab;

    // The amount of space between the room wall and the tiles.
    private static float TILES_PUZZLE_PADDING = 0.5f;
    private static int TILES_SIDE = 5;

    private enum TILE_TYPE { UNASSIGNED, PATH_START, PATH_END, PATH, TRAP };

    private static bool IsValidTileForPath(TILE_TYPE tileType) {
        return tileType == TILE_TYPE.UNASSIGNED || tileType == TILE_TYPE.PATH_END;
    }

    private static PuzzleTileController.TILE_TYPE GetTileType(TILE_TYPE type) {
        switch (type) {
            case TILE_TYPE.TRAP:
                return PuzzleTileController.TILE_TYPE.BAD;
            case TILE_TYPE.PATH:
                return PuzzleTileController.TILE_TYPE.GOOD;
            case TILE_TYPE.PATH_START:
                return PuzzleTileController.TILE_TYPE.START;
            case TILE_TYPE.PATH_END:
                return PuzzleTileController.TILE_TYPE.END;
            default:
                return PuzzleTileController.TILE_TYPE.GOOD;
        }
    }

    public void GenerateRoom(CardinalDirection start, CardinalDirection end, int dungeonX, float dungeonFloorY, int dungeonZ) {
        if (start == end) {
            throw new System.Exception("The puzzle start and end can't be the same");
        }

        // Part 1: Generate maze with one possible path. Mark the rest as traps.
        TILE_TYPE[,] tiles = new TILE_TYPE[TILES_SIDE, TILES_SIDE];

        // Initialize area.
        for (int i = 0; i < TILES_SIDE; i++) {
            for (int j = 0; j < TILES_SIDE; j++) {
                tiles[i, j] = TILE_TYPE.UNASSIGNED;
            }
        }

        // Assign start and end. 
        int x = -1, z = -1;
        if (start == CardinalDirection.North) {
            x = TILES_SIDE / 2;
            z = TILES_SIDE - 1;
        }
        if (start == CardinalDirection.South) {
            x = TILES_SIDE / 2;
            z = 0;
        }
        if (start == CardinalDirection.East) {
            x = TILES_SIDE - 1;
            z = TILES_SIDE / 2;
        }
        if (start == CardinalDirection.West) {
            x = 0;
            z = TILES_SIDE / 2;
        }
        tiles[x, z] = TILE_TYPE.PATH_START;


        if (end == CardinalDirection.North) {
            tiles[TILES_SIDE / 2, TILES_SIDE - 1] = TILE_TYPE.PATH_END;
        }
        if (end == CardinalDirection.South) {
            tiles[TILES_SIDE / 2, 0] = TILE_TYPE.PATH_END;
        }
        if (end == CardinalDirection.East) {
            tiles[TILES_SIDE - 1, TILES_SIDE / 2] = TILE_TYPE.PATH_END;
        }
        if (end == CardinalDirection.West) {
            tiles[0, TILES_SIDE / 2] = TILE_TYPE.PATH_END;
        }


        // Generate tile puzzle.
        Stack<(int x, int z)> path = new Stack<(int x, int z)>();
        List<(int x, int z)> moveOptions = new List<(int x, int z)>();
        while (true) {
            // If final cell, break.
            if (tiles[x, z] == TILE_TYPE.PATH_END) {
                //print("Final tile reached");
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
                if (IsValidTileForPath(tiles[x + 1, z]))
                    moveOptions.Add((x + 1, z));
            if (z < (TILES_SIDE - 1))
                if (IsValidTileForPath(tiles[x, z + 1]))
                    moveOptions.Add((x, z + 1));

            // If no more move options, check cell as TRAP. Go back.
            if (moveOptions.Count == 0) {
                tiles[x, z] = TILE_TYPE.TRAP;

                if (path.Count == 0) {
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
        for (int i = 0; i < TILES_SIDE; i++) {
            for (int j = 0; j < TILES_SIDE; j++) {
                if (tiles[i, j] == TILE_TYPE.UNASSIGNED)
                    tiles[i, j] = TILE_TYPE.TRAP;
            }
        }

        // Debug: print
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < TILES_SIDE; i++) {
            for (int j = 0; j < TILES_SIDE; j++) {
                switch (tiles[i, j]) {
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
        //print(stringBuilder.ToString());

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
                //print($"Spawning tile at: {topLeftTileCenterX + tileIndexX} {topLeftTileCenterZ + tileIndexZ}");
                GameObject tile = Instantiate(tilePrefab,
                            new Vector3(
                                topLeftTileCenterX + (tileIndexX * TILES_SIDE_SIZE),
                                dungeonFloorY,
                                topLeftTileCenterZ + (tileIndexZ * TILES_SIDE_SIZE)),
                                Quaternion.identity);
                tile.GetComponent<PuzzleTileController>().type = GetTileType(tiles[tileIndexX, tileIndexZ]);
            }
        }

        // Go through all tiles to generate from that center


        // Prototype: Spawn a cube in each area of the space, being 0, 0 of the cube the center.
        // new Vector3(GetTransformValueFromDungeonIndex(x), 
        //                 0, 
        //                 GetTransformValueFromDungeonIndex(z));
    }

}
