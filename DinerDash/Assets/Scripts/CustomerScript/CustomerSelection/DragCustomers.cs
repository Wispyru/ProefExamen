using UnityEngine;
using UnityEngine.UIElements;

public class DragCustomers : MonoBehaviour
{
    private Camera _camera;
    private SelectCustomerGroup _customerGroup;
    private Vector2 _fingerPosition;
    private Vector3 _position;
    private float _distanceFromCamera = 12.5f;


    private void Start()
    {
        _customerGroup = GetComponent<SelectCustomerGroup>();
        _camera = Camera.main;
    }

    public void GetMousePosition()
    {

        _fingerPosition.x = Input.mousePosition.x;
        _fingerPosition.y = Input.mousePosition.y;


        _position = _camera.ScreenToWorldPoint(new Vector3(_fingerPosition.x, _fingerPosition.y, _distanceFromCamera));
        MoveCustomerGroup(_position);
    }

    private void MoveCustomerGroup(Vector3 newPosition)
    {
        _customerGroup.Group.transform.position = newPosition;
    }
}
