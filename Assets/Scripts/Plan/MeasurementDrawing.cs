using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This script manages the display of measurements for all sides
public class MeasurementDrawing: MonoBehaviour, IRoomEventListener
{
    [SerializeField] private GameObject _prefabLine2D;
    [SerializeField] private GameObject _prefabMeasurement2D;
    [SerializeField] private GameObject _canvas2DRoot;

    // Lines
    private GameObject _measurementRoot;
    private GameObject _lineWidth;
    private GameObject _lineLength;
    private GameObject _lineHeight;

    // Values
    private GameObject _measurementHeight;
    private GameObject _measurementWidth;
    private GameObject _measurementLength;
    private GameObject _areaUI;

    [SerializeField] private RoomEvent _roomEvent;
    [SerializeField] private RoomManager _roomManager;
    [SerializeField] private CameraManager _cameraManager;

    private float _offsetLine = 0.5f;
    private float _offsetValue = 0.75f;

    // Values
    public GameObject MeasurementWidth => _measurementWidth;
    public GameObject MeasurementLength => _measurementLength;
    public GameObject MeasurementHeight => _measurementHeight;
    public GameObject MeasurementArea => _areaUI;

    // Lines
    public GameObject LineWidth => _lineWidth;
    public GameObject LineLength => _lineLength;
    public GameObject LineHeight => _lineHeight;

    // Offsets width lines
    private float WidthLinePositionOffset_y;
    private float WidthLinePositionOffset_z;
    private float HeigthLinePositionOffset_y;
    private float LengthLinePositionOffset_y;

    private float _roomLength = 1f;
    private float _roomWidth = 1f;
    private float _roomHeight = 1f;
    private float _roomArea = 1f;


    private void Awake()
    {
        _measurementRoot = new GameObject("Measurements");

        // Lines
        _lineWidth = Object.Instantiate(_prefabLine2D, _measurementRoot.transform);
        _lineWidth.name = "RoomWidth";

        _lineLength = Object.Instantiate(_prefabLine2D, _measurementRoot.transform);
        _lineLength.name = "RoomLength";

        _lineHeight = Object.Instantiate(_prefabLine2D, _measurementRoot.transform);
        _lineHeight.name = "RoomHeight";

        // Values
        _measurementLength = Object.Instantiate(_prefabMeasurement2D, _canvas2DRoot.transform);
        _measurementLength.name = "RoomLengthValue";

        _measurementWidth = Object.Instantiate(_prefabMeasurement2D, _canvas2DRoot.transform);
        _measurementWidth.name = "RoomWidthValue";

        _areaUI = Object.Instantiate(_prefabMeasurement2D, _canvas2DRoot.transform);
        _areaUI.name = "RoomAreaValue";

        _measurementHeight = Object.Instantiate(_prefabMeasurement2D, _canvas2DRoot.transform);
        _measurementHeight.name = "RoomHeightValue";
    }

    void Start() 
    {
        if (_roomManager == null)
        {
            Debug.Log("MeasurementDrawing - _roomManager is not assigned");
            return;
        }

        WidthLinePositionOffset_y = -_roomManager.GetWallHeight() / 2f + 1f;
        WidthLinePositionOffset_z = _roomManager.GetLength() / 2f + 0.7f;
        HeigthLinePositionOffset_y = _roomManager.GetWallHeight() / 2f;
        LengthLinePositionOffset_y = -_roomManager.GetWallHeight() / 4f + 0.2f;

        if (_cameraManager == null)
        {
            Debug.Log("MeasurementDrawing - _cameraManager is not assigned");
            return;
        }  
    }

    private void OnEnable()
    {
        _roomEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _roomEvent.UnregisterListener(this);
    }

    void UpdateMeasurements()
    {
        Debug.Log($"MeasurementDrawing - UPDATEMEASUREMENTS()");

        // Lines 
        _lineWidth.transform.position = new Vector3(0, 0, _roomLength / 2.0f + 0.5f);
        _lineWidth.transform.localScale = new Vector3(_roomWidth, 0.01f, 0.05f);

        _lineLength.transform.position = new Vector3(_roomWidth / 2.0f + 0.5f, 0, 0);
        _lineLength.transform.localScale = new Vector3(0.05f, 0.01f, _roomLength);

        _lineHeight.transform.position = new Vector3(_roomWidth / 2.0f + 0.5f, _roomHeight / 2.0f, _roomLength / 2.0f + 0.5f); 
        _lineHeight.transform.localScale = new Vector3(0.05f, _roomHeight, 0.01f);

        // Values
        _measurementLength.transform.position = new Vector3(_roomWidth / 2.0f + 1.25f, 0, 0);
        _measurementLength.GetComponent<TMP_Text>().text = _roomLength.ToString("F2") + " m";

        _measurementWidth.transform.position = new Vector3(0, -0.5f, _roomLength / 2.0f + 0.85f);
        _measurementWidth.GetComponent<TMP_Text>().text = _roomWidth.ToString("F2") + " m";

        _measurementHeight.transform.position = new Vector3(_roomWidth / 2.0f + 1.25f, _roomHeight / 2.0f, _roomLength / 2.0f + 0.5f);
        _measurementHeight.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _measurementHeight.GetComponent<TMP_Text>().text = _roomHeight.ToString("F2") + " m";

        _areaUI.transform.position = new Vector3(0f, 0f, -1f);
        _areaUI.GetComponent<TMP_Text>().text = _roomArea + " m²";
    }


