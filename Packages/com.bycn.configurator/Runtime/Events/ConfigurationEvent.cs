using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConfigurationEvent : ScriptableObject
{
    private List<IConfigurationEventListener> listeners =
        new List<IConfigurationEventListener>();

    public void Raise(string p_configuration)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnConfigurationEventRaised(p_configuration);
    }

    public void RaiseRequest(GameObject p_object, string p_configurationGroupName, bool p_requested)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnConfigurationEventRequestRaised(p_object, p_configurationGroupName, p_requested);
    }

    public void RaiseInfos(ConfigurationOption p_option)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnConfigurationEventInfosRaised(p_option);
    }

    public void RaiseOptionUpdated(ConfigurationOption p_option)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnConfigurationEventOptionUpdatedRaised(p_option);
    }

    public void RegisterListener(IConfigurationEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(IConfigurationEventListener listener)
    { listeners.Remove(listener); }
}
