using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IFurnitureEventListener
{
    public void OnFurnitureAddedRaised(GameObject p_furniture);
    public void OnFurnitureRemovedRaised(GameObject p_furniture);
}
