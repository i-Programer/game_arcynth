using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    [Header("Dungeon Size (room grid)")]
    public int width = 18;
    public int height = 18;
    public int maxRooms = 10;

    [Header("Room Tile Size")]
    public int roomTilesWidth = 18;
    public int roomTilesHeight = 18;

    [Header("References")]
    public Transform dungeonRoot;
    public GameObject playerPrefab;

    private GameObject[,] grid;
    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();
    private GameObject startRoom;

    void Start()
    {
        grid = new GameObject[width, height];
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Vector2Int startPos = new Vector2Int(width / 2, height / 2);
        SpawnRoom(startPos, out startRoom);

        int roomsSpawned = 1;

        while (roomsSpawned < maxRooms)
        {
            Vector2Int basePos = occupiedPositions[Random.Range(0, occupiedPositions.Count)];
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int newPos = basePos + dir;
                if (!IsValidPosition(newPos)) continue;

                SpawnRoom(newPos);
                roomsSpawned++;

                if (roomsSpawned >= maxRooms)
                    break;
            }
        }

        SetupDoors();
        CenterDungeon();

        // Spawn player in the start room
        GameObject playerInstance = Instantiate(
            playerPrefab,
            startRoom.transform.position,
            Quaternion.identity
        );

        Camera.main.GetComponent<CameraFollow>().player = playerInstance.transform;
    }

    void SpawnRoom(Vector2Int gridPos, out GameObject spawnedRoom)
    {
        int prefabIndex = Random.Range(0, roomPrefabs.Length);

        Vector3 worldPos = new Vector3(
            gridPos.x * roomTilesWidth,
            gridPos.y * roomTilesHeight,
            0
        );

        spawnedRoom = Instantiate(
            roomPrefabs[prefabIndex],
            worldPos,
            Quaternion.identity,
            dungeonRoot
        );

        grid[gridPos.x, gridPos.y] = spawnedRoom;
        occupiedPositions.Add(gridPos);
    }

    void SpawnRoom(Vector2Int gridPos)
    {
        SpawnRoom(gridPos, out _);
    }

    bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width
            && pos.y >= 0 && pos.y < height
            && grid[pos.x, pos.y] == null;
    }

    void SetupDoors()
    {
        // First pass: turn everything OFF once
        foreach (Vector2Int pos in occupiedPositions)
        {
            Room r = grid[pos.x, pos.y].GetComponent<Room>();
            r.topDoor.SetActive(false);
            r.bottomDoor.SetActive(false);
            r.leftDoor.SetActive(false);
            r.rightDoor.SetActive(false);
        }

        // Second pass: create connections ONCE
        foreach (Vector2Int pos in occupiedPositions)
        {
            Room room = grid[pos.x, pos.y].GetComponent<Room>();

            // RIGHT connection
            Vector2Int right = pos + Vector2Int.right;
            if (IsInsideGrid(right) && grid[right.x, right.y] != null)
            {
                room.rightDoor.SetActive(true);
                grid[right.x, right.y].GetComponent<Room>().leftDoor.SetActive(true);
            }

            // UP connection
            Vector2Int up = pos + Vector2Int.up;
            if (IsInsideGrid(up) && grid[up.x, up.y] != null)
            {
                room.topDoor.SetActive(true);
                grid[up.x, up.y].GetComponent<Room>().bottomDoor.SetActive(true);
            }
        }
    }

    bool IsInsideGrid(Vector2Int p)
    {
        return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
    }



    void CenterDungeon()
    {
        Vector3 offset = -startRoom.transform.position;
        offset.z = 0;
        dungeonRoot.position += offset;
    }
}
