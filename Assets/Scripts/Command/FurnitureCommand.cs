using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureCommand : ICommand
{
    private readonly GameObject _go;
    private Vector3 _rotate;
    private Vector3 _up;
    private Vector3 _right;

    public FurnitureCommand(GameObject go, Vector3 rotate, Vector3 up, Vector3 right)
    {
        _rotate = rotate;
        _go = go;
        _up = up;
        _right = right;
    }

    public void Execute()
    {
        //if (!Utility.CheckZeroVec(new Vector3[] { _rotate, _up, _right }))
        {
            _go.GetComponent<Furniture>().UpdateFurniture(_rotate, _up, _right);
        }
    }

    public void Undo()
    {
        //if (!Utility.CheckZeroVec(new Vector3[] { _rotate, _up, _right }))
        {
            _go.GetComponent<Furniture>().UpdateFurniture(-_rotate, -_up, -_right);
        }
    }

    public string String()
    {
        return "";
    }
}
