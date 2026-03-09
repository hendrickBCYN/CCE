using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicConfiguredObject : MonoBehaviour
{
    [SerializeField] string _configurationGroupName;
    [SerializeField] ConfigurationEvent _configurationEvent;
    int _temporaryIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        RequestConfigurationOption();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UnrequestConfigurationOption();
    }

    void RequestConfigurationOption()
    {
        _configurationEvent.RaiseRequest(gameObject, _configurationGroupName, true);
    }

    void UnrequestConfigurationOption()
    {
        _configurationEvent.RaiseRequest(gameObject, _configurationGroupName, false);
    }

    public void SetTemporaryIndex(int p_index)
    {
        _temporaryIndex = p_index;
    }

    public int TemporaryIndex => _temporaryIndex;

    public string ConfigurationGroupName => _configurationGroupName;
}
