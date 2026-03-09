using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour, IRoomEventListener
{
    //MANAGERS
    [SerializeField] FloorManager _floorManager;
    [SerializeField] WallManager _wallManager;
    [SerializeField] WindowManager _windowManager;
    [SerializeField] CeilingManager _ceilingManager;
    [SerializeField] BathroomManager _bathroomManager;
    [SerializeField] DoorManager _doorManager;
    [SerializeField] FurnitureManager _furnitureManager;

    //ROOM DATAS
    private float _length;
    private float _lengthMin = 3.05f;
    private float _width;
    private float _widthMin = 3.05f;
    private float _widthMax = 4.10f;
    private float _area;
    private Vector2 _areaLimits = new Vector2(19, 30);
    private float _falseCeilingHeight = 2.5f;

    private int _numberOfWalls = 4;

    //ROOM GAMEOBJECT
    private List<float> _angles;
    [SerializeField] private GameObject _room;
    private BoxCollider _roomBox;

    [SerializeField] private RoomEvent _roomUpdated;
    [SerializeField] private WallEvent _wallProtectionUpdated;

    private void OnEnable()
    {
        _roomUpdated.RegisterListener(this);
    }

    private void OnDisable()
    {
        _roomUpdated.UnregisterListener(this);
    }

    //START FUNCTION
    private void Start()
    {
        if (_room == null)
            _room = new GameObject("SingleRoom");
        _room.layer = LayerMask.NameToLayer("Room");

        _roomBox = _room.GetComponent<BoxCollider>();
        if (_roomBox == null)
            _roomBox = _room.AddComponent<BoxCollider>();

        _width = 3.5f;
        _area = 21;
        _length = _area / _width;

        _floorManager.InitFloor(_room, _width, _length);

        _angles = new List<float>() { 90.0f, 90.0f, 90.0f, 90.0f };

        _wallManager.InitWalls(_room, _numberOfWalls, _width, _length, _angles);

        _ceilingManager.InitCeiling(_room, _width, _length, GetWallHeight(), GetWallThickness(), _falseCeilingHeight);

        _furnitureManager.InitFurnitures(_room);
        List<FurnitureRef> furnitureRefs = _furnitureManager.GetFurnituresRefs();
        foreach(FurnitureRef fRef in furnitureRefs)
        {
            if (!fRef.IsMobile)
                _roomUpdated.RaiseObstacleAdded(fRef.gameObject);
        }

        _doorManager.InitEntryDoor(_room);

        _roomUpdated.Raise(_width, _length, _area, GetWallHeight());
        UpdateRoom();
    }

    //TRANSFORM FUNCTION ------------------------------------------------

    #region WALLS
    void UpdateWalls()
    {
        _wallManager.UpdateWalls(_width, _length, _angles);
        _wallManager.AdjustWalls(_width, _bathroomManager, _doorManager, _windowManager);
    }

    public void MakeWallProtected(int id, bool isProtected)
    {
        Debug.Log("MakeWallProtected");
        _wallManager.SetWallProtected(id, isProtected);
        UpdateRoom();
    }

    public void MakeProtectionFloorTexture(bool p_active)
    {
        _wallManager.SetWallProtectionFloorExtension(p_active);
        if (p_active)
            _wallManager.UpdateProtectionsMaterial(_floorManager.GetFloorMaterial());
    }

    public bool[] GetIsWallProtected()
    {
        return _wallManager.GetWallsProtected();
    }

    public string[] GetWallMaterialsString()
    {
        return _wallManager.GetWallMaterialsAsStringArray();
    }

    public string[] GetWallProtectionMaterialsString()
    {
        return _wallManager.GetWallProtectionMaterialsAsStringArray();
    }

    public bool IsWallProtectionFloorExtension => _wallManager.IsWallProtectionFloorExtension;

    public void UpdateWallProtectionsMaterial(Material p_material)
    {
        _wallManager.UpdateProtectionsMaterial(p_material);
    }

    public void ReinitWallsTransparency()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<Wall>().UnsetTransparent();
        }
    }
    #endregion


    #region BATHROOM
    void UpdateBathroom()
    {
        Debug.Log("UpdateBathRoom");
        _bathroomManager.UpdateBathroom(_width, _length, GetWallHeight());
    }

    public void ChangeBathroomFloorMaterial(Material p_material)
    {
        _bathroomManager.ChangeBathroomFloorMaterial(p_material);
    }

    #endregion


    #region FLOOR
    public void ChangeFloorMaterial(Material p_material)
    {
        _floorManager.ChangeFloorMaterial(p_material);
    }

    void UpdateFloor()
    {
        _floorManager.UpdateFloor(_width, _length);
    }

    public string GetFloorMaterialString()
    {
        Material tmpMat = _floorManager.GetFloorMaterial();
        return tmpMat.name.Substring(0, tmpMat.name.Length);
    }
    #endregion


    #region CEILING
    void UpdateCeiling()
    {
        _ceilingManager.UpdateCeiling(_width, _length, GetWallHeight(), GetWallThickness(), _falseCeilingHeight, _bathroomManager.GetBathroomBounds(), _bathroomManager.GetBathroomExtentOutside());
    }
    #endregion


    #region FURNITURES
    public void SetCurrentFurniture(GameObject p_furnitureObject)
    {
        _furnitureManager.SetCurrentFurniture(p_furnitureObject);
    }

    public GameObject GetCurrentFurniture()
    {
        return _furnitureManager.GetCurrentFurniture();
    }

    public void InitFurniture(GameObject p_furniturePrefab)
    {
        _furnitureManager.InitFurniture(p_furniturePrefab, _width, _length);
    }

    public void RemoveFurniture()
    {
        _furnitureManager.RemoveFurniture(_furnitureManager.GetCurrentFurniture());
    }
    #endregion


    #region DOOR
    void UpdateDoor()
    {
        _doorManager.UpdateDoor(_width, _length, (_wallManager.GetPostDimensions().x - _wallManager.GetWallThickness()) / 2f);
    }
    #endregion


    //----------------------------------------------------------------------------------------
    public void UpdateRoomDimensions(float p_roomWidthToAdd, float p_roomAreaToAdd)
    {
        Debug.Log($"RoomManager - UpdateRoomDimensions {p_roomWidthToAdd}-{p_roomAreaToAdd}");
        float newWidth = _width + p_roomWidthToAdd;
        float newArea = _area + p_roomAreaToAdd;
        float newLength = newArea / newWidth;

        if (newWidth >= _widthMin && newWidth <= _widthMax &&
            newLength >= _lengthMin &&
            newArea >= _areaLimits.x && newArea <= _areaLimits.y)
        {
            _width += p_roomWidthToAdd;
            _area += p_roomAreaToAdd;
            _length = _area / _width;
            _roomUpdated.Raise(_width, _length, _area, GetWallHeight());
            UpdateRoom();
        }
    }

    void UpdateRoomBounds()
    {
        _roomBox.center = new Vector3(0, _wallManager.GetWallHeight() / 2f, 0);
        _roomBox.size = new Vector3(_width, _wallManager.GetWallHeight(), _length);
    }

    public void UpdateWallDimensions(float p_wallHeightToAdd, float p_wallThicknessToAdd, float p_wallProtectionHeigthToAdd)
    {
        Debug.Log($"RoomManager - UpdateWallDimensions {p_wallHeightToAdd}-{p_wallThicknessToAdd}-{p_wallProtectionHeigthToAdd}");
        _wallManager.UpdateDimensions(p_wallHeightToAdd, p_wallThicknessToAdd, p_wallProtectionHeigthToAdd);
        _roomUpdated.Raise(_width, _length, _area, GetWallHeight());
        UpdateRoom();
    }

    private void UpdateRoom()
    {
        Debug.Log("UpdateRoom");
        UpdateFloor();
        UpdateCeiling();
        UpdateBathroom();
        UpdateDoor();
        UpdateWalls();
        UpdateRoomBounds();
    }


    #region DATA
    public void LoadData(float width, float area, float p_wallHeight, float p_wallThickness, float p_wallProtectionHeight, float p_wallProtectionThickness)
    {
        UpdateRoomDimensions(width - GetWidth(), area - GetArea());
        UpdateWallDimensions(p_wallHeight - GetWallHeight(), p_wallThickness - GetWallThickness(), p_wallProtectionHeight - GetWallHeightProtection());

        //Config manager now
        /*_wallManager.SetWallsProtected(isProtected);
        
        //ChangeBathroom(idBathroom);
        Material l_floorMaterial = Resources.Load<Material>(LoadConfig.FindPath(1, floorMaterial));
        _floorManager.ChangeFloorMaterial(l_floorMaterial);

        List<Material> l_wallMaterials = new List<Material>();
        List<Material> l_wallProtectionMaterials = new List<Material>();
        for (int i = 0; i < _numberOfWalls; i++)
        {
            Material tmp = Resources.Load<Material>(LoadConfig.FindPath(0, wallsMaterial[i]));
            l_wallMaterials.Add(tmp);
            if (!wallsProtectedMaterialAsFloor)
            {
                string path = LoadConfig.FindPath(0, wallsProtectedMaterial[i]);
                if (path == "")
                {
                    path = LoadConfig.FindPath(1, wallsProtectedMaterial[i]);
                }
                tmp = Resources.Load<Material>(path);
                l_wallProtectionMaterials.Add(tmp);
            }
        }
        _wallManager.UpdateMaterials(l_wallMaterials, l_wallProtectionMaterials, wallsProtectedMaterialAsFloor);
        MakeProtectionFloorTexture(wallsProtectedMaterialAsFloor);*/

        UpdateRoom();
    }

    public void LoadFurnituresData(List<string> names, List<Vector3> pos, List<Vector3> rot)
    {
        _furnitureManager.LoadData(names, pos, rot, _width, _length);
    }
    #endregion


    //----------------------------------------------------------------------------------
    public float GetWidth() { return _width; }
    public float GetLength() { return _length; }
    public float GetArea() { return _area; }
    public float GetWallHeight() { return _wallManager.GetWallHeight(); }
    public float GetWallHeightProtection() { return _wallManager.GetWallProtectionHeight(); }
    public float GetWallThickness() { return _wallManager.GetWallThickness(); }

    public float GetWallThicknessProtection() { return _wallManager.GetWallProtectionThickness(); }

    public BoxCollider GetRoomBounds() { return _roomBox; }

    public List<FurnitureRef> GetFurnituresRefs() { return _furnitureManager.GetFurnituresRefs(); }

    void IRoomEventListener.OnRoomEventRaised(float p_width, float p_length, float p_area, float p_height)
    {
        //throw new System.NotImplementedException();
    }

    void IRoomEventListener.OnRoomEventBathroomUpdatedRaised(GameObject p_bathroom)
    {
        _bathroomManager.SetBathroom(p_bathroom);
        UpdateRoom();
    }

    void IRoomEventListener.OnRoomEventWindowUpdatedRaised()
    {
        UpdateRoom();
    }


    void IRoomEventListener.OnRoomEventObstacleAddedRaised(GameObject p_object)
    {
        //throw new System.NotImplementedException();
    }

    void IRoomEventListener.OnRoomEventObstacleRemovedRaised(GameObject p_object)
    {
        //throw new System.NotImplementedException();
    }
}
