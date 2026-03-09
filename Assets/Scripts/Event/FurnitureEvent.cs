using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FurnitureEvent : ScriptableObject
{
    private List<IFurnitureEventListener> listeners =
        new List<IFurnitureEventListener>();

    public void Added(GameObject p_furniture)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnFurnitureAddedRaised(p_furniture);
    }

    public void Removed(GameObject p_furniture)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnFurnitureRemovedRaised(p_furniture);
    }

    public void RegisterListener(IFurnitureEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IFurnitureEventListener listener)
    { listeners.Remove(listener); }
}