    // --- DISPLAY MEASUREMENTS ---
    public void DisplayRoomMeasurements()
    {
        DisplayRoomMeasurementsForWindowSide();
        DisplayRoomMeasurementsForEntrySide();
        DisplayRoomMeasurementsForBedSide();
        DisplayRoomMeasurementsForTvSide();
        DisplayRoomMeasurementsForCeilingSide();
        DisplayRoomMeasurementsForFloorSide();
    }

    // WINDOW side
    public void DisplayRoomMeasurementsForWindowSide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Window)
        {
            UpdateMeasurementsForWindowSide();
            
            DeactivateMeasurements(_measurementLength);
            DeactivateMeasurements(_lineLength);
            DeactivateMeasurements(_areaUI);

            ActivateMeasurements(_measurementHeight);
            ActivateMeasurements(_lineHeight);
            ActivateMeasurements(_measurementWidth);
            ActivateMeasurements(_lineWidth);
        }
    }

    // ENTRY side
    public void DisplayRoomMeasurementsForEntrySide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Entry)
        {
            UpdateMeasurementsForEntrySide();

            DeactivateMeasurements(_measurementLength);
            DeactivateMeasurements(_lineLength);
            DeactivateMeasurements(_areaUI);

            ActivateMeasurements(_measurementHeight);
            ActivateMeasurements(_lineHeight);
            ActivateMeasurements(_measurementWidth);
            ActivateMeasurements(_lineWidth);
        }
    }

    // BED side
    public void DisplayRoomMeasurementsForBedSide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Bed)
        {
            UpdateMeasurementsForBedSide();

            DeactivateMeasurements(_measurementWidth);
            DeactivateMeasurements(_lineWidth);
            DeactivateMeasurements(_areaUI);

            ActivateMeasurements(_measurementHeight);
            ActivateMeasurements(_lineHeight);
            ActivateMeasurements(_measurementLength);
            ActivateMeasurements(_lineLength);
        }
    }

    // TV side
    public void DisplayRoomMeasurementsForTvSide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Tv)
        {
            UpdateMeasurementsForTVSide();

            DeactivateMeasurements(_measurementWidth);
            DeactivateMeasurements(_lineWidth);
            DeactivateMeasurements(_areaUI);

            ActivateMeasurements(_measurementHeight);
            ActivateMeasurements(_lineHeight);
            ActivateMeasurements(_measurementLength);
            ActivateMeasurements(_lineLength);
        }
    }

    // CEILING side
    public void DisplayRoomMeasurementsForCeilingSide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Ceiling)
        {
            UpdateMeasurementsForCeilingSide();

            DeactivateMeasurements(_measurementHeight);
            DeactivateMeasurements(_lineHeight);
            DeactivateMeasurements(_areaUI);

            ActivateMeasurements(_measurementLength);
            ActivateMeasurements(_lineLength);
            ActivateMeasurements(_measurementWidth);
            ActivateMeasurements(_lineWidth);
        }
    }

    // FLOOR side
    public void DisplayRoomMeasurementsForFloorSide()
    {
        if (_cameraManager.GetCurrentSide() == CameraManager.Side.Floor)
        {
            UpdateMeasurementsForFloorSide();

            DeactivateMeasurements(_measurementHeight);
            DeactivateMeasurements(_lineHeight);

            ActivateMeasurements(_measurementLength);
            ActivateMeasurements(_lineLength);
            ActivateMeasurements(_measurementWidth);
            ActivateMeasurements(_lineWidth);
            ActivateMeasurements(_areaUI);
        }
    }


    // --- UPDATE measurements position

     // WINDOW side
    private void UpdateMeasurementsForWindowSide()
    {
        UpdateMeasurementsPosition(_lineWidth.transform, new Vector3(0, -_offsetLine, -_roomLength / 2f), new Vector3(90f, 0, 0));
        UpdateMeasurementsPosition(_measurementWidth.transform, new Vector3(0, -_offsetValue, -_roomLength / 2f), new Vector3(180f, 0, 180f));
        UpdateMeasurementsPosition(_lineHeight.transform, new Vector3(_roomWidth / 2f + _offsetLine, _roomHeight/ 2f, -_roomLength / 2f), Vector3.zero);
        UpdateMeasurementsPosition(_measurementHeight.transform, new Vector3(_roomWidth / 2f + _offsetValue, _roomHeight / 2f, -_roomLength / 2f), new Vector3(0, 180f, 90f));
    }

    // ENTRY side
    private void UpdateMeasurementsForEntrySide()
    {
        UpdateMeasurementsPosition(_lineWidth.transform, new Vector3(0, -_offsetLine, _roomLength / 2f), new Vector3(90f, 0, 0));
        UpdateMeasurementsPosition(_measurementWidth.transform, new Vector3(0, -_offsetValue, _roomLength / 2f), new Vector3(0, 0, 0));
        UpdateMeasurementsPosition(_lineHeight.transform, new Vector3(_roomWidth / 2f + _offsetLine, _roomHeight / 2f, _roomLength / 2f), Vector3.zero);
        UpdateMeasurementsPosition(_measurementHeight.transform, new Vector3(_roomWidth / 2f + _offsetValue, _roomHeight / 2f, _roomLength / 2f), new Vector3(0, 0, -90f));
    }

    // BED side
    private void UpdateMeasurementsForBedSide()
    {
        UpdateMeasurementsPosition(_lineHeight.transform, new Vector3(-_roomWidth / 2f, _roomHeight / 2f, -_roomLength / 2f - _offsetLine), new Vector3(0, 90f, 0));
        UpdateMeasurementsPosition(_measurementHeight.transform, new Vector3(-_roomWidth / 2f, _roomHeight / 2f, -_roomLength / 2f - _offsetValue), new Vector3(0, -90f, 90f));
        UpdateMeasurementsPosition(_lineLength.transform, new Vector3(-_roomWidth / 2f, -_offsetLine, 0), new Vector3(0, 0, 90f));
        UpdateMeasurementsPosition(_measurementLength.transform, new Vector3(-_roomWidth / 2f, -_offsetValue, 0), new Vector3(0, -90f, 0));
    }


    // TV side
    private void UpdateMeasurementsForTVSide()
    {
        UpdateMeasurementsPosition(_lineHeight.transform, new Vector3(_roomWidth / 2f, _roomHeight / 2f, -_roomLength / 2f - _offsetLine), new Vector3(0, 90f, 0));
        UpdateMeasurementsPosition(_measurementHeight.transform, new Vector3(_roomWidth / 2f, _roomHeight / 2f, -_roomLength / 2f - _offsetValue), new Vector3(0, 90f, -90f));
        UpdateMeasurementsPosition(_lineLength.transform, new Vector3(_roomWidth / 2f, -_offsetLine, 0), new Vector3(0, 0, 90f));
        UpdateMeasurementsPosition(_measurementLength.transform, new Vector3(_roomWidth / 2f, -_offsetValue, 0), new Vector3(0, 90f, 0));
    }

    // CEILING side
    private void UpdateMeasurementsForCeilingSide()
    {
        UpdateMeasurementsPosition(_lineWidth.transform, new Vector3(0, _roomHeight - 1f, -_roomLength / 2f - _offsetLine), new Vector3(0, 0, 0));
        UpdateMeasurementsPosition(_measurementWidth.transform, new Vector3(0, _roomHeight - 1f, -_roomLength / 2f - _offsetValue), new Vector3(90f, 0f, 0));
        UpdateMeasurementsPosition(_lineLength.transform, new Vector3(_roomWidth / 2f + _offsetLine, _roomHeight - 1f, 0), new Vector3(0, 0, 0));
        UpdateMeasurementsPosition(_measurementLength.transform, new Vector3(_roomWidth / 2f + _offsetValue, _roomHeight - 1f, 0), new Vector3(90f, 90f, 0));
    }
 

    // FLOOR side
    private void UpdateMeasurementsForFloorSide()
    {
        UpdateMeasurementsPosition(_lineWidth.transform, new Vector3(0, 0, -_roomLength / 2f - _offsetLine), new Vector3(0, 0, 0));
        UpdateMeasurementsPosition(_measurementWidth.transform, new Vector3(0, 0, -_roomLength / 2f - _offsetValue), new Vector3(90f, 0, 0));
        UpdateMeasurementsPosition(_lineLength.transform, new Vector3(_roomWidth / 2f + _offsetLine, 0, 0), new Vector3(0, 0, 0));
        UpdateMeasurementsPosition(_measurementLength.transform, new Vector3(_roomWidth / 2f + _offsetValue, 0, 0), new Vector3(90f, 90f, 0));
        UpdateMeasurementsPosition(_areaUI.transform, new Vector3(1, 0, -1), new Vector3(90f, 0, 0));
    }

    // Utils
    private void UpdateMeasurementsPosition(Transform transform, Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void DeactivateMeasurements(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void ActivateMeasurements(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

     void IRoomEventListener.OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height)
    {
        _roomHeight = p_height;
        _roomWidth = p_width;
        _roomLength = p_length;
        _roomArea = p_area;
        UpdateMeasurements();
        Debug.Log($"MeasurementDrawing - OnRoomEventRaised - heigth: {p_height}");
    }

    void IRoomEventListener.OnRoomEventBathroomUpdatedRaised(GameObject p_bathroom)
    {
        //throw new System.NotImplementedException();
    }

    void IRoomEventListener.OnRoomEventWindowUpdatedRaised() { }

    void IRoomEventListener.OnRoomEventObstacleAddedRaised(GameObject p_object)
    {
        //throw new System.NotImplementedException();
    }

    void IRoomEventListener.OnRoomEventObstacleRemovedRaised(GameObject p_object)
    {
        //throw new System.NotImplementedException();
    }
}
