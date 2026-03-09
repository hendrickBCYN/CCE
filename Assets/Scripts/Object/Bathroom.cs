using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathroom : Furniture
{
    [SerializeField] private GameObject _extraPart;
    [SerializeField] private GameObject _falseCeiling;
    [SerializeField] private float _height;
    [SerializeField] private float _thickness;
    [SerializeField] private Vector2 _extentOutside;
    [SerializeField] private float _falseCeilingBaseHeigh = 2.5f;
    [SerializeField] private Collider _bounds;
    [SerializeField] Floor [] _floors;

    [SerializeField] RoomEvent _roomEvent;

    // Update is called once per frame
    void Update()
    {
        
    }

    override protected void Init()
    {
        _floors = GetComponentsInChildren<Floor>(true);
        if (_roomEvent)
            _roomEvent.RaiseBathroomUpdated(gameObject);
    }

    public void UpdateExtraPart(float p_wallHeight)
    {
        if (_extraPart != null)
        {
            float diff = p_wallHeight - _height;
            if (diff <= 0)
            {
                _extraPart.transform.localScale = new Vector3(1, 0.001f, 1);
                _extraPart.SetActive(false);
            }
            else
            {
                _extraPart.SetActive(true);
                _extraPart.transform.localScale = new Vector3(1, diff, 1);
            }
        }
    }

    public void UpdateFalseCeiling(float p_wallHeight)
    {
        if (_falseCeiling != null)
        {
            float diff = p_wallHeight - _falseCeilingBaseHeigh;
            if (diff <= 0)
            {
                _falseCeiling.SetActive(false);
            }
            else
            {
                _falseCeiling.SetActive(true);
                Vector3 l_scale = _falseCeiling.transform.localScale;
                l_scale.y = diff;
                _falseCeiling.transform.localScale = l_scale;
                Vector3 l_pos = _falseCeiling.transform.localPosition;
                l_pos.y = _falseCeilingBaseHeigh + diff / 2f;
                _falseCeiling.transform.localPosition = l_pos;
            }
        }
    }
    public float GetThickness() => _thickness;

    public Collider GetBounds() { return _bounds; }

    public Vector2 GetExtentOutside() {  return _extentOutside; }

    public void ChangeFloorMaterial(Material m)
    {
        foreach (Floor l_floor in _floors)
        {
            l_floor.ChangeMaterial(m);
            l_floor.UpdateMaterialTiling(_bounds.bounds.size.x, _bounds.bounds.size.z);
        }
    }
}
