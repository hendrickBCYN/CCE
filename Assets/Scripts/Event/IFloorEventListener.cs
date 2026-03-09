using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IFloorEventListener
{
    public void OnFloorEventRaised(Material p_material);
}
