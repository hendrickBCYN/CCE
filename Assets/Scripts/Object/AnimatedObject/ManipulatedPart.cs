using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ManipulatedPart: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private ManipulationManager.ManipulationPlane _manipulationPlane = ManipulationManager.ManipulationPlane.XZ;
    [SerializeField] private bool _manipulationAxisXFixed = false;
    [SerializeField] private bool _manipulationAxisYFixed = false;
    [SerializeField] private bool _manipulationAxisZFixed = false;
    [SerializeField] private ManipulationManager.ManipulationRotation _manipulationRotation = ManipulationManager.ManipulationRotation.Quarter;
    List<Transform> _limitPoints = new List<Transform>();
    bool _manipulationActive = false;
    Vector2 _startPointer;
    [SerializeField] ManipulationEvent _manipulationEvent;
    [SerializeField] bool _useExistingBounds = false;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] Dictionary<GameObject, GameObject> _checkCollider = new Dictionary<GameObject, GameObject>();
    [SerializeField] Dictionary<GameObject, GameObject> _checkColliderObj = new Dictionary<GameObject, GameObject>();
    [SerializeField] Dictionary<GameObject, Vector2> _collidingXZ = new Dictionary<GameObject, Vector2>();
    [SerializeField] bool _freeManipulation = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _manipulationEvent.RaiseBeginDrag(gameObject);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("ManipulatedPart Drag " + gameObject.name);
        _manipulationEvent.RaiseDrag(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _manipulationEvent.RaiseEndDrag(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
        if (_manipulationEvent != null)
            _manipulationEvent.RaiseClick(gameObject, (int)_manipulationPlane, _manipulationAxisXFixed, _manipulationAxisYFixed, _manipulationAxisZFixed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        UpdateLimitPoints();
        EnableLimitPoints(true);
        if (_manipulationEvent != null)
            _manipulationEvent.RaisePointer(gameObject, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        EnableLimitPoints(false);
        if (_manipulationEvent != null)
            _manipulationEvent.RaisePointer(gameObject, false);
    }

    private void Start()
    {
        UpdateLimitPoints();
        EnableLimitPoints(false);
    }

    private void Update()
    {
        
    }

    public void Reset()
    {
        UpdateLimitPoints();
        EnableLimitPoints(false);
        foreach (var v in _checkCollider.Values)
            v.SetActive(false);
        foreach (var v in _checkColliderObj.Values)
            v.SetActive(false);
    }

    void CreateLimitPoint(int p_index, Vector3 p_position)
    {
        int manipulationHelperLayer = LayerMask.NameToLayer("ManipulationHelper");
        if (p_index == _limitPoints.Count)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "LimitPoint" + _limitPoints.Count;
            go.transform.parent = transform;
            go.transform.position = p_position;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one / 10f;
            if (manipulationHelperLayer != -1)
                go.layer = LayerMask.NameToLayer("ManipulationHelper");
            go.GetComponent<Renderer>().enabled = false;

            _limitPoints.Add(go.transform);
        }
        else _limitPoints[p_index].position = p_position;
    }

    public void UpdateLimitPoints()
    {
        Bounds bounds = new Bounds();
        if (_useExistingBounds)
        {
            _boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
            if(_boxCollider != null )
            {
                bounds = _boxCollider.bounds;
            }
        }
        else
        {
            _boxCollider = gameObject.GetComponent<BoxCollider>();
            if (_boxCollider == null)
                _boxCollider = gameObject.AddComponent<BoxCollider>();
            bounds = Utility.CalculateLocalBounds(gameObject);
            _boxCollider.center = bounds.center;
            _boxCollider.size = bounds.size;

            bounds.center = transform.TransformPoint(bounds.center);
            bounds.size = transform.TransformDirection(bounds.size);
        }

        if(_boxCollider != null)
        {
            Vector3 extents = bounds.extents;
            Vector3 center = bounds.center;
            CreateLimitPoint(0, center + new Vector3(extents.x, 0, 0));
            CreateLimitPoint(1, center + new Vector3(-extents.x, 0, 0));
            CreateLimitPoint(2, center + new Vector3(0, 0, extents.z));
            CreateLimitPoint(3, center + new Vector3(0, 0, -extents.z));
            CreateLimitPoint(4, center + new Vector3(0, extents.y, 0));
            CreateLimitPoint(5, center + new Vector3(0, -extents.y, 0));
        }
    }

    void EnableLimitPoints(bool p_enable)
    {
        /*if (_boxCollider != null)
        {
            _boxCollider.enabled = p_enable;
            _boxCollider.isTrigger = p_enable;
        }*/
        foreach(var v in _checkCollider.Values)
            v.SetActive(p_enable);
        foreach (var v in _checkColliderObj.Values)
            v.SetActive(p_enable);

        foreach (Transform t in _limitPoints)
            t.gameObject.SetActive(p_enable);
    }

    public float GetAngleFromManipulationRotation()
    {
        float l_angle = 0;
        switch (_manipulationRotation)
        {
            case ManipulationManager.ManipulationRotation.One:
                l_angle = 360f / 1f;
                break;
            case ManipulationManager.ManipulationRotation.Half:
                l_angle = 360f / 2f;
                break;
            case ManipulationManager.ManipulationRotation.Quarter:
                l_angle = 360f / 4f;
                break;
            case ManipulationManager.ManipulationRotation.Eight:
                l_angle = 360f / 8f;
                break;
            case ManipulationManager.ManipulationRotation.Sixteenth:
                l_angle = 360f / 16f;
                break;
        }

        return l_angle;
    }

    public void ValidatePosition(ref Vector3 newWorldPosition, Vector3 roomDimensions, List<GameObject> obstacles)
    {
        Vector3 correction = Vector3.zero;
        foreach (Transform t in _limitPoints)
        {
            Vector3 position = transform.TransformDirection(t.localPosition) + newWorldPosition;

            //RoomBox check
            if (position.x > roomDimensions.x / 2f || position.x < -roomDimensions.x / 2f ||
                position.z > roomDimensions.y / 2f || position.z < -roomDimensions.y / 2f)
            {
                if (position.x > roomDimensions.x / 2f)
                {
                    //Debug.Log("X too big");
                    newWorldPosition.x -= (position.x - roomDimensions.x / 2f);
                }
                else if (position.x < -roomDimensions.x / 2f)
                {
                    //Debug.Log("X too small");
                    newWorldPosition.x += (-roomDimensions.x / 2f - position.x);
                }
                if (position.z > roomDimensions.z / 2f)
                {
                    //Debug.Log("Z too big");
                    newWorldPosition.z -= (position.z - roomDimensions.z / 2f);
                }
                else if (position.z < -roomDimensions.z / 2f)
                {
                    //Debug.Log("Z too small");
                    newWorldPosition.z += (-roomDimensions.z / 2f - position.z);
                }
                if (position.y > roomDimensions.y)
                {
                    //Debug.Log("Z too big");
                    newWorldPosition.y -= (position.y - roomDimensions.y / 2f);
                }
                else if (position.y < 0f)
                {
                    //Debug.Log("Z too small");
                    newWorldPosition.y += (0 - position.y);
                }
                //Debug.Log("LimitPoint " + t.name + " " + position.ToString("F6") + " vs " + (roomDimensions / 2f).ToString("F6"));
            }
        }

        if (_freeManipulation)
            return;

        bool tmpCollision = false;
        //Obstacles check
        List<BoxCollider> colliders = new List<BoxCollider>();
        int manipulationHelperLayer = LayerMask.NameToLayer("ManipulationHelper");
        foreach (GameObject obj in obstacles)
        {
            if (obj != gameObject && obj.transform.childCount > 0 && obj.GetComponentInChildren<Furniture>() != null)
            {
                Bounds bounds = obj.GetComponentInChildren<Furniture>().GetLocalBounds();// Utility.CalculateLocalBounds(obj);

                if (!_checkCollider.ContainsKey(obj))
                {
                    GameObject checkCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    checkCollider.name = "CheckCollider_" + gameObject.name + "_" + obj.name;
                    //_checkCollider.transform.parent = transform;
                    Color c = UIManager.Blue;
                    c.a = 0.75f;
                    checkCollider.GetComponent<Renderer>().material.color = c;
                    Utility.SetMaterialTransparent(checkCollider.GetComponent<Renderer>().material);
                    checkCollider.GetComponent<Renderer>().enabled = true;
                    if (manipulationHelperLayer != -1)
                        checkCollider.layer = LayerMask.NameToLayer("ManipulationHelper");
                    checkCollider.SetActive(false);
                    _checkCollider.Add(obj, checkCollider);
                }
                _checkCollider[obj].SetActive(true);

                if (!_checkColliderObj.ContainsKey(obj))
                {
                    GameObject checkColliderObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    checkColliderObj.name = "CheckObstacleCollider_" + obj.name;
                    Color c = UIManager.Blue;
                    c.a = 0.75f;
                    checkColliderObj.GetComponent<Renderer>().material.color = c;
                    Utility.SetMaterialTransparent(checkColliderObj.GetComponent<Renderer>().material);
                    checkColliderObj.GetComponent<Renderer>().enabled = false;
                    if (manipulationHelperLayer != -1)
                        checkColliderObj.layer = LayerMask.NameToLayer("ManipulationHelper");

                    _checkColliderObj.Add(obj, checkColliderObj);
                }
                _checkColliderObj[obj].SetActive(true);

                BoxCollider checkColliderBox = _checkCollider[obj].GetComponent<BoxCollider>();
                _checkCollider[obj].transform.localScale = _boxCollider.size;
                checkColliderBox.size = Vector3.one;
                checkColliderBox.center = Vector3.zero;
                _checkCollider[obj].transform.rotation = transform.rotation;
                _checkCollider[obj].transform.position = newWorldPosition + transform.TransformDirection(_boxCollider.center);

                BoxCollider obstacleColliderBox = _checkColliderObj[obj].GetComponent<BoxCollider>();
                _checkColliderObj[obj].transform.localScale = bounds.size;
                obstacleColliderBox.size = Vector3.one;
                obstacleColliderBox.center = Vector3.zero;
                _checkColliderObj[obj].transform.rotation = obj.transform.rotation;
                _checkColliderObj[obj].transform.position = obj.transform.position + obj.transform.TransformDirection(bounds.center);

                if (checkColliderBox.bounds.Intersects(obstacleColliderBox.bounds))
                {
                    colliders.Add(obstacleColliderBox);

                    tmpCollision = true;
                    Vector3 closestPoint = obstacleColliderBox.ClosestPoint(checkColliderBox.transform.position);// checkColliderBox.ClosestPoint(bounds.center);
                    Vector3 localClosestPoint = closestPoint - _checkCollider[obj].transform.position;  
                    
                    Vector3 offset = Vector3.zero;
                    float ratioX = Mathf.Abs(localClosestPoint.x / checkColliderBox.bounds.extents.x);
                    float ratioZ = Mathf.Abs(localClosestPoint.z / checkColliderBox.bounds.extents.z);

                    if (!_collidingXZ.ContainsKey(obj) || _collidingXZ[obj] == Vector2.zero)
                    {
                        if (!_collidingXZ.ContainsKey(obj))
                            _collidingXZ.Add(obj, Vector2.zero);

                        Vector2 collidingXZ = Vector2.zero;
                        if (ratioX - ratioZ > 0.0001f)
                            collidingXZ.x = Mathf.Sign(localClosestPoint.x);
                        else if(ratioZ - ratioX > 0.0001f)
                            collidingXZ.y = Mathf.Sign(localClosestPoint.z);

                        _collidingXZ[obj] = collidingXZ;
                    }

                    if (_collidingXZ[obj] == Vector2.zero)
                    {
                        Vector3 localObst = obstacleColliderBox.transform.InverseTransformPoint(checkColliderBox.transform.position);
                        offset.x = -(newWorldPosition.x - transform.position.x);
                        offset.z = -(newWorldPosition.z - transform.position.z);
                    }
                    else
                    {
                        Vector3 innerOffset = Vector3.zero;
                        if ((Mathf.Abs(localClosestPoint.z) < 0.0001f && _collidingXZ[obj].y != 0) || (Mathf.Abs(localClosestPoint.x) < 0.0001f && _collidingXZ[obj].x != 0))
                        {
                            innerOffset = obstacleColliderBox.transform.TransformDirection(obstacleColliderBox.transform.InverseTransformPoint(checkColliderBox.transform.position));
                        }
                        if (_collidingXZ[obj].x != 0)
                        {
                            if (ratioX > 0.0001f)
                                offset.x = (localClosestPoint.x - Mathf.Sign(localClosestPoint.x) * checkColliderBox.bounds.extents.x);
                            else
                            {
                                offset.x = -((innerOffset.x + 0.5f * Mathf.Sign(_collidingXZ[obj].x)) * obstacleColliderBox.bounds.size.x + checkColliderBox.bounds.extents.x * Mathf.Sign(_collidingXZ[obj].x));
                            }
                        }
                        else if (_collidingXZ[obj].y != 0)
                        {
                            if (ratioZ > 0.0001f)
                                offset.z = (localClosestPoint.z - Mathf.Sign(localClosestPoint.z) * checkColliderBox.bounds.extents.z);
                            else
                            {
                                offset.z = -((innerOffset.z + 0.5f * Mathf.Sign(_collidingXZ[obj].y)) * obstacleColliderBox.bounds.size.z + checkColliderBox.bounds.extents.z * Mathf.Sign(_collidingXZ[obj].y));
                            }
                        }
                    }

                    newWorldPosition.x += offset.x;
                    newWorldPosition.z += offset.z;

                }
                else
                {
                    if (_collidingXZ.ContainsKey(obj))
                        _collidingXZ[obj] = Vector2.zero;
                    if(_checkCollider.ContainsKey(obj))
                        _checkCollider[obj].SetActive(false);
                    if (_checkColliderObj.ContainsKey(obj))
                        _checkColliderObj[obj].SetActive(false);
                }
            }
            else
            {
                if (_collidingXZ.ContainsKey(obj))
                    _collidingXZ[obj] = Vector2.zero;
            }

        }

        if (!tmpCollision)
        {
            //_checkCollider.GetComponent<Renderer>().enabled = false;
        }
    }
}
