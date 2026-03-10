using System;
using System.Collections.Generic;
// using System.IO;
using UnityEngine;

public class SaveConfig : MonoBehaviour
{
    private Config config;

    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private FurnitureManager _furnitureManager;
    [SerializeField] private RoomManager _roomManager;
    [SerializeField] private PMRManager _pmrManager;

    /// <summary>
    /// Save data into a Json file
    /// </summary>
    public void SaveToJson()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // En WebGL: on délègue la sauvegarde à React via le NetworkManager
        string configData = BuildConfigJson();
        NetworkManager.Instance.RequestSave(configData);
#else
        // En éditeur: on garde le comportement fichier local
        string l_folderPath = FileDialogManager.OpenFolder();
        if (l_folderPath != null)
        {
            try
            {
                WriteData(l_folderPath + "/" + "ehpad_config.json");
                Debug.Log($"SaveConfig - SaveToJson() - folderPath : {l_folderPath}");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }
#endif
    }

    // Construit le JSON de configuration sans écrire de fichier.
    // Utilisé par le mode WebGL pour envoyer les données à React.
    private string BuildConfigJson()
    {
        RoomSave l_room = new(
            _roomManager.GetWidth(),
            _roomManager.GetArea(),
            _roomManager.GetWallHeight(),
            _roomManager.GetWallThickness(),
            _roomManager.GetWallHeightProtection(),
            _roomManager.GetWallThicknessProtection()
        );
        FurnitureSave furniture = new(_roomManager.GetFurnituresRefs());
        CamSave cam = new(
            _cameraManager.GetCurrentCameraType(),
            _cameraManager.GetCamerasType(),
            _cameraManager.GetCamerasTransformVector3(),
            _cameraManager.GetFieldOfView(),
            _cameraManager.GetDistToTarget()
        );
        FinishSave finish = new(
            _roomManager.GetWallMaterialsString(),
            _roomManager.GetFloorMaterialString(),
            _roomManager.GetWallProtectionMaterialsString(),
            _roomManager.IsWallProtectionFloorExtension
        );
        PMRSave pmr = new(
            _pmrManager.GetAllObjectsNames(),
            _pmrManager.GetAllObjectsPos(),
            _pmrManager.GetAllObjectsRot()
        );

        string userName = "WebGL_User";
        Config configObj = new Config(l_room, cam, finish, furniture, pmr, userName);
        return JsonUtility.ToJson(configObj);
    }

    void WriteData(string path)
    {
        RoomSave l_room = new(
            _roomManager.GetWidth(),
            _roomManager.GetArea(),
            _roomManager.GetWallHeight(),
            _roomManager.GetWallThickness(),
            _roomManager.GetWallHeightProtection(),
            _roomManager.GetWallThicknessProtection()
        );
        FurnitureSave furniture = new(_roomManager.GetFurnituresRefs());
        CamSave cam = new(
            _cameraManager.GetCurrentCameraType(),
            _cameraManager.GetCamerasType(),
            _cameraManager.GetCamerasTransformVector3(),
            _cameraManager.GetFieldOfView(),
            _cameraManager.GetDistToTarget()
        );
        FinishSave finish = new(
            _roomManager.GetWallMaterialsString(),
            _roomManager.GetFloorMaterialString(),
            _roomManager.GetWallProtectionMaterialsString(),
            _roomManager.IsWallProtectionFloorExtension
        );
        PMRSave pmr = new(
            _pmrManager.GetAllObjectsNames(),
            _pmrManager.GetAllObjectsPos(),
            _pmrManager.GetAllObjectsRot()
        );

        string userName = Environment.UserName;
        config = new Config(l_room, cam, finish, furniture, pmr, userName);
        string configData = JsonUtility.ToJson(config);
        System.IO.File.WriteAllText(path, configData);
        config = null;
    }

    /// <summary>
    /// Load data from a Json file
    /// Charge la configuration.
    /// En WebGL → demande à React de charger depuis l'API.
    /// En Standalone → lit un fichier local.
    /// </summary>
    public void LoadFromJson()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    // En WebGL, on demande à React de charger depuis l'API
    NetworkManager.Instance.RequestLoad("latest");
#else
        string l_folderPath = FileDialogManager.OpenFolder();
        if (l_folderPath != null)
        {
            try
            {
                LoadData(l_folderPath + "/" + "ehpad_config.json");
                Debug.Log($"SaveConfig - LoadFromJson() - folderPath : {l_folderPath}");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }
#endif
    }

