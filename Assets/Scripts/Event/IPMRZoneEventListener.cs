using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPMRZoneEventListener
{
    public void OnPMRZoneCollisionEventRaised(PMRZone p_zone);

    public void OnPMRZoneRulesActivated(bool p_activation);
}
