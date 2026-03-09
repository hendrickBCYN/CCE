using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRoomEventListener
{
    public void OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height);
    public void OnRoomEventBathroomUpdatedRaised(GameObject p_bathroom);
    public void OnRoomEventWindowUpdatedRaised();
    public void OnRoomEventObstacleAddedRaised(GameObject p_object);
    public void OnRoomEventObstacleRemovedRaised(GameObject p_object);
}
