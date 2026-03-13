using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Top-level controller for the Match Minigame.
/// Acts as the entry point called by the outer customer/order system.
/// Passes the active RecipeConfig to RecipeManager, TileSpawner, and MinigameUIManager,
/// and listens for recipe completion to signal the order system.
/// </summary>
public class MinigameManager : MonoBehaviour
{
    /// <summary>
    /// Fired when the player successfully completes the active recipe.
    /// The outer order/customer system listens to this to mark the order as ready.
    /// </summary>
    public UnityEvent OnMinigameComplete = new UnityEvent();
    
    [SerializeField] private RecipeManager _recipeManager;
    [SerializeField] private TileSpawner _tileSpawner;
    [SerializeField] private MinigameUIManager _uiManager;
    
    private RecipeConfig _activeRecipe;
    
    private void OnEnable()
    {
        _recipeManager.OnRecipeComplete.AddListener(OnRecipeComplete);
        Debug.Log("[MinigameManager] OnEnable() — subscribed to RecipeManager.OnRecipeComplete.");
    }

    private void OnDisable()
    {
        _recipeManager.OnRecipeComplete.RemoveListener(OnRecipeComplete);
        Debug.Log("[MinigameManager] OnDisable() — unsubscribed from RecipeManager.OnRecipeComplete.");
    }
    
    /// <summary>
    /// Starts a new minigame session for the given recipe.
    /// Called by the outer order/customer system when a customer places an order.
    /// Passes the recipe to RecipeManager (progress tracking), TileSpawner (spawn bias),
    /// and MinigameUIManager (bar display).
    /// </summary>
    public void StartMinigame(RecipeConfig recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("[MinigameManager] StartMinigame() — received a null RecipeConfig! Check the order system.");
            return;
        }

        _activeRecipe = recipe;

        Debug.Log($"[MinigameManager] StartMinigame() — starting session for '{recipe.RecipeName}'.");

        _recipeManager.LoadRecipe(recipe);
        _tileSpawner.SetActiveRecipe(recipe);
        _uiManager.LoadRecipeUI(recipe);

        Debug.Log($"[MinigameManager] StartMinigame() — RecipeManager, TileSpawner and UIManager have all received '{recipe.RecipeName}'.");
    }
    
    /// <summary>
    /// Called when RecipeManager fires OnRecipeComplete.
    /// Logs the result and fires OnMinigameComplete for the outer order system to handle.
    /// </summary>
    private void OnRecipeComplete()
    {
        Debug.Log($"[MinigameManager] OnRecipeComplete() — '{_activeRecipe.RecipeName}' finished! Firing OnMinigameComplete.");
        OnMinigameComplete.Invoke();
    }
}