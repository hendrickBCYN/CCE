using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanEvent : ScriptableObject
{
    private List<IPlanEventListener> listeners =
        new List<IPlanEventListener>();

    public void Updated(GameObject p_object, PlanFilter.SideFilter [] p_filters)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnPlanFilterUpdated(p_object, p_filters);
    }

    public void RegisterListener(IPlanEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IPlanEventListener listener)
    { listeners.Remove(listener); }
}
