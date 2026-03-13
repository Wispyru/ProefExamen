using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a single recipe in the Match Minigame.
/// Create one asset per recipe via Assets > Create > Diner Dash > Recipe Config.
/// All values are set in the Unity Inspector — nothing is hardcoded.
/// </summary>
[CreateAssetMenu(fileName = "NewRecipeConfig", menuName = "Diner Dash/Recipe Config")]
public class RecipeConfig : ScriptableObject
{

   
    /// Display name of the recipe shown in the UI (e.g. "Burger", "Ice Cream").
    public string RecipeName;
    
    /// Sprite representing this recipe in the UI. 
    public Sprite RecipeSprite; /// change acording to named sprite.
    
    /// List of ingredient requirements that must all be fulfilled to complete this recipe.
    /// Each entry specifies an IngredientType and how many matched tiles are needed.
    public List<IngredientRequirement> Requirements;
    

    /// <summary>
    /// Returns the required count for a given ingredient type.
    /// Returns 0 if the ingredient is not part of this recipe.
    /// </summary>
    public int GetRequiredCount(IngredientType type)
    {
        foreach (IngredientRequirement requirement in Requirements)
        {
            if (requirement.Type == type)
            {
                return requirement.RequiredCount;
            }
        }

        Debug.LogWarning($"[RecipeConfig] '{RecipeName}': GetRequiredCount called for {type}, which is not in this recipe. Returning 0.");
        return 0;
    }

    /// <summary>
    /// Returns a list of all unique ingredient types required by this recipe.
    /// Used by TileSpawner to determine which types receive a spawn bias boost.
    /// </summary>
    public List<IngredientType> GetRequiredTypes()
    {
        List<IngredientType> types = new List<IngredientType>();

        foreach (IngredientRequirement requirement in Requirements)
        {
            if (!types.Contains(requirement.Type))
            {
                types.Add(requirement.Type);
            }
        }

        Debug.Log($"[RecipeConfig] '{RecipeName}': GetRequiredTypes() returning {types.Count} unique type(s): {string.Join(", ", types)}");
        return types;
    }

    /// <summary>
    /// Logs the full contents of this RecipeConfig to the console.
    /// Call this on load to verify the ScriptableObject is configured correctly.
    /// </summary>
    public void LogRecipeDetails()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            Debug.LogWarning($"[RecipeConfig] '{RecipeName}': LogRecipeDetails() — Requirements list is empty or null! Check the Inspector.");
            return;
        }

        Debug.Log($"[RecipeConfig] --- Recipe Loaded: '{RecipeName}' ---");
        Debug.Log($"[RecipeConfig] '{RecipeName}': Total requirements: {Requirements.Count}");

        foreach (IngredientRequirement requirement in Requirements)
        {
            Debug.Log($"[RecipeConfig] '{RecipeName}': Requires {requirement.RequiredCount}x {requirement.Type}");
        }

        Debug.Log($"[RecipeConfig] '{RecipeName}': Has sprite assigned: {RecipeSprite != null}");
        Debug.Log($"[RecipeConfig] --- End of '{RecipeName}' ---");
    }
}