using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PMRManager : MonoBehaviour
{
    [SerializeField] bool _activeRules = false;
    [SerializeField] PMRZoneEvent _pmrEvent;
    List<PMRObject> _pmrObjects = new List<PMRObject>();


    PMRObject _currentObject = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitObject(GameObject p_prefab)
    {
        GameObject l_go = Instantiate(p_prefab);
        AddObject(l_go.GetComponent<PMRObject>());
        SetCurrentObject(l_go.GetComponent<PMRObject>());
    }

    public void AddObject(PMRObject p_object)
    {
        if(!_pmrObjects.Contains(p_object)) { 
            _pmrObjects.Add(p_object);
        }
    }

    public void RemoveObject()
    {
        if(_currentObject != null && _pmrObjects.Contains(_currentObject))
        {
            _pmrObjects.Remove(_currentObject);
            Destroy(_currentObject.gameObject);
        }
    }

    public void SetCurrentObject(PMRObject p_object)
    {
        _currentObject = p_object;
    }

    public PMRObject GetCurrentObject() => _currentObject;

    public List<string> GetAllObjectsNames()
    {
        List<string> names = new List<string>();

        foreach (PMRObject pmrobj in _pmrObjects)
        {
            names.Add(pmrobj.name.Replace("(Clone)", ""));
        }

        return names;
    }

    public List<Vector3> GetAllObjectsPos()
    {
        List<Vector3> pos = new List<Vector3>();

        foreach (PMRObject pmrobj in _pmrObjects)
        {
            pos.Add(pmrobj.transform.position);
        }

        return pos;
    }

    public List<Vector3> GetAllObjectsRot()
    {
        List<Vector3> rot = new List<Vector3>();

        foreach (PMRObject pmrobj in _pmrObjects)
        {
            rot.Add(pmrobj.transform.rotation.eulerAngles);
        }

        return rot;

    }

    public void LoadData(List<string> names, List<Vector3> pos, List<Vector3> rot)
    {
        Reset();

        for (int i = 0; i < names.Count; i++)
        {
            GameObject l_pmrObject = Resources.Load<GameObject>(LoadConfig.FindPath(2, names[i]));
            InitObject(l_pmrObject);
            _currentObject.transform.position = pos[i];
            _currentObject.transform.rotation = Quaternion.Euler(rot[i]);
        }
    }

    public void Reset()
    {
        if (_pmrObjects != null)
        {
            foreach (PMRObject go in _pmrObjects)
            {
                Destroy(go);
            }

            _pmrObjects = new List<PMRObject>();
        }
    }

    public void ActivateRules(bool p_enable)
    {
        _activeRules = p_enable;
        _pmrEvent.RaiseRulesActivated(_activeRules);
    }
}
