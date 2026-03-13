using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single ingredient progress bar in the minigame UI.
/// Displays the ingredient icon, a fill bar using EXP achter/voor sprites,
/// a count label, and a Vinkje checkmark when the requirement is fulfilled.
/// One bar is instantiated per IngredientRequirement in the active recipe.
/// Receives updates from RecipeManager via MinigameUIManager.
/// </summary>
public class IngredientBar : MonoBehaviour
{

    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private Image _barFill;
    [SerializeField] private Image _checkmark;
    [SerializeField] private TMPro.TextMeshProUGUI _countLabel;
    
    private IngredientType _type;
    private int _requiredCount;
    private int _currentCount;
    private bool _isFulfilled;

    private const float FillAnimationDuration = 0.3f;
    

    /// <summary>
    /// Sets up this bar with the correct ingredient sprite and required count.
    /// Called by MinigameUIManager when a new recipe is loaded.
    /// </summary>
    public void Initialise(IngredientType type, int requiredCount, Sprite icon)
    {
        _type          = type;
        _requiredCount = requiredCount;
        _currentCount  = 0;
        _isFulfilled   = false;

        _ingredientIcon.sprite = icon;
        _barFill.fillAmount    = 0f;
        _checkmark.gameObject.SetActive(false);

        UpdateCountLabel();

        Debug.Log($"[IngredientBar] Initialised for {type} — 0 / {requiredCount}.");
    }

    /// <summary>
    /// Updates the bar's progress to reflect the current match count.
    /// Animates the fill, updates the count label, and shows the checkmark if fulfilled.
    /// Called by MinigameUIManager when RecipeManager fires OnProgressUpdated.
    /// </summary>
    public void UpdateProgress(int currentCount)
    {
        if (_isFulfilled)
        {
            Debug.Log($"[IngredientBar] {_type} already fulfilled — ignoring UpdateProgress call.");
            return;
        }

        _currentCount = Mathf.Clamp(currentCount, 0, _requiredCount);

        float targetFill = (float)_currentCount / _requiredCount;

        Debug.Log($"[IngredientBar] {_type} progress updated — {_currentCount} / {_requiredCount} (fill: {targetFill:P0}).");

        StopAllCoroutines();
        StartCoroutine(AnimateFill(targetFill));

        UpdateCountLabel();

        if (_currentCount >= _requiredCount)
        {
            OnFulfilled();
        }
    }
    
    /// <summary>
    /// Smoothly animates the bar fill from its current value to the target fill amount.
    /// </summary>
    private IEnumerator AnimateFill(float targetFill)
    {
        float startFill = _barFill.fillAmount;
        float elapsed   = 0f;

        while (elapsed < FillAnimationDuration)
        {
            elapsed            += Time.deltaTime;
            _barFill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / FillAnimationDuration);
            yield return null;
        }

        _barFill.fillAmount = targetFill;

        Debug.Log($"[IngredientBar] {_type} fill animation complete — fill at {targetFill:P0}.");
    }

    /// <summary>
    /// Updates the count label text to show current / required.
    /// </summary>
    private void UpdateCountLabel()
    {
        _countLabel.text = $"{_currentCount} / {_requiredCount}";
    }

    /// <summary>
    /// Called when the ingredient requirement is fully met.
    /// Locks the bar at full and shows the Vinkje checkmark.
    /// </summary>
    private void OnFulfilled()
    {
        _isFulfilled        = true;
        _barFill.fillAmount = 1f;
        _checkmark.gameObject.SetActive(true);

        Debug.Log($"[IngredientBar] {_type} FULFILLED — bar locked at full, checkmark shown.");
    }
}