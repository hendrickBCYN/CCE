using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class RoomEvent : ScriptableObject
{
    private List<IRoomEventListener> listeners =
        new List<IRoomEventListener>();

    public void Raise(float p_width, float p_length, float p_area, float p_height)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnRoomEventRaised(p_width, p_length, p_area, p_height);
    }

    public void RaiseBathroomUpdated(GameObject p_bathroom)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnRoomEventBathroomUpdatedRaised(p_bathroom);
    }

    public void RaiseWindowUpdated()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnRoomEventWindowUpdatedRaised();
    }

    public void RaiseObstacleAdded(GameObject p_object)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnRoomEventObstacleAddedRaised(p_object);
    }

    public void RaiseObstacleRemoved(GameObject p_object)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnRoomEventObstacleRemovedRaised(p_object);
    }

    public void RegisterListener(IRoomEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IRoomEventListener listener)
    { listeners.Remove(listener); }
}
