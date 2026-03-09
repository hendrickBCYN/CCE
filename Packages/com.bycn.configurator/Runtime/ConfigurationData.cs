using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigurationData
{
    [Serializable]
    public class ConfigurationGroupData
    {
        public string _groupName;
        public int _groupDefaultOption;
        public int _groupCurrentOption;
        public bool _groupContribution;
        public float _groupTotalCost;

        public void Init(ConfigurationGroup p_group)
        {
            _groupName = p_group.Name;
            _groupDefaultOption = p_group.DefaultOptionIndex;
            _groupCurrentOption = p_group.CurrentOptionIndex;
            _groupContribution = p_group.ContributeToTotalCost;
            _groupTotalCost = p_group.CurrentOption != null ? p_group.CurrentOption.TotalCost : 0;
        }
    }

    public ConfigurationGroupData[] _groups;
    public float _totalCost;

    public void Init(ConfigurationManager p_configManager)
    {
        List<ConfigurationGroupData> l_groupsData = new List<ConfigurationGroupData>();
        foreach (ConfigurationGroup group in p_configManager.ConfigurationGroups)
        {
            ConfigurationGroupData l_groupData = new ConfigurationGroupData();
            l_groupData.Init(group);
            l_groupsData.Add(l_groupData);
        }

        _groups = l_groupsData.ToArray();
        _totalCost = p_configManager.TotalCost;
    }
}
