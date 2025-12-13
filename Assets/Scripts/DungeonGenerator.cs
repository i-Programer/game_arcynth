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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject roomObj = grid[x, y];
                if (roomObj == null) continue;

                Room room = roomObj.GetComponent<Room>();

                // Reset everything first
                room.topDoor.SetActive(false);
                room.bottomDoor.SetActive(false);
                room.leftDoor.SetActive(false);
                room.rightDoor.SetActive(false);

                // UP
                if (y + 1 < height && grid[x, y + 1] != null)
                {
                    room.topDoor.SetActive(true);
                    grid[x, y + 1].GetComponent<Room>().bottomDoor.SetActive(true);
                }

                // DOWN
                if (y - 1 >= 0 && grid[x, y - 1] != null)
                {
                    room.bottomDoor.SetActive(true);
                    grid[x, y - 1].GetComponent<Room>().topDoor.SetActive(true);
                }

                // RIGHT
                if (x + 1 < width && grid[x + 1, y] != null)
                {
                    room.rightDoor.SetActive(true);
                    grid[x + 1, y].GetComponent<Room>().leftDoor.SetActive(true);
                }

                // LEFT
                if (x - 1 >= 0 && grid[x - 1, y] != null)
                {
                    room.leftDoor.SetActive(true);
                    grid[x - 1, y].GetComponent<Room>().rightDoor.SetActive(true);
                }
            }
        }
    }


    void CenterDungeon()
    {
        Vector3 offset = -startRoom.transform.position;
        offset.z = 0;
        dungeonRoot.position += offset;
    }
}
