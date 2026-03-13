using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class CameraPan : MonoBehaviour
{
    private Camera _mainCam;
    private Vector3 _cursorOrigin;
    private float _groundZ = 0;

    private void Start()
    {
        _mainCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (GameData.CurrentGameState == GameState.IdleMode) CameraPanning();
    }

    public void CameraPanning()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _cursorOrigin = getWorldPosition(_groundZ);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 _direction = _cursorOrigin - getWorldPosition(_groundZ);
            Vector3 _camPos = _mainCam.transform.position;
            _camPos += new Vector3(_direction.x, 0, 0);
            _mainCam.transform.position = _camPos;
        }
    }

    private Vector3 getWorldPosition(float z)
    {
        Ray _mousePos = _mainCam.ScreenPointToRay(Input.mousePosition);
        Plane _ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float _distance;
        _ground.Raycast(_mousePos, out _distance);
        return _mousePos.GetPoint(_distance);
    }
}
