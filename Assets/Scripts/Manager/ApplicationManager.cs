// using System.Windows.Forms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private InputManager _controls;
    [SerializeField] private SaveConfig _config;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private RoomManager _roomManager;
    [SerializeField] private PMRManager _pmrManager;
    [SerializeField] private CommandInvoker _command;
    [SerializeField] private ManipulationManager _manipulationManager;
    [SerializeField] private UIManager _uIManager;
    [SerializeField] private ConfigurationManager _configurationManager;


    [SerializeField] private bool _isChangeWallColor;
    private bool _isChangeProtectionColor;
    private bool _isChangeFloorColor;
    private bool _isChangeBathroomFloorColor;
    private Material _material;
    private Material _floorMaterial;
    private Material _bathroomFloorMaterial;

    [SerializeField] private GameObject _camera;

    public enum ChoiceMode { Wall, WallProtection, Floor, Bathroom, Room};
    [SerializeField] private ChoiceMode _choiceMode = ChoiceMode.Room;

    private void Awake()
    {
        LoadConfig.Init();
    }

    void Start()
    {
        _isChangeWallColor = false;
        _isChangeFloorColor = false;
        _isChangeBathroomFloorColor = false;
        _isChangeProtectionColor = false;
        _choiceMode = ChoiceMode.Room;
    }

    private void Update()
    {
        if (_controls.LeftDoubleClick())
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _cameraManager.GetRay(mousePosition);

            RaycastHit[] hits;
            switch (_choiceMode)
            {
                case ChoiceMode.Room:
                    hits = Physics.RaycastAll(ray);
                    foreach (RaycastHit h in hits)
                    {
                        if (h.collider.gameObject.GetComponentInParent<AnimatedPart>() != null)
                        {
                            h.collider.gameObject.GetComponentInParent<AnimatedPart>().StartStopAnimation();
                        }
                    }
                    break;
            }
        }
    }

    public void UpdateWidth(float p_widthToAdd)
    {
        _roomManager.UpdateRoomDimensions(p_widthToAdd, 0);
        Debug.Log($"APPLICATION MANAGER - UpdateWidth() - p_widthToAdd = {p_widthToAdd}");
    }

    public void UpdateArea(float p_areaToAdd)
    {
        _roomManager.UpdateRoomDimensions(0, p_areaToAdd);
        Debug.Log($"APPLICATION MANAGER - UpdateArea() - p_areaToAdd = {p_areaToAdd}");
    }

    public void UpdateWallHeight(float p_heightToAdd)
    {
        _roomManager.UpdateWallDimensions(p_heightToAdd, 0, 0);
        Debug.Log($"APPLICATION MANAGER - UpdateWallHeight() - p_heightToAdd = {p_heightToAdd}");
        Debug.Log($"APPLICATION MANAGER - GetWallHeight() = {_roomManager.GetWallHeight()}");
    }

    public void UpdateWallHeightProtection(float p_heightToAdd)
    {
        _roomManager.UpdateWallDimensions(0, 0, p_heightToAdd);
    }

    public void ChooseColor(Material mat)
    {
        _material = mat;
        _isChangeWallColor = true;
        _choiceMode = ChoiceMode.Wall;
    }

    public void ChooseFloorTexture(Material mat)
    {
        _floorMaterial = mat;
        _isChangeFloorColor = true;
        _choiceMode = ChoiceMode.Floor;
        _roomManager.ChangeFloorMaterial(mat);
        if (_roomManager.IsWallProtectionFloorExtension)
            _roomManager.UpdateWallProtectionsMaterial(_floorMaterial);
        _choiceMode = ChoiceMode.Room;
    }

    public void ChooseBathroomFloorTexture(Material mat)
    {
        _bathroomFloorMaterial = mat;
        _isChangeBathroomFloorColor = true;
        _choiceMode = ChoiceMode.Bathroom;
        _roomManager.ChangeBathroomFloorMaterial(mat);
        _choiceMode = ChoiceMode.Room;
    }

    public void ChooseProtectionColor(Material mat)
    {
        _material = mat;
        _isChangeProtectionColor = true;
        _roomManager.MakeProtectionFloorTexture(false);
        _roomManager.UpdateWallProtectionsMaterial(mat);
    }

    public void MakeProtectionFloorTexture()
    {
        _roomManager.MakeProtectionFloorTexture(true);
    }

    public void SaveData()
    {
        bool l_save = false;
        _config.SaveToJson();

        if (_configurationManager != null)
            l_save = _configurationManager.SaveConfiguration();
        //if (_notificationManager != null)
        //    _notificationManager.DisplayNotification(l_save ? "Configuration sauv�e." : "Impossible de sauver la configuration.");


    }

    public void LoadData()
    {
        bool l_load = false;

        _config.LoadFromJson();
        //_command.Reset();
        if (_configurationManager != null)
            l_load = _configurationManager.LoadConfiguration();

        //if (_notificationManager != null)
        //    _notificationManager.DisplayNotification(l_load ? "Configuration charg�e." : "Impossible de charger la configuration.");
    }

    public void InitFurniture(GameObject prefab)
    {
        _roomManager.InitFurniture(prefab);
        _manipulationManager.SetCurrentActiveObject(_roomManager.GetCurrentFurniture());
    }

    public void RemoveFurniture()
    {
        _roomManager.RemoveFurniture();
    }

    #region PMR
    public void InitPMRObject(GameObject prefab)
    {
        _pmrManager.InitObject(prefab);
        _manipulationManager.SetCurrentActiveObject(_pmrManager.GetCurrentObject().gameObject);
    }
    public void RemovePMRObject()
    {
        _pmrManager.RemoveObject();
    }
    #endregion

    #region MANIPULATION
    public void RemoveCurrentObject()
    {
        if (_manipulationManager.CurrentActiveObject != null)
        {
            if (_manipulationManager.CurrentActiveObject.GetComponent<Furniture>() != null)
            {
                RemoveFurniture();
                _manipulationManager.Reset();
            }
            else if (_manipulationManager.CurrentActiveObject.GetComponent<PMRObject>() != null)
            {
                RemovePMRObject();
                _manipulationManager.Reset();
            }
        }
    }

    public void UpdateObjectRotation(float degree)
    {
        _manipulationManager.UpdateObjectTransform(Vector3.up * degree, Vector3.zero, Vector3.zero);
    }

    public void UpdateObjectUp(float up)
    {
        Vector3 l_forward = _cameraManager.GetCurrentForwardVector();
        _manipulationManager.UpdateObjectTransform(Vector3.zero, l_forward * up, Vector3.zero);
    }

    public void UpdateObjectLeft(float left)
    {
        Vector3 l_left = -_cameraManager.GetCurrentRightVector();
        _manipulationManager.UpdateObjectTransform(Vector3.zero, Vector3.zero, l_left * left);
    }
    #endregion

    public void QuitApplication()
    {
        UnityEngine.Application.Quit();
    }
}
