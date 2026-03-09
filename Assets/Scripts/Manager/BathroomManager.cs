using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BathroomManager : MonoBehaviour
{
    //PREFAB
    private GameObject _bathroom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateBathroom(float p_width, float p_length, float p_wallHeight)
    {
        Debug.Log("Update BathRoom");
        if (_bathroom != null)
        {
            //_bathroom.GetComponent<Bathroom>().UpdateExtraPart(p_wallHeight);
            _bathroom.GetComponent<Bathroom>().UpdateFalseCeiling(p_wallHeight);
        }
    }

    public void ChangeBathroomFloorMaterial(Material p_material)
    {
        if(_bathroom != null)
            _bathroom.GetComponent<Bathroom>().ChangeFloorMaterial(p_material);
    }

    public Vector3 GetBathroomBounds()
    {
        if (_bathroom != null)
        {
            return _bathroom.GetComponent<Bathroom>().GetBounds().bounds.size; 
        }
        return Vector3.zero;
    }

    public float GetBathroomThickness() {
        if(_bathroom != null)
            return _bathroom.GetComponent<Bathroom>().GetThickness();
        return 0f;
    }

    public Vector2 GetBathroomExtentOutside()
    {
        if (_bathroom != null)
            return _bathroom.GetComponent<Bathroom>().GetExtentOutside();
        return Vector2.zero;
    }

    public void SetBathroom(GameObject p_bathroom)
    {
        _bathroom = p_bathroom;
    }
}
