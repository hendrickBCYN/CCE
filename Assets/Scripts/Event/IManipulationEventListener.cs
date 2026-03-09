using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IManipulationEventListener
{
    public void OnManipulationEventClickRaised(GameObject p_object, int p_manipulationPlane, bool p_axisXFixed, bool p_axisYFixed, bool p_axisZFixed);
    public void OnManipulationEventPointerRaised(GameObject p_object, bool p_pointerDown);
    public void OnManipulationEventBeginDragRaised(GameObject p_object);
    public void OnManipulationEventDragRaised(GameObject p_object);
    public void OnManipulationEventEndDragRaised(GameObject p_object);
}
