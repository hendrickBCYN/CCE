using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabFloor;
    [SerializeField] private ColoredUIItem[] _gammes;
    [SerializeField] private FloorEvent _floorEvent;
    [SerializeField] private Material _defaultMaterial;

    [SerializeField] private Floor _floor;

    [SerializeField] private ConfigurationOptionMaterial _configProtectionCommeSol;

    public Material DefaultMaterial => _defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (_gammes != null && _gammes.Length > 0)
            ChooseGamme(_gammes[0]);
    }

    public void InitFloor(GameObject p_roomParent, float width, float length)
    {
        //Instantiate floor
        if(_floor == null)
            _floor = Instantiate(_prefabFloor, Vector3.zero, Quaternion.identity, p_roomParent.transform).GetComponent<Floor>();
        _floor.InitFloor(width, length);
        ChangeFloorMaterial(_defaultMaterial);
    }

    public void ChooseGamme(ColoredUIItem p_gamme)
    {
        foreach (ColoredUIItem item in _gammes)
        {
            item.SetColors(
                item == p_gamme ? UIManager.Orange : Color.white,
                item == p_gamme ? Color.white : UIManager.Orange,
                item == p_gamme ? Color.white : Color.black
            );
        }
    }

    public void UpdateFloor(float p_width, float p_length)
    {
        _floor.UpdateFloor(p_width, p_length);   
        UpdateFloorMaterialTiling();
    }

    public void ChangeFloorMaterial(Material m)
    {
        _floor.ChangeMaterial(m);
        UpdateFloorMaterialTiling();
        if (_configProtectionCommeSol != null)
            _configProtectionCommeSol.SetMaterialOptionData(m, 0f);
        _floorEvent.Raise(m);
    }

    void UpdateFloorMaterialTiling()
    {
        _floor.UpdateMaterialTiling(_floor.transform.localScale.x, _floor.transform.localScale.z);
    }

    public Material GetFloorMaterial()
    {
        return _floor.GetCurrentMaterial();
    }
}
