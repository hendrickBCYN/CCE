using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialOption", menuName = "Configurator/MaterialOption", order = 1)]
public class ConfigurationOptionMaterial : ConfigurationOption
{
    [SerializeField] Material _material;

    public Material Material => _material;

    public void SetMaterialOptionData(Material p_material, float p_cost)
    {
        _optionType = OptionEnum.Material;
        SetOptionData(p_cost);
        _material = p_material;
        if (_configurationEvent != null)
            _configurationEvent.RaiseOptionUpdated(this);
    }
}
