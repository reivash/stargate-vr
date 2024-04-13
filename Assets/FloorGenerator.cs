using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

using static Constants; 

public class FloorGenerator : MonoBehaviour {

    public GameObject dungeonSpacePrefab;

    private int numRooms;
    private static List<Room> roomsList = new List<Room>();
    private static Room[,] roomsMatrix = new Room[DUNGEON_SIZE, DUNGEON_SIZE];

    class Room {
        public enum Type { EMPTY, PUZZLE, START, END };
        public Type type;

        // Dungeon index for the logical map of dungeon rooms. These are not coordinates in the real world.
        public int dx, dz;

        public Room(int dx, int dz, Type type) {
            this.dx = dx;
            this.dz = dz;

            if (roomsMatrix[dx, dz] != null) {
                throw new System.Exception($"There's already a room at position ({dx}, {dz})");
            }
            roomsMatrix[dx, dz] = this;

            if (type == Type.PUZZLE) {
                throw new System.Exception("Rooms shouldn't be initialized as PUZZLE. The puzzle room can only have two exists and during floor generation we may not know if that'll be true.");
            }
            this.type = type;
        }

        public void GeneratePuzzle() {
            this.type = Type.PUZZLE;
            // TODO: Implement
        }
    }

    bool RoomHasAdjacentSpace(Room room) {
        int dx = room.dx;
        int dz = room.dz;
        if (roomsMatrix[dx - 1, dz - 0] == null) { return true; }
        if (roomsMatrix[dx - 0, dz - 1] == null) { return true; }
        if (roomsMatrix[dx + 1, dz + 0] == null) { return true; }
        if (roomsMatrix[dx + 0, dz + 1] == null) { return true; }
        return false;
    }

    float DistanceToStartRoom(Room room) {
        return Mathf.Sqrt(Mathf.Pow(HALF_DUNGEON_SIZE - room.dx, 2) + Mathf.Pow(HALF_DUNGEON_SIZE - room.dz, 2));
    }

    void PrintDungeonLayout() {
        string dungeon = "";
        for (int x = 0; x < DUNGEON_SIZE; x++) {
            for (int z = 0; z < DUNGEON_SIZE; z++) {
                if (roomsMatrix[x, z] == null) {
                    dungeon += " ";
                } else if (roomsMatrix[x, z].type == Room.Type.START) {
                    dungeon += "S";
                } else if (roomsMatrix[x, z].type == Room.Type.END) {
                    dungeon += "E";
                } else {
                    dungeon += "X";
                }
            }
            dungeon += "\n";
        }
        print(dungeon);
    }

    void GenerateFloorMap() {
        numRooms = MIN_ROOMS + (int)(Random.value * (MAX_ROOMS - MIN_ROOMS));

        // Generate starting room.
        roomsList.Add(new Room(HALF_DUNGEON_SIZE, HALF_DUNGEON_SIZE, Room.Type.START));

        while (roomsList.Count < numRooms) {
            // Take indexes of rooms that have at least one side without adjacent room.
            List<int> roomsWithAdjacentSpaceIndexes = new List<int>();
            for (int i = 0; i < roomsList.Count; i++) {
                if (RoomHasAdjacentSpace(roomsList[i])) {
                    roomsWithAdjacentSpaceIndexes.Add(i);
                }
            }

            // Choose one at random.
            int randomIndex = (int)(Random.value * roomsWithAdjacentSpaceIndexes.Count);
            Room roomWithAdjacentSpace = roomsList[roomsWithAdjacentSpaceIndexes[randomIndex]];

            // Check which sides don't have yet a room.
            List<(int, int)> sides = new List<(int, int)>();
            int dx = roomWithAdjacentSpace.dx;
            int dz = roomWithAdjacentSpace.dz;
            if (roomsMatrix[dx - 1, dz - 0] == null) { sides.Add((dx - 1, dz - 0)); }
            if (roomsMatrix[dx - 0, dz - 1] == null) { sides.Add((dx - 0, dz - 1)); }
            if (roomsMatrix[dx + 1, dz + 0] == null) { sides.Add((dx + 1, dz + 0)); }
            if (roomsMatrix[dx + 0, dz + 1] == null) { sides.Add((dx + 0, dz + 1)); }

            // Choose one at random.
            (int, int) adjacentSpaceDungeonIndexes = sides[(int)(Random.value * sides.Count)];

            // Add new room.
            roomsList.Add(new Room(adjacentSpaceDungeonIndexes.Item1, adjacentSpaceDungeonIndexes.Item2
                , Room.Type.EMPTY));
        }

        // Choose end room as the furthest one.
        float maxDistanceToStartRoom = 0;
        int idx = -1;
        for (int i = 0; i < roomsList.Count; i++) {
            float distance = DistanceToStartRoom(roomsList[i]);
            if (distance > maxDistanceToStartRoom) {
                maxDistanceToStartRoom = distance;
                idx = i;
            }
        }

        roomsList[idx].type = Room.Type.END;
        PrintDungeonLayout();
    }
    void MaybeDestroyChildGameObject(GameObject gameObject, string childName) {
        Transform westDoor = gameObject.transform.Find(childName);
        if (westDoor != null) {
            UnityEngine.Object.DestroyImmediate(westDoor.gameObject);
            print($"GameObject destroyed! ({childName})");
        } else {
            print("Could not find " + childName);
        }
    }
    void GenerateGameObjects() {
        for (int i = 0; i < roomsList.Count; i++) {
            Room room = roomsList[i];
            GameObject newRoom = Instantiate(dungeonSpacePrefab,
                        new Vector3(GetTransformValueFromDungeonIndex(room.dx),
                        0,
                        GetTransformValueFromDungeonIndex(room.dz)), Quaternion.identity);

            // Connect adjacent rooms.
            if (roomsMatrix[room.dx - 1, room.dz - 0] != null) { MaybeDestroyChildGameObject(newRoom, "WestWall/WestDoorFrame/WestDoor"); }
            if (roomsMatrix[room.dx - 0, room.dz - 1] != null) { MaybeDestroyChildGameObject(newRoom, "SouthWall/SouthDoorFrame/SouthDoor"); }
            if (roomsMatrix[room.dx + 1, room.dz + 0] != null) { MaybeDestroyChildGameObject(newRoom, "EastWall/EastDoorFrame/EastDoor"); }
            if (roomsMatrix[room.dx + 0, room.dz + 1] != null) { MaybeDestroyChildGameObject(newRoom, "NorthWall/NorthDoorFrame/NorthDoor"); }
        }
    }

    void Start() {
        print("Start FloorGenerator!");
        GenerateFloorMap();
        GenerateGameObjects();
        print("End FloorGenerator!");
    }
}
