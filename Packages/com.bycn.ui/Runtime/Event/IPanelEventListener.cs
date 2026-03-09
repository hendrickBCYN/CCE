using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPanelEventListener
{
    public void OnPanelEventRaised(bool p_opened, float p_width);
}
