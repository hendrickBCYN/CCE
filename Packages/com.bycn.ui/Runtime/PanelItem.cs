using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelItem : MonoBehaviour
{
    [SerializeField] SubpanelItem [] _subpanelItems;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPanel()
    {
        foreach (var item in _subpanelItems)
        {
            item.SetPanel(this);
        }
    }

    public void SelectSubpanelItem(SubpanelItem p_subpanelItem)
    {
        foreach(SubpanelItem subpanelItem in _subpanelItems)
        {
            subpanelItem.gameObject.SetActive(subpanelItem == p_subpanelItem);
        }
    }
}
