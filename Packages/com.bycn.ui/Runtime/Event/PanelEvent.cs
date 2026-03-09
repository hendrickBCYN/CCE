using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PanelEvent : ScriptableObject
{
    private List<IPanelEventListener> listeners =
        new List<IPanelEventListener>();

    public void Raise(bool p_opened, float p_width)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnPanelEventRaised(p_opened, p_width);
    }

    public void RegisterListener(IPanelEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IPanelEventListener listener)
    { listeners.Remove(listener); }
}
