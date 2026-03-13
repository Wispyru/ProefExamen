using System;
using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    
    private Camera _mainCam;
    private GameData _gameData;
    private int _zoomBounds = 0;

   

    private void Awake()
    {
        _mainCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if(GameData.CurrentGameState == GameState.IdleMode) CameraZoomBounds();
    }

    private void CameraZoomBounds()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && _zoomBounds >= -4)
        {
            ZoomIn();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && _zoomBounds <= 4)
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        _mainCam.transform.position = new Vector3(_mainCam.transform.position.x, _mainCam.transform.position.y - .5f, _mainCam.transform.position.z + .5f);
        _mainCam.transform.Rotate(-2,0,0);
        _zoomBounds--;
    }
    private void ZoomOut()
    {
        _mainCam.transform.position = new Vector3(_mainCam.transform.position.x, _mainCam.transform.position.y + .5f, _mainCam.transform.position.z - .5f);
        _mainCam.transform.Rotate(2,0,0);
        _zoomBounds++;
    }
}