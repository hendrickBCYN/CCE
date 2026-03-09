using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationInformationPanel : MonoBehaviour, IConfigurationEventListener
{
    [SerializeField] private ConfigurationEvent _configurationEvent;
    [SerializeField] TMPro.TextMeshProUGUI m_CurrentOptionName;
    [SerializeField] TMPro.TextMeshProUGUI m_CurrentOptionReference;
    [SerializeField] TMPro.TextMeshProUGUI m_CurrentOptionDimensions;
    [SerializeField] TMPro.TextMeshProUGUI m_CurrentOptionCost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _configurationEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _configurationEvent.UnregisterListener(this);
    }

    public void OnConfigurationEventInfosRaised(ConfigurationOption p_option)
    {
        if (p_option == null)
            return;
        //throw new System.NotImplementedException();
        if(m_CurrentOptionName != null)
            m_CurrentOptionName.text = p_option.Name;
        if(m_CurrentOptionReference != null)
            m_CurrentOptionReference.text = p_option.Reference;
        if(m_CurrentOptionDimensions != null)
            m_CurrentOptionDimensions.text = p_option.Dimensions;
        if (m_CurrentOptionCost != null)
            m_CurrentOptionCost.text = p_option.CostToString();
    }

    public void OnConfigurationEventRaised(string p_configuration)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfigurationEventRequestRaised(GameObject p_object, string p_configurationGroupName, bool p_requested)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfigurationEventOptionUpdatedRaised(ConfigurationOption p_option)
    {

    }
}
