using UnityEngine;

[System.Serializable]
public class TipSettings
{
    public float MinimumTip;
    public float MaximumTip;
}

public class CustomerData : MonoBehaviour
{
    public CustomerTypeEnum CustomerType;

    // Group Variables
    public int CustomerAmount;

    [Header("Tip Settings")]
    [SerializeField] private TipSettings _tipSettings;

    public float TotalTip { get; private set; }

    // Timer variables
    public float CustomerTime;

    /// <summary>
    /// Sets the calculated tip for this specific customer
    /// </summary>
    public void SetTip(float amount)
    {
        TotalTip = amount;
    }

    /// <summary>
    /// Returns the tip multiplier based on customer type
    /// </summary>
    public float GetTipMultiplier()
    {
        switch (CustomerType)
        {
            case CustomerTypeEnum.impatient:
                return 1.3f;

            case CustomerTypeEnum.reservation:
                return 1.5f; // template, adjust later

            default:
                return 1f;
        }
    }
}