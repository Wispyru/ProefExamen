using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Vector3 cursorOrigin;
    public float GroundZ = 0;

    private void Update()
    {
        CameraPanning();
    }

    private void CameraPanning()
    {
        if(Input.GetMouseButtonDown(0))
        {
            cursorOrigin = getWorldPosition(GroundZ);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = cursorOrigin - getWorldPosition(GroundZ);
            mainCam.transform.position += direction;
        }
    }

    private Vector3 getWorldPosition(float z)
    {
        Ray mousePos = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}
