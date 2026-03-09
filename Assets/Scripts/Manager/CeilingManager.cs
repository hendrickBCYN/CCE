using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CeilingManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabCeiling;
    //RailsPrefabs

    [SerializeField] private GameObject _ceilingRoot;
    [SerializeField] private GameObject _falseCeilingRoot;
    
    private Ceiling _ceiling;
    private Ceiling _falseCeiling;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitCeiling(GameObject p_roomParent, float p_width, float p_length, float p_wallHeight, float p_wallThickness, float p_falseCeilingHeight)
    {
        if (_ceilingRoot == null)
        {
            _ceilingRoot = new GameObject("Ceiling");
        }
        _ceilingRoot.transform.parent = p_roomParent.transform;
        _ceilingRoot.transform.localPosition = new Vector3(0, p_wallHeight, 0);
        //Instantiate floor
        _ceiling = Instantiate(_prefabCeiling, Vector3.zero, Quaternion.identity, _ceilingRoot.transform).GetComponent<Ceiling>();
        //_ceiling.name = "MainCeiling";
        _ceiling.InitCeiling(p_width, p_length, p_wallThickness, false);

        _falseCeilingRoot.transform.localPosition = new Vector3(_falseCeilingRoot.transform.localPosition.x, p_falseCeilingHeight, _falseCeilingRoot.transform.localPosition.z);
        _falseCeiling = Instantiate(_prefabCeiling, Vector3.zero, Quaternion.identity, _falseCeilingRoot.transform).GetComponent<Ceiling>();
        _falseCeiling.name = "FalseCeiling";
        _falseCeiling.InitCeiling(1f, 1f, p_wallThickness, true);
    }

    public void UpdateCeiling(float p_width, float p_length, float p_wallHeight, float p_wallThickness, float p_falseCeilingHeight, Vector3 p_bathroomBounds, Vector2 p_bathroomExtentOutside)
    {
        _ceilingRoot.transform.localPosition = new Vector3(0, p_wallHeight, 0);
        _ceiling.UpdateCeiling(p_width, p_length, p_wallThickness);

        float l_fWidth = p_width - p_bathroomBounds.x + p_bathroomExtentOutside.x;
        float l_fLength = p_bathroomBounds.z - p_bathroomExtentOutside.y;
        _falseCeilingRoot.transform.localPosition = new Vector3((p_width - l_fWidth) / 2, p_falseCeilingHeight, (p_length - l_fLength)/2f);
        _falseCeiling.UpdateCeiling(l_fWidth, l_fLength, p_wallThickness / 2f);
        _falseCeiling.UpdateCeilingThickness(p_wallHeight - p_falseCeilingHeight);
    }
}
