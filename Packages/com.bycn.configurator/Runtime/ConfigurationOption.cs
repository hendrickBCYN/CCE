using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationOption : ScriptableObject
{
    [SerializeField] protected string _optionName = "Mon option";
    public enum OptionEnum { Material, Object}
    [SerializeField] protected OptionEnum _optionType = OptionEnum.Material;

    [SerializeField] protected string _optionReference = "Référence de mon option";

    [SerializeField] private bool _hasDimensions = false;
    [SerializeField] private string _optionDimensions = "Dimensions de mon option";

    [SerializeField] private bool _hasCost = false;
    public enum OptionCostType { Piece, Surface, MetreLineaire }
    [SerializeField] private OptionCostType _optionCostType = OptionCostType.Piece;
    [SerializeField] private float _optionCost = 0f;
    [SerializeField] private float _optionCostTotal = 0f;

    [SerializeField] private RawImage _optionImage = null;

    private float _costCoeff = 1f;

    [SerializeField] protected ConfigurationEvent _configurationEvent;

    public void SetOptionData(float p_cost)
    {
        _optionCost = p_cost;
    }

    public void SetCostCoeff(float p_coeff)
    {
        _costCoeff = p_coeff;
    }

    public string Name => _optionName;
    public string Reference => _optionReference;
    public OptionEnum OptionType => _optionType;

    public bool HasCost => _hasCost;
    public float Cost => _optionCost * _costCoeff;
    public float TotalCost => _optionCostTotal * _costCoeff;
    public string CostToString ()
    {
        string l_result = Cost.ToString("F2") + " €";
        switch(_optionCostType)
        {
            case OptionCostType.Piece:
                l_result += "/piece";
                break;
            case OptionCostType.Surface:
                l_result += "/m²";
                break;
            case OptionCostType.MetreLineaire:
                l_result += "/ml";
                break;
        }
        l_result += " HT";

        return _hasCost ? l_result : "/";
    }

    public bool HasDimensions => _hasDimensions;
    public string Dimensions => _hasDimensions ? _optionDimensions : "/";
}