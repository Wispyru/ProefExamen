using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;

public class DragCustomers : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private float initialMouseY;
    private float initialDepth;

    
    [SerializeField] private float depthSensitivity = 0.1f;

    private void OnMouseDown()
    {

        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        initialMouseY = Input.mousePosition.y;
        initialDepth = screenPoint.z;
    }

    private void OnMouseDrag()
    {

        float mouseDeltaY = Input.mousePosition.y - initialMouseY;
        float newDepth = initialDepth + (mouseDeltaY * depthSensitivity);


        newDepth = Mathf.Max(0.1f, newDepth);

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, newDepth);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
}
