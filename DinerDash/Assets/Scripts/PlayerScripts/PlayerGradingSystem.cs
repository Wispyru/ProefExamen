using UnityEngine;

[System.Serializable]
public class TipTier
{
    public float MinimumTime;
    public float TipAmount;
}

public class PlayerGradingSystem : MonoBehaviour
{
    private float _dishCost;

    private CustomerData _customerData;

    [Header("Tip Settings")]
    [SerializeField] private TipTier[] _tipTiers;

    private float _calculatedTip;

    private void Start()
    {
        _customerData = GetComponent<CustomerData>();
    }

    /// <summary>
    /// Calculates the tip based on remaining customer time
    /// </summary>
    public void CalculateTip(float remainingTime)
    {
        Debug.Log("Calculating tip. Remaining time: " + remainingTime);

        for (int i = 0; i < _tipTiers.Length; i++)
        {
            if (remainingTime >= _tipTiers[i].MinimumTime)
            {
                _calculatedTip = _tipTiers[i].TipAmount;
                break;
            }
        }

        float multiplier = _customerData.GetTipMultiplier();
        _calculatedTip *= multiplier;

        _calculatedTip = Mathf.Round(_calculatedTip * 100f) / 100f;

        _customerData.SetTip(_calculatedTip);

        Debug.Log("Final tip after multiplier: $" + _calculatedTip);
    }

    /// <summary>
    /// Handles customer payment
    /// </summary>
    public void CustomerPayment()
    {
        float totalPayment = _dishCost + _customerData.TotalTip;

        GameData.Money += totalPayment;
        GameData.TipMoney += _customerData.TotalTip;

        Debug.Log("Customer paid: $" + totalPayment);
    }
}

