using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private ColoredUIItem _mainMenuItem;
    [SerializeField] private MenuItem [] _menuItems;

    [SerializeField] private GameObject _RootPanels;
    [SerializeField] private PanelItem[] _panelItems;

    [SerializeField] private Color _orange;
    [SerializeField] private Color _green;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _darkBlue;
    [SerializeField] private Color _black;
    [SerializeField] private Color _white;
    [SerializeField] private Color _gray;

    static public UIManager Instance { get; private set; }
    static public Color Orange => Instance._orange;
    static public Color Green => Instance._green;
    static public Color Gray => Instance._gray;
    static public Color White => Instance._white;
    static public Color Black => Instance._black;

    static public Color Blue => Instance._blue;

    static public Color DarkBlue => Instance._darkBlue;

    static public Color MainThemeColor => DarkBlue;
    static public Color MainThemeTextColor => White;

    static public Color UIMenuItemBackgroundColor => DarkBlue;
    static public Color UIMenuItemBackgroundColorHover => White;
    static public Color UIMenuItemBackgroundColorSelected => Orange;

    static public Color UIMenuItemTextColor => White;
    static public Color UIMenuItemTextColorHover => Black;
    static public Color UIMenuItemTextColorSelected => White;

    static public Color UIMenuItemBorderColor => White;
    static public Color UIMenuItemBorderColorHover => Black;
    static public Color UIMenuItemBorderColorSelected => White;

    [SerializeField] protected PanelEvent _panelEvent;

    private void Awake()
    {
        Instance = this;
        InitMenu();
        InitPanels();
    }
    // Start is called before the first frame update
    void Start()
    {
        SelectMenuItem(null);
        ClosePanels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectMenuItem(MenuItem p_item)
    {
        //ClosePanels();

        foreach(MenuItem mitem in _menuItems)
        {
            bool selected = mitem == p_item;
            mitem.SelectItem(selected);
        }
    }

    public void UnselectMenuItems()
    {
        foreach (MenuItem mitem in _menuItems)
        {
            mitem.SelectItem(false);
        }
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    { 

    }

    public void EmptyClick()
    {
        if (_RootPanels != null)
        {
            UnselectMenuItems();
        }
    }
   
    void InitMenu()
    {
        if (_mainMenuItem != null)
            _mainMenuItem.SetColors(MainThemeColor, MainThemeColor, MainThemeTextColor);
    }
        
    void InitPanels()
    {
        if (_RootPanels != null && _RootPanels.GetComponent<Image>() != null)
            _RootPanels.GetComponent<Image>().color = MainThemeColor;
        foreach(PanelItem pitem in _panelItems)
        {
            pitem.InitPanel();
        }
    }

    public void SelectPanelItem(PanelItem p_panel)
    {
        OpenPanel();
        HidePanels();
        p_panel.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(p_panel.GetComponent<RectTransform>());
    }

    public void SelectSubpanelItem(SubpanelItem p_subpanel)
    {
        if(p_subpanel != null)
        {
            SelectPanelItem(p_subpanel.Panel);
            p_subpanel.Panel.SelectSubpanelItem(p_subpanel);
        }
    }

    public void OpenPanel()
    {
        if (_RootPanels != null)
        {
            _RootPanels.SetActive(true);
            _panelEvent.Raise(true, _RootPanels.GetComponent<RectTransform>().rect.width);
        }
    }

    public void ClosePanel()
    {
        if (_RootPanels != null)
        {
            _RootPanels.SetActive(false);
            _panelEvent.Raise(false, 0);
        }
    }

    public void ClosePanels()
    {
        HidePanels();
        ClosePanel();    
    }

    void HidePanels()
    {
        foreach(PanelItem item in _panelItems)
            item.gameObject.SetActive(false);
    }
}

