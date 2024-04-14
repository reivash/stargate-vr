using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

using static Coordinates;
using static PuzzleRoomGenerator;
using static Constants;
using UnityEditor.Rendering;

public class FloorGenerator : MonoBehaviour {

    // TODO: Can I avoid having this extra boilerplate code and just call the function? Is using static bad?
    public GameObject dungeonSpacePrefab;
    public GameObject puzzleGeneratorGameObject;
    public GameObject stargateGeneratorGameObject;
    public GameObject weaponRoomGeneratorGameObject;
    public GameObject monsterRoomGeneratorGameObject;
    public GameObject roomDecoratorGameObject;
    private PuzzleRoomGenerator puzzleGenerator;
    private StargateRoomGenerator stargateGenerator;
    private WeaponRoomGenerator weaponRoomGenerator;
    private MonsterRoomGenerator monsterRoomGenerator;
    private RoomDecorator roomDecorator;

    private int numRooms;
    private List<Room> roomsList = new List<Room>();
    private Room[,] roomsMatrix = new Room[DUNGEON_SIZE, DUNGEON_SIZE];
    private int numPuzzleRooms = 0;
    private int numWeaponRooms = 0;
    private int numMonsterRooms = 0;

    class Room {
        public enum Type { EMPTY, PUZZLE, START, END };
        public Type type;

        // Dungeon index for the logical map of dungeon rooms. These are not coordinates in the real world.
        public int dx, dz;

        public Room(int dx, int dz, Type type, Room[,] roomsMatrix) {
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
    }

    public void Awake() {
        puzzleGenerator = puzzleGeneratorGameObject.GetComponent<PuzzleRoomGenerator>();
        if (puzzleGenerator == null) {
            throw new System.Exception("Could not find the PuzzleGenerator");
        }
        stargateGenerator = stargateGeneratorGameObject.GetComponent<StargateRoomGenerator>();
        if (puzzleGenerator == null) {
            throw new System.Exception("Could not find the StargateGenerator");
        }
        weaponRoomGenerator = weaponRoomGeneratorGameObject.GetComponent<WeaponRoomGenerator>();
        if (weaponRoomGenerator == null) {
            throw new System.Exception("Could not find the WeaponRoomGenerator");
        }
        monsterRoomGenerator = monsterRoomGeneratorGameObject.GetComponent<MonsterRoomGenerator>();
        if (monsterRoomGenerator == null) {
            throw new System.Exception("Could not find the MonsterRoomGenerator");
        }
        roomDecorator = roomDecoratorGameObject.GetComponent<RoomDecorator>();
        if (roomDecorator == null) {
            throw new System.Exception("Could not find the RoomDecorator");
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
        roomsList.Add(new Room(HALF_DUNGEON_SIZE, HALF_DUNGEON_SIZE, Room.Type.START, roomsMatrix));

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
                , Room.Type.EMPTY, roomsMatrix));
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
    void DestroyChildGameObject(GameObject gameObject, string childName) {
        Transform westDoor = gameObject.transform.Find(childName);
        if (westDoor != null) {
            UnityEngine.Object.DestroyImmediate(westDoor.gameObject);
            print($"GameObject destroyed! ({childName})");
        } else {
            throw new System.Exception("Could not find " + childName);
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
            List<CardinalDirection> adjacentRoomsDirections = new List<CardinalDirection>();
            if (roomsMatrix[room.dx - 1, room.dz - 0] != null) { DestroyChildGameObject(newRoom, "WestWall/WestDoorFrame/WestDoor"); adjacentRoomsDirections.Add(CardinalDirection.West); }
            if (roomsMatrix[room.dx - 0, room.dz - 1] != null) { DestroyChildGameObject(newRoom, "SouthWall/SouthDoorFrame/SouthDoor"); adjacentRoomsDirections.Add(CardinalDirection.South); }
            if (roomsMatrix[room.dx + 1, room.dz + 0] != null) { DestroyChildGameObject(newRoom, "EastWall/EastDoorFrame/EastDoor"); adjacentRoomsDirections.Add(CardinalDirection.East); }
            if (roomsMatrix[room.dx + 0, room.dz + 1] != null) { DestroyChildGameObject(newRoom, "NorthWall/NorthDoorFrame/NorthDoor"); adjacentRoomsDirections.Add(CardinalDirection.North); }

            if (room.type == Room.Type.START) {
                roomDecorator.DecorateRoom(room.dx, room.dz);
                continue;
            }

            if (room.type == Room.Type.END) {
                stargateGenerator.SpawnStargate(room.dx, room.dz);
                roomDecorator.DecorateRoom(room.dx, room.dz);
                continue;
            }

            // If room is not START or END and has two open doors, set as PUZZLE.
            if (adjacentRoomsDirections.Count == 2 && numPuzzleRooms < PUZZLE_ROOMS) {
                puzzleGenerator.GenerateRoom(adjacentRoomsDirections[0], adjacentRoomsDirections[1], room.dx, DUNGEON_FLOOR_Y, room.dz);
                numPuzzleRooms++;
            } else if (numWeaponRooms < WEAPON_ROOMS) {
                weaponRoomGenerator.GenerateRoom(room.dx, room.dz);
                numWeaponRooms++;
                roomDecorator.DecorateRoom(room.dx, room.dz);
            } else if (numMonsterRooms < MONSTER_ROOMS) {
                monsterRoomGenerator.GenerateRoom(room.dx, room.dz);
                numMonsterRooms++;
                roomDecorator.DecorateRoom(room.dx, room.dz);
            }

        }
    }

    void Start() {
        print("Start FloorGenerator!");
        GenerateFloorMap();
        GenerateGameObjects();
        print("End FloorGenerator!");
    }
}
