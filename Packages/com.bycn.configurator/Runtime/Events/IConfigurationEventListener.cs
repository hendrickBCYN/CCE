using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IConfigurationEventListener
{
    public void OnConfigurationEventRaised(string p_configuration);

    public void OnConfigurationEventRequestRaised(GameObject p_object, string p_configurationGroupName, bool p_requested);

    public void OnConfigurationEventInfosRaised(ConfigurationOption p_option);

    public void OnConfigurationEventOptionUpdatedRaised(ConfigurationOption p_option);
}
