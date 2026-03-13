using UnityEngine;


/// MonoBehaviour attached to each tile GameObject on the grid.
/// Stores the tile's IngredientType and manages its own visual states.
/// Does not reference any other script — all commands come from GridManager.
public class Tile : MonoBehaviour
{
    
    /// The ingredient type this tile represents. Read-only after Initialise() is called.
    public IngredientType Type { get; private set; }

    
    /// The grid position of this tile (column, row).
    /// Updated by GridManager whenever the tile moves.
    public Vector2Int GridPosition { get; private set; }
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    
    private bool _isAnimating;
    
    /// <summary>
    /// Sets up this tile with an ingredient type and grid position.
    /// Called by TileSpawner immediately after instantiation.
    /// </summary>
    public void Initialise(IngredientType type, Vector2Int gridPosition)
    {
        Type = type;
        GridPosition = gridPosition;

        Debug.Log($"[Tile] Initialised at {gridPosition} as {type}.");
    }

    /// <summary>
    /// Updates the tile's recorded grid position.
    /// Called by GridManager after a swap or gravity shift.
    /// </summary>
    public void SetGridPosition(Vector2Int newPosition)
    {
        Debug.Log($"[Tile] {Type} moved from {GridPosition} to {newPosition}.");
        GridPosition = newPosition;
    }

    /// <summary>
    /// Plays the selected visual state (e.g. highlight outline).
    /// Called by InputHandler when the player presses down on this tile.
    /// </summary>
    public void PlaySelectedVisual()
    {
        Debug.Log($"[Tile] {Type} at {GridPosition} selected.");

        if (_animator != null)
        {
            _animator.SetTrigger("Selected");
        }
    }

    /// <summary>
    /// Resets the tile back to its idle visual state.
    /// Called by InputHandler when a drag is cancelled or invalid.
    /// </summary>
    public void PlayIdleVisual()
    {
        Debug.Log($"[Tile] {Type} at {GridPosition} returned to idle.");

        if (_animator != null)
        {
            _animator.SetTrigger("Idle");
        }
    }

    /// <summary>
    /// Plays the match animation then destroys this tile.
    /// Called by GridManager when this tile is part of a resolved match.
    /// </summary>
    public void PlayMatchAnimation()
    {
        Debug.Log($"[Tile] {Type} at {GridPosition} matched and will be destroyed.");

        if (_animator != null)
        {
            _animator.SetTrigger("Matched");
        }

        Destroy(gameObject, 0.4f);
    }

    /// <summary>
    /// Moves this tile to a new world position to simulate falling after gravity.
    /// Called by GridManager during ApplyGravity().
    /// </summary>
    public void PlayFallAnimation(Vector3 targetWorldPosition)
    {
        Debug.Log($"[Tile] {Type} at {GridPosition} falling to world position {targetWorldPosition}.");

        StopAllCoroutines();
        StartCoroutine(MoveToPosition(targetWorldPosition));
    }
    
    /// <summary>
    /// Smoothly moves the tile to the target world position over a short duration.
    /// </summary>
    private System.Collections.IEnumerator MoveToPosition(Vector3 targetWorldPosition)
    {
        _isAnimating = true;

        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetWorldPosition, elapsed / duration);
            yield return null;
        }

        transform.position = targetWorldPosition;
        _isAnimating = false;

        Debug.Log($"[Tile] {Type} finished falling. Now at world position {targetWorldPosition}.");
    }
}