using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IWallEventListener
{
    public void OnWallEventRaised(int p_wallID, bool p_protected);
    public void OnWallDimensionsEventRaised(float p_height, float p_thickness, float p_protectionHeight);
}
