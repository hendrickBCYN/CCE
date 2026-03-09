using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour, IWallEventListener
{
    private float _nextWallAngle;
    private float _rotation;
    [SerializeField] private float _length;
    private float _thickness;
    [SerializeField] private float _height;
    private float _heightProtection;
    private float _thicknessProtection;
    private float _height2D = 0.01f;

    private bool _isProtected = false;
    private bool _isExtruded = false;
    private bool _extrusionInitialized = false;

    //Main 3D Wall
    [SerializeField] private GameObject _wallMain;
    [SerializeField] private GameObject _wallExtraRight;
    [SerializeField] private GameObject _wallExtraTop;
    [SerializeField] private GameObject _wallExtraBottom;
    [SerializeField] private GameObject _wallProtection;
    [SerializeField] private GameObject _wallProtectionExtra;
    [SerializeField] private GameObject _wall2D;
    [SerializeField] private GameObject _wall2DExtra;
    [SerializeField] private WallEvent _wallEvent;

    //Extrusion
    [SerializeField] private float _extrusionLength;
    [SerializeField] private float _extrusionHeight;
    [SerializeField] private Vector2 _extrusionBase;

    public Material _currentMaterial;
    [SerializeField] private Material _transparentMaterial = null;
    [SerializeField] private Material _protectedMaterial = null;



    private int _wallID = -1;
    private string _wallName = "Wall";

    private bool _isTransparent = false;

    public void Init(int p_ID, string p_name, bool p_extruded)
    {
        _wallID = p_ID;
        _wallName = p_name;
        _isExtruded = p_extruded;
        this.name = _wallName;
        _wallEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if(_wallEvent != null)
            _wallEvent.UnregisterListener(this);
    }

    public void InitWall(float length, float rotation, float nextWall, float height, float thickness, float heightProtection, float thicknessProtection)
    {
        _length = length;
        _nextWallAngle = nextWall;
        _rotation = rotation;
        _height = height;
        _thickness = thickness;
        _heightProtection = heightProtection;
        _thicknessProtection = thicknessProtection;
        _isProtected = false;

        WallProtection(false);

        if (_isExtruded)
        {
            InitExtrusion(0.5f, 0.5f, 0f, 0.5f);
        }

        if (_currentMaterial == null)
            _currentMaterial = _wallMain.GetComponent<Renderer>().material;
        if (_protectedMaterial == null)
        {
            if(_wallProtection.GetComponentInChildren<Renderer>() != null)
                _protectedMaterial = _wallProtection.GetComponentInChildren<Renderer>().material;
        }

        transform.rotation = Quaternion.Euler(0, _rotation, 0);
        UpdateTransforms();
    }

    public void UpdateWall(float length, float rotation, float nextWall, float height, float thickness, float heightProtection, float thicknessProtection)
    {
        _length = length;
        _nextWallAngle = nextWall;
        _rotation = rotation;
        _height = height;
        _thickness = thickness;
        _heightProtection = heightProtection;
        _thicknessProtection = thicknessProtection;

        if (_isExtruded)
        {
            UpdateExtrusion();
        }

        transform.rotation = Quaternion.Euler(0, _rotation, 0);
        UpdateTransforms();
    }
    

    protected virtual void UpdateTransforms()
    {
        if (!_isExtruded)
        {
            _wallMain.transform.localScale = new Vector3(_length, _height, _thickness);
            _wallMain.transform.localPosition = new Vector3(0, _height / 2f, _thickness / 2f);
            _wall2D.transform.localScale = new Vector3(_length, _height2D, _thickness);
            _wall2D.transform.localPosition = new Vector3(0, _height2D / 2f, _thickness / 2f);
            _wallProtection.transform.localScale = new Vector3(_length, _heightProtection, _thicknessProtection);
            _wallProtection.transform.localPosition = new Vector3(0, _heightProtection / 2f, _thicknessProtection / 5f);
        }
        else
        {
            if (_wallMain.activeInHierarchy)
            {
                _wall2D.transform.localScale = new Vector3(_wallMain.transform.localScale.x, _height2D, _thickness);
                _wall2D.transform.localPosition = new Vector3(_wallMain.transform.localPosition.x, _height2D / 2f, _thickness / 2f);
                _wallProtection.transform.localScale = new Vector3(_wallMain.transform.localScale.x, _heightProtection, _thicknessProtection);
                _wallProtection.transform.localPosition = new Vector3(_wallMain.transform.localPosition.x, _heightProtection / 2f, -_thicknessProtection / 2f);
            }
            if(_wall2DExtra != null && _wallExtraRight != null && _wallExtraRight.activeInHierarchy)
            {
                _wall2DExtra.transform.localScale = new Vector3(_wallExtraRight.transform.localScale.x, _height2D, _thickness);
                _wall2DExtra.transform.localPosition = new Vector3(_wallExtraRight.transform.localPosition.x, _height2D / 2f, _thickness / 2f);
                _wallProtectionExtra.transform.localScale = new Vector3(_wallExtraRight.transform.localScale.x, _heightProtection, _thicknessProtection);
                _wallProtectionExtra.transform.localPosition = new Vector3(_wallExtraRight.transform.localPosition.x, _heightProtection / 2f, -_thicknessProtection / 2f);
            }
        }
    }

    public void UpdateWall(float length)
    {
        _length = length;
        UpdateTransforms();
        WallProtection(_isProtected);
    }

    public void ActivateWallProtection(GameObject p_protectionObject)
    {
        WallProtection(p_protectionObject != null);
    }

    public void ActivateWallProtectionNull()
    {
        WallProtection(false);
    }

    public void WallProtection(bool isProtected)
    {
        _isProtected = isProtected;
        _wallProtection.SetActive(isProtected);
        if(_wallProtectionExtra != null)
            _wallProtectionExtra.SetActive(isProtected);
        if(_isProtected)
            UpdateWallProtectionMaterialTiling();
    }

    public void SetCurrentMaterial(Material p_material)
    {
        _currentMaterial = p_material;
        UpdateMaterials();
    }

    public void ChangeMaterial(Material m)
    {
        _currentMaterial = m;
        if (!_isTransparent)
        {
            _wallMain.GetComponent<Renderer>().material = m;
            if(_isExtruded)
            {
                if(_wallExtraBottom != null)
                    _wallExtraBottom.GetComponent<Renderer>().material = m;
                if (_wallExtraTop != null)
                    _wallExtraTop.GetComponent<Renderer>().material = m;
                if (_wallExtraRight != null)
                    _wallExtraRight.GetComponent<Renderer>().material = m;
            }
        }
        _wall2D.GetComponent<Renderer>().material = m;
        if(_isExtruded && _wall2DExtra != null)
            _wall2DExtra.GetComponent<Renderer>().material = m;
    }

    public void SetCurrentProtectionMaterial(Material p_material)
    {
        _protectedMaterial = p_material;
        UpdateMaterials();
        UpdateWallProtectionMaterialTiling();
    }
    public void ChangeProtectedMaterial(Material m)
    {
        _protectedMaterial = m;
        if (!_isTransparent)
        {
            if(_wallProtection.GetComponentInChildren<Renderer>() != null)
                _wallProtection.GetComponentInChildren<Renderer>().material = m;
            if(_isExtruded && _wallProtectionExtra != null && _wallProtectionExtra.GetComponentInChildren<Renderer>())
                _wallProtectionExtra.GetComponentInChildren<Renderer>().material = m;
            UpdateWallProtectionMaterialTiling();
        }
    }

    void UpdateWallProtectionMaterialTiling()
    {
        if (_isProtected)
        {
            if (_wallProtection.GetComponentInChildren<Renderer>() != null)
                _wallProtection.GetComponentInChildren<Renderer>().material.mainTextureScale = new(_wallProtection.transform.localScale.x, _heightProtection);
            if (_isExtruded && _wallProtectionExtra != null && _wallProtectionExtra.GetComponentInChildren<Renderer>())
                _wallProtectionExtra.GetComponentInChildren<Renderer>().material.mainTextureScale = new(_wallProtectionExtra.transform.localScale.x, _heightProtection);
        }
    }

    void UpdateMaterials()
    {
        if (_isTransparent)
        {
            SetTransparent();
        }
        else UnsetTransparent();
    }

    public void SetTransparent()
    {
        if (_wallMain != null)
        {
            _wallMain.GetComponent<Renderer>().material = _transparentMaterial;
            if (_wall2D != null)
                _wall2D.GetComponent<Renderer>().material = _currentMaterial;
            if (_wallProtection != null && _wallProtection.GetComponentInChildren<Renderer>())
                _wallProtection.GetComponentInChildren<Renderer>().material = _transparentMaterial;
            if (_isExtruded)
            {
                if (_wall2DExtra != null)
                    _wall2DExtra.GetComponent<Renderer>().material = _currentMaterial;
                if (_wallExtraBottom != null)
                    _wallExtraBottom.GetComponent<Renderer>().material = _transparentMaterial;
                if (_wallExtraTop != null)
                    _wallExtraTop.GetComponent<Renderer>().material = _transparentMaterial;
                if (_wallExtraRight != null)
                    _wallExtraRight.GetComponent<Renderer>().material = _transparentMaterial;
                if(_wallProtectionExtra != null && _wallProtectionExtra.GetComponentInChildren<Renderer>())
                    _wallProtectionExtra.GetComponentInChildren<Renderer>().material = _transparentMaterial;
            }
            _isTransparent = true;
        }
    }

    public void UnsetTransparent()
    {
        if (_wallMain != null)
        {
            _wallMain.GetComponent<Renderer>().material = _currentMaterial;
            if(_wall2D != null)
                _wall2D.GetComponent<Renderer>().material = _currentMaterial;
            if (_wallProtection != null && _wallProtection.GetComponentInChildren<Renderer>() != null)
                _wallProtection.GetComponentInChildren<Renderer>().material = _protectedMaterial;
            if (_isExtruded)
            {
                if(_wall2DExtra != null)
                    _wall2DExtra.GetComponent<Renderer>().material = _currentMaterial;
                if (_wallExtraBottom != null)
                    _wallExtraBottom.GetComponent<Renderer>().material = _currentMaterial;
                if (_wallExtraTop != null)
                    _wallExtraTop.GetComponent<Renderer>().material = _currentMaterial;
                if (_wallExtraRight != null)
                    _wallExtraRight.GetComponent<Renderer>().material = _currentMaterial;
                if (_wallProtectionExtra != null && _wallProtectionExtra.GetComponentInChildren<Renderer>() != null)
                    _wallProtectionExtra.GetComponentInChildren<Renderer>().material = _protectedMaterial;
            }
            UpdateWallProtectionMaterialTiling();
            _isTransparent = false;
        }
    }

    public float GetNextWallAngle() { return _nextWallAngle; }
    public float GetRotation() { return _rotation; }
    public float GetLength() { return _length; }
    public float GetThickness() { return _thickness; }
    public float GetHeight() { return _height; }

    public Material GetCurrentMaterial() { return _currentMaterial; }

    public bool IsProtected() { return _isProtected; }

    public Material GetProtectionMaterial()
    {
        if (_isProtected)
        {
            return _protectedMaterial;
        }

        return null;
    }

    void IWallEventListener.OnWallEventRaised(int p_wallID, bool p_protected)
    {
        
    }

    void IWallEventListener.OnWallDimensionsEventRaised(float p_height, float p_thickness, float p_protectionHeight)
    {
        _height = p_height;
        _thickness = p_thickness;
        _heightProtection = p_protectionHeight;
        UpdateTransforms();
    }

    #region EXTRUSION
    public void InitExtrusion(float p_extrusionLength, float p_extrusionHeight, float p_extrusionBaseX, float p_extrusionBaseY)
    {
        _extrusionLength = p_extrusionLength;
        _extrusionHeight = p_extrusionHeight;
        _extrusionBase = new Vector2(p_extrusionBaseX, p_extrusionBaseY);

        if(_extrusionLength >= _length)
        {
            Debug.LogWarning("WARNING: Extrusion length is greater than wall length !");
            return;
        }    
        if(_extrusionHeight >= _height)
        {
            Debug.LogWarning("WARNING: Extrusion height is greater than wall height !");
            return;
        }

        _extrusionInitialized = true;
        UpdateExtrusion();
    }

    public void UpdateExtrudedWall(float p_wallWidth, Vector3 p_extrusionDimensions, Vector3 p_extrusionLocalPosition)
    {
        _length = p_wallWidth;
        _extrusionLength = p_extrusionDimensions.x;
        _extrusionHeight = p_extrusionDimensions.y;
        _extrusionBase = p_extrusionLocalPosition;

        UpdateExtrusion();
    }

    public void UpdateExtrusion()
    {
        float l_rightWidth = _length - (_extrusionBase.x + (_extrusionLength / 2f));
        float l_leftWidth = _extrusionBase.x - (_extrusionLength / 2f);

        if (l_leftWidth > 0)
        {
            _wallMain.transform.localScale = new Vector3(l_leftWidth, _height, _thickness);
            _wallMain.transform.localPosition = new Vector3((-_length + l_leftWidth) / 2f, _height / 2f, _thickness / 2f);
        }

        if (l_rightWidth > 0)
        {
            if (_wallExtraRight == null)
            {
                _wallExtraRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _wallExtraRight.transform.parent = transform;
                _wallExtraRight.name = "ExtraRight";
                _wallExtraRight.layer = LayerMask.NameToLayer("CamLayer3D");
                _wallExtraRight.tag = "Wall";
                _wallExtraRight.AddComponent<BoxCollider>();
            }
            if(_wall2DExtra == null)
            {
                _wall2DExtra = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _wall2DExtra.transform.parent = transform;
                _wall2DExtra.name = "2DExtra";
                _wall2DExtra.layer = LayerMask.NameToLayer("CamLayer");
            }
            if(_wallProtectionExtra == null)
            {
                _wallProtectionExtra = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _wallProtectionExtra.transform.parent = transform;
                _wallProtectionExtra.name = "ProtectionExtra";
                _wallProtectionExtra.tag = "Protection";
                _wallProtectionExtra.SetActive(_isProtected);
            }

            _wallExtraRight.SetActive(true);
            _wallExtraRight.transform.localScale = new Vector3(l_rightWidth, _height, _thickness);
            _wallExtraRight.transform.localPosition = new Vector3((_length - l_rightWidth) / 2f, _height / 2f, _thickness / 2f);
        }
        else
        {
            if (_wallExtraRight != null)
                _wallExtraRight.SetActive(false);
        }

        float l_topHeight = _height - _extrusionBase.y - _extrusionHeight;
        float l_bottomHeight = _extrusionBase.y;

        if (l_topHeight > 0)
        {
            if (_wallExtraTop == null)
            {
                _wallExtraTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _wallExtraTop.transform.parent = transform;
                _wallExtraTop.name = "ExtraTop";
                _wallExtraTop.layer = LayerMask.NameToLayer("CamLayer3D");
                _wallExtraTop.tag = "Wall";
                _wallExtraTop.AddComponent<BoxCollider>();
            }
            _wallExtraTop.SetActive(true);
            _wallExtraTop.transform.localScale = new Vector3(_extrusionLength, l_topHeight, _thickness);
            _wallExtraTop.transform.localPosition = new Vector3(_extrusionBase.x - _length / 2f, _height - l_topHeight / 2f, _thickness / 2f);
        }
        else
        {
            if (_wallExtraTop != null)
                _wallExtraTop.SetActive(false);
        }

        if (l_bottomHeight > 0)
        {
            if (_wallExtraBottom == null)
            {
                _wallExtraBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _wallExtraBottom.transform.parent = transform;
                _wallExtraBottom.name = "ExtraBottom";
                _wallExtraBottom.layer = LayerMask.NameToLayer("CamLayer3D");
                _wallExtraBottom.tag = "Wall";
                _wallExtraBottom.AddComponent<BoxCollider>();
            }
            _wallExtraBottom.SetActive(true);
            _wallExtraBottom.transform.localScale = new Vector3(_extrusionLength, l_bottomHeight, _thickness);
            _wallExtraBottom.transform.localPosition = new Vector3(_extrusionBase.x - _length / 2f, l_bottomHeight / 2f, _thickness / 2f);
        }
        else
        {
            if (_wallExtraBottom != null)
                _wallExtraBottom.SetActive(false);
        }


        UpdateTransforms();
    }

    public bool IsExtruded => _isExtruded;
    public bool IsExtrusionInitialized => _extrusionInitialized;


    #endregion
}
