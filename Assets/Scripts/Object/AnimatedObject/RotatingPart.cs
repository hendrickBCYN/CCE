using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPart: AnimatedPart
{
    [SerializeField] float _maxAngle = 90f;
    private bool _opened = false;
    private Quaternion _initialRotation = Quaternion.identity;
    private bool _inProgress = false;
    override public void StartStopAnimation()
    {
        if (_opened)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if(!_inProgress)
            StartCoroutine(RoutineOpen());
    }

    public void Close()
    {
        if(!_inProgress)
            StartCoroutine(RoutineClose());
    }

    IEnumerator RoutineOpen()
    {
        //Debug.Log("StartRoutine Open Door");
        _inProgress = true;
        _elapsedTime = 0;
        _initialRotation = _animatedObject.transform.localRotation;
        while (_elapsedTime <= _animationDuration)
        {
            float l_newAngle = Mathf.LerpAngle(0, _maxAngle, _elapsedTime / _animationDuration);
            if (_animationInverted)
                l_newAngle *= -1;

            Vector3 l_rotAxis = _animatedObjectZUp ? transform.forward : transform.up;
            //Debug.Log("RotAxis=" + l_rotAxis.ToString()); 
            _animatedObject.transform.localRotation = _initialRotation * Quaternion.Euler(l_rotAxis * l_newAngle);

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        _opened = true;
        _inProgress = false;
        yield return null;
    }

    IEnumerator RoutineClose()
    {
        //Debug.Log("StartRoutine Close Door");
        _inProgress = true;
        _elapsedTime = 0;
        _initialRotation = _animatedObject.transform.localRotation;
        while (_elapsedTime <= _animationDuration)
        {
            float l_newAngle = Mathf.LerpAngle(0, _maxAngle, _elapsedTime / _animationDuration);

            if (_animationInverted)
                l_newAngle *= -1;

            Vector3 l_rotAxis = _animatedObjectZUp ? transform.forward : transform.up;
            _animatedObject.transform.localRotation = _initialRotation * Quaternion.Euler(l_rotAxis * -l_newAngle);

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _opened = false;
        _inProgress = false;
        yield return null;
    }
}
