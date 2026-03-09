using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[ExecuteInEditMode]
public class ConfigurationManager : MonoBehaviour, IConfigurationEventListener
{
    [SerializeField] string _configurationName = "configuration";
    [SerializeField] ConfigurationGroup[] _configurationGroups;
    public ConfigurationGroup[] ConfigurationGroups => _configurationGroups;
    [SerializeField] ConfigurationEvent _configurationEvent;
    [SerializeField] string _currentConfigurationName;
    [SerializeField] TMPro.TextMeshProUGUI _currentConfigurationTotalCost;
    [SerializeField] float _costCoeff = 1.3f;

    [SerializeField] float _totalCost = 0f;
    float _baseCost = 0f;

    public float TotalCost => _totalCost;

    public string ConfigurationName => _configurationName;

    private ConfigurationSerialization _configurationSerialization;

    [Serializable]
    public struct CostDetail
    {
        public string _name;
        public float _cost;
    }
    [SerializeField] List<CostDetail> _costDetails = new List<CostDetail>();

    private void Awake()
    {
        _configurationSerialization = new ConfigurationSerialization();
        if (_configurationGroups == null || _configurationGroups.Length == 0)
            _configurationGroups = GetComponentsInChildren<ConfigurationGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Application.isPlaying)
            InitializeConfigurations();
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

    public void InitializeConfigurations()
    {
        {
            foreach (var group in _configurationGroups)
            {
                group.SetCostCoeff(_costCoeff);
                group.SetOption(0);
            }
        }
        ComputeTotalCost();
    }

    public void ClearConfigurations()
    {
        foreach(var group in _configurationGroups)
        {
            group.ClearOptions();
        }
    }

    public void SetConfigurationOption(string p_option)
    {
        string[] l_splits = p_option.Split(';');
        if (l_splits.Length == 2)
        {
            string l_groupName = l_splits[0];
            int l_optionID = int.Parse(l_splits[1]);

            foreach (var group in _configurationGroups) 
            {
                if (group.Name == l_groupName && l_optionID >= 0 && l_optionID < group.NbOptions)
                {
                    group.SetOption(l_optionID);
                    UpdateConfigurationName();
                    _configurationEvent.Raise(_currentConfigurationName);
                    _configurationEvent.RaiseInfos(group.CurrentOption);
                    return;
                }
            }
            Debug.LogWarning($"Error: wrong choice of option/group ({l_groupName + " - " + l_optionID}). Group does not exist or option is out of bounds");
        }
        else Debug.LogWarning($"Error: wrong choice of option ({p_option}). Option needs to be GroupName:OptionId");
    }

    public void SetConfigurationGroupContribution(string p_contribution)
    {
        string[] l_splits = p_contribution.Split(';');
        if (l_splits.Length == 2)
        {
            string l_groupName = l_splits[0];
            bool l_contribution = bool.Parse(l_splits[1]);

            foreach (var group in _configurationGroups)
            {
                if (group.Name == l_groupName)
                {
                    group.SetContributeToTotalCost(l_contribution);
                    return;
                }
            }
            Debug.LogWarning($"Error: wrong choice of group ({l_groupName}). Group does not exist.");
        }
        else Debug.LogWarning($"Error: wrong choice of p_contribution ({p_contribution}). Contribution needs to be GroupName:ContributionBool");
    }

    public void UpdateConfigurationName()
    {
        string l_result = "";
        for (int iGroup = 0; iGroup < 4; ++iGroup)
        {
            l_result += _configurationGroups[iGroup].CurrentOptionIndex.ToString();
            if (iGroup < 4 - 1)
                l_result += "_";
        }
        _currentConfigurationName = l_result;
    }

    public void ComputeTotalCost()
    {
        float l_result = 0;
        _costDetails.Clear();
        foreach (var group in _configurationGroups)
        {
            if (group.CurrentOption != null && group.ContributeToTotalCost)
            {
                l_result += group.CurrentOption.TotalCost;
                CostDetail costDetail = new CostDetail();
                costDetail._name = group.Name;
                costDetail._cost = group.CurrentOption.TotalCost;
                _costDetails.Add(costDetail);
            }
        }

        _totalCost = l_result - _baseCost;

        var nfi = new NumberFormatInfo();
        nfi.NumberGroupSeparator = " "; // set the group separator to a space
        nfi.NumberDecimalSeparator = ","; // set decimal separator to comma
        if (_currentConfigurationTotalCost != null)
            _currentConfigurationTotalCost.text = _totalCost.ToString("N0", nfi);
    }

    public string CurrentConfigurationName()
    {
        return _currentConfigurationName;
    }

    public void OnConfigurationEventRaised(string p_configuration)
    {
        ComputeTotalCost();
    }

    public void OnConfigurationEventRequestRaised(GameObject p_object, string p_configurationGroupName, bool p_requested)
    {
        ConfigurationGroup l_configurationGroup = null;
        foreach(ConfigurationGroup l_group in _configurationGroups)
        {
            if(l_group.Name == p_configurationGroupName)
            {
                l_configurationGroup = l_group;
                break;
            }
        }
        if(l_configurationGroup != null)
        {
            if (p_requested)
            {
                l_configurationGroup.AddConfiguredObject(p_object);
                l_configurationGroup.SetOption(l_configurationGroup.CurrentOptionIndex);
            }
            else
            {
                l_configurationGroup.RemoveConfiguredObject(p_object);
            }
            
        }
    }

    public void OnConfigurationEventInfosRaised(ConfigurationOption p_option)
    {

    }

    public void OnConfigurationEventOptionUpdatedRaised(ConfigurationOption p_option)
    {

    }

    public void RaiseCurrentOptionInfos(string p_groupName)
    {
        foreach(ConfigurationGroup l_group in _configurationGroups)
        {
            if(l_group.Name == p_groupName)
            {
                _configurationEvent.RaiseInfos(l_group.CurrentOption);
                return;
            }
        }

        Debug.LogWarning($"Error: wrong choice of group ({p_groupName}). Group does not exist.");
    }

    public bool SaveConfiguration()
    {
        Debug.Log("ConfigurationManager SaveConfiguration");
        if(_configurationSerialization != null)
        {
            string l_file = _configurationSerialization.SerializeConfiguration(this);
            return l_file != null && l_file.Length > 0;
        }
        return false;
    }

    public bool LoadConfiguration()
    {
        if(_configurationSerialization != null)
        {
            ConfigurationData l_configData = _configurationSerialization.LoadSerializedConfiguration(this);
            if(l_configData != null && l_configData._groups != null)
            {
                foreach(ConfigurationData.ConfigurationGroupData l_groupData in l_configData._groups)
                {
                    foreach(ConfigurationGroup l_group in _configurationGroups)
                    {
                        if(l_groupData._groupName == l_group.Name)
                        {
                            l_group.SetCostCoeff(_costCoeff);
                            l_group.SetOption(l_groupData._groupCurrentOption);
                            l_group.SetContributeToTotalCost(l_groupData._groupContribution);
                            break;
                        }
                    }
                }
                ComputeTotalCost();
                return true;
            }
        }

        return false;
    }
}
