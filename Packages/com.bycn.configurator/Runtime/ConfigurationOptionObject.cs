using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectOption", menuName = "Configurator/ObjectOption", order = 1)]
public class ConfigurationOptionObject : ConfigurationOption
{
    [SerializeField] GameObject _object;
    [SerializeField] Vector3 _position = Vector3.zero;
    [SerializeField] Vector3 _orientation = Vector3.zero;
    [SerializeField] Vector3 _scale = Vector3.one;

    public GameObject Object => _object;
    public Vector3 Position => _position;
    public Vector3 Orientation => _orientation;
    public Vector3 Scale => _scale;

    public void SetObjectOptionData(GameObject p_object, float p_cost, Vector3 p_position, Vector3 p_orientation, Vector3 p_scale)
    {
        _optionType = OptionEnum.Object;
        SetOptionData(p_cost);
        _object = p_object;
        _position = p_position;
        _orientation = p_orientation;
        _scale = p_scale;
    }
}
