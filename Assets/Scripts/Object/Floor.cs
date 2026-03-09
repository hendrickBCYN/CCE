using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Floor : MonoBehaviour
{
    public Material _currentMaterial;

    private float _width;
    private float _length;
    private float _thickness = 0.01f;


    public void InitFloor(float width, float length)
    {
        if (_currentMaterial == null)
            _currentMaterial = GetComponent<Renderer>().material;

        UpdateTransforms();
    }

    void UpdateTransforms()
    {
        transform.position = new Vector3(0, -_thickness/2f, 0);
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(_width, _thickness, _length);
    }

    public void UpdateFloor(float p_width, float p_length)
    {
        _length = p_length;
        _width = p_width;
        UpdateTransforms();
    }

    public void ChangeMaterial(Material m)
    {
        _currentMaterial = m;
        GetComponent<Renderer>().material = m;
    }

    public void UpdateMaterialTiling(float x, float y)
    {
        GetComponent<Renderer>().material.mainTextureScale = new(x, y);
    }

    public float GetLength() { return _length; }

    public float GetWidth() { return _width; }

    public Material GetCurrentMaterial() { return _currentMaterial; }

}
