using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private List<GameObject> tilesList;
    private List<List<GameObject>> tilesGrid2D = new List<List<GameObject>>();

    [SerializeField] private Vector2 gridSize = new Vector2(4, 4);
    [Tooltip("X and Z dimensions of the tile")]
    [SerializeField] private Vector2 tileDimensions2D = new Vector2(50f, 50f); //X and Z dimensions of the tile
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;
    [SerializeField] private Vector2 p1SpawnTileIndex = new Vector2(0, 0);
    [SerializeField] private Vector2 p2SpawnTileIndex = new Vector2(0, 0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateGrid();
    }

    /// <summary>
    /// Creates a grid of tiles based on the specified grid size and tile dimensions. Each tile is randomly selected from the provided list of tile prefabs and instantiated at the appropriate position in the grid.
    /// </summary>
    public void CreateGrid()
    {
        tilesGrid2D.Clear();

        for (int i = 0; i < gridSize.y; i++)
        {
            List<GameObject> row = new List<GameObject>();

            for (int j = 0; j < gridSize.x; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, tilesList.Count);

                GameObject tile = Instantiate(tilesList[randomIndex], transform);

                Vector3 position = new Vector3(
                    gridOrigin.x + (j * tileDimensions2D.x),
                    gridOrigin.y,
                    gridOrigin.z - (i * tileDimensions2D.y)
                );

                tile.transform.localPosition = position;

                row.Add(tile);
            }

            tilesGrid2D.Add(row);
        }

        SpawnTiles();
    }

    private void SpawnTiles()
    {
        // Spawn P1 tile
        if (IsValidTileIndex(p1SpawnTileIndex))
        {
            GameObject p1Tile = tilesGrid2D[(int)p1SpawnTileIndex.y][(int)p1SpawnTileIndex.x];
            // Add code to spawn P1 on p1Tile
        }
        else
        {
            Debug.LogWarning($"P1 spawn tile index {p1SpawnTileIndex} is invalid. No tile will be spawned for P1.");
        }

        // Spawn P2 tile
        if (IsValidTileIndex(p2SpawnTileIndex))
        {
            GameObject p2Tile = tilesGrid2D[(int)p2SpawnTileIndex.y][(int)p2SpawnTileIndex.x];
            // Add code to spawn P2 on p2Tile
        }
        else
        {
            Debug.LogWarning($"P2 spawn tile index {p2SpawnTileIndex} is invalid. No tile will be spawned for P2.");
        }
    }

    private bool IsValidTileIndex(Vector2 index)
    {
        if (index.x < 0 || index.y < 0 || index.x >= gridSize.x || index.y >= gridSize.y)
        {
            return false;
        }
        int row = Mathf.FloorToInt(index.y);
        int col = Mathf.FloorToInt(index.x);
        if (tilesGrid2D.Count == 0 || row >= tilesGrid2D.Count || col >= tilesGrid2D[row].Count)
        {
            return false;
        }
        return true;
    }

    public void SetP1SpawnTileIndex(Vector2 index)
    {
        // Check if index is within gridSize bounds
        if (index.x < 0 || index.y < 0 || index.x >= gridSize.x || index.y >= gridSize.y)
        {
            Debug.LogWarning($"P1 spawn tile index {index} is out of grid size bounds {gridSize}.");
            return;
        }
        // Check if tilesGrid2D is initialized and index is within the list bounds
        int row = Mathf.FloorToInt(index.y);
        int col = Mathf.FloorToInt(index.x);
        if (tilesGrid2D.Count == 0 || row >= tilesGrid2D.Count || col >= tilesGrid2D[row].Count)
        {
            Debug.LogWarning($"P1 spawn tile index {index} is out of tilesGrid2D bounds.");
            return;
        }
        p1SpawnTileIndex = index;
    }
    public void SetP2SpawnTileIndex(Vector2 index)
    {
        // Check if index is within gridSize bounds
        if (index.x < 0 || index.y < 0 || index.x >= gridSize.x || index.y >= gridSize.y)
        {
            Debug.LogWarning($"P2 spawn tile index {index} is out of grid size bounds {gridSize}.");
            return;
        }
        // Check if tilesGrid2D is initialized and index is within the list bounds
        int row = Mathf.FloorToInt(index.y);
        int col = Mathf.FloorToInt(index.x);
        if (tilesGrid2D.Count == 0 || row >= tilesGrid2D.Count || col >= tilesGrid2D[row].Count)
        {
            Debug.LogWarning($"P2 spawn tile index {index} is out of tilesGrid2D bounds.");
            return;
        }
        p2SpawnTileIndex = index;
    }
}