using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FloorEvent : ScriptableObject
{
    private List<IFloorEventListener> listeners =
        new List<IFloorEventListener>();

    public void Raise(Material p_material)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnFloorEventRaised(p_material);
    }

    public void RegisterListener(IFloorEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IFloorEventListener listener)
    { listeners.Remove(listener); }
}
