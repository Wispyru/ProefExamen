using UnityEngine;

public class PlayerGradingSystem : MonoBehaviour
{
    private float _dishCost;
    public GameData GameData;
    private CustomerData _customerData;

    public void Start()
    {

    }

    public void CustomerPayment()
    {
        GameData.Money += _dishCost;
    }

    /*public void CalculateTip()
    {
        TotalTip = Random.Range(MiniumTip, MaximumTip);

        Debug.Log(TotalTip.ToString()); // Delete later
    }
    */

}
