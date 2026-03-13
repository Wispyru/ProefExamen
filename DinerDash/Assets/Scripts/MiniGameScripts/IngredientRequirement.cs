using System;

/// <summary>
/// Defines a single ingredient requirement within a recipe.
/// Stores which ingredient type is needed and how many matches are required to fulfil it.
/// Used as a data entry inside RecipeConfig.
/// </summary>
[Serializable]
public struct IngredientRequirement
{
    /// <summary>
    /// The ingredient type the player needs to match.
    /// </summary>
    public IngredientType Type;

    /// <summary>
    /// Total number of matched tiles of this type needed to fulfil this requirement.
    /// For recipes that list the same ingredient twice (e.g. Vegetables x2), increase this value accordingly.
    /// </summary>
    public int RequiredCount;
}