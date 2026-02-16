using UnityEngine;

public class CustomerData : MonoBehaviour
{
    public CustomerTypeEnum CustomerType;
    public float CustomerTime;
    public float MiniumTip;
    public float MaximumTip;
    private CustomerTimer _customerTimer;
    public float TotalTip;


    /*private void Start()
    {
        CalculateTip();
        _customerTimer = GetComponent<CustomerTimer>();
    }

    private void Update()
    {

    }*/

}
