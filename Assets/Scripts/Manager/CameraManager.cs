using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using static CameraManager;
using System.Linq;

public class CameraManager : MonoBehaviour, IPlanEventListener
{
    private CameraManager Instance; //Singleton

    // Managers
    [SerializeField] private RoomManager _roomManager;
    [SerializeField] private FloorManager _floorManager;
    [SerializeField] private MeasurementDrawing _measurementDrawing;
    [SerializeField] private InputManager _controls;//Input System


    // Cameras
    [SerializeField] private GameObject _camera3DPrefab;
    [SerializeField] private GameObject _cameraImmersivePrefab;
    [SerializeField] private GameObject _camera2DPrefab;
    private GameObject _cameraRoot;
    private List<GameObject> _cameras = null;
    public enum CameraType { C2D, C3D, CImmersive};
    [SerializeField] CameraType _currentCameraType = CameraType.C3D;
    [SerializeField] CameraType _currentSingleCamera = CameraType.C3D;
    private CameraType _lastCurrentCameraType = CameraType.C3D;
    public Dictionary<Side, CameraTransform> _2dCameraTransforms;
    private Vector3 _camera2DCenterPosition;
    private bool isFreeCamera;

    // Encapsulate the position and rotation of the 2D camera
    public struct CameraTransform 
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public CameraTransform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }


    [SerializeField] private GameObject _splitLine;
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _panels;
    [SerializeField] private GameObject _areaLight;

    private bool _screenSplitted = false;
    private bool _planActivated = false;


    // Offsets
    [SerializeField] private float _currentCameraRectOffsetX = 0f;
    [SerializeField] private Vector2 _currentCameraNearClipPlaneOffset = new Vector2(0f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Floor = new Vector2(0f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Entry = new Vector2(0f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Bed = new Vector2(-1f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Window = new Vector2(0f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Tv = new Vector2(0f, 10f);
    [SerializeField] private Vector2 _cameraNearClipPlaneOffset_Ceiling = new Vector2(0f, 1.5f);


    // Sides
    [SerializeField] private Side _currentSide = Side.Floor;
    [Serializable]
    public enum Side
    {
        Floor,
        Entry,
        Bed,
        Window,
        Tv,
        Ceiling
    }

    [SerializeField] Dictionary<GameObject, PlanFilter.SideFilter[]> _planFilters = new Dictionary<GameObject, PlanFilter.SideFilter[]>();
    [SerializeField] PlanEvent _planEvent;

    public static string SideToText(Side side)
    {
        string result = "";
        switch(side)
        {
            case Side.Floor:
                result = "Sol";
                break;
            case Side.Entry:
                result = "Porte palière";
                break;
            case Side.Bed:
                result = "Tête de lit";
                break;
            case Side.Window:
                result = "Fenêtre";
                break;
            case Side.Tv:
                result = "Pied de lit";
                break;
            case Side.Ceiling:
                result = "Plafond";
                break;
        }
        return result;
    }

    // Scaling + Images data
    private enum ScaleInCm
    {
        _1_50,
        _1_100,
        _1_250
    };
    [SerializeField] private float _imageSizeInCm = 19f;
    [SerializeField] private ScaleInCm _scaleInCm = ScaleInCm._1_50;
    [SerializeField] private RenderTexture _renderTexture;
    
    
    [SerializeField] bool isPaused = false;

    // Properties
    public GameObject Camera3DPrefab => _camera3DPrefab;
    public GameObject Camera2DPrefab => _camera2DPrefab;
    public GameObject CameraImmersivePrefab => _cameraImmersivePrefab;
    public CameraType GetCamera3DType => CameraType.C3D;
    public CameraType GetCamera2DType => CameraType.C2D;
    public Vector3 Camera2DCenterPosition => _camera2DCenterPosition;
    public float CurrentCameraNearClipPlaneOffset => _currentCameraNearClipPlaneOffset.x;
    public float CurrentCameraFarClipPlaneOffset => _currentCameraNearClipPlaneOffset.y;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        _camera2DCenterPosition = new Vector3(0f, 20f, .5f);

        _2dCameraTransforms = new Dictionary<Side, CameraTransform>
        {
            { Side.Floor, new CameraTransform(new Vector3(0f, 2.5f, 0f), Quaternion.Euler(90, 0, 0)) },
            { Side.Ceiling, new CameraTransform(new Vector3(0f, 3f, 0f), Quaternion.Euler(90, 0, 0)) },
            { Side.Window, new CameraTransform(new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0, 180, 0)) },
            { Side.Bed, new CameraTransform(new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0, -90, 0)) },
            { Side.Entry, new CameraTransform(new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0, 0, 0)) },
            { Side.Tv, new CameraTransform(new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0, 90, 0)) }
        };
    }

    private void Start()
    {
        _cameraRoot = new GameObject("Cameras");

        InitCameras();
        SwitchCamera((int)_currentCameraType);

        isFreeCamera = true;

        _splitLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPaused)
            return;

        if (_controls.View3D())
        {
            SwitchCamera(1);
        }
        if (_controls.ViewImmersive())
        {
            SwitchCamera(2);
        }

        if (_controls.View2D())
        {
            SwitchCamera(0);
        }

        if (_controls.SplitScreen())
        {
            SplitScreen();
        }

        if (_controls.OneScreen())
        {
            OneScreen();
        }

        if (!_screenSplitted)
        {
            if (isFreeCamera)
            {

                if (EventSystem.current != null)
                {
                    //if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        // Le curseur n'est pas sur un objet UI
                        _cameras[(int)_currentCameraType].GetComponent<ICamera>().UpdateCam();
                    }
                }
            }
        }
        else
        {
            if (isFreeCamera)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                float screenWidth = Screen.width;
                float screenMidX = (screenWidth + _currentCameraRectOffsetX) / 2f;

                if (EventSystem.current != null)
                {
                    //if (!(EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject.layer != LayerMask.NameToLayer("UI")))
                    {
                        // Le curseur n'est pas sur un objet UI
                        if (mousePosition.x < screenMidX)
                        {
                            _cameras[(int)_currentSingleCamera].GetComponent<ICamera>().UpdateCam();
                        }
                        else
                        {
                            _cameras[(int)CameraType.C2D].GetComponent<ICamera>().UpdateCam();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        if (_planEvent != null)
            _planEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (_planEvent != null)
            _planEvent.UnregisterListener(this);
    }

    public void InitCameras()
    {
        if (_cameras == null)
            _cameras = new List<GameObject>();
        if (_cameras.Count != 4)
            _cameras.Clear();

        _cameras.Add(InitCamera(CameraType.C2D));
        _cameras.Add(InitCamera(CameraType.C3D));
        _cameras.Add(InitCamera(CameraType.CImmersive));
    }

    public void ActivatePlans(bool p_value)
    {
        _planActivated = p_value;

        if (_planActivated)
        {
            OneScreen();
            SwitchCamera((int)CameraType.C2D);
            SetCurrentSide(_currentSide);
            Activate3dObjectsLayer(true);
            //_cameras[(int)CameraType.C2D].GetComponent<Camera>().nearClipPlane = 0f;
            _areaLight.SetActive(false);
        }
        else
        {
            SwitchCamera((int)_currentCameraType);
            Activate3dObjectsLayer(false);
            _areaLight.SetActive(true);
            FilterObjectsBySide(_planActivated, _currentSide);
        }
        
    }

    public void SwitchCamera(int type)
    {
        _currentCameraType = (CameraType)type;

        if (!_planActivated)
        {
            if (_screenSplitted && _currentSingleCamera == CameraType.C2D)
            {
                _currentSingleCamera = CameraType.C3D;
            }
            else
            {
                _currentSingleCamera = _currentCameraType;
            }
        }

        if (_cameras[(int)CameraType.C3D] != null)
        {

            _cameras[(int)CameraType.C3D].SetActive(_currentCameraType == CameraType.C3D || (_screenSplitted && _currentSingleCamera == CameraType.C3D));
        }

        if(_cameras[(int)CameraType.C2D] != null)
        {
            _cameras[(int)CameraType.C2D].GetComponent<Camera>().backgroundColor = Color.white;
            if (_currentCameraType == CameraType.C2D || _screenSplitted)
            {
                if (!_planActivated)
                {
                    SetCurrentSide(Side.Floor);
                    //_cameras[(int)CameraType.C2D].GetComponent<Camera>().nearClipPlane = 0f;
                }
                SetCurrentSide(Side.Floor);
                _measurementDrawing.DisplayRoomMeasurements();
            }
            _cameras[(int)CameraType.C2D].SetActive(_currentCameraType == CameraType.C2D || _screenSplitted);
        }

        
        
        if (_cameras[(int)CameraType.CImmersive] != null)
            _cameras[(int)CameraType.CImmersive].SetActive(_currentCameraType == CameraType.CImmersive || (_screenSplitted && _currentSingleCamera == CameraType.CImmersive));
    
    }

    public GameObject InitCamera(CameraType p_type)
    {
        GameObject cam = null;
        switch (p_type)
        {
            case CameraType.C3D:
                cam = Instantiate(_camera3DPrefab, Vector3.zero, Quaternion.identity, _cameraRoot.transform);
                break;
            case CameraType.CImmersive:
                cam = Instantiate(_cameraImmersivePrefab, Vector3.zero, Quaternion.identity, _cameraRoot.transform);
                break;
            case CameraType.C2D:
                cam = Instantiate(_camera2DPrefab, Vector3.zero, Quaternion.identity, _cameraRoot.transform);
                break;
            default:
                break;
        }

        if (cam != null)
        {
            cam.GetComponent<ICamera>().InitData(_roomManager, _target);
            cam.GetComponent<ICamera>().Reset();
        }

        return cam;
    }

    public void SplitScreen()
    {
        _screenSplitted = true;
        _splitLine.gameObject.SetActive(true);
        SwitchCamera((int)_currentCameraType);
        UpdateViewport(_currentCameraRectOffsetX);  
    }

    public void OneScreen()
    {
        _screenSplitted = false;
        _splitLine.gameObject.SetActive(false);
        _currentCameraType = _currentSingleCamera;
        SwitchCamera((int)_currentCameraType);
        UpdateViewport(_currentCameraRectOffsetX);
    }

    public void LoadData(List<int> p_currentTypes, List<int> p_type, List<Vector3> p_transforms, List<float> p_fovs, List<float> p_dtts)
    {
        if (p_currentTypes.Count == 2)
        {
            _currentCameraType = (CameraType)p_currentTypes[0];
            _currentSingleCamera = (CameraType)p_currentTypes[1];
        }

        if (p_transforms.Count == 3)
        {
            for(int i = 0; i < p_transforms.Count; ++i)
            {
                _cameras[i].transform.localPosition = p_transforms[i * 3];
                _cameras[i].transform.localRotation = Quaternion.Euler(p_transforms[i * 3 + 1]);
                _cameras[i].transform.localScale = p_transforms[i * 3 + 2];
                _cameras[i].GetComponent<ICamera>().SetFieldOfView(p_fovs[i]);
                _cameras[i].GetComponent<ICamera>().SetDistToTarget(p_dtts[i]);
            }
        }

        SwitchCamera((int)_currentCameraType);
    }

    void Clear()
    {
        _roomManager.ReinitWallsTransparency();
    }

    public List<Vector3> GetCamerasTransformVector3()
    {
        List<Vector3> transforms = new List<Vector3>();
        for(int i = 0; i < _cameras.Count; ++i)
        {
            transforms.Add(_cameras[i].transform.localPosition);
            transforms.Add(_cameras[i].transform.localRotation.eulerAngles);
            transforms.Add(_cameras[i].transform.localScale);
        }
        return transforms;
    }

    public List<int> GetCurrentCameraType()
    {
        List<int> ctypes = new List<int>();
        ctypes.Add((int)_currentCameraType);
        ctypes.Add((int)_currentSingleCamera);
        return ctypes;
    }

    public List<int> GetCamerasType()
    {
        List<int> types = new List<int>();
        types.Add((int)CameraType.C2D);
        types.Add((int)CameraType.C3D);
        types.Add((int)CameraType.CImmersive);
        return types;
    }

    public List<float> GetFieldOfView()
    {
        List<float> fovs = new List<float>();
        for (int i = 0; i < _cameras.Count; ++i)
        {
            fovs.Add(_cameras[i].GetComponent<ICamera>().GetFieldOfView());
        }

        return fovs;
    }

    public List<float> GetDistToTarget()
    {
        List<float> dtts = new List<float>();
        for (int i = 0; i < _cameras.Count; ++i)
        {
            dtts.Add(_cameras[i].GetComponent<ICamera>().GetDistToTarget());
        }

        return dtts;
    }

    public List<GameObject> GetCamera()
    {
        return _cameras;
    }

    public Ray GetRay(Vector2 mousePosition)
    {
        if (_screenSplitted)
        {
            if (mousePosition.x < UnityEngine.Screen.width / 2)
            {
                return _cameras[(int)_currentSingleCamera].GetComponent<Camera>().ScreenPointToRay(mousePosition);
            }
            else
            {
                return _cameras[(int)CameraType.C2D].GetComponent<Camera>().ScreenPointToRay(mousePosition);
            }
        }
            
        return _cameras[(int)_currentSingleCamera].GetComponent<Camera>().ScreenPointToRay(mousePosition);
    }

    public Vector2 GetScreenPosition(Vector3 p_worldPosition)
    {
        if (_screenSplitted)
        {
            return _cameras[(int)CameraType.C2D].GetComponent<Camera>().WorldToScreenPoint(p_worldPosition);
        }

        return _cameras[(int)_currentSingleCamera].GetComponent<Camera>().WorldToScreenPoint(p_worldPosition);
    }

    public Vector3 GetCurrentForwardVector()
    {
        return _cameras[(int)_currentSingleCamera].GetComponent<ICamera>().GetPlaneForwardVector();
    }

    public Vector3 GetCurrentRightVector()
    {
        return _cameras[(int)_currentSingleCamera].GetComponent<ICamera>().GetPlaneRightVector();
    }

    public void PauseUpdate(bool pause)
    {
        isPaused = pause;
    }

    private void ChangeBackgroundColor(Camera camera, Color color)
    {
        camera.backgroundColor = color;
    }

    public void UpdateViewport(float offsetX)
    {
        _currentCameraRectOffsetX = offsetX;
        if (_screenSplitted)
        {
            SetCameraViewportRect(CameraType.C3D, offsetX / (float)Screen.width, 0, (Screen.width - offsetX) / (2f * Screen.width), 1);
            SetCameraViewportRect(CameraType.CImmersive, offsetX / (float)Screen.width, 0, (Screen.width - offsetX) / (2f * Screen.width), 1);
            SetCameraViewportRect(CameraType.C2D, (Screen.width + offsetX) / (2f * Screen.width), 0, 1, 1);
        }
        else
        {
            SetCameraViewportRect(CameraType.C3D, offsetX / (float)Screen.width, 0, 1, 1);
            SetCameraViewportRect(CameraType.CImmersive, offsetX / (float)Screen.width, 0, 1, 1);
            SetCameraViewportRect(CameraType.C2D, offsetX / (float)Screen.width, 0, 1, 1);
        }
    }

    public void SetCameraNearFarClipPLane(CameraType cameraType, Side side)
    {
        Camera camera = _cameras[(int)cameraType].GetComponent<Camera>();

        if (_cameras != null && _cameras[(int)cameraType] != null)
        {
            switch(side)
            {
                case Side.Floor:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Floor.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Floor.y;
                    break;
                case Side.Ceiling:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Ceiling.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Ceiling.y;
                    break;
                case Side.Entry:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Entry.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Entry.y;
                    break;
                case Side.Window:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Window.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Window.y;
                    break;
                case Side.Bed:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Bed.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Bed.y;
                    break;
                case Side.Tv:
                    camera.nearClipPlane = _cameraNearClipPlaneOffset_Tv.x;
                    camera.farClipPlane = _cameraNearClipPlaneOffset_Tv.y;
                    break;
                default:
                    camera.nearClipPlane = _currentCameraNearClipPlaneOffset.x;
                    camera.farClipPlane = _currentCameraNearClipPlaneOffset.y;
                    break;
            }
        }
    }

    public void SetCameraViewportRect(CameraType cameraType, float newX, float newY, float newWidth, float newHeight) 
    {
        if (_cameras != null && _cameras[(int)cameraType] != null)
        {
            Camera camera = _cameras[(int)cameraType].GetComponent<Camera>();
            camera.rect = new Rect(newX, newY, newWidth, newHeight);
        }
    }

    public Camera GetCamera3D()
    {
        if(_cameras != null && _cameras[(int)CameraType.C3D] != null)
            return _cameras[(int)CameraType.C3D].GetComponent<Camera>();
        return null;
    }

    #region 2DCAMERA


    public void SetCamera2DTransform(CameraType cameraType, Vector3 position)
    {
        _cameras[(int)cameraType].transform.position = position;
    }

    public void SetCamera2DTransform(Vector3 newPosition, Quaternion newRotation)
    {
        if (_cameras != null && _cameras.Count > (int)CameraType.C2D && _cameras[(int)CameraType.C2D] != null)
        {
            _cameras[(int)CameraType.C2D].transform.position = newPosition;
            _cameras[(int)CameraType.C2D].transform.rotation = newRotation;
        }
        else
        {
            Debug.Log("Camera2D is not initialized");
        }
    }

    public Dictionary<Side, CameraTransform> GetCamera2DTransforms()
    {
        return _2dCameraTransforms;
    }

    public CameraTransform GetCurrentCamera2DTransform()
    {
        if (_2dCameraTransforms.TryGetValue(_currentSide, out CameraTransform transform))
        {
            return transform;
        }
        else
        {
            Debug.Log($"CameraManager - GetCurrentCameraTransform() - No camera transform found for the current side : {_currentSide}");
            return default;
        }
    }

    public void SetCurrentSide(int iSide)
    {
        SetCurrentSide((Side)iSide);
    }

    public void SetCurrentSide(Side side)
    {
        if (_2dCameraTransforms.TryGetValue(side, out CameraTransform transform))
        {
            Vector3 l_position = transform.Position;
            l_position.y = side == Side.Ceiling ? _roomManager.GetWallHeight() : _roomManager.GetWallHeight() / 2f;
            SetCamera2DTransform(l_position, transform.Rotation);
            _currentSide = side;
            SetCameraNearFarClipPLane(CameraType.C2D, _currentSide);
            FilterObjectsBySide(_planActivated, _currentSide);
        }
        else
        {
            Debug.Log($"CameraManager - SetCurrentSide() - No camera transform found for side: {side}");
        }
    }

    void FilterObjectsBySide(bool p_filterOn, Side side)
    {
        foreach (GameObject p_object in _planFilters.Keys)
        {
            if (p_filterOn)
            {
                foreach (PlanFilter.SideFilter filter in _planFilters[p_object])
                    if (filter.side == CameraManager.SideToText(side))
                    {
                        p_object.SetActive(filter.show);
                        break;
                    }
            }
            else p_object.SetActive(true);
        }
    }

    public Side GetCurrentSide()
    {
        return _currentSide;
    }

    public Camera GetCamera2D()
    {
        if(_cameras != null && _cameras[(int)CameraType.C2D] != null)
            return _cameras[(int)CameraType.C2D].GetComponent<Camera>();
        return null;
    }

    private void Activate3dObjectsLayer(bool p_activate) 
    {
        Camera camera = _cameras[(int)CameraType.C2D].GetComponent<Camera>();

        int indexLayerToActivate = LayerMask.NameToLayer("CamLayer3D");
        if (p_activate)
            camera.cullingMask |= 1 << indexLayerToActivate;
        else
            camera.cullingMask &= ~(1 << indexLayerToActivate);
        indexLayerToActivate = LayerMask.NameToLayer("Ceiling");
        if (p_activate)
            camera.cullingMask |= 1 << indexLayerToActivate;
        else
            camera.cullingMask &= ~(1 << indexLayerToActivate);
    }

    private void ActivateCeilingLayer(bool p_activate)
    {
        Camera camera = _cameras[(int)CameraType.C3D].GetComponent<Camera>();

        int indexLayerToActivate = LayerMask.NameToLayer("Ceiling");
        if(p_activate)
            camera.cullingMask |= 1 << indexLayerToActivate;
        else camera.cullingMask &= ~(1 << indexLayerToActivate);
    }

    public void SetupCamerasForCoverImageCapture()
    {
        SetCurrentSide(Side.Floor);
        _measurementDrawing.DisplayRoomMeasurementsForFloorSide();

        Vector3 l_cam3dPositionIntented = new Vector3(5.44316339f, 7.89498186f, -4.74662066f);
        Vector3 l_cam3dRotationIntented = new Vector3(40.3931465f, 311.089508f, -1.12100304e-06f);

        GetCamera3D().transform.position = l_cam3dPositionIntented;
        GetCamera3D().transform.eulerAngles = l_cam3dRotationIntented;

        GetCamera2D().rect = new Rect(0f, 0f, 1f, 1f);
        GetCamera3D().rect = new Rect(0f, 0f, 1f, 1f);
    }

    public void SetupCameraForPlansCapture(out float p_currentOrthographicSize, out Rect p_currentRect)
    {
        Camera camera = GetCamera2D();

        p_currentOrthographicSize = 5f; 
        p_currentRect = new Rect();

        if (camera != null)
        {
            p_currentOrthographicSize = camera.orthographicSize;
            p_currentRect = camera.rect;

            float l_orthographicSizeInMeter = (_imageSizeInCm / ScaleEnumToFloat(_scaleInCm) / 100f);
            camera.orthographicSize = l_orthographicSizeInMeter / 2f;
            camera.rect = new Rect(0, 0, 1, 1);
            camera.targetTexture = _renderTexture;
            RenderTexture.active = _renderTexture;
        }
    }

    public void Restore2dCameraSettings(float p_currentOrthographicSize, Rect p_currentRect)
    {
        Camera camera = GetCamera2D();

        if (camera != null)
        {
            RenderTexture.active = null;
            camera.targetTexture = null;

            camera.orthographicSize = p_currentOrthographicSize;
            camera.rect = p_currentRect;
            camera.nearClipPlane = _currentCameraNearClipPlaneOffset.x;
            camera.farClipPlane = _currentCameraNearClipPlaneOffset.y;
        }
    }

    #endregion

    private float ScaleEnumToFloat(ScaleInCm p_scale)
    {
        float l_result;

        switch(p_scale)
        {
            case ScaleInCm._1_50:
                l_result = 1 / 50f;
                break;
            case ScaleInCm._1_100:
                l_result = 1 / 100f;
                break;
            case ScaleInCm._1_250:
                l_result = 1 / 250f;
                break;
            default:
                Debug.Log($"CaptureCameraViews - Unsupported scale value");
                l_result = 1 / 50f; // default scale value
                break;
        }

        return l_result;
    }

    void IPlanEventListener.OnPlanFilterUpdated(GameObject p_object, PlanFilter.SideFilter[] p_filters)
    {
        if (_planFilters.ContainsKey(p_object))
            _planFilters[p_object] = p_filters;
        else
            _planFilters.Add(p_object, p_filters);
    }
}
