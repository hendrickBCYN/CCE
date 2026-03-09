using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraImmersive : ICamera
{
    [SerializeField] float _mouseSensitivity;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _fovSpeed = 2f;
    [SerializeField] float _fovMin = 45f;
    [SerializeField] float _fovMax = 100f;

    override
    public void Start()
    {
        base.Start();
        _id = 2;
    }

    private void OnEnable()
    {
        UpdateTransparent();
    }

    override
    public void UpdateCam()
    {

        if(_controls != null)
        {
            if (_controls.ZoomInput != 0 && _controls.isInViewport && !_controls.isOverUI)
            {
                Zoom();
                UpdateTransparent();
            }

            if (_controls.LeftClickHold() && _controls.isInViewport && !_controls.isOverUI)
            {
                Rotate();
                UpdateTransparent();
            }

            if (_controls.Reset())
            {
                Reset();
                UpdateTransparent();
            }

            Move();
        }
    }

    /// <summary>
    /// The rotation movement for this camera
    /// </summary>
    private void Rotate()
    {
        float panAmountX = _controls.PanInputX * _mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, panAmountX);
        float panAmountY = _controls.PanInputY * _mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.right, -panAmountY);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
    }

    override
    /// <summary>
    /// The movement of the camera with the Input System
    /// </summary>
    protected void Move()
    {
        if (_controls.RightArrow())
        {
            Vector3 direction = transform.right;
            direction.y = 0;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        if (_controls.LeftArrow())
        {
            Vector3 direction = -transform.right;
            direction.y = 0;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        if (_controls.UpArrow())
        {
            Vector3 direction = transform.forward;
            direction.y = 0;
            Debug.Log("Direction = " + direction.ToString());
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        if (_controls.DownArrow())
        {
            Vector3 direction = -transform.forward;
            direction.y = 0;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

    }

    /// <summary>
    /// Zoom function with the field of view
    /// </summary>
    private void Zoom()
    {
        float zoomDelta = _controls.ZoomInput * _fovSpeed * Time.deltaTime;
        float fov = GetFieldOfView() - zoomDelta;

        //Debug.Log($"NewZoomDistance = {newZoomDistance} - newDistToTarget = {newDistToTarget}"); 
        fov = Mathf.Clamp(fov, _fovMin, _fovMax);
        SetFieldOfView(fov);

    }

    override
    public void Reset()
    {
        transform.position = new Vector3(0.0f, 1.6f, 0.0f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        SetFieldOfView(60f);
    }

    override
    public float GetFieldOfView()
    {
        return GetComponent<Camera>().fieldOfView;
    }

    override
    public void SetFieldOfView(float zoom)
    {
        GetComponent<Camera>().fieldOfView = zoom;
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
        Vector3 l_result = transform.rotation.eulerAngles.x >= 45 ? transform.up : transform.forward;
        l_result.y = 0;
        if (Mathf.Abs(l_result.x) - Mathf.Abs(l_result.z) >= 0)
        {
            l_result.x = Mathf.Sign(l_result.x);
            l_result.z = 0;
        }
        else
        {
            l_result.x = 0;
            l_result.z = Mathf.Sign(l_result.z);
        }

        Debug.Log("Final = " + l_result);
        return l_result;
    }

    public override Vector3 GetPlaneRightVector()
    {
        Vector3 l_result = transform.right;
        l_result.y = 0;
        if (Mathf.Abs(l_result.x) - Mathf.Abs(l_result.z) >= 0)
        {
            l_result.x = Mathf.Sign(l_result.x);
            l_result.z = 0;
        }
        else
        {
            l_result.x = 0;
            l_result.z = Mathf.Sign(l_result.z);
        }

        return l_result;
    }
}
