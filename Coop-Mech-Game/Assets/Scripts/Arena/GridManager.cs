using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Header("Tile Animation Settings")]
    [SerializeField][Range(0,100)] private int tileAnimationStartHeightMin = 10;
    [SerializeField][Range(0,100)] private int tileAnimationStartHeightMax = 30;
    [SerializeField] private float tileAnimationDuration;
    [Header("DEBUG")]
    [SerializeField] private bool startUpTilesOnStart;
            
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

        if (p1SpawnTileIndex == p2SpawnTileIndex)
        {
            Debug.LogError($"P1 and P2 spawn tile indices cannot be the same. Both are set to {p1SpawnTileIndex}. Please set different spawn tile indices for P1 and P2.");
            return;
        }

        for (int i = 0; i < gridSize.y; i++)
        {
            List<GameObject> row = new List<GameObject>();

            for (int j = 0; j < gridSize.x; j++)
            {
                GameObject tile;
                if(i == p1SpawnTileIndex.y && j == p1SpawnTileIndex.x)
                {
                    tile = Instantiate(tilesList[14], transform);
                }
                else if(i == p2SpawnTileIndex.y && j == p2SpawnTileIndex.x)
                {
                    tile = Instantiate(tilesList[15], transform);
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, tilesList.Count);

                    tile = Instantiate(tilesList[randomIndex], transform);
                }

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
        if (startUpTilesOnStart) //Debug code to test tile animation without having to start a match
        {
            StartUpTiles();
        }
    }

    public void StartUpTiles()
    {
        foreach (List<GameObject> row in tilesGrid2D)
        {
            foreach (GameObject tile in row)
            {
                Vector3 position = tile.transform.position;
                position.y -= UnityEngine.Random.Range(tileAnimationStartHeightMin, tileAnimationStartHeightMax);
                tile.transform.position = position;
            }
        }
        AnimateTiles();
    }

    private void AnimateTiles()
    {
        foreach (List<GameObject> row in tilesGrid2D)
        {
            foreach (GameObject tile in row)
            {
                StartCoroutine(AnimateSingleTile(tile));
            }
        }
    }

    private IEnumerator AnimateSingleTile(GameObject tile)
    {
        Vector3 targetPosition = tile.transform.position;
        targetPosition.y = gridOrigin.y;

        while (Math.Sqrt(Math.Pow(tile.transform.position.y - gridOrigin.y, 2)) > 0.01f)
        {
            tile.transform.position = Vector3.Lerp(tile.transform.position, targetPosition, tileAnimationDuration * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    public void ClearGrid()
    {
        foreach (List<GameObject> row in tilesGrid2D)
        {
            foreach (GameObject tile in row)
            {
                Destroy(tile);
            }
        }
        tilesGrid2D.Clear();
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