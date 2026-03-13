using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plain C# class — no MonoBehaviour, no GameObject needed.
/// Scans the grid for horizontal and vertical matches of 3 or more identical IngredientTypes.
/// Called exclusively by GridManager. Has no knowledge of any other system.
/// </summary>
public class MatchChecker
{
    private const int MinMatchLength = 3;
    
    /// <summary>
    /// Scans the entire grid and returns a deduplicated list of all matched tile positions.
    /// A match is 3 or more identical IngredientTypes in a straight horizontal or vertical line.
    /// Returns an empty list if no matches are found.
    /// </summary>
    public List<Vector2Int> FindMatches(Tile[,] grid)
    {
        Debug.Log("[MatchChecker] FindMatches() called — scanning grid.");

        HashSet<Vector2Int> matchedPositions = new HashSet<Vector2Int>();

        CheckHorizontal(grid, matchedPositions);
        CheckVertical(grid, matchedPositions);

        List<Vector2Int> result = new List<Vector2Int>(matchedPositions);

        if (result.Count > 0)
        {
            Debug.Log($"[MatchChecker] Found {result.Count} matched tile(s) across all matches.");
        }
        else
        {
            Debug.Log("[MatchChecker] No matches found.");
        }

        return result;
    }
    
    /// <summary>
    /// Scans every row left-to-right for runs of 3 or more identical IngredientTypes.
    /// Adds all matched positions to the shared matchedPositions set.
    /// </summary>
    private void CheckHorizontal(Tile[,] grid, HashSet<Vector2Int> matchedPositions)
    {
        int rows    = grid.GetLength(0);
        int columns = grid.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            int runStart  = 0;
            int runLength = 1;

            for (int col = 1; col < columns; col++)
            {
                Tile current  = grid[row, col];
                Tile previous = grid[row, col - 1];

                bool isContinuation = current != null
                    && previous != null
                    && current.Type == previous.Type;

                if (isContinuation)
                {
                    runLength++;
                }
                else
                {
                    RegisterRun(grid, matchedPositions, runStart, row, runLength, isHorizontal: true);
                    runStart  = col;
                    runLength = 1;
                }
            }

            RegisterRun(grid, matchedPositions, runStart, row, runLength, isHorizontal: true);
        }
    }

    /// <summary>
    /// Scans every column top-to-bottom for runs of 3 or more identical IngredientTypes.
    /// Adds all matched positions to the shared matchedPositions set.
    /// </summary>
    private void CheckVertical(Tile[,] grid, HashSet<Vector2Int> matchedPositions)
    {
        int rows    = grid.GetLength(0);
        int columns = grid.GetLength(1);

        for (int col = 0; col < columns; col++)
        {
            int runStart  = 0;
            int runLength = 1;

            for (int row = 1; row < rows; row++)
            {
                Tile current  = grid[row, col];
                Tile previous = grid[row - 1, col];

                bool isContinuation = current != null
                    && previous != null
                    && current.Type == previous.Type;

                if (isContinuation)
                {
                    runLength++;
                }
                else
                {
                    RegisterRun(grid, matchedPositions, runStart, col, runLength, isHorizontal: false);
                    runStart  = row;
                    runLength = 1;
                }
            }

            RegisterRun(grid, matchedPositions, runStart, col, runLength, isHorizontal: false);
        }
    }

    /// <summary>
    /// Checks if a completed run meets the minimum match length.
    /// If it does, all positions in the run are added to matchedPositions.
    /// </summary>
    private void RegisterRun(Tile[,] grid, HashSet<Vector2Int> matchedPositions, int runStart, int fixedIndex, int runLength, bool isHorizontal)
    {
        if (runLength < MinMatchLength)
            return;

        string direction = isHorizontal ? "horizontal" : "vertical";
        IngredientType matchType = isHorizontal
            ? grid[fixedIndex, runStart].Type
            : grid[runStart, fixedIndex].Type;

        Debug.Log($"[MatchChecker] {direction} match found: {runLength}x {matchType} starting at " +
                  (isHorizontal ? $"row {fixedIndex}, col {runStart}" : $"row {runStart}, col {fixedIndex}"));

        for (int i = 0; i < runLength; i++)
        {
            Vector2Int position = isHorizontal
                ? new Vector2Int(fixedIndex, runStart + i)
                : new Vector2Int(runStart + i, fixedIndex);

            matchedPositions.Add(position);
        }
    }
}