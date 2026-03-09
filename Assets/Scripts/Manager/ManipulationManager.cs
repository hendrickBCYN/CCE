using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ManipulationManager : MonoBehaviour, IManipulationEventListener, IRoomEventListener, IWallEventListener
{
    [SerializeField] private GameObject _currentActiveObject = null;
    [SerializeField] CameraManager _cameraManager;
    [SerializeField] ManipulationEvent _manipulationEvent;
    [SerializeField] RoomEvent _roomEvent;
    [SerializeField] WallEvent _wallEvent;
    [SerializeField] private GameObject _manipulationPanel;
    [SerializeField] private Button _manipulationMoveButton;

    [SerializeField] bool _manipulationActive = false;
    bool _manipulationAllowed = true;
    public enum ManipulationPlane { XZ, XY, YZ }
    [SerializeField] ManipulationPlane _manipulationPlane;
    [SerializeField] bool _manipulationAxisXFixed;
    [SerializeField] bool _manipulationAxisYFixed;
    [SerializeField] bool _manipulationAxisZFixed;

    public enum ManipulationRotation { One, Half, Quarter, Eight, Sixteenth }
    [SerializeField] ManipulationRotation _manipulationRotation;

    [SerializeField] GameObject _room;
    [SerializeField] Vector3 _roomDimensions = Vector3.zero;

    Vector2 _pointerStart = Vector2.zero;
    Vector3 _pointerWorldStart = Vector3.zero;
    Vector3 _currentActiveObjectInitialPosition = Vector3.zero;
    Vector2 _offset2D = Vector2.zero;

    private List<ManipulatedPart> _manipulatedParts = new List<ManipulatedPart>();

    [SerializeField] private List<GameObject> _obstacles = new List<GameObject>();

    bool _isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        _manipulationPanel.SetActive(false);
        _manipulatedParts = new List<ManipulatedPart>(_room.GetComponentsInChildren<ManipulatedPart>());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if(_isDragging)
        {
            if (_currentActiveObject != null && _manipulationActive)
            {
                Vector2 currentPointer = Pointer.current.position.ReadValue();
                Ray ray = _cameraManager.GetRay(currentPointer);
                float rayLength = 1f;
                bool inters = false;
                if (_manipulationPlane == ManipulationPlane.XZ)
                    inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.up, out rayLength);
                else if (_manipulationPlane == ManipulationPlane.XY)
                    inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.forward, out rayLength);
                else if (_manipulationPlane == ManipulationPlane.YZ)
                    inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.right, out rayLength);
                if (inters)
                {
                    Vector3 currentPointerWorld = ray.origin + ray.direction * rayLength;
                    Vector3 diffPos = (currentPointerWorld - _pointerWorldStart);
                    if (_manipulationAxisXFixed)
                        diffPos.x = 0;
                    if (_manipulationAxisYFixed)
                        diffPos.y = 0;
                    if (_manipulationAxisZFixed)
                        diffPos.z = 0;
                    Vector3 newPos = _currentActiveObjectInitialPosition + diffPos;
                    if (_currentActiveObject.GetComponent<ManipulatedPart>() != null)
                    {
                        _currentActiveObject.GetComponent<ManipulatedPart>().ValidatePosition(ref newPos, _roomDimensions, _obstacles);
                    }
                    _currentActiveObject.transform.position = newPos;
                    if (_currentActiveObject.GetComponent<Anchor>() != null)
                    {
                        _currentActiveObject.GetComponent<Anchor>().UpdatePositionOffset();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        _manipulationEvent.RegisterListener(this);
        _roomEvent.RegisterListener(this);
        _wallEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _manipulationEvent.UnregisterListener(this);
        _roomEvent.UnregisterListener(this);
        _wallEvent.UnregisterListener(this);
    }

    public void SetCurrentActiveObject(GameObject p_object)
    {
        if (p_object != null)
        {
            if (p_object.GetComponent<Outline>() != null)
                p_object.GetComponent<Outline>().SetSelected(true);
        }

        if (_currentActiveObject != null && _currentActiveObject != p_object)
        {
            if (_currentActiveObject.GetComponent<Outline>() != null)
                _currentActiveObject.GetComponent<Outline>().SetSelected(false);
        }
        _currentActiveObject = p_object;
    }

    public void Reset()
    {
        if (_currentActiveObject != null)
        {
            if (_currentActiveObject.GetComponent<ManipulatedPart>() != null)
                _currentActiveObject.GetComponent<ManipulatedPart>().Reset();
            if (_currentActiveObject.GetComponent<Outline>() != null)
                _currentActiveObject.GetComponent<Outline>().SetSelected(false);
        }
        _currentActiveObject = null;
        if (_manipulationPanel != null)
        {
            _manipulationPanel.SetActive(false);
        }
    }

    public void UpdateObjectTransform(Vector3 rotation, Vector3 up, Vector3 left)
    {
        Debug.Log($"UpdateObjectTransform {rotation} - {up} - {left}");
        _currentActiveObject.transform.localPosition += up + left;
        _currentActiveObject.transform.Rotate(rotation);
    }

    public void EnableManipulation(bool p_enable)
    {
        if (!p_enable)
            StopManipulation();
        _manipulationActive = p_enable;
    }

    void IManipulationEventListener.OnManipulationEventClickRaised(GameObject p_object, int p_manipulationPlane, bool p_axisXFixed, bool p_axisYFixed, bool p_axisZFixed)
    {
        //Debug.Log("OnManipulationEventRaised " + p_object);
        {
            SetCurrentActiveObject(p_object);
            _manipulationPlane = (ManipulationPlane)p_manipulationPlane;
            _manipulationAxisXFixed = p_axisXFixed;
            _manipulationAxisYFixed = p_axisYFixed;
            _manipulationAxisZFixed = p_axisZFixed;
            if (_manipulationPanel != null)
            {
                _manipulationPanel.SetActive(true);
            }
        }
    }

    void IManipulationEventListener.OnManipulationEventBeginDragRaised(GameObject p_object)
    {
        foreach (ManipulatedPart part in _manipulatedParts)
            if (part.gameObject.GetComponent<Outline>() != null)
                part.gameObject.GetComponent<Outline>().enabled = part.gameObject == p_object;
    }

    void IManipulationEventListener.OnManipulationEventDragRaised(GameObject p_object)
    {
        if (p_object == _currentActiveObject)
            Drag();
    }

    void IManipulationEventListener.OnManipulationEventEndDragRaised(GameObject p_object)
    {
        foreach (ManipulatedPart part in _manipulatedParts)
            if (part.gameObject.GetComponent<Outline>() != null)
                part.gameObject.GetComponent<Outline>().enabled = true;
        _isDragging = false;
    }

    void StartManipulation()
    {
        if (_currentActiveObject == null || !_manipulationActive)
            return;

        Debug.Log("StartManipulation");
        _pointerStart = Pointer.current.position.ReadValue();
        Ray ray = _cameraManager.GetRay(_pointerStart);
        float rayLength = 1f;
        bool inters = false;
        if (_manipulationPlane == ManipulationPlane.XZ)
            inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.up, out rayLength);
        else if (_manipulationPlane == ManipulationPlane.XY)
            inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.forward, out rayLength);
        else if (_manipulationPlane == ManipulationPlane.YZ)
            inters = Utility.RayPlaneIntersection(ray, _currentActiveObject.transform.position, _currentActiveObject.transform.right, out rayLength);
        if (inters)
        {
            _pointerWorldStart = ray.origin + ray.direction * rayLength;
            _currentActiveObjectInitialPosition = _currentActiveObject.transform.position;
            _offset2D = new Vector2(_manipulationPanel.transform.position.x, _manipulationPanel.transform.position.y) - _pointerStart;
        }
        _cameraManager.PauseUpdate(true);
    }

    void IManipulationEventListener.OnManipulationEventPointerRaised(GameObject p_object, bool p_pointerDown)
    {
        if (p_object == _currentActiveObject)
        {
            if (p_pointerDown)
                StartManipulation();
            else StopManipulation();
        }
    }

    void StopManipulation()
    {
        if (_manipulationActive)
        {
            _cameraManager.PauseUpdate(false);
        }
    }

    public void Drag()
    {
        _isDragging = true;
    }

    public void RotateObject(bool clockwise)
    {
        if (_currentActiveObject != null)
        {
            float l_y = _currentActiveObject.GetComponent<ManipulatedPart>() != null ? _currentActiveObject.GetComponent<ManipulatedPart>().GetAngleFromManipulationRotation() : 90f;
            if (clockwise) l_y *= -1;
            _currentActiveObject.transform.rotation *= Quaternion.Euler(0, l_y, 0);
            if (_currentActiveObject.GetComponent<ManipulatedPart>() != null)
            {
                Vector3 l_newPos = _currentActiveObject.transform.position;
                _currentActiveObject.GetComponent<ManipulatedPart>().ValidatePosition(ref l_newPos, _roomDimensions, _obstacles);
                _currentActiveObject.transform.position = l_newPos;
            }
            if (_currentActiveObject.GetComponent<Anchor>() != null)
            {
                _currentActiveObject.GetComponent<Anchor>().UpdatePositionOffset();
            }
        }
    }

    void IRoomEventListener.OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height)
    {
        _roomDimensions.x = p_width;
        _roomDimensions.z = p_length;

        UpdateManipulatedParts();
    }

    public void UpdateManipulatedParts()
    {
        foreach (ManipulatedPart l_part in _manipulatedParts)
        {
            l_part.UpdateLimitPoints();
            Vector3 l_newPos = l_part.transform.position;
            l_part.ValidatePosition(ref l_newPos, _roomDimensions, _obstacles);
            l_part.transform.position = l_newPos;

            /*if (l_part.gameObject.GetComponent<Anchor>() != null)
            {
                l_part.gameObject.GetComponent<Anchor>().UpdatePositionOffset();
            }*/
        }
    }

    void IRoomEventListener.OnRoomEventBathroomUpdatedRaised(GameObject p_bathroom)
    {
        //throw new System.NotImplementedException();
    }

    void IRoomEventListener.OnRoomEventWindowUpdatedRaised()
    {

    }

    void IRoomEventListener.OnRoomEventObstacleAddedRaised(GameObject p_object)
    {
        if(!_obstacles.Contains(p_object))
            _obstacles.Add(p_object);
    }

    void IRoomEventListener.OnRoomEventObstacleRemovedRaised(GameObject p_object)
    {
        if(_obstacles.Contains(p_object))
            _obstacles.Remove(p_object);
    }

    void IWallEventListener.OnWallEventRaised(int p_wallID, bool p_protected)
    {
        //throw new System.NotImplementedException();
    }

    void IWallEventListener.OnWallDimensionsEventRaised(float p_height, float p_thickness, float p_protectionHeight)
    {
        _roomDimensions.y = p_height;
    }

    public void AllowManipulation(bool p_allow)
    {
        _manipulationAllowed = p_allow;
        foreach(ManipulatedPart l_part in _manipulatedParts)
        {
            l_part.enabled = _manipulationAllowed;
            if(l_part.gameObject.GetComponent<Outline>()  != null)
                l_part.gameObject.GetComponent<Outline>().enabled = _manipulationAllowed;
        }
    }

    public GameObject CurrentActiveObject => _currentActiveObject;
    public bool IsManipulationActive => _manipulationActive;
}
