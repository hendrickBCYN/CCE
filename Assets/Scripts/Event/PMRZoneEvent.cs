using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PMRZoneEvent : ScriptableObject
{
    private List<IPMRZoneEventListener> listeners =
        new List<IPMRZoneEventListener>();

    public void Raise(PMRZone p_zone)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnPMRZoneCollisionEventRaised(p_zone);
    }

    public void RaiseRulesActivated(bool p_activation)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnPMRZoneRulesActivated(p_activation);
    }

    public void RegisterListener(IPMRZoneEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IPMRZoneEventListener listener)
    { listeners.Remove(listener); }
}
