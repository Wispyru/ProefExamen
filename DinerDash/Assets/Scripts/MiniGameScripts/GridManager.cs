using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central hub of the Match Minigame.
/// Owns the tile grid, executes swaps, resolves matches, applies gravity,
/// triggers spawning, and drives the full cascade loop.
/// Broadcasts OnTilesMatched after each cascade step so RecipeManager can update progress.
/// </summary>
public class GridManager : MonoBehaviour
{

    /// <summary>
    /// Fired after a set of tiles is matched and cleared.
    /// Passes the list of cleared IngredientTypes to RecipeManager.
    /// </summary>
    public UnityEvent<List<IngredientType>> OnTilesMatched = new UnityEvent<List<IngredientType>>();

    /// <summary>
    /// Fired when a swap produces no match and is reverted.
    /// InputHandler listens to this to re-enable input.
    /// </summary>
    public UnityEvent OnSwapReverted = new UnityEvent();

    /// <summary>
    /// Fired at the start of cascade resolution to lock player input.
    /// </summary>
    public UnityEvent OnCascadeStart = new UnityEvent();

    /// <summary>
    /// Fired when cascade resolution is fully complete and the grid is stable.
    /// InputHandler listens to this to unlock player input.
    /// </summary>
    public UnityEvent OnCascadeEnd = new UnityEvent();
    
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private TileSpawner _tileSpawner;
    [SerializeField] private float _tileSize = 1f;
    
    private const int Columns = 8;
    private const int Rows    = 4;

    private Tile[,] _grid;
    private MatchChecker _matchChecker;
    
    private void Awake()
    {
        _matchChecker = new MatchChecker();
        _grid         = new Tile[Rows, Columns];

        Debug.Log($"[GridManager] Awake() — grid initialised as {Rows} rows x {Columns} columns.");
    }

    private void Start()
    {
        BuildGrid();
    }
    
    /// <summary>
    /// Attempts to swap two adjacent tiles.
    /// If the swap produces at least one match the cascade begins.
    /// If not, the swap is reverted and OnSwapReverted is fired.
    /// Called by InputHandler after a valid drag is released.
    /// </summary>
    public void SwapTiles(Vector2Int posA, Vector2Int posB)
    {
        Debug.Log($"[GridManager] SwapTiles() called — swapping {posA} and {posB}.");

        PerformSwap(posA, posB);

        List<Vector2Int> matches = _matchChecker.FindMatches(_grid);

        if (matches.Count == 0)
        {
            Debug.Log("[GridManager] Swap produced no matches — reverting.");
            PerformSwap(posA, posB);
            OnSwapReverted.Invoke();
            return;
        }

        Debug.Log($"[GridManager] Swap valid — {matches.Count} tile(s) matched. Starting cascade.");
        StartCoroutine(ResolveCascade());
    }

    /// <summary>
    /// Returns the Tile at a given grid position, or null if the cell is empty or out of bounds.
    /// </summary>
    public Tile GetTileAt(Vector2Int position)
    {
        if (!IsInBounds(position))
        {
            Debug.LogWarning($"[GridManager] GetTileAt({position}) — position is out of bounds.");
            return null;
        }

        return _grid[position.x, position.y];
    }
    
