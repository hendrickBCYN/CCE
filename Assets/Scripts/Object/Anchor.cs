using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Anchor : MonoBehaviour, IRoomEventListener
{

    public enum AnchorType { Center, TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight }
    public enum AnchorRef { Room, Ceiling }

    [SerializeField] private AnchorType _anchorType;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _currentOffset;
    [SerializeField] private RoomEvent _roomEvent;

    private Vector3 _roomDimensions = Vector3.zero;

    void IRoomEventListener.OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height)
    {
        _roomDimensions = new Vector3(p_width, p_length, p_area);
        Vector3 l_newPosition = transform.localPosition;
        switch (_anchorType)
        {
            case AnchorType.Center:
                l_newPosition = new Vector3(0, l_newPosition.y, 0);
                break;
            case AnchorType.TopLeft:
                l_newPosition = new Vector3(-p_width / 2f, l_newPosition.y, p_length / 2f);
                break;
            case AnchorType.TopCenter:
                l_newPosition = new Vector3(0f, l_newPosition.y, p_length / 2f);
                break;
            case AnchorType.TopRight:
                l_newPosition = new Vector3(p_width / 2f, l_newPosition.y, p_length / 2f);
                break;
            case AnchorType.BottomLeft:
                l_newPosition = new Vector3(-p_width / 2f, l_newPosition.y, -p_length / 2f);
                break;
            case AnchorType.BottomCenter:
                l_newPosition = new Vector3(0f, l_newPosition.y, -p_length / 2f);
                break;
            case AnchorType.BottomRight:
                l_newPosition = new Vector3(p_width / 2f, l_newPosition.y, -p_length / 2f);
                break;
        }

        l_newPosition += _offset + _currentOffset;
        transform.localPosition = l_newPosition;
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
        _roomEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _roomEvent.UnregisterListener(this);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePositionOffset()
    {
        Vector3 l_newPosition = transform.localPosition;
        Vector3 l_theoreticalPosition = Vector3.zero;
        switch (_anchorType)
        {
            case AnchorType.Center:
                l_theoreticalPosition = new Vector3(0, l_newPosition.y, 0);
                break;
            case AnchorType.TopLeft:
                l_theoreticalPosition = new Vector3(-_roomDimensions.x / 2f, l_newPosition.y, _roomDimensions.y / 2f);
                break;
            case AnchorType.TopCenter:
                l_theoreticalPosition = new Vector3(0f, l_newPosition.y, _roomDimensions.y / 2f);
                break;
            case AnchorType.TopRight:
                l_theoreticalPosition = new Vector3(_roomDimensions.x / 2f, l_newPosition.y, _roomDimensions.y / 2f);
                break;
            case AnchorType.BottomLeft:
                l_theoreticalPosition = new Vector3(-_roomDimensions.x / 2f, l_newPosition.y, -_roomDimensions.y / 2f);
                break;
            case AnchorType.BottomCenter:
                l_theoreticalPosition = new Vector3(0f, l_newPosition.y, -_roomDimensions.y / 2f);
                break;
            case AnchorType.BottomRight:
                l_theoreticalPosition = new Vector3(_roomDimensions.x / 2f, l_newPosition.y, -_roomDimensions.y / 2f);
                break;
        }

        l_theoreticalPosition += _offset;
        _currentOffset = l_newPosition - l_theoreticalPosition;
    }
}
