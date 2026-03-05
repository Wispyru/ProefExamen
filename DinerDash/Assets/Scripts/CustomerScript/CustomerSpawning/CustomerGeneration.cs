using System.Collections.Generic;
using UnityEngine;

public class CustomerGeneration : MonoBehaviour
{
    // serialized variables
    [SerializeField]
    private GameObject _customersToSpawn;

    // private variables
    private Transform[] _customerSlot;
    private int _minCustomers = 0;
    private int _maxCustomers = 4;

    // public variables
    public int SpawnedCustomerAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Instantiate(_customersToSpawn, _customerSlot[i].position, _customerSlot[i].rotation, _customerSlot[i]);
        }
    }
}
