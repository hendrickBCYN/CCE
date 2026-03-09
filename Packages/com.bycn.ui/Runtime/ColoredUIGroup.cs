using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredUIGroup : MonoBehaviour
{
    [SerializeField] ColoredUIItem[] _items;
    [SerializeField] string _groupName = string.Empty;
    [SerializeField] int _defaultOption = 0;
    int _currentOption = -1;

    // Start is called before the first frame update
    void Awake()
    {
        InitOptions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        InitOptions();
    }

    public void InitOptions()
    {
        if(_items == null || _items.Length == 0)
            _items = GetComponentsInChildren<ColoredUIItem>(true);

        if (_items.Length > 0)
        {
            if(_currentOption >= 0 && _currentOption < _items.Length)
            {
                SelectItem(_items[_currentOption]);
            }
            else{
                if(_defaultOption >= 0 && _defaultOption < _items.Length)
                    SelectItem(_items[_defaultOption]);
                else SelectItem(_items[0]);
            }
        }
    }

    public void SelectItem(ColoredUIItem p_item)
    {
        for(int i = 0; i < _items.Length; ++i)
        {
            _items[i].SetSelected(_items[i] == p_item);
            if (_items[i] == p_item)
                _currentOption = i;
        }
    }

    public void SelectItem(int p_itemIndex)
    {
        if(p_itemIndex >= 0 &&  p_itemIndex < _items.Length)
            SelectItem(_items[p_itemIndex]);
    }
}