    /// <summary>
    /// Instantiates and positions all tiles to fill the grid at game start.
    /// </summary>
    private void BuildGrid()
    {
        Debug.Log("[GridManager] BuildGrid() — filling grid with initial tiles.");

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Vector2Int gridPos     = new Vector2Int(row, col);
                Vector3    worldPos    = GridToWorldPosition(gridPos);
                Tile       tile        = SpawnTileAt(gridPos, worldPos);

                _grid[row, col] = tile;
            }
        }

        Debug.Log("[GridManager] BuildGrid() — complete. All cells filled.");
    }

    /// <summary>
    /// Coroutine that drives the full cascade loop.
    /// Clears matches → applies gravity → spawns new tiles → scans again.
    /// Repeats until no new matches are found, then fires OnCascadeEnd.
    /// </summary>
    private IEnumerator ResolveCascade()
    {
        OnCascadeStart.Invoke();
        Debug.Log("[GridManager] ResolveCascade() — cascade loop started.");

        int cascadeStep = 0;

        while (true)
        {
            List<Vector2Int> matches = _matchChecker.FindMatches(_grid);

            if (matches.Count == 0)
            {
                Debug.Log($"[GridManager] ResolveCascade() — no matches found after step {cascadeStep}. Cascade complete.");
                break;
            }

            cascadeStep++;
            Debug.Log($"[GridManager] ResolveCascade() — cascade step {cascadeStep}: clearing {matches.Count} tile(s).");

            List<IngredientType> clearedTypes = ClearMatchedTiles(matches);

            Debug.Log($"[GridManager] Firing OnTilesMatched with {clearedTypes.Count} ingredient(s): {string.Join(", ", clearedTypes)}");
            OnTilesMatched.Invoke(clearedTypes);

            yield return new WaitForSeconds(0.45f);

            ApplyGravity();

            yield return new WaitForSeconds(0.25f);

            FillEmptyCells();

            yield return new WaitForSeconds(0.25f);
        }

        Debug.Log("[GridManager] ResolveCascade() — firing OnCascadeEnd. Input re-enabled.");
        OnCascadeEnd.Invoke();
    }

    /// <summary>
    /// Swaps two tiles in the grid array and updates their world positions and GridPositions.
    /// Used both for the initial swap and for reverting if no match is found.
    /// </summary>
    private void PerformSwap(Vector2Int posA, Vector2Int posB)
    {
        Tile tileA = _grid[posA.x, posA.y];
        Tile tileB = _grid[posB.x, posB.y];

        _grid[posA.x, posA.y] = tileB;
        _grid[posB.x, posB.y] = tileA;

        if (tileA != null)
        {
            tileA.SetGridPosition(posB);
            tileA.PlayFallAnimation(GridToWorldPosition(posB));
        }

        if (tileB != null)
        {
            tileB.SetGridPosition(posA);
            tileB.PlayFallAnimation(GridToWorldPosition(posA));
        }

        Debug.Log($"[GridManager] PerformSwap() — {posA} and {posB} swapped.");
    }

    /// <summary>
    /// Destroys all tiles at the matched positions and clears their cells in the grid array.
    /// Returns a list of the IngredientTypes that were cleared, for RecipeManager.
    /// </summary>
    private List<IngredientType> ClearMatchedTiles(List<Vector2Int> matchedPositions)
    {
        List<IngredientType> clearedTypes = new List<IngredientType>();

        foreach (Vector2Int pos in matchedPositions)
        {
            Tile tile = _grid[pos.x, pos.y];

            if (tile == null)
            {
                Debug.LogWarning($"[GridManager] ClearMatchedTiles() — tile at {pos} is already null, skipping.");
                continue;
            }

            Debug.Log($"[GridManager] Clearing {tile.Type} at {pos}.");
            clearedTypes.Add(tile.Type);
            tile.PlayMatchAnimation();
            _grid[pos.x, pos.y] = null;
        }

        return clearedTypes;
    }

    /// <summary>
    /// Moves tiles downward to fill any empty cells created by cleared matches.
    /// Works column by column from the bottom row upward.
    /// </summary>
    private void ApplyGravity()
    {
        Debug.Log("[GridManager] ApplyGravity() — shifting tiles down.");

        for (int col = 0; col < Columns; col++)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (_grid[row, col] != null)
                    continue;

                for (int above = row - 1; above >= 0; above--)
                {
                    if (_grid[above, col] == null)
                        continue;

                    Tile fallingTile = _grid[above, col];
                    _grid[row, col]  = fallingTile;
                    _grid[above, col] = null;

                    Vector2Int newGridPos = new Vector2Int(row, col);
                    fallingTile.SetGridPosition(newGridPos);
                    fallingTile.PlayFallAnimation(GridToWorldPosition(newGridPos));

                    Debug.Log($"[GridManager] ApplyGravity() — {fallingTile.Type} fell from row {above} to row {row} in column {col}.");
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Asks TileSpawner to fill any remaining empty cells after gravity has resolved.
    /// </summary>
    private void FillEmptyCells()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (_grid[row, col] == null)
                {
                    emptyCells.Add(new Vector2Int(row, col));
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            Debug.Log("[GridManager] FillEmptyCells() — no empty cells found, skipping spawn.");
            return;
        }

        Debug.Log($"[GridManager] FillEmptyCells() — sending {emptyCells.Count} empty cell(s) to TileSpawner.");
        _tileSpawner.SpawnTiles(emptyCells, _grid);
    }

    /// <summary>
    /// Registers a newly spawned tile into the grid array at the correct position.
    /// Called by TileSpawner after it instantiates each tile.
    /// </summary>
    public void RegisterSpawnedTile(Tile tile, Vector2Int gridPosition)
    {
        _grid[gridPosition.x, gridPosition.y] = tile;
        Debug.Log($"[GridManager] RegisterSpawnedTile() — {tile.Type} registered at {gridPosition}.");
    }

    /// <summary>
    /// Instantiates a single tile prefab at the given world position and initialises it.
    /// Used during BuildGrid(). Spawning during play is handled by TileSpawner.
    /// </summary>
    private Tile SpawnTileAt(Vector2Int gridPos, Vector3 worldPos)
    {
        GameObject tileObject = Instantiate(_tilePrefab, worldPos, Quaternion.identity, transform);
        Tile tile = tileObject.GetComponent<Tile>();

        IngredientType randomType = (IngredientType)Random.Range(0, System.Enum.GetValues(typeof(IngredientType)).Length);
        tile.Initialise(randomType, gridPos);

        return tile;
    }

    /// <summary>
    /// Converts a grid position (row, column) to a world space Vector3.
    /// </summary>
    private Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.y * _tileSize, -gridPosition.x * _tileSize, 0f);
    }

    /// <summary>
    /// Returns true if the given grid position is within the bounds of the grid.
    /// </summary>
    private bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < Rows
            && position.y >= 0 && position.y < Columns;
    }
}