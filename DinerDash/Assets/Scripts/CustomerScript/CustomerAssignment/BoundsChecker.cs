using System;
using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    private GameObject _customerGroup;
    private float _sphereRadius = 3f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_customerGroup == null) return;
        RaycastBoundsCheck();
    }

    private void RaycastBoundsCheck()
    {
        _customerGroup = GetComponent<SelectCustomerGroup>().Group;
        if (Physics.CheckSphere(_customerGroup.transform.position, _sphereRadius, 3)) return;
    }
}
