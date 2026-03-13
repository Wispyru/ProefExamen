using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerGeneration : MonoBehaviour
{
    // serialized variables
    [SerializeField]
    private GameObject _customersToSpawn;
    [SerializeField]
    private GameObject[] _customerList;

    // private variables
    private Transform[] _customerSlot;
    private int _minCustomers = 0;
    private int _maxCustomers = 4;
    private int _customerSelectionIndex;
    private List<int> selectedCustomers;

    // public variables
    public int SpawnedCustomerAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedCustomers = new List<int>();
        SpawnedCustomerAmount = Random.Range(_minCustomers, _maxCustomers);
        _customerSlot = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < _customerSlot.Length; i++)
        {
            _customerSlot[i] = gameObject.transform.GetChild(i);
        }

        SpawnCustomers();
    }

    public void SpawnCustomers()
    {
        for (int i = 0; i <= SpawnedCustomerAmount; i++)
        {
            Debug.Log("Spawn Customer");
            
            SelectCustomer();
            Instantiate(_customersToSpawn, _customerSlot[i].position, _customerSlot[i].rotation, _customerSlot[i]);
        }
    }

    private void SelectCustomer()
    {
        _customerSelectionIndex = Random.Range(0, 3);
        if (selectedCustomers.Contains(_customerSelectionIndex)) return;
        
        selectedCustomers.Add(_customerSelectionIndex); 
        _customersToSpawn = _customerList[_customerSelectionIndex];
    }
}
