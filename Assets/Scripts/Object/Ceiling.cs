using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ceiling : MonoBehaviour
{
    [SerializeField] Material _currentMaterial;
    [SerializeField] Material _transparentMaterial;

    private float _width;
    private float _length;
    private float _thickness = 0.01f;
    private bool _isFalseCeiling = false;
    private bool _isTransparent = false;

    public void InitCeiling(float p_width, float p_length, float p_wallThickness, bool p_falseCeiling)
    {
        if (_currentMaterial == null)
            _currentMaterial = GetComponent<Renderer>().material;

        _isFalseCeiling = p_falseCeiling;

        UpdateCeiling(p_width, p_length, p_wallThickness);
    }

    void UpdateTransforms()
    {
        transform.localPosition = new Vector3(0, _thickness/2f, 0);
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(_width, _thickness, _length);
    }

    public void UpdateCeiling(float p_width, float p_length, float p_wallThickness)
    {
        _length = p_length + 2 * p_wallThickness;
        _width = p_width + 2 * p_wallThickness;
        UpdateTransforms();
    }

    public void UpdateCeilingThickness(float p_thickness)
    {
        _thickness = p_thickness;
        UpdateTransforms();
    }

    /*public void ChangeMaterial(Material m)
    {
        _currentMaterial = m;
        GetComponent<Renderer>().material = m;
    }

    public void UpdateMaterialTiling(float x, float y)
    {
        GetComponent<Renderer>().material.mainTextureScale = new(x, y);
    }*/

    public void SetTransparent()
    {
        gameObject.SetActive(_isFalseCeiling);
        GetComponent<Renderer>().material = _transparentMaterial;
        _isTransparent = true;
    }

    public void UnsetTransparent()
    {
        gameObject.SetActive(true);
        GetComponent<Renderer>().material = _currentMaterial;
        _isTransparent = false;
    }

    public float GetLength() { return _length; }

    public float GetWidth() { return _width; }

    public Material GetCurrentMaterial() { return _currentMaterial; }

}
