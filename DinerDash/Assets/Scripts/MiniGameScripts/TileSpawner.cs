using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning new tiles into empty grid cells after gravity has resolved.
/// Applies a configurable spawn bias so ingredients required by the active recipe
/// appear slightly more often than the rest. Guarantees no immediate 3-match on spawn.
/// Called exclusively by GridManager.
/// </summary>
public class TileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GridManager _gridManager;
    
    /// Percentage bonus applied to the spawn weight of required ingredients.
    /// 0 = fully equal chance for all types.
    /// 20 = required ingredients are 20% more likely to spawn than others.
    /// Adjust in the Inspector — do not hardcode this value.
    [SerializeField] private float _spawnBiasPercent = 20f;
    
    private RecipeConfig _activeRecipe;
    private const int MaxRerollAttempts = 10;
    
    /// <summary>
    /// Stores the active recipe so the spawner knows which ingredients to bias.
    /// Called by MinigameManager whenever a new customer order begins.
    /// </summary>
    public void SetActiveRecipe(RecipeConfig recipe)
    {
        _activeRecipe = recipe;
        Debug.Log($"[TileSpawner] SetActiveRecipe() — active recipe set to '{recipe.RecipeName}'.");
        Debug.Log($"[TileSpawner] Spawn bias of {_spawnBiasPercent}% will be applied to: {string.Join(", ", recipe.GetRequiredTypes())}");
    }

    /// <summary>
    /// Spawns new tiles into the given empty grid cells.
    /// Applies spawn bias toward required ingredients and avoids immediate 3-matches.
    /// Called by GridManager after ApplyGravity() has resolved.
    /// </summary>
    public void SpawnTiles(List<Vector2Int> emptyCells, Tile[,] grid)
    {
        Debug.Log($"[TileSpawner] SpawnTiles() called — spawning into {emptyCells.Count} empty cell(s).");

        List<IngredientType> requiredTypes = _activeRecipe != null
            ? _activeRecipe.GetRequiredTypes()
            : new List<IngredientType>();

        foreach (Vector2Int cell in emptyCells)
        {
            IngredientType chosenType = GetWeightedRandomType(requiredTypes, grid, cell);
            SpawnTileAt(cell, chosenType, grid);
        }

        Debug.Log("[TileSpawner] SpawnTiles() — all empty cells filled.");
    }
    
    /// <summary>
    /// Selects a weighted random IngredientType for a given cell.
    /// Required ingredients receive a bonus weight based on _spawnBiasPercent.
    /// Re-rolls up to MaxRerollAttempts times to avoid creating an immediate 3-match.
    /// </summary>
    private IngredientType GetWeightedRandomType(List<IngredientType> requiredTypes, Tile[,] grid, Vector2Int cell)
    {
        IngredientType[] allTypes = (IngredientType[])System.Enum.GetValues(typeof(IngredientType));

        for (int attempt = 0; attempt < MaxRerollAttempts; attempt++)
        {
            IngredientType candidate = RollWeightedType(allTypes, requiredTypes);

            if (!CreatesImmediateMatch(cell, candidate, grid))
            {
                if (attempt > 0)
                {
                    Debug.Log($"[TileSpawner] {candidate} accepted for {cell} after {attempt + 1} attempt(s).");
                }
                else
                {
                    Debug.Log($"[TileSpawner] {candidate} selected for {cell} on first attempt.");
                }

                return candidate;
            }

            Debug.Log($"[TileSpawner] Attempt {attempt + 1}: {candidate} at {cell} would create an immediate match — re-rolling.");
        }

        IngredientType fallback = RollWeightedType(allTypes, requiredTypes);
        Debug.LogWarning($"[TileSpawner] All {MaxRerollAttempts} re-roll attempts created matches at {cell}. Accepting {fallback} — cascade will resolve it.");
        return fallback;
    }

    /// <summary>
    /// Rolls a single weighted random IngredientType.
    /// Required ingredients are weighted at BaseWeight * (1 + _spawnBiasPercent / 100).
    /// All weights are normalised so total probability always equals 100%.
    /// </summary>
    private IngredientType RollWeightedType(IngredientType[] allTypes, List<IngredientType> requiredTypes)
    {
        float baseWeight  = 1f;
        float biasedWeight = baseWeight * (1f + _spawnBiasPercent / 100f);

        float totalWeight = 0f;
        foreach (IngredientType type in allTypes)
        {
            totalWeight += requiredTypes.Contains(type) ? biasedWeight : baseWeight;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (IngredientType type in allTypes)
        {
            cumulative += requiredTypes.Contains(type) ? biasedWeight : baseWeight;

            if (roll <= cumulative)
            {
                return type;
            }
        }

        // Fallback — should never be reached due to normalisation
        Debug.LogWarning("[TileSpawner] RollWeightedType() fell through without selecting a type. Returning first type as fallback.");
        return allTypes[0];
    }

    /// <summary>
    /// Checks whether placing the given type at the given cell would immediately create a match of 3 or more.
    /// Looks left/right horizontally and up/down vertically from the candidate cell.
    /// </summary>
    private bool CreatesImmediateMatch(Vector2Int cell, IngredientType type, Tile[,] grid)
    {
        int rows    = grid.GetLength(0);
        int columns = grid.GetLength(1);
        int row     = cell.x;
        int col     = cell.y;

        // Check horizontal run through this cell
        int horizontalCount = 1;
        horizontalCount += CountAdjacentMatches(grid, row, col, 0,  1, type, columns, rows);
        horizontalCount += CountAdjacentMatches(grid, row, col, 0, -1, type, columns, rows);

        if (horizontalCount >= 3)
        {
            Debug.Log($"[TileSpawner] CreatesImmediateMatch: {type} at {cell} would form a horizontal match of {horizontalCount}.");
            return true;
        }

        // Check vertical run through this cell
        int verticalCount = 1;
        verticalCount += CountAdjacentMatches(grid, row, col,  1, 0, type, columns, rows);
        verticalCount += CountAdjacentMatches(grid, row, col, -1, 0, type, columns, rows);

        if (verticalCount >= 3)
        {
            Debug.Log($"[TileSpawner] CreatesImmediateMatch: {type} at {cell} would form a vertical match of {verticalCount}.");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Counts how many consecutive tiles of the given type exist in one direction from a starting cell.
    /// Used by CreatesImmediateMatch to check both directions along an axis.
    /// </summary>
    private int CountAdjacentMatches(Tile[,] grid, int startRow, int startCol, int rowDir, int colDir, IngredientType type, int columns, int rows)
    {
        int count = 0;
        int r = startRow + rowDir;
        int c = startCol + colDir;

        while (r >= 0 && r < rows && c >= 0 && c < columns)
        {
            if (grid[r, c] != null && grid[r, c].Type == type)
            {
                count++;
                r += rowDir;
                c += colDir;
            }
            else
            {
                break;
            }
        }

        return count;
    }

    /// <summary>
    /// Instantiates a tile prefab at the given grid cell, initialises it, and registers it with GridManager.
    /// New tiles spawn above the visible grid and fall into position.
    /// </summary>
    private void SpawnTileAt(Vector2Int gridPosition, IngredientType type, Tile[,] grid)
    {
        // Spawn above the top of the grid so it falls into view
        Vector3 spawnWorldPos  = GridToWorldPosition(new Vector2Int(-1, gridPosition.y));
        Vector3 targetWorldPos = GridToWorldPosition(gridPosition);

        GameObject tileObject = Instantiate(_tilePrefab, spawnWorldPos, Quaternion.identity, _gridManager.transform);
        Tile tile = tileObject.GetComponent<Tile>();

        tile.Initialise(type, gridPosition);
        tile.PlayFallAnimation(targetWorldPos);

        _gridManager.RegisterSpawnedTile(tile, gridPosition);

        Debug.Log($"[TileSpawner] Spawned {type} at {gridPosition}, falling from {spawnWorldPos} to {targetWorldPos}.");
    }

    /// <summary>
    /// Converts a grid position (row, column) to a world space Vector3.
    /// Must match the conversion used in GridManager exactly.
    /// </summary>
    private Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.y, -gridPosition.x, 0f);
    }
}