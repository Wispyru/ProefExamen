using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles all player drag input for the Match Minigame.
/// Detects which tile the player pressed down on, resolves the drag direction on release,
/// validates adjacency, and passes valid swap requests to GridManager.
/// Locks input during cascade resolution and unlocks when the grid is stable.
/// </summary>
public class InputHandler : MonoBehaviour
{
    // ─── Serialized Private Variables ────────────────────────────────────────

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Camera _camera;

    // ─── Private Variables ───────────────────────────────────────────────────

    private Tile _dragOrigin;
    private Vector2 _dragStartScreenPos;
    private bool _inputLocked;

    // ─── Unity Methods ───────────────────────────────────────────────────────

    private void OnEnable()
    {
        _gridManager.OnSwapReverted.AddListener(OnSwapReverted);
        _gridManager.OnCascadeStart.AddListener(OnCascadeStart);
        _gridManager.OnCascadeEnd.AddListener(OnCascadeEnd);

        Debug.Log("[InputHandler] OnEnable() — subscribed to GridManager events.");
    }

    private void OnDisable()
    {
        _gridManager.OnSwapReverted.RemoveListener(OnSwapReverted);
        _gridManager.OnCascadeStart.RemoveListener(OnCascadeStart);
        _gridManager.OnCascadeEnd.RemoveListener(OnCascadeEnd);

        Debug.Log("[InputHandler] OnDisable() — unsubscribed from GridManager events.");
    }

    private void Update()
    {
        if (_inputLocked)
            return;

        if (Input.GetMouseButtonDown(0))
            HandlePointerDown();

        if (Input.GetMouseButtonUp(0))
            HandlePointerUp();
    }

    // ─── Public Methods ──────────────────────────────────────────────────────

    /// <summary>
    /// Locks or unlocks player input.
    /// Called by GridManager at the start and end of cascade resolution.
    /// </summary>
    public void SetInputLocked(bool locked)
    {
        _inputLocked = locked;
        Debug.Log($"[InputHandler] SetInputLocked({locked}) — input is now {(locked ? "LOCKED" : "UNLOCKED")}.");
    }

    // ─── Private Methods ─────────────────────────────────────────────────────

    /// <summary>
    /// Fires when the player presses down. Raycasts to find the tile under the cursor
    /// and stores it as the drag origin.
    /// </summary>
    private void HandlePointerDown()
    {
        Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Tile tile = GetTileAtWorldPosition(worldPos);

        if (tile == null)
        {
            Debug.Log("[InputHandler] HandlePointerDown() — no tile found at press position.");
            return;
        }

        _dragOrigin          = tile;
        _dragStartScreenPos  = Input.mousePosition;

        _dragOrigin.PlaySelectedVisual();

        Debug.Log($"[InputHandler] HandlePointerDown() — drag started on {tile.Type} at grid position {tile.GridPosition}.");
    }

    /// <summary>
    /// Fires when the player releases. Resolves the drag direction from start to release,
    /// finds the target adjacent cell, and submits the swap to GridManager if valid.
    /// </summary>
    private void HandlePointerUp()
    {
        if (_dragOrigin == null)
            return;

        Vector2 dragDelta     = (Vector2)Input.mousePosition - _dragStartScreenPos;
        Vector2Int swapOffset = ResolveDragDirection(dragDelta);

        if (swapOffset == Vector2Int.zero)
        {
            Debug.Log("[InputHandler] HandlePointerUp() — drag distance too small, ignoring.");
            _dragOrigin.PlayIdleVisual();
            _dragOrigin = null;
            return;
        }

        Vector2Int originPos = _dragOrigin.GridPosition;
        Vector2Int targetPos = originPos + swapOffset;

        Debug.Log($"[InputHandler] HandlePointerUp() — drag released. Origin: {originPos}, Target: {targetPos}, Offset: {swapOffset}.");

        if (!IsAdjacent(originPos, targetPos))
        {
            Debug.Log($"[InputHandler] {targetPos} is not adjacent to {originPos} — swap rejected.");
            _dragOrigin.PlayIdleVisual();
            _dragOrigin = null;
            return;
        }

        Tile targetTile = _gridManager.GetTileAt(targetPos);

        if (targetTile == null)
        {
            Debug.Log($"[InputHandler] No tile found at target position {targetPos} — swap rejected.");
            _dragOrigin.PlayIdleVisual();
            _dragOrigin = null;
            return;
        }

        Debug.Log($"[InputHandler] Valid swap submitted — {_dragOrigin.Type} at {originPos} <-> {targetTile.Type} at {targetPos}.");

        _dragOrigin.PlayIdleVisual();
        _dragOrigin = null;

        _gridManager.SwapTiles(originPos, targetPos);
    }

    /// <summary>
    /// Converts a drag delta in screen pixels to a cardinal grid direction (up/down/left/right).
    /// Returns Vector2Int.zero if the drag is too short to count as intentional.
    /// </summary>
    private Vector2Int ResolveDragDirection(Vector2 dragDelta)
    {
        float minDragDistance = 20f;

        if (dragDelta.magnitude < minDragDistance)
            return Vector2Int.zero;

        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            // Horizontal drag
            return dragDelta.x > 0
                ? new Vector2Int(0,  1)   // right
                : new Vector2Int(0, -1);  // left
        }
        else
        {
            // Vertical drag — note: row index increases downward in grid space
            return dragDelta.y > 0
                ? new Vector2Int(-1, 0)   // up on screen = lower row index
                : new Vector2Int( 1, 0);  // down on screen = higher row index
        }
    }

    /// <summary>
    /// Returns true if posB is directly adjacent (up/down/left/right) to posA.
    /// Diagonal adjacency is not valid.
    /// </summary>
    private bool IsAdjacent(Vector2Int posA, Vector2Int posB)
    {
        int rowDiff = Mathf.Abs(posA.x - posB.x);
        int colDiff = Mathf.Abs(posA.y - posB.y);

        bool adjacent = (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);

        Debug.Log($"[InputHandler] IsAdjacent({posA}, {posB}) — rowDiff: {rowDiff}, colDiff: {colDiff}, result: {adjacent}.");

        return adjacent;
    }

    /// <summary>
    /// Raycasts at a world position to find a Tile component.
    /// Returns null if nothing is hit or the hit object has no Tile.
    /// </summary>
    private Tile GetTileAtWorldPosition(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null)
        {
            return null;
        }

        Tile tile = hit.collider.GetComponent<Tile>();

        if (tile == null)
        {
            Debug.Log($"[InputHandler] GetTileAtWorldPosition() — hit {hit.collider.gameObject.name} but it has no Tile component.");
        }

        return tile;
    }

    // ─── Event Listeners ─────────────────────────────────────────────────────

    /// <summary>
    /// Called when GridManager reverts a swap that produced no matches.
    /// Ensures input is re-enabled so the player can try again.
    /// </summary>
    private void OnSwapReverted()
    {
        Debug.Log("[InputHandler] OnSwapReverted received — unlocking input.");
        SetInputLocked(false);
    }

    /// <summary>
    /// Called when GridManager starts cascade resolution.
    /// Locks input so the player cannot act mid-cascade.
    /// </summary>
    private void OnCascadeStart()
    {
        Debug.Log("[InputHandler] OnCascadeStart received — locking input.");
        SetInputLocked(true);
    }

    /// <summary>
    /// Called when GridManager finishes cascade resolution and the grid is stable.
    /// Unlocks input so the player can make their next move.
    /// </summary>
    private void OnCascadeEnd()
    {
        Debug.Log("[InputHandler] OnCascadeEnd received — unlocking input.");
        SetInputLocked(false);
    }
}