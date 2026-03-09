using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ManipulationEvent : ScriptableObject
{
    private List<IManipulationEventListener> listeners =
        new List<IManipulationEventListener>();

    public void RaiseClick(GameObject p_object, int p_manipulationPlane, bool p_axisXFixed, bool p_axisYFixed, bool p_axisZFixed)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnManipulationEventClickRaised(p_object, p_manipulationPlane, p_axisXFixed, p_axisYFixed, p_axisZFixed);
    }

    public void RaiseBeginDrag(GameObject p_object)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnManipulationEventBeginDragRaised(p_object);
    }

    public void RaiseDrag(GameObject p_object)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnManipulationEventDragRaised(p_object);
    }
    public void RaiseEndDrag(GameObject p_object)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnManipulationEventEndDragRaised(p_object);
    }

    public void RaisePointer(GameObject p_object, bool p_pointerDown)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnManipulationEventPointerRaised(p_object, p_pointerDown);
    }

    public void RegisterListener(IManipulationEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IManipulationEventListener listener)
    { listeners.Remove(listener); }
}
