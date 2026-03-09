using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class PlanFilter : MonoBehaviour
{
    [SerializeField] PlanManager _planManager;
    [SerializeField] List<SideFilter> _filterBySide = new List<SideFilter>();
    [SerializeField] PlanEvent _planEvent;

    [Serializable]
    public class SideFilter
    {
        [HideInInspector] public string side;
        public bool show;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_planEvent != null)
            _planEvent.Updated(gameObject, _filterBySide.ToArray());
    }

    private void OnEnable()
    {
        //if( _planManager != null )
        {
            CameraManager.Side[] sides = (CameraManager.Side[])Enum.GetValues(typeof(CameraManager.Side));
            foreach( CameraManager.Side side in sides )
            {
                bool l_sideFound = false;
                foreach( SideFilter filter in _filterBySide)
                {
                    if (filter.side == CameraManager.SideToText(side))
                    {
                        l_sideFound = true;
                        break;
                    }
                }
                if(!l_sideFound)
                    _filterBySide.Add(new SideFilter() { side = CameraManager.SideToText(side), show = false });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
