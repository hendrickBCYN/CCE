using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WallManager : MonoBehaviour, IWallEventListener
{ 
    [SerializeField] private GameObject _prefabWall;
    [SerializeField] private List<GameObject> _walls;
    [SerializeField] private GameObject _post;

    [SerializeField] Toggle _ProtectionTeteDeLit;
    [SerializeField] Toggle _ProtectionMurFacade;
    [SerializeField] Toggle _ProtectionPiedDeLit;
    [SerializeField] Toggle _ProtectionGlobal;
    [SerializeField] GameObject _DetailsToggles;

    [SerializeField] WallEvent _wallEvent;

    private bool[] _isWallProtected = { false, false, false, false };
    private string[] _wallNames = { "Mur_Entree", "Mur_PiedDeLit", "Mur_Fenetre", "Mur_TeteDeLit" };

    private int _numberOfWalls;
    [SerializeField] private float _wallHeight = 3.0f;
    [SerializeField] private float _wallHeightMin = 1.8f;
    [SerializeField] private float _wallThickness = 0.2f;
    [SerializeField] private float _wallProtectionHeight = 0.8f;
    [SerializeField] private float _wallProtectionHeightMin = 0f;
    [SerializeField] private float _wallProtectionThickness = 0.001f;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _defaultProtectionMaterial;
    [SerializeField] private float _postLength = 0.4f;
    [SerializeField] private float _postWidth = 0.25f;
    Material _currentProtectionMaterial;

    private bool _wallProtectionFloorExtensionActive = false;
    public bool IsWallProtectionFloorExtension => _wallProtectionFloorExtensionActive;

    [SerializeField] private ConfigurationManager _configurationManager;

    // Start is called before the first frame update
    void Start()
    { 
        _ProtectionTeteDeLit.onValueChanged.AddListener(delegate { ProtectionUpdated(_ProtectionTeteDeLit); });
        _ProtectionMurFacade.onValueChanged.AddListener(delegate { ProtectionUpdated(_ProtectionMurFacade); });
        _ProtectionPiedDeLit.onValueChanged.AddListener(delegate { ProtectionUpdated(_ProtectionPiedDeLit); });
        _ProtectionGlobal.onValueChanged.AddListener(delegate { ProtectionUpdated(_ProtectionGlobal); });

        _currentProtectionMaterial = _defaultProtectionMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        _wallEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _wallEvent.UnregisterListener(this);
    }

    public void UpdateDimensions(float p_height, float p_thickness, float p_protectionHeight)
    {
        float newHeight = _wallHeight + p_height;
        float newThickness = _wallThickness + p_thickness;
        float newProtectionHeight = _wallProtectionHeight + p_protectionHeight;

        if (newHeight >= _wallHeightMin && newThickness > 0 && newProtectionHeight >= _wallProtectionHeightMin)
        {
            _wallHeight += p_height;
            _wallThickness += p_thickness;
            _wallProtectionHeight += p_protectionHeight;
            _wallEvent.RaiseDimensions(_wallHeight, _wallThickness, _wallProtectionHeight);
        }
    }

    public void InitWalls(GameObject p_roomParent, int p_numberOfWalls, float p_width, float p_length, List<float> p_angles)
    {
        Debug.Log("WallManager - InitWalls");
        _numberOfWalls = p_numberOfWalls;
        bool l_create = true;
        if (_walls.Count != _numberOfWalls)
        {
            Debug.LogWarning("WallManager - Room must have 4 walls");
            return;
        }
        
        foreach(var wall in _walls)
            if(wall.GetComponent<Wall>() == null)
            {
                Debug.LogWarning($"WallManager - Wall {wall.name} does not have a Wall component");
                return;
            }

        //Init post
        if(_post != null && _post.GetComponent<Post>() != null)
        {
            _post.GetComponent<Post>().UpdatePost(_postWidth, _postLength, p_width, p_length, _wallThickness, _wallHeight);
        }

        //Initialize walls
        for (int i = 0; i < _numberOfWalls; i++)
        {
            float sign = (i < _numberOfWalls / 2.0f) ? 1 : -1;
            float size = (i % 2 == 0) ? p_width : p_length + _wallThickness * 2f;
            float invertSize = (i % 2 == 0) ? p_length : p_width;

            GameObject wall = _walls[i];
            wall.GetComponent<Wall>().Init(i, _wallNames[i], i % 2 == 0);
            if(i == 0)
                _walls[i].GetComponent<Wall>().InitWall(size, 0, p_angles[i], _wallHeight, _wallThickness, _wallProtectionHeight, _wallProtectionThickness);
            else
            {
                float rot = _walls[i - 1].GetComponent<Wall>().GetNextWallAngle() + _walls[i - 1].transform.eulerAngles.y;
                _walls[i].GetComponent<Wall>().InitWall(size, rot, p_angles[i], _wallHeight, _wallThickness, _wallProtectionHeight, _wallProtectionThickness);
            }
            Vector3 newPosition = (i % 2 == 0) ? new Vector3(0, 0, (invertSize / 2) * sign) : new Vector3((invertSize / 2) * sign, 0, 0);
            _walls[i].transform.localPosition = newPosition;
            _walls[i].GetComponent<Wall>().WallProtection(_isWallProtected[i]);
            _walls[i].GetComponent<Wall>().ChangeMaterial(_defaultMaterial);
            _walls[i].GetComponent<Wall>().ChangeProtectedMaterial(_defaultProtectionMaterial);
        }
        _wallEvent.RaiseDimensions(_wallHeight, _wallThickness, _wallProtectionHeight);
    }

    public void UpdateWalls(float p_width, float p_length, List<float> p_angles)
    {
        Debug.Log("WallManager - UpdateWalls");

        if (_post != null && _post.GetComponent<Post>() != null)
            _post.GetComponent<Post>().UpdatePost(_postWidth, _postLength, p_width, p_length, _wallThickness, _wallHeight);

        for (int i = 0; i < _numberOfWalls; i++)
        {
            Vector3 position = new Vector3(0, 0, 0);

            if (_numberOfWalls == 4)
            {
                float sign = (i < _numberOfWalls / 2.0f) ? 1 : -1;
                float size = (i % 2 == 0) ? p_width : p_length + _wallThickness * 2f;
                float invertSize = (i % 2 == 0) ? p_length : p_width;

                if (i > 0)
                {
                    float rot = _walls[i - 1].GetComponent<Wall>().GetNextWallAngle() + _walls[i - 1].transform.eulerAngles.y;
                    _walls[i].GetComponent<Wall>().UpdateWall(size, rot, p_angles[i], _wallHeight, _wallThickness, _wallProtectionHeight, _wallProtectionThickness);
                }
                else
                {
                    _walls[i].GetComponent<Wall>().UpdateWall(size, 0, p_angles[i], _wallHeight, _wallThickness, _wallProtectionHeight, _wallProtectionThickness);
                }
                position = (i % 2 == 0) ? new Vector3(0, 0, (invertSize / 2) * sign) : new Vector3((invertSize / 2) * sign, 0, 0);
                _walls[i].transform.localPosition = position;
                _walls[i].GetComponent<Wall>().WallProtection(_isWallProtected[i]);
            }
        }
    }

    public void SetAllWallsProtected(bool p_protected)
    {
        for(int i = 1; i < _numberOfWalls; i++)
        {
            _isWallProtected[i] = p_protected;
            _walls[i].GetComponent<Wall>().WallProtection(_isWallProtected[i]);
        }
    }

    public void SetWallProtected(int p_id, bool p_protected)
    {
        _isWallProtected[p_id] = p_protected;
        _walls[p_id].GetComponent<Wall>().WallProtection(_isWallProtected[p_id]);
    }

    public void SetWallsProtected(bool[] p_protected)
    {
        _isWallProtected = p_protected;
        for (int i = 1; i < _numberOfWalls; i++)
        {
            _walls[i].GetComponent<Wall>().WallProtection(_isWallProtected[i]);
        }
    }

    public bool [] GetWallsProtected() { return _isWallProtected; }
    
    void ProtectionUpdated(Toggle p_toggle)
    {
        Debug.Log("WallManager - ProtectionUpdated");
        if(p_toggle == _ProtectionTeteDeLit)
        {
            _wallEvent.Raise(3, p_toggle.isOn);
        }
        else if(p_toggle == _ProtectionMurFacade)
        {
            _wallEvent.Raise(2, p_toggle.isOn);
        }
        else if (p_toggle == _ProtectionPiedDeLit)
        {
            _wallEvent.Raise(1, p_toggle.isOn);
        }
        else if (p_toggle == _ProtectionGlobal)
        {
            _ProtectionTeteDeLit.isOn = p_toggle.isOn;
            _ProtectionTeteDeLit.interactable = p_toggle.isOn;
            _ProtectionMurFacade.isOn = p_toggle.isOn;
            _ProtectionMurFacade.interactable = p_toggle.isOn;
            _ProtectionPiedDeLit.isOn = p_toggle.isOn;
            _ProtectionPiedDeLit.interactable = p_toggle.isOn;
        }

        //UpdateProtectionsMaterial(_currentProtectionMaterial);
    }

    public void UpdateMaterials(List<Material> p_wallMaterials, List<Material> p_wallProtectionMaterials, bool p_wallProtectionMaterialAsFloor)
    {
        for (int i = 0; i < _numberOfWalls; i++) //Wall 0 is WallDoor
        {
            _walls[i].GetComponent<Wall>().ChangeMaterial(p_wallMaterials[i]);

            if (!p_wallProtectionMaterialAsFloor)
            {
                if (_walls[i].GetComponent<Wall>().IsProtected())
                {
                    _walls[i].GetComponent<Wall>().ChangeProtectedMaterial(p_wallProtectionMaterials[i]);
                }
            }
        }
    }

    public void UpdateProtectionsMaterial(Material p_material)
    {
        GameObject[] protectionObjects = GameObject.FindGameObjectsWithTag("Protection");
        _currentProtectionMaterial = p_material;
        foreach (GameObject obj in protectionObjects)
        {
            if(_wallProtectionFloorExtensionActive)
                obj.transform.parent.GetComponent<Wall>().ChangeProtectedMaterial(new Material(_currentProtectionMaterial));
            else
                obj.transform.parent.GetComponent<Wall>().ChangeProtectedMaterial(_currentProtectionMaterial);
        }

        if (_wallProtectionFloorExtensionActive)
        {
            UpdateWallProtectionMaterialTiling();
        }
    }

    public void SetWallProtectionFloorExtension(bool p_value)
    {
        _wallProtectionFloorExtensionActive = p_value;
    }

    Material GetWallMaterial(int p_id)
    {
        if (p_id >= 0 && p_id < _walls.Count)
            return _walls[p_id].GetComponent<Wall>().GetCurrentMaterial();
        return null;
    }

    public string[] GetWallMaterialsAsStringArray()
    {
        string[] result = new string[_walls.Count];
        for (int i = 0; i < _walls.Count; i++)
        {
            Material tmpMat = GetWallMaterial(i);
            result[i] = tmpMat != null ? tmpMat.name.Replace(" (Instance)", "") : "null";
        }

        return result;
    }

    Material GetWallProtectionMaterial(int p_id)
    {
        if (p_id >= 0 && p_id < _walls.Count)
            return _walls[p_id].GetComponent<Wall>().GetProtectionMaterial();
        return null;
    }

    public string[] GetWallProtectionMaterialsAsStringArray()
    {
        string[] result = new string[_walls.Count];
        for (int i = 0; i < _walls.Count; i++)
        {
            Material tmpMat = GetWallProtectionMaterial(i);
            result[i] = tmpMat != null ? tmpMat.name.Replace(" (Instance)", "") : "null";
        }

        return result;
    }

    void UpdateWallProtectionMaterialTiling()
    {
        for (int i = 0; i < _walls.Count; i++)
        {
            if (_isWallProtected[i])
            {
                _walls[i].GetComponent<Wall>().GetProtectionMaterial().mainTextureScale = new(_walls[i].GetComponent<Wall>().GetLength(), _wallProtectionHeight);
            }
        }
    }

    public void AdjustWalls(float p_roomWidth, BathroomManager p_bathroomManager, DoorManager p_doorManager, WindowManager p_windowManager)
    {
        Vector3 l_bathroomBounds = p_bathroomManager.GetBathroomBounds();
        float l_bathroomThickness = p_bathroomManager.GetBathroomThickness();
        Vector2 l_bathroomExtentOutside = p_bathroomManager.GetBathroomExtentOutside();

        Vector2 l_entryDoorDimensions = p_doorManager.GetEntryDoorDimensions();
        Vector3 l_entryDoorOffset = Vector3.zero;// p_doorManager.GetEntryDoorOffset();
        //Update Wall 0 (Entree)
        //New wall widht is roomWidth - size of Bathroom (which is in the corner). The entry door is "inside" wall width
        float l_newWidth = _walls[0].GetComponent<Wall>().GetLength() - (l_bathroomBounds.x - l_bathroomThickness) + l_bathroomExtentOutside.x - ((_post.GetComponent<Post>().Width / 2f - _wallThickness / 2f));
        Vector2 l_doorLocalPosition = new Vector2(l_newWidth + l_entryDoorOffset.x - l_entryDoorDimensions.x / 2f - ((_post.GetComponent<Post>().Width / 2f - _wallThickness / 2f)), l_entryDoorOffset.y);
        _walls[0].GetComponent<Wall>().UpdateExtrudedWall(l_newWidth, l_entryDoorDimensions, l_doorLocalPosition);
        _walls[0].transform.localPosition = new Vector3(-(_walls[0].GetComponent<Wall>().GetLength() - p_roomWidth) / 2f - ((_post.GetComponent<Post>().Width / 2f - _wallThickness / 2f)), _walls[0].transform.localPosition.y, _walls[0].transform.localPosition.z);

        //Update Wall 1 (PiedDeLit)
        l_newWidth = _walls[1].GetComponent<Wall>().GetLength() - (_post.GetComponent<Post>().Length);
        _walls[1].GetComponent<Wall>().UpdateWall(l_newWidth);
        _walls[1].transform.localPosition = new Vector3(_walls[1].transform.localPosition.x, _walls[1].transform.localPosition.y, -(_post.GetComponent<Post>().Length) / 2f);

        Vector2 l_windowDimensions = p_windowManager.GetWindowDimensions();
        Vector3 l_windowOffset = p_windowManager.GetWindowOffset();
        //Update Wall 2 (Facade)
        Vector2 l_windowLocalPosition = new Vector2(p_roomWidth / 2f + l_windowOffset.x, l_windowOffset.y);
        _walls[2].GetComponent<Wall>().UpdateExtrudedWall(p_roomWidth, l_windowDimensions, l_windowLocalPosition);

    }

    public float GetWallHeight() { return _wallHeight; }
    public float GetWallThickness() {  return _wallThickness; }
    public float GetWallProtectionHeight() {  return _wallProtectionHeight; }

    public float GetWallProtectionThickness() {  return _wallProtectionThickness; }

    public Vector2 GetPostDimensions() { return new Vector2(_postWidth, _postLength); }
    void IWallEventListener.OnWallEventRaised(int p_wallID, bool p_protected)
    {
        //SetWallProtected(p_wallID, p_protected);
        if(_configurationManager != null)
        {
            DynamicConfiguredObject[] l_comps = _walls[p_wallID].GetComponentsInChildren<DynamicConfiguredObject>();
            if (l_comps != null)
            {
                string l_config = l_comps[0].ConfigurationGroupName + ";" + (p_protected ? "1" : "0");
                Debug.Log("Update config :" + l_config);
                _configurationManager.SetConfigurationOption(l_config);
            }
        }
    }

    void IWallEventListener.OnWallDimensionsEventRaised(float p_height, float p_thickness, float p_protectionHeight)
    {
        UpdateWallProtectionMaterialTiling();
    }
}
