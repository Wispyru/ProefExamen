using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks the player's ingredient match progress against the active recipe.
/// Listens to GridManager's OnTilesMatched event and increments counts per ingredient type.
/// Fires OnProgressUpdated so the UI can refresh, and OnRecipeComplete when all
/// requirements are met.
/// </summary>
public class RecipeManager : MonoBehaviour
{

    /// <summary>
    /// Fired whenever progress on a specific ingredient increases.
    /// Passes the ingredient type, current count, and required count to IngredientBar UI.
    /// </summary>
    public UnityEvent<IngredientType, int, int> OnProgressUpdated = new UnityEvent<IngredientType, int, int>();

    /// <summary>
    /// Fired when all ingredient requirements for the active recipe are fulfilled.
    /// MinigameManager listens to this to signal the order system.
    /// </summary>
    public UnityEvent OnRecipeComplete = new UnityEvent();
    
    [SerializeField] private GridManager _gridManager;
    
    private RecipeConfig _activeRecipe;
    private Dictionary<IngredientType, int> _progress;
    private Dictionary<IngredientType, int> _requirements;
    private bool _recipeComplete;
    
    private void OnEnable()
    {
        _gridManager.OnTilesMatched.AddListener(OnTilesMatched);
        Debug.Log("[RecipeManager] OnEnable() — subscribed to GridManager.OnTilesMatched.");
    }

    private void OnDisable()
    {
        _gridManager.OnTilesMatched.RemoveListener(OnTilesMatched);
        Debug.Log("[RecipeManager] OnDisable() — unsubscribed from GridManager.OnTilesMatched.");
    }
    
    /// <summary>
    /// Loads a new recipe, resets all progress, and prepares the manager for a new session.
    /// Called by MinigameManager when a customer places an order.
    /// </summary>
    public void LoadRecipe(RecipeConfig recipe)
    {
        _activeRecipe  = recipe;
        _recipeComplete = false;
        _progress      = new Dictionary<IngredientType, int>();
        _requirements  = new Dictionary<IngredientType, int>();

        foreach (IngredientRequirement requirement in recipe.Requirements)
        {
            _requirements[requirement.Type] = requirement.RequiredCount;
            _progress[requirement.Type]     = 0;

            Debug.Log($"[RecipeManager] LoadRecipe() — tracking {requirement.Type}: 0 / {requirement.RequiredCount} required.");
        }

        Debug.Log($"[RecipeManager] LoadRecipe() — '{recipe.RecipeName}' loaded. Tracking {_requirements.Count} ingredient type(s).");
        recipe.LogRecipeDetails();
    }
    
    /// <summary>
    /// Receives the list of cleared ingredient types from GridManager after each match.
    /// Increments progress for any type that is part of the active recipe.
    /// </summary>
    private void OnTilesMatched(List<IngredientType> matchedTypes)
    {
        if (_activeRecipe == null)
        {
            Debug.LogWarning("[RecipeManager] OnTilesMatched() — no active recipe loaded. Ignoring matched tiles.");
            return;
        }

        if (_recipeComplete)
        {
            Debug.Log("[RecipeManager] OnTilesMatched() — recipe already complete, ignoring incoming matches.");
            return;
        }

        Debug.Log($"[RecipeManager] OnTilesMatched() — received {matchedTypes.Count} tile(s): {string.Join(", ", matchedTypes)}.");

        foreach (IngredientType type in matchedTypes)
        {
            if (!_progress.ContainsKey(type))
            {
                Debug.Log($"[RecipeManager] {type} is not required for '{_activeRecipe.RecipeName}' — ignoring.");
                continue;
            }

            if (_progress[type] >= _requirements[type])
            {
                Debug.Log($"[RecipeManager] {type} already fulfilled ({_progress[type]} / {_requirements[type]}) — ignoring extra matches.");
                continue;
            }

            _progress[type]++;

            Debug.Log($"[RecipeManager] {type} progress: {_progress[type]} / {_requirements[type]}.");

            OnProgressUpdated.Invoke(type, _progress[type], _requirements[type]);
        }

        CheckCompletion();
    }

    /// <summary>
    /// Checks whether all ingredient requirements have been met.
    /// Fires OnRecipeComplete if so.
    /// </summary>
    private void CheckCompletion()
    {
        foreach (KeyValuePair<IngredientType, int> requirement in _requirements)
        {
            if (_progress[requirement.Key] < requirement.Value)
            {
                Debug.Log($"[RecipeManager] CheckCompletion() — '{_activeRecipe.RecipeName}' not yet complete. " +
                          $"{requirement.Key} still needs {requirement.Value - _progress[requirement.Key]} more.");
                return;
            }
        }

        _recipeComplete = true;

        Debug.Log($"[RecipeManager] CheckCompletion() — '{_activeRecipe.RecipeName}' COMPLETE! Firing OnRecipeComplete.");
        OnRecipeComplete.Invoke();
    }
}