    // Appelé par le NetworkManager quand React envoie une configuration chargée.
    public void LoadedFromJson(string configJson)
    {
        config = JsonUtility.FromJson<Config>(configJson);
        ApplyConfig();
        config = null;
    }

    void LoadData(string path)
    {
        string configData = System.IO.File.ReadAllText(path);
        config = JsonUtility.FromJson<Config>(configData);
        ApplyConfig();
        config = null;
    }

    void ApplyConfig()
    {
        _roomManager.LoadData(
            config.room.width,
            config.room.area,
            config.room.wallHeight,
            config.room.wallThickness,
            config.room.wallProtectionHeight,
            config.room.wallProtectionThickness
        );
        _roomManager.LoadFurnituresData(
            config.furniture.refname,
            config.furniture.refposition,
            config.furniture.refrotation
        );
        _cameraManager.LoadData(
            config.cam.currentTypes,
            config.cam.type,
            config.cam.transforms,
            config.cam.fovs,
            config.cam.dtts
        );
        _pmrManager.LoadData(
            config.pmr.name,
            config.pmr.position,
            config.pmr.rotation
        );
    }
}

/// <summary>
/// Config class is the save of the different value usefull for the configurator
/// </summary>
[System.Serializable]
public class Config
{
    public RoomSave room;
    public CamSave cam;
    public FinishSave finish;
    public FurnitureSave furniture;
    public PMRSave pmr;
    public string author;

    public Config(RoomSave p_roomSave, CamSave p_cam, FinishSave p_finish, FurnitureSave p_furniture, PMRSave p_pmr, string p_author)
    {
        room = p_roomSave;
        cam = p_cam;
        finish = p_finish;
        furniture = p_furniture;
        pmr = p_pmr;
        author = p_author;
    }
}

[System.Serializable]
public class RoomSave
{
    public float width;
    public float area;
    public float wallHeight;
    public float wallThickness;
    public float wallProtectionHeight;
    public float wallProtectionThickness;

    public RoomSave(float p_width, float p_area, float p_wallHeight, float p_wallThickness, float p_wallProtectionHeight, float p_wallProtectionThickness)
    {
        width = p_width;
        area = p_area;
        wallHeight = p_wallHeight;
        wallThickness = p_wallThickness;
        wallProtectionHeight = p_wallProtectionHeight;
        wallProtectionThickness = p_wallProtectionThickness;
    }
}

[System.Serializable]
public class CamSave
{
    public List<int> currentTypes;
    public List<int> type;
    public List<Vector3> transforms;
    public List<float> fovs;
    public List<float> dtts;

    public CamSave(List<int> p_currentTypes, List<int> p_type, List<Vector3> p_transforms, List<float> p_fovs, List<float> p_dtts)
    {
        currentTypes = p_currentTypes;
        type = p_type;
        transforms = p_transforms;
        fovs = p_fovs;
        dtts = p_dtts;
    }
}

[System.Serializable]
public class FinishSave
{
    public string[] wallMaterials;
    public string floorMaterial;
    public string[] wallProtectedMaterials;
    public bool wallProtectedMaterialAsFloor;

    public FinishSave(string[] p_wallMaterials, string p_floorMaterial, string[] p_wallProtectedMaterials, bool p_wallProtectedMaterialsAsFloor)
    {
        wallMaterials = p_wallMaterials;
        floorMaterial = p_floorMaterial;
        wallProtectedMaterials = p_wallProtectedMaterials;
        wallProtectedMaterialAsFloor = p_wallProtectedMaterialsAsFloor;
    }

}

[System.Serializable]
public class FurnitureSave
{
    public List<string> refname;
    public List<Vector3> refposition;
    public List<Vector3> refrotation;

    public FurnitureSave(List<FurnitureRef> p_furnituresrefs)
    {
        refname = new List<string>();
        refposition = new List<Vector3>();
        refrotation = new List<Vector3>();
        foreach (FurnitureRef l_ref in p_furnituresrefs)
        {
            refname.Add(l_ref.name);
            refposition.Add(l_ref.transform.localPosition);
            refrotation.Add(l_ref.transform.localRotation.eulerAngles);
        }
    }
}

[System.Serializable]
public class PMRSave
{
    public List<string> name;
    public List<Vector3> position;
    public List<Vector3> rotation;

    public PMRSave(List<string> p_name, List<Vector3> p_position, List<Vector3> p_rotation)
    {
        name = p_name;
        position = p_position;
        rotation = p_rotation;
    }
}