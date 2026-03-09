using UnityEngine;

public class ToggleButtonManager : MonoBehaviour
{
    [SerializeField] private ToggleButton[] _toggleButtons;

    private void Start()
    {
        if (_toggleButtons == null || _toggleButtons.Length == 0)
        {
            Debug.LogError("ToggleButtonManager - _toggleButtons is null or empty");
            return;
        }

        foreach (ToggleButton toggleButton in _toggleButtons)
        {
            toggleButton.Initialize(this);
        }
    }

    // When a button is clicked to close other panels
    public void OnToggleButtonClicked(ToggleButton clickedButton)
    {
        foreach (ToggleButton toggleButton in _toggleButtons)
        {
            if (toggleButton != clickedButton)
            {
                toggleButton.ClosePanel();
            }
        }
    }
}

