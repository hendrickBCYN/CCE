using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubpanelItem : MonoBehaviour
{

    private PanelItem _panel;

    public PanelItem Panel => _panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPanel(PanelItem p_panel)
    {
        _panel = p_panel;
    }

    public void ForceRebuildLayoutItems(Transform t)
    {
        RectTransform[] rects = t.GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform r in rects)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(r);
        }
    }
}
