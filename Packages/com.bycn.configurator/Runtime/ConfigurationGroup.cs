using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationGroup : MonoBehaviour, IConfigurationEventListener
{
    [Serializable]
    public struct MessagedObject
    {
        public GameObject _object;
        public string _method;
    }

    [SerializeField] string _name;
    [SerializeField] GameObject[] _configuredObjects;
    [SerializeField] MessagedObject[] _messagedObjects;
    List<GameObject> _temporaryConfiguredObjects = new List<GameObject>();
    [SerializeField] ConfigurationOption[] _configurationOptions;
    [SerializeField] bool _contributeToTotalCost = true;

    public enum ConfigurationGroupType { Piece, Surface, MetreLineaire }
    [SerializeField] ConfigurationGroupType _configurationGroupType = ConfigurationGroupType.Piece;

    [SerializeField] int _nbPieces = 0;
    [SerializeField] float _surface = 0f;
    [SerializeField] float _metres = 0f;

    private int _defaultOptionIndex = 0;
    private int _currentOptionIndex = 0;
    private int _optionOffset = 0;
    private ConfigurationOption _currentOption = null;
    [SerializeField] ColoredUIGroup _uiOptionGroup = null;

    [SerializeField] float _totalCost = 0f;

    public string Name => _name;
    public int DefaultOptionIndex => _defaultOptionIndex;
    public int CurrentOptionIndex => _currentOptionIndex;
    public int NbOptions => _configurationOptions.Length;

    private GameObject _instantiatedObject = null;

    public ConfigurationOption CurrentOption => _currentOption;

    public bool ContributeToTotalCost => _contributeToTotalCost;

    [SerializeField] private ConfigurationEvent _configurationEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        ClearOptions();
        if (_uiOptionGroup != null)
            _uiOptionGroup.InitOptions();
        SetOption(_currentOptionIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (_configurationEvent != null)
            _configurationEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if(_configurationEvent != null)
            _configurationEvent.UnregisterListener(this);
    }

    public void SetCostCoeff(float p_costCoeff)
    {
        foreach(var option in _configurationOptions)
        {
            option.SetCostCoeff(p_costCoeff);
        }
    }

    public void SetOption(int p_option, bool p_immediate = false)
    {
        if (p_option >= 0 && p_option < _configurationOptions.Length)
        {
            _currentOptionIndex = p_option + _optionOffset;
            _currentOption = _configurationOptions[p_option];
            switch (_currentOption.OptionType)
            {
                case ConfigurationOption.OptionEnum.Material:
                    ApplyMaterial((ConfigurationOptionMaterial)_currentOption, p_immediate);
                    break;
                case ConfigurationOption.OptionEnum.Object:
                    ApplyObject((ConfigurationOptionObject)_currentOption, p_immediate);
                    break;
            }

            _totalCost = _currentOption.TotalCost;

            if (_uiOptionGroup != null)
            {
                _uiOptionGroup.SelectItem(p_option);
            }
        }
        else
        {
            Debug.LogWarning($"Error : wrong option index ({p_option})! ");
            _currentOption = null;
            _totalCost = 0f;
        }
    }

    public void SetContributeToTotalCost(bool p_contribute)
    {
        _contributeToTotalCost = p_contribute;
    }

    public void ClearOptions()
    {
        if(_configurationOptions.Length > 0 )
        {
            switch (_configurationOptions[0].OptionType)
            {
                case ConfigurationOption.OptionEnum.Material:
                    break;
                case ConfigurationOption.OptionEnum.Object:
                    ClearObject();
                    break;
            }
        }
        _currentOption = null;
    }

    void ApplyMaterial(ConfigurationOptionMaterial p_material, bool p_immediate)
    {
        foreach (var obj in _configuredObjects)
        {
            Material[] l_materials = new Material[obj.GetComponent<Renderer>().materials.Length];
            for (int i = 0; i < l_materials.Length; ++i)
                l_materials[i] = p_material.Material;
            obj.GetComponent<Renderer>().materials = l_materials;
        }

        foreach(var obj in _messagedObjects)
        {
            obj._object.SendMessage(obj._method, p_material.Material);
        }

        foreach (var obj in _temporaryConfiguredObjects)
        {
            if (obj != null)
            {
                if (obj.GetComponentInChildren<Renderer>())
                {
                    Material[] l_materials = new Material[obj.GetComponentInChildren<Renderer>().materials.Length];
                    for (int i = 0; i < l_materials.Length; ++i)
                        l_materials[i] = p_material.Material;
                    obj.GetComponentInChildren<Renderer>().materials = l_materials;
                }
            }
        }
    }

    void ApplyObject(ConfigurationOptionObject p_object, bool p_immediate)
    {
        foreach (var obj in _configuredObjects)
        {
            foreach(Transform t in obj.transform)
            {
                if (t.gameObject.layer != LayerMask.NameToLayer("ManipulationHelper"))
                {
                    if (p_immediate)
                        DestroyImmediate(t.gameObject);
                    else
                        Destroy(t.gameObject);
                }
            }

            if (p_object != null && p_object.Object != null)
            {
                GameObject l_newObject = Instantiate(p_object.Object, obj.transform);
                l_newObject.transform.localPosition = p_object.Position;
                l_newObject.transform.localRotation = Quaternion.Euler(p_object.Orientation);
                l_newObject.transform.localScale = p_object.Scale != Vector3.zero ? p_object.Scale : Vector3.one;
            }
        }

        foreach (var obj in _messagedObjects)
        {
            obj._object.SendMessage((p_object.Object != null ? obj._method : obj._method+"Null"), p_object.Object);
        }

        foreach (var obj in _temporaryConfiguredObjects)
        {
            if (obj != null)
            {
                foreach (Transform t in obj.transform)
                {
                    if (t.gameObject.layer != LayerMask.NameToLayer("ManipulationHelper"))
                    {
                        if (p_immediate)
                            DestroyImmediate(t.gameObject);
                        else
                            Destroy(t.gameObject);
                    }
                }

                if (p_object != null && p_object.Object != null)
                {
                    GameObject l_newObject = Instantiate(p_object.Object, obj.transform);
                    l_newObject.transform.localPosition = p_object.Position;
                    l_newObject.transform.localRotation = Quaternion.Euler(p_object.Orientation);
                    l_newObject.transform.localScale = p_object.Scale != Vector3.zero ? p_object.Scale : Vector3.one;
                }
            }
        }
    }

    public void AddConfiguredObject(GameObject p_objectToConfigure)
    {
        if(!_temporaryConfiguredObjects.Contains(p_objectToConfigure))
            _temporaryConfiguredObjects.Add(p_objectToConfigure);

        for(int i =0; i <  _temporaryConfiguredObjects.Count; i++)
        {
            if (_temporaryConfiguredObjects[i] == p_objectToConfigure && _temporaryConfiguredObjects[i].GetComponent<DynamicConfiguredObject>() != null)
            {
                p_objectToConfigure.GetComponent<DynamicConfiguredObject>().SetTemporaryIndex(i);
                break;
            }
        }
    }

    public void RemoveConfiguredObject(GameObject p_objectToConfigure)
    {
        if (p_objectToConfigure != null && p_objectToConfigure.GetComponent<DynamicConfiguredObject>() != null)
        {

        }
        else if (p_objectToConfigure != null)
        {
            if (_temporaryConfiguredObjects.Contains(p_objectToConfigure))
                _temporaryConfiguredObjects.Remove(p_objectToConfigure);
        }
        else
        {
            int l_count = 0;
            while(l_count < _temporaryConfiguredObjects.Count)
            {
                if (_temporaryConfiguredObjects[l_count] == null)
                {
                    _temporaryConfiguredObjects.RemoveAt(l_count);
                }
                else { l_count++; }
            }
        }
    }

    void ClearObject()
    {
        if(_instantiatedObject != null)
            Destroy(_instantiatedObject );
        _instantiatedObject = null;
    }

    public void OnConfigurationEventRaised(string p_configuration)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfigurationEventRequestRaised(GameObject p_object, string p_configurationGroupName, bool p_requested)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfigurationEventInfosRaised(ConfigurationOption p_option)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfigurationEventOptionUpdatedRaised(ConfigurationOption p_option)
    {
        foreach(ConfigurationOption l_option in _configurationOptions)
        {
            if (l_option == p_option && l_option == _currentOption)
                SetOption(_currentOptionIndex);
        }
        //throw new System.NotImplementedException();
    }
}
