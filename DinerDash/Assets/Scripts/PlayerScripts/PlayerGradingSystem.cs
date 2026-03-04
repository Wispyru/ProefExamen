using UnityEngine;

public class PlayerGradingSystem : MonoBehaviour
{
    private float _dishCost;
    public GameData GameData;
    public CustomerData _customerData;
    private float totalTip;

    public void Start()
    {
        CalculateTip();
        _customerData = GetComponent<CustomerData>(); // fills in the empty slot of CustomerData in 
    }

    public void CustomerPayment()
    {
        GameData.Money += _dishCost;
    }

    public void CalculateTip()
    {
        Debug.Log("Started caluclating");
        totalTip = Random.Range(CustomerData.MiniumTip, CustomerData.MaximumTip);
        CustomerData.TotalTip = totalTip;
        totalTip = Mathf.Round(totalTip * 100f) / 100f;
        Debug.Log("total tip is " + totalTip);
    }

    /*public void CalculateTip()
    {
        TotalTip = Random.Range(MiniumTip, MaximumTip);

        Debug.Log(TotalTip.ToString()); // Delete later
    }*/


}
