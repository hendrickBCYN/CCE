using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera3D : ICamera
{
    [SerializeField] float _zoomSpeed;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _panSpeed;

    private float _distToTargetInitial = 10f;
    private Quaternion _rotationInitial = Quaternion.Euler(25, 0, 0);

    private float _distToTarget = 10f;
    private float _distToTargetMin = 0.5f;
    private float _distToTargetMax = 15;

    override
    public void Start()
    {
        base.Start();
        _id = 1;
        UpdateTransparent();
    }

    override
    public void UpdateCam()
    {
        if (_controls != null)
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
        }
    }

    private void OnEnable()
    {
        UpdateTransparent();
    }

    /// <summary>
    /// Zoom function with the field of view
    /// </summary>
    private void Zoom()
    {
        float zoomDelta = _controls.ZoomInput * _zoomSpeed * Time.deltaTime;
        _distToTarget = (transform.position - _target.transform.position).magnitude - zoomDelta;

        //Debug.Log($"NewZoomDistance = {newZoomDistance} - newDistToTarget = {newDistToTarget}"); 
        _distToTarget = Mathf.Clamp(_distToTarget, _distToTargetMin, _distToTargetMax);

        transform.position = _target.transform.position - transform.forward * _distToTarget;
    }

    override
    /// <summary>
    /// Move the camera 
    /// </summary>
    protected void Move()
    {
        Vector3 movement = -transform.right * (_controls.PanInputX * _panSpeed * Time.deltaTime) + -transform.up * (_controls.PanInputY * _panSpeed * Time.deltaTime);
        _target.transform.position += movement;
        transform.position += movement;
    }

    private void Rotate()
    {
        float verticalRotation = Mathf.Clamp(transform.eulerAngles.x - _controls.PanInputY * _rotationSpeed * Time.deltaTime, 0f, 90f);
        float horizontalRotation = transform.eulerAngles.y + _controls.PanInputX * _rotationSpeed * Time.deltaTime;

        Quaternion newRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        Vector3 newPosition = _target.transform.position - newRotation * Vector3.forward * _distToTarget;

        transform.rotation = newRotation;
        transform.position = newPosition;
    }

    override
    public void Reset()
    {
        transform.rotation = _rotationInitial;
        transform.position = _target.transform.position - transform.forward * _distToTargetInitial;
        
        GetComponent<Camera>().fieldOfView = 60.0f;
        UpdateTransparent();
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
        return _distToTarget;
    }
    override
    public void SetDistToTarget(float dist)
    {
        _distToTarget = dist;
        transform.position = _target.transform.position - transform.forward * _distToTarget;
    }

    public override Vector3 GetPlaneForwardVector()
    {
        Debug.Log("GetPlaneForwardVector");
        Vector3 l_result = transform.rotation.eulerAngles.x >= 45 ? transform.up : transform.forward;
        Debug.Log("Original = " + l_result);
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