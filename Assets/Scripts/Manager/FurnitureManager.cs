using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FurnitureManager : MonoBehaviour, IFurnitureEventListener
{
    private GameObject[] _furnituresPrefab;
    [SerializeField] GameObject _pmrZonePrefab;
    [SerializeField] private List<FurnitureRef> _furnituresRefs;
    [SerializeField] private List<GameObject> _furnitures;

    private FurnitureManager Instance; //Singleton

    private GameObject _furnitureRoot = null;
    private GameObject _currentFurniture = null;

    [SerializeField] private FurnitureEvent _furnitureEvent;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        _furnitures = new List<GameObject>();
    }

    public void Start()
    {
        
    }

    private void OnEnable()
    {
        if(_furnitureEvent != null)
            _furnitureEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if(_furnitureEvent != null)
            _furnitureEvent.UnregisterListener(this);
    }

    public void InitFurnitures(GameObject p_room)
    {
        _furnitureRoot = p_room;
        _furnituresRefs = new List<FurnitureRef>(_furnitureRoot.GetComponentsInChildren<FurnitureRef>());
    }

    public void InitFurniture(GameObject prefab, float p_roomWidth, float p_roomLength)
    {
        GameObject go = Instantiate(prefab, _furnitureRoot.transform);
        if(go.GetComponent<PMRCheck>() != null)
        {
            go.GetComponent<PMRCheck>().InitPMRZones(_pmrZonePrefab);
        }
        _furnitures.Add(go);
        _currentFurniture = go;
    }

    public void RemoveFurniture(GameObject go)
    {
        foreach(GameObject g in _furnitures)
        {
            if (g == go)
            {
                Destroy(g);
                _furnitures.Remove(g);
                return;
            }
        }
        _currentFurniture = null;
    }

    public void LoadData(List<string> names, List<Vector3> pos, List<Vector3> rot, float roomWidth, float roomLength)
    {
        Reset();

        for (int i = 0; i < names.Count; i++)
        {
            foreach (FurnitureRef l_ref in _furnituresRefs)
                if (l_ref.name == names[i])
                {
                    l_ref.transform.localPosition = pos[i];
                    l_ref.transform.localRotation = Quaternion.Euler(rot[i]);
                    if (l_ref.gameObject.GetComponent<Anchor>() != null)
                        l_ref.gameObject.GetComponent<Anchor>().UpdatePositionOffset();
                    break;
                }
            /*GameObject l_furnitureObject = Resources.Load<GameObject>(LoadConfig.FindPath(2, names[i]));
            InitFurniture(l_furnitureObject, roomWidth, roomLength);
            _currentFurniture.transform.position = pos[i];
            _currentFurniture.transform.rotation = Quaternion.Euler(rot[i]);*/
        }
    }

    public void Reset()
    {
        if( _furnitures != null )
        {
            foreach (GameObject go in _furnitures)
            {
                Destroy(go);
            }

            _furnitures = new List<GameObject>();
        }
    }

    public void InstantiateFurniture(GameObject prefab, Vector3 pos, Vector3 rot)
    {
        GameObject go = Instantiate(prefab);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(rot);
        _furnitures.Add(go);
    }

    public List<string> GetNameFurniture()
    {
        List<string> names = new List<string>();

        foreach (GameObject furns in _furnitures)
        {
            names.Add(furns.name.Replace("(Clone)", ""));
        }

        return names;
    }

    public List<Vector3> GetPosFurniture()
    {
        List<Vector3> pos = new List<Vector3>();

        foreach (GameObject furns in _furnitures)
        {
            pos.Add(furns.transform.position);
        }

        return pos;
    }

    public List<Vector3> GetRotFurniture()
    {
        List<Vector3> rot = new List<Vector3>();

        foreach (GameObject furns in _furnitures)
        {
            rot.Add(furns.transform.rotation.eulerAngles);
        }

        return rot;

    }

    public GameObject GetCurrentFurniture() => _currentFurniture;

    public void SetCurrentFurniture(GameObject p_furniture)
    {
        foreach (GameObject l_go in _furnitures)
            if (l_go == p_furniture)
            {
                _currentFurniture = l_go;
                return;
            }
    }

    void IFurnitureEventListener.OnFurnitureAddedRaised(GameObject p_furniture)
    {
        if(p_furniture.GetComponent<PMRCheck>() != null)
        {
            p_furniture.GetComponent<PMRCheck>().InitPMRZones(_pmrZonePrefab);
            _furnitures.Add(p_furniture);
        }
    }

    void IFurnitureEventListener.OnFurnitureRemovedRaised(GameObject p_furniture)
    {
        _furnitures.Remove(p_furniture);
    }

    public List<FurnitureRef> GetFurnituresRefs() { return _furnituresRefs; }
}
