using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Manage the state of a button (arrows included) and an associated panel
public class ToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ToggleButtonManager _manager;

    [SerializeField] private Sprite _arrowDown;
    [SerializeField] private Sprite _arrowUp;
    [SerializeField] private Sprite _arrowDownColored;
    [SerializeField] private Sprite _arrowUpColored;
    [SerializeField] private Image _arrowImage;

    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _parentToRefresh;

    private bool _isOpen = false;
    private bool _activePanel = false;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log($"Cursor Entering {name} + GameObject");

        if (_isOpen)
        {
            _arrowImage.sprite = _arrowUpColored;
        }
        else
        {
            _arrowImage.sprite = _arrowDownColored;
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Debug.Log($"Cursor Exiting {name} + GameObject");

        if (_isOpen)
        {
            _arrowImage.sprite = _arrowUp;
        }
        else
        {
            _arrowImage.sprite = _arrowDown;
        }
    }

    public void Initialize(ToggleButtonManager manager)
    {
        _manager = manager;
    }

    public void TogglePanel()
    {
        if (_panel != null)
        {
            _panel.SetActive(!_panel.activeSelf);
            _isOpen = _panel.activeSelf;

            // Update the arrow based on the panel state
            UpdateArrow();

            // Notify the manager about the change
            _manager.OnToggleButtonClicked(this);
        }
        if(transform.parent != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        if (_parentToRefresh != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_parentToRefresh.GetComponent<RectTransform>());
        }
    }

    public void ClosePanel()
    {
        if (_panel != null)
        {
            _panel.SetActive(false);
            _isOpen = false;
            UpdateArrow();
        }
    }

    // Update the arrow image based on panel state
    private void UpdateArrow()
    {
        if (_isOpen)
        {
            _arrowImage.sprite = _arrowUp;
        }
        else
        {
            _arrowImage.sprite = _arrowDown;
        }
    }
}