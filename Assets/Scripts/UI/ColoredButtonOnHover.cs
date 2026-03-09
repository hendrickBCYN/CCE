using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Thiss class manages hover behavior for an UI button
public class ColoredButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _text;

    [SerializeField] private GameObject _imageOnHover;
    [SerializeField] private GameObject _imageOffHover;

    private Color _orange = new Color32(0xE7, 0x51, 0x13, 0xFF);
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ChangeTextColor(_orange);
        ActivateImageOnHover();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ChangeTextColor(Color.white);
        ActivateImageOffHover();
    }

    private void ChangeTextColor(Color p_color)
    {
        if (_text != null)
        {
            _text.color = p_color;
        }
    }

    private void ActivateImageOnHover()
    {
        if (_imageOffHover != null && _imageOnHover != null)
        {
            _imageOnHover.SetActive(true);
            _imageOffHover.SetActive(false);
        }

    }

    private void ActivateImageOffHover()
    {
        if (_imageOffHover != null && _imageOnHover != null)
        {
            _imageOnHover.SetActive(false);
            _imageOffHover.SetActive(true);
        }
    }
}
