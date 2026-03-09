using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPlanEventListener
{
    public void OnPlanFilterUpdated(GameObject p_object, PlanFilter.SideFilter [] p_filters);
}
