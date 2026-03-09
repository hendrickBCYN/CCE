using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmenuItem : MonoBehaviour
{
    [SerializeField] ColoredUIItem[] _coloredUIItems;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Image>() != null)
            GetComponent<Image>().color = UIManager.MainThemeColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        
    }

    public void ResetColoredUIItems()
    {
        foreach (var item in _coloredUIItems)
            item.SetSelected(false);
    }
}
