using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] GameObject _prefabWindow;
    private GameObject _window;
    private Vector3 _dimensions = Vector3.zero;
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [SerializeField] private Anchor _mextAnchor;

    [SerializeField] RoomEvent _roomEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWindow(GameObject p_object)
    {
        _window = p_object;
        if(_window.GetComponent<Mext>() != null)
        {
            _offset = Vector3.up * _window.GetComponent<Mext>().GetBreastHeight();
        }
        Vector3 l_bbSize = _window.GetComponent<BoxCollider>().size;
        _dimensions = new Vector3(_window.transform.localScale.x * l_bbSize.x, _window.transform.localScale.y * l_bbSize.y, _window.transform.localScale.z * l_bbSize.z);

        if(_roomEvent != null)
            _roomEvent.RaiseWindowUpdated();
    }

    public Vector3 GetWindowDimensions()
    {
        return _dimensions;
    }

    public Vector3 GetWindowOffset() {  return _offset; }
}
