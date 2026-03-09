using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2D : ICamera
{

    [SerializeField] float _panSpeed;
    [SerializeField] float _zoomSpeed;

    override
    public void Start()
    {
        base.Start();
        _id = 0;
    }

    private void OnEnable()
    {
        UpdateTransparent();
    }

    override
    public void UpdateCam()
    {
        if(_controls!= null)
        {
            if (_controls.ZoomInput != 0 && _controls.isInViewport && !_controls.isOverUI)
            {
                Zoom();
            }

            if (_controls.LeftClickHold() && _controls.isInViewport && !_controls.isOverUI)
            {
                Move();
            }

            if (_controls.Reset())
            {
                Reset();
            }
        }
    }

    override
    /// <summary>
    /// Move the camera
    /// </summary>
    protected void Move()
    {
        float panAmountX = _controls.PanInputX * _panSpeed * Time.deltaTime;
        float panAmountY = _controls.PanInputY * _panSpeed * Time.deltaTime;
        Vector3 movement = new Vector3(-panAmountX, -panAmountY, 0f);
        transform.Translate(movement);
    }

    /// <summary>
    /// Zoom the ortho camera with the orthographicSize
    /// </summary>
    private void Zoom()
    {
        float zoomDelta = _controls.ZoomInput * _zoomSpeed * Time.deltaTime;
        float newZoomDistance = GetComponent<Camera>().orthographicSize - zoomDelta;

        float _minZoomDistance = 1.0f;
        float _maxZoomDistance = 100.0f;

        newZoomDistance = Mathf.Clamp(newZoomDistance, _minZoomDistance, _maxZoomDistance);

        GetComponent<Camera>().orthographicSize = newZoomDistance;

    }

    override
    public void Reset()
    {
        transform.position = new Vector3(0.0f, 20.0f, 0.0f);
        transform.transform.rotation = Quaternion.Euler(90f, 0f, 0);
        GetComponent<Camera>().orthographicSize = 5f;
        GetComponent<Camera>().nearClipPlane = 0f;
        GetComponent<Camera>().farClipPlane = 50f;
    }

    override
    public float GetFieldOfView()
    {
        return GetComponent<Camera>().orthographicSize;
    }

    override
    public void SetFieldOfView(float zoom)
    {
        GetComponent<Camera>().orthographicSize = zoom;
    }

    override
    public float GetDistToTarget()
    {
        return 0f;
    }

    override
    public void SetDistToTarget(float zoom)
    {
    }

    public override Vector3 GetPlaneForwardVector()
    {
        return transform.up;
    }

    public override Vector3 GetPlaneRightVector()
    {
        return transform.right;
    }
}
