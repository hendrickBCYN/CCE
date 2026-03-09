using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ColoredUIItem _mainItem;
    [SerializeField] ColoredUIItem _idItem;

    [SerializeField] SubmenuItem _submenuItem;
    [SerializeField] bool _isSelected;

    [SerializeField] UIManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectItem(bool p_selected)
    {
        if(p_selected)
        {
            _mainItem.SetColors(UIManager.UIMenuItemBackgroundColorSelected, UIManager.UIMenuItemBorderColorSelected, UIManager.UIMenuItemTextColorSelected);
            _idItem.SetColors(UIManager.White, UIManager.DarkBlue, UIManager.Black);
            SelectSubmenuItem(true);
        }
        else
        {
            _mainItem.SetColors(UIManager.UIMenuItemBackgroundColor, UIManager.UIMenuItemBorderColor, UIManager.UIMenuItemTextColor);
            _idItem.SetColors(UIManager.Orange, UIManager.White, UIManager.White);
            SelectSubmenuItem(false);
        }
        SetSelelected(p_selected);
    }

    public void SelectSubmenuItem(bool p_selected)
    {
        if(_submenuItem != null)
        {
            _submenuItem.ResetColoredUIItems();
            _submenuItem.gameObject.SetActive(p_selected);
        }
    }

    public void HighlightItem(bool p_highlighted)
    {
        if (_isSelected)
            return;
        if (p_highlighted)
        {
            _mainItem.SetColors(UIManager.UIMenuItemBackgroundColorHover, UIManager.UIMenuItemBorderColorHover, UIManager.UIMenuItemTextColorHover);
            _idItem.SetColors(UIManager.Orange, UIManager.Black, UIManager.White);
        }
        else
        {
            _mainItem.SetColors(UIManager.UIMenuItemBackgroundColor, UIManager.UIMenuItemBorderColor, UIManager.UIMenuItemTextColor);
            _idItem.SetColors(UIManager.Orange, UIManager.White, UIManager.White);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightItem(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_isSelected)
            HighlightItem(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.selectedObject == gameObject)
        {
            _uiManager.SelectMenuItem(this);
            //if (!_isSelected)
            //    _uiManager.SelectMenuItem(this);
            //else
            //    _uiManager.SelectMenuItem(null);
        }
    }

    public void SetSelelected(bool p_selected) { _isSelected = p_selected; }
}
