using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIManagerEHPAD : UIManager, IRoomEventListener, IWallEventListener, IFloorEventListener, IPanelEventListener
{
    [SerializeField] private TMPro.TextMeshProUGUI _roomAreaText;
    [SerializeField] private TMPro.TextMeshProUGUI _roomWidthText;
    [SerializeField] private TMPro.TextMeshProUGUI _roomLengthText;
    [SerializeField] private TMPro.TextMeshProUGUI _jigHeightText;
    [SerializeField] private TMPro.TextMeshProUGUI _jigThicknessText;

    [SerializeField] private RoomEvent _roomUpdated;
    [SerializeField] private WallEvent _wallUpdated;
    [SerializeField] private FloorEvent _floorUpdated;

    [SerializeField] Button [] _WallProtectionFloorExtensionButtons;

    [SerializeField] private GameObject _panels;
    [SerializeField] private CameraManager _cameraManager;

    [SerializeField] private GameObject _cameraSplitLine;

    void IRoomEventListener.OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height)
    { 
        UpdateRoomValues(p_width, p_length, p_area, p_height);
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

    private void OnEnable()
    {
        _roomUpdated.RegisterListener(this);
        _wallUpdated.RegisterListener(this);
        _floorUpdated.RegisterListener(this);
        // PanelEvent.RegisterListener(this);
    }
    private void OnDisable()
    { 
        _roomUpdated.UnregisterListener(this);
        _wallUpdated.UnregisterListener(this);
        _floorUpdated.UnregisterListener(this);
        // PanelEvent.UnregisterListener(this);
    }

    
    void UpdateRoomValues(float p_width, float p_length, float p_area, float p_height)
    {
        _roomAreaText.text = p_area.ToString("F0") + " m²";
        _roomWidthText.text = p_width.ToString("F2") + " m";
        _roomLengthText.text = p_length.ToString("F2") + " m";
        _jigHeightText.text = p_height.ToString("F2") + " m";
    }

    void UpdateWallValues(float p_height, float p_thickness)
    {
        _jigHeightText.text = p_height.ToString("F1") + " m";
        _jigThicknessText.text = p_thickness.ToString("F2") + " m";
    }

    void IWallEventListener.OnWallEventRaised(int p_wallID, bool p_protected)
    {
        
    }

    void IWallEventListener.OnWallDimensionsEventRaised(float p_height, float p_thickness, float p_protectionHeight)
    {
        UpdateWallValues(p_height, p_thickness);
    }

    void IFloorEventListener.OnFloorEventRaised(Material p_material)
    {
        foreach(Button b in _WallProtectionFloorExtensionButtons)
            if(b != null)
                b.GetComponent<RawImage>().texture = p_material.mainTexture;
    }

    public void UpdateViewport(float p_width)
    {
        if (_panels != null)
        {
            _cameraManager.UpdateViewport(p_width);
        }
    }

    void IPanelEventListener.OnPanelEventRaised(bool p_opened, float p_width)
    {
        UpdateViewport(p_width);
        _cameraSplitLine.GetComponent<RectTransform>().localPosition = new Vector3(p_width / 2f, 0, 0);
    }
}