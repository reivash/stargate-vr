using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class FloorGenerator : MonoBehaviour {

    private const int MIN_ROOMS = 5;
    private const int MAX_ROOMS = 8;

    private int numRooms;
    private Room[] rooms = new Room[MAX_ROOMS];

    class Room {
        int x, z;
        enum Type { EMPTY, PUZZLE };
        Type type;
    }


    void Start() {
        numRooms = MIN_ROOMS + (int)(Random.value * (MAX_ROOMS - MIN_ROOMS));
        print("Number of rooms: " +  rooms.Length);
    }
}
