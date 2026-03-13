using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the minigame UI — spawns one IngredientBar per recipe requirement
/// and routes progress updates from RecipeManager to the correct bar.
/// Also displays the active recipe icon.
/// Bars are laid out in a vertical column on the side of the screen
/// using a Vertical Layout Group on the BarContainer.
/// </summary>
public class MinigameUIManager : MonoBehaviour
{
    [SerializeField] private RecipeManager _recipeManager;

    [Header("Bar Setup")]
    [SerializeField] private GameObject _ingredientBarPrefab;
    [SerializeField] private Transform _barContainer;

    [Header("Recipe Display")]
    [SerializeField] private Image _recipeIcon;

    [Header("Ingredient Sprites")]
    [SerializeField] private Sprite _spriteRawMeat;
    [SerializeField] private Sprite _spriteCheese;
    [SerializeField] private Sprite _spriteEggs;
    [SerializeField] private Sprite _spriteSoda;
    [SerializeField] private Sprite _spriteMilk;
    [SerializeField] private Sprite _spriteVegetables;
    [SerializeField] private Sprite _spriteFruit;
    [SerializeField] private Sprite _spriteDough;
    [SerializeField] private Sprite _spriteSugar;
    

    private Dictionary<IngredientType, IngredientBar> _activeBars;
    
    private void OnEnable()
    {
        _recipeManager.OnProgressUpdated.AddListener(OnProgressUpdated);
        Debug.Log("[MinigameUIManager] OnEnable() — subscribed to RecipeManager.OnProgressUpdated.");
    }

    private void OnDisable()
    {
        _recipeManager.OnProgressUpdated.RemoveListener(OnProgressUpdated);
        Debug.Log("[MinigameUIManager] OnDisable() — unsubscribed from RecipeManager.OnProgressUpdated.");
    }
    
    /// <summary>
    /// Builds the UI for a new recipe — clears any existing bars, spawns fresh ones,
    /// and updates the recipe icon.
    /// Called by MinigameManager when StartMinigame() is invoked.
    /// </summary>
    public void LoadRecipeUI(RecipeConfig recipe)
    {
        Debug.Log($"[MinigameUIManager] LoadRecipeUI() — building UI for '{recipe.RecipeName}'.");

        ClearBars();

        _recipeIcon.sprite = recipe.RecipeSprite;
        _activeBars        = new Dictionary<IngredientType, IngredientBar>();

        foreach (IngredientRequirement requirement in recipe.Requirements)
        {
            SpawnBar(requirement.Type, requirement.RequiredCount);
        }

        Debug.Log($"[MinigameUIManager] LoadRecipeUI() — {_activeBars.Count} bar(s) spawned for '{recipe.RecipeName}'.");
    }
    
    /// <summary>
    /// Instantiates a single IngredientBar prefab into the bar container,
    /// initialises it with the correct sprite and count, and registers it.
    /// </summary>
    private void SpawnBar(IngredientType type, int requiredCount)
    {
        Sprite icon = GetSpriteForType(type);

        if (icon == null)
        {
            Debug.LogWarning($"[MinigameUIManager] SpawnBar() — no sprite assigned for {type} in the Inspector. Bar will show a blank icon.");
        }

        GameObject barObject  = Instantiate(_ingredientBarPrefab, _barContainer);
        IngredientBar bar     = barObject.GetComponent<IngredientBar>();

        bar.Initialise(type, requiredCount, icon);
        _activeBars[type] = bar;

        Debug.Log($"[MinigameUIManager] SpawnBar() — bar spawned for {type} (required: {requiredCount}).");
    }

    /// <summary>
    /// Destroys all currently active bar GameObjects and clears the dictionary.
    /// Called before building UI for a new recipe to avoid leftover bars.
    /// </summary>
    private void ClearBars()
    {
        if (_activeBars == null || _activeBars.Count == 0)
        {
            Debug.Log("[MinigameUIManager] ClearBars() — no existing bars to clear.");
            return;
        }

        foreach (KeyValuePair<IngredientType, IngredientBar> entry in _activeBars)
        {
            if (entry.Value != null)
            {
                Destroy(entry.Value.gameObject);
            }
        }

        _activeBars.Clear();
        Debug.Log("[MinigameUIManager] ClearBars() — all previous bars destroyed.");
    }

    /// <summary>
    /// Receives a progress update from RecipeManager and forwards it to the correct bar.
    /// </summary>
    private void OnProgressUpdated(IngredientType type, int current, int required)
    {
        Debug.Log($"[MinigameUIManager] OnProgressUpdated() — {type}: {current} / {required}.");

        if (_activeBars == null || !_activeBars.ContainsKey(type))
        {
            Debug.LogWarning($"[MinigameUIManager] OnProgressUpdated() — no active bar found for {type}. Was LoadRecipeUI() called first?");
            return;
        }

        _activeBars[type].UpdateProgress(current);
    }

    /// <summary>
    /// Maps an IngredientType to its corresponding sprite.
    /// All sprites are assigned in the Inspector — nothing is hardcoded.
    /// </summary>
    private Sprite GetSpriteForType(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.RawMeat:    return _spriteRawMeat;
            case IngredientType.Cheese:     return _spriteCheese;
            case IngredientType.Eggs:       return _spriteEggs;
            case IngredientType.Soda:       return _spriteSoda;
            case IngredientType.Milk:       return _spriteMilk;
            case IngredientType.Vegetables: return _spriteVegetables;
            case IngredientType.Fruit:      return _spriteFruit;
            case IngredientType.Dough:      return _spriteDough;
            case IngredientType.Sugar:      return _spriteSugar;
            default:
                Debug.LogWarning($"[MinigameUIManager] GetSpriteForType() — unhandled IngredientType: {type}. Returning null.");
                return null;
        }
    }
}