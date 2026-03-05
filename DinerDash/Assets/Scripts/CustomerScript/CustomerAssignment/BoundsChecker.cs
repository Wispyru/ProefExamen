using System;
using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    private GameObject _customerGroup;
    private float _sphereRadius = 3f;

    private void Start()
    {
        _customerGroup = GetComponent<SelectCustomerGroup>().Group;
    }

    private void Update()
    {
        RaycastBoundsCheck();
    }

    private void RaycastBoundsCheck()
    {
       if (Physics.CheckSphere(_customerGroup.transform.position, _sphereRadius, 3)) return;
    }
}
