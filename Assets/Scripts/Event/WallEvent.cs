using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WallEvent : ScriptableObject
{
    private List<IWallEventListener> listeners =
        new List<IWallEventListener>();

    public void Raise(int p_wallID, bool p_protected)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnWallEventRaised(p_wallID, p_protected);
    }

    public void RaiseDimensions(float p_height, float p_thickness, float p_protectionHeight)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnWallDimensionsEventRaised(p_height, p_thickness, p_protectionHeight);
    }

    public void RegisterListener(IWallEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IWallEventListener listener)
    { listeners.Remove(listener); }
}
