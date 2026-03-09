using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomCommand : ICommand
{
    private float _width;
    private float _area;

    private float _wallThickness;
    private float _wallHeight;
    private float _wallHeightProtection;

    public RoomCommand(float width, float area, float wallThickness, float wallHeight, float wallHeigthProtection)
    {
        _width= width;
        _area= area;
        _wallThickness= wallThickness;
        _wallHeight= wallHeight;
        _wallHeightProtection = wallHeigthProtection;
    }

    public void Execute()
    {
        if (!Utility.CheckZero(new float[5] { _width, _area, _wallThickness, _wallHeight, _wallHeightProtection }))
        {
            //Room.CommandRoom(_width, _area, _wallThickness, _wallHeight, _wallHeightProtection);
        }
    }

    public void Undo()
    {
        if (!Utility.CheckZero(new float[5] { _width , _area , _wallThickness , _wallHeight , _wallHeightProtection }))
        {
            //Room.CommandRoom(-_width, -_area, -_wallThickness, -_wallHeight, -_wallHeightProtection);
            
        }
    }

    public string String()
    {
        return "Room Command : " + GetHashCode() + " ( " +  _width + " ; " + _area + " ; " + _wallThickness + " ; " + _wallHeight + " ; " + _wallHeightProtection + " )";
    }
}
