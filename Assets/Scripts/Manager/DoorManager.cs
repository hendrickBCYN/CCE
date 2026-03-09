using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorManager : MonoBehaviour
{
    [SerializeField] GameObject _prefabDoor;
    private GameObject _entryDoor;
    private Vector3 _dimensions = Vector3.zero;
    [SerializeField] private Vector3 _offset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitEntryDoor(GameObject p_roomParent)
    {
        //Instantiate entry door
        _entryDoor = Instantiate(_prefabDoor, Vector3.zero, Quaternion.identity, p_roomParent.transform);
        _entryDoor.name = "PorteEntree";
        _entryDoor.transform.localScale = new Vector3(1.2f, 1f, 1f);
        Vector3 l_bbSize = _entryDoor.GetComponent<AnimatedPart>().GetAnimatedObjectDimensions();

        _dimensions = new Vector3(_entryDoor.transform.localScale.x * l_bbSize.x, _entryDoor.transform.localScale.y * l_bbSize.y, _entryDoor.transform.localScale.z * l_bbSize.z);
    }

    public void UpdateDoor(float p_roomWidht, float p_roomLength, float p_postOffset)
    {
        if (_entryDoor != null)
            _entryDoor.transform.localPosition = new Vector3(p_roomWidht / 2f - p_postOffset, 0, (p_roomLength + _dimensions.z) / 2f) + _offset;
    }

    public Vector3 GetEntryDoorDimensions() { return _dimensions; }

    public Vector3 GetEntryDoorOffset() { return _offset; }
}
