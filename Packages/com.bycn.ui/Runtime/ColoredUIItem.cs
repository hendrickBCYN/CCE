using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColoredUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _colorHover;
    [SerializeField] private Color _colorSelected;
    [SerializeField] private Color _colorBorder;
    [SerializeField] private Color _colorBorderHover;
    [SerializeField] private Color _colorBorderSelected;
    [SerializeField] private Color _colorText;
    [SerializeField] private Color _colorTextHover;
    [SerializeField] private Color _colorTextSelected;
    [SerializeField] Image _background;
    [SerializeField] Image _border;
    [SerializeField] TMPro.TextMeshProUGUI _text;
    [SerializeField] bool _hoverActions = false;

    public Color InitialColor => _color;
    public Color InitialTextColor => _colorText;

    private bool _isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_hoverActions)
            SetSelected(_isSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColors(Color p_background, Color p_border, Color p_text)
    {
        if(_background != null)
            _background.color = p_background;
        if(_border != null)
            _border.color = p_border;
        if(_text != null)
            _text.color = p_text;
    }

    public void SetSelected(bool p_selected)
    {
        _isSelected = p_selected;
        if (_isSelected)
            SetColors(_colorSelected, _colorBorderSelected, _colorTextSelected);
        else
            SetColors(_color, _colorBorder, _colorText);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverActions)
        {
            SetColors(_colorHover, _colorBorderHover, _colorTextHover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverActions)
        {
            SetSelected(_isSelected);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_hoverActions)
        {
            SetSelected(true);
        }
    }
}
