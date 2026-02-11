using UnityEngine;

public class CustomerData : MonoBehaviour
{
    public CustomerTypeEnum CustomerType;
    public float CustomerTime;
    public float MiniumTip;
    public float MaxiumTip;
    private CustomerTimer _customerTimer;
    public float CustomerTip;


    public void CalculateTip()
    {
        CustomerTip = Random.Range(MiniumTip, MaxiumTip);
    }

    private void Start()
    {
        //_customerTimer = GetComponent<CustomerTimer>();
    }

    private void Update()
    {

    }

}
