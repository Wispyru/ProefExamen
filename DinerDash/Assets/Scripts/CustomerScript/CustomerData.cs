using UnityEngine;

public class CustomerData : MonoBehaviour
{
    public CustomerTypeEnum CustomerType;


    //Group Variables
    public int CustomerAmount;

    //tip variables
    public float MiniumTip;
    public float MaximumTip;
    public float TotalTip;

    //timer variables
    public float CustomerTime;

    //private variables
    private CustomerTimer _customerTimer;

    /*private void Start()
    {
        CalculateTip();
        _customerTimer = GetComponent<CustomerTimer>();
    }

    private void Update()
    {

    }*/

}